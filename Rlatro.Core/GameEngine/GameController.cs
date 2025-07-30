using Balatro.Core.Contracts.Display;
using Balatro.Core.Contracts.Input;
using Balatro.Core.GameEngine.Contracts;
using Balatro.Core.GameEngine.GameStateController.PhaseStates;

namespace Balatro.Core.GameEngine
{
    public class GameController : BaseGameController
    {
        public GameController(IGameDisplay display, IInputManager inputManager)
            : base(display, inputManager)
        {
        }

        protected override void InitializeGamePhase()
        {
            var initialPhase = GameContext.GetPhase<BlindSelectionState>();
            initialPhase.GenerateAnteTags();
            GamePhaseState = initialPhase;
            GamePhaseState.OnEnterPhase();
        }

        protected override bool GetDefaultGameOverCondition()
        {
            return GameContext?.PersistentState?.Ante > 8 || GameContext?.IsGameOver == true;
        }
    }
}