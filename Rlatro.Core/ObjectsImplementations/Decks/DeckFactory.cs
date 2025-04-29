using Balatro.Core.CoreObjects.Card;
using Balatro.Core.CoreObjects.CardContainer.DeckImplementation;
using Balatro.Core.CoreObjects.Contracts.Factories;

namespace Balatro.Core.ObjectsImplementations.Decks
{
    public class DefaultDeckFactory : IDeckFactory
    {
        public Deck CreateDeck()
        {
            var deck = new Deck();
            foreach (Rank rank in Enum.GetValues<Rank>())
            {
                foreach (Suit suit in Enum.GetValues<Suit>())
                {
                    Card32 card = Card32.Create(rank, suit);
                    deck.Add(card);
                }
            }

            return deck;
        }
    }

    public class CheckeredDeckFactory : IDeckFactory
    {
        public Deck CreateDeck()
        {
            var deck = new Deck();
            foreach (Rank rank in Enum.GetValues<Rank>())
            {
                var heart = Card32.Create(rank, Suit.Heart);
                var spade = Card32.Create(rank, Suit.Spade);
                
                deck.Add(heart);
                deck.Add(heart);
                deck.Add(spade);
                deck.Add(spade);
            }

            return deck;
        }
    }
}