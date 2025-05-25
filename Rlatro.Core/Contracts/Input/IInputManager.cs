using Balatro.Core.GameEngine.Contracts;
using Balatro.Core.GameEngine.GameStateController;
using Balatro.Core.GameEngine.GameStateController.PhaseActions;

namespace Balatro.Core.Contracts.Input
{
    public interface IInputManager
    {
        BasePlayerAction GetPlayerAction(GameContext gameContext, IGamePhaseState currentState);
    }
}