using Balatro.Core.Contracts.Input;
using Balatro.Core.GameEngine.Contracts;
using Balatro.Core.GameEngine.GameStateController;
using Balatro.Core.GameEngine.GameStateController.PhaseActions;
using Balatro.Core.GameEngine.GameStateController.PhaseActions.ActionIntents;
using Balatro.Core.GameEngine.StateController;

namespace Balatro.Core.Input
{
    public class ConsoleInputManager : IInputManager
    {
        public BasePlayerAction GetPlayerAction(GameContext gameContext, IGamePhaseState currentState)
        {
            return currentState.Phase switch
            {
                GamePhase.Round => GetRoundAction(gameContext),
                GamePhase.Shop => GetShopAction(gameContext),
                GamePhase.ArcanaPack => GetPackActionWithTargets(gameContext, "Arcana"),
                GamePhase.SpectralPack => GetPackActionWithTargets(gameContext, "Spectral"),
                GamePhase.PlanetPack => GetPackAction(gameContext, "Planet"),
                GamePhase.JokerPack => GetPackAction(gameContext, "Joker"),
                GamePhase.CardPack => GetPackAction(gameContext, "Standard"),
                GamePhase.BlindSelection => GetBlindSelectionAction(gameContext),
                _ => throw new NotImplementedException($"Input handling for {currentState.Phase} not implemented")
            };
        }

