using Balatro.Core.Contracts.Factories;
using Balatro.Core.CoreObjects;
using Balatro.Core.CoreObjects.Cards.CardObject;
using Balatro.Core.CoreObjects.Cards.CardsContainer;
using Balatro.Core.CoreObjects.CoreEnums;

namespace Balatro.Core.ObjectsImplementations.Decks
{
    public class DefaultDeckFactory : IDeckFactory
    {
        public Deck CreateDeck(CoreObjectsFactory objectsFactory)
        {
            var deck = new Deck();
            foreach (Rank rank in Enum.GetValues<Rank>())
            {
                foreach (Suit suit in Enum.GetValues<Suit>())
                {
                    Card64 card = objectsFactory.CreateCard(rank, suit);
                    deck.Add(card);
                }
            }

            return deck;
        }
    }

    public class CheckeredDeckFactory : IDeckFactory
    {
        public Deck CreateDeck(CoreObjectsFactory objectsFactory)
        {
            var deck = new Deck();
            foreach (Rank rank in Enum.GetValues<Rank>())
            {
                var heart = objectsFactory.CreateCard(rank, Suit.Heart);
                var spade = objectsFactory.CreateCard(rank, Suit.Spade);
                
                deck.Add(heart);
                deck.Add(heart);
                deck.Add(spade);
                deck.Add(spade);
            }

            return deck;
        }
    }
}