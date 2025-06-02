using Balatro.Core.CoreObjects.Cards.CardObject;
using Balatro.Core.CoreObjects.CoreEnums;

namespace Balatro.Core.CoreObjects.Shop.ShopObjects
{
    public struct ShopItem
    {
        public ShopItemType Type { get; set; }
        public Edition Edition {get; set; }
        public uint Id { get; set; }
        public int StaticId { get; set; }
        public uint Card64Raw { get; set; }

        public static ShopItem FromCard(Card64 card)
        {
            return new ShopItem
            {
                Edition = card.GetEdition(),
                Id = card.Id,
                StaticId = 0,
                Type = ShopItemType.PlayingCard,
                Card64Raw = card.GetRaw()
            };
        }
    }
}