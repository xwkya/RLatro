using Balatro.Core.Contracts.Factories;
using Balatro.Core.CoreObjects;
using Balatro.Core.CoreObjects.Cards.CardObject;
using Balatro.Core.CoreObjects.Cards.CardsContainer;
using Balatro.Core.CoreObjects.CoreEnums;

namespace Balatro.Core.ObjectsImplementations.Decks
{
    public record InitialConfiguration
    {
        public int JokerSlots { get; init; } = 5;
        public int Hands { get; init; } = 4;
        public int Discards { get; init; } = 3;
        public int HandSize { get; init; } = 8;
        public int StartingGold { get; init; } = 4;
    }
    
    public class RedDeckFactory : IDeckFactory
    {
        private static readonly InitialConfiguration Configurations = new InitialConfiguration
        {
            Discards = 4
        };
        
        public InitialConfiguration Configuration => Configurations;

        public void InitializeDeck(Deck deck, CoreObjectsFactory objectsFactory)
        {
            deck.Clear();
            foreach (Rank rank in Enum.GetValues<Rank>())
            {
                foreach (Suit suit in Enum.GetValues<Suit>())
                {
                    Card64 card = objectsFactory.CreateCard(rank, suit);
                    deck.Add(card);
                }
            }
        }
    }

    public class CheckeredDeckFactory : IDeckFactory
    {
        private static readonly InitialConfiguration Configurations = new InitialConfiguration();
        
        public InitialConfiguration Configuration => Configurations;

        public void InitializeDeck(Deck deck, CoreObjectsFactory objectsFactory)
        {
            foreach (Rank rank in Enum.GetValues<Rank>())
            {
                var heart = objectsFactory.CreateCard(rank, Suit.Heart);
                var spade = objectsFactory.CreateCard(rank, Suit.Spade);

                deck.Add(heart);
                deck.Add(heart);
                deck.Add(spade);
                deck.Add(spade);
            }
        }
    }
}