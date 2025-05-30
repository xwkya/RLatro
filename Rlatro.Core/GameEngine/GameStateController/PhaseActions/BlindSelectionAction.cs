using Balatro.Core.GameEngine.GameStateController.PhaseActions.ActionIntents;

namespace Balatro.Core.GameEngine.GameStateController.PhaseActions
{
    public sealed class BlindSelectionAction : BasePlayerAction
    {
        public BlindSelectionActionIntent Intent { get; set; }
    }
}