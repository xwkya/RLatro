using System.Text;
using Balatro.Core.CoreObjects.Cards.CardObject;
using Balatro.Core.CoreObjects.Consumables.ConsumableObject;
using Balatro.Core.CoreObjects.Registries;
using Balatro.Core.CoreObjects.Shop.ShopObjects;
using Balatro.Core.GameEngine.GameStateController;
using Balatro.Core.GameEngine.GameStateController.PhaseStates;

namespace Balatro.Core.Display.Components
{
    public static class SimplePackDisplayComponent
    {
        public static void DisplayArcanaPackState(StringBuilder sb, GameContext gameContext, ArcanaPackState packState)
        {
            sb.AppendLine("=== ARCANA PACK ===");
            sb.AppendLine($"Gold: ${gameContext.PersistentState.EconomyHandler.GetCurrentGold()}");
            
            var arcanaCards = packState.GetArcanaCards();
            DisplayConsumableCards(sb, arcanaCards, "Tarot");
            
            sb.AppendLine();
            sb.AppendLine("Commands: 'c [index] [targets...]' (use card), 's' (skip pack)");
            sb.AppendLine();
        }

        public static void DisplaySpectralPackState(StringBuilder sb, GameContext gameContext, SpectralPackState packState)
        {
            sb.AppendLine("=== SPECTRAL PACK ===");
            sb.AppendLine($"Gold: ${gameContext.PersistentState.EconomyHandler.GetCurrentGold()}");
            
            var spectralCards = packState.GetSpectralCards();
            DisplayConsumableCards(sb, spectralCards, "Spectral");
            
            sb.AppendLine();
            sb.AppendLine("Commands: 'c [index] [targets...]' (use card), 's' (skip pack)");
            sb.AppendLine();
        }

        public static void DisplayPlanetPackState(StringBuilder sb, GameContext gameContext, PlanetPackState packState)
        {
            sb.AppendLine("=== CELESTIAL PACK ===");
            sb.AppendLine($"Gold: ${gameContext.PersistentState.EconomyHandler.GetCurrentGold()}");
            
            var planetCards = packState.GetPlanetCards();
            DisplayShopItems(sb, planetCards, "Planet");
            
            sb.AppendLine();
            sb.AppendLine("Commands: 'c [index]' (choose planet), 's' (skip pack)");
            sb.AppendLine();
        }

        public static void DisplayJokerPackState(StringBuilder sb, GameContext gameContext, JokerPackState packState)
        {
            sb.AppendLine("=== BUFFOON PACK ===");
            sb.AppendLine($"Gold: ${gameContext.PersistentState.EconomyHandler.GetCurrentGold()}");
            
            var jokerObjects = packState.GetJokerObjects();
            DisplayShopItems(sb, jokerObjects, "Joker");
            
            sb.AppendLine();
            sb.AppendLine("Commands: 'c [index]' (choose joker), 's' (skip pack)");
            sb.AppendLine();
        }

        public static void DisplayCardPackState(StringBuilder sb, GameContext gameContext, CardPackState packState)
        {
            sb.AppendLine("=== STANDARD PACK ===");
            sb.AppendLine($"Gold: ${gameContext.PersistentState.EconomyHandler.GetCurrentGold()}");
            
            var packCards = packState.GetPackCards();
            DisplayPlayingCards(sb, packCards);
            
            sb.AppendLine();
            sb.AppendLine("Commands: 'c [index]' (choose card), 's' (skip pack)");
            sb.AppendLine();
        }

        private static void DisplayConsumableCards(StringBuilder sb, List<Consumable> cards, string cardType)
        {
            if (cards.Count == 0)
            {
                sb.AppendLine($"{cardType} Cards: (None)");
                return;
            }

            sb.Append($"{cardType} Cards: ");
            var cardNames = new List<string>();
            for (int i = 0; i < cards.Count; i++)
            {
                cardNames.Add($"[{i}] {cards[i].GetType().Name}");
            }
            sb.AppendLine(string.Join(", ", cardNames));
        }

        private static void DisplayShopItems(StringBuilder sb, List<ShopItem> items, string itemType)
        {
            if (items.Count == 0)
            {
                sb.AppendLine($"{itemType} Cards: (None)");
                return;
            }

            sb.Append($"{itemType} Cards: ");
            var itemNames = new List<string>();
            for (int i = 0; i < items.Count; i++)
            {
                var name = GetItemNameByType(items[i].StaticId, itemType);
                itemNames.Add($"[{i}] {name}");
            }
            sb.AppendLine(string.Join(", ", itemNames));
        }

        private static void DisplayPlayingCards(StringBuilder sb, List<Card64> cards)
        {
            if (cards.Count == 0)
            {
                sb.AppendLine("Playing Cards: (None)");
                return;
            }

            sb.Append("Playing Cards: ");
            var cardNames = new List<string>();
            for (int i = 0; i < cards.Count; i++)
            {
                var representation = cards[i].Representation();
                // Simplify the representation to just rank and suit
                var parts = representation.Split(' ');
                var shortName = parts.Length >= 2 ? $"{parts[0]}{parts[1][0]}" : representation;
                cardNames.Add($"[{i}] {shortName}");
            }
            sb.AppendLine(string.Join(", ", cardNames));
        }

        private static string GetItemNameByType(int staticId, string itemType)
        {
            try
            {
                return itemType switch
                {
                    "Joker" => JokerRegistry.GetType(staticId).Name,
                    "Planet" => ConsumableRegistry.GetType(staticId).Name,
                    _ => $"{itemType}_{staticId}"
                };
            }
            catch
            {
                return $"{itemType}_{staticId}";
            }
        }
    }
}