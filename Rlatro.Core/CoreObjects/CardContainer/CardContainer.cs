using System.Runtime.InteropServices;
using System.Text;
using Balatro.Core.CoreObjects.Card;

namespace Balatro.Core.CoreObjects.CardContainer
{
    public abstract class CardContainer
    {
        protected List<Card32> Cards { get; set; }
        public abstract int Capacity();

        protected CardContainer()
        {
            Cards = new List<Card32>();
        }

        public void Add(Card32 c)
        {
            if (Cards.Count < Capacity())
            {
                Cards.Add(c);
            }
            else
            {
                throw new InvalidOperationException("Container is full");
            }
        }

        public void AddMany(IEnumerable<Card32> cards)
        {
            foreach (var card in cards)
            {
                Add(card);
            }
        }
        
        /// <summary>
        /// Will transform a card by creating a copy of it.
        /// Probably faster than exposing a reference and mutating it since card is 4Bytes.
        /// </summary>
        public void TransformCard(Func<Card32, Card32> transform, int index)
        {
            if (index < 0 || index >= Cards.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "Index out of range");
            }

            Cards[index] = transform(Cards[index]);
        }

        public void Remove(int[] index)
        {
            foreach (var i in index.OrderByDescending(x => x))
            {
                if (i < 0 || i >= Cards.Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(index), "Index out of range");
                }

                Cards.RemoveAt(i);
            }
        }

        public void MoveCardTo(int idx, CardContainer target)
        {
            target.Add(Cards[idx]);
            Cards.RemoveAt(idx);
        }

        public void MoveAllCardsTo(CardContainer target)
        {
            target.AddMany(Cards);
            Cards.Clear();
        }

        public string Display()
        {
            var stringBuilder = new StringBuilder();
            foreach (var card in Cards)
            {
                stringBuilder.AppendLine(card.Representation());
            }
            
            return stringBuilder.ToString();
        }

        private Span<Card32> GetCardsSpan()
        {
            return CollectionsMarshal.AsSpan(Cards);
        }
    }
}