using Balatro.Core.GameEngine.GameStateController.PhaseActions.ActionIntents;

namespace Balatro.Core.GameEngine.GameStateController.PhaseActions
{
    public sealed class PackAction : BasePlayerAction
    {
        public PackActionIntent Intent { get; set; }
        public int CardIndex { get; set; }
    }
    
    public sealed class PackActionWithTargets : BasePlayerAction
    {
        public PackActionIntent Intent { get; set; }
        public int CardIndex { get; set; }
        public int[] TargetIndices { get; set; }
    }
}