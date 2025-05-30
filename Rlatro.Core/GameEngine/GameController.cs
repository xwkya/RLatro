﻿using Balatro.Core.Contracts.Display;
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
        private GameContextBuilder GameContextBuilder { get; set; }
        
        public GameController(IGameDisplay display, IInputManager inputManager)
        {
            Display = display;
            InputManager = inputManager;
        }

        public void NewGame(IGameContextFactory gameContextFactory, string seed)
        {
            GameContextBuilder = GameContextBuilder;
            GameContext = gameContextFactory.CreateGameContext(seed);
            
            GameContext.Deck.Shuffle(GameContext.RngController);
            
            // Initialize the blind state
            var initialPhase = GameContext.GetPhase<BlindSelectionState>();
            initialPhase.GenerateAnteTags();
            GamePhaseState = initialPhase;
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
                        var currentPhase = GamePhaseState;
                        var nextPhase = currentPhase.GetNextPhaseState();
                        
                        // Only unload the current phase if the next phase is temporary (pack phase)
                        if (nextPhase.ShouldInitializeNextState)
                            currentPhase.OnExitPhase();
                        
                        if (currentPhase.ShouldInitializeNextState)
                            nextPhase.OnEnterPhase();

                        GamePhaseState = nextPhase;
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
            return GameContext?.PersistentState?.Ante > 8 || GameContext?.IsGameOver == true;
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
                    
                    if (GamePhaseState.ShouldInitializeNextState)
                    {
                        GamePhaseState.OnEnterPhase();
                    }
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