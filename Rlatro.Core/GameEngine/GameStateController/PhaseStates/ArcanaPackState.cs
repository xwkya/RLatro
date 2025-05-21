using Balatro.Core.CoreObjects.BoosterPacks;
using Balatro.Core.CoreObjects.Consumables.ConsumableObject;
using Balatro.Core.GameEngine.GameStateController.PhaseActions;
using Balatro.Core.GameEngine.GameStateController.PhaseActions.ActionIntents;
using Balatro.Core.GameEngine.PseudoRng;
using Balatro.Core.GameEngine.StateController;

namespace Balatro.Core.GameEngine.GameStateController.PhaseStates
{
    public class ArcanaPackState : IGamePhaseState
    {
        private IGamePhaseState IncomingState { get; }
        private int ArcanaPackSize { get; init; }
        private int NumberOfChoices { get; set; }
        private List<Consumable> ArcanaCards { get; } = new();
        private GameContext GameContext { get; }
        
        public ArcanaPackState(GameContext ctx, IGamePhaseState incomingState, BoosterPackType type)
        {
            GameContext = ctx;
            IncomingState = incomingState;
            
            var t = type.GetPackSizeAndChoice();
            ArcanaPackSize = t.size;
            NumberOfChoices = t.choice;
            
            FillCardsChoice();
            FillHand();
        }
        
        public GamePhase Phase => GamePhase.ArcanaPack;
        
        public bool HandleAction(BasePlayerAction action)
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
            GameContext.GameEventBus.PublishConsumableRemovedFromContext(consumable.StaticId);
            ArcanaCards.RemoveAt(packAction.CardIndex);

            NumberOfChoices--;
            return NumberOfChoices == 0;
        }

        public IGamePhaseState GetNextPhaseState()
        {
            return IncomingState;
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
            for (var i = 0; i < ArcanaPackSize; i++)
            {
                var card = GameContext.GlobalPoolManager.GenerateTarotCard(RngActionType.RandomPackConsumable);
                ArcanaCards.Add(card);
            }
        }
    }
}