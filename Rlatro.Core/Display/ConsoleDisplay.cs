using System.Text;
using Balatro.Core.Contracts.Display;
using Balatro.Core.Display.Components;
using Balatro.Core.GameEngine.Contracts;
using Balatro.Core.GameEngine.GameStateController;
using Balatro.Core.GameEngine.GameStateController.PhaseStates;
using Balatro.Core.GameEngine.StateController;

namespace Balatro.Core.Display
{
    public class ConsoleGameDisplay : IGameDisplay
    {
        public void DisplayGameState(GameContext gameContext, IGamePhaseState currentState)
        {
            Clear();
            
            var sb = new StringBuilder();
            sb.AppendLine("=== BALATRO ===");
            sb.AppendLine($"Phase: {currentState.Phase} | Gold: ${gameContext.PersistentState.EconomyHandler.GetCurrentGold()} | Round: {gameContext.PersistentState.Round} | Ante: {gameContext.PersistentState.Ante}");
            sb.AppendLine();

            // Display phase-specific information
            switch (currentState.Phase)
            {
                case GamePhase.Round:
                    DisplayRoundState(sb, gameContext, currentState as RoundState);
                    break;
                case GamePhase.Shop:
                    SimpleShopDisplayComponent.DisplayShopState(sb, gameContext, currentState as ShopState);
                    break;
                case GamePhase.ArcanaPack:
                    SimplePackDisplayComponent.DisplayArcanaPackState(sb, gameContext, currentState as ArcanaPackState);
                    break;
                case GamePhase.SpectralPack:
                    SimplePackDisplayComponent.DisplaySpectralPackState(sb, gameContext, currentState as SpectralPackState);
                    break;
                case GamePhase.PlanetPack:
                    SimplePackDisplayComponent.DisplayPlanetPackState(sb, gameContext, currentState as PlanetPackState);
                    break;
                case GamePhase.JokerPack:
                    SimplePackDisplayComponent.DisplayJokerPackState(sb, gameContext, currentState as JokerPackState);
                    break;
                case GamePhase.CardPack:
                    SimplePackDisplayComponent.DisplayCardPackState(sb, gameContext, currentState as CardPackState);
                    break;
                case GamePhase.BlindSelection:
                    SimpleBlindSelectionDisplayComponent.DisplayBlindSelectionState(sb, gameContext, currentState as BlindSelectionState);
                    break;
                default:
                    sb.AppendLine($"[{currentState.Phase} - display not implemented]");
                    break;
            }

            // Always show jokers and consumables compactly
            DisplayJokersCompact(sb, gameContext);
            DisplayConsumablesCompact(sb, gameContext);

            Console.WriteLine(sb.ToString());
        }

        private void DisplayRoundState(StringBuilder sb, GameContext gameContext, RoundState roundState)
        {
            sb.AppendLine("=== ROUND ===");
            sb.AppendLine($"Hands: {roundState?.Hands ?? gameContext.GetHands()} | Discards: {roundState?.Discards ?? gameContext.GetDiscards()} | Score: {roundState?.CurrentChipsScore ?? 0}/{roundState?.CurrentChipsRequirement ?? 0}");
            sb.AppendLine();

            // Display hand compactly
            sb.Append("Hand: ");
            if (gameContext.Hand.Count == 0)
            {
                sb.AppendLine("(Empty)");
            }
            else
            {
                var handCards = new List<string>();
                for (int i = 0; i < gameContext.Hand.Count; i++)
                {
                    var card = gameContext.Hand.Span[i];
                    var parts = card.Representation().Split(' ');
                    var shortName = parts.Length >= 2 ? $"{parts[0]}{parts[1][0]}" : card.ToString();
                    handCards.Add($"[{i}] {shortName}");
                }
                sb.AppendLine(string.Join(", ", handCards));
            }

            // Display played cards if any
            if (gameContext.PlayContainer.Count > 0)
            {
                sb.Append("Played: ");
                var playedCards = new List<string>();
                for (int i = 0; i < gameContext.PlayContainer.Count; i++)
                {
                    var card = gameContext.PlayContainer.Span[i];
                    var parts = card.Representation().Split(' ');
                    var shortName = parts.Length >= 2 ? $"{parts[0]}{parts[1][0]}" : card.ToString();
                    playedCards.Add(shortName);
                }
                sb.AppendLine(string.Join(", ", playedCards));
            }

            sb.AppendLine($"Deck: {gameContext.Deck.Count} | Discard: {gameContext.DiscardPile.Count}");
            sb.AppendLine();
            sb.AppendLine("Commands: 'p [indices]' (play), 'd [indices]' (discard), 'sj [index]' (sell joker), 'sc [index]' (sell consumable), 'uc [index] [targets]' (use consumable)");
            sb.AppendLine();
        }

        private void DisplayJokersCompact(StringBuilder sb, GameContext gameContext)
        {
            if (gameContext.JokerContainer.Jokers.Count > 0)
            {
                sb.Append("Jokers: ");
                var jokers = new List<string>();
                for (int i = 0; i < gameContext.JokerContainer.Jokers.Count; i++)
                {
                    var joker = gameContext.JokerContainer.Jokers[i];
                    var edition = joker.Edition != CoreObjects.CoreEnums.Edition.None ? $"({joker.Edition})" : "";
                    jokers.Add($"[{i}] {joker.GetType().Name}{edition}");
                }
                sb.AppendLine(string.Join(", ", jokers));
            }
            else
            {
                sb.AppendLine("Jokers: (None)");
            }
        }

        private void DisplayConsumablesCompact(StringBuilder sb, GameContext gameContext)
        {
            if (gameContext.ConsumableContainer.Consumables.Count > 0)
            {
                sb.Append("Consumables: ");
                var consumables = new List<string>();
                for (int i = 0; i < gameContext.ConsumableContainer.Consumables.Count; i++)
                {
                    var consumable = gameContext.ConsumableContainer.Consumables[i];
                    var negative = consumable.IsNegative ? "(Neg)" : "";
                    consumables.Add($"[{i}] {consumable.GetType().Name}{negative}");
                }
                sb.AppendLine(string.Join(", ", consumables));
            }
            else
            {
                sb.AppendLine("Consumables: (None)");
            }
        }

        public void DisplayMessage(string message)
        {
            Console.WriteLine($">>> {message}");
        }

        public void DisplayError(string errorMessage)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"ERROR: {errorMessage}");
            Console.ResetColor();
        }

        public void Clear()
        {
            Console.Clear();
        }
    }
}