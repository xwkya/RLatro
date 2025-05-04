using Balatro.Core.GameEngine.GameStateController.PhaseActions;
using Balatro.Core.GameEngine.StateController;

namespace Balatro.Core.GameEngine.GameStateController.PhaseStates
{
    public class ShopState : IGamePhaseState
    {
        public GamePhase Phase => GamePhase.Shop;
        
        /// <summary>
        /// Gets the number of rolls the player has paid -> Chaos free roll should not increment this counter.
        /// </summary>
        public byte NumberOfRollsPaidThisTurn { get; set; }
        
        /// <summary>
        /// Gets the number of free rolls the player has (e.g. from Chaos or tags that grant free rolls).
        /// Theoretically never bigger than 2 so could be a 3 bit packed field.
        /// </summary>
        public byte NumberOfFreeRolls { get; set; }

        public bool IsPhaseOver { get; }
        
        public bool HandleAction(BasePlayerAction action)
        {
            throw new NotImplementedException();
        }
    }
}