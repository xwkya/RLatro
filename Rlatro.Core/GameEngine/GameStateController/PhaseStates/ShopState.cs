using Balatro.Core.CoreObjects.BoosterPacks;
using Balatro.Core.CoreObjects.Consumables.ConsumableObject;
using Balatro.Core.CoreObjects.Jokers.Joker;
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
        private ShopContainer ShopContainer { get; set; }
        private VoucherContainer VoucherContainer { get; set; }
        public BoosterContainer BoosterContainer { get; set; }
        private GamePhase NextPhase { get; set; }
        private BoosterPackType? OpenedPackType { get; set; }
        
        public ShopState(GameContext ctx)
        {
            GameContext = ctx;
        }
        public GamePhase Phase => GamePhase.Shop;
        
        /// <summary>
        /// Gets the number of rolls the player has paid -> Chaos free roll should not increment this counter.
        /// </summary>
        public int NumberOfRollsPaidThisTurn { get; set; }
        
        /// <summary>
        /// Gets the number of free rolls the player has (e.g. from Chaos or tags that grant free rolls).
        /// </summary>
        public int NumberOfFreeRolls { get; set; }
        
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
                case ShopActionIntent.SellJoker:
                    return ExecuteSellJoker(shopAction.JokerIndex);
                case ShopActionIntent.BuyVoucher:
                    return ExecuteBuyVoucher(shopAction.VoucherIndex);
                case ShopActionIntent.BuyFromShop:
                    return ExecuteBuyFromShop(shopAction.ShopIndex);
                case ShopActionIntent.SellConsumable:
                    return ExecuteSellConsumable(shopAction.ConsumableIndex);
                case ShopActionIntent.BuyBoosterPack:
                    return ExecuteBuyBoosterPack(shopAction.BoosterPackIndex);
                default:
                    throw new ArgumentOutOfRangeException(nameof(shopAction), shopAction, null);
            }
        }

        public IGamePhaseState GetNextPhaseState()
        {
            // Internal verification
            if (NextPhase != GamePhase.BlindSelection && !OpenedPackType.HasValue)
                throw new ApplicationException(
                    "Next phase is a pack opening but the pack type has not been set. This is a bug.");
                    
            if (NextPhase == GamePhase.ArcanaPack)
            {
                return new ArcanaPackState(GameContext, this, OpenedPackType!.Value);
            }
            
            if (NextPhase == GamePhase.JokerPack)
            {
                return new JokerPackState(GameContext, this, OpenedPackType!.Value);
            }

            if (NextPhase == GamePhase.PlanetPack)
            {
                return new PlanetPackState(this);
            }
            
            if (NextPhase == GamePhase.SpectralPack)
            {
                return new SpectralPackState(this);
            }
            
            if (NextPhase == GamePhase.Round)
            {
                return new RoundState(GameContext);
            }

            return new BlindSelectionState(GameContext);
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

        public bool ExecuteBuyFromShop(int shopIndex)
        {
            var shopItem = ShopContainer.Items[shopIndex];
            switch (shopItem.ShopItemType)
            {
                case ShopItemType.TarotCard:
                case ShopItemType.PlanetCard:
                case ShopItemType.SpectralCard:
                    GameContext.ConsumableContainer.AddConsumable((Consumable)shopItem);
                    break;
                case ShopItemType.Joker:
                    GameContext.JokerContainer.AddJoker(GameContext, (JokerObject)shopItem);
                    break;
                case ShopItemType.PlayingCard:
                    throw new NotImplementedException("Buyable playing cards not yet added");
            }

            return false;
        }
        
        private bool ExecuteBuyBoosterPack(int boosterPackIndex)
        {
            throw new NotImplementedException();
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
                case ShopActionIntent.BuyBoosterPack:
                    // TODO: Implement booster packs
                default:
                    throw new ArgumentOutOfRangeException(nameof(action.ActionIntent), action.ActionIntent, null);
            }
        }
    }
}