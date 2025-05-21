using Balatro.Core.GameEngine.GameStateController.PhaseActions.ActionIntents;

namespace Balatro.Core.GameEngine.GameStateController.PhaseActions
{
    /// <summary>
    /// A round action is an action that can be performed during a round.
    /// </summary>
    public sealed class RoundAction : BasePlayerAction
    {
        /// <summary>
        /// Defines the intent of the action
        /// </summary>
        public RoundActionIntent ActionIntent { get; init; }
    }
}