using Balatro.Core.Contracts.Consumables;
using Balatro.Core.Contracts.Shop;
using Balatro.Core.CoreObjects.Shop.ShopObjects;
using Balatro.Core.GameEngine.GameStateController;

namespace Balatro.Core.CoreObjects.Consumables.ConsumableObject
{
    public abstract class Consumable : IShopObject, IConsumableEffect, IUsageCondition
    {
        public ConsumableType Type;
        private ushort BonusValue { get; set; }
        
        public int StaticId { get; } // Static definition ID
        public uint Id { get; private set; }
        
        public int SellValue => (BaseSellValue + BonusValue);
        public bool IsNegative { get; } // For Perkeo
        
        public abstract void Apply(GameContext context, int[] targetCards);
        public abstract bool IsUsable(GameContext ctx, int[] targetCards);
        
        public Consumable(int staticId, uint id, bool isNegative = false)
        {
            Id = id;
            StaticId = staticId;
            BonusValue = 0;
            IsNegative = isNegative;
        }

        public int BaseSellValue => BaseCost / 2;
        public abstract int BaseCost { get; }
        public ShopItemType ShopItemType => GetShopItemType();

        private ShopItemType GetShopItemType()
        {
            return Type switch
            {
                ConsumableType.Tarot => ShopItemType.TarotCard,
                ConsumableType.Planet => ShopItemType.PlanetCard,
                ConsumableType.Spectral => ShopItemType.SpectralCard,
                _ => throw new ArgumentOutOfRangeException(nameof(Type), Type, null)
            };
        }
    }
}