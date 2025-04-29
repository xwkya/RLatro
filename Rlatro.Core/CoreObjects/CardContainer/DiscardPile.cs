namespace Balatro.Core.CoreObjects.CardContainer
{
    public class DiscardPile : CardContainer
    {
        public override int Capacity()
        {
            return int.MaxValue;
        }
    }
}