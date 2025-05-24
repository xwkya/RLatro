using Balatro.Core.CoreObjects.CoreEnums;

namespace Balatro.Core.CoreObjects.Shop.ShopObjects
{
    public struct ShopItem
    {
        public ShopItemType Type { get; set; }
        public Edition Edition {get; set; }
        public uint Id { get; set; }
        public int StaticId { get; set; }
    }
}