using Balatro.Core.CoreObjects;
using Balatro.Core.CoreObjects.BoosterPacks;
using Balatro.Core.CoreObjects.Shop.ShopContainers;
using Balatro.Core.CoreObjects.Shop.ShopObjects;
using Balatro.Core.GameEngine.Contracts;
using Balatro.Core.GameEngine.GameStateController.PhaseActions;
using Balatro.Core.GameEngine.StateController;

namespace Balatro.Core.GameEngine.GameStateController.PhaseStates
{
    public class ShopState : BaseGamePhaseState
    {
        private ShopContainer ShopContainer { get; set; }
        private VoucherContainer VoucherContainer { get; set; }
        public BoosterContainer BoosterContainer { get; set; }
        private GamePhase NextPhase { get; set; }
        private BoosterPackType? OpenedPackType { get; set; }
        public override GamePhase Phase => GamePhase.Shop;
        public ShopState(GameContext ctx) : base(ctx) { }
        
        /// <summary>
        /// Gets the number of rolls the player has paid -> Chaos free roll should not increment this counter.
        /// </summary>
        public int NumberOfRollsPaidThisTurn { get; set; }
        
        /// <summary>
        /// Gets the number of free rolls the player has (e.g. from Chaos or tags that grant free rolls).
        /// </summary>
        public int NumberOfFreeRolls { get; set; }
        
