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
using Balatro.Core.GameEngine.Contracts;
using Balatro.Core.GameEngine.GameStateController.EventBus;
using Balatro.Core.GameEngine.GameStateController.PersistentStates;
using Balatro.Core.GameEngine.PseudoRng;

namespace Balatro.Core.GameEngine.GameStateController
{
    public class GameContextBuilder : IGameContextFactory
    {
        private GameContext GameContext { get; set; }
        private GameContextBuilder()
        {
        }
        
        public static GameContextBuilder Create(string seed)
        {
            var gameEventBus = new GameEventBus();
            
            var persistentState = new PersistentState()
            {
                Discards = 4,
                Hands = 4,
                HandSize = 8,
                HandTracker = new HandTracker(),
                Round = 1,
            };
            var startingGold = 10;
            var economyHandler = new EconomyHandler(persistentState, startingGold);
            persistentState.EconomyHandler = economyHandler;

            var gameContext = new GameContext()
            {
                GameEventBus = gameEventBus,
                JokerContainer = new JokerContainer(),
                ConsumableContainer = new ConsumableContainer(),
                DiscardPile = new DiscardPile(),
                PlayContainer = new PlayContainer(),
                Hand = new Hand(),
                Deck = new Deck(),
                PersistentState = persistentState,
                RngController = new RngController(seed),
                CoreObjectsFactory = new CoreObjectsFactory(),
                TagHandler = new TagHandler(),
            };
            
            // Wire up the event bus
            gameContext.GlobalPoolManager = new GlobalPoolManager(gameContext);

            persistentState.Subscribe(gameEventBus);
            gameContext.GlobalPoolManager.Subscribe(gameEventBus);
            
            
            
            return new GameContextBuilder()
            {
                GameContext = gameContext,
            };
        }
        
        public GameContextBuilder WithDeck(IDeckFactory deckFactory)
        {
            GameContext.Deck = deckFactory.CreateDeck(GameContext.CoreObjectsFactory);
            GameContext.JokerContainer.Slots = deckFactory.JokerSlots();
            return this;
        }

        public GameContextBuilder WithJoker(JokerObject joker)
        {
            GameContext.JokerContainer.Jokers.Add(joker);
            return this;
        }
        
        public GameContextBuilder WithJokers(List<JokerObject> jokers)
        {
            GameContext.JokerContainer.Jokers.AddRange(jokers);
            return this;
        }

        public GameContextBuilder WithHand(List<Card64> cardsInHand)
        {
            GameContext.Hand.AddMany(CollectionsMarshal.AsSpan(cardsInHand));
            return this;
        }
        
        public GameContext CreateGameContext()
        {
            if (GameContext.Hand is null)
            {
                throw new InvalidOperationException($"Provide a deck factory with {nameof(WithDeck)} is not set.");
            }
            
            GameContext.GlobalPoolManager.InitializePools();

            
            return GameContext;
        }
    }
}