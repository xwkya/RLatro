/*─────────────────────────────────────────────────────────────────────────
   Benchmark tentative
 ─────────────────────────────────────────────────────────────────────────*/

// using System;
// using System.Collections.Generic;
// using System.Linq;
// using BenchmarkDotNet.Attributes;
// using BenchmarkDotNet.Running;
//
// // ─────────────── data model ───────────────
// public enum Suit : byte { Spades, Hearts, Clubs, Diamonds }
// public enum Rank : byte { Two = 2, Three, Four, Five, Six, Seven,
//                           Eight, Nine, Ten, Jack, Queen, King, Ace }
//
// public readonly record struct Card(Rank Rank, Suit Suit, bool Wild = false);
//
// // bit-flag per card
// [Flags]
// public enum SuitMask : byte
// {
//     None      = 0b0000,
//     Spades    = 0b0001,
//     Hearts    = 0b0010,
//     Clubs     = 0b0100,
//     Diamonds  = 0b1000,
//     All       = 0b1111
// }
//
// public readonly record struct CardView(Rank Rank, SuitMask Mask);
//
// // ─────────────── matcher variant ───────────────
// public interface IMatcher
// {
//     bool Match(Card c, Suit s);
// }
//
// public sealed class ColourMergeMatcher : IMatcher
// {
//     public bool Match(Card c, Suit asked) =>
//         asked switch
//         {
//             Suit.Spades  => c.Suit is Suit.Spades  or Suit.Clubs,
//             Suit.Hearts  => c.Suit is Suit.Hearts  or Suit.Diamonds,
//             _            => c.Suit == asked
//         };
// }
//
// public sealed class EvalContext
// {
//     private readonly List<IMatcher> _m = [];
//
//     public EvalContext(IEnumerable<IMatcher> ms) => _m.AddRange(ms);
//
//     public bool Match(Card c, Suit s) => _m.All(m => m.Match(c, s));
// }
//
// static class FlushAlgos
// {
//     /*──────────────── matcher chain ───────────────*/
//     public static bool MatcherFlush(ReadOnlySpan<Card> hand, EvalContext ctx)
//     {
//         Span<byte> cnt = stackalloc byte[4];
//         foreach (var c in hand)
//             for (int s = 0; s < 4; s++)
//                 if (ctx.Match(c, (Suit)s)) cnt[s]++;
//
//         return cnt[0] >= 5 || cnt[1] >= 5 ||
//                cnt[2] >= 5 || cnt[3] >= 5;
//     }
//
//     /*──────────────── suit-mask counters ───────────*/
//     public static CardView[] Canon(ReadOnlySpan<Card> hand)
//     {
//         var v = new CardView[hand.Length];
//         for (int i = 0; i < hand.Length; i++)
//         {
//             var c = hand[i];
//             SuitMask m = c.Wild ? SuitMask.All
//                                 : (SuitMask)(1 << (int)c.Suit);
//             if (!c.Wild)
//                 m |= c.Suit switch
//                 {
//                     Suit.Clubs    => SuitMask.Spades,
//                     Suit.Diamonds => SuitMask.Hearts,
//                     _             => SuitMask.None
//                 };
//             v[i] = new CardView(c.Rank, m);
//         }
//         return v;
//     }
//
//     public static bool MaskFlush(ReadOnlySpan<CardView> v)
//     {
//         Span<byte> cnt = stackalloc byte[4];
//         foreach (var c in v)
//         {
//             byte m = (byte)c.Mask;
//             cnt[0] += (byte)(m & 1);
//             cnt[1] += (byte)((m >> 1) & 1);
//             cnt[2] += (byte)((m >> 2) & 1);
//             cnt[3] += (byte)((m >> 3) & 1);
//         }
//         return cnt[0] >= 5 || cnt[1] >= 5 ||
//                cnt[2] >= 5 || cnt[3] >= 5;
//     }
//
//     /*──────────────── contiguous-packed 4-bit lanes ───────────*/
//     // pattern “0001 0001 0001 0001 0001” for 5 cards
//     private const int P_SPADES   = 0b0001_0001_0001_0001_0001;
//     private const int P_HEARTS   = P_SPADES << 1;
//     private const int P_CLUBS    = P_SPADES << 2;
//     private const int P_DIAMONDS = P_SPADES << 3;
//
//     public static bool PackedFlush(ReadOnlySpan<CardView> v)
//     {
//         int packed = 0;
//         for (int i = 0; i < v.Length; i++)
//             packed |= ((int)v[i].Mask & 0b1111) << (4 * i);
//
//         // flush exists if every card has the same suit bit → AND pattern hit
//         return (packed & P_SPADES)   == P_SPADES   ||
//                (packed & P_HEARTS)   == P_HEARTS   ||
//                (packed & P_CLUBS)    == P_CLUBS    ||
//                (packed & P_DIAMONDS) == P_DIAMONDS;
//     }
// }
//
// // ─────────────── benchmark harness ───────────────
// [MemoryDiagnoser, WarmupCount(3), IterationCount(10)]
// public class FlushBench
// {
//     private readonly Card[] _hand;
//     private readonly CardView[] _view;
//     private readonly EvalContext _ctx = new([new ColourMergeMatcher()]);
//
//     public FlushBench()
//     {
//         var rng = new Random(42);
//         _hand = Enumerable.Range(0, 5).Select(_ =>
//         {
//             var suit = (Suit)rng.Next(4);
//             var rank = (Rank)rng.Next(2, 15);
//             return new Card(rank, suit);
//         }).ToArray();
//
//         _view = FlushAlgos.Canon(_hand);
//     }
//
//     [Benchmark(Baseline = true)]
//     public bool MatcherFlush() => FlushAlgos.MatcherFlush(_hand, _ctx);
//
//     [Benchmark]
//     public bool MaskFlush()    => FlushAlgos.MaskFlush(_view);
//
//     [Benchmark]
//     public bool PackedFlush()  => FlushAlgos.PackedFlush(_view);
// }

public static class Program
{
    public static void Main(){} //BenchmarkRunner.Run<FlushBench>();
}
