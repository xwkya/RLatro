using Balatro.Core.CoreObjects.CoreEnums;
using Balatro.Core.CoreRules.CanonicalViews;

namespace Balatro.Core.CoreRules.Scoring
{
    public static class HandRankGetter
    {
        private static void Clear(Span<byte> buf) => buf.Fill(0);

        public static bool TryFlush(ReadOnlySpan<CardView> cards,
            bool fourFingers,
            out SuitMask suitPicked,
            Span<byte> mark)
        {
            int need = fourFingers ? 4 : 5;
            if (cards.Length < need)
            {
                suitPicked = SuitMask.None;
                return false;
            }
            
            Span<byte> suitCnt = stackalloc byte[4]; // S H C D

            // pass 1 … count “how many cards could be spades, hearts…”
            for (int i = 0; i < cards.Length; i++)
            {
                byte s = (byte)cards[i].Suits;
                if ((s & (byte)SuitMask.Spade) != 0) suitCnt[0]++;
                if ((s & (byte)SuitMask.Heart) != 0) suitCnt[1]++;
                if ((s & (byte)SuitMask.Club) != 0) suitCnt[2]++;
                if ((s & (byte)SuitMask.Diamond) != 0) suitCnt[3]++;
            }
            
            int best = -1;
            for (int i = 0; i < 4; ++i)
                if (suitCnt[i] >= need)
                {
                    best = i;
                }

            suitPicked = best switch
            {
                0 => SuitMask.Spade,
                1 => SuitMask.Heart,
                2 => SuitMask.Club,
                3 => SuitMask.Diamond,
                _ => SuitMask.None
            };

            if (suitPicked == 0) return false;

            // pass 2 … mark the cards that share that suit
            Clear(mark);
            for (int i = 0; i < cards.Length; ++i)
                if ((cards[i].Suits & suitPicked) != 0)
                    mark[i] = 1;

            return true;
        }

        public static bool TryStraight(ReadOnlySpan<CardView> cards,
            bool fourFingers,
            bool shortcut,
            out int lowRank, // 0 = A-5 low …  9 = 10-A
            Span<byte> mark)
        {
            int need = fourFingers ? 4 : 5;
            if (cards.Length < need)
            {
                lowRank = -1;
                return false;
            }   
            
            uint ranks = 0; // 14 Presence bits (1-Ace low, 2-2, .., 14-Ace high)
            for (int i = 0; i < cards.Length; ++i)
            {
                int r = (int)cards[i].Rank + 1; // 1-13
                ranks |= (uint)1 << r;
                if (r == 13) ranks |= 1; // Ace low duplicate
            }

            int maxGap = shortcut ? 2 : 1;
            int bestFoundCount = 0;
            lowRank = -1;
            int highRank = -1;

            // try every possible start rank  (0 .. 10)
            for (int s = 10; s >= 0; --s)
            {
                if ((ranks & (1 << s)) == 0) continue; // If no card is at s, skip
                int found = 1;
                int last = s;
                
                // Check if we have a straight starting from s
                for (int r = s + 1; r <= 14 && found < 5; ++r)
                {
                    bool foundCard = (ranks & (1 << r)) != 0;
                    
                    if (!foundCard) 
                    {
                        continue;
                    }
                    
                    var gap = r - last;
                    if (gap <= maxGap)
                    {
                        last = r;
                        found++;
                        continue;
                    }

                    break;
                }

                if (found >= need && found >= bestFoundCount)
                {
                    bestFoundCount = found;
                    lowRank = s;
                    highRank = last;
                }
            }

            if (bestFoundCount < need) return false;

            // mark: any card whose rank lies between lowRank and highRank
            Clear(mark);

            for (int i = 0; i < cards.Length; ++i)
            {
                var r = (int)cards[i].Rank + 1; // 1-13
                if (r >= lowRank && r <= highRank)
                    mark[i] = 1;
                
                // Handle Ace low case
                if (lowRank == 0 && r == 13)
                {
                    mark[i] = 1;
                }
            }

            return true;
        }

