using Balatro.Core.CoreObjects.Cards.CardObject;
using Balatro.Core.CoreObjects.CoreEnums;
using Balatro.Core.CoreRules.CanonicalViews;
using Balatro.Core.CoreRules.Scoring;
using Balatro.Core.GameEngine.GameStateController;

namespace Balatro.Core.CoreObjects.Jokers.Joker
{
    public abstract class JokerObject
    {
        public uint Id { get; private set; }
            
        public uint Scaling { get; set; } // Scaling counter, each joker can use this however they want.
        public SuitMask Suit { get; set; } // Whatever suit the joker is targeting.
        public Rank? Rank { get; set; } // Whatever rank the joker is targeting.
        public Edition Edition { get; set; }
        public int BonusSellValue { get; set; }
        public abstract int BaseSellValue { get; }
        
        public JokerObject(uint id, Edition edition = Edition.None)
        {
            Id = id;
            Scaling = 0;
            Suit = SuitMask.None;
            Rank = null;
            Edition = edition;
            BonusSellValue = 0;
        }
        
        public abstract bool HasOnPlayedCardTriggerEffect { get; }
        public abstract bool HasOnHeldInHandTriggerEffect { get; }
        
        /// <summary>
        /// Effect applied on card scoring.
        /// </summary>
        /// <param name="ctx">The game context</param>
        /// <param name="cardView">The card view itself</param>
        /// <param name="card">The card reference (if the card needs to be changed by the joker)</param>
        /// <param name="scoreCtx">The score context when the card is triggered</param>
        /// <returns>The Card64 in case it was modified during the scoring</returns>
        public Card64 OnPlayedCardTriggerEffect(
            GameContext ctx,
            CardView cardView,
            Card64 card,
            ref ScoreContext scoreCtx)
        {
            return card;
        }
        
        public Card64 OnHeldInHandTriggerEffect(
            GameContext ctx,
            CardView cardView,
            Card64 card,
            ref ScoreContext scoreCtx)
        {
            return card;
        }

        public void OnCardTriggerDone(
            GameContext ctx,
            ref ScoreContext scoreCtx)
        {
        }

        public byte AddPlayedCardTriggers(
            GameContext ctx,
            CardView cardView,
            int cardPosition)
        {
            return 0;
        }
        
        public byte AddHeldInHandTriggers(
            GameContext ctx,
            CardView cardView)
        {
            return 0;
        }

        public void OnHandDetermined(
            GameContext ctx,
            ReadOnlySpan<CardView> playedCards,
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