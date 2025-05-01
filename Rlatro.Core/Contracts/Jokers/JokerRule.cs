using Balatro.Core.CoreObjects.Cards.CardObject;
using Balatro.Core.CoreObjects.Jokers.Joker;
using Balatro.Core.CoreRules.CanonicalViews;
using Balatro.Core.CoreRules.Scoring;
using Balatro.Core.GameEngine.GameStateController;

namespace Balatro.Core.Contracts.Jokers
{
    public abstract class JokerRule
    {
        public byte BasePrice { get; set; }
        public virtual bool OnTriggerApplies(GameContext ctx, in CardView cardView, JokerObject inst) => false;

        public virtual void OnCardTriggerEffect(GameContext ctx, in Card32 card, ref ScoreContext scoreCtx, JokerObject inst)
        {
        }

        public virtual void OnHandDetermined(GameContext ctx, ref ScoreContext scoreCtx)
        {
        }

        public virtual void OnCardTriggersEffectDone(GameContext ctx, ref ScoreContext scoreCtx) { }
    }
}