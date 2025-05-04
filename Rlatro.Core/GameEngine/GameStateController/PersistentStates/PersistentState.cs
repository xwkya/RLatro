using Balatro.Core.CoreObjects.CoreEnums;
using Balatro.Core.CoreRules.Scoring;

namespace Balatro.Core.GameEngine.GameStateController.PersistentStates
{
    public class PersistentState
    {
        public uint Gold { get; set; }
        public int Discards { get; set; }
        public int Hands { get; set; }
        public int HandSize { get; set; }

        // TODO: Implement the logic for these properties
        public ScoreContext GetHandScore(HandRank rank) => new ScoreContext()
        {
            HandRank = rank,
            Chips = 10,
            MultNumerator = 2,
            MultDenominator = 1
        };
    }
}