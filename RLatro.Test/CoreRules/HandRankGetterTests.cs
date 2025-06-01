using Balatro.Core.CoreObjects.CoreEnums;
using Balatro.Core.CoreRules.CanonicalViews;
using Balatro.Core.CoreRules.Scoring;

namespace RLatro.Test.CoreRules
{
    public static class HandRankGetterHelper
    {
        public static HandRank Eval(bool fourFingers,
            bool shortcut,
            out int numberOfCardsFlagged,
            params CardView[] cards)
        {
            Span<int> flags = stackalloc int[cards.Length];
            var hand = HandRankGetter.GetRank(fourFingers, shortcut, cards, flags);
            numberOfCardsFlagged = 0;
            for (int i = 0; i < cards.Length; i++)
            {
                if (flags[i] != 0)
                    numberOfCardsFlagged++;
            }

            return hand;
        }
    }
    
    [TestFixture]
    public sealed class HandRankGetterTests
    {
        [TestFixture, Category("Straights")]
        public sealed class StraightRankGetterTests
        {
            #region Regular straights positive

            [Test]
            public void StraightPositive()
            {
                var rank = HandRankGetterHelper.Eval(false, false,
                    out var numberOfCardsFlagged,
                    CardView.Create(Rank.Two, SuitMask.Heart),
                    CardView.Create(Rank.Three, SuitMask.Spade),
                    CardView.Create(Rank.Four, SuitMask.Club),
                    CardView.Create(Rank.Five, SuitMask.Diamond),
                    CardView.Create(Rank.Six, SuitMask.Heart));

                Assert.That(rank, Is.EqualTo(HandRank.Straight));
                Assert.That(numberOfCardsFlagged, Is.EqualTo(5));
            }

            [Test]
            public void StraightPositiveShuffled()
            {
                var rank = HandRankGetterHelper.Eval(false, false,
                    out var numberOfCardsFlagged,
                    CardView.Create(Rank.Ace, SuitMask.Heart),
                    CardView.Create(Rank.Jack, SuitMask.Spade),
                    CardView.Create(Rank.Ten, SuitMask.Club),
                    CardView.Create(Rank.King, SuitMask.Diamond),
                    CardView.Create(Rank.Queen, SuitMask.Heart));

                Assert.That(rank, Is.EqualTo(HandRank.Straight));
                Assert.That(numberOfCardsFlagged, Is.EqualTo(5));
            }

            [Test]
            public void StraightPositiveWithLowAce()
            {
                var rank = HandRankGetterHelper.Eval(false, false,
                    out var numberOfCardsFlagged,
                    CardView.Create(Rank.Ace, SuitMask.Spade),
                    CardView.Create(Rank.Two, SuitMask.Heart),
                    CardView.Create(Rank.Three, SuitMask.Diamond),
                    CardView.Create(Rank.Four, SuitMask.Spade),
                    CardView.Create(Rank.Five, SuitMask.Spade));

                Assert.That(rank, Is.EqualTo(HandRank.Straight));
                Assert.That(numberOfCardsFlagged, Is.EqualTo(5));
            }

            #endregion

            #region Regular straights negative

            [Test]
            public void StraightNegative()
            {
                var rank = HandRankGetterHelper.Eval(false, false,
                    out var _,
                    CardView.Create(Rank.Two, SuitMask.Heart),
                    CardView.Create(Rank.Three, SuitMask.Spade),
                    CardView.Create(Rank.Four, SuitMask.Club),
                    CardView.Create(Rank.Six, SuitMask.Diamond),
                    CardView.Create(Rank.Seven, SuitMask.Heart));

                Assert.That(rank, Is.Not.EqualTo(HandRank.Straight));
            }

            [Test]
            public void StraightNegativeShuffled()
            {
                var rank = HandRankGetterHelper.Eval(false, false,
                    out var _,
                    CardView.Create(Rank.Ace, SuitMask.Heart),
                    CardView.Create(Rank.Jack, SuitMask.Spade),
                    CardView.Create(Rank.Ten, SuitMask.Club),
                    CardView.Create(Rank.King, SuitMask.Diamond),
                    CardView.Create(Rank.Eight, SuitMask.Heart));

                Assert.That(rank, Is.Not.EqualTo(HandRank.Straight));
            }

            [Test]
            public void WrappedStraightNegative()
            {
                var rank = HandRankGetterHelper.Eval(false, false,
                    out var _,
                    CardView.Create(Rank.King, SuitMask.Spade),
                    CardView.Create(Rank.Ace, SuitMask.Heart),
                    CardView.Create(Rank.Two, SuitMask.Diamond),
                    CardView.Create(Rank.Three, SuitMask.Spade),
                    CardView.Create(Rank.Four, SuitMask.Spade));

                Assert.That(rank, Is.Not.EqualTo(HandRank.Straight));
            }

            [Test]
            public void StraightWithFourCardsNegative()
            {
                var rank = HandRankGetterHelper.Eval(false, false,
                    out var _,
                    CardView.Create(Rank.Two, SuitMask.Heart),
                    CardView.Create(Rank.Three, SuitMask.Spade),
                    CardView.Create(Rank.Four, SuitMask.Club),
                    CardView.Create(Rank.Five, SuitMask.Diamond));

                Assert.That(rank, Is.Not.EqualTo(HandRank.Straight));
            }

