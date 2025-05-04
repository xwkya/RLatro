using Balatro.Core.GameEngine.GameStateController.PhaseActions;
using Balatro.Core.GameEngine.StateController;

namespace Balatro.Core.GameEngine.GameStateController.PhaseStates
{
    public interface IGamePhaseState
    {
        GamePhase Phase { get; }
        bool IsPhaseOver { get; }
        bool HandleAction(BasePlayerAction action);
        void OnEnterPhase() {}
        void OnExitPhase() {}
    }
}