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
            
            // Start the game
            gameController.NewGame(GetGameContextBuilder("DEMO"));
            gameController.RunGameLoop();
            
            Console.WriteLine("Thanks for playing!");
        }
        
        private static GameContextBuilder GetGameContextBuilder(string seed)
        {
            return GameContextBuilder.Create(seed)
                .WithDeck(new DefaultDeckFactory());
        }
    }
}