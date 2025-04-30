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
        
        private GameController()
        {
            GameContext = new GameContext();
        }

        public void NewGame(IGameStateFactory gameStateFactory)
        {
            GameContext = gameStateFactory.CreateGameState();
        }

        public void HandleAction(BasePlayerAction action)
        {
            GamePhaseState.HandleAction(GameContext, action);
        }
    }
}