            [Test]
            public void StraightWithRepeatCardsNegative()
            {
                var rank = HandRankGetterHelper.Eval(false, false,
                    out var _,
                    CardView.Create(Rank.Two, SuitMask.Heart),
                    CardView.Create(Rank.Three, SuitMask.Spade),
                    CardView.Create(Rank.Three, SuitMask.Club),
                    CardView.Create(Rank.Four, SuitMask.Diamond),
                    CardView.Create(Rank.Five, SuitMask.Heart));

                Assert.That(rank, Is.Not.EqualTo(HandRank.Straight));
            }

            #endregion
        }

        [TestFixture, Category("Flushes")]
        public sealed class FlushRankGetterTests
        {
            #region Regular flush positive

            [Test]
            public void FlushPositive()
            {
                var rank = HandRankGetterHelper.Eval(false, false,
                    out var numberOfCardsFlagged,
                    CardView.Create(Rank.Ace, SuitMask.Heart),
                    CardView.Create(Rank.Jack, SuitMask.Heart),
                    CardView.Create(Rank.Five, SuitMask.Heart),
                    CardView.Create(Rank.Seven, SuitMask.Heart),
                    CardView.Create(Rank.Queen, SuitMask.Heart));

                Assert.That(rank, Is.EqualTo(HandRank.Flush));
                Assert.That(numberOfCardsFlagged, Is.EqualTo(5));
            }

            [Test]
            public void FlushPositiveWithWild()
            {
                var rank = HandRankGetterHelper.Eval(false, false,
                    out var numberOfCardsFlagged,
                    CardView.Create(Rank.Two, SuitMask.All),
                    CardView.Create(Rank.Four, SuitMask.Heart),
                    CardView.Create(Rank.Six, SuitMask.Heart),
                    CardView.Create(Rank.Eight, SuitMask.Heart | SuitMask.Spade),
                    CardView.Create(Rank.Ten, SuitMask.Heart));

                Assert.That(rank, Is.EqualTo(HandRank.Flush));
                Assert.That(numberOfCardsFlagged, Is.EqualTo(5));
            }

            #endregion

            #region Regular flush negative

            [Test]
            public void FlushNegative()
            {
                var rank = HandRankGetterHelper.Eval(false, false,
                    out var _,
                    CardView.Create(Rank.Ace, SuitMask.Heart),
                    CardView.Create(Rank.Jack, SuitMask.Spade),
                    CardView.Create(Rank.Ten, SuitMask.Heart),
                    CardView.Create(Rank.King, SuitMask.Heart),
                    CardView.Create(Rank.Queen, SuitMask.Heart));

                Assert.That(rank, Is.Not.EqualTo(HandRank.Flush));
            }

            [Test]
            public void FlushNegativeWithWild()
            {
                var rank = HandRankGetterHelper.Eval(false, false,
                    out var _,
                    CardView.Create(Rank.Two, SuitMask.All),
                    CardView.Create(Rank.Four, SuitMask.Heart),
                    CardView.Create(Rank.Six, SuitMask.Heart),
                    CardView.Create(Rank.Eight, SuitMask.Spade),
                    CardView.Create(Rank.Ten, SuitMask.Heart));

                Assert.That(rank, Is.Not.EqualTo(HandRank.Flush));
            }

            [Test]
            public void FlushNegativeWithTwoCards()
            {
                var rank = HandRankGetterHelper.Eval(false, false,
                    out var _,
                    CardView.Create(Rank.Ace, SuitMask.Heart),
                    CardView.Create(Rank.Jack, SuitMask.Spade));

                Assert.That(rank, Is.Not.EqualTo(HandRank.Flush));
            }

            #endregion

            #region Flush with four fingers positive

            [Test]
            public void FlushPositiveFourFingersFiveCards()
            {
                var rank = HandRankGetterHelper.Eval(true, false,
                    out var numberOfCardsFlagged,
                    CardView.Create(Rank.Ace, SuitMask.Heart),
                    CardView.Create(Rank.Jack, SuitMask.Heart),
                    CardView.Create(Rank.Ten, SuitMask.Heart),
                    CardView.Create(Rank.Three, SuitMask.Heart),
                    CardView.Create(Rank.Five, SuitMask.Heart));

                Assert.That(rank, Is.EqualTo(HandRank.Flush));
                Assert.That(numberOfCardsFlagged, Is.EqualTo(5));
            }

            [Test]
            public void FlushPositiveFourFingersFourCards()
            {
                var rank = HandRankGetterHelper.Eval(true, false,
                    out var numberOfCardsFlagged,
                    CardView.Create(Rank.Ace, SuitMask.Heart),
                    CardView.Create(Rank.Jack, SuitMask.Heart),
                    CardView.Create(Rank.Ten, SuitMask.Heart),
                    CardView.Create(Rank.Three, SuitMask.Heart),
                    CardView.Create(Rank.Five, SuitMask.Club));

                Assert.That(rank, Is.EqualTo(HandRank.Flush));
                Assert.That(numberOfCardsFlagged, Is.EqualTo(4));
            }

            #endregion

            #region Flush with four fingers negative

