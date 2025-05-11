using Balatro.Core.Contracts.Shop;
using Balatro.Core.CoreObjects.Shop.ShopObjects;
using Balatro.Core.GameEngine.GameStateController;

namespace Balatro.Core.CoreObjects.Shop.ShopContainers
{
    public class ShopContainer
    {
        public int Capacity { get; set; }
        public List<IShopObject> Items { get; set; }

        public void ClearItems(GameContext ctx)
        {
            foreach (var item in Items)
            {
                if (item.ShopItemType == ShopItemType.Joker)
                {
                    ctx.GameEventBus.PublishJokerRemovedFromContext(item.StaticId);
                }
                if (item.ShopItemType == ShopItemType.PlanetCard || 
                    item.ShopItemType == ShopItemType.TarotCard ||
                    item.ShopItemType == ShopItemType.SpectralCard)
                {
                    ctx.GameEventBus.PublishConsumableRemovedFromContext(item.StaticId);
                }
            }
        }
    }
}