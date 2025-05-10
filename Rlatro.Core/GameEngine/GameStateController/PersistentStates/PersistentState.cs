using Balatro.Core.CoreObjects.CoreEnums;
using Balatro.Core.CoreObjects.Vouchers;
using Balatro.Core.CoreRules.Scoring;
using Balatro.Core.GameEngine.GameStateController.EventBus;

namespace Balatro.Core.GameEngine.GameStateController.PersistentStates
{
    public class PersistentState : IEventBusSubscriber
    {
        public bool[] OwnedVouchers = new bool[Enum.GetValues(typeof(VoucherType)).Length];
        public int Gold { get; set; }
        public int MinGold { get; set; }
        public int Discards { get; set; }
        public int Hands { get; set; }
        public int HandSize { get; set; }
        public int Round { get; set; }
        public int StartingRollPrice { get; set; } = 5;
        public int Ante => Round / 3;
        
        public HandTracker HandTracker { get; set; }

        public ScoreContext GetHandScore(HandRank rank)
        {
            return HandTracker.GetHandScore(rank);
        }

        public void OnVoucherBought(VoucherType voucherType)
        {
            OwnedVouchers[(int)voucherType] = true;
        }

        public void Subscribe(GameEventBus eventBus)
        {
            eventBus.SubscribeToVoucherBought(OnVoucherBought);
        }
    }
}