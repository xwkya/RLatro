using Balatro.Core.CoreObjects.Shop.ShopObjects;

namespace Balatro.Core.Contracts.Shop
{
    public interface IShopObject
    {
        public ShopItemType ShopItemType { get; }
        public int StaticId { get; }
    }
}