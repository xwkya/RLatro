using Balatro.Core.GameEngine.PseudoRng;

namespace Balatro.Core.CoreObjects.CoreEnums
{
    public enum Rank : byte
    {
        Two,
        Three,
        Four,
        Five,
        Six,
        Seven,
        Eight,
        Nine,
        Ten,
        Jack,
        Queen,
        King,
        Ace,
    }
    
    public static class RankExtensions
    {
        private static readonly Rank[] FaceRanks =
        {
            Rank.Jack,
            Rank.Queen,
            Rank.King,
        };
        
        private static readonly Rank[] NumberedRanks =
        {
            Rank.Two,
            Rank.Three,
            Rank.Four,
            Rank.Five,
            Rank.Six,
            Rank.Seven,
            Rank.Eight,
            Rank.Nine,
            Rank.Ten,
        };
        
        public static Rank RandomFaceRank(RngController rng, RngActionType actionType)
        {
            var randomIndex = rng.RandomInt(0, FaceRanks.Length - 1, actionType);
            return FaceRanks[randomIndex];
        }
        
        public static Rank RandomNumberedRank(RngController rng, RngActionType actionType)
        {
            var randomIndex = rng.RandomInt(0, NumberedRanks.Length - 1, actionType);
            return NumberedRanks[randomIndex];
        }
    }
}