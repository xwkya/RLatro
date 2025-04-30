using Balatro.Core.GameEngine.GameStateController.PhaseActions;
using Balatro.Core.GameEngine.StateController;

namespace Balatro.Core.GameEngine.GameStateController.PhaseStates
{
    public class ArcanaPackState : IGamePhaseState
    {
        public GamePhase Phase => GamePhase.ArcanaPack;
        public byte ArcanaPackSize;
        public byte NumberOfChoices;
        public bool IsPhaseOver { get; private set; }
        public bool HandleAction(GameContext context, BasePlayerAction action)
        {
            throw new NotImplementedException();
        }
    }
}