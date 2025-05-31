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
        public int StartingRollPrice { get; set; } = 5;
        public int? TheFoolStorageStaticId = null;
        public int Ante => 1 + (Round - 1) / 3;
        public AppearanceRates AppearanceRates { get; }
        public HandTracker HandTracker { get; set; }
        public EconomyHandler EconomyHandler { get; }
        public bool FirstShopHasBeenVisited { get; set; }
        public int UnusedDiscards { get; set; }
        public int NumberOfHandsPlayed { get; set; }
        public int NumberOfTarotCardsUsed { get; set; }

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
            StartingRollPrice = 5;
            Round = 1;
            UnusedDiscards = 0;
            NumberOfHandsPlayed = 0;
            NumberOfTarotCardsUsed = 0;
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
    }
}