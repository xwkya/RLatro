using Balatro.Core.CoreObjects.Consumables.ConsumableObject;
using Balatro.Core.CoreObjects.CoreEnums;
using Balatro.Core.CoreObjects.Registries;
using Balatro.Core.CoreObjects.Vouchers;
using Balatro.Core.CoreRules.Scoring;
using Balatro.Core.GameEngine.GameStateController.EventBus;
using Balatro.Core.ObjectsImplementations.Consumables;
using Balatro.Core.ObjectsImplementations.Decks;

namespace Balatro.Core.GameEngine.GameStateController.PersistentStates
{
    public class PersistentState : IEventBusSubscriber
    {
        public bool[] OwnedVouchers = new bool[Enum.GetValues(typeof(VoucherType)).Length];
        public int Discards { get; set; }
        public int Hands { get; set; }
        public int HandSize { get; set; }
        public int Round { get; set; }
        public int StartingRollPrice => OwnedVouchers[(int)VoucherType.RerollGlut] ? 1 : OwnedVouchers[(int)VoucherType.RerollSurplus] ? 3 : 5;
        public int? TheFoolStorageStaticId = null;
        public int Ante => 1 + (Round - 1) / 3;
        public AppearanceRates AppearanceRates { get; }
        public HandTracker HandTracker { get; set; }
        public EconomyHandler EconomyHandler { get; }
        public bool FirstShopHasBeenVisited { get; set; }
        public int UnusedDiscards { get; set; }
        public int NumberOfHandsPlayed { get; set; }
        public int NumberOfTarotCardsUsed { get; set; }
        public int EctoplasmUsageCount { get; set; }

        public PersistentState()
        {
            EconomyHandler = new EconomyHandler(this);
            AppearanceRates = new AppearanceRates(this);
            HandTracker = new HandTracker();
        }
        
        public void Reset(InitialConfiguration configuration)
        {
            EconomyHandler.Reset(configuration.StartingGold);
            AppearanceRates.Reset();
            
            HandSize = configuration.HandSize;
            Hands = configuration.Hands;
            Discards = configuration.Discards;
            FirstShopHasBeenVisited = false;
            Round = 1;
            UnusedDiscards = 0;
            NumberOfHandsPlayed = 0;
            NumberOfTarotCardsUsed = 0;
            EctoplasmUsageCount = 0;

            ResetOwnedVouchers();
        }

        
        public ScoreContext GetHandScore(HandRank rank)
        {
            return HandTracker.GetHandScore(rank);
        }

        public void OnVoucherBought(VoucherType voucherType)
        {
            OwnedVouchers[(int)voucherType] = true;
        }
        
        public void OnConsumableUsed(int staticId)
        {
            var consumableType = ConsumableRegistry.GetAttribute(staticId).Type;
            if (staticId != ConsumableRegistry.GetStaticId(typeof(TheFool)) && consumableType != ConsumableType.Spectral)
            {
                TheFoolStorageStaticId = staticId;
            }

            if (consumableType == ConsumableType.Tarot)
            {
                NumberOfTarotCardsUsed++;
            }
        }

        public void Subscribe(GameEventBus eventBus)
        {
            eventBus.SubscribeToVoucherBought(OnVoucherBought);
            eventBus.SubscribeToConsumableUsed(OnConsumableUsed);
            
            // Pass down the subscription to the owned objects
            AppearanceRates.Subscribe(eventBus);
            HandTracker.Subscribe(eventBus);
        }
        
        private void ResetOwnedVouchers()
        {
            for (int i = 0; i < OwnedVouchers.Length; i++)
            {
                OwnedVouchers[i] = false;
            }
        }
        
        /// <summary>
        /// Gets the persistent number of hands per round, including voucher bonuses.
        /// Uses dynamic calculation based on voucher ownership.
        /// Excludes joker bonuses, see <see cref="GameContext"/> method.
        /// </summary>
        public int GetCurrentHands()
        {
            var hands = Hands;
        
            if (OwnedVouchers[(int)VoucherType.NachoTong])
                hands += 2;
            else if (OwnedVouchers[(int)VoucherType.Grabber])
                hands += 1;
            
            // Apply reduction vouchers
            if (OwnedVouchers[(int)VoucherType.Hieroglyph])
                hands = Math.Max(1, hands - 1);
            
            return hands;
        }
        
        /// <summary>
        /// Gets the persistent number of discards per round, including voucher bonuses.
        /// Uses dynamic calculation based on voucher ownership.
        /// Excludes joker bonuses, see <see cref="GameContext"/> method.
        /// </summary>
        public int GetCurrentDiscards()
        {
            var discards = Discards;
        
            if (OwnedVouchers[(int)VoucherType.Recyclomancy])
                discards += 2;
            else if (OwnedVouchers[(int)VoucherType.Wasteful])
                discards += 1;
            
            // Apply reduction vouchers
            if (OwnedVouchers[(int)VoucherType.Petroglyph])
                discards = Math.Max(1, discards - 1);
            
            return discards;
        }
        
        /// <summary>
        /// Gets the persistent hand size
        /// Excludes joker bonuses, see <see cref="GameContext"/> method.
        /// </summary>
        public int GetCurrentHandSize()
        {
            var handSize = HandSize;
        
            if (OwnedVouchers[(int)VoucherType.Palette])
                handSize += 2;
            else if (OwnedVouchers[(int)VoucherType.PaintBrush])
                handSize += 1;

            for (int i = 1; i <= EctoplasmUsageCount; i++)
            {
                handSize -= EctoplasmUsageCount;
            }

            return handSize;
        }
        
        public int GetCurrentShopCapacity()
        {
            return OwnedVouchers[(int)VoucherType.OverstockPlus] ? 4 
                : OwnedVouchers[(int)VoucherType.Overstock] ? 3 : 2;
        }
    }
}