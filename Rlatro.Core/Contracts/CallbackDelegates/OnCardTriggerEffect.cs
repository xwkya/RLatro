using Balatro.Core.CoreObjects.Cards.CardObject;
using Balatro.Core.CoreRules.Scoring;
using Balatro.Core.GameEngine.GameStateController;

namespace Balatro.Core.Contracts.CallbackDelegates
{
    public delegate void OnCardTriggerEffect(
        GameContext ctx,
        ref Card64 card,
        ref ScoreContext scoreCtx);
}