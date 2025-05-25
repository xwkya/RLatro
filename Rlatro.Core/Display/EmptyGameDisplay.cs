using Balatro.Core.Contracts;
using Balatro.Core.Contracts.Display;
using Balatro.Core.GameEngine.Contracts;
using Balatro.Core.GameEngine.GameStateController;

namespace Balatro.Core.Display
{
    public class EmptyGameDisplay : IGameDisplay
    {
        public void DisplayGameState(GameContext gameContext, IGamePhaseState currentState) { }
        public void DisplayMessage(string message) { }
        public void DisplayError(string errorMessage) { }
        public void Clear() { }
    }
}