            [Test]
            public void FlushNegativeFourFingers()
            {
                var rank = HandRankGetterHelper.Eval(true, false,
                    out var _,
                    CardView.Create(Rank.Ace, SuitMask.Heart),
                    CardView.Create(Rank.Jack, SuitMask.Spade),
                    CardView.Create(Rank.Ten, SuitMask.Spade),
                    CardView.Create(Rank.King, SuitMask.Heart),
                    CardView.Create(Rank.Queen, SuitMask.Heart));

                Assert.That(rank, Is.Not.EqualTo(HandRank.Flush));
            }

            [Test]
            public void FlushLosesToStraightFlush()
            {
                var rank = HandRankGetterHelper.Eval(true, false,
                    out var _,
                    CardView.Create(Rank.Ace, SuitMask.Heart),
                    CardView.Create(Rank.Jack, SuitMask.Heart),
                    CardView.Create(Rank.Ten, SuitMask.Heart),
                    CardView.Create(Rank.King, SuitMask.Heart),
                    CardView.Create(Rank.Queen, SuitMask.Heart));

                Assert.That(rank, Is.EqualTo(HandRank.StraightFlush));
            }

            [Test]
            public void FlushLosesToFullHouse()
            {
                var rank = HandRankGetterHelper.Eval(true, false,
                    out var _,
                    CardView.Create(Rank.Ace, SuitMask.Heart),
                    CardView.Create(Rank.Ace, SuitMask.Heart),
                    CardView.Create(Rank.Ace, SuitMask.Heart),
                    CardView.Create(Rank.King, SuitMask.Spade),
                    CardView.Create(Rank.King, SuitMask.Spade));

                Assert.That(rank, Is.Not.EqualTo(HandRank.Flush));
            }

            [Test]
            public void FlushLosesToFlushHouse()
            {
                var rank = HandRankGetterHelper.Eval(true, false,
                    out var _,
                    CardView.Create(Rank.Ace, SuitMask.Heart),
                    CardView.Create(Rank.Ace, SuitMask.Heart),
                    CardView.Create(Rank.Ace, SuitMask.Heart),
                    CardView.Create(Rank.King, SuitMask.Heart),
                    CardView.Create(Rank.King, SuitMask.Spade));
            }

            #endregion
        }

        [TestFixture, Category("Pairs")]
        public sealed class PairsRankGetterTests
        {
            #region Pair  ──────────────── POSITIVE

            [Test]
            public void Pair_Positive_TwoCards()
            {
                var r = HandRankGetterHelper.Eval(false, false, out var n,
                    CardView.Create(Rank.Five, SuitMask.Spade),
                    CardView.Create(Rank.Five, SuitMask.Heart));
                Assert.That(r, Is.EqualTo(HandRank.Pair));
                Assert.That(n, Is.EqualTo(2));
            }

            [Test]
            public void Pair_Positive_ThreeCards()
            {
                var r = HandRankGetterHelper.Eval(false, false, out var n,
                    CardView.Create(Rank.Ten, SuitMask.Club),
                    CardView.Create(Rank.Ten, SuitMask.Diamond),
                    CardView.Create(Rank.Ace, SuitMask.Spade));
                Assert.That(r, Is.EqualTo(HandRank.Pair));
                Assert.That(n, Is.EqualTo(2));
            }

            [Test]
            public void Pair_Positive_FourCards()
            {
                var r = HandRankGetterHelper.Eval(false, false, out var n,
                    CardView.Create(Rank.Seven, SuitMask.Spade),
                    CardView.Create(Rank.Jack, SuitMask.Heart),
                    CardView.Create(Rank.Seven, SuitMask.Club),
                    CardView.Create(Rank.Three, SuitMask.Diamond));
                Assert.That(r, Is.EqualTo(HandRank.Pair));
                Assert.That(n, Is.EqualTo(2));
            }

            [Test]
            public void Pair_Positive_FiveCards_Shuffled()
            {
                var r = HandRankGetterHelper.Eval(false, false, out var n,
                    CardView.Create(Rank.Queen, SuitMask.Spade),
                    CardView.Create(Rank.Two, SuitMask.Heart),
                    CardView.Create(Rank.Eight, SuitMask.Club),
                    CardView.Create(Rank.Queen, SuitMask.Diamond),
                    CardView.Create(Rank.Four, SuitMask.Spade));
                Assert.That(r, Is.EqualTo(HandRank.Pair));
                Assert.That(n, Is.EqualTo(2));
            }

            #endregion

            #region Pair  ──────────────── NEGATIVE

            [Test]
            public void Pair_Negative_NoDuplicates()
            {
                var r = HandRankGetterHelper.Eval(false, false, out _,
                    CardView.Create(Rank.Two, SuitMask.Spade),
                    CardView.Create(Rank.Three, SuitMask.Heart),
                    CardView.Create(Rank.Four, SuitMask.Club));
                Assert.That(r, Is.Not.EqualTo(HandRank.Pair));
            }

            [Test]
            public void Pair_Negative_TripsBeatsPair()
            {
                var r = HandRankGetterHelper.Eval(false, false, out _,
                    CardView.Create(Rank.Nine, SuitMask.Spade),
                    CardView.Create(Rank.Nine, SuitMask.Diamond),
                    CardView.Create(Rank.Nine, SuitMask.Heart));
                Assert.That(r, Is.EqualTo(HandRank.ThreeOfAKind));
            }

