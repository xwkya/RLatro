using Balatro.Core.CoreObjects.Cards.CardObject;
using Balatro.Core.CoreObjects.Registries;
using Balatro.Core.GameEngine.GameStateController.PhaseActions;
using Balatro.Core.GameEngine.GameStateController.PhaseActions.ActionIntents;
using Balatro.Core.GameEngine.StateController;

namespace Balatro.Core.GameEngine.GameStateController.PhaseStates
{
    public class CardPackState : BasePackState<CardPackState>
    {
        private List<Card64> PackCards { get; } = new();

        public CardPackState(GameContext gameContext)
            : base(gameContext)
        {
        }
        
        public List<Card64> GetPackCards()
        {
            return PackCards;
        }

        public override GamePhase Phase => GamePhase.CardPack;

        protected override bool HandleStateSpecificAction(BasePlayerAction action)
        {
            if (action is not PackAction packAction)
            {
                throw new ArgumentException($"Action {action} is not a {nameof(PackAction)}.");
            }
            
            ValidatePossibleAction(packAction);
            
            if (packAction.Intent == PackActionIntent.SkipPack)
            {
                return true; // No further action needed
            }
            
            // Card chosen is the only alternative
            NumberOfChoices--;
            var card = PackCards[packAction.CardIndex];
            GameContext.Deck.Add(card);

            return NumberOfChoices == 0;
        }
        
        public override void OnEnterPhase()
        {
            FillCardsChoice();
        }

        public override void OnExitPhase()
        {
            ClearCardChoice();
        }
        
        private void ValidatePossibleAction(PackAction packAction)
        {
            if (packAction.Intent == PackActionIntent.GetCard)
            {
                if (packAction.CardIndex < 0 || packAction.CardIndex >= PackCards.Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(packAction.CardIndex), "Card index is out of range.");
                }
            }
        }

        private void FillCardsChoice()
        {
            for (int i = 0; i < PackSize; i++)
            {
                var card = CardRegistry.CreateCard(GameContext, fromPack: true,
                    GameContext.PersistentState.Ante.ToString());
                
                PackCards.Add(card);
            }
        }

        private void ClearCardChoice()
        {
            PackCards.Clear();
        }
    }
}