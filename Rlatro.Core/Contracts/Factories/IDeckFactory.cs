using Balatro.Core.CoreObjects;
using Balatro.Core.CoreObjects.Cards.CardsContainer;
using Balatro.Core.ObjectsImplementations.Decks;

namespace Balatro.Core.Contracts.Factories
{
    public interface IDeckFactory
    {
        public void InitializeDeck(Deck deck, CoreObjectsFactory objectsFactory);
        public InitialConfiguration Configuration { get; }
    }
}