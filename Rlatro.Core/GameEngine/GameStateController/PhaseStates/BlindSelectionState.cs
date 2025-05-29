using Balatro.Core.CoreObjects.BoosterPacks;
using Balatro.Core.GameEngine.Contracts;
using Balatro.Core.GameEngine.GameStateController.PhaseActions;
using Balatro.Core.GameEngine.StateController;

namespace Balatro.Core.GameEngine.GameStateController.PhaseStates
{
    public class BlindSelectionState : BaseGamePhaseState
    {
        public BlindSelectionState(GameContext ctx) : base(ctx) { }

        public override GamePhase Phase => GamePhase.BlindSelection;
        public override bool ShouldInitializeNextState => true;
        private BoosterPackType? PackToOpen { get; set; }
        private int PackCount { get; set; }

        protected override bool HandleStateSpecificAction(BasePlayerAction action)
        {
            throw new NotImplementedException();
        }

        public override IGamePhaseState GetNextPhaseState()
        {
            throw new NotImplementedException();
        }

        public void RegisterPackOpening(BoosterPackType type, int count)
        {
            PackToOpen = type;
            PackCount = count;
        }
    }
}