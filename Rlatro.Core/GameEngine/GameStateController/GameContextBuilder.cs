using System.Runtime.InteropServices;
using Balatro.Core.Contracts.Factories;
using Balatro.Core.CoreObjects;
using Balatro.Core.CoreObjects.Cards.CardObject;
using Balatro.Core.CoreObjects.Cards.CardsContainer;
using Balatro.Core.CoreObjects.Consumables.ConsumablesContainer;
using Balatro.Core.CoreObjects.Jokers.Joker;
using Balatro.Core.CoreObjects.Jokers.JokersContainer;
using Balatro.Core.CoreObjects.Pools;
using Balatro.Core.CoreObjects.Tags;
using Balatro.Core.CoreRules.Modifiers;
using Balatro.Core.GameEngine.Contracts;
using Balatro.Core.GameEngine.GameStateController.EventBus;
using Balatro.Core.GameEngine.GameStateController.PersistentStates;
using Balatro.Core.GameEngine.PseudoRng;

namespace Balatro.Core.GameEngine.GameStateController
{
    public class GameContextBuilder : IGameContextFactory
    {
        private GameContext GameContext { get; set; }
        private IDeckFactory DeckFactory { get; set; }
        private List<JokerObject> Jokers { get; set; } = new();
        private List<Card64> CardsInHand { get; set; }
        
        public static GameContextBuilder Create()
        {
            var gameEventBus = new GameEventBus();
            var persistentState = new PersistentState();
            
            var gameContext = new GameContext()
            {
                GameEventBus = gameEventBus,
                JokerContainer = new JokerContainer(),
                ConsumableContainer = new ConsumableContainer { Capacity = 2 },
                DiscardPile = new DiscardPile(),
                PlayContainer = new PlayContainer(),
                Hand = new Hand(),
                Deck = new Deck(),
                PersistentState = persistentState,
                RngController = new RngController(),
                CoreObjectsFactory = new CoreObjectsFactory(),
                TagHandler = new TagHandler(),
            };
            
            // Wire up the event bus
            gameContext.GlobalPoolManager = new GlobalPoolManager(gameContext);
            gameContext.VoucherEffectHandler = new VoucherEffectHandler(gameContext);

            persistentState.Subscribe(gameEventBus);
            gameContext.GlobalPoolManager.Subscribe(gameEventBus);
            gameContext.VoucherEffectHandler.Subscribe(gameEventBus);

            return new GameContextBuilder()
            {
                GameContext = gameContext,
            };
        }
        
        public GameContextBuilder WithDeck(IDeckFactory deckFactory)
        {
            DeckFactory = deckFactory ?? throw new ArgumentNullException(nameof(deckFactory), "Deck factory cannot be null.");
            
            return this;
        }

        public GameContextBuilder WithJoker(JokerObject joker)
        {
            Jokers.Add(joker);
            return this;
        }

        public GameContextBuilder WithHand(List<Card64> cardsInHand)
        {
            CardsInHand = cardsInHand;
            return this;
        }

        public GameContext CreateGameContext(string seed)
        {
            if (DeckFactory is null)
            {
                throw new InvalidOperationException($"Provide a deck factory with {nameof(WithDeck)} is not set.");
            }
            
            GameContext.InitializeStateCache();
            ResetGameContext(seed);

            return GameContext;
        }
        
        private void ResetGameContext(string seed)
        {
            // Initialize the rng controller with the provided seed
            GameContext.CoreObjectsFactory.Reset();
            GameContext.RngController.Initialize(seed);
            
            // Unsubscribe and clear owned jokers
            for (int i = 0; i < GameContext.JokerContainer.Jokers.Count; i++)
            {
                GameContext.JokerContainer.RemoveJoker(GameContext, i);
            }
            GameContext.JokerContainer.Slots = DeckFactory.Configuration.JokerSlots;
            
            // Clear all consumables
            GameContext.ConsumableContainer.Consumables.Clear();
            GameContext.ConsumableContainer.Capacity = 2;
            
            // Clear all containers
            GameContext.Hand.Clear();
            GameContext.DiscardPile.Clear();
            GameContext.PlayContainer.Clear();
            
            // Reinitialize the global pool manager
            GameContext.GlobalPoolManager.InitializePools();
            
            // Reset the persistent state
            GameContext.PersistentState.Reset(DeckFactory.Configuration);
            
            // Reinitialize the phase states
            GameContext.ResetPhaseStates();
            
            // Build the deck
            DeckFactory.InitializeDeck(GameContext.Deck, GameContext.CoreObjectsFactory);
            
            // Add the jokers if any were provided
            Jokers.ForEach(j =>
            {
                GameContext.JokerContainer.AddJoker(GameContext, j);
                GameContext.GameEventBus.PublishJokerAddedToContext(j.StaticId); // We need to publish as it was never in the context before
            });
            
            // Add the cards in hand if any were provided
            GameContext.Hand.AddMany(CollectionsMarshal.AsSpan(CardsInHand));
        }
    }
}