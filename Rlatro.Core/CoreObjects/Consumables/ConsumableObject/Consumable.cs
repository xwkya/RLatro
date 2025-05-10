using Balatro.Core.Contracts.Consumables;
using Balatro.Core.Contracts.Shop;
using Balatro.Core.CoreObjects.Shop.ShopObjects;
using Balatro.Core.GameEngine.GameStateController;

namespace Balatro.Core.CoreObjects.Consumables.ConsumableObject
{
    public class Consumable : IShopObject
    {
        public ConsumableDef Definition { get; }
        private ushort BonusValue { get; set; }
        public uint Id { get; private set; }
        
        public int BaseSellValue => Definition.BaseCost / 2;
        public IConsumableEffect ConsumableEffect => Definition.ConsumableEffect;
        public int SellValue => (BaseSellValue + BonusValue);
        public bool IsNegative { get; } // For Perkeo

        public bool IsUsable(GameContext context, byte[] cardIndexes)
        {
            return Definition.UsageCondition.IsUsable(context, cardIndexes);
        }
        public void ApplyEffect(GameContext context, byte[] targetCards)
        {
            ConsumableEffect.Apply(context, targetCards);
        }
        public Consumable(uint id, ConsumableDef def, bool isNegative = false)
        {
            Id = id;
            Definition = def;
            BonusValue = 0;
            IsNegative = isNegative;
        }

        public int BaseCost => Definition.BaseCost;
        public ShopItemType ItemType => GetShopItemType();

        private ShopItemType GetShopItemType()
        {
            switch (Definition.Type)
            {
                case ConsumableType.Planet:
                    return ShopItemType.PlanetCard;
                case ConsumableType.Tarot:
                    return ShopItemType.TarotCard;
                case ConsumableType.Spectral:
                    return ShopItemType.SpectralCard;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}