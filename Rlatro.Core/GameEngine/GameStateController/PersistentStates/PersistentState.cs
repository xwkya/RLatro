using Balatro.Core.CoreObjects.CoreEnums;
using Balatro.Core.CoreRules.Scoring;

namespace Balatro.Core.GameEngine.GameStateController.PersistentStates
{
    public class PersistentState
    {
        public uint Gold { get; set; }
        public byte Discards { get; set; }
        public byte Hands { get; set; }
        public byte HandSize { get; set; }

        // TODO: Implement the logic for these properties
        public ScoreContext GetHandScore(HandRank rank) => new ScoreContext()
        {
            HandRank = rank,
            Chips = 10,
            Mult = 2,
        };
    }
}