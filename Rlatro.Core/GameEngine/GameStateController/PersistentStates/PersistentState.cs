using Balatro.Core.CoreObjects.CoreEnums;
using Balatro.Core.CoreRules.Scoring;

namespace Balatro.Core.GameEngine.GameStateController.PersistentStates
{
    public class PersistentState
    {
        public int Gold { get; set; }
        public int Discards { get; set; }
        public int Hands { get; set; }
        public int HandSize { get; set; }
        public int JokerSlots { get; set; }
        public HandTracker HandTracker { get; set; }

        public Dictionary<HandRank, int> HandLevels()
        {
            return new Dictionary<HandRank, int>()
            {
                { HandRank.FlushFive, 1},
                { HandRank.FlushHouse, 1},
                { HandRank.FiveOfAKind, 1},
                { HandRank.StraightFlush, 1 },
                { HandRank.FourOfAKind, 1 },
                { HandRank.FullHouse, 1 },
                { HandRank.Flush, 1 },
                { HandRank.Straight, 1 },
                { HandRank.ThreeOfAKind, 1 },
                { HandRank.TwoPair, 1 },
                { HandRank.Pair, 1 },
                { HandRank.HighCard, 1 }
            };
        }

        public ScoreContext GetHandScore(HandRank rank)
        {
            return HandTracker.GetHandScore(rank);
        }
    }
}