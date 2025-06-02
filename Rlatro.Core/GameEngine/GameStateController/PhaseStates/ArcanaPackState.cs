using Balatro.Core.CoreObjects.Consumables.ConsumableObject;
using Balatro.Core.GameEngine.GameStateController.PhaseActions;
using Balatro.Core.GameEngine.GameStateController.PhaseActions.ActionIntents;
using Balatro.Core.GameEngine.PseudoRng;
using Balatro.Core.GameEngine.StateController;

namespace Balatro.Core.GameEngine.GameStateController.PhaseStates
{
    public class ArcanaPackState : BasePackState<ArcanaPackState>
    {
        private List<Consumable> ArcanaCards { get; } = new();
        
        public ArcanaPackState(GameContext ctx) : base(ctx) { }
        
        public List<Consumable> GetArcanaCards()
        {
            return ArcanaCards;
        }
        
        public override GamePhase Phase => GamePhase.ArcanaPack;

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
            
            var consumable = ArcanaCards[packAction.CardIndex];
            consumable.Apply(GameContext, packAction.TargetIndices);
            
            // Remove and cleanup
            GameContext.GameEventBus.PublishConsumableUsed(consumable.StaticId);
            GameContext.GameEventBus.PublishConsumableRemovedFromContext(consumable.StaticId);
            ArcanaCards.RemoveAt(packAction.CardIndex);

            NumberOfChoices--;
            return NumberOfChoices == 0;
        }
        
        public override void OnExitPhase()
        {
            GameContext.Hand.MoveAllTo(GameContext.Deck);
            GameContext.Deck.Shuffle(GameContext.RngController);
            ClearCardChoice();
        }

        public override void OnEnterPhase()
        {
            FillCardsChoice();
            FillHand();
        }

        private void ValidatePossibleAction(PackActionWithTargets packAction)
        {
            if (packAction.Intent == PackActionIntent.GetCard && packAction.CardIndex >= ArcanaCards.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(packAction.CardIndex), packAction.CardIndex, $"Card index {packAction.CardIndex} is out of range for Arcana Pack size {ArcanaCards.Count}.");
            }

            if (packAction.Intent == PackActionIntent.GetCard &&
                !ArcanaCards[packAction.CardIndex].IsUsable(GameContext, packAction.TargetIndices))
            {
                throw new ArgumentException(
                    $"{ArcanaCards[packAction.CardIndex].GetType().Name} is not usable in the current pack context");
            }
        }

        private void FillHand()
        {
            var cardsToDraw = GameContext.GetHandSize();
            GameContext.Deck.DrawTopTo(cardsToDraw, GameContext.Hand);
        }

        private void FillCardsChoice()
        {
            for (var i = 0; i < PackSize; i++)
            {
                var card = GameContext.GlobalPoolManager.GeneratePackConsumable(RngActionType.RandomPackConsumable, ConsumableType.Tarot);
                ArcanaCards.Add(card);
            }
        }
        
        private void ClearCardChoice()
        {
            foreach (var card in ArcanaCards)
            {
                GameContext.GameEventBus.PublishConsumableRemovedFromContext(card.StaticId);
            }
            ArcanaCards.Clear();
        }
    }
}