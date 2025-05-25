using Balatro.Core.CoreObjects;
using Balatro.Core.CoreObjects.Consumables.ConsumableObject;
using Balatro.Core.CoreObjects.Shop.ShopObjects;
using Balatro.Core.GameEngine.GameStateController.PhaseActions;
using Balatro.Core.GameEngine.GameStateController.PhaseActions.ActionIntents;
using Balatro.Core.GameEngine.PseudoRng;
using Balatro.Core.GameEngine.StateController;

namespace Balatro.Core.GameEngine.GameStateController.PhaseStates
{
    public class PlanetPackState : BasePackState<PlanetPackState>
    {
        private List<ShopItem> PlanetCards { get; } = new();
        private static int[] TargetCards = new int[] { };

        public PlanetPackState(GameContext gameContext) :
            base(gameContext)
        {
        }

        public override GamePhase Phase => GamePhase.PlanetPack;

        protected override bool HandleStateSpecificAction(BasePlayerAction action)
        {
            if (action is not PackAction packAction)
            {
                throw new ArgumentException($"Action {action} is not a {nameof(PackAction)}.");
            }

            ValidatePossibleAction(packAction);

            if (packAction.Intent == PackActionIntent.SkipPack)
            {
                return true;
            }

            // Card chosen is the only alternative
            NumberOfChoices--;
            var consumableShopItem = PlanetCards[packAction.CardIndex];
            var consumable = CoreObjectsFactory.CreateConsumable(consumableShopItem);
            consumable.Apply(GameContext, TargetCards);

            GameContext.GameEventBus.PublishConsumableRemovedFromContext(consumable.StaticId);
            PlanetCards.RemoveAt(packAction.CardIndex);

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

        private void ValidatePossibleAction(PackAction action)
        {
            if (action.Intent == PackActionIntent.GetCard && action.CardIndex >= PlanetCards.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(action), action, "Card index is out of range.");
            }
        }

        private void FillCardsChoice()
        {
            for (var i = 0; i < PackSize; i++)
            {
                var card = GameContext.GlobalPoolManager.GenerateShopConsumable(RngActionType.RandomPackConsumable,
                    ConsumableType.Planet);
                PlanetCards.Add(card);
            }
        }

        private void ClearCardChoice()
        {
            foreach (var t in PlanetCards)
            {
                GameContext.GameEventBus.PublishConsumableRemovedFromContext(t.StaticId);
            }

            PlanetCards.Clear();
        }
    }
}