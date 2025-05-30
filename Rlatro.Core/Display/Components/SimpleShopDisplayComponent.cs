using System.Text;
using Balatro.Core.CoreObjects.Registries;
using Balatro.Core.CoreObjects.Shop.ShopContainers;
using Balatro.Core.CoreObjects.Shop.ShopObjects;
using Balatro.Core.GameEngine.GameStateController;
using Balatro.Core.GameEngine.GameStateController.PhaseStates;

namespace Balatro.Core.Display.Components
{
    public static class SimpleShopDisplayComponent
    {
        public static void DisplayShopState(StringBuilder sb, GameContext gameContext, ShopState shopState)
        {
            sb.AppendLine("=== SHOP ===");
            sb.AppendLine($"Gold: ${gameContext.PersistentState.EconomyHandler.GetCurrentGold()} | Roll Cost: ${GetRollPrice(gameContext, shopState)}");
            
            DisplayShopItems(sb, gameContext, shopState.ShopContainer);
            DisplayBoosterPacks(sb, gameContext, shopState.BoosterContainer);
            DisplayVouchers(sb, gameContext, shopState.VoucherContainer);
            
            sb.AppendLine();
            sb.AppendLine(
                "Commands: 'r' (roll), 'bi [index]' (buy item), 'bp [index]' (buy pack), 'bv [index]' (buy voucher), 'n' (next phase)");
            sb.AppendLine();
        }

        private static void DisplayShopItems(StringBuilder sb, GameContext gameContext, ShopContainer shopContainer)
        {
            if (shopContainer?.Items?.Count > 0)
            {
                sb.Append("Shop Items: ");
                var items = new List<string>();
                
                for (int i = 0; i < shopContainer.Items.Count; i++)
                {
                    var item = shopContainer.Items[i];
                    var price = gameContext.PersistentState.EconomyHandler.GetShopItemPrice(item);
                    var canAfford = gameContext.PersistentState.EconomyHandler.CanBuy(item);
                    var affordIndicator = canAfford ? "" : "*";
                    var name = GetItemName(item);
                    
                    items.Add($"[{i}] {name} (${price}){affordIndicator}");
                }
                
                sb.AppendLine(string.Join(", ", items));
            }
            else
            {
                sb.AppendLine("Shop Items: (Empty)");
            }
        }

        private static void DisplayBoosterPacks(StringBuilder sb, GameContext gameContext, BoosterContainer boosterContainer)
        {
            if (boosterContainer?.BoosterPacks?.Count > 0)
            {
                sb.Append("Available Packs: ");
                var packs = new List<string>();
                
                for (int i = 0; i < boosterContainer.BoosterPacks.Count; i++)
                {
                    var pack = boosterContainer.BoosterPacks[i];
                    var price = gameContext.PersistentState.EconomyHandler.GetBoosterPackPrice(pack.BoosterPackType);
                    var canAfford = gameContext.PersistentState.EconomyHandler.CanBuy(pack.BoosterPackType);
                    var affordIndicator = canAfford ? "" : "*";
                    
                    packs.Add($"[{i}] {pack.BoosterPackType} (${price}){affordIndicator}");
                }
                
                sb.AppendLine(string.Join(", ", packs));
            }
            else
            {
                sb.AppendLine("Available Packs: (None)");
            }
        }

        private static void DisplayVouchers(StringBuilder sb, GameContext gameContext, VoucherContainer voucherContainer)
        {
            if (voucherContainer?.Vouchers?.Count > 0)
            {
                sb.Append("Vouchers: ");
                var vouchers = new List<string>();
                
                for (int i = 0; i < voucherContainer.Vouchers.Count; i++)
                {
                    var voucher = voucherContainer.Vouchers[i];
                    var price = gameContext.PersistentState.EconomyHandler.GetVoucherPrice();
                    var canAfford = gameContext.PersistentState.EconomyHandler.CanBuyVoucher();
                    var affordIndicator = canAfford ? "" : "*";
                    
                    vouchers.Add($"[{i}] {voucher} (${price}){affordIndicator}");
                }
                
                sb.AppendLine(string.Join(", ", vouchers));
            }
            else
            {
                sb.AppendLine("Vouchers: (None)");
            }
        }

        private static string GetItemName(ShopItem item)
        {
            try
            {
                return item.Type switch
                {
                    ShopItemType.Joker => $"{JokerRegistry.GetType(item.StaticId).Name} (edition: {item.Edition})",
                    ShopItemType.TarotCard or ShopItemType.PlanetCard or ShopItemType.SpectralCard 
                        => ConsumableRegistry.GetType(item.StaticId).Name,
                    ShopItemType.PlayingCard => "PlayingCard",
                    _ => item.Type.ToString()
                };
            }
            catch
            {
                return item.Type.ToString();
            }
        }

        private static int GetRollPrice(GameContext gameContext, ShopState shopState)
        {
            if (shopState.NumberOfFreeRolls > 0)
                return 0;
            return gameContext.PersistentState.StartingRollPrice + shopState.NumberOfRollsPaidThisTurn;
        }
    }
}