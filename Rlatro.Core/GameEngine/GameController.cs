using Balatro.Core.CoreObjects.CardContainer;
using Balatro.Core.CoreObjects.CardContainer.DeckImplementation;
using Balatro.Core.CoreObjects.Contracts.Factories;

namespace Balatro.Core.GameEngine
{
    public class GameController
    {
        private Deck Deck;
        private Hand Hand;
        private DiscardPile DiscardPile;
        
        private GameController()
        {
        }

        public void NewGame(IDeckFactory deckFactory)
        {
            Deck = deckFactory.CreateDeck();
            Hand = new Hand(); // Empty hand
            DiscardPile = new DiscardPile(); // Empty discard pile
        }
    }
}