﻿using Balatro.Core.GameEngine.GameStateController.PhaseActions;
using Balatro.Core.GameEngine.StateController;

namespace Balatro.Core.GameEngine.Contracts
{
    public interface IGamePhaseState
    {
        GamePhase Phase { get; }
        public bool ShouldInitializeNextState { get; }
        bool HandleAction(BasePlayerAction action);
        public void OnEnterPhase() {}
        public void OnExitPhase() {}
        IGamePhaseState GetNextPhaseState();
        public void Reset();
    }
}