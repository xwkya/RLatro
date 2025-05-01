using Balatro.Core.CoreObjects.CoreEnums;
using Balatro.Core.CoreRules.CanonicalViews;

namespace Balatro.Core.CoreRules.Scoring
{
    public static class HandRankGetter
    {
        // Impl 2
        private static void Clear(Span<byte> buf) => buf.Fill(0);

        private static bool TryFlush(ReadOnlySpan<CardView> cards,
            bool fourFingers,
            out SuitMask suitPicked,
            Span<byte> mark)
        {
            Span<byte> suitCnt = stackalloc byte[4]; // S H C D

            // pass 1 … count “how many cards could be spades, hearts…”
            for (int i = 0; i < cards.Length; ++i)
            {
                byte s = (byte)cards[i].Suits;
                if ((s & (byte)SuitMask.Spade) != 0) suitCnt[0]++;
                if ((s & (byte)SuitMask.Spade) != 0) suitCnt[1]++;
                if ((s & (byte)SuitMask.Spade) != 0) suitCnt[2]++;
                if ((s & (byte)SuitMask.Spade) != 0) suitCnt[3]++;
            }

            int need = fourFingers ? 4 : 5;
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

        private static bool TryStraight(ReadOnlySpan<CardView> cards,
            bool fourFingers,
            bool shortcut,
            out int lowRank, // 0 = A low …  9 = 10-A
            Span<byte> mark)
        {
            Span<ushort> rankMaskPerIdx = stackalloc ushort[cards.Length];
            ushort ranks = 0; // presence bits (Ace low = bit 0, Ace high = bit 13)
            for (int i = 0; i < cards.Length; ++i)
            {
                int r = (int)cards[i].Rank; // 0-12
                ranks |= (ushort)(1 << r);
                if (r == 12) ranks |= 1; // Ace low duplicate
                rankMaskPerIdx[i] = (ushort)(1 << r);
            }

            int need = fourFingers ? 4 : 5;
            lowRank = -1;

            // try every possible start rank  (0 .. 9)
            for (int s = 0; s <= 9; ++s)
            {
                int found = 0, gaps = 0, last = -5;
                for (int r = s; r < 13 && found < need; ++r)
                {
                    if ((ranks & (1 << r)) != 0)
                    {
                        // rank exists
                        if (last != -5) gaps += r - last - 1;
                        last = r;
                        found++;
                    }
                }

                if (found >= need &&
                    (!shortcut ? gaps == 0 : gaps <= (need - 1))) // gap rule
                {
                    lowRank = s;
                    break;
                }
            }

            if (lowRank < 0) return false;

            // mark: any card whose rank lies in [lowRank, … lowRank + 2*(need-1)]
            Clear(mark);
            int highLimit = lowRank + 2 * (need - 1);
            for (int i = 0; i < cards.Length; ++i)
            {
                int r = (int)cards[i].Rank;
                if (r == 12 && lowRank == 0) r = -1; // treat Ace-low as rank -1 so A-2-3-4 (+gap) works
                if (r >= lowRank && r <= highLimit)
                    mark[i] = 1;
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
                        outIdx[i] = 1; // union, e.g. 4C in your example

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
        
        // Impl 1 -> TODO: DISCARD ONCE IMPL2 is tested

        // pattern “0001 0001 0001 0001 0001” for 5 cards flush
        private const int P_SPADES = 0b0001_0001_0001_0001_0001;
        private const int P_HEARTS = P_SPADES << 1;
        private const int P_CLUBS = P_SPADES << 2;
        private const int P_DIAMONDS = P_SPADES << 3;

        // pattern "11111" for 5 cards straight
        private const int P_STRAIGHT = 0b11111;

        /// <summary>
        /// Checks for a standard poker flush in the hand.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static bool IsRegularFlush(ReadOnlySpan<CardView> p)
        {
            if (p.Length < 5) return false;

            // Each suit is a 4-bit mask, and we have 5 cards.
            // Lay out all suits in 4-bit lanes for a total of 
            int packed = 0;
            for (int i = 0; i < p.Length; i++)
                packed |= ((int)p[i].Suits & 0b1111) << (4 * i);

            // flush exists if every card has the same suit bit → AND pattern hit
            return (packed & P_SPADES) == P_SPADES ||
                   (packed & P_HEARTS) == P_HEARTS ||
                   (packed & P_CLUBS) == P_CLUBS ||
                   (packed & P_DIAMONDS) == P_DIAMONDS;
        }

        /// <summary>
        /// Checks for a standard poker straight in the hand.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static bool IsRegularStraight(ReadOnlySpan<CardView> p)
        {
            if (p.Length < 5) return false;

            // Rank is an enum with values going from 0 to 12.
            // We lay out 1 bit for each rank in a 14-bit lane (to account for Ace high and low).
            int packed = 0;
            for (int i = 0; i < p.Length; i++)
                packed |= 1 << ((byte)p[i].Rank + 1);

            // If Ace is set also set the low Ace bit
            packed |= (packed & (0b1 << 13)) >> 13;

            // Check for 5 consecutive bits set in the packed representation.
            for (int i = 0; i < 10; i++)
            {
                if ((packed & (P_STRAIGHT << i)) == (P_STRAIGHT << i))
                    return true;
            }

            return false;
        }
    }
}