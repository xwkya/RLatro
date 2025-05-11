using Balatro.Core.Contracts.Shop;
using Balatro.Core.CoreObjects.Consumables.ConsumableObject;
using Balatro.Core.CoreObjects.Jokers.Joker;
using Balatro.Core.CoreObjects.Shop.ShopObjects;
using Balatro.Core.GameEngine.GameStateController;
using Balatro.Core.GameEngine.PseudoRng;

namespace Balatro.Core.CoreObjects.Pools
{
    public class GlobalPoolManager
    {
        private GameContext GameContext { get; }
        private RngController RngController { get; }
        private JokerPoolManager JokerPoolManager { get; }
        private VoucherPool VoucherPool { get; }
        private ConsumablePoolManager ConsumablePoolManager { get; }

        public GlobalPoolManager(GameContext ctx)
        {
            GameContext = ctx;
            JokerPoolManager = new JokerPoolManager(ctx);
            VoucherPool = new VoucherPool();
            ConsumablePoolManager = new ConsumablePoolManager(ctx);
            RngController = ctx.RngController;
        }
        
        public void InitializePools()
        {
            JokerPoolManager.InitializePool();
            ConsumablePoolManager.InitializePool();
            // TODO: VoucherPool.InitializeVoucherPool();
        }

        public IShopObject GenerateShopItem()
        {
            var itemType = CalculateShopItemType();
            if (itemType == ShopItemType.Joker)
            {
                var rarity = CalculateJokerRarity();
                var jokerStaticId = JokerPoolManager.GetRandomStaticId(rarity, RngController);
                GameContext.GameEventBus.PublishJokerAddedToContext(jokerStaticId);
                return GameContext.CoreObjectsFactory.CreateJoker(jokerStaticId);
            }

            if (itemType == ShopItemType.PlayingCard)
            {
                throw new NotImplementedException("Playing cards in shop are not yet implemented");
            }
            
            // Consumables
            var consumableType = itemType switch
            {
                ShopItemType.TarotCard => ConsumableType.Tarot,
                ShopItemType.PlanetCard => ConsumableType.Planet,
                ShopItemType.SpectralCard => ConsumableType.Spectral,
                _ => throw new ArgumentOutOfRangeException(nameof(itemType), itemType, null)
            };
            var consumable = ConsumablePoolManager.GetRandomStaticId(consumableType, RngController);
            GameContext.GameEventBus.PublishConsumableAddedToContext(consumable);
            return GameContext.CoreObjectsFactory.CreateConsumable(consumable);
        }

        private ShopItemType CalculateShopItemType()
        {
            var sumOfRates = GameContext.PersistentState.AppearanceRates.JokerRate +
                             GameContext.PersistentState.AppearanceRates.PlanetRate +
                             GameContext.PersistentState.AppearanceRates.TarotRate +
                             GameContext.PersistentState.AppearanceRates.SpectralRate +
                             GameContext.PersistentState.AppearanceRates.PlayingCardRate;
            
            var randomValue = RngController.RandomInt(1, sumOfRates, RngActionType.ShopItemType, GameContext.PersistentState.Ante.ToString());
            if (randomValue <= GameContext.PersistentState.AppearanceRates.JokerRate)
            {
                return ShopItemType.Joker;
            }
            if (randomValue <= GameContext.PersistentState.AppearanceRates.JokerRate + GameContext.PersistentState.AppearanceRates.PlanetRate)
            {
                return ShopItemType.PlanetCard;
            }
            if (randomValue <= GameContext.PersistentState.AppearanceRates.JokerRate + GameContext.PersistentState.AppearanceRates.PlanetRate + GameContext.PersistentState.AppearanceRates.TarotRate)
            {
                return ShopItemType.TarotCard;
            }
            if (randomValue <= GameContext.PersistentState.AppearanceRates.JokerRate + GameContext.PersistentState.AppearanceRates.PlanetRate + GameContext.PersistentState.AppearanceRates.TarotRate + GameContext.PersistentState.AppearanceRates.SpectralRate)
            {
                return ShopItemType.SpectralCard;
            }
            
            return ShopItemType.PlayingCard;
        }

        private JokerRarity CalculateJokerRarity()
        {
            var randomValue = RngController.GetRandomProbability(RngActionType.JokerRarity,
                GameContext.PersistentState.Ante.ToString());
            if (randomValue <= GameContext.PersistentState.AppearanceRates.CommonRarityRate)
            {
                return JokerRarity.Common;
            }

            if (randomValue <= GameContext.PersistentState.AppearanceRates.CommonRarityRate +
                GameContext.PersistentState.AppearanceRates.UncommonRarityRate)
            {
                return JokerRarity.Uncommon;
            }

            return JokerRarity.Rare;
        }
    }
}