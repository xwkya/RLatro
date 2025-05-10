using Balatro.Core.CoreObjects.Shop.ShopObjects;

namespace Balatro.Core.Contracts.Shop
{
    public interface IShopObject
    {
        public int BaseCost { get; }
        public ShopItemType ItemType { get; }
    }
}