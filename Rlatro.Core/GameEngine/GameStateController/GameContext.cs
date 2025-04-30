using Balatro.Core.CoreObjects.Cards.CardsContainer;
using Balatro.Core.CoreObjects.Consumables.ConsumablesContainer;
using Balatro.Core.GameEngine.GameStateController.PersistentStates;

namespace Balatro.Core.GameEngine.GameStateController
{
    public class GameContext
    {
        public Deck Deck { get; set; }
        public Hand Hand { get; set; }
        public DiscardPile DiscardPile { get; set; }
        public PlayContainer PlayContainer { get; set; }
        public ConsumableContainer ConsumableContainer { get; set; }
        public PersistentState PersistentState { get; set; }
        public int Round { get; set; }
        public int Ante => Round / 3;

        public byte GetHandSize()
        {
            return PersistentState.HandSize;
        }

        public uint GetCurrentGold()
        {
            return PersistentState.Gold;
        }

        public byte GetDiscards()
        {
            return PersistentState.Discards;
        }

        public byte GetHands()
        {
            return PersistentState.Hands;
        }
    }
}