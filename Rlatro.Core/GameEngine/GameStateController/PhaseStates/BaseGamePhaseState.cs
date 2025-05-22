using Balatro.Core.GameEngine.Contracts;
using Balatro.Core.GameEngine.GameStateController.PhaseActions;
using Balatro.Core.GameEngine.GameStateController.PhaseActions.ActionIntents;
using Balatro.Core.GameEngine.StateController;

namespace Balatro.Core.GameEngine.GameStateController.PhaseStates
{
    public abstract class BaseGamePhaseState : IGamePhaseState
    {
        protected GameContext GameContext { get; }
        public abstract GamePhase Phase { get; }
        
        public BaseGamePhaseState(GameContext gameContext)
        {
            GameContext = gameContext;
        }
        
        public bool HandleAction(BasePlayerAction action)
        {
            if (action.SharedActionIntent is not null)
            {
                return HandleSharedActionIntent(action);
            }

            return HandleStateSpecificAction(action);
        }
        
        protected abstract bool HandleStateSpecificAction(BasePlayerAction action);

        public abstract IGamePhaseState GetNextPhaseState();

        private bool HandleSharedActionIntent(BasePlayerAction action)
        {
            ValidatePossibleAction(action);
            
            switch (action.SharedActionIntent)
            {
                case SharedActionIntent.SellConsumable:
                    return ExecuteSellConsumable(action.ConsumableIndex);
                case SharedActionIntent.UseConsumable:
                    return ExecuteUseConsumable(action);
                case SharedActionIntent.SellJoker:
                    return ExecuteSellJoker(action.JokerIndex);
                default:
                    throw new NotImplementedException($"Unhandled action intent: {action.SharedActionIntent}");
            }
        }
        
        private bool ExecuteSellConsumable(int consumableIndex)
        {
            var sellValue = GameContext.ConsumableContainer.Consumables[consumableIndex].SellValue;
            
            GameContext.GameEventBus.PublishConsumableRemovedFromContext(consumableIndex);
            GameContext.ConsumableContainer.RemoveConsumable(consumableIndex);

            GameContext.PersistentState.Gold += sellValue;
            return false;
        }

        private bool ExecuteUseConsumable(BasePlayerAction action)
        {
            var consumable = GameContext.ConsumableContainer.Consumables[action.ConsumableIndex];
            consumable.Apply(GameContext, action.CardIndexes);
            
            GameContext.GameEventBus.PublishConsumableRemovedFromContext(consumable.StaticId);
            GameContext.ConsumableContainer.RemoveConsumable(action.ConsumableIndex);

            return false;
        }

        private bool ExecuteSellJoker(int jokerIndex)
        {
            var joker = GameContext.JokerContainer.Jokers[jokerIndex];
            var sellValue = ComputationHelpers.ComputeSellValue(GameContext, joker.BaseSellValue, joker.BonusSellValue);
            
            GameContext.GameEventBus.PublishJokerRemovedFromContext(joker.StaticId);
            GameContext.JokerContainer.RemoveJoker(GameContext, jokerIndex);

            GameContext.PersistentState.Gold += sellValue;
            return false;
        }

        private void ValidatePossibleAction(BasePlayerAction action)
        {
            if (action.SharedActionIntent is null) return;
            
            switch (action.SharedActionIntent)
            {
                case SharedActionIntent.SellConsumable:
                    if (GameContext.ConsumableContainer.Consumables.Count <= action.ConsumableIndex)
                    {
                        throw new ArgumentOutOfRangeException(nameof(action.ConsumableIndex), action.ConsumableIndex, "Cannot sell consumable");
                    }
                    break;
                case SharedActionIntent.UseConsumable:
                    if (GameContext.ConsumableContainer.Consumables.Count <= action.ConsumableIndex)
                    {
                        throw new ArgumentOutOfRangeException(nameof(action.ConsumableIndex), action.ConsumableIndex, "Cannot use consumable");
                    }
                    break;
                case SharedActionIntent.SellJoker:
                    if (GameContext.JokerContainer.Jokers.Count <= action.JokerIndex)
                    {
                        throw new ArgumentOutOfRangeException(nameof(action.JokerIndex), action.JokerIndex, "Cannot sell joker");
                    }
                    break;
            }
        }
    }
}