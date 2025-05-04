using System.Text;

namespace Balatro.Core.GameEngine.PseudoRng
{
    public interface ISortId
    {
        int? SortId { get; }
    }

    /// <summary>
    /// Copy of the Balatro’s pseudorandom helpers.
    /// </summary>
    public sealed class BalatroRng
    {
        public BalatroRng(string gameSeed)
        {
            _seedString = gameSeed;
            _hashedSeed = Pseudohash(gameSeed);
        }

        readonly double _hashedSeed;
        readonly string _seedString;

        // per-key rolling values   (Lua: G.GAME.pseudorandom)
        readonly Dictionary<string, double> _table = new();


        // -- PUBLIC API --
        // -- doubles --
        public double NextDouble(string key) => RawRng(key).NextDouble();
        public double NextDouble(RngActionType a) => NextDouble(a.Key());

        // -- ints --
        public int NextInt(string key, int min, int max)
            => (int)RawRng(key).Next((ulong)min, (ulong)max);

        public int NextInt(RngActionType a, int min, int max)
            => NextInt(a.Key(), min, max);

        // -- shuffle --
        public void Shuffle<T>(IList<T> list, string key)
        {
            var rng = RawRng(key);

            if (list.Count > 0 && list[0] is ISortId)
                list = list.Cast<ISortId>()
                    .OrderBy(e => e.SortId ?? 1)
                    .Cast<T>().ToList(); // stable pre-sort

            for (int i = list.Count - 1; i >= 1; --i)
            {
                int j = (int)rng.Next((ulong)(i + 1)) - 1; // Lua is 1-based
                (list[i], list[j]) = (list[j], list[i]);
            }
        }
        
        /// <summary>
        /// Convenience method to feed in <see cref="RngActionType"/> instead of a string.
        /// </summary>
        public void Shuffle<T>(IList<T> list, RngActionType a) => Shuffle(list, a.Key());

        // -- random element --
        public (V value, K key) RandomElement<K, V>(IDictionary<K, V> dict, string key)
        {
            var rng = RawRng(key);
            var ordered = dict.First().Value is ISortId
                ? dict.OrderBy(kv => ((ISortId)kv.Value!).SortId ?? int.MaxValue).ToArray()
                : dict.OrderBy(kv => kv.Key).ToArray();

            var pick = ordered[(int)rng.Next((ulong)ordered.Length)];
            return (pick.Value, pick.Key);
        }

        public (V value, K key) RandomElement<K, V>(IDictionary<K, V> dict, RngActionType a)
            => RandomElement(dict, a.Key());

        // -- random string --
        public string RandomString(int length, string key)
        {
            var rng = RawRng(key);
            var sb = new StringBuilder(length);
            for (int i = 0; i < length; ++i)
            {
                double g = rng.NextDouble();
                int c = g > 0.7 ? RandIn(rng, '1', '9')
                    : g > 0.45 ? RandIn(rng, 'A', 'N')
                    : RandIn(rng, 'P', 'Z');
                sb.Append((char)c);
            }

            return sb.ToString().ToUpperInvariant();

            static int RandIn(LuaRandom r, char lo, char hi)
                => (int)r.Next((ulong)lo, (ulong)hi);
        }

        public string RandomString(int length, RngActionType a)
            => RandomString(length, a.Key());


        // -- INTERNALS --
        private LuaRandom RawRng(string key)
        {
            double seed01 = Pseudoseed(key);
            ulong lo = (ulong)(seed01 * ulong.MaxValue);
            return new LuaRandom(lo); // hi part 0
        }

        private double Pseudoseed(string key)
        {
            if (!_table.TryGetValue(key, out double x))
                x = Pseudohash(key + _seedString);

            x = Math.Abs(Trim((2.134453429141 + x * 1.72431234) % 1));
            _table[key] = x;
            return (x + _hashedSeed) / 2;
        }

        // Balatro’s pseudohash
        private static double Pseudohash(string str)
        {
            double num = 1.0;
            for (int i = str.Length - 1; i >= 0; --i)
            {
                num = (1.1239285023 / num) * str[i] * Math.PI + Math.PI * (i + 1);
                num %= 1.0;
            }

            return num;
        }

        private static double Trim(double d) => double.Parse(d.ToString("0.#############"));
    }
}