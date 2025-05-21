using Balatro.Core.GameEngine.GameStateController.PhaseActions.ActionIntents;

namespace Balatro.Core.GameEngine.GameStateController.PhaseActions
{
    public sealed class PackAction : BasePlayerAction
    {
        public PackActionIntent Intent { get; }
        public int CardIndex { get; }
    }
    
    public sealed class PackActionWithTargets : BasePlayerAction
    {
        public PackActionIntent Intent { get; }
        public int CardIndex { get; }
        public int[] TargetIndices { get; }
    }
}