        private BasePlayerAction GetRoundAction(GameContext gameContext)
        {
            while (true)
            {
                Console.Write("Round> ");
                var input = Console.ReadLine()?.Trim().ToLower();
                if (string.IsNullOrEmpty(input)) continue;

                try
                {
                    var parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    var command = parts[0];

                    return command switch
                    {
                        "p" => CreateRoundAction(RoundActionIntent.Play, parts.Skip(1)),
                        "d" => CreateRoundAction(RoundActionIntent.Discard, parts.Skip(1)),
                        "sj" => CreateSellJokerAction(parts, gameContext),
                        "sc" => CreateSellConsumableAction(parts, gameContext),
                        "uc" => CreateUseConsumableAction(parts, gameContext),
                        _ => throw new ArgumentException("Invalid command. Use: p/d [indices], sj/sc [index], uc [index] [targets]")
                    };
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
        }

        private BasePlayerAction GetShopAction(GameContext gameContext)
        {
            while (true)
            {
                Console.Write("Shop> ");
                var input = Console.ReadLine()?.Trim().ToLower();
                if (string.IsNullOrEmpty(input)) continue;

                try
                {
                    var parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    var command = parts[0];

                    return command switch
                    {
                        "r" => new ShopAction { ActionIntent = ShopActionIntent.Roll },
                        "bi" => CreateBuyItemAction(parts),
                        "bp" => CreateBuyPackAction(parts),
                        "bv" => CreateBuyVoucherAction(parts),
                        "sj" => CreateSellJokerAction(parts, gameContext),
                        "sc" => CreateSellConsumableAction(parts, gameContext),
                        "uc" => CreateUseConsumableAction(parts, gameContext),
                        "n" => new ShopAction { ActionIntent = ShopActionIntent.NextPhase },
                        _ => throw new ArgumentException("Invalid command. Use: r (roll), bi/bp/bv [index] (buy), sj/sc [index] (sell), uc [index] [targets] (use), n (next phase)")
                    };
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
        }

        private BasePlayerAction GetPackAction(GameContext gameContext, string packType)
        {
            while (true)
            {
                Console.Write($"{packType} Pack> ");
                var input = Console.ReadLine()?.Trim().ToLower();
                if (string.IsNullOrEmpty(input)) continue;

                try
                {
                    var parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    var command = parts[0];

                    return command switch
                    {
                        "c" => CreateChooseCardAction(parts),
                        "s" => new PackAction { Intent = PackActionIntent.SkipPack },
                        "sj" => CreateSellJokerAction(parts, gameContext),
                        "sc" => CreateSellConsumableAction(parts, gameContext),
                        "uc" => CreateUseConsumableAction(parts, gameContext),
                        _ => throw new ArgumentException("Invalid command. Use: c [index] (choose), s (skip), sj/sc [index] (sell), uc [index] [targets] (use)")
                    };
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
        }

        private BasePlayerAction GetPackActionWithTargets(GameContext gameContext, string packType)
        {
            while (true)
            {
                Console.Write($"{packType} Pack> ");
                var input = Console.ReadLine()?.Trim().ToLower();
                if (string.IsNullOrEmpty(input)) continue;

                try
                {
                    var parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    var command = parts[0];

                    return command switch
                    {
                        "c" => CreateChooseCardWithTargetsAction(parts),
                        "s" => new PackActionWithTargets { Intent = PackActionIntent.SkipPack },
                        "sj" => CreateSellJokerAction(parts, gameContext),
                        "sc" => CreateSellConsumableAction(parts, gameContext),
                        "uc" => CreateUseConsumableAction(parts, gameContext),
                        _ => throw new ArgumentException("Invalid command. Use: c [index] [targets...] (choose), s (skip), sj/sc [index] (sell), uc [index] [targets] (use)")
                    };
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
        }

        private BasePlayerAction GetBlindSelectionAction(GameContext gameContext)
        {
            while (true)
            {
                Console.Write("Blind Selection> ");
                var input = Console.ReadLine()?.Trim().ToLower();
                if (string.IsNullOrEmpty(input)) continue;

                try
                {
                    var parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    var command = parts[0];

                    return command switch
                    {
                        "p" => new BlindSelectionAction { Intent = BlindSelectionActionIntent.Play },
                        "s" => CreateSkipBlindAction(gameContext),
                        "sj" => CreateSellJokerAction(parts, gameContext),
                        "sc" => CreateSellConsumableAction(parts, gameContext),
                        "uc" => CreateUseConsumableAction(parts, gameContext),
                        _ => throw new ArgumentException("Invalid command. Use: p (play blind), s (skip blind), sj/sc [index] (sell), uc [index] [targets] (use)")
                    };
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
        }
        
        private BlindSelectionAction CreateSkipBlindAction(GameContext gameContext)
        {
            var roundType = (gameContext.PersistentState.Round - 1) % 3;
            
            // Check if this is a boss blind (cannot be skipped)
            if (roundType == 2)
            {
                throw new ArgumentException("Boss blinds cannot be skipped!");
            }

            return new BlindSelectionAction { Intent = BlindSelectionActionIntent.Skip };
        }

        // Action creation methods
        private RoundAction CreateRoundAction(RoundActionIntent intent, IEnumerable<string> cardIndicesStr)
        {
            var cardIndices = cardIndicesStr.Select(int.Parse).ToArray();
            return new RoundAction
            {
                ActionIntent = intent,
                CardIndexes = cardIndices
            };
        }

        private BasePlayerAction CreateSellJokerAction(string[] parts, GameContext gameContext)
        {
            if (parts.Length != 2)
                throw new ArgumentException("Sell joker requires joker index: sj [index]");

            var jokerIndex = int.Parse(parts[1]);
            
            // Validate joker exists
            if (jokerIndex < 0 || jokerIndex >= gameContext.JokerContainer.Jokers.Count)
                throw new ArgumentException($"Invalid joker index {jokerIndex}. Available jokers: 0-{gameContext.JokerContainer.Jokers.Count - 1}");

            return new RoundAction
            {
                SharedActionIntent = SharedActionIntent.SellJoker,
                JokerIndex = jokerIndex
            };
        }

        private BasePlayerAction CreateSellConsumableAction(string[] parts, GameContext gameContext)
        {
            if (parts.Length != 2)
                throw new ArgumentException("Sell consumable requires consumable index: sc [index]");

            var consumableIndex = int.Parse(parts[1]);
            
            // Validate consumable exists
            if (consumableIndex < 0 || consumableIndex >= gameContext.ConsumableContainer.Consumables.Count)
                throw new ArgumentException($"Invalid consumable index {consumableIndex}. Available consumables: 0-{gameContext.ConsumableContainer.Consumables.Count - 1}");

            return new RoundAction
            {
                SharedActionIntent = SharedActionIntent.SellConsumable,
                ConsumableIndex = consumableIndex
            };
        }

        private BasePlayerAction CreateUseConsumableAction(string[] parts, GameContext gameContext)
        {
            if (parts.Length < 2)
                throw new ArgumentException("Use consumable requires consumable index: uc [index] [targets...]");

            var consumableIndex = int.Parse(parts[1]);
            
            // Validate consumable exists
            if (consumableIndex < 0 || consumableIndex >= gameContext.ConsumableContainer.Consumables.Count)
                throw new ArgumentException($"Invalid consumable index {consumableIndex}. Available consumables: 0-{gameContext.ConsumableContainer.Consumables.Count - 1}");

            var targetCards = parts.Skip(2).Select(int.Parse).ToArray();
            var consumable = gameContext.ConsumableContainer.Consumables[consumableIndex];
            
            // Check if consumable is usable with provided targets
            if (!consumable.IsUsable(gameContext, targetCards))
            {
                Console.WriteLine($"Warning: {consumable.GetType().Name} cannot be used with the provided targets or in the current context.");
                Console.Write("Continue anyway? (y/n): ");
                var response = Console.ReadLine()?.Trim().ToLower();
                if (response != "y" && response != "yes")
                {
                    throw new ArgumentException("Consumable usage cancelled.");
                }
            }

            return new RoundAction
            {
                SharedActionIntent = SharedActionIntent.UseConsumable,
                ConsumableIndex = consumableIndex,
                CardIndexes = targetCards
            };
        }

        private ShopAction CreateBuyItemAction(string[] parts)
        {
            if (parts.Length != 2)
                throw new ArgumentException("Buy item requires item index: bi [index]");

            return new ShopAction
            {
                ActionIntent = ShopActionIntent.BuyFromShop,
                ShopIndex = byte.Parse(parts[1])
            };
        }

        private ShopAction CreateBuyPackAction(string[] parts)
        {
            if (parts.Length != 2)
                throw new ArgumentException("Buy pack requires pack index: bp [index]");

            return new ShopAction
            {
                ActionIntent = ShopActionIntent.BuyBoosterPack,
                BoosterPackIndex = byte.Parse(parts[1])
            };
        }

        private ShopAction CreateBuyVoucherAction(string[] parts)
        {
            if (parts.Length != 2)
                throw new ArgumentException("Buy voucher requires voucher index: bv [index]");

            return new ShopAction
            {
                ActionIntent = ShopActionIntent.BuyVoucher,
                VoucherIndex = byte.Parse(parts[1])
            };
        }

        private PackAction CreateChooseCardAction(string[] parts)
        {
            if (parts.Length != 2)
                throw new ArgumentException("Choose card requires card index: c [index]");

            return new PackAction
            {
                Intent = PackActionIntent.GetCard,
                CardIndex = int.Parse(parts[1])
            };
        }

        private PackActionWithTargets CreateChooseCardWithTargetsAction(string[] parts)
        {
            if (parts.Length < 2)
                throw new ArgumentException("Choose card requires card index: c [index] [targets...]");

            var cardIndex = int.Parse(parts[1]);
            var targetIndices = parts.Skip(2).Select(int.Parse).ToArray();

            return new PackActionWithTargets
            {
                Intent = PackActionIntent.GetCard,
                CardIndex = cardIndex,
                TargetIndices = targetIndices
            };
        }
    }
}