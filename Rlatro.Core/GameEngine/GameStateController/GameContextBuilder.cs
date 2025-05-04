using System.Runtime.InteropServices;
using Balatro.Core.Contracts.Factories;
using Balatro.Core.CoreObjects;
using Balatro.Core.CoreObjects.Cards.CardObject;
using Balatro.Core.CoreObjects.Cards.CardsContainer;
using Balatro.Core.CoreObjects.Consumables.ConsumablesContainer;
using Balatro.Core.CoreObjects.Jokers.Joker;
using Balatro.Core.CoreObjects.Jokers.JokersContainer;
using Balatro.Core.GameEngine.Contracts;
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
            var persistentState = new PersistentState()
            {
                Discards = 4,
                Hands = 4,
                Gold = 10,
                HandSize = 8
            };

            var gameContext = new GameContext()
            {
                JokerContainer = new JokerContainer(),
                ConsumableContainer = new ConsumableContainer(),
                DiscardPile = new DiscardPile(),
                PlayContainer = new PlayContainer(),
                Hand = new Hand((ushort)persistentState.HandSize),
                Round = 1,
                Deck = new Deck(),
                PersistentState = new PersistentState(),
                RngController = new RngController(seed),
                ObjectsFactory = new CoreObjectsFactory(),
            };
            
            return new GameContextBuilder()
            {
                GameContext = gameContext,
            };
        }
        
        public GameContextBuilder WithDeck(IDeckFactory deckFactory)
        {
            GameContext.Deck = deckFactory.CreateDeck(GameContext.ObjectsFactory);
            return this;
        }

        public GameContextBuilder WithJoker(JokerObject joker)
        {
            GameContext.JokerContainer.Jokers.Add(joker);
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
            
            return GameContext;
        }
    }
}