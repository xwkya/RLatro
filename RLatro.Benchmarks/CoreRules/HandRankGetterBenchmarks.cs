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
        // Using readonly static fields avoids recreating arrays repeatedly during benchmarks

        private static readonly CardView[] s_highCardHand = {
            CardView.Create(Rank.Two, SuitMask.Heart),
            CardView.Create(Rank.Four, SuitMask.Spade),
            CardView.Create(Rank.Six, SuitMask.Club),
            CardView.Create(Rank.Eight, SuitMask.Diamond),
            CardView.Create(Rank.Queen, SuitMask.Heart)
        };

        private static readonly CardView[] s_pairHand = {
            CardView.Create(Rank.Queen, SuitMask.Spade),
            CardView.Create(Rank.Two, SuitMask.Heart),
            CardView.Create(Rank.Eight, SuitMask.Club),
            CardView.Create(Rank.Queen, SuitMask.Diamond),
            CardView.Create(Rank.Four, SuitMask.Spade)
        };

        private static readonly CardView[] s_twoPairHand = {
            CardView.Create(Rank.Ace, SuitMask.Spade),
            CardView.Create(Rank.Six, SuitMask.Spade),
            CardView.Create(Rank.Six, SuitMask.Heart),
            CardView.Create(Rank.Ten, SuitMask.Club),
            CardView.Create(Rank.Ten, SuitMask.Diamond)
        };

        private static readonly CardView[] s_threeOfAKindHand = {
            CardView.Create(Rank.Ace, SuitMask.Spade),
            CardView.Create(Rank.Ace, SuitMask.Heart),
            CardView.Create(Rank.Ace, SuitMask.Club),
            CardView.Create(Rank.King, SuitMask.Diamond),
            CardView.Create(Rank.Two, SuitMask.Spade)
        };

        private static readonly CardView[] s_straightHand = {
            CardView.Create(Rank.Two, SuitMask.Heart),
            CardView.Create(Rank.Three, SuitMask.Spade),
            CardView.Create(Rank.Four, SuitMask.Club),
            CardView.Create(Rank.Five, SuitMask.Diamond),
            CardView.Create(Rank.Six, SuitMask.Heart)
        };

        private static readonly CardView[] s_flushHand = {
            CardView.Create(Rank.Ace, SuitMask.Heart),
            CardView.Create(Rank.Jack, SuitMask.Heart),
            CardView.Create(Rank.Five, SuitMask.Heart),
            CardView.Create(Rank.Seven, SuitMask.Heart),
            CardView.Create(Rank.Queen, SuitMask.Heart)
        };

        private static readonly CardView[] s_fullHouseHand = {
            CardView.Create(Rank.Ten, SuitMask.Spade),
            CardView.Create(Rank.Ten, SuitMask.Heart),
            CardView.Create(Rank.Ten, SuitMask.Club),
            CardView.Create(Rank.King, SuitMask.Diamond),
            CardView.Create(Rank.King, SuitMask.Spade)
        };

        private static readonly CardView[] s_fourOfAKindHand = {
            CardView.Create(Rank.Ten, SuitMask.Spade),
            CardView.Create(Rank.Ten, SuitMask.Heart),
            CardView.Create(Rank.Ten, SuitMask.Club),
            CardView.Create(Rank.Ten, SuitMask.Diamond),
            CardView.Create(Rank.King, SuitMask.Spade)
        };
        
        private static readonly CardView[] s_fiveOfAKindHand = {
            CardView.Create(Rank.Two, SuitMask.Spade),
            CardView.Create(Rank.Two, SuitMask.Heart),
            CardView.Create(Rank.Two, SuitMask.Club),
            CardView.Create(Rank.Two, SuitMask.Diamond),
            CardView.Create(Rank.Two, SuitMask.All)
        };

        private static readonly CardView[] s_straightFlushHand = {
            CardView.Create(Rank.Two, SuitMask.Heart),
            CardView.Create(Rank.Three, SuitMask.Heart),
            CardView.Create(Rank.Four, SuitMask.Heart),
            CardView.Create(Rank.Five, SuitMask.Heart),
            CardView.Create(Rank.Six, SuitMask.Heart)
        };
        
        // --- Parameters ---
        // BenchmarkDotNet will run each benchmark method for each combination of these parameters

        [Params(false, true)]
        public bool FourFingers { get; set; }

        [Params(false, true)]
        public bool Shortcut { get; set; }

        // --- Benchmarks ---
        // Each method is a benchmark that will be run by BenchmarkDotNet

        [Benchmark(Description = "High Card")]
        public HandRank BenchmarkHighCard()
        {
            Span<byte> flags = stackalloc byte[s_highCardHand.Length];
            return HandRankGetter.GetRank(FourFingers, Shortcut, s_highCardHand, flags);
        }

        [Benchmark(Description = "Pair")]
        public HandRank BenchmarkPair()
        {
            Span<byte> flags = stackalloc byte[s_pairHand.Length];
            return HandRankGetter.GetRank(FourFingers, Shortcut, s_pairHand, flags);
        }

        [Benchmark(Description = "Two Pair")]
        public HandRank BenchmarkTwoPair()
        {
            Span<byte> flags = stackalloc byte[s_twoPairHand.Length];
            return HandRankGetter.GetRank(FourFingers, Shortcut, s_twoPairHand, flags);
        }

        [Benchmark(Description = "Three of a Kind")]
        public HandRank BenchmarkThreeOfAKind()
        {
            Span<byte> flags = stackalloc byte[s_threeOfAKindHand.Length];
            return HandRankGetter.GetRank(FourFingers, Shortcut, s_threeOfAKindHand, flags);
        }

        [Benchmark(Description = "Straight")]
        public HandRank BenchmarkStraight()
        {
            Span<byte> flags = stackalloc byte[s_straightHand.Length];
            return HandRankGetter.GetRank(FourFingers, Shortcut, s_straightHand, flags);
        }

        [Benchmark(Description = "Flush")]
        public HandRank BenchmarkFlush()
        {
            Span<byte> flags = stackalloc byte[s_flushHand.Length];
            return HandRankGetter.GetRank(FourFingers, Shortcut, s_flushHand, flags);
        }

        [Benchmark(Description = "Full House")]
        public HandRank BenchmarkFullHouse()
        {
            Span<byte> flags = stackalloc byte[s_fullHouseHand.Length];
            return HandRankGetter.GetRank(FourFingers, Shortcut, s_fullHouseHand, flags);
        }

        [Benchmark(Description = "Four of a Kind")]
        public HandRank BenchmarkFourOfAKind()
        {
            Span<byte> flags = stackalloc byte[s_fourOfAKindHand.Length];
            return HandRankGetter.GetRank(FourFingers, Shortcut, s_fourOfAKindHand, flags);
        }
        
        [Benchmark(Description = "Five of a Kind")]
        public HandRank BenchmarkFiveOfAKind()
        {
            Span<byte> flags = stackalloc byte[s_fiveOfAKindHand.Length];
            return HandRankGetter.GetRank(FourFingers, Shortcut, s_fiveOfAKindHand, flags);
        }

        [Benchmark(Description = "Straight Flush")]
        public HandRank BenchmarkStraightFlush()
        {
            Span<byte> flags = stackalloc byte[s_straightFlushHand.Length];
            return HandRankGetter.GetRank(FourFingers, Shortcut, s_straightFlushHand, flags);
        }
    }
}