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
            var consoleDisplay = new ConsoleGameDisplay();
            var fileLogger = new FileLoggerDisplay("game-log.txt");
            var compositeDisplay = new CompositeDisplay(consoleDisplay, fileLogger);
            
            IInputManager inputManager = new ConsoleInputManager();
            
            // Create game controller
            var gameController = new FirstRoundGameController(compositeDisplay, inputManager);
            
            var contextBuilder = GetGameContextBuilder();
            
            try
            {
                // Start the game
                gameController.NewGame(contextBuilder, "DEMO");
                gameController.RunGameLoop();
            }
            finally
            {
                // Clean up file resources
                compositeDisplay.Dispose();
            }
            
            Console.WriteLine("Thanks for playing!");
            Console.WriteLine("Game states have been logged to: game-log.txt");
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