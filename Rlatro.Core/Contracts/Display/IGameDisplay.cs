using Balatro.Core.GameEngine.Contracts;
using Balatro.Core.GameEngine.GameStateController;

namespace Balatro.Core.Contracts.Display
{
    public interface IGameDisplay
    {
        void DisplayGameState(GameContext gameContext, IGamePhaseState currentState);
        void DisplayMessage(string message);
        void DisplayError(string errorMessage);
        void Clear();
    }
}