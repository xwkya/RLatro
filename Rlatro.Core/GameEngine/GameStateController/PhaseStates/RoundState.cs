using Balatro.Core.CoreObjects.Cards.CardObject;
using Balatro.Core.CoreObjects.CoreEnums;
using Balatro.Core.CoreRules.CanonicalViews;
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
        public byte Hands { get; set; }
        public byte Discards { get; set; }
        public byte HandSize { get; set; }
        public uint CurrentChipsScore { get; set; } // TODO: This will be a problem for hands above 4T chips
        public uint CurrentChipsRequirement { get; set; }
        public bool IsPhaseOver => (CurrentChipsScore >= CurrentChipsRequirement) || (Hands == 0) || (HandSize <= 0);

        public bool HandleAction(GameContext context, BasePlayerAction action)
        {
            if (action is not RoundAction roundAction)
            {
                throw new ArgumentException($"Action {action} is not a RoundAction.");
            }

            if (!IsActionPossible(context, roundAction))
            {
                throw new InvalidOperationException($"Action {roundAction} is not possible in the current state.");
            }

            return roundAction.ActionIntent switch
            {
                RoundActionIntent.Play => ExecutePlay(context, roundAction.CardIndexes),
                RoundActionIntent.Discard => ExecuteDiscard(context, roundAction.CardIndexes),
                RoundActionIntent.SellConsumable => ExecuteSellConsumable(context, roundAction.ConsumableIndex),
                RoundActionIntent.UseConsumable => ExecuteUseConsumable(context, roundAction),
                RoundActionIntent.SellJoker => ExecuteSellJoker(context, roundAction.JokerIndex),
                _ => throw new ArgumentOutOfRangeException(nameof(roundAction), roundAction, null)
            };
        }

        /// <summary>
        /// Draw cards until hand reaches the hand size.
        /// </summary>
        private void DrawCards(GameContext ctx)
        {
            int need = HandSize - ctx.Hand.Count;
            if (need > 0)
            {
                int draw = Math.Min(need, ctx.Deck.Count);
                ctx.Deck.DrawTopTo(draw, ctx.Hand);
            }
        }

        private bool ExecuteDiscard(GameContext ctx, ReadOnlySpan<byte> cardIndexes)
        {
            // Commit action
            Discards--;

            ctx.Deck.MoveMany(cardIndexes, ctx.DiscardPile); // hand -> discard
            DrawCards(ctx);
            return false;
        }

        private bool ExecutePlay(GameContext ctx, ReadOnlySpan<byte> cardIndexes)
        {
            Hands--;
            ctx.Hand.MoveMany(cardIndexes, ctx.PlayContainer); // hand -> play
            var scoreContext = ScoringCalculation.EvaluateHand(ctx);
            ctx.Hand.MoveMany(cardIndexes, ctx.DiscardPile); // hand -> discard
            DrawCards(ctx);

            return Hands == 0 || ctx.Hand.Count == 0;
        }

        private bool ExecuteSellConsumable(GameContext ctx, byte consumableIndex)
        {
            var sellValue = ctx.ConsumableContainer.Consumables[consumableIndex].SellValue;
            ctx.ConsumableContainer.RemoveConsumable(consumableIndex);

            ctx.PersistentState.Gold += sellValue;
            return false;
        }

        private bool ExecuteUseConsumable(GameContext ctx, RoundAction action)
        {
            var consumable = ctx.ConsumableContainer.Consumables[action.ConsumableIndex];
            consumable.ApplyEffect(ctx, action.CardIndexes);
            ctx.ConsumableContainer.RemoveConsumable(action.ConsumableIndex);

            return false;
        }

        private bool ExecuteSellJoker(GameContext ctx, byte jokerIndex)
        {
            var joker = ctx.JokerContainer.Jokers[jokerIndex];
            var sellValue = ComputationHelpers.ComputeSellValue(ctx, joker.BaseSellValue, joker.BonusSellValue);
            ctx.JokerContainer.RemoveJoker(ctx, jokerIndex);

            ctx.PersistentState.Gold += sellValue;
            return false;
        }
        
        private bool IsActionPossible(GameContext context, RoundAction action)
        {
            return action.ActionIntent switch
            {
                RoundActionIntent.Discard => Discards > 0,
                RoundActionIntent.SellConsumable => context.ConsumableContainer.Consumables.Count >
                                                    action.ConsumableIndex,
                RoundActionIntent.UseConsumable => context.ConsumableContainer.Consumables.Count >
                                                   action.ConsumableIndex
                                                   && context.ConsumableContainer.Consumables[action.ConsumableIndex]
                                                       .IsUsable(context, action.CardIndexes),
                RoundActionIntent.Play => Hands > 0, // Should always be possible, if Hands == 0 the game is over
                RoundActionIntent.SellJoker => context.JokerContainer.Jokers.Count > action.JokerIndex,
                _ => throw new ArgumentOutOfRangeException(nameof(action), action, null)
            };
        }
    }
}