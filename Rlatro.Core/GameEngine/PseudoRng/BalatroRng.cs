namespace Balatro.Core.GameEngine.PseudoRng
{
    /// <summary>
    /// Copy of the Balatro’s pseudorandom helpers.
    /// </summary>
    public sealed class BalatroRng
    {
        public BalatroRng(string gameSeed)
        {
            GameSeed = gameSeed;
            InitializeLuaRandoms();
        }

        readonly string GameSeed;

        // per-key LuaRandom
        readonly Dictionary<string, LuaRandom> KeyToRandomTable = new();

        // -- PUBLIC API --
        // -- doubles --
        public double NextDouble(string key) => RawRng(key).NextDouble();
        public double NextDouble(RngActionType a) => NextDouble(a.Key());

        // -- ints --
        public int NextInt(string key, int min, int max)
            => (int)RawRng(key).Next((ulong)min, (ulong)max);

        public int NextInt(RngActionType a, int min, int max)
            => NextInt(a.Key(), min, max);

        /// <summary>
        /// Shuffle index span in place assuming indexes is a permutation of [0, 1, ..., n-1].
        /// </summary>
        public void ShuffleContiguous(in Span<int> indexes, uint[] keys, RngActionType key)
        {
            if (indexes.Length != keys.Length)
            {
                throw new ArgumentException("list and keys Spans must have the same length.");
            }

            int n = indexes.Length;
            if (n == 0)
            {
                return; // Nothing to do for an empty list
            }
            
            // Sort the indexes based on the keys
            indexes.Sort((indexA, indexB) => keys[indexA].CompareTo(keys[indexB]));
            
            // Perform Fisher-Yates shuffle on the sorted indexes
            var rng = RawRng(key.ToString());
            for (int i = indexes.Length - 1; i >= 1; --i)
            {
                int j = (int)rng.Next((ulong)i + 1) - 1; // To reproduce the Lua behavior (1 based)
                (indexes[i], indexes[j]) = (indexes[j], indexes[i]);
            }
        }
        
        public void Shuffle(in Span<int> indexes, RngActionType key)
        {
            // Perform Fisher-Yates shuffle on the sorted indexes
            var rng = RawRng(key.ToString());
            for (int i = indexes.Length - 1; i >= 1; --i)
            {
                int j = (int)rng.Next((ulong)i + 1) - 1; // To reproduce the Lua behavior (1 based)
                (indexes[i], indexes[j]) = (indexes[j], indexes[i]);
            }
        }
        
        /// <summary>
        /// Returns a random index from the provided list of indexes assuming indexes is a permutation of [0, 1, ..., n-1].
        /// </summary>
        public int RandomIndexContiguous(in Span<int> indexes, uint[] keys, RngActionType key)
        {
            if (indexes.Length != keys.Length)
            {
                throw new ArgumentException("list and keys Spans must have the same length.");
            }
            
            indexes.Sort((indexA, indexB) => keys[indexA].CompareTo(keys[indexB]));
            var rng = RawRng(key.ToString());
            int randomIndex = (int)rng.Next((ulong)indexes.Length) - 1;
            
            return indexes[randomIndex];
        }
        
        /// <summary>
        /// Returns a random index from the provided list of indexes assuming indexes may be out of bounds for keys.
        /// Indexes and keys must still be of the same length.
        /// </summary>
        public int RandomIndexNonContiguous(in Span<int> indexes, uint[] keys, RngActionType key)
        {
            if (indexes.Length != keys.Length)
            {
                throw new ArgumentException("list and keys Spans must have the same length.");
            }
            
            // Create a contiguous index array
            Span<int> contiguousIndexes = stackalloc int[indexes.Length];
            for (int i = 0; i < indexes.Length; i++)
            {
                contiguousIndexes[i] = i;
            }
            
            // Sort the contiguous array of indexes
            contiguousIndexes.Sort((indexA, indexB) => keys[indexA].CompareTo(keys[indexB]));
            var rng = RawRng(key.ToString());
            int randomIndex = (int)rng.Next((ulong)contiguousIndexes.Length) - 1;
            
            // Return the use index
            return indexes[randomIndex];
        }
        
        /// <summary>
        /// Random element without keys (sample from the provided element span as-is)
        /// </summary>
        public int RandomElement(in Span<int> elements, RngActionType key)
        {
            elements.Sort();
            var rng = RawRng(key.ToString());
            int randomIndex = (int)rng.Next((ulong)elements.Length) - 1;
            
            return elements[randomIndex];
        }
        

        private void InitializeLuaRandoms()
        {
            foreach (var action in Enum.GetValues<RngActionType>())
            {
                var key = $"{GameSeed}_{action.Key()}";
                
                if (!KeyToRandomTable.ContainsKey(key))
                {
                    var hashCode = HashString(key);
                    KeyToRandomTable[key] = new LuaRandom(hashCode);
                }
            }
        }
        
        private LuaRandom RawRng(string key)
        {
            var gameKey = $"{GameSeed}_{key}";
            if (!KeyToRandomTable.ContainsKey(key))
            {
                var hashCode = HashString(gameKey);
                KeyToRandomTable[key] = new LuaRandom(hashCode);
            }

            return KeyToRandomTable[key];
        }
        
        private static ulong HashString(string input)
        {
            const ulong FnvOffsetBasis = 14695981039346656037;
            const ulong FnvPrime = 1099511628211;
    
            ulong hash = FnvOffsetBasis;
            foreach (char c in input)
            {
                hash ^= c;
                hash *= FnvPrime;
            }
            return hash;
        }
    }
}