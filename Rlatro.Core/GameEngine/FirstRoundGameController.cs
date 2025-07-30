using Balatro.Core.Contracts.Display;
using Balatro.Core.Contracts.Input;
using Balatro.Core.GameEngine.Contracts;
using Balatro.Core.GameEngine.GameStateController.PhaseStates;

namespace Balatro.Core.GameEngine
{
    public class FirstRoundGameController : BaseGameController
    {
        public FirstRoundGameController(IGameDisplay display, IInputManager inputManager)
            : base(display, inputManager)
        {
        }

        public override void NewGame(IGameContextFactory gameContextFactory, string seed)
        {
            base.NewGame(gameContextFactory, seed);
            
            // Ensure we start in Round 1 (Ante is computed from Round)
            GameContext.PersistentState.Round = 1;
        }

        protected override void InitializeGamePhase()
        {
            // Start directly in RoundState, skip blind selection
            var roundPhase = GameContext.GetPhase<RoundState>();
            GamePhaseState = roundPhase;
            GamePhaseState.OnEnterPhase();
        }

        protected override bool GetDefaultGameOverCondition()
        {
            // Game over if:
            // 1. Standard game over (no plays left and score not met)
            // 2. We've moved past round 1
            return GameContext?.IsGameOver == true || 
                   GameContext?.PersistentState?.Round != 1;
        }

        public override void ResetGame(string seed)
        {
            base.ResetGame(seed);
            
            // Ensure we reset to Round 1 (Ante is computed from Round)
            GameContext.PersistentState.Round = 1;
        }
    }
}