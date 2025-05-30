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
        
        public override IGamePhaseState GetNextPhaseState()
        {
            if (PackToOpen is null || PackCount <= 0)
            {
                return GameContext.GamePhaseStates[typeof(RoundState)];
            }
            
            
            if (PackToOpen == BoosterPackType.ArcanaMega)
            {
                ArcanaPackState arcanaPackState;
                if (GameContext.GamePhaseStates.ContainsKey(typeof(ArcanaPackState)))
                {
                    arcanaPackState = (ArcanaPackState)GameContext.GamePhaseStates[typeof(ArcanaPackState)];
                }
                else
                {
                    arcanaPackState = new ArcanaPackState(GameContext);
                    GameContext.GamePhaseStates[typeof(ArcanaPackState)] = arcanaPackState;
                }

                arcanaPackState.SetIncomingState(this)
                    .SetPackType(PackToOpen.Value);

                return arcanaPackState;
            }
            
            if (PackToOpen == BoosterPackType.BuffoonMega)
            {
                JokerPackState jokerPackState;
                if (GameContext.GamePhaseStates.ContainsKey(typeof(JokerPackState)))
                {
                    jokerPackState = (JokerPackState)GameContext.GamePhaseStates[typeof(JokerPackState)];
                }
                else
                {
                    jokerPackState = new JokerPackState(GameContext);
                    GameContext.GamePhaseStates[typeof(JokerPackState)] = jokerPackState;
                }

                jokerPackState.SetIncomingState(this)
                    .SetPackType(PackToOpen.Value);

                return jokerPackState;
            }

            if (PackToOpen == BoosterPackType.CelestialMega)
            {
                PlanetPackState planetPackState;
                if (GameContext.GamePhaseStates.ContainsKey(typeof(PlanetPackState)))
                {
                    planetPackState = (PlanetPackState)GameContext.GamePhaseStates[typeof(PlanetPackState)];
                }
                else
                {
                    planetPackState = new PlanetPackState(GameContext);
                    GameContext.GamePhaseStates[typeof(PlanetPackState)] = planetPackState;
                }

                planetPackState.SetIncomingState(this)
                    .SetPackType(PackToOpen.Value);

                return planetPackState;
            }
            
            if (PackToOpen == BoosterPackType.SpectralNormal)
            {
                SpectralPackState spectralPackState;
                if (GameContext.GamePhaseStates.ContainsKey(typeof(SpectralPackState)))
                {
                    spectralPackState = (SpectralPackState)GameContext.GamePhaseStates[typeof(SpectralPackState)];
                }
                else
                {
                    spectralPackState = new SpectralPackState(GameContext);
                    GameContext.GamePhaseStates[typeof(SpectralPackState)] = spectralPackState;
                }

                spectralPackState.SetIncomingState(this)
                    .SetPackType(PackToOpen.Value);

                return spectralPackState;
            }

            if (PackToOpen == BoosterPackType.StandardMega)
            {
                CardPackState cardPackState;
                if (GameContext.GamePhaseStates.ContainsKey(typeof(CardPackState)))
                {
                    cardPackState = (CardPackState)GameContext.GamePhaseStates[typeof(CardPackState)];
                }
                else
                {
                    cardPackState = new CardPackState(GameContext);
                    GameContext.GamePhaseStates[typeof(CardPackState)] = cardPackState;
                }

                cardPackState.SetIncomingState(this)
                    .SetPackType(PackToOpen.Value);

                return cardPackState;
            }

            throw new ApplicationException("Unhandled pack type: " + PackToOpen);
        }

        public void GenerateAnteTags()
        {
            for (int i = 0; i < 3; i++)
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