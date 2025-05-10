using Balatro.Core.Contracts.Shop;

namespace Balatro.Core.CoreObjects.Shop.ShopContainers
{
    public class ShopContainer
    {
        public int Capacity { get; set; }
        public List<IShopObject> Items { get; set; }
        
        
    }
}