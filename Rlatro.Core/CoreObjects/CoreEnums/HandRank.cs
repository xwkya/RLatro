namespace Balatro.Core.CoreObjects.CoreEnums
{
    public enum HandRank : byte
    {
        HighCard,
        Pair,
        TwoPair,
        ThreeOfAKind,
        Straight,
        Flush,
        FullHouse,
        FourOfAKind,
        StraightFlush,
        FiveOfAKind,
        FlushHouse,
        FlushFive,
    }

    public static class HandRankExtensions
    {
        private static readonly HashSet<HandRank> PairRanks =
        [
            HandRank.Pair,
            HandRank.TwoPair,
            HandRank.ThreeOfAKind,
            HandRank.FullHouse,
            HandRank.FourOfAKind,
            HandRank.FiveOfAKind,
            HandRank.FlushHouse,
            HandRank.FlushFive
        ];
        
        private static readonly HashSet<HandRank> TwoPairRanks = 
        [
            HandRank.TwoPair,
            HandRank.FullHouse,
            HandRank.FlushHouse,
        ];
        
        private static readonly HashSet<HandRank> ThreeOfAKindRanks = 
        [
            HandRank.ThreeOfAKind,
            HandRank.FullHouse,
            HandRank.FourOfAKind,
            HandRank.FlushHouse,
            HandRank.FiveOfAKind
        ];
        
        private static readonly HashSet<HandRank> FourOfAKindRanks = 
        [
            HandRank.FourOfAKind,
            HandRank.FiveOfAKind,
            HandRank.FlushFive,
        ];
        
        private static readonly HashSet<HandRank> FlushRanks = 
        [
            HandRank.Flush,
            HandRank.StraightFlush,
            HandRank.FlushHouse,
            HandRank.FlushFive,
        ];
        
        public static bool Contains(this HandRank rank, HandRank containedRank)
        {
            if (containedRank == HandRank.Pair)
            {
                return PairRanks.Contains(rank);
            }
            if (containedRank == HandRank.TwoPair)
            {
                return TwoPairRanks.Contains(rank);
            }
            if (containedRank == HandRank.ThreeOfAKind)
            {
                return ThreeOfAKindRanks.Contains(rank);
            }
            if (containedRank == HandRank.FourOfAKind)
            {
                return FourOfAKindRanks.Contains(rank);
            }
            if (containedRank == HandRank.Flush)
            {
                return FlushRanks.Contains(rank);
            }

            return false;
        }
    }
}