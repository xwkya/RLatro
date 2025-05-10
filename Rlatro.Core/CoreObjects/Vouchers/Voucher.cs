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
        PlanetMerchant = 16,
        PlanetTycoon = 17,
        SeedMoney = 18,
        MoneyTree = 19,
        Blank = 20,
        Antimatter = 21,
        MagicTrick = 22,
        Illusion = 23,
        Hieroglyph = 24,
        Petroglyph = 25,
        DirectorsCut = 26,
        Retcon = 27,
        PaintBrush = 28,
        Palette = 29,
    }
    
    public static class VoucherHelpers
    {
        public static bool IsBaseVoucher(this VoucherType voucherType)
        {
            return (int)voucherType % 2 == 0;
        }
    }
}