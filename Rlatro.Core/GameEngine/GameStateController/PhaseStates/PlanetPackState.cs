using Balatro.Core.GameEngine.GameStateController.PhaseActions;
using Balatro.Core.GameEngine.StateController;

namespace Balatro.Core.GameEngine.GameStateController.PhaseStates
{
    public class PlanetPackState : IGamePhaseState
    {
        private ShopState IncomingState { get; }
        
        public PlanetPackState(ShopState incomingState)
        {
            IncomingState = incomingState;
        }
        
        public GamePhase Phase { get; }
        
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