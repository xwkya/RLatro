using Balatro.Core.CoreObjects.Shop.ShopObjects;

namespace Balatro.Core.CoreObjects.Consumables.ConsumableObject
{
    public enum ConsumableType
    {
        Planet,
        Tarot,
        Spectral
    }
    
    public static class ConsumableTypeExtensions
    {
        public static ShopItemType GetShopItemType(this ConsumableType consumableType)
        {
            return consumableType switch
            {
                ConsumableType.Tarot => ShopItemType.TarotCard,
                ConsumableType.Planet => ShopItemType.PlanetCard,
                ConsumableType.Spectral => ShopItemType.SpectralCard,
                _ => throw new ArgumentOutOfRangeException(nameof(consumableType), consumableType, null)
            };
        }
    }
}