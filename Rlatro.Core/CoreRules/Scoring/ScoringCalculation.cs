using Balatro.Core.CoreObjects.Cards.CardObject;
using Balatro.Core.CoreObjects.CoreEnums;
using Balatro.Core.CoreRules.CanonicalViews;
using Balatro.Core.GameEngine.GameStateController;
using Balatro.Core.GameEngine.PseudoRng;

namespace Balatro.Core.CoreRules.Scoring
{
    public static class ScoringCalculation
    {
        // Edition bonuses
        private const uint FoilChips = 50;
        private const uint HoloMult = 10;
        private const uint PolychromeMultNumerator = 3;
        private const uint PolychromeMultDenominator = 2;
        
        private static void TriggerEdition(Edition edition, ref ScoreContext scoreCtx)
        {
            switch (edition)
            {
                case Edition.Foil:
                    scoreCtx.AddChips(FoilChips);
                    break;
                case Edition.Holo:
                    scoreCtx.AddMult(HoloMult);
                    break;
                case Edition.Poly:
                    scoreCtx.TimesMult(PolychromeMultNumerator, PolychromeMultDenominator);
                    break;
            }
        }
        
        // Enhancement bonuses
        private const uint BonusEnhancementChips = 30;
        private const uint GlassEnhancementMult = 2;
        private const uint MultEnhancementValue = 4;
        private const uint StoneEnhancementChips = 50;
        private const uint LuckyEnhancementMult = 20;
        private const int LuckyEnhancementGold = 20;
        private const float LuckyEnhancementMultProbability = 0.2f;     // 1 in 5
        private const float LuckyEnhancementGoldProbability = 1f / 15f; // 1 in 15
        
        public static ScoreContext EvaluateHand(GameContext ctx)
        {
            Span<int> scoringCards = stackalloc int[ctx.PlayContainer.Count];
            Span<CardView> playedCardViews = stackalloc CardView[ctx.PlayContainer.Count];
            Span<CardView> handCardViews = stackalloc CardView[ctx.Hand.Count];
            
            // Create card views for played cards
            ctx.PlayContainer.FillCardViews(ctx, playedCardViews, true);
            ctx.Hand.FillCardViews(ctx, handCardViews, false);
            
            var handRank = HandRankGetter.GetRank(
                ctx.JokerContainer.FourFingers(),
                ctx.JokerContainer.Shortcut(),
                playedCardViews,
                scoringCards);
            
            var scoreContext = ctx.PersistentState.GetHandScore(handRank);
            
            // On hand determined
            foreach (var joker in ctx.JokerContainer.Jokers)
            {
                joker.OnHandDetermined(ctx, playedCardViews, ref scoreContext);
            }
            
            // Publish the event
            ctx.GameEventBus.PublishHandPlayed(playedCardViews, handRank);
            
            // There is a chance the card view has changed (vampire can remove wild effect)
            ctx.PlayContainer.FillCardViews(ctx, playedCardViews, true);
            
            // Trigger played cards
            TriggerPlayedCards(ctx, playedCardViews, scoringCards, ref scoreContext);
            
            // Trigger cards held in hand
            TriggerHandCards(ctx, handCardViews, ref scoreContext);
            
            // Compute joker post trigger effects
            ComputeJokerPostTriggerEffects(ctx, ref scoreContext);
            
            return scoreContext;
        }

        private static int CountCardTriggers(
            GameContext ctx,
            bool inPlay,
            int cardIndex,
            CardView cardView)
        {
            // TODO: Handle boss blinds that disable cards here
            var totalTriggers = 1;
            if (cardView.IsRedSeal)
            {
                totalTriggers += 1;
            }

            foreach (var joker in ctx.JokerContainer.Jokers)
            {
                if (inPlay)
                {
                    totalTriggers += joker.AddPlayedCardTriggers(ctx, cardView, cardIndex);
                }
                else
                {
                    totalTriggers += joker.AddHeldInHandTriggers(ctx, cardView);
                }
            }

            return totalTriggers;
        }
        
        private static void TriggerHandCards(
            GameContext ctx,
            ReadOnlySpan<CardView> handCardViews,
            ref ScoreContext scoreCtx)
        {
            // Count how many times the card will be triggered
            Span<int> countPlayedTriggers = stackalloc int[ctx.PlayContainer.Count];
            
            countPlayedTriggers.Fill(0); 
            
            // Triggers from red seals + joker triggers
            for (var i = 0; i < ctx.Hand.Count; i++)
            {
                countPlayedTriggers[i] = CountCardTriggers(ctx, false, i, handCardViews[i]);
            }
            
            // Trigger all cards in hand
            for (var i = 0; i < ctx.Hand.Count; i++)
            {
                var cardToScore = ctx.Hand.Span[i];
                var cardView = handCardViews[i];
                var cardTriggers = countPlayedTriggers[i];
                
                for (var trigger = 0; trigger < cardTriggers; trigger++)
                {
                    cardToScore = TriggerSingleHandCard(ctx, cardView, cardToScore, ref scoreCtx);
                }
                
                ctx.Hand.Span[i] = cardToScore; // update the card in the container
            }
        }

