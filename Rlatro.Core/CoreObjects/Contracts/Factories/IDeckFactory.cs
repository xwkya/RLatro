using Balatro.Core.CoreObjects.CardContainer.DeckImplementation;

namespace Balatro.Core.CoreObjects.Contracts.Factories
{
    public interface IDeckFactory
    {
        public Deck CreateDeck();
    }
}