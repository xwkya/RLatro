using Balatro.Core.CoreObjects;
using Balatro.Core.CoreObjects.BoosterPacks;
using Balatro.Core.CoreObjects.Shop.ShopContainers;
using Balatro.Core.CoreObjects.Shop.ShopObjects;
using Balatro.Core.CoreObjects.Vouchers;
using Balatro.Core.GameEngine.Contracts;
using Balatro.Core.GameEngine.GameStateController.PhaseActions;
using Balatro.Core.GameEngine.PseudoRng;
using Balatro.Core.GameEngine.StateController;

namespace Balatro.Core.GameEngine.GameStateController.PhaseStates
{
    public class ShopState : BaseGamePhaseState
    {
        public ShopContainer ShopContainer { get; set; }
        public VoucherContainer VoucherContainer { get; set; }
        public BoosterContainer BoosterContainer { get; set; }
        private GamePhase NextPhase { get; set; }
        private BoosterPackType? OpenedPackType { get; set; }
        public override GamePhase Phase => GamePhase.Shop;
        public override bool ShouldInitializeNextState => true;
        private VoucherType CurrentAnteVoucher { get; set; }

        public ShopState(GameContext ctx) : base(ctx)
        {
            ShopContainer = new ShopContainer(2); // TODO: Use vouchers for this
            VoucherContainer = new VoucherContainer();
            BoosterContainer = new BoosterContainer();
        }
        
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
                case ShopActionIntent.NextPhase:
                    NextPhase = GamePhase.BlindSelection;
                    return true;
                default:
                    throw new ArgumentOutOfRangeException(nameof(shopAction), shopAction, null);
            }
        }

        protected override void ResetPhaseSpecificState()
        {
            NumberOfRollsPaidThisTurn = 0;
            NumberOfFreeRolls = 0;
            OpenedPackType = null;
            NextPhase = GamePhase.BlindSelection; // Default next phase
            ShopContainer.ClearItems(GameContext);
            BoosterContainer.BoosterPacks.Clear();
            VoucherContainer.Vouchers.Clear();
            CurrentAnteVoucher = GameContext.GlobalPoolManager.GetNewAnteVoucher();
        }

        public override void OnEnterPhase()
        {
            NumberOfRollsPaidThisTurn = 0;
            NumberOfFreeRolls = 0; // Reset free rolls at the start of the shop phase
            
            // TODO: NumberOfFreeRolls = GameContext.JokerContainer.Jokers.Any(x => x.StaticId == JokerRegistry.GetStaticId(typeof(ChaosTheClown)));
            FillShopContainer();
            FillBoosterPackContainer();
            FillVoucherContainer();
            CreateBlindsIfNewAnte();
        }

        public override void OnExitPhase()
        {
            ShopContainer.ClearItems(GameContext);
            BoosterContainer.BoosterPacks.Clear();
            GameContext.PersistentState.FirstShopHasBeenVisited = true;
        }
        
        public override IGamePhaseState GetNextPhaseState()
        {
            // Internal verification
            if (NextPhase != GamePhase.BlindSelection && !OpenedPackType.HasValue)
                throw new ApplicationException(
                    "Next phase is a pack opening but the pack type has not been set. This is a bug.");
                    
            if (NextPhase == GamePhase.ArcanaPack)
            {
                var arcanaPackState = GameContext.GetPhase<ArcanaPackState>();

                arcanaPackState.SetIncomingState(this)
                    .SetPackType(OpenedPackType!.Value);

                return arcanaPackState;
            }
            
            if (NextPhase == GamePhase.JokerPack)
            {
                var jokerPackState = GameContext.GetPhase<JokerPackState>();

                jokerPackState.SetIncomingState(this)
                    .SetPackType(OpenedPackType!.Value);

                return jokerPackState;
            }

            if (NextPhase == GamePhase.PlanetPack)
            {
                var planetPackState = GameContext.GetPhase<PlanetPackState>();

                planetPackState.SetIncomingState(this)
                    .SetPackType(OpenedPackType!.Value);

                return planetPackState;
            }
            
            if (NextPhase == GamePhase.SpectralPack)
            {
                var spectralPackState = GameContext.GetPhase<SpectralPackState>();

                spectralPackState.SetIncomingState(this)
                    .SetPackType(OpenedPackType!.Value);

                return spectralPackState;
            }

            if (NextPhase == GamePhase.CardPack)
            {
                var cardPackState = GameContext.GetPhase<CardPackState>();

                cardPackState.SetIncomingState(this)
                    .SetPackType(OpenedPackType!.Value);

                return cardPackState;
            }

            // Otherwise we use a round state
            return GameContext.GetPhase<BlindSelectionState>();
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

        private void FillBoosterPackContainer()
        {
            var appearanceRates = GameContext.PersistentState.AppearanceRates;
            
            for (int i = 0; i < BoosterContainer.BoosterPackSlots; i++)
            {
                BoosterPackType selectedPackType;
                // Enforce a buffoon pack if it's the first shop of the game
                if (!GameContext.PersistentState.FirstShopHasBeenVisited && i == 0)
                {
                    selectedPackType = BoosterPackType.BuffoonNormal;
                }
                else
                {
                    selectedPackType = SelectWeightedBoosterPack(appearanceRates.GetAllBoosterPackWeights());
                }
                var boosterPack = new BoosterPack { BoosterPackType = selectedPackType };
                BoosterContainer.AddPack(boosterPack);
            }
        }
        
        private BoosterPackType SelectWeightedBoosterPack(IReadOnlyDictionary<BoosterPackType, float> weights)
        {
            var randomValue = GameContext.RngController.GetRandomProbability(RngActionType.RandomPack);
            
            float currentWeight = 0f;
            foreach (var kvp in weights)
            {
                currentWeight += kvp.Value;
                if (randomValue <= currentWeight)
                {
                    return kvp.Key;
                }
            }

            throw new ApplicationException("Failed to select a weighted booster pack. This should never happen.");
        }

        private void FillVoucherContainer()
        {
            // Clear all to prevent vouchers from tags to persist
            VoucherContainer.Vouchers.Clear();
            
            // Create a new voucher if it's round 1
            if (GameContext.PersistentState.Round % 3 == 1)
            {
                CurrentAnteVoucher = GameContext.GlobalPoolManager.GetNewAnteVoucher();
            }
            
            VoucherContainer.Vouchers.Add(CurrentAnteVoucher);
        }
        
        private void CreateBlindsIfNewAnte()
        {
            // TODO: Create boss blind if it's a new ante
            if (GameContext.PersistentState.Round % 3 == 1)
            {
                var blindState = GameContext.GetPhase<BlindSelectionState>();
                blindState.GenerateAnteTags();
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
                case ShopActionIntent.NextPhase:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(action.ActionIntent), action.ActionIntent, null);
            }
        }
    }
}