            [Test]
            public void Pair_Negative_FlushBeatsPair()
            {
                var r = HandRankGetterHelper.Eval(false, false, out _,
                    CardView.Create(Rank.Two, SuitMask.Club),
                    CardView.Create(Rank.Two, SuitMask.Club), // duplicate rank
                    CardView.Create(Rank.Five, SuitMask.Club),
                    CardView.Create(Rank.Seven, SuitMask.Club),
                    CardView.Create(Rank.King, SuitMask.Club)); // 5 clubs → flush outranks
                Assert.That(r, Is.EqualTo(HandRank.Flush));
            }

            [Test]
            public void Pair_Negative_StraightBeatsPair()
            {
                var r = HandRankGetterHelper.Eval(true, false, out var n,
                    CardView.Create(Rank.Five, SuitMask.Spade),
                    CardView.Create(Rank.Five, SuitMask.Heart), // duplicate rank
                    CardView.Create(Rank.Six, SuitMask.Diamond),
                    CardView.Create(Rank.Seven, SuitMask.Club),
                    CardView.Create(Rank.Eight, SuitMask.Spade)); // straight present

                Assert.That(r, Is.EqualTo(HandRank.Straight));
                Assert.That(n, Is.EqualTo(5));
            }

            #endregion
        }

        [TestFixture, Category("TwoPairs")]
        public sealed class TwoPairsRankGetterTests
        {
            #region Two-Pair  ──────────── POSITIVE

            [Test]
            public void TwoPair_Positive_FourCards()
            {
                var r = HandRankGetterHelper.Eval(false, false, out var n,
                    CardView.Create(Rank.Three, SuitMask.Spade),
                    CardView.Create(Rank.Three, SuitMask.Club),
                    CardView.Create(Rank.Jack, SuitMask.Heart),
                    CardView.Create(Rank.Jack, SuitMask.Diamond));
                Assert.That(r, Is.EqualTo(HandRank.TwoPair));
                Assert.That(n, Is.EqualTo(4));
            }

            [Test]
            public void TwoPair_Positive_FiveCards_WithKicker()
            {
                var r = HandRankGetterHelper.Eval(false, false, out var n,
                    CardView.Create(Rank.Ace, SuitMask.Spade), // kicker
                    CardView.Create(Rank.Six, SuitMask.Spade),
                    CardView.Create(Rank.Six, SuitMask.Heart),
                    CardView.Create(Rank.Ten, SuitMask.Club),
                    CardView.Create(Rank.Ten, SuitMask.Diamond));
                Assert.That(r, Is.EqualTo(HandRank.TwoPair));
                Assert.That(n, Is.EqualTo(4));
            }

            #endregion

            #region Two-Pair  ──────────── NEGATIVE

            [Test]
            public void TwoPair_Negative_SinglePairOnly()
            {
                var r = HandRankGetterHelper.Eval(false, false, out _,
                    CardView.Create(Rank.Four, SuitMask.Spade),
                    CardView.Create(Rank.Four, SuitMask.Heart),
                    CardView.Create(Rank.Nine, SuitMask.Diamond),
                    CardView.Create(Rank.King, SuitMask.Club));
                Assert.That(r, Is.EqualTo(HandRank.Pair));
            }

            [Test]
            public void TwoPair_Negative_FullHouseOutranks()
            {
                var r = HandRankGetterHelper.Eval(false, false, out _,
                    CardView.Create(Rank.Queen, SuitMask.Spade),
                    CardView.Create(Rank.Queen, SuitMask.Heart),
                    CardView.Create(Rank.Queen, SuitMask.Club), // trips
                    CardView.Create(Rank.Two, SuitMask.Diamond),
                    CardView.Create(Rank.Two, SuitMask.Spade)); // + pair ⇒ full house
                Assert.That(r, Is.EqualTo(HandRank.FullHouse));
            }

            [Test]
            public void TwoPair_Negative_FourOfAKindOutranks()
            {
                var r = HandRankGetterHelper.Eval(false, false, out _,
                    CardView.Create(Rank.Eight, SuitMask.Spade),
                    CardView.Create(Rank.Eight, SuitMask.Heart),
                    CardView.Create(Rank.Eight, SuitMask.Club),
                    CardView.Create(Rank.Eight, SuitMask.Diamond),
                    CardView.Create(Rank.Three, SuitMask.Spade));
                Assert.That(r, Is.EqualTo(HandRank.FourOfAKind));
            }

            [Test]
            public void TwoPair_Negative_FlushOutranks()
            {
                var r = HandRankGetterHelper.Eval(false, false, out _,
                    CardView.Create(Rank.Two, SuitMask.Heart),
                    CardView.Create(Rank.Two, SuitMask.Heart),
                    CardView.Create(Rank.Seven, SuitMask.Heart),
                    CardView.Create(Rank.Jack, SuitMask.Heart),
                    CardView.Create(Rank.King, SuitMask.Heart)); // flush beats TP
                Assert.That(r, Is.EqualTo(HandRank.Flush));
            }

            #endregion
        }

        [TestFixture, Category("ThreeOfAKind")]
        public sealed class ThreeOfAKindRankGetterTests
        {
            #region Three-of-a-Kind  ──── POSITIVE

