using System.Runtime.InteropServices;
using System.Text;
using Balatro.Core.CoreObjects.Cards.CardObject;
using Balatro.Core.CoreRules.CanonicalViews;
using Balatro.Core.GameEngine.GameStateController;

namespace Balatro.Core.CoreObjects.Cards.CardsContainer
{
    public abstract class CardContainer
    {
        protected readonly List<Card32> Cards = [];
        public ushort Capacity { get; set; }

        protected CardContainer(ushort capacity)
        {
            Capacity = capacity;
        }
        
        // Fast helpers
        protected Span<Card32> Span => CollectionsMarshal.AsSpan(Cards);
        public int   Count      => Cards.Count;
        public bool  IsEmpty    => Cards.Count == 0;
        public List<Card32> GetCards() => Cards;


        public void Add(Card32 c)
        {
            if (Cards.Count < Capacity)
            {
                Cards.Add(c);
            }
            else
            {
                throw new InvalidOperationException("Container is full");
            }
        }

        public void AddMany(ReadOnlySpan<Card32> cards)
        {
            foreach (var c in cards)
            {
                Add(c);
            }
        }

        /// <summary>
        /// Will transform a card by creating a copy of it.
        /// Probably faster than exposing a reference and mutating it since card is 4Bytes.
        /// </summary>
        public void TransformCard(Func<Card32, Card32> transform, int index)
        {
            Span[index] = transform(Span[index]);
        }
        
        /// <summary>
        /// Remove indices that are already sorted descending.
        /// </summary>
        public void RemoveSortedDescending(ReadOnlySpan<int> indices)
        {
            foreach (int i in indices)
            {
                Cards.RemoveAt(i); // Tail-shifting
            }
        }
        
        public void MoveCardTo(int index, CardContainer target)
        {
            target.Add(Cards[index]);
            Cards.RemoveAt(index);
        }
        
        /// <remarks>
        /// Accepts any <see cref="ReadOnlySpan{T}"/> of indices
        /// (int, byte, short… whatever).  Sorts into a stackalloc buffer if not ordered by descending
        /// </remarks>
        public void MoveMany<T>(ReadOnlySpan<T> indices, CardContainer target)
            where T : unmanaged, IConvertible
        {
            if (indices.IsEmpty) return;

            // ensure descending order so RemoveAt doesn't re-index earlier picks
            Span<int> tmp = indices.Length <= 32
                ? stackalloc int[indices.Length]
                : new int[indices.Length];

            for (int i = 0; i < indices.Length; i++)
                tmp[i] = Convert.ToInt32(indices[i]);

            tmp.Sort();
            tmp.Reverse();

            foreach (int i in tmp)
                target.Add(Cards[i]);

            RemoveSortedDescending(tmp);
        }
        
        public void MoveAllTo(CardContainer target)
        {
            target.AddMany(Span);
            Cards.Clear();
        }

        public void MoveAllCardsTo(CardContainer target)
        {
            target.AddMany(Span);
            Cards.Clear();
        }

        public string Display()
        {
            var sb = new StringBuilder();
            foreach (ref readonly var c in Span)
                sb.AppendLine(c.Representation());
            return sb.ToString();
        }

        public void FillCardViews(
            GameContext ctx,
            Span<CardView> cardViews,
            bool inPlay)
        {
            var src = Span;
            
            // Throw if the buffer has not been initialized to fit exactly the container.
            if (src.Length != Count)
                throw new ArgumentOutOfRangeException(nameof(src), "CardContainer has a different number of cards than the destination buffer.");
            
            for (var i = src.Length; i > 0; i--)
            {
                cardViews[i] = CardView.Create(src[i], ctx);
            }
        }
    }
}