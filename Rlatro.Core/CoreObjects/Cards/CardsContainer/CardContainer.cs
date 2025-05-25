using System.Runtime.InteropServices;
using System.Text;
using Balatro.Core.CoreObjects.Cards.CardObject;
using Balatro.Core.CoreRules.CanonicalViews;
using Balatro.Core.GameEngine.GameStateController;
using Balatro.Core.GameEngine.PseudoRng;

namespace Balatro.Core.CoreObjects.Cards.CardsContainer
{
    public abstract class CardContainer
    {
        protected readonly List<Card64> Cards = [];
        
        // Fast helpers
        public Span<Card64> Span => CollectionsMarshal.AsSpan(Cards);
        public int   Count      => Cards.Count;
        public bool  IsEmpty    => Cards.Count == 0;

        public void Add(Card64 c)
        {
            Cards.Add(c);
        }

        public void AddMany(ReadOnlySpan<Card64> cards)
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
        public void TransformCard(Func<Card64, Card64> transform, int index)
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
        
        public void MoveMany(ReadOnlySpan<int> indices, CardContainer target)
        {
            if (indices.IsEmpty) return;
            
            foreach (byte i in indices)
                target.Add(Cards[i]);
            
            // ensure descending order so RemoveAt doesn't re-index earlier picks
            Span<int> tmp = indices.Length <= 32
                ? stackalloc int[indices.Length]
                : new int[indices.Length];
            
            for (int i = 0; i < indices.Length; i++)
                tmp[i] = indices[i];
            
            tmp.Sort();
            tmp.Reverse();
            
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
            
            for (var i = 0; i < src.Length; i++)
            {
                cardViews[i] = CardView.Create(src[i], ctx);
            }
        }

        public void FillCardViews(
            GameContext ctx,
            Span<CardView> cardViews,
            ReadOnlySpan<int> cardIndexes)
        {
            for (var i = 0; i < cardIndexes.Length; i++)
            {
                cardViews[i] = CardView.Create(Span[cardIndexes[i]], ctx);
            }
        }
        
        /// <summary>
        /// Randomly permutes the order of <see cref="Cards"/> in-place.
        /// Zero allocation shuffling.
        /// </summary>
        public void Shuffle(RngController rngController)
        {
            Span<int> indexSpan = stackalloc int[Count];
            uint[] runtimeIds = new uint[Count];
            for (int i = 0; i < Count; i++)
            {
                indexSpan[i] = i;
                runtimeIds[i] = Span[i].Id;
            }
            
            rngController.GetShuffle(indexSpan, runtimeIds, RngActionType.Shuffle);
            
            // Reorder the Cards based on the shuffled indexes
            ApplyPermutationInPlace(indexSpan);
        }
        
        /// <summary>
        /// Applies a permutation to the Cards list in-place using cycle decomposition.
        /// Time: O(n), Space: O(1) additional (reuses the indexSpan as a visited marker)
        /// </summary>
        /// <param name="permutation">The permutation to apply, where permutation[i] is the new position of element i</param>
        private void ApplyPermutationInPlace(Span<int> permutation)
        {
            var cards = Span;
    
            for (int start = 0; start < permutation.Length; start++)
            {
                // Skip if this element is already in the correct position or already processed
                if (permutation[start] == start || permutation[start] < 0)
                    continue;
            
                // Follow the cycle starting from 'start'
                var temp = cards[start];
                int current = start;
        
                while (true)
                {
                    int next = permutation[current];
            
                    // Mark current as visited by making it negative
                    permutation[current] = -permutation[current] - 1;
            
                    if (next == start)
                    {
                        // End of cycle, place the temp value
                        cards[current] = temp;
                        break;
                    }
            
                    // Move the card and continue the cycle
                    cards[current] = cards[next];
                    current = next;
                }
            }
        }
    }
}