            [Test]
            public void ThreeOfAKind_Positive_FourCards()
            {
                var r = HandRankGetterHelper.Eval(false, false, out var n,
                    CardView.Create(Rank.Ten, SuitMask.Spade),
                    CardView.Create(Rank.Ten, SuitMask.Heart),
                    CardView.Create(Rank.Ten, SuitMask.Club),
                    CardView.Create(Rank.King, SuitMask.Diamond));
                Assert.That(r, Is.EqualTo(HandRank.ThreeOfAKind));
                Assert.That(n, Is.EqualTo(3));
            }

            [Test]
            public void ThreeOfAKind_Positive_FiveCards()
            {
                var r = HandRankGetterHelper.Eval(false, false, out var n,
                    CardView.Create(Rank.Ace, SuitMask.Spade),
                    CardView.Create(Rank.Ace, SuitMask.Heart),
                    CardView.Create(Rank.Ace, SuitMask.Club),
                    CardView.Create(Rank.King, SuitMask.Diamond),
                    CardView.Create(Rank.Two, SuitMask.Spade));
                Assert.That(r, Is.EqualTo(HandRank.ThreeOfAKind));
                Assert.That(n, Is.EqualTo(3));
            }

            [Test]
            public void ThreeOfAKind_Positive_ThreeCards()
            {
                var r = HandRankGetterHelper.Eval(false, false, out var n,
                    CardView.Create(Rank.Queen, SuitMask.Spade),
                    CardView.Create(Rank.Queen, SuitMask.Heart),
                    CardView.Create(Rank.Queen, SuitMask.Club));
                Assert.That(r, Is.EqualTo(HandRank.ThreeOfAKind));
                Assert.That(n, Is.EqualTo(3));
            }

            #endregion

            #region Three-of-a-Kind  ──── NEGATIVE

            [Test]
            public void ThreeOfAKind_Negative_NoDuplicates()
            {
                var r = HandRankGetterHelper.Eval(false, false, out _,
                    CardView.Create(Rank.Two, SuitMask.Spade),
                    CardView.Create(Rank.Three, SuitMask.Heart),
                    CardView.Create(Rank.Four, SuitMask.Club));
                Assert.That(r, Is.Not.EqualTo(HandRank.ThreeOfAKind));
            }

            [Test]
            public void ThreeOfAKind_Negative_FlushBeatsThreeOfAKind()
            {
                var r = HandRankGetterHelper.Eval(false, false, out _,
                    CardView.Create(Rank.Two, SuitMask.Heart),
                    CardView.Create(Rank.Two, SuitMask.Heart),
                    CardView.Create(Rank.Two, SuitMask.Heart),
                    CardView.Create(Rank.Five, SuitMask.Heart),
                    CardView.Create(Rank.Seven, SuitMask.Heart)); // flush beats 3oK
                Assert.That(r, Is.EqualTo(HandRank.Flush));
            }

            [Test]
            public void ThreeOfAKind_Negative_StraightBeatsThreeOfAKind()
            {
                var r = HandRankGetterHelper.Eval(false, false, out var n,
                    CardView.Create(Rank.Two, SuitMask.Spade),
                    CardView.Create(Rank.Three, SuitMask.Heart),
                    CardView.Create(Rank.Four, SuitMask.Club),
                    CardView.Create(Rank.Five, SuitMask.Diamond),
                    CardView.Create(Rank.Five, SuitMask.Spade),
                    CardView.Create(Rank.Five, SuitMask.Heart),
                    CardView.Create(Rank.Six, SuitMask.Spade)); // straight beats 3oK
                Assert.That(r, Is.EqualTo(HandRank.Straight));
                Assert.That(n, Is.EqualTo(7));
            }

            [Test]
            public void ThreeOfAKind_Negative_FourOfAKindBeatsThreeOfAKind()
            {
                var r = HandRankGetterHelper.Eval(false, false, out _,
                    CardView.Create(Rank.Two, SuitMask.Spade),
                    CardView.Create(Rank.Two, SuitMask.Heart),
                    CardView.Create(Rank.Two, SuitMask.Club),
                    CardView.Create(Rank.Two, SuitMask.Diamond),
                    CardView.Create(Rank.Five, SuitMask.Spade)); // 4oK beats 3oK
                Assert.That(r, Is.EqualTo(HandRank.FourOfAKind));
            }

            [Test]
            public void ThreeOfAKind_Negative_FullHouseBeatsThreeOfAKind()
            {
                var r = HandRankGetterHelper.Eval(false, false, out _,
                    CardView.Create(Rank.Two, SuitMask.Spade),
                    CardView.Create(Rank.Two, SuitMask.Heart),
                    CardView.Create(Rank.Two, SuitMask.Club),
                    CardView.Create(Rank.Five, SuitMask.Diamond),
                    CardView.Create(Rank.Five, SuitMask.Spade)); // full house beats 3oK
                Assert.That(r, Is.EqualTo(HandRank.FullHouse));
            }

            #endregion
        }

        [TestFixture, Category("FullHouses")]
        public sealed class FullHouseRankGetterTests
        {
            #region Full-house ──────────── POSITIVE

