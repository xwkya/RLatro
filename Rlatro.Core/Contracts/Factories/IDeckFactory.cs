using Balatro.Core.CoreObjects;
using Balatro.Core.CoreObjects.Cards.CardsContainer;

namespace Balatro.Core.Contracts.Factories
{
    public interface IDeckFactory
    {
        public Deck CreateDeck(CoreObjectsFactory objectsFactory);
    }
}