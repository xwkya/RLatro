namespace Balatro.Core.GameEngine.GameStateController.PhaseActions
{
    public enum RoundActionIntent
    {
        Play,
        Discard,
        UseConsumable,
        SellConsumable,
        SellJoker,
    }
    
    /// <summary>
    /// A round action is an action that can be performed during a round.
    /// </summary>
    public sealed class RoundAction : BasePlayerAction
    {
        /// <summary>
        /// Defines the intent of the action
        /// </summary>
        public RoundActionIntent ActionIntent { get; init; }
        
        /// <summary>
        /// Defines the indexes of the cards that the intent targets
        /// </summary>
        public byte[] CardIndexes { get; init; }
        
        /// <summary>
        /// Defines the index of the consumable if the action is to use or sell a consumable
        /// </summary>
        public byte ConsumableIndex { get; init; }
        
        /// <summary>
        /// Defines the index of the joker if the action is to sell a joker
        /// </summary>
        public byte JokerIndex { get; init; }
    }
}