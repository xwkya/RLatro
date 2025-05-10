using Balatro.Core.CoreObjects.Shop.ShopContainers;
using Balatro.Core.GameEngine.GameStateController.PhaseActions;
using Balatro.Core.GameEngine.StateController;

namespace Balatro.Core.GameEngine.GameStateController.PhaseStates
{
    public class ShopState : IGamePhaseState
    {
        private const int VoucherPrice = 10;
        public GameContext GameContext { get; set; }
        public ShopContainer ShopContainer { get; set; }
        public VoucherContainer VoucherContainer { get; set; }
        public BoosterContainer BoosterContainer { get; set; }
        public ShopState(GameContext ctx)
        {
            GameContext = ctx;
        }
        public GamePhase Phase => GamePhase.Shop;
        
        /// <summary>
        /// Gets the number of rolls the player has paid -> Chaos free roll should not increment this counter.
        /// </summary>
        public byte NumberOfRollsPaidThisTurn { get; set; }
        
        /// <summary>
        /// Gets the number of free rolls the player has (e.g. from Chaos or tags that grant free rolls).
        /// Theoretically never bigger than 2 so could be a 3 bit packed field.
        /// </summary>
        public byte NumberOfFreeRolls { get; set; }

        public bool IsPhaseOver { get; }
        
        public bool HandleAction(BasePlayerAction action)
        {
            if (action is not ShopAction shopAction)
            {
                throw new ArgumentException($"Action {action} is not a {nameof(ShopAction)}.");
            }
            
            ValidatePossibleAction(shopAction);
            
            switch (shopAction.ActionIntent)
            {
                case ShopActionIntent.Roll:
                    return ExecuteRoll(shopAction);
                case ShopActionIntent.BuyFromShop:
                    return ExecuteBuyFromShop(shopAction);
                case ShopActionIntent.SellJoker:
                    return ExecuteSellJoker(shopAction.JokerIndex);
                case ShopActionIntent.BuyVoucher:
                    return ExecuteBuyVoucher(shopAction.VoucherIndex);
                default:
                    throw new ArgumentOutOfRangeException(nameof(shopAction), shopAction, null);
            }
        }
        
        private bool ExecuteRoll(ShopAction action)
        {
            if (NumberOfFreeRolls > 0)
            {
                NumberOfFreeRolls--;
            }

            GameContext.PersistentState.Gold -= (GameContext.PersistentState.StartingRollPrice + NumberOfRollsPaidThisTurn);
            NumberOfRollsPaidThisTurn++;
            
            // TODO: Implement roll logic
            return false;
        }
        
        private bool ExecuteSellJoker(int jokerIndex)
        {
            var joker = GameContext.JokerContainer.Jokers[jokerIndex];
            var sellValue = ComputationHelpers.ComputeSellValue(GameContext, joker.BaseSellValue, joker.BonusSellValue);
            GameContext.JokerContainer.RemoveJoker(GameContext, jokerIndex);

            GameContext.PersistentState.Gold += sellValue;
            return false;
        }
        
        public bool ExecuteBuyVoucher(int voucherIndex)
        {
            var voucher = VoucherContainer.Vouchers[voucherIndex];
            VoucherContainer.Vouchers.RemoveAt(voucherIndex);
            
            GameContext.PersistentState.Gold -= VoucherPrice;
            GameContext.GameEventBus.PublishVoucherBought(voucher);
            return false;
        }

        private void FillShopContainer()
        {
            
        }
        
        private void ValidatePossibleAction(ShopAction action)
        {
            switch (action.ActionIntent)
            {
                case ShopActionIntent.Roll:
                    if (GameContext.PersistentState.Gold < GameContext.PersistentState.MinGold)
                    {
                        throw new InvalidOperationException("Not enough gold to roll.");
                    }
                    break;
                case ShopActionIntent.BuyFromShop:
                    if (ShopContainer.Items.Count <= action.ShopIndex)
                    {
                        throw new ArgumentOutOfRangeException(nameof(action.ShopIndex), action.ShopIndex, $"Cannot buy item {action.ShopIndex} from shop, item does not exist");
                    }

                    if (GameContext.PersistentState.Gold - ShopContainer.Items[action.ShopIndex].BaseCost <
                        GameContext.PersistentState.MinGold)
                    {
                        throw new InvalidOperationException("Not enough gold to buy item.");
                    }
                    break;
                case ShopActionIntent.SellJoker:
                    if (GameContext.JokerContainer.Jokers.Count <= action.JokerIndex)
                    {
                        throw new ArgumentOutOfRangeException(nameof(action.JokerIndex), action.JokerIndex, "Cannot sell joker");
                    }
                    break;
                case ShopActionIntent.BuyVoucher:
                    if (VoucherContainer.Vouchers.Count <= action.VoucherIndex)
                    {
                        throw new ArgumentOutOfRangeException(nameof(action.VoucherIndex), action.VoucherIndex, $"Voucher {action.VoucherIndex} does not exist");;
                    }
                    if (GameContext.PersistentState.Gold - VoucherPrice < GameContext.PersistentState.MinGold)
                    {
                        throw new InvalidOperationException("Not enough gold to buy voucher.");
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(action.ActionIntent), action.ActionIntent, null);
            }
        }
    }
}