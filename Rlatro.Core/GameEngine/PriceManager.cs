using Balatro.Core.Contracts.Shop;
using Balatro.Core.CoreObjects.CoreEnums;
using Balatro.Core.CoreObjects.Consumables.ConsumableObject;
using Balatro.Core.CoreObjects.Jokers.Joker;
using Balatro.Core.CoreObjects.Vouchers;
using Balatro.Core.CoreObjects.Cards.CardObject;
using Balatro.Core.GameEngine.GameStateController;

namespace Balatro.Core.GameEngine
{
    /// <summary>
    /// Centralised helper used to compute buy and sell prices for shop items.
    /// </summary>
    public class PriceManager
    {
        private readonly GameContext _ctx;

        private const int VoucherBasePrice = 10;
        private const int PlayingCardBaseCost = 1;

        public PriceManager(GameContext ctx)
        {
            _ctx = ctx;
        }

        /// <summary>
        /// Returns the gold required to buy a shop object taking into account
        /// owned discount vouchers.
        /// </summary>
        public int GetBuyPrice(IShopObject shopObject)
        {
            int price = shopObject.BaseCost;
            return ApplyDiscount(price);
        }

        /// <summary>
        /// Returns the price for a joker defined by its static id.
        /// </summary>
        public int GetBuyPriceForJoker(int staticId)
        {
            var price = Registries.JokerRegistry.GetBasePrice(staticId);
            return ApplyDiscount(price);
        }

        /// <summary>
        /// Base price of a playing card taking into account discounts.
        /// </summary>
        public int GetBuyPrice(Card64 card)
        {
            return ApplyDiscount(PlayingCardBaseCost);
        }

        /// <summary>
        /// Gets the price for a voucher taking discounts into account.
        /// </summary>
        public int GetBuyPrice(VoucherType voucher)
        {
            return ApplyDiscount(VoucherBasePrice);
        }

        /// <summary>
        /// Compute the sell value of a shop object. Discounts do not apply.
        /// </summary>
        public int GetSellPrice(IShopObject shopObject)
        {
            return shopObject.BaseCost / 2 + GetBonusSellValue(shopObject);
        }

        private static int GetBonusSellValue(IShopObject shopObject)
        {
            return shopObject switch
            {
                JokerObject joker => joker.BonusSellValue,
                Consumable consumable => consumable.SellValue - consumable.BaseCost / 2,
                _ => 0
            };
        }

        private int ApplyDiscount(int basePrice)
        {
            float discount = 1f;
            var owned = _ctx.PersistentState.OwnedVouchers;
            if (owned[(int)VoucherType.ClearanceSale])
            {
                discount *= 0.7f; // 30% discount
            }

            if (owned[(int)VoucherType.Liquidation])
            {
                discount *= 0.5f; // 50% discount
            }

            return (int)MathF.Ceiling(basePrice * discount);
        }
    }
}
