using BenchmarkDotNet.Attributes;
using Balatro.Core.CoreObjects.CoreEnums;
using Balatro.Core.CoreRules.CanonicalViews;
using Balatro.Core.CoreRules.Scoring;

namespace RLatro.Benchmarks.CoreRules
{
    [MemoryDiagnoser]
    public class HandRankGetterBenchmarks
    {
        // --- Hands Data (Define representative hands once) ---
        private static readonly (CardView[] Hand, string Name) s_highCard = (new[]
        {
            CardView.Create(Rank.Two, SuitMask.Heart), CardView.Create(Rank.Queen, SuitMask.Heart)
        }, "HighCard");

        private static readonly (CardView[] Hand, string Name) s_pair = (new[]
        {
            CardView.Create(Rank.Queen, SuitMask.Spade), CardView.Create(Rank.Two, SuitMask.Heart),
            CardView.Create(Rank.Queen, SuitMask.Diamond)
        }, "Pair");

        private static readonly (CardView[] Hand, string Name) s_twoPair = (new[]
        {
            CardView.Create(Rank.Ace, SuitMask.Spade), CardView.Create(Rank.Six, SuitMask.Spade),
            CardView.Create(Rank.Six, SuitMask.Heart), CardView.Create(Rank.Ten, SuitMask.Club),
            CardView.Create(Rank.Ten, SuitMask.Diamond)
        }, "TwoPair");

        private static readonly (CardView[] Hand, string Name) s_threeOfAKind = (new[]
        {
            CardView.Create(Rank.Ace, SuitMask.Spade), CardView.Create(Rank.Ace, SuitMask.Heart),
            CardView.Create(Rank.Ace, SuitMask.Club), CardView.Create(Rank.King, SuitMask.Diamond),
        }, "ThreeKind");

        private static readonly (CardView[] Hand, string Name) s_straight = (new[]
        {
            CardView.Create(Rank.Two, SuitMask.Heart), CardView.Create(Rank.Three, SuitMask.Spade),
            CardView.Create(Rank.Four, SuitMask.Club), CardView.Create(Rank.Five, SuitMask.Diamond),
            CardView.Create(Rank.Six, SuitMask.Heart)
        }, "Straight");

        private static readonly (CardView[] Hand, string Name) s_flush = (new[]
        {
            CardView.Create(Rank.Ace, SuitMask.Heart), CardView.Create(Rank.Jack, SuitMask.Heart),
            CardView.Create(Rank.Five, SuitMask.Heart), CardView.Create(Rank.Seven, SuitMask.Heart),
            CardView.Create(Rank.Queen, SuitMask.Heart)
        }, "Flush");

        private static readonly (CardView[] Hand, string Name) s_fullHouse = (new[]
        {
            CardView.Create(Rank.Ten, SuitMask.Spade), CardView.Create(Rank.Ten, SuitMask.Heart),
            CardView.Create(Rank.Ten, SuitMask.Club), CardView.Create(Rank.King, SuitMask.Diamond),
            CardView.Create(Rank.King, SuitMask.Spade)
        }, "FullHouse");

        private static readonly (CardView[] Hand, string Name) s_fourOfAKind = (new[]
        {
            CardView.Create(Rank.Ten, SuitMask.Spade), CardView.Create(Rank.Ten, SuitMask.Heart),
            CardView.Create(Rank.Ten, SuitMask.Club), CardView.Create(Rank.Ten, SuitMask.Diamond),
            CardView.Create(Rank.King, SuitMask.Spade)
        }, "FourKind");

        private static readonly (CardView[] Hand, string Name) s_fiveOfAKind = (new[]
        {
            CardView.Create(Rank.Two, SuitMask.Spade), CardView.Create(Rank.Two, SuitMask.Heart),
            CardView.Create(Rank.Two, SuitMask.Club), CardView.Create(Rank.Two, SuitMask.Diamond),
            CardView.Create(Rank.Two, SuitMask.All)
        }, "FiveKind"); // Assumes a wild card view

        private static readonly (CardView[] Hand, string Name) s_straightFlush = (new[]
        {
            CardView.Create(Rank.Two, SuitMask.Heart), CardView.Create(Rank.Three, SuitMask.Heart),
            CardView.Create(Rank.Four, SuitMask.Heart), CardView.Create(Rank.Five, SuitMask.Heart),
            CardView.Create(Rank.Six, SuitMask.Heart)
        }, "StraightFlush");

        // --- Data Source for Benchmarks ---
        // Provides the different hands to the benchmark methods
        public IEnumerable<(CardView[] Hand, string Name)> HandsDataSource()
        {
            yield return s_highCard;
            yield return s_pair;
            yield return s_twoPair;
            yield return s_threeOfAKind;
            yield return s_straight;
            yield return s_flush;
            yield return s_fullHouse;
            yield return s_fourOfAKind;
            yield return s_fiveOfAKind;
            yield return s_straightFlush;
        }

        // --- Parameters ---
        private const bool FourFingers = false;
        private const bool Shortcut = true;

        // --- Benchmarks ---
        [Benchmark(Description = "TryFlush")]
        [ArgumentsSource(nameof(HandsDataSource))] // Get data from our source
        public bool BenchmarkTryFlush((CardView[] Hand, string Name) data)
        {
            // stackalloc is fine here as hand length is known within the iteration
            Span<int> flags = stackalloc int[data.Hand.Length];
            // We return the result to prevent dead code elimination
            return HandRankGetter.TryFlush(data.Hand, FourFingers, flags);
        }

        [Benchmark(Description = "TryStraight")]
        [ArgumentsSource(nameof(HandsDataSource))]
        public bool BenchmarkTryStraight((CardView[] Hand, string Name) data)
        {
            Span<int> flags = stackalloc int[data.Hand.Length];
            return HandRankGetter.TryStraight(data.Hand, FourFingers, Shortcut, flags);
        }

        [Benchmark(Description = "GetRank")]
        [ArgumentsSource(nameof(HandsDataSource))]
        public HandRank BenchmarkGetRank((CardView[] Hand, string Name) data)
        {
            Span<int> flags = stackalloc int[data.Hand.Length];
            return HandRankGetter.GetRank(FourFingers, Shortcut, data.Hand, flags);
        }
    }
}