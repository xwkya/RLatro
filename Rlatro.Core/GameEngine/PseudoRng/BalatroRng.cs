
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
        /// shuffle index span in place assuming indexes is 0, 1, 2,
        /// </summary>
        /// <param name="indexes">Indexes to shuffle </param>
        /// <param name="keys">Keys to sort the array</param>
        /// <param name="key">Action type</param>
        /// <exception cref="ArgumentException"></exception>
        public void Shuffle(in Span<int> indexes, uint[] keys, RngActionType key)
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

        private void InitializeLuaRandoms()
        {
            foreach (var action in Enum.GetValues<RngActionType>())
            {
                var key = $"{GameSeed}_{action.Key()}";
                
                if (!KeyToRandomTable.ContainsKey(key))
                {
                    KeyToRandomTable[key] = new LuaRandom((ulong)key.GetHashCode());
                }
            }
        }
        
        private LuaRandom RawRng(string key)
        {
            var gameKey = $"{GameSeed}_{key}";
            if (!KeyToRandomTable.ContainsKey(key))
            {
                KeyToRandomTable[key] = new LuaRandom((ulong)gameKey.GetHashCode());
            }

            return KeyToRandomTable[key];
        }
    }
}