namespace Balatro.Core.GameEngine.PseudoRng
{
    public enum RngActionType
    {
        LuckyCardMoney,
        LuckyCardMult,
        Shuffle,
        WheelOfFortune,
        GrosMichel,
        Cavendish,
        SpaceJoker,
        GetSingleVoucher,
        GetTagVouchers,
        RandomShopJoker,
        RandomPackJoker,
        RandomShopConsumable,
        RandomPackConsumable,
        ShopItemType,
        JokerRarity,
        ShopCardBase,
        PackCardBase,
        ShopCardEnhancement,
        PackCardEnhancement,
        ShopCardEdition,
        PackCardEdition,
        ShopCardSeal,
        PackCardSeal,
    }
    
    static class RngActionTypeExt
    {
        public static string Key(this RngActionType a) => a.ToString();
    }
}