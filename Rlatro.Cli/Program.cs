using Balatro.Core.Contracts.Display;
using Balatro.Core.Contracts.Input;
using Balatro.Core.Display;
using Balatro.Core.GameEngine;
using Balatro.Core.GameEngine.GameStateController;
using Balatro.Core.Input;
using Balatro.Core.ObjectsImplementations.Decks;

namespace Rlatro.Cli
{
    public static class Program
    {
        public static void Main()
        {
            Console.WriteLine("Welcome to Balatro CLI!");
            Console.WriteLine("Starting a new game...");
            Console.WriteLine();

            // Setup dependencies
            IGameDisplay display = new ConsoleGameDisplay();
            IInputManager inputManager = new ConsoleInputManager();
            
            // Create game controller
            var gameController = new GameController(display, inputManager);
            
            var contextBuilder = GetGameContextBuilder();
            
            // Start the game
            gameController.NewGame(contextBuilder, "DEMO");
            gameController.RunGameLoop();
            
            Console.WriteLine("Thanks for playing!");
        }
        
        private static string RandomSeed()
        {
            return Guid.NewGuid().ToString();
        }
        
        private static GameContextBuilder GetGameContextBuilder()
        {
            return GameContextBuilder.Create()
                .WithDeck(new RedDeckFactory());
        }
    }
}