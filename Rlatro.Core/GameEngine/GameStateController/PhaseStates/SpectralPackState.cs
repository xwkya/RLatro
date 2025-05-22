using Balatro.Core.GameEngine.Contracts;
using Balatro.Core.GameEngine.GameStateController.PhaseActions;
using Balatro.Core.GameEngine.StateController;

namespace Balatro.Core.GameEngine.GameStateController.PhaseStates
{
    public class SpectralPackState : BaseGamePhaseState
    {
        private ShopState IncomingState { get; }
        
        public SpectralPackState(ShopState incomingState, GameContext ctx) : base(ctx)
        {
            IncomingState = incomingState;
        }

        public override GamePhase Phase => GamePhase.SpectralPack;
        

        protected override bool HandleStateSpecificAction(BasePlayerAction action)
        {
            throw new NotImplementedException();
        }

        public override IGamePhaseState GetNextPhaseState()
        {
            throw new NotImplementedException();
        }
    }
}