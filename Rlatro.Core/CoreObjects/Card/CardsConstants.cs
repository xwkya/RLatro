namespace Balatro.Core.CoreObjects.Card
{
    public static class CardsConstants
    {
        public static uint GetRankChips(this Rank rank)
        {
            return rank switch
            {
                Rank.Two => 2,
                Rank.Three => 3,
                Rank.Four => 4,
                Rank.Five => 5,
                Rank.Six => 6,
                Rank.Seven => 7,
                Rank.Eight => 8,
                Rank.Nine => 9,
                Rank.Ten => 10,
                Rank.Jack => 10,
                Rank.Queen => 10,
                Rank.King => 10,
                Rank.Ace => 11,
                _ => throw new ArgumentOutOfRangeException(nameof(rank), rank, null)
            };
        }
        
        public static bool IsNaturalFaceCard(this Rank rank)
        {
            return (int)rank >= (int)Rank.Jack && (int)rank <= (int)Rank.King;
        }
    }
}