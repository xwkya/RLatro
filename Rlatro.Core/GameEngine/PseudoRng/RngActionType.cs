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
        RandomJoker,
        RandomConsumable,
        ShopItemType,
        JokerRarity,
    }
    
    static class RngActionTypeExt
    {
        public static string Key(this RngActionType a) => a.ToString();
    }
}