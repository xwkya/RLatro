namespace Balatro.Core.GameEngine.GameStateController.PhaseActions
{
    public enum ShopActionIntent
    {
        BuyFromShop,
        BuyBoosterPack,
        BuyVoucher,
        SellJoker,
        SellConsumable,
        Roll,
    }
    
    public sealed class ShopAction : BasePlayerAction
    {
        public ShopActionIntent ActionIntent { get; init; }
        public byte JokerIndex { get; init; }
        public byte ConsumableIndex { get; init; }
        public byte BoosterPackIndex { get; init; }
        public byte ShopIndex { get; init; }
        public byte VoucherIndex { get; init; }
    }
}