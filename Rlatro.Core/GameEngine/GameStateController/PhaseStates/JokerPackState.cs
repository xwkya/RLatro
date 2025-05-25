using Balatro.Core.CoreObjects;
using Balatro.Core.CoreObjects.Shop.ShopObjects;
using Balatro.Core.GameEngine.GameStateController.PhaseActions;
using Balatro.Core.GameEngine.GameStateController.PhaseActions.ActionIntents;
using Balatro.Core.GameEngine.PseudoRng;
using Balatro.Core.GameEngine.StateController;

namespace Balatro.Core.GameEngine.GameStateController.PhaseStates
{
    public class JokerPackState : BasePackState<JokerPackState>
    {
        private List<ShopItem> JokerObjects { get; set; } = new();
        
        public JokerPackState(GameContext ctx) : base(ctx)
        {
        }
        
        public override GamePhase Phase => GamePhase.JokerPack;

        protected override bool HandleStateSpecificAction(BasePlayerAction action)
        {
            if (action is not PackAction packAction)
            {
                throw new ArgumentException($"Action is not a {nameof(PackAction)}.");
            }
            
            ValidatePossibleAction(packAction);
            if (packAction.Intent == PackActionIntent.SkipPack)
            {
                return true;
            }
            
            var jokerShopItem = JokerObjects[packAction.CardIndex];
            var joker = CoreObjectsFactory.CreateJoker(jokerShopItem);
            GameContext.JokerContainer.AddJoker(GameContext, joker);
            JokerObjects.RemoveAt(packAction.CardIndex);
            NumberOfChoices--;
            
            return NumberOfChoices == 0;
        }

        public override void OnEnterPhase()
        {
            FillJokerChoices();
        }

        public override void OnExitPhase()
        {
            ClearJokerChoices();
        }

        private void ValidatePossibleAction(PackAction packAction)
        {
            if (packAction.Intent == PackActionIntent.GetCard)
            {
                if (packAction.CardIndex < 0 || packAction.CardIndex >= JokerObjects.Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(packAction), "Card index is out of range.");
                }

                if (GameContext.JokerContainer.Slots <= GameContext.JokerContainer.Jokers.Count)
                {
                    throw new ArgumentException("Joker container is full.", nameof(packAction));
                }
            }
        }

        private void FillJokerChoices()
        {
            JokerObjects = new List<ShopItem>();
            for (int i = 0; i < PackSize; i++)
            {
                var joker = GameContext.GlobalPoolManager.GenerateJokerShopItem(RngActionType.RandomPackJoker);
                JokerObjects.Add(joker);
            }
        }

        private void ClearJokerChoices()
        {
            foreach (var t in JokerObjects)
            {
                GameContext.GameEventBus.PublishConsumableRemovedFromContext(t.StaticId);
            }

            JokerObjects.Clear();
        }
    }
}