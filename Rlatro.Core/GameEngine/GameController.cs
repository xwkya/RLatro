using Balatro.Core.Contracts.Display;
using Balatro.Core.Contracts.Input;
using Balatro.Core.GameEngine.Contracts;
using Balatro.Core.GameEngine.GameStateController;
using Balatro.Core.GameEngine.GameStateController.PhaseActions;
using Balatro.Core.GameEngine.GameStateController.PhaseStates;

namespace Balatro.Core.GameEngine
{
    public class GameController
    {
        private GameContext GameContext;
        public IGamePhaseState GamePhaseState { get; private set; }
        private readonly IGameDisplay Display;
        private readonly IInputManager InputManager;
        
        public GameController(IGameDisplay display, IInputManager inputManager)
        {
            Display = display;
            InputManager = inputManager;
        }

        public void NewGame(IGameContextFactory gameContextFactory)
        {
            GameContext = gameContextFactory.CreateGameContext();
            
            GameContext.Deck.Shuffle(GameContext.RngController);
            GamePhaseState = new RoundState(GameContext);
            GamePhaseState.OnEnterPhase();
            
            Display.DisplayMessage("New game started!");
            Display.DisplayGameState(GameContext, GamePhaseState);
        }

        public void RunGameLoop()
        {
            if (GameContext == null || GamePhaseState == null)
            {
                Display.DisplayError("Game not initialized. Call NewGame() first.");
                return;
            }

            while (true)
            {
                try
                {
                    // Display current state
                    Display.DisplayGameState(GameContext, GamePhaseState);

                    // Check for game end conditions
                    if (IsGameOver())
                    {
                        Display.DisplayMessage("Game Over!");
                        break;
                    }

                    // Get player action
                    var action = InputManager.GetPlayerAction(GameContext, GamePhaseState);

                    // Process action
                    var phaseOver = GamePhaseState.HandleAction(action);
                    
                    if (phaseOver)
                    {
                        Display.DisplayMessage($"Phase {GamePhaseState.Phase} completed!");
                        GamePhaseState.OnExitPhase();
                        GamePhaseState = GamePhaseState.GetNextPhaseState();
                        GamePhaseState.OnEnterPhase();
                        Display.DisplayMessage($"Entering {GamePhaseState.Phase} phase");
                    }
                }
                catch (Exception ex)
                {
                    Display.DisplayError($"Action failed: {ex.Message}");
                    throw;
                }
            }
        }

        private bool IsGameOver()
        {
            // Simple game over condition - you can expand this
            return GameContext?.PersistentState?.Round > 10; // End after 10 rounds for demo
        }

        public void HandleSingleAction(BasePlayerAction action)
        {
            try
            {
                var phaseOver = GamePhaseState.HandleAction(action);
                
                if (phaseOver)
                {
                    GamePhaseState.OnExitPhase();
                    GamePhaseState = GamePhaseState.GetNextPhaseState();
                    GamePhaseState.OnEnterPhase();
                }
                
                Display.DisplayGameState(GameContext, GamePhaseState);
            }
            catch (Exception ex)
            {
                Display.DisplayError($"Action failed: {ex.Message}");
            }
        }
    }
}