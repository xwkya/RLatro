using Balatro.Core.CoreObjects;
using Balatro.Core.CoreObjects.Cards.CardsContainer;
using Balatro.Core.CoreObjects.Consumables.ConsumablesContainer;
using Balatro.Core.CoreObjects.Jokers.JokersContainer;
using Balatro.Core.CoreObjects.Pools;
using Balatro.Core.GameEngine.Contracts;
using Balatro.Core.GameEngine.GameStateController.EventBus;
using Balatro.Core.GameEngine.GameStateController.PersistentStates;
using Balatro.Core.GameEngine.GameStateController.PhaseStates;
using Balatro.Core.GameEngine.PseudoRng;

namespace Balatro.Core.GameEngine.GameStateController
{
    public class GameContext
    {
        public RngController RngController { get; set; }
        public Deck Deck { get; set; }
        public Hand Hand { get; set; }
        public DiscardPile DiscardPile { get; set; }
        public PlayContainer PlayContainer { get; set; }
        public JokerContainer JokerContainer { get; set; }
        public ConsumableContainer ConsumableContainer { get; set; }
        public PersistentState PersistentState { get; set; }
        public GlobalPoolManager GlobalPoolManager { get; set; }
        public GameEventBus GameEventBus { get; set; }
        public CoreObjectsFactory CoreObjectsFactory { get; set; }
        public Dictionary<Type, IGamePhaseState> GamePhaseStates = new Dictionary<Type, IGamePhaseState>();

        public int GetHandSize()
        {
            return PersistentState.HandSize;
        }

        public int GetDiscards()
        {
            return PersistentState.Discards;
        }

        public int GetHands()
        {
            return PersistentState.Hands;
        }
    }
}