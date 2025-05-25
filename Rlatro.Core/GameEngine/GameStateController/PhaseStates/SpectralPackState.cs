using Balatro.Core.CoreObjects.BoosterPacks;
using Balatro.Core.CoreObjects.Consumables.ConsumableObject;
using Balatro.Core.GameEngine.Contracts;
using Balatro.Core.GameEngine.GameStateController.PhaseActions;
using Balatro.Core.GameEngine.GameStateController.PhaseActions.ActionIntents;
using Balatro.Core.GameEngine.PseudoRng;
using Balatro.Core.GameEngine.StateController;

namespace Balatro.Core.GameEngine.GameStateController.PhaseStates
{
    public class SpectralPackState : BaseGamePhaseState
    {
        private ShopState IncomingState { get; }
        private int NumberOfChoices { get; set; }
        private int NumberOfCards { get; set; }
        private List<Consumable> SpectralCards { get; set; } = new();
        
        public SpectralPackState(GameContext ctx, ShopState incomingState, BoosterPackType packType) : base(ctx)
        {
            IncomingState = incomingState;
            var sizeAndChoices = packType.GetPackSizeAndChoice();
            NumberOfCards = sizeAndChoices.size;
            NumberOfChoices = sizeAndChoices.choice;
            FillCardChoices();
            FillHand();
        }

        public override GamePhase Phase => GamePhase.SpectralPack;
        

        protected override bool HandleStateSpecificAction(BasePlayerAction action)
        {
            if (action is not PackActionWithTargets packAction)
            {
                throw new ArgumentException($"Action {action} is not a {nameof(PackActionWithTargets)}.");
            }
            
            ValidatePossibleAction(packAction);
            
            // When skipping, nothing to handle for now
            if (packAction.Intent == PackActionIntent.SkipPack)
            {
                return true;
            }
            
            var consumable = SpectralCards[packAction.CardIndex];
            consumable.Apply(GameContext, packAction.TargetIndices);
            
            // Remove and cleanup
            GameContext.GameEventBus.PublishConsumableRemovedFromContext(consumable.StaticId);
            SpectralCards.RemoveAt(packAction.CardIndex);

            NumberOfChoices--;
            return NumberOfChoices == 0;
        }

        public override IGamePhaseState GetNextPhaseState()
        {
            return IncomingState;
        }

        public override void OnExitPhase()
        {
            GameContext.Hand.MoveAllTo(GameContext.Deck);
            GameContext.Deck.Shuffle(GameContext.RngController);
        }

        private void ValidatePossibleAction(PackActionWithTargets packAction)
        {
            if (packAction.Intent == PackActionIntent.GetCard && packAction.CardIndex >= SpectralCards.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(packAction.CardIndex), packAction.CardIndex, $"Card index {packAction.CardIndex} is out of range for Arcana Pack size {SpectralCards.Count}.");
            }

            if (packAction.Intent == PackActionIntent.GetCard &&
                !SpectralCards[packAction.CardIndex].IsUsable(GameContext, packAction.TargetIndices))
            {
                throw new ArgumentException(
                    $"{SpectralCards[packAction.CardIndex].GetType().Name} is not usable in the current pack context");
            }
        }
        
        private void FillHand()
        {
            var cardsToDraw = GameContext.GetHandSize();
            GameContext.Deck.DrawTopTo(cardsToDraw, GameContext.Hand);
        }

        private void FillCardChoices()
        {
            for (var i = 0; i < NumberOfCards; i++)
            {
                var card = GameContext.GlobalPoolManager.GenerateConsumable(RngActionType.RandomPackConsumable, ConsumableType.Spectral);
                SpectralCards.Add(card);
            }
        }
    }
}