using Balatro.Core.CoreRules.Scoring;
using Balatro.Core.GameEngine.GameStateController.PhaseActions;
using Balatro.Core.GameEngine.StateController;

namespace Balatro.Core.GameEngine.GameStateController.PhaseStates
{
    /// <summary>
    /// Tracks the current round state.
    /// </summary>
    public record RoundState : IGamePhaseState
    {
        public GamePhase Phase => GamePhase.Round;
        public int Hands { get; set; }
        public int Discards { get; set; }
        public GameContext GameContext { get; set; }
        public uint CurrentChipsScore { get; set; }  // TODO: This will be a problem for hands above 4T chips
        public uint CurrentChipsRequirement { get; set; }
        public bool IsPhaseOver => (CurrentChipsScore >= CurrentChipsRequirement) || (Hands == 0) || (GameContext.GetHandSize() <= 0);

        public RoundState(GameContext ctx)
        {
            GameContext = ctx;
            CurrentChipsScore = 0;
            Hands = GameContext.GetHands();
            Discards = GameContext.GetDiscards();
        }
        
        public bool HandleAction(BasePlayerAction action)
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
                RoundActionIntent.SellConsumable => ExecuteSellConsumable(roundAction.ConsumableIndex),
                RoundActionIntent.UseConsumable => ExecuteUseConsumable(roundAction),
                RoundActionIntent.SellJoker => ExecuteSellJoker(roundAction.JokerIndex),
                _ => throw new ArgumentOutOfRangeException(nameof(roundAction), roundAction, null)
            };
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

        private bool ExecuteDiscard(ReadOnlySpan<byte> cardIndexes)
        {
            // Commit action
            Discards--;
            GameContext.Deck.MoveMany(cardIndexes, GameContext.DiscardPile); // hand -> discard
            DrawCards();
            return false;
        }

        private bool ExecutePlay(ReadOnlySpan<byte> cardIndexes)
        {
            Hands--;
            GameContext.Hand.MoveMany(cardIndexes, GameContext.PlayContainer); // hand -> play
            var scoreContext = ScoringCalculation.EvaluateHand(GameContext);
            CurrentChipsScore += scoreContext.Chips * scoreContext.MultNumerator / scoreContext.MultDenominator;
            GameContext.PlayContainer.MoveMany(cardIndexes, GameContext.DiscardPile); // play -> discard
            DrawCards();

            return Hands == 0 || GameContext.Hand.Count == 0 || CurrentChipsScore >= CurrentChipsRequirement;
        }

        private bool ExecuteSellConsumable(byte consumableIndex)
        {
            var sellValue = GameContext.ConsumableContainer.Consumables[consumableIndex].SellValue;
            GameContext.ConsumableContainer.RemoveConsumable(consumableIndex);

            GameContext.PersistentState.Gold += (int)sellValue;
            return false;
        }

        private bool ExecuteUseConsumable(RoundAction action)
        {
            var consumable = GameContext.ConsumableContainer.Consumables[action.ConsumableIndex];
            consumable.ApplyEffect(GameContext, action.CardIndexes);
            GameContext.ConsumableContainer.RemoveConsumable(action.ConsumableIndex);

            return false;
        }

        private bool ExecuteSellJoker(byte jokerIndex)
        {
            var joker = GameContext.JokerContainer.Jokers[jokerIndex];
            var sellValue = ComputationHelpers.ComputeSellValue(GameContext, joker.BaseSellValue, joker.BonusSellValue);
            GameContext.JokerContainer.RemoveJoker(GameContext, jokerIndex);

            GameContext.PersistentState.Gold += sellValue;
            return false;
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
                case RoundActionIntent.SellConsumable:
                    if (GameContext.ConsumableContainer.Consumables.Count <= action.ConsumableIndex)
                    {
                        throw new ArgumentOutOfRangeException(nameof(action.ConsumableIndex), action.ConsumableIndex, "Cannot sell consumable");
                    }
                    break;
                case RoundActionIntent.UseConsumable:
                    if (GameContext.ConsumableContainer.Consumables.Count <= action.ConsumableIndex)
                    {
                        throw new ArgumentOutOfRangeException(nameof(action.ConsumableIndex), action.ConsumableIndex, "Cannot use consumable");
                    }
                    break;
                case RoundActionIntent.Play:
                    if (Hands <= 0)
                    {
                        throw new InvalidOperationException("No hands left.");
                    }
                    break;
                case RoundActionIntent.SellJoker:
                    if (GameContext.JokerContainer.Jokers.Count <= action.JokerIndex)
                    {
                        throw new ArgumentOutOfRangeException(nameof(action.JokerIndex), action.JokerIndex, "Cannot sell joker");
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(action.ActionIntent), action.ActionIntent, null);
            }
        }
    }
}