            [Test]
            public void FullHouse_Positive()
            {
                var r = HandRankGetterHelper.Eval(false, false, out var n,
                    CardView.Create(Rank.Ten, SuitMask.Spade),
                    CardView.Create(Rank.Ten, SuitMask.Heart),
                    CardView.Create(Rank.Ten, SuitMask.Club),
                    CardView.Create(Rank.King, SuitMask.Diamond),
                    CardView.Create(Rank.King, SuitMask.Spade));
                ;
                Assert.That(r, Is.EqualTo(HandRank.FullHouse));
                Assert.That(n, Is.EqualTo(5));
            }

            [Test]
            public void FullHouse_HighCards()
            {
                var r = HandRankGetterHelper.Eval(false, false, out var n,
                    CardView.Create(Rank.Ace, SuitMask.Spade),
                    CardView.Create(Rank.Ace, SuitMask.Heart),
                    CardView.Create(Rank.Ace, SuitMask.Club),
                    CardView.Create(Rank.King, SuitMask.Diamond),
                    CardView.Create(Rank.King, SuitMask.Spade));
                ;

                Assert.That(r, Is.EqualTo(HandRank.FullHouse));
                Assert.That(n, Is.EqualTo(5));
            }

            #endregion

            #region Full house ────────────── NEGATIVE

            [Test]
            public void FullHouse_Negative_TripsOnly()
            {
                var r = HandRankGetterHelper.Eval(false, false, out _,
                    CardView.Create(Rank.Ten, SuitMask.Spade),
                    CardView.Create(Rank.Ten, SuitMask.Heart),
                    CardView.Create(Rank.Ten, SuitMask.Club),
                    CardView.Create(Rank.King, SuitMask.Diamond),
                    CardView.Create(Rank.Queen, SuitMask.Spade)); // trips only
                Assert.That(r, Is.EqualTo(HandRank.ThreeOfAKind));
            }

            [Test]
            public void FullHouse_Negative_LosesToFiveOfAKind()
            {
                var r = HandRankGetterHelper.Eval(false, false, out _,
                    CardView.Create(Rank.Ten, SuitMask.Spade),
                    CardView.Create(Rank.Ten, SuitMask.Heart),
                    CardView.Create(Rank.Ten, SuitMask.Club),
                    CardView.Create(Rank.Ten, SuitMask.Diamond),
                    CardView.Create(Rank.Ten, SuitMask.Spade)); // 5oK beats full house
                Assert.That(r, Is.EqualTo(HandRank.FiveOfAKind));
            }

            [Test]
            public void FullHouse_Negative_LosesToFlushHouse()
            {
                var r = HandRankGetterHelper.Eval(false, false, out _,
                    CardView.Create(Rank.Two, SuitMask.Heart),
                    CardView.Create(Rank.Two, SuitMask.Heart),
                    CardView.Create(Rank.Two, SuitMask.Heart),
                    CardView.Create(Rank.Five, SuitMask.Heart),
                    CardView.Create(Rank.Five, SuitMask.Heart)); // flush house beats full house

                Assert.That(r, Is.EqualTo(HandRank.FlushHouse));
            }

            [Test]
            public void FullHouse_Negative_FourFingerAndShortCut()
            {
                var r = HandRankGetterHelper.Eval(true, true, out _,
                    CardView.Create(Rank.Two, SuitMask.Heart),
                    CardView.Create(Rank.Two, SuitMask.Heart),
                    CardView.Create(Rank.Five, SuitMask.Heart),
                    CardView.Create(Rank.Five, SuitMask.Heart)); // No full house

                Assert.That(r, Is.Not.EqualTo(HandRank.FullHouse));
            }

            #endregion
        }

        [TestFixture, Category("FourOfAKind")]
        public sealed class FourOfAKindRankGetterTests
        {
            #region Four-of-a-Kind ────── POSITIVE

            [Test]
            public void FourOfAKind_Positive_FiveCards()
            {
                var r = HandRankGetterHelper.Eval(false, false, out var n,
                    CardView.Create(Rank.Ten, SuitMask.Spade),
                    CardView.Create(Rank.Ten, SuitMask.Heart),
                    CardView.Create(Rank.Ten, SuitMask.Club),
                    CardView.Create(Rank.Ten, SuitMask.Diamond),
                    CardView.Create(Rank.King, SuitMask.Spade));
                Assert.That(r, Is.EqualTo(HandRank.FourOfAKind));
                Assert.That(n, Is.EqualTo(4));
            }

            [Test]
            public void FourOfAKindWinsAgainstFlush()
            {
                var r = HandRankGetterHelper.Eval(true, false, out var n,
                    CardView.Create(Rank.Ten, SuitMask.All),
                    CardView.Create(Rank.Ten, SuitMask.All),
                    CardView.Create(Rank.Ten, SuitMask.All),
                    CardView.Create(Rank.Ten, SuitMask.All),
                    CardView.Create(Rank.Five, SuitMask.All));

                Assert.That(r, Is.EqualTo(HandRank.FourOfAKind));
                Assert.That(n, Is.EqualTo(4));
            }

            #endregion

            #region Four-of-a-Kind ────── NEGATIVE

