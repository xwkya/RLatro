using Balatro.Core.CoreObjects.Cards.CardObject;
using Balatro.Core.CoreObjects.CoreEnums;
using Balatro.Core.CoreObjects.Jokers.Joker;
using Balatro.Core.CoreObjects.Registries;
using Balatro.Core.GameEngine.GameStateController;
using Balatro.Core.GameEngine.GameStateController.PhaseActions;
using Balatro.Core.GameEngine.GameStateController.PhaseActions.ActionIntents;
using Balatro.Core.GameEngine.GameStateController.PhaseStates;
using Balatro.Core.ObjectsImplementations.Decks;
using Balatro.Core.ObjectsImplementations.Jokers;

namespace RLatro.Test.CoreRules
{
    [TestFixture]
    public sealed class HandEvaluationTest
    {
        private GameContext GameContext { get; set; }
        private RoundState RoundState { get; set; }
        
        public void SetUpTestState(IEnumerable<Card64> hand, IEnumerable<JokerObject> jokers)
        {
            var seed = "ABCDEF";
            var contextBuilder = GameContextBuilder.Create();
            contextBuilder.WithDeck(new RedDeckFactory());
            contextBuilder.WithHand(hand.ToList());
            foreach (var j in jokers)
            {
                contextBuilder.WithJoker(j);
            }

            GameContext = contextBuilder.CreateGameContext(seed);
            RoundState = new RoundState(GameContext)
            {
                Hands = 1,
            };
        }
        
        [TestCaseSource(nameof(HandTestData))]
        public void TestWholeHand(Card64[] cards, JokerObject[] jokers, int expectedScore)
        {
            SetUpTestState(cards.ToList(), jokers.ToList());
            RoundState.HandleAction(new RoundAction()
            {
                ActionIntent = RoundActionIntent.Play,
                CardIndexes = Enumerable.Range(0, Math.Min(cards.Length, 5)).ToArray(),
            });

            Assert.That(RoundState.CurrentChipsScore, Is.EqualTo(expectedScore));
        }
        
        private static IEnumerable<TestCaseData> HandTestData()
        {
            yield return new TestCaseData(FullHouse, SomeJokers, 7260)
                .SetName("FullHouse with SomeJokers expecting 7260");
            
            yield return new TestCaseData(FullHouse2, SomeJokers, 31536)
                .SetName("Full house 2 with SomeJokers expecting 21024");
            
            yield return new TestCaseData(FullHouseWithSteelInHand, SomeJokers, 249581)
                .SetName("Full house with steel in hand expecting 249581");
        }

        #region Hands

        private static readonly Card64[] FullHouse =
        [
            Card64.Create(100, Rank.Nine, Suit.Club),
            Card64.Create(100, Rank.Nine, Suit.Club),
            Card64.Create(100, Rank.Queen, Suit.Heart, Enhancement.Mult),
            Card64.Create(100, Rank.Queen, Suit.Heart, Enhancement.Mult),
            Card64.Create(100, Rank.Queen, Suit.Heart, Enhancement.Mult)
        ];
        
        private static readonly Card64[] FullHouse2 =
        [
            Card64.Create(100, Rank.Ace, Suit.Spade, Enhancement.None, Seal.Red, Edition.Holo),
            Card64.Create(101, Rank.King, Suit.Heart, Enhancement.None, Seal.Red, Edition.Holo),
            Card64.Create(102, Rank.Ace, Suit.Spade, Enhancement.None, Seal.Red, Edition.Holo),
            Card64.Create(103, Rank.King, Suit.Heart, Enhancement.None, Seal.Red, Edition.Holo),
            Card64.Create(104, Rank.King, Suit.Club, Enhancement.None, Seal.Red, Edition.Holo),
        ];
        
        private static readonly Card64[] FullHouseWithSteelInHand = 
        [
            Card64.Create(100, Rank.Ace, Suit.Spade, Enhancement.None, Seal.Red, Edition.Holo),
            Card64.Create(101, Rank.King, Suit.Heart, Enhancement.None, Seal.Red, Edition.Holo),
            Card64.Create(102, Rank.Ace, Suit.Spade, Enhancement.None, Seal.Red, Edition.Holo),
            Card64.Create(103, Rank.King, Suit.Club, Enhancement.None, Seal.Red, Edition.Holo),
            Card64.Create(104, Rank.King, Suit.Heart, Enhancement.None, Seal.Red, Edition.Poly),
            Card64.Create(105, Rank.Nine, Suit.Club, Enhancement.Steel, Seal.Red),
            Card64.Create(105, Rank.Nine, Suit.Club, Enhancement.Steel, Seal.Red),
        ];

        #endregion
        
        #region Jokers

        private static readonly JokerObject[] SomeJokers =
        [
            JokerRegistry.CreateInstance<JollyJoker>(100),
            JokerRegistry.CreateInstance<Joker>(100),
            JokerRegistry.CreateInstance<ZanyJoker>(100, Edition.Poly),
            JokerRegistry.CreateInstance<GluttonousJoker>(100),
            JokerRegistry.CreateInstance<LustyJoker>(100),
        ];

        #endregion
    }
}