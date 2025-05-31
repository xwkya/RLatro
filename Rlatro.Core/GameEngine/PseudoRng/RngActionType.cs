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
        RandomPack,
        RandomShopJoker,
        RandomPackJoker,
        RandomShopConsumable,
        RandomPackConsumable,
        ShopItemType,
        TopUpTagJokerPoll,
        JokerRarity,
        ShopCardBase,
        PackCardBase,
        ShopCardEnhancement,
        PackCardEnhancement,
        ShopCardEdition,
        PackCardEdition,
        ShopCardSeal,
        PackCardSeal,
        JokerEdition,
        GetNewAnteSkipTags,
        TheHighPriestess,
        Judgement,
        SpectralFamiliar,
        SpectralGrim,
        SpectralIncantation,
        SpectralAura,
        SpectralWraith,
        SpectralSigil,
        SpectralOuija,
        SpectralEctoplasm,
        SpectralImmolate,
        SpectralAnkh,
        SpectralHex,
        SpectralTheSoul
    }
    
    static class RngActionTypeExt
    {
        public static string Key(this RngActionType a) => a.ToString();
    }
}