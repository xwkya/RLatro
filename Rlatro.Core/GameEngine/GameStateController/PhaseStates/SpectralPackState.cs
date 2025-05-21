using Balatro.Core.GameEngine.GameStateController.PhaseActions;
using Balatro.Core.GameEngine.StateController;

namespace Balatro.Core.GameEngine.GameStateController.PhaseStates
{
    public class SpectralPackState : IGamePhaseState
    {
        private ShopState IncomingState { get; }
        
        public SpectralPackState(ShopState incomingState)
        {
            IncomingState = incomingState;
        }

        public GamePhase Phase => GamePhase.SpectralPack;
        
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