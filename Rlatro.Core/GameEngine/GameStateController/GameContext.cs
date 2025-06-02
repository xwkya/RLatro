using Balatro.Core.CoreObjects;
using Balatro.Core.CoreObjects.Cards.CardsContainer;
using Balatro.Core.CoreObjects.Consumables.ConsumablesContainer;
using Balatro.Core.CoreObjects.Jokers.JokersContainer;
using Balatro.Core.CoreObjects.Pools;
using Balatro.Core.CoreObjects.Tags;
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
        public TagHandler TagHandler { get; set; }
        private Dictionary<Type, IGamePhaseState> GamePhaseStates = new Dictionary<Type, IGamePhaseState>();
        public bool IsGameOver { get; private set; }

        public void NotifyLoss()
        {
            IsGameOver = true;
        }

        public int GetHandSize()
        {
            return PersistentState.GetCurrentHandSize();
        }

        public int GetDiscards()
        {
            return PersistentState.GetCurrentDiscards();
        }

        public int GetHands()
        {
            return PersistentState.GetCurrentHands();
        }

        public T GetPhase<T>()
        {
            return (T)GamePhaseStates[typeof(T)];
        }
        
        public void InitializeStateCache()
        {
            GamePhaseStates[typeof(RoundState)] = new RoundState(this);
            GamePhaseStates[typeof(BlindSelectionState)] = new BlindSelectionState(this);
            GamePhaseStates[typeof(ShopState)] = new ShopState(this);
            GamePhaseStates[typeof(ArcanaPackState)] = new ArcanaPackState(this);
            GamePhaseStates[typeof(CardPackState)] = new CardPackState(this);
            GamePhaseStates[typeof(JokerPackState)] = new JokerPackState(this);
            GamePhaseStates[typeof(PlanetPackState)] = new PlanetPackState(this);
            GamePhaseStates[typeof(SpectralPackState)] = new SpectralPackState(this);
        }

        public void ResetPhaseStates()
        {
            foreach (var phaseState in GamePhaseStates.Values)
            {
                phaseState.Reset();
            }
        }
    }
}