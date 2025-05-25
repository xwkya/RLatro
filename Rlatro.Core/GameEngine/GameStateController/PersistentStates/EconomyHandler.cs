using Balatro.Core.CoreObjects.BoosterPacks;
using Balatro.Core.CoreObjects.Consumables.ConsumableObject;
using Balatro.Core.CoreObjects.CoreEnums;
using Balatro.Core.CoreObjects.Jokers.Joker;
using Balatro.Core.CoreObjects.Registries;
using Balatro.Core.CoreObjects.Shop.ShopObjects;
using Balatro.Core.CoreObjects.Vouchers;

namespace Balatro.Core.GameEngine.GameStateController.PersistentStates
{
    public class EconomyHandler
    {
        private const int Discount30VoucherIndex = (int)VoucherType.ClearanceSale;
        private const int Discount50VoucherIndex = (int)VoucherType.Liquidation;
        private const int VoucherBasePrice = 10;
        private const int FoilPrice = 2;
        private const int HologramPrice = 3;
        private const int PolychromePrice = 5;
        private const int NegativePrice = 5;
        private const int BaseGoldPerHandLeft = 1;

        private const int SmallBlindReward = 3;
        private const int BigBlindReward = 4;
        private const int BossBlindReward = 5;

        /// <summary>
        /// Reference to the game instance's persistent state for voucher lookup.
        /// </summary>
        private PersistentState PersistentState { get; set; }

        private int CurrentGold { get; set; }
        private int MinGold { get; set; } = 0;

        public EconomyHandler(PersistentState persistentState, int startingGold)
        {
            PersistentState = persistentState;
            CurrentGold = startingGold;
        }

        public int GetCurrentGold()
        {
            return CurrentGold;
        }

        public void AddGold(int amount)
        {
            CurrentGold += amount;
        }

        public void SpendGold(int amount)
        {
            CurrentGold -= amount;
        }

        public bool CanBuy(ShopItem item)
        {
            return CanSpend(GetShopItemPrice(item));
        }

        public bool CanBuy(BoosterPackType packType)
        {
            return CanSpend(GetBoosterPackPrice(packType));
        }

        public bool CanBuyVoucher()
        {
            return CanSpend(GetVoucherPrice());
        }

        public bool CanSpend(int goldAmount)
        {
            return CurrentGold - goldAmount >= MinGold;
        }

        public int GetVoucherPrice()
        {
            return GetDiscountedPrice(VoucherBasePrice);
        }

        public int GetGoldPerHandLeft()
        {
            return BaseGoldPerHandLeft;
        }

        public int CalculateInterest()
        {
            int maxInterest;
            if (PersistentState.OwnedVouchers[(int)VoucherType.MoneyTree])
            {
                maxInterest = 100;
            }
            else if (PersistentState.OwnedVouchers[(int)VoucherType.SeedMoney])
            {
                maxInterest = 50;
            }
            else
            {
                maxInterest = 25;
            }

            var interest = CurrentGold > maxInterest ? maxInterest / 5 : CurrentGold / 5;
            return interest;
        }

        public int CalculateRoundGold()
        {
            var roundType = (PersistentState.Round - 1) % 3;
            return roundType switch
            {
                0 => SmallBlindReward,
                1 => BigBlindReward,
                2 => BossBlindReward,
                _ => 0,
            };
        }

        public int GetShopItemPrice(ShopItem item)
        {
            var basePrice = GetShopItemBasePrice(item);
            var editionBonus = GetEditionBonusValue(item.Edition);
            var discountedPrice = GetDiscountedPrice(basePrice + editionBonus);
            return discountedPrice < 1 ? 1 : discountedPrice;
        }

        public int GetBoosterPackPrice(BoosterPackType type)
        {
            return GetDiscountedPrice(type.GetPackPrice());
        }

        public int GetJokerSellPrice(JokerObject joker)
        {
            // The price is calculated as if it was bought now, plus its sell value
            var basePrice = GetJokerBasePrice(joker.StaticId);
            var editionBonus = GetEditionBonusValue(joker.Edition);
            var discountedPrice = GetDiscountedPrice(basePrice + editionBonus);

            var sellPrice = discountedPrice / 2 + joker.BonusSellValue;
            return sellPrice < 1 ? 1 : sellPrice;
        }

        public int GetConsumableSellPrice(Consumable consumable)
        {
            var basePrice = GetConsumableBasePrice(consumable.StaticId);
            var editionAdditionalValue = consumable.IsNegative ? NegativePrice : 0;
            return GetDiscountedPrice(basePrice + editionAdditionalValue) / 2 + consumable.BonusValue;
        }

        private int GetDiscountedPrice(int price)
        {
            if (PersistentState.OwnedVouchers[Discount50VoucherIndex])
            {
                return price / 2;
            }

            if (PersistentState.OwnedVouchers[Discount30VoucherIndex])
            {
                return price * 7 / 10; // 30% discount
            }

            return price;
        }

        private int GetShopItemBasePrice(ShopItem item)
        {
            switch (item.Type)
            {
                case ShopItemType.Joker:
                    return GetJokerBasePrice(item.StaticId);
                default:
                    return GetConsumableBasePrice(item.StaticId);
            }
        }

        private int GetEditionBonusValue(Edition edition)
        {
            switch (edition)
            {
                case Edition.Foil:
                    return FoilPrice;
                case Edition.Holo:
                    return HologramPrice;
                case Edition.Poly:
                    return PolychromePrice;
                case Edition.Negative:
                    return NegativePrice;
                default:
                    return 0;
            }
        }

        private int GetJokerBasePrice(int jokerStaticId)
        {
            return JokerRegistry.GetAttribute(jokerStaticId).BasePrice;
        }

        private int GetConsumableBasePrice(int consumableStaticId)
        {
            return ConsumableRegistry.GetAttribute(consumableStaticId).BasePrice;
        }

        public void ActivateCreditCardJoker()
        {
            MinGold = -20;
        }

        public void DisableCreditCardJoker()
        {
            MinGold = 0;
        }
    }
}