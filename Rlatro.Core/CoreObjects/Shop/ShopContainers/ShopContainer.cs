using Balatro.Core.CoreObjects.Shop.ShopObjects;
using Balatro.Core.GameEngine.GameStateController;

namespace Balatro.Core.CoreObjects.Shop.ShopContainers
{
    public class ShopContainer
    {
        public int Capacity { get; set; }
        public List<ShopItem> Items { get; set; }
        
        public ShopContainer(int capacity)
        {
            Capacity = capacity;
            Items = new List<ShopItem>(capacity);
        }

        public void ClearItems(GameContext ctx)
        {
            foreach (var item in Items)
            {
                if (item.Type == ShopItemType.Joker)
                {
                    ctx.GameEventBus.PublishJokerRemovedFromContext(item.StaticId);
                }
                if (item.Type == ShopItemType.PlanetCard || 
                    item.Type == ShopItemType.TarotCard ||
                    item.Type == ShopItemType.SpectralCard)
                {
                    ctx.GameEventBus.PublishConsumableRemovedFromContext(item.StaticId);
                }
            }
        }
    }
}