using System.Numerics;

namespace Balatro.Core.GameEngine.PseudoRng
{
    /// <summary>
    /// Re-implementation of Lua 5.4/5.5  math.randomseed/math.random
    ///   • xoshiro256**  (Blackman & Vigna)
    ///   • 128-bit seed  (one or two ulong args, second defaults to 0)
    ///   • unbiased integer projection for math.random(m,n)
    /// Behaviour is bit-for-bit identical to upstream lmathlib.c.
    /// </summary>
    public class LuaRandom
    {
        private readonly ulong[] s = new ulong[4];   // 256-bit PRNG state

        static ulong RotL(ulong x, int k) =>
            BitOperations.RotateLeft(x, k);

        /* one raw 64-bit output of xoshiro256** */
        ulong Next64()
        {
            ulong result = RotL(s[1] * 5, 7) * 9;

            ulong t = s[1] << 17;

            s[2] ^= s[0];
            s[3] ^= s[1];
            s[1] ^= s[2];
            s[0] ^= s[3];

            s[2] ^= t;
            s[3] = RotL(s[3], 45);

            return result;
        }

        public LuaRandom(ulong loSeed = 0, ulong hiSeed = 0)
        {
            Seed(loSeed, hiSeed);
        }

        public void Seed(ulong loSeed, ulong hiSeed = 0)
        {
            /*  Lua’s layout after math.randomseed(a,b):
                    s[0] = a
                    s[1] = 0xFF 
                    s[2] = b
                    s[3] = 0
                    burn first 16 outputs to get the state into a
            */
            s[0] = loSeed;
            s[1] = 0xFF;
            s[2] = hiSeed;
            s[3] = 0;

            for (var i = 0; i < 16; ++i) Next64();
        }
        
        public double NextDouble()
        {
            /* Lua takes the top-most 53 bits (width of IEEE 754 mantissa),
               shifts them down, and divides by 2^53. */
            ulong r = Next64() >> 11;
            return r * (1.0 / (1UL << 53));
        }
        
        /* unbiased projection of 64-bit RNG to 0…n  (from lmathlib.c) */
        static ulong Project(ulong ran, ulong n)
        {
            ulong lim = n;
            int sh = 1;

            /* first build next-higher all-1-bits number ≥ n (Mersenne) */
            while ((lim & (lim + 1)) != 0)
            {
                lim |= (lim >> sh);
                sh <<= 1;
            }

            /* then keep drawing until inside the wanted interval */
            while ((ran & lim) > n)
            {
                ran = RotL(ran * 5, 7) * 9;   // one inline xoshiro step
            }
            return ran & lim;
        }

        /// <summary> Lua’s 1-arg form: random( n )  → 1 … n </summary>
        public ulong Next(ulong nInclusive) =>
            Next(1UL, nInclusive);

        /// <summary> Lua’s 2-arg form: random( m, n )  → m … n </summary>
        public ulong Next(ulong minInclusive, ulong maxInclusive)
        {
            if (minInclusive > maxInclusive)
                throw new ArgumentException("empty interval");

            ulong span = maxInclusive - minInclusive;
            ulong r = Project(Next64(), span);
            return r + minInclusive;
        }
    }
}