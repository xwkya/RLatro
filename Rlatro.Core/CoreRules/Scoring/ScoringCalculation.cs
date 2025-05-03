using Balatro.Core.CoreObjects.Cards.CardObject;
using Balatro.Core.CoreObjects.CoreEnums;
using Balatro.Core.CoreRules.CanonicalViews;
using Balatro.Core.GameEngine.GameStateController;

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
        private const uint MultEnhancementValue = 4;
        private const uint StoneEnhancementChips = 50;
        private const uint LuckyEnhancementMult = 20;
        private const uint LuckyEnhancementGold = 20;
        private const float LuckyEnhancementMultProbability = 0.2f;     // 1 in 5
        private const float LuckyEnhancementGoldProbability = 1f / 15f; // 1 in 15
        
        private static void TriggerEnhancement(GameContext ctx, Enhancement enhancement, ref ScoreContext scoreCtx)
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
                    break;
            }
        }
        
        public static ScoreContext ComputeHandScore(GameContext ctx)
        {
            Span<byte> scoringCards = stackalloc byte[ctx.PlayContainer.Count];
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
            
            // There is a chance the card view has changed (vampire can remove wild effect)
            ctx.PlayContainer.FillCardViews(ctx, playedCardViews, true);
            
            // -- Count how many times the card will be triggered --
            Span<byte> countPlayedTriggers = stackalloc byte[ctx.PlayContainer.Count];
            
            // One natural trigger TODO: Handle boss blinds that disable cards
            countPlayedTriggers.Fill(1); 
            
            // Triggers from red seals + joker triggers
            for (var i = 0; i < ctx.PlayContainer.Count; i++)
            {
                if (ctx.PlayContainer.Span[i].GetSeal() == Seal.Red)
                {
                    countPlayedTriggers[i] += 1;
                }

                foreach (var joker in ctx.JokerContainer.Jokers)
                {
                    countPlayedTriggers[i] += joker.AddTriggers(ctx, in playedCardViews[i], i);
                }
            }
            
            // Trigger all cards in hand TODO: If this is a performance bottleneck consider accessing the internal span directly
            for (var i = 0; i < ctx.PlayContainer.Count; i++)
            {
                var cardToScore = ctx.PlayContainer.Span[i];
                var cardView = playedCardViews[i];
                var cardTriggers = countPlayedTriggers[i];
                
                for (var trigger = 0; trigger < cardTriggers; trigger++)
                {
                    TriggerCard(ctx, in cardView, ref cardToScore, ref scoreContext);
                }
            }

            return scoreContext;
        }
        
        private static void TriggerCard(
            GameContext ctx,
            in CardView cardView,
            ref Card32 card,
            ref ScoreContext scoreCtx)
        {
            
            foreach (var joker in ctx.JokerContainer.Jokers)
            {
                joker.OnCardTriggerEffect(ctx, cardView, ref card, ref scoreCtx);
            }
        }

        private static void TriggerCardNaturalEffects(
            in Card32 card,
            ref ScoreContext scoreCtx)
        {
            var cardChips = card.GetTotalChipsValue();
            scoreCtx.AddChips(cardChips);
        }

        private static void TriggerCardEnhancement(
            in Card32 card,
            ref ScoreContext scoreCtx)
        {
            var enhancement = card.GetEnh();
            switch (enhancement)
            {
                case Enhancement.Bonus:
                    scoreCtx.AddChips(100);
                    break;
            }
        }
        
        private static void TriggerEdition(
            in Card32 card,
            ref ScoreContext scoreCtx)
        {
            var edition = card.GetEdition();
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
    }
}