using System.Text;
using Balatro.Core.Contracts.Display;
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
            sb.AppendLine("=== BALATRO GAME STATE ===");
            sb.AppendLine($"Phase: {currentState.Phase}");
            sb.AppendLine($"Gold: ${gameContext.PersistentState.EconomyHandler.GetCurrentGold()}");
            sb.AppendLine($"Round: {gameContext.PersistentState.Round} | Ante: {gameContext.PersistentState.Ante}");
            sb.AppendLine();

            // Display phase-specific information
            switch (currentState.Phase)
            {
                case GamePhase.Round:
                    DisplayRoundState(sb, gameContext, currentState as RoundState);
                    break;
                case GamePhase.Shop:
                    DisplayShopState(sb, gameContext, currentState as ShopState);
                    break;
                default:
                    sb.AppendLine($"[{currentState.Phase} state - basic display]");
                    break;
            }

            // Always show jokers and consumables
            DisplayJokers(sb, gameContext);
            DisplayConsumables(sb, gameContext);

            Console.WriteLine(sb.ToString());
        }

        private void DisplayRoundState(StringBuilder sb, GameContext gameContext, RoundState roundState)
        {
            sb.AppendLine("=== ROUND STATE ===");
            sb.AppendLine($"Hands remaining: {roundState?.Hands ?? gameContext.GetHands()}");
            sb.AppendLine($"Discards remaining: {roundState?.Discards ?? gameContext.GetDiscards()}");
            sb.AppendLine($"Current score: {roundState?.CurrentChipsScore ?? 0}");
            sb.AppendLine($"Target score: {roundState?.CurrentChipsRequirement ?? 0}");
            sb.AppendLine();

            // Display hand
            sb.AppendLine("=== YOUR HAND ===");
            if (gameContext.Hand.Count == 0)
            {
                sb.AppendLine("(Empty)");
            }
            else
            {
                for (int i = 0; i < gameContext.Hand.Count; i++)
                {
                    var card = gameContext.Hand.Span[i];
                    sb.AppendLine($"[{i}] {card.Representation()}");
                }
            }
            sb.AppendLine();

            // Display played cards if any
            if (gameContext.PlayContainer.Count > 0)
            {
                sb.AppendLine("=== PLAYED CARDS ===");
                for (int i = 0; i < gameContext.PlayContainer.Count; i++)
                {
                    var card = gameContext.PlayContainer.Span[i];
                    sb.AppendLine($"  {card.Representation()}");
                }
                sb.AppendLine();
            }

            // Display deck and discard counts
            sb.AppendLine($"Deck: {gameContext.Deck.Count} cards | Discard pile: {gameContext.DiscardPile.Count} cards");
            sb.AppendLine();
        }

        private void DisplayShopState(StringBuilder sb, GameContext gameContext, ShopState shopState)
        {
            sb.AppendLine("=== SHOP STATE ===");
            sb.AppendLine("Welcome to the shop!");
            sb.AppendLine("(Shop display not fully implemented yet)");
            sb.AppendLine();
        }

        private void DisplayJokers(StringBuilder sb, GameContext gameContext)
        {
            sb.AppendLine("=== JOKERS ===");
            if (gameContext.JokerContainer.Jokers.Count == 0)
            {
                sb.AppendLine("(No jokers)");
            }
            else
            {
                for (int i = 0; i < gameContext.JokerContainer.Jokers.Count; i++)
                {
                    var joker = gameContext.JokerContainer.Jokers[i];
                    sb.AppendLine($"[{i}] {joker.GetType().Name} (ID: {joker.StaticId})");
                }
            }
            sb.AppendLine();
        }

        private void DisplayConsumables(StringBuilder sb, GameContext gameContext)
        {
            sb.AppendLine("=== CONSUMABLES ===");
            if (gameContext.ConsumableContainer.Consumables.Count == 0)
            {
                sb.AppendLine("(No consumables)");
            }
            else
            {
                for (int i = 0; i < gameContext.ConsumableContainer.Consumables.Count; i++)
                {
                    var consumable = gameContext.ConsumableContainer.Consumables[i];
                    sb.AppendLine($"[{i}] {consumable.GetType().Name}");
                }
            }
            sb.AppendLine();
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