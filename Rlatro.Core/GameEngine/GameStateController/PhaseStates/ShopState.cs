using Balatro.Core.CoreObjects.Shop.ShopContainers;
using Balatro.Core.CoreObjects.Shop.ShopObjects;
using Balatro.Core.GameEngine.GameStateController.PhaseActions;
using Balatro.Core.GameEngine;
using Balatro.Core.GameEngine.StateController;

namespace Balatro.Core.GameEngine.GameStateController.PhaseStates
{
    public class ShopState : IGamePhaseState
    {
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
                    return ExecuteRoll();
                case ShopActionIntent.BuyFromShop:
                    //return ExecuteBuyFromShop(shopAction);
                case ShopActionIntent.SellJoker:
                    return ExecuteSellJoker(shopAction.JokerIndex);
                case ShopActionIntent.BuyVoucher:
                    return ExecuteBuyVoucher(shopAction.VoucherIndex);
                default:
                    throw new ArgumentOutOfRangeException(nameof(shopAction), shopAction, null);
            }
        }
        
        private bool ExecuteRoll()
        {
            // Book-keep the roll prices
            if (NumberOfFreeRolls > 0)
            {
                NumberOfFreeRolls--;
            }

            GameContext.PersistentState.Gold -= (GameContext.PersistentState.StartingRollPrice + NumberOfRollsPaidThisTurn);
            NumberOfRollsPaidThisTurn++;
            
            // Perform the roll
            ShopContainer.ClearItems(GameContext);
            FillShopContainer();
            
            return false;
        }
        
        private bool ExecuteSellJoker(int jokerIndex)
        {
            var joker = GameContext.JokerContainer.Jokers[jokerIndex];
            var sellValue = GameContext.PriceManager.GetSellPrice(joker);
            
            // Remove the joker from the game context and the joker container
            GameContext.GameEventBus.PublishJokerRemovedFromContext(joker.StaticId);
            GameContext.JokerContainer.RemoveJoker(GameContext, jokerIndex);
        
            // Add the sell value to the player's gold
            GameContext.PersistentState.Gold += sellValue;
            return false;
        }
        
        private bool ExecuteSellConsumable(byte consumableIndex)
        {
            var consumable = GameContext.ConsumableContainer.Consumables[consumableIndex];
            var sellValue = GameContext.PriceManager.GetSellPrice(consumable);
            
            GameContext.GameEventBus.PublishConsumableRemovedFromContext(consumableIndex);
            GameContext.ConsumableContainer.RemoveConsumable(consumableIndex);

            GameContext.PersistentState.Gold += sellValue;
            return false;
        }
        
        public bool ExecuteBuyVoucher(int voucherIndex)
        {
            var voucher = VoucherContainer.Vouchers[voucherIndex];
            VoucherContainer.Vouchers.RemoveAt(voucherIndex);
            
            GameContext.PersistentState.Gold -= GameContext.PriceManager.GetBuyPrice(voucher);
            GameContext.GameEventBus.PublishVoucherBought(voucher);
            return false;
        }

        private void FillShopContainer()
        {
            for (int i = 0; i < ShopContainer.Capacity; i++)
            {
                var item = GameContext.GlobalPoolManager.GenerateShopItem();
                ShopContainer.Items.Add(item);
            }
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

                    var itemCost = GameContext.PriceManager.GetBuyPrice(ShopContainer.Items[action.ShopIndex]);
                    if (GameContext.PersistentState.Gold - itemCost <
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
                    var voucherCost = GameContext.PriceManager.GetBuyPrice(VoucherContainer.Vouchers[action.VoucherIndex]);
                    if (GameContext.PersistentState.Gold - voucherCost < GameContext.PersistentState.MinGold)
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