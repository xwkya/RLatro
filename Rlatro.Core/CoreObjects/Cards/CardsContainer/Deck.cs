
namespace Balatro.Core.CoreObjects.Cards.CardsContainer
{
    public class Deck : CardContainer
    {
        /// <summary>
        /// Randomly permutes the order of <see cref="Cards"/> in-place.
        /// Zero allocation shuffling.
        /// </summary>
        /// <param name="rng">
        /// Optional RNG. Pass one in when you need deterministic unit tests;
        /// otherwise we default to <see cref="Random.Shared"/>.
        /// </param>
        public void Shuffle(Random? rng = null)
        {
            (rng ?? Random.Shared).Shuffle(Span);
        }

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