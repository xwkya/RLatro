using Balatro.Core.GameEngine.GameStateController;

namespace Balatro.Core.GameEngine.Contracts
{
    public interface IGameContextFactory
    {
        public GameContext CreateGameContext();
    }
}