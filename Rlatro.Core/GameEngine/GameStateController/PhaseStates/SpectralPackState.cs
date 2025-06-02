using Balatro.Core.CoreObjects.Consumables.ConsumableObject;
using Balatro.Core.GameEngine.GameStateController.PhaseActions;
using Balatro.Core.GameEngine.GameStateController.PhaseActions.ActionIntents;
using Balatro.Core.GameEngine.PseudoRng;
using Balatro.Core.GameEngine.StateController;

namespace Balatro.Core.GameEngine.GameStateController.PhaseStates
{
    public class SpectralPackState : BasePackState<SpectralPackState>
    {
        private List<Consumable> SpectralCards { get; set; } = new();
        
        public SpectralPackState(GameContext ctx) : base(ctx)
        {
        }
        
        public List<Consumable> GetSpectralCards()
        {
            return SpectralCards;
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
            GameContext.GameEventBus.PublishConsumableUsed(consumable.StaticId);
            GameContext.GameEventBus.PublishConsumableRemovedFromContext(consumable.StaticId);
            SpectralCards.RemoveAt(packAction.CardIndex);

            NumberOfChoices--;
            return NumberOfChoices == 0;
        }
        
        public override void OnEnterPhase()
        {
            FillCardChoices();
            FillHand();
        }

        public override void OnExitPhase()
        {
            GameContext.Hand.MoveAllTo(GameContext.Deck);
            GameContext.Deck.Shuffle(GameContext.RngController);
            ClearCardChoices();
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
            for (var i = 0; i < PackSize; i++)
            {
                // Use pack generation logic which can include The Soul (0.3% chance)
                var card = GameContext.GlobalPoolManager.GeneratePackConsumable(RngActionType.RandomPackConsumable, ConsumableType.Spectral);
                SpectralCards.Add(card);
            }
        }

        private void ClearCardChoices()
        {
            foreach (var t in SpectralCards)
            {
                GameContext.GameEventBus.PublishConsumableRemovedFromContext(t.StaticId);
            }

            SpectralCards.Clear();
        }
    }
}