        private static void TriggerPlayedCards(
            GameContext ctx,
            ReadOnlySpan<CardView> playedCardViews,
            ReadOnlySpan<int> scoringCards,
            ref ScoreContext scoreCtx)
        {
            // Count how many times the card will be triggered
            Span<int> countPlayedTriggers = stackalloc int[ctx.PlayContainer.Count];
            
            countPlayedTriggers.Fill(0); 
            
            // Triggers from red seals + joker triggers
            for (var i = 0; i < ctx.PlayContainer.Count; i++)
            {
                if (scoringCards[i] == 0) continue;

                countPlayedTriggers[i] = CountCardTriggers(ctx, true, i, playedCardViews[i]);
            }
            
            // Trigger all cards in play if it counts for scoring
            for (var i = 0; i < ctx.PlayContainer.Count; i++)
            {
                if (scoringCards[i] == 0) continue;
                
                var cardToScore = ctx.PlayContainer.Span[i];
                var cardView = playedCardViews[i];
                var cardTriggers = countPlayedTriggers[i];
                
                for (var trigger = 0; trigger < cardTriggers; trigger++)
                {
                    cardToScore = TriggerSinglePlayedCard(ctx, cardView, cardToScore, ref scoreCtx);
                }
                
                ctx.PlayContainer.Span[i] = cardToScore; // update the card in the container
            }
        }
        
        /// <summary>
        /// Single trigger of a card in play
        /// </summary>
        private static Card64 TriggerSinglePlayedCard(
            GameContext ctx,
            CardView cardView,
            Card64 card,
            ref ScoreContext scoreCtx)
        {
            // Natural effects
            TriggerCardNaturalEffects(card, ref scoreCtx);
            
            // Trigger enhancement
            TriggerCardEnhancement(ctx, card.GetEnh(), ref scoreCtx);
            
            // Trigger edition
            TriggerEdition(card.GetEdition(), ref scoreCtx);
            
            // Trigger joker effects
            foreach (var joker in ctx.JokerContainer.Jokers)
            {
                if (joker.HasOnPlayedCardTriggerEffect)
                    card = joker.OnPlayedCardTriggerEffect(ctx, cardView, card, ref scoreCtx);
            }

            return card;
        }
        
        /// <summary>
        /// Single trigger of a card in hand (enhancement + joker effects only)
        /// </summary>
        private static Card64 TriggerSingleHandCard(
            GameContext ctx,
            CardView cardView,
            Card64 card,
            ref ScoreContext scoreCtx)
        {
            // Enhancement
            TriggerHandCardEnhancement(card, ref scoreCtx);
            
            // Trigger jokers
            foreach (var joker in ctx.JokerContainer.Jokers)
            {
                if (joker.HasOnHeldInHandTriggerEffect)
                    card = joker.OnHeldInHandTriggerEffect(ctx, cardView, card, ref scoreCtx);
            }

            return card;
        }

        private static void TriggerCardNaturalEffects(
            Card64 card,
            ref ScoreContext scoreCtx)
        {
            var cardChips = card.GetTotalChipsValue();
            scoreCtx.AddChips(cardChips);
        }
        
        private static void TriggerCardEnhancement(GameContext ctx, Enhancement enhancement, ref ScoreContext scoreCtx)
        {
            switch (enhancement)
            {
                case Enhancement.Bonus:
                    scoreCtx.AddChips(BonusEnhancementChips);
                    break;
                case Enhancement.Mult:
                    scoreCtx.AddMult(MultEnhancementValue);
                    break;
                case Enhancement.Stone:
                    scoreCtx.AddChips(StoneEnhancementChips);
                    break;
                case Enhancement.Lucky:
                    if (ctx.RngController.ProbabilityCheck(LuckyEnhancementMultProbability, RngActionType.LuckyCardMult))
                    {
                        scoreCtx.AddMult(LuckyEnhancementMult);
                    }
                    if (ctx.RngController.ProbabilityCheck(LuckyEnhancementGoldProbability, RngActionType.LuckyCardMoney))
                    {
                        ctx.PersistentState.Gold += LuckyEnhancementGold;
                    }
                    break;
                case Enhancement.Glass:
                    scoreCtx.TimesMult(GlassEnhancementMult, 1);
                    break;
            }
        }

        private static void TriggerHandCardEnhancement(
            Card64 card,
            ref ScoreContext scoreContext)
        {
            var enhancement = card.GetEnh();
            switch (enhancement)
            {
                case Enhancement.Steel:
                    scoreContext.TimesMult(3, 2);
                    break;
            }
        }

        private static void ComputeJokerPostTriggerEffects(
            GameContext ctx,
            ref ScoreContext scoreCtx)
        {
            foreach (var joker in ctx.JokerContainer.Jokers)
            {
                var jokerEdition = joker.Edition;
                joker.OnCardTriggerDone(ctx, ref scoreCtx);
                TriggerEdition(jokerEdition, ref scoreCtx);
            }
        }
    }
}