            [Test]
            public void FourOfAKind_Negative_FourCards()
            {
                var r = HandRankGetterHelper.Eval(false, false, out var _,
                    CardView.Create(Rank.Five, SuitMask.Spade),
                    CardView.Create(Rank.Ten, SuitMask.Heart),
                    CardView.Create(Rank.Ten, SuitMask.Club),
                    CardView.Create(Rank.Ten, SuitMask.Diamond));
                Assert.That(r, Is.Not.EqualTo(HandRank.FourOfAKind));
            }

            [Test]
            public void FourOfAKind_Negative_FiveOfAKindWins()
            {
                var r = HandRankGetterHelper.Eval(false, false, out var _,
                    CardView.Create(Rank.Ten, SuitMask.Spade),
                    CardView.Create(Rank.Ten, SuitMask.Heart),
                    CardView.Create(Rank.Ten, SuitMask.Club),
                    CardView.Create(Rank.Ten, SuitMask.Diamond),
                    CardView.Create(Rank.Ten, SuitMask.All));

                Assert.That(r, Is.EqualTo(HandRank.FiveOfAKind));
            }

            #endregion
        }

        [TestFixture, Category("FiveOfAKind")]
        public sealed class FiveOfAKindRankGetterTests
        {
            #region Five-of-a-Kind ────── POSITIVE

            [Test]
            public void FiveOfAKind_Positive_FiveCards()
            {
                var r = HandRankGetterHelper.Eval(false, false, out var n,
                    CardView.Create(Rank.Two, SuitMask.Spade),
                    CardView.Create(Rank.Two, SuitMask.Heart),
                    CardView.Create(Rank.Two, SuitMask.Club),
                    CardView.Create(Rank.Two, SuitMask.Diamond),
                    CardView.Create(Rank.Two, SuitMask.All));

                Assert.That(r, Is.EqualTo(HandRank.FiveOfAKind));
                Assert.That(n, Is.EqualTo(5));
            }

            [Test]
            public void FiveOfAKind_Positive_Aces()
            {
                var r = HandRankGetterHelper.Eval(false, false, out var n,
                    CardView.Create(Rank.Ace, SuitMask.Spade),
                    CardView.Create(Rank.Ace, SuitMask.Heart),
                    CardView.Create(Rank.Ace, SuitMask.Club),
                    CardView.Create(Rank.Ace, SuitMask.Diamond),
                    CardView.Create(Rank.Ace, SuitMask.All));

                Assert.That(r, Is.EqualTo(HandRank.FiveOfAKind));
                Assert.That(n, Is.EqualTo(5));
            }

            #endregion

            #region Five-of-a-Kind ────── NEGATIVE

            [Test]
            public void FiveOfAKind_Negative_FourCards()
            {
                var r = HandRankGetterHelper.Eval(false, false, out var _,
                    CardView.Create(Rank.Ten, SuitMask.Spade),
                    CardView.Create(Rank.Ten, SuitMask.Heart),
                    CardView.Create(Rank.Ten, SuitMask.Club),
                    CardView.Create(Rank.Ten, SuitMask.Diamond));

                Assert.That(r, Is.Not.EqualTo(HandRank.FiveOfAKind));
            }

            [Test]
            public void FiveOfAKind_Negative_LosesToFlushFive()
            {
                var r = HandRankGetterHelper.Eval(false, false, out var _,
                    CardView.Create(Rank.Ace, SuitMask.All),
                    CardView.Create(Rank.Ace, SuitMask.All),
                    CardView.Create(Rank.Ace, SuitMask.All),
                    CardView.Create(Rank.Ace, SuitMask.All),
                    CardView.Create(Rank.Ace, SuitMask.All));
                Assert.That(r, Is.EqualTo(HandRank.FlushFive));
            }

            #endregion
        }

        [TestFixture, Category("StraightFlushes")]
        public sealed class StraightFlushRankGetterTests
        {
            #region Regular straight-flush positive

            [Test]
            public void StraightFlushPositive()
            {
                var rank = HandRankGetterHelper.Eval(false, false,
                    out var numberOfCardsFlagged,
                    CardView.Create(Rank.Two, SuitMask.Heart),
                    CardView.Create(Rank.Three, SuitMask.Heart),
                    CardView.Create(Rank.Four, SuitMask.Heart),
                    CardView.Create(Rank.Five, SuitMask.Heart),
                    CardView.Create(Rank.Six, SuitMask.Heart));

                Assert.That(rank, Is.EqualTo(HandRank.StraightFlush));
                Assert.That(numberOfCardsFlagged, Is.EqualTo(5));
            }

            [Test]
            public void StraightFlushPositiveShuffled()
            {
                var rank = HandRankGetterHelper.Eval(false, false,
                    out var numberOfCardsFlagged,
                    CardView.Create(Rank.Ace, SuitMask.Heart),
                    CardView.Create(Rank.Jack, SuitMask.Heart),
                    CardView.Create(Rank.Ten, SuitMask.Heart),
                    CardView.Create(Rank.King, SuitMask.Heart),
                    CardView.Create(Rank.Queen, SuitMask.Heart));

                Assert.That(rank, Is.EqualTo(HandRank.StraightFlush));
                Assert.That(numberOfCardsFlagged, Is.EqualTo(5));
            }

