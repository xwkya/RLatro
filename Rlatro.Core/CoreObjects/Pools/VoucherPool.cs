using Balatro.Core.CoreObjects.Vouchers;
using Balatro.Core.GameEngine.GameStateController;
using Balatro.Core.GameEngine.GameStateController.EventBus;
using Balatro.Core.GameEngine.PseudoRng;

namespace Balatro.Core.CoreObjects.Pools
{
    /// <summary>
    /// Tracks all available vouchers.
    /// </summary>
    public sealed class VoucherPool : IEventBusSubscriber
    {
        private GameContext GameContext { get; }
        private readonly bool[] AvailableVouchers = new bool[Enum.GetValues<VoucherType>().Length];
        private int NumberOfAvailableVouchers;
        
        /// <summary>
        /// Initialize the voucher pool with the already owned vouchers.
        /// </summary>
        public VoucherPool(GameContext ctx)
        {
            GameContext = ctx;
        }
        
        public void InitializePool()
        {
            NumberOfAvailableVouchers = AvailableVouchers.Length / 2;
            
            // Set all base vouchers to true.
            for (int i = 0; i < AvailableVouchers.Length/2; i++)
            {
                AvailableVouchers[2 * i] = true;
            }
            
            // Check each owned voucher
            for(var i = 0; i < GameContext.PersistentState.OwnedVouchers.Length; i++)
            {
                if (!GameContext.PersistentState.OwnedVouchers[i]) continue;
                var voucher = (VoucherType)i;
                
                // If the voucher is a base voucher, we need to unlock the next one
                // and mark the current one as unavailable.
                if (voucher.IsBaseVoucher())
                {
                    AvailableVouchers[(int)voucher] = false;
                    AvailableVouchers[(int)voucher + 1] = true;
                }
                // If the voucher is already the upgraded voucher
                // the first voucher must have already been bought hence both are unavailable.
                else
                {
                    AvailableVouchers[(int)voucher] = false;
                    AvailableVouchers[(int)voucher - 1] = false;
                    NumberOfAvailableVouchers--; // We have one less available voucher.
                }
            }
        }
        
        public void Subscribe(GameEventBus eventBus)
        {
            eventBus.SubscribeToVoucherBought(OnVoucherBought);
        }
        
        public VoucherType[] GetTagVoucher(int numberOfVouchers, RngController rng)
        {
            Span<int> availableVouchers = stackalloc int[NumberOfAvailableVouchers];
            int c = 0;
            for (int i = 0; i < AvailableVouchers.Length; i++)
            {
                if (AvailableVouchers[i])
                {
                    availableVouchers[c] = i;
                    c++;
                }
            }
            
            // Shuffle the available vouchers
            rng.GetShuffle(availableVouchers, RngActionType.GetTagVouchers);
            var vouchers = new VoucherType[numberOfVouchers];
            for (int i = 0; i < numberOfVouchers; i++)
            {
                // Get the voucher type from the shuffled list
                vouchers[i] = (VoucherType)availableVouchers[i];
            }
            
            return vouchers;
        }

        public VoucherType GetNewAnteVoucher(RngController rng)
        {
            Span<int> availableVouchers = stackalloc int[NumberOfAvailableVouchers];
            int c = 0;
            for (int i = 0; i < AvailableVouchers.Length; i++)
            {
                if (AvailableVouchers[i])
                {
                    availableVouchers[c] = i;
                    c++;
                }
            }
            
            // Sample 1 random element (the vouchers are already sorted)
            var voucher = rng.GetRandomElement(availableVouchers, RngActionType.GetSingleVoucher);
            return (VoucherType)voucher;
        }

        private void OnVoucherBought(VoucherType type)
        {
            AvailableVouchers[(int)type] = false;
            if (type.IsBaseVoucher())
            {
                AvailableVouchers[(int)type + 1] = true;
            }
            else
            {
                NumberOfAvailableVouchers--;
            }
        }
    }
}