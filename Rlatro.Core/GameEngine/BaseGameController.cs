using Balatro.Core.Contracts.Display;
using Balatro.Core.Contracts.Input;
using Balatro.Core.GameEngine.Contracts;
using Balatro.Core.GameEngine.GameStateController;
using Balatro.Core.GameEngine.GameStateController.PhaseActions;

namespace Balatro.Core.GameEngine
{
    public abstract class BaseGameController
    {
        protected GameContext GameContext;
        public IGamePhaseState GamePhaseState { get; protected set; }
        protected readonly IGameDisplay Display;
        protected readonly IInputManager InputManager;
        protected IGameContextFactory GameContextFactory;
        protected string CurrentSeed;
        
        public Func<bool> IsGameOverOverride { get; set; }
        
        protected BaseGameController(IGameDisplay display, IInputManager inputManager)
        {
            Display = display;
            InputManager = inputManager;
        }

        public virtual void NewGame(IGameContextFactory gameContextFactory, string seed)
        {
            GameContextFactory = gameContextFactory;
            CurrentSeed = seed;
            
            GameContext = gameContextFactory.CreateGameContext(seed);
            GameContext.Deck.Shuffle(GameContext.RngController);
            
            InitializeGamePhase();
            
            Display.DisplayMessage("New game started!");
            Display.DisplayGameState(GameContext, GamePhaseState);
        }

        protected abstract void InitializeGamePhase();

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
                    Display.DisplayGameState(GameContext, GamePhaseState);

                    if (IsGameOver())
                    {
                        Display.DisplayMessage("Game Over!");
                        break;
                    }

                    var action = InputManager.GetPlayerAction(GameContext, GamePhaseState);
                    var phaseOver = GamePhaseState.HandleAction(action);
                    
                    if (phaseOver)
                    {
                        Display.DisplayMessage($"Phase {GamePhaseState.Phase} completed!");
                        var currentPhase = GamePhaseState;
                        var nextPhase = currentPhase.GetNextPhaseState();
                        
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
            if (IsGameOverOverride != null)
            {
                return IsGameOverOverride();
            }
            
            return GetDefaultGameOverCondition();
        }

        protected abstract bool GetDefaultGameOverCondition();

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

        public virtual void ResetGame(string seed)
        {
            if (GameContextFactory == null)
            {
                throw new InvalidOperationException("Cannot reset game: No game has been initialized. Call NewGame() first.");
            }

            CurrentSeed = seed;
            GameContextFactory.ResetGameContext(seed);
            GameContext.Deck.Shuffle(GameContext.RngController);
            
            InitializeGamePhase();
            
            Display.DisplayMessage("Game reset!");
            Display.DisplayGameState(GameContext, GamePhaseState);
        }

        public GameContext GetGameContext()
        {
            return GameContext;
        }
    }
}