            [Test]
            public void StraightFlushPositiveWithLowAce()
            {
                var rank = HandRankGetterHelper.Eval(false, false,
                    out var numberOfCardsFlagged,
                    CardView.Create(Rank.Ace, SuitMask.Spade),
                    CardView.Create(Rank.Two, SuitMask.Spade),
                    CardView.Create(Rank.Three, SuitMask.Spade),
                    CardView.Create(Rank.Four, SuitMask.Spade),
                    CardView.Create(Rank.Five, SuitMask.Spade));

                Assert.That(rank, Is.EqualTo(HandRank.StraightFlush));
                Assert.That(numberOfCardsFlagged, Is.EqualTo(5));
            }

            #endregion

            #region Regular straight-flush negative

            [Test]
            public void StraightFlushNegative()
            {
                var rank = HandRankGetterHelper.Eval(false, false,
                    out var _,
                    CardView.Create(Rank.Two, SuitMask.Heart),
                    CardView.Create(Rank.Three, SuitMask.Spade),
                    CardView.Create(Rank.Four, SuitMask.Club),
                    CardView.Create(Rank.Six, SuitMask.Diamond),
                    CardView.Create(Rank.Seven, SuitMask.Heart));

                Assert.That(rank, Is.Not.EqualTo(HandRank.StraightFlush));
            }

            [Test]
            public void StraightFlushNegativeShuffled()
            {
                var rank = HandRankGetterHelper.Eval(false, false,
                    out var _,
                    CardView.Create(Rank.Ace, SuitMask.Heart),
                    CardView.Create(Rank.Jack, SuitMask.Spade),
                    CardView.Create(Rank.Ten, SuitMask.Club),
                    CardView.Create(Rank.King, SuitMask.Diamond),
                    CardView.Create(Rank.Eight, SuitMask.Heart));

                Assert.That(rank, Is.Not.EqualTo(HandRank.StraightFlush));
            }

            #endregion

            #region Straight-flush with four fingers positive

            [Test]
            public void StraightFlushPositiveFourFingersFiveCards()
            {
                var rank = HandRankGetterHelper.Eval(true, false,
                    out var numberOfCardsFlagged,
                    CardView.Create(Rank.Ace, SuitMask.Heart),
                    CardView.Create(Rank.King, SuitMask.Heart),
                    CardView.Create(Rank.Queen, SuitMask.Heart),
                    CardView.Create(Rank.Jack, SuitMask.Spade),
                    CardView.Create(Rank.Five, SuitMask.Heart));

                Assert.That(rank, Is.EqualTo(HandRank.StraightFlush));
                Assert.That(numberOfCardsFlagged, Is.EqualTo(5));
            }

            [Test]
            public void StraightFlushPositiveFourFingersFourCards()
            {
                var rank = HandRankGetterHelper.Eval(true, false,
                    out var numberOfCardsFlagged,
                    CardView.Create(Rank.Ace, SuitMask.Heart),
                    CardView.Create(Rank.King, SuitMask.Heart),
                    CardView.Create(Rank.Queen, SuitMask.Heart),
                    CardView.Create(Rank.Jack, SuitMask.All),
                    CardView.Create(Rank.Five, SuitMask.Spade));

                Assert.That(rank, Is.EqualTo(HandRank.StraightFlush));
                Assert.That(numberOfCardsFlagged, Is.EqualTo(4));
            }

            [Test]
            public void StraightFlushPositiveFourFingersRepeatedCard()
            {
                var rank = HandRankGetterHelper.Eval(true, false,
                    out var numberOfCardsFlagged,
                    CardView.Create(Rank.Queen, SuitMask.Diamond),
                    CardView.Create(Rank.Queen, SuitMask.Diamond), // Part of the flush
                    CardView.Create(Rank.King, SuitMask.Heart),
                    CardView.Create(Rank.Jack, SuitMask.All),
                    CardView.Create(Rank.Ten, SuitMask.Diamond));

                Assert.That(rank, Is.EqualTo(HandRank.StraightFlush));
                Assert.That(numberOfCardsFlagged, Is.EqualTo(5));
            }

            #endregion

            #region Straight-flush with four fingers negative

            [Test]
            public void StraightFlushNegativeFourFingers()
            {
                var rank = HandRankGetterHelper.Eval(true, false,
                    out var _,
                    CardView.Create(Rank.Ace, SuitMask.Heart),
                    CardView.Create(Rank.King, SuitMask.Spade),
                    CardView.Create(Rank.Queen, SuitMask.Heart),
                    CardView.Create(Rank.Ten, SuitMask.Heart),
                    CardView.Create(Rank.Nine, SuitMask.Heart));

                Assert.That(rank, Is.Not.EqualTo(HandRank.StraightFlush));
            }

            [Test]
            public void StraightFlushWithShortcutGapsTooLong()
            {
                var rank = HandRankGetterHelper.Eval(true, true,
                    out var _,
                    CardView.Create(Rank.Ace, SuitMask.Heart),
                    CardView.Create(Rank.King, SuitMask.Heart),
                    CardView.Create(Rank.Queen, SuitMask.Heart),
                    CardView.Create(Rank.Nine, SuitMask.Heart),
                    CardView.Create(Rank.Eight, SuitMask.Heart));

                Assert.That(rank, Is.EqualTo(HandRank.Flush));
            }

            #endregion
        }
    }
}