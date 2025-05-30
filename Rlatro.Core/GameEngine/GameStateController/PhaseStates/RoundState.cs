using Balatro.Core.CoreObjects.CoreEnums;
using Balatro.Core.CoreRules.CanonicalViews;
using Balatro.Core.CoreRules.Scoring;
using Balatro.Core.GameEngine.Contracts;
using Balatro.Core.GameEngine.GameStateController.PhaseActions;
using Balatro.Core.GameEngine.GameStateController.PhaseActions.ActionIntents;
using Balatro.Core.GameEngine.StateController;

namespace Balatro.Core.GameEngine.GameStateController.PhaseStates
{
    /// <summary>
    /// Tracks the current round state.
    /// </summary>
    public class RoundState : BaseGamePhaseState
    {
        public override GamePhase Phase => GamePhase.Round;
        public override bool ShouldInitializeNextState => true;

        public int Hands { get; set; }
        public int Discards { get; set; }
        public uint CurrentChipsScore { get; set; }  // TODO: This will be a problem for hands above 4T chips
        public uint CurrentChipsRequirement { get; set; }
        public bool IsPhaseOver => (CurrentChipsScore >= CurrentChipsRequirement) || (Hands == 0) || (GameContext.GetHandSize() <= 0);
        private HandRank? LastPlayedHand { get; set; }

        public RoundState(GameContext ctx) : base(ctx)
        {
            CurrentChipsScore = 0;
            Hands = GameContext.GetHands();
            Discards = GameContext.GetDiscards();
        }

        public override void OnEnterPhase()
        {
            base.OnEnterPhase();
            DrawCards();
            CurrentChipsRequirement = 300;
        }

        public override void OnExitPhase()
        {
            base.OnExitPhase();

            TriggerEndOfRoundEvent();
            
            GameContext.Hand.MoveAllTo(GameContext.Deck);
            GameContext.DiscardPile.MoveAllTo(GameContext.Deck);
            GameContext.Deck.Shuffle(GameContext.RngController);

            GameContext.PersistentState.UnusedDiscards += Discards;
            
            // Move up one round
            GameContext.PersistentState.Round++;
        }

        protected override bool HandleStateSpecificAction(BasePlayerAction action)
        {
            if (action is not RoundAction roundAction)
            {
                throw new ArgumentException($"Action {action} is not a {nameof(RoundAction)}.");
            }

            ValidatePossibleAction(roundAction);

            return roundAction.ActionIntent switch
            {
                RoundActionIntent.Play => ExecutePlay(roundAction.CardIndexes),
                RoundActionIntent.Discard => ExecuteDiscard(roundAction.CardIndexes),
                _ => throw new ArgumentOutOfRangeException(nameof(roundAction), roundAction, null)
            };
        }

        public override IGamePhaseState GetNextPhaseState()
        {
            if (GameContext.GamePhaseStates.ContainsKey(typeof(ShopState)))
            {
                return GameContext.GamePhaseStates[typeof(ShopState)];
            }

            var shopState = new ShopState(GameContext);
            GameContext.GamePhaseStates[typeof(ShopState)] = shopState;
            return shopState;
        }

        /// <summary>
        /// Draw cards until hand reaches the hand size.
        /// </summary>
        private void DrawCards()
        {
            int need = GameContext.GetHandSize() - GameContext.Hand.Count;
            if (need > 0)
            {
                int draw = Math.Min(need, GameContext.Deck.Count);
                GameContext.Deck.DrawTopTo(draw, GameContext.Hand);
            }
        }

        private bool ExecuteDiscard(ReadOnlySpan<int> cardIndexes)
        {
            // Commit action
            Discards--;
            
            // Create the card view of the discarded cards
            Span<CardView> discardedCardViews = stackalloc CardView[cardIndexes.Length];
            GameContext.Hand.FillCardViews(GameContext, discardedCardViews, cardIndexes);
            
            // Execute and publish the event
            GameContext.GameEventBus.PublishHandDiscarded(discardedCardViews);
            GameContext.Hand.MoveMany(cardIndexes, GameContext.DiscardPile); // hand -> discard
            
            DrawCards();
            return false;
        }

        private bool ExecutePlay(ReadOnlySpan<int> cardIndexes)
        {
            Hands--;
            GameContext.Hand.MoveMany(cardIndexes, GameContext.PlayContainer); // hand -> play
            var scoreContext = ScoringCalculation.EvaluateHand(GameContext);
            LastPlayedHand = scoreContext.HandRank;
            CurrentChipsScore += scoreContext.Chips * scoreContext.MultNumerator / scoreContext.MultDenominator;
            GameContext.PlayContainer.MoveAllTo(GameContext.DiscardPile); // play -> discard
            GameContext.PersistentState.NumberOfHandsPlayed += Hands;
            
            // Handle game over condition
            if (Hands == 0 && CurrentChipsScore < CurrentChipsRequirement)
            {
                GameContext.NotifyLoss();
            }
            
            // Handle transition to the next phase
            var isPhaseOver = IsPhaseOver;

            if (!isPhaseOver)
            {
                DrawCards(); // We DO NOT DRAW if we won
            }

            return isPhaseOver;
        }

        private void TriggerEndOfRoundEvent()
        {
            // Trigger blue seal, gold cards, etc..
            ScoringCalculation.TriggerHandCardsEndOfRound(GameContext, LastPlayedHand!.Value);
            
            // Trigger end of round event for jokers
            foreach (var joker in GameContext.JokerContainer.Jokers)
            {
                joker.OnRoundEnd(GameContext);
            }
            
            // Interest and money from hands left
            var goldFromHandsLeft = GameContext.PersistentState.EconomyHandler.GetGoldPerHandLeft() * Hands;
            var goldFromInterest = GameContext.PersistentState.EconomyHandler.CalculateInterest();
            var blindReward = GameContext.PersistentState.EconomyHandler.CalculateRoundGold();
            
            GameContext.PersistentState.EconomyHandler.AddGold(goldFromHandsLeft + goldFromInterest + blindReward);
        }
        
        private void ValidatePossibleAction(RoundAction action)
        {
            switch (action.ActionIntent)
            {
                case RoundActionIntent.Discard:
                    if (Discards <= 0)
                    {
                        throw new InvalidOperationException("No discards left.");
                    }
                    break;
                case RoundActionIntent.Play:
                    if (Hands <= 0)
                    {
                        throw new InvalidOperationException("No hands left.");
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(action.ActionIntent), action.ActionIntent, null);
            }
        }
    }
}