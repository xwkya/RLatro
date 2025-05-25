namespace Balatro.Core.CoreObjects.Cards.CardsContainer
{
    public class Deck : CardContainer
    {
        public void DrawTopTo(int count, CardContainer target)
        {
            if (count <= 0) return;
            if (count > Count)
                throw new ArgumentOutOfRangeException(nameof(count));
            
            int start = Count - count;
            var slice = Span.Slice(start, count);   // view into List’s tail
            target.AddMany(slice);
            Cards.RemoveRange(start, count);
        }
    }
}