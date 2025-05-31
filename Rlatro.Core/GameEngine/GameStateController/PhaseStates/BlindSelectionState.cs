using Balatro.Core.CoreObjects.BoosterPacks;
using Balatro.Core.CoreObjects.Tags;
using Balatro.Core.GameEngine.Contracts;
using Balatro.Core.GameEngine.GameStateController.PhaseActions;
using Balatro.Core.GameEngine.GameStateController.PhaseActions.ActionIntents;
using Balatro.Core.GameEngine.PseudoRng;
using Balatro.Core.GameEngine.StateController;

namespace Balatro.Core.GameEngine.GameStateController.PhaseStates
{
    public class BlindSelectionState : BaseGamePhaseState
    {
        public BlindSelectionState(GameContext ctx) : base(ctx) { }

        public override GamePhase Phase => GamePhase.BlindSelection;
        public override bool ShouldInitializeNextState => true;
        public TagEffect[] AnteTags = new TagEffect[2];
        private BoosterPackType? PackToOpen { get; set; }
        private int PackCount { get; set; }

        protected override bool HandleStateSpecificAction(BasePlayerAction action)
        {
            if (action is not BlindSelectionAction blindSelectionAction)
            {
                throw new ArgumentException(
                    "Invalid action type for BlindSelectionState: " + action.GetType().Name);
            }
            
            // We can skip every blind except the boss blind
            if (blindSelectionAction.Intent == BlindSelectionActionIntent.Skip &&
                GameContext.PersistentState.Round % 3 != 0)
            {
                GameContext.TagHandler.GetTag(AnteTags[GameContext.PersistentState.Round % 2 - 1], GameContext);
                GameContext.PersistentState.Round++;
                return PackToOpen > 0;
            }
            
            // The play action was selected, move to the round state
            return true;
        }

        protected override void ResetPhaseSpecificState()
        {
            PackToOpen = null;
            PackCount = 0;
            GenerateAnteTags();
        }

        public override IGamePhaseState GetNextPhaseState()
        {
            if (PackToOpen is null || PackCount <= 0)
            {
                return GameContext.GetPhase<RoundState>();
            }
            
            PackCount--;
            
            if (PackToOpen == BoosterPackType.ArcanaMega)
            {
                var arcanaPackState = GameContext.GetPhase<ArcanaPackState>();

                arcanaPackState.SetIncomingState(this)
                    .SetPackType(PackToOpen.Value);

                return arcanaPackState;
            }
            
            if (PackToOpen == BoosterPackType.BuffoonMega)
            {
                var jokerPackState = GameContext.GetPhase<JokerPackState>();

                jokerPackState.SetIncomingState(this)
                    .SetPackType(PackToOpen.Value);

                return jokerPackState;
            }

            if (PackToOpen == BoosterPackType.CelestialMega)
            {
                var planetPackState = GameContext.GetPhase<PlanetPackState>();

                planetPackState.SetIncomingState(this)
                    .SetPackType(PackToOpen.Value);

                return planetPackState;
            }
            
            if (PackToOpen == BoosterPackType.SpectralNormal)
            {
                var spectralPackState = GameContext.GetPhase<SpectralPackState>();

                spectralPackState.SetIncomingState(this)
                    .SetPackType(PackToOpen.Value);

                return spectralPackState;
            }

            if (PackToOpen == BoosterPackType.StandardMega)
            {
                var cardPackState = GameContext.GetPhase<CardPackState>();

                cardPackState.SetIncomingState(this)
                    .SetPackType(PackToOpen.Value);

                return cardPackState;
            }

            throw new ApplicationException("Unhandled pack type: " + PackToOpen);
        }

        public void GenerateAnteTags()
        {
            for (int i = 0; i < 2; i++)
            {
                var numberOfTags = Enum.GetValues<TagEffect>().Length;
                AnteTags[i] = (TagEffect)GameContext.RngController.RandomInt(
                    0, numberOfTags, RngActionType.GetNewAnteSkipTags,
                    suffix: GameContext.PersistentState.Ante.ToString());
            }
        }

        public void RegisterPackOpening(BoosterPackType type, int count)
        {
            PackToOpen = type;
            PackCount = count;
        }
    }
}