        protected override bool HandleStateSpecificAction(BasePlayerAction action)
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
                case ShopActionIntent.BuyVoucher:
                    return ExecuteBuyVoucher(shopAction.VoucherIndex);
                case ShopActionIntent.BuyFromShop:
                    return ExecuteBuyFromShop(shopAction.ShopIndex);
                case ShopActionIntent.BuyBoosterPack:
                    return ExecuteBuyBoosterPack(shopAction.BoosterPackIndex);
                default:
                    throw new ArgumentOutOfRangeException(nameof(shopAction), shopAction, null);
            }
        }

        public override void OnEnterPhase()
        {
            if (ShopContainer is null)
            {
                ShopContainer = new ShopContainer(2); // TODO: Use vouchers for this
            }
            
            FillShopContainer();
        }

        public override IGamePhaseState GetNextPhaseState()
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
                return new PlanetPackState(GameContext, this, OpenedPackType!.Value);
            }
            
            if (NextPhase == GamePhase.SpectralPack)
            {
                return new SpectralPackState(GameContext, this, OpenedPackType!.Value);
            }

            if (NextPhase == GamePhase.CardPack)
            {
                throw new NotImplementedException();
            }
            
            if (NextPhase == GamePhase.Round)
            {
                return new RoundState(GameContext);
            }

            return new BlindSelectionState(GameContext);
        }

        private bool ExecuteRoll()
        {
            int nextRollPrice;
            
            // Book-keep the roll prices
            if (NumberOfFreeRolls > 0)
            {
                NumberOfFreeRolls--;
                nextRollPrice = 0;
            }

            else
            {
                nextRollPrice = GameContext.PersistentState.StartingRollPrice + NumberOfRollsPaidThisTurn;
                NumberOfRollsPaidThisTurn++;
            }

            GameContext.PersistentState.EconomyHandler.SpendGold(nextRollPrice);
            
            // Perform the roll
            ShopContainer.ClearItems(GameContext);
            FillShopContainer();
            
            return false;
        }
        
        public bool ExecuteBuyVoucher(int voucherIndex)
        {
            var voucher = VoucherContainer.Vouchers[voucherIndex];
            VoucherContainer.Vouchers.RemoveAt(voucherIndex);
            
            var voucherPrice = GameContext.PersistentState.EconomyHandler.GetVoucherPrice();
            GameContext.PersistentState.EconomyHandler.SpendGold(voucherPrice);
            GameContext.GameEventBus.PublishVoucherBought(voucher);
            return false;
        }

        public bool ExecuteBuyFromShop(int shopIndex)
        {
            var shopItem = ShopContainer.Items[shopIndex];
            switch (shopItem.Type)
            {
                case ShopItemType.TarotCard:
                case ShopItemType.PlanetCard:
                case ShopItemType.SpectralCard:
                    GameContext.ConsumableContainer.AddConsumable(CoreObjectsFactory.CreateConsumable(shopItem));
                    break;
                case ShopItemType.Joker:
                    GameContext.JokerContainer.AddJoker(GameContext, CoreObjectsFactory.CreateJoker(shopItem));
                    break;
                case ShopItemType.PlayingCard:
                    throw new NotImplementedException("Buyable playing cards not yet added");
            }

            return false;
        }

        private int RollPrice()
        {
            if (NumberOfFreeRolls > 0)
            {
                return 0; // Free roll
            }
            
            return GameContext.PersistentState.StartingRollPrice + NumberOfRollsPaidThisTurn;
        }
        
        private bool ExecuteBuyBoosterPack(int boosterPackIndex)
        {
            var packType = BoosterContainer.BoosterPacks[boosterPackIndex].BoosterPackType;
            
            GameContext.PersistentState.EconomyHandler.SpendGold(GameContext.PersistentState.EconomyHandler.GetBoosterPackPrice(packType));
            OpenedPackType = packType;
            NextPhase = GetBoosterPackPhase(packType);
            BoosterContainer.BoosterPacks.RemoveAt(boosterPackIndex);
            return true;
        }
        
        private GamePhase GetBoosterPackPhase(BoosterPackType packType)
        {
            switch (packType)
            {
                case BoosterPackType.ArcanaNormal:
                case BoosterPackType.ArcanaJumbo:
                case BoosterPackType.ArcanaMega:
                    return GamePhase.ArcanaPack;
                
                case BoosterPackType.CelestialNormal:
                case BoosterPackType.CelestialJumbo:
                case BoosterPackType.CelestialMega:
                    return GamePhase.PlanetPack;
                
                case BoosterPackType.BuffoonNormal:
                case BoosterPackType.BuffoonJumbo:
                case BoosterPackType.BuffoonMega:
                    return GamePhase.JokerPack;
                
                case BoosterPackType.SpectralNormal:
                case BoosterPackType.SpectralJumbo:
                case BoosterPackType.SpectralMega:
                    return GamePhase.SpectralPack;
                
                case BoosterPackType.StandardNormal:
                case BoosterPackType.StandardJumbo:
                case BoosterPackType.StandardMega:
                    return GamePhase.CardPack;
            }
            
            throw new ArgumentOutOfRangeException(nameof(packType), packType, null);
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
                    if (!GameContext.PersistentState.EconomyHandler.CanSpend(RollPrice()))
                    {
                        throw new InvalidOperationException("Not enough gold to roll.");
                    }
                    break;
                
                case ShopActionIntent.BuyFromShop:
                    if (ShopContainer.Items.Count <= action.ShopIndex)
                    {
                        throw new ArgumentOutOfRangeException(nameof(action.ShopIndex), action.ShopIndex, $"Cannot buy item {action.ShopIndex} from shop, item does not exist");
                    }

                    if (!GameContext.PersistentState.EconomyHandler.CanBuy(ShopContainer.Items[action.ShopIndex]))
                    {
                        throw new InvalidOperationException("Not enough gold to buy item.");
                    }
                    break;
                
                case ShopActionIntent.BuyVoucher:
                    if (VoucherContainer.Vouchers.Count <= action.VoucherIndex)
                    {
                        throw new ArgumentOutOfRangeException(nameof(action.VoucherIndex), action.VoucherIndex, "Voucher index is out of range.");
                    }

                    if (!GameContext.PersistentState.EconomyHandler.CanBuyVoucher())
                    {
                        throw new InvalidOperationException("Not enough gold to buy voucher.");
                    }
                    break;
                
                case ShopActionIntent.BuyBoosterPack:
                    if (BoosterContainer.BoosterPacks.Count <= action.BoosterPackIndex)
                    {
                        throw new ArgumentOutOfRangeException(nameof(action.BoosterPackIndex), action.BoosterPackIndex, "Booster pack index is out of range.");
                    }

                    if (!GameContext.PersistentState.EconomyHandler.CanBuy(BoosterContainer
                            .BoosterPacks[action.BoosterPackIndex].BoosterPackType))
                    {
                        throw new InvalidOperationException("Not enough gold to buy booster pack.");
                    }
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException(nameof(action.ActionIntent), action.ActionIntent, null);
            }
        }
    }
}