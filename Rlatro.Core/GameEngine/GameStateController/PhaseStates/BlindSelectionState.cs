using Balatro.Core.GameEngine.Contracts;
using Balatro.Core.GameEngine.GameStateController.PhaseActions;
using Balatro.Core.GameEngine.StateController;

namespace Balatro.Core.GameEngine.GameStateController.PhaseStates
{
    public class BlindSelectionState : BaseGamePhaseState
    {
        public BlindSelectionState(GameContext ctx) : base(ctx) { }

        public override GamePhase Phase => GamePhase.BlindSelection;

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