        public static HandRank GetRank(bool fourFingers,
            bool shortcut,
            ReadOnlySpan<CardView> hand,
            Span<byte> outIdx)
        {
            // cache-local counts of each rank for kinds & pairs detection
            Span<byte> cnt = stackalloc byte[13];
            foreach (ref readonly var c in hand) cnt[(byte)c.Rank]++;

            // Detect straights and flushes
            Span<byte> straightIdx = stackalloc byte[hand.Length];
            Span<byte> flushIdx = stackalloc byte[hand.Length];

            bool hasStraight = TryStraight(hand, fourFingers, shortcut, out int low, straightIdx);
            bool hasFlush = TryFlush(hand, fourFingers, out SuitMask flushSuit, flushIdx);

            // 1. “combined condition” hands
            if (hasStraight && hasFlush)
            {
                Clear(outIdx);
                for (int i = 0; i < hand.Length; ++i)
                    if (straightIdx[i] == 1 || flushIdx[i] == 1)
                        outIdx[i] = 1; // Work around since we can't use a bitwise OR on bytes

                return HandRank.StraightFlush;
            }

            // Flush Five / Flush House
            bool fiveKind = false;
            int fiveRank = -1;
            bool fourKind = false;
            int fourRank = -1;
            bool trips = false;
            int tripRank = -1;
            int pairs = 0, pair1 = -1, pair2 = -1;
            for (int r = 0; r < 13; ++r)
            {
                switch (cnt[r])
                {
                    case 5:
                        fiveKind = true;
                        fiveRank = r;
                        break;
                    case 4:
                        fourKind = true;
                        fourRank = r;
                        break;
                    case 3:
                        trips = true;
                        tripRank = r;
                        break;
                    case 2:
                        if (pairs == 0) pair1 = r;
                        else pair2 = r;
                        ++pairs;
                        break;
                }
            }

            if (fiveKind && hasFlush) // Flush Five
            {
                Clear(outIdx);
                for (int i = 0; i < hand.Length; ++i) outIdx[i] = 1; // all cards score
                return HandRank.FlushFive;
            }

            if (trips && pairs > 0 && hasFlush) // Flush House
            {
                Clear(outIdx);
                for (int i = 0; i < hand.Length; ++i) outIdx[i] = 1;
                return HandRank.FlushHouse;
            }

            // 2. regular hierarchy (longest to weakest)

            if (fiveKind)
            {
                MarkSame(hand, fiveRank, outIdx);
                return HandRank.FiveOfAKind;
            }

            if (fourKind)
            {
                MarkSame(hand, fourRank, outIdx);
                return HandRank.FourOfAKind;
            }

            if (trips && pairs > 0)
            {
                MarkFullHouse(hand, tripRank, pair1, outIdx);
                return HandRank.FullHouse;
            }

            if (hasFlush)
            {
                flushIdx.CopyTo(outIdx);
                return HandRank.Flush;
            }

            if (hasStraight)
            {
                straightIdx.CopyTo(outIdx);
                return HandRank.Straight;
            }

            if (trips)
            {
                MarkSame(hand, tripRank, outIdx);
                return HandRank.ThreeOfAKind;
            }

            if (pairs >= 2)
            {
                MarkTwoPair(hand, pair1, pair2, outIdx);
                return HandRank.TwoPair;
            }

            if (pairs == 1)
            {
                MarkSame(hand, pair1, outIdx);
                return HandRank.Pair;
            }

            // High card
            int best = 0;
            for (int i = 1; i < hand.Length; ++i)
                if (hand[i].Rank > hand[best].Rank)
                    best = i;
            Clear(outIdx);
            outIdx[best] = 1;
            return HandRank.HighCard;
        }

        // Marks *all* cards whose Rank == targetRank
        private static void MarkSame(ReadOnlySpan<CardView> hand,
            int targetRank,
            Span<byte> dst)
        {
            Clear(dst);
            for (int i = 0; i < hand.Length; ++i)
                if ((int)hand[i].Rank == targetRank)
                    dst[i] = 1;
        }

        // Marks the first 3 cards of rankA  +  the first 2 cards of rankB
        private static void MarkFullHouse(ReadOnlySpan<CardView> hand,
            int rankA, // three-of-a-kind
            int rankB, // pair
            Span<byte> dst)
        {
            Clear(dst);
            int needA = 3, needB = 2;
            for (int i = 0; i < hand.Length && (needA > 0 || needB > 0); ++i)
            {
                int r = (int)hand[i].Rank;
                if (r == rankA && needA-- > 0) dst[i] = 1;
                else if (r == rankB && needB-- > 0) dst[i] = 1;
            }
        }

        // Marks the first 2 cards of each pair-rank
        private static void MarkTwoPair(ReadOnlySpan<CardView> hand,
            int rank1,
            int rank2,
            Span<byte> dst)
        {
            Clear(dst);
            int need1 = 2, need2 = 2;
            for (int i = 0; i < hand.Length && (need1 > 0 || need2 > 0); ++i)
            {
                int r = (int)hand[i].Rank;
                if (r == rank1 && need1-- > 0) dst[i] = 1;
                else if (r == rank2 && need2-- > 0) dst[i] = 1;
            }
        }
    }
}