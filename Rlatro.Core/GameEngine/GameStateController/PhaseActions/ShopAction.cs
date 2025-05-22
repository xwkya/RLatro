namespace Balatro.Core.GameEngine.GameStateController.PhaseActions
{
    public enum ShopActionIntent
    {
        BuyFromShop,
        BuyBoosterPack,
        BuyVoucher,
        Roll,
    }
    
    public sealed class ShopAction : BasePlayerAction
    {
        public ShopActionIntent ActionIntent { get; init; }
        public byte BoosterPackIndex { get; init; }
        public byte ShopIndex { get; init; }
        public byte VoucherIndex { get; init; }
    }
}