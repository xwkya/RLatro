using Balatro.Core.CoreRules.Scoring;
using Balatro.Core.GameEngine.GameStateController;

namespace Balatro.Core.Contracts.CallbackDelegates
{
    public delegate void OnCardTriggersDone(
        GameContext ctx,
        ref ScoreContext scoreCtx);
}