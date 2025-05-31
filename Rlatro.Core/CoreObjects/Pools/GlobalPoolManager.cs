using Balatro.Core.CoreObjects.Consumables.ConsumableObject;
using Balatro.Core.CoreObjects.CoreEnums;
using Balatro.Core.CoreObjects.Jokers.Joker;
using Balatro.Core.CoreObjects.Shop.ShopObjects;
using Balatro.Core.CoreObjects.Vouchers;
using Balatro.Core.GameEngine.GameStateController;
using Balatro.Core.GameEngine.GameStateController.EventBus;
using Balatro.Core.GameEngine.PseudoRng;

namespace Balatro.Core.CoreObjects.Pools
{
    public class GlobalPoolManager : IEventBusSubscriber
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
            VoucherPool = new VoucherPool(ctx);
            ConsumablePoolManager = new ConsumablePoolManager(ctx);
            RngController = ctx.RngController;
        }
        
        public void InitializePools()
        {
            JokerPoolManager.InitializePool();
            ConsumablePoolManager.InitializePool();
            VoucherPool.InitializePool();
        }
        
        public void Subscribe(GameEventBus eventBus)
        {
            ConsumablePoolManager.Subscribe(eventBus);
            JokerPoolManager.Subscribe(eventBus);
            VoucherPool.Subscribe(eventBus);
        }

        public ShopItem GenerateShopConsumable(RngActionType actionType, ConsumableType type)
        {
            var consumable = ConsumablePoolManager.GetRandomStaticId(type, RngController, actionType);
            GameContext.GameEventBus.PublishConsumableAddedToContext(consumable);
            
            var runtimeId = GameContext.CoreObjectsFactory.GetNextRuntimeId();
            return new ShopItem()
            {
                Id = runtimeId,
                StaticId = consumable,
                Type = type.GetShopItemType(),
                Edition = Edition.None,
            };
        }

        public VoucherType GetNewAnteVoucher()
        {
            return VoucherPool.GetNewAnteVoucher(GameContext.RngController);
        }

        public VoucherType[] GetTagVoucher(int numberOfVouchers)
        {
            return VoucherPool.GetTagVoucher(numberOfVouchers, GameContext.RngController);
        }

        public Consumable GenerateConsumable(RngActionType actionType, ConsumableType type)
        {
            var consumableStaticId = ConsumablePoolManager.GetRandomStaticId(type, RngController, actionType);
            GameContext.GameEventBus.PublishConsumableAddedToContext(consumableStaticId);

            return GameContext.CoreObjectsFactory.CreateConsumable(consumableStaticId);
        }

        public ShopItem GenerateJokerShopItem(RngActionType actionType)
        {
            var rarity = CalculateJokerRarity();
            var jokerStaticId = JokerPoolManager.GetRandomStaticId(rarity, RngController, actionType);
            var edition = GetJokerEdition();
            GameContext.GameEventBus.PublishJokerAddedToContext(jokerStaticId);
            
            var runtimeId = GameContext.CoreObjectsFactory.GetNextRuntimeId();
            return new ShopItem()
            {
                Id = runtimeId,
                StaticId = jokerStaticId,
                Type = ShopItemType.Joker,
                Edition = edition,
            };
        }

        public JokerObject GenerateJoker(RngActionType actionType, JokerRarity? rarity = null)
        {
            if (rarity == null)
            {
                rarity = CalculateJokerRarity();
            }
            
            var jokerStaticId = JokerPoolManager.GetRandomStaticId(rarity.Value, RngController, actionType);
            var edition = GetJokerEdition();
            var joker = GameContext.CoreObjectsFactory.CreateJoker(jokerStaticId, edition: edition);
            GameContext.GameEventBus.PublishJokerAddedToContext(jokerStaticId);
            return joker;
        }

        public ShopItem GenerateShopItem()
        {
            var itemType = CalculateShopItemType();
            if (itemType == ShopItemType.Joker)
            {
                return GenerateJokerShopItem(RngActionType.RandomShopJoker);
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

            return GenerateShopConsumable(RngActionType.RandomShopConsumable, consumableType);
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
            if (randomValue <= GameContext.PersistentState.AppearanceRates.JokerRate 
                + GameContext.PersistentState.AppearanceRates.PlanetRate)
            {
                return ShopItemType.PlanetCard;
            }
            if (randomValue <= GameContext.PersistentState.AppearanceRates.JokerRate 
                + GameContext.PersistentState.AppearanceRates.PlanetRate 
                + GameContext.PersistentState.AppearanceRates.TarotRate)
            {
                return ShopItemType.TarotCard;
            }
            if (randomValue <= GameContext.PersistentState.AppearanceRates.JokerRate
                + GameContext.PersistentState.AppearanceRates.PlanetRate 
                + GameContext.PersistentState.AppearanceRates.TarotRate
                + GameContext.PersistentState.AppearanceRates.SpectralRate)
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

        private Edition GetJokerEdition()
        {
            var randomValue = RngController.GetRandomProbability(RngActionType.JokerEdition);
            float runningSum = GameContext.PersistentState.AppearanceRates.PolychromeJokerRate;
            if (randomValue <= runningSum)
            {
                return Edition.Poly;
            }
            
            runningSum += GameContext.PersistentState.AppearanceRates.FoilJokerRate;
            
            if (randomValue <= runningSum)
            {
                return Edition.Foil;
            }
            
            runningSum += GameContext.PersistentState.AppearanceRates.HoloJokerRate;
            
            if (randomValue <= runningSum)
            {
                return Edition.Holo;
            }

            runningSum += GameContext.PersistentState.AppearanceRates.NegativeJokerRate;


            if (randomValue <= runningSum)
            {
                return Edition.Negative;
            }

            return Edition.None;
        }
    }
}