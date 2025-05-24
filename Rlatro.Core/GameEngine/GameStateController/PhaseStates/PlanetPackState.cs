using Balatro.Core.CoreObjects;
using Balatro.Core.CoreObjects.BoosterPacks;
using Balatro.Core.CoreObjects.Consumables.ConsumableObject;
using Balatro.Core.CoreObjects.Shop.ShopObjects;
using Balatro.Core.GameEngine.Contracts;
using Balatro.Core.GameEngine.GameStateController.PhaseActions;
using Balatro.Core.GameEngine.GameStateController.PhaseActions.ActionIntents;
using Balatro.Core.GameEngine.PseudoRng;
using Balatro.Core.GameEngine.StateController;

namespace Balatro.Core.GameEngine.GameStateController.PhaseStates
{
    public class PlanetPackState : IGamePhaseState
    {
        public GameContext GameContext { get; }
        private ShopState IncomingState { get; }
        private int PlanetPackSize { get; init; }
        private int NumberOfChoices { get; set; }
        private List<ShopItem> PlanetCards { get; } = new();
        private static int[] TargetCards = new int[] { };
        
        public PlanetPackState(GameContext gameContext, ShopState incomingState, BoosterPackType packType)
        {
            GameContext = gameContext;
            IncomingState = incomingState;
            
            var t = packType.GetPackSizeAndChoice();
            NumberOfChoices = t.choice;
            PlanetPackSize = t.size;
            FillCardsChoice();
        }
        
        public GamePhase Phase => GamePhase.PlanetPack;
        
        public bool HandleAction(BasePlayerAction action)
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

        public IGamePhaseState GetNextPhaseState()
        {
            return IncomingState;
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
            for (var i = 0; i < PlanetPackSize; i++)
            {
                var card = GameContext.GlobalPoolManager.GenerateShopConsumable(RngActionType.RandomPackConsumable, ConsumableType.Planet);
                PlanetCards.Add(card);
            }
        }
    }
}