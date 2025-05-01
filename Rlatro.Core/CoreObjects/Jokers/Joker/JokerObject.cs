using Balatro.Core.CoreObjects.Cards.CardObject;
using Balatro.Core.CoreObjects.CoreEnums;
using Balatro.Core.CoreRules.CanonicalViews;
using Balatro.Core.CoreRules.Scoring;
using Balatro.Core.GameEngine.GameStateController;

namespace Balatro.Core.CoreObjects.Jokers.Joker
{
    public abstract class JokerObject
    {
        // private readonly JokerRule Rule;
        public uint Scaling { get; set; } // Scaling counter, each joker can use this however they want.
        public Suit Suit { get; set; } // Whatever suit the joker is targeting.
        public Rank Rank { get; set; } // Whatever rank the joker is targeting.
        public Edition Edition { get; set; }
        public ushort BonusSellValue { get; set; }
        public byte BaseSellValue { get; set; }

        public void OnCardTriggerEffect(
            GameContext ctx,
            in CardView cardView,
            ref Card32 card,
            ref ScoreContext scoreCtx)
        {
        }

        public void OnCardTriggerDone(
            GameContext ctx,
            ref ScoreContext scoreCtx)
        {
        }

        public void OnHandDetermined(
            GameContext ctx,
            ref ScoreContext scoreCtx)
        {
        }

        public void OnRoundEnd(
            GameContext ctx)
        {
        }

        public void OnBuy(GameContext ctx)
        {
        }

        public void OnRemove(GameContext ctx)
        {
        }
    }
}