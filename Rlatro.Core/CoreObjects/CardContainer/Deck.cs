namespace Balatro.Core.CoreObjects.CardContainer.DeckImplementation
{
    public class Deck : CardContainer
    {
        public override int Capacity()
        {
            return int.MaxValue;
        }
    }
}