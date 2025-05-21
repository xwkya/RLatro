using Balatro.Core.GameEngine.GameStateController.PhaseActions;
using Balatro.Core.GameEngine.StateController;

namespace Balatro.Core.GameEngine.GameStateController.PhaseStates
{
    public class BlindSelectionState : IGamePhaseState
    {
        private GameContext GameContext { get; }
        public BlindSelectionState(GameContext ctx)
        {
            GameContext = ctx;
        }

        public GamePhase Phase => GamePhase.BlindSelection;
        
        public bool HandleAction(BasePlayerAction action)
        {
            throw new NotImplementedException();
        }

        public IGamePhaseState GetNextPhaseState()
        {
            throw new NotImplementedException();
        }
    }
}