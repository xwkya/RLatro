namespace Balatro.Core.CoreObjects.Vouchers
{
    public enum VoucherType : int
    {
        Overstock = 0,
        OverstockPlus = 1,
        ClearanceSale = 2,
        Liquidation = 3,
        Hone = 4,
        GlowUp = 5,
        RerollSurplus = 6,
        RerollGlut = 7,
        CrystalBall = 8,
        OmenGlobe = 9,
        Telescope = 10,
        Observatory = 11,
        Grabber = 12,
        NachoTong = 13,
        Wasteful = 14,
        Recyclomancy = 15,
        TarotMerchant = 16,
        TarotTycoon = 17,
        PlanetMerchant = 18,
        PlanetTycoon = 19,
        SeedMoney = 20,
        MoneyTree = 21,
        Blank = 22,
        Antimatter = 23,
        MagicTrick = 24,
        Illusion = 25,
        Hieroglyph = 26,
        Petroglyph = 27,
        DirectorsCut = 28,
        Retcon = 29,
        PaintBrush = 30,
        Palette = 31
    }
    
    public static class VoucherHelpers
    {
        public static bool IsBaseVoucher(this VoucherType voucherType)
        {
            return (int)voucherType % 2 == 0;
        }
    }
}