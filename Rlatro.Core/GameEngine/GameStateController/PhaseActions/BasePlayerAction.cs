using Balatro.Core.GameEngine.GameStateController.PhaseActions.ActionIntents;

namespace Balatro.Core.GameEngine.GameStateController.PhaseActions
{
    public abstract class BasePlayerAction
    {
        public SharedActionIntent? SharedActionIntent { get; set; }
        
        /// <summary>
        /// Defines the index of the consumable if the action is to use or sell a consumable
        /// </summary>
        public int ConsumableIndex { get; set; }
        
        /// <summary>
        /// Defines the index of the joker if the action is to sell a joker
        /// </summary>
        public int JokerIndex { get; set; }
        
        /// <summary>
        /// Defines the indexes of the cards that the intent targets
        /// </summary>
        public int[] CardIndexes { get; set; }
    }
}