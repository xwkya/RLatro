using Balatro.Core.GameEngine.GameStateController;

namespace Balatro.Core.GameEngine.Contracts
{
    public interface IGameStateFactory
    {
        public GameContext CreateGameState();
    }
}