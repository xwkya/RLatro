using Balatro.Core.Contracts.Input;
using Balatro.Core.GameEngine.Contracts;
using Balatro.Core.GameEngine.GameStateController;
using Balatro.Core.GameEngine.GameStateController.PhaseActions;
using Balatro.Core.GameEngine.GameStateController.PhaseActions.ActionIntents;
using Balatro.Core.GameEngine.GameStateController.PhaseStates;
using Balatro.Core.GameEngine.StateController;

namespace Balatro.Core.Input
{
    public class ConsoleInputManager : IInputManager
    {
        public BasePlayerAction GetPlayerAction(GameContext gameContext, IGamePhaseState currentState)
        {
            switch (currentState.Phase)
            {
                case GamePhase.Round:
                    return GetRoundAction(gameContext, currentState as RoundState);
                case GamePhase.Shop:
                    return GetShopAction(gameContext, currentState as ShopState);
                default:
                    throw new NotImplementedException($"Input handling for {currentState.Phase} not implemented yet");
            }
        }

        private BasePlayerAction GetRoundAction(GameContext gameContext, RoundState roundState)
        {
            while (true)
            {
                Console.WriteLine("=== ROUND ACTIONS ===");
                Console.WriteLine("p [card indices] - Play cards (e.g., 'p 0 1 2')");
                Console.WriteLine("d [card indices] - Discard cards (e.g., 'd 3 4')");
                Console.WriteLine("sj [joker index] - Sell joker (e.g., 'sj 0')");
                Console.WriteLine("sc [consumable index] - Sell consumable (e.g., 'sc 0')");
                Console.WriteLine("uc [consumable index] [target cards] - Use consumable (e.g., 'uc 0 1 2')");
                Console.WriteLine();
                Console.Write("Enter action: ");

                var input = Console.ReadLine()?.Trim().ToLower();
                if (string.IsNullOrEmpty(input))
                    continue;

                try
                {
                    var parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    var command = parts[0];

                    switch (command)
                    {
                        case "p":
                            return CreateRoundAction(RoundActionIntent.Play, parts.Skip(1));
                        case "d":
                            return CreateRoundAction(RoundActionIntent.Discard, parts.Skip(1));
                        case "sj":
                            return CreateSellJokerAction(parts);
                        case "sc":
                            return CreateSellConsumableAction(parts);
                        case "uc":
                            return CreateUseConsumableAction(parts);
                        default:
                            Console.WriteLine("Invalid command. Try again.");
                            continue;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error parsing command: {ex.Message}");
                    continue;
                }
            }
        }

        private BasePlayerAction GetShopAction(GameContext gameContext, ShopState shopState)
        {
            Console.WriteLine("=== SHOP ACTIONS ===");
            Console.WriteLine("(Shop actions not implemented yet - returning to round)");
            Console.WriteLine("Press any key to continue to next round...");
            Console.ReadKey();
            
            // For now, just return an action that will transition to the next phase
            return new ShopAction { ActionIntent = ShopActionIntent.Roll };
        }

        private RoundAction CreateRoundAction(RoundActionIntent intent, IEnumerable<string> cardIndicesStr)
        {
            var cardIndices = cardIndicesStr.Select(int.Parse).ToArray();
            return new RoundAction
            {
                ActionIntent = intent,
                CardIndexes = cardIndices
            };
        }

        private BasePlayerAction CreateSellJokerAction(string[] parts)
        {
            if (parts.Length != 2)
                throw new ArgumentException("Sell joker requires joker index");

            return new RoundAction
            {
                SharedActionIntent = SharedActionIntent.SellJoker,
                JokerIndex = int.Parse(parts[1])
            };
        }

        private BasePlayerAction CreateSellConsumableAction(string[] parts)
        {
            if (parts.Length != 2)
                throw new ArgumentException("Sell consumable requires consumable index");

            return new RoundAction
            {
                SharedActionIntent = SharedActionIntent.SellConsumable,
                ConsumableIndex = int.Parse(parts[1])
            };
        }

        private BasePlayerAction CreateUseConsumableAction(string[] parts)
        {
            if (parts.Length < 2)
                throw new ArgumentException("Use consumable requires consumable index and optional target cards");

            var consumableIndex = int.Parse(parts[1]);
            var targetCards = parts.Skip(2).Select(int.Parse).ToArray();

            return new RoundAction
            {
                SharedActionIntent = SharedActionIntent.UseConsumable,
                ConsumableIndex = consumableIndex,
                CardIndexes = targetCards
            };
        }
    }
}