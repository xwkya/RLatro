using Balatro.Core.CoreObjects.Cards.CardObject;
using Balatro.Core.CoreObjects.CoreEnums;
using Balatro.Core.CoreObjects.Jokers.Joker;
using Balatro.Core.CoreObjects.Registries;
using Balatro.Core.CoreRules.Scoring;
using Balatro.Core.GameEngine.GameStateController;
using Balatro.Core.GameEngine.GameStateController.PhaseStates;
using Balatro.Core.ObjectsImplementations.Decks;
using Balatro.Core.ObjectsImplementations.Jokers;
using BenchmarkDotNet.Attributes;

namespace RLatro.Benchmarks.CoreRules
{
    [MemoryDiagnoser]
    public class HandScorerBenchmarks
    {
        private GameContext GameContextFullHouse { get; set; }
        private GameContext GameContextPair { get; set; }
        public RoundState RoundStateFullHouse { get; set; }
        public RoundState RoundStatePair { get; set; }
        
        private static readonly List<JokerObject> SomeJokers =
        [
            JokerRegistry.CreateInstance<JollyJoker>(100),
            JokerRegistry.CreateInstance<Joker>(100),
            JokerRegistry.CreateInstance<ZanyJoker>(100, Edition.Poly),
            JokerRegistry.CreateInstance<GluttonousJoker>(100),
            JokerRegistry.CreateInstance<LustyJoker>(100),
        ];
        
        private void FullHouseState(IEnumerable<Card64> hand, IEnumerable<JokerObject> jokers)
        {
            var seed = "ABCDEF";
            var contextBuilder = GameContextBuilder.Create(seed);
            contextBuilder.WithDeck(new DefaultDeckFactory());
            contextBuilder.WithHand(hand.ToList());
            foreach (var j in jokers)
            {
                contextBuilder.WithJoker(j);
            }

            GameContextFullHouse = contextBuilder.CreateGameContext();
            RoundStateFullHouse = new RoundState(GameContextFullHouse);
            GameContextFullHouse.Hand.MoveMany([0, 1, 2, 3, 4], GameContextFullHouse.PlayContainer);
        }
        
        private void PairState(IEnumerable<Card64> hand, IEnumerable<JokerObject> jokers)
        {
            var seed = "ABCDEF";
            var contextBuilder = GameContextBuilder.Create(seed);
            contextBuilder.WithDeck(new DefaultDeckFactory());
            contextBuilder.WithHand(hand.ToList());
            foreach (var j in jokers)
            {
                contextBuilder.WithJoker(j);
            }

            GameContextPair = contextBuilder.CreateGameContext();
            RoundStatePair = new RoundState(GameContextPair);
            GameContextPair.Hand.MoveMany([0, 1], GameContextPair.PlayContainer);
        }
        
        [GlobalSetup]
        public void SetUp()
        {
            FullHouseState(
            [
                Card64.Create(100, Rank.Ace, Suit.Spade, Enhancement.None, Seal.Red, Edition.Holo),
                Card64.Create(101, Rank.King, Suit.Heart, Enhancement.None, Seal.Red, Edition.Holo),
                Card64.Create(102, Rank.Ace, Suit.Spade, Enhancement.None, Seal.Red, Edition.Holo),
                Card64.Create(103, Rank.King, Suit.Heart, Enhancement.None, Seal.Red, Edition.Holo),
                Card64.Create(104, Rank.King, Suit.Club, Enhancement.None, Seal.Red, Edition.Holo),
            ], SomeJokers);
            
            PairState(
            [
                Card64.Create(100, Rank.Ace, Suit.Heart),
                Card64.Create(101, Rank.Ace, Suit.Heart),
            ], SomeJokers);
        }
        
        [Benchmark]
        public void ScoreFullHouseWithRedSeals()
        {
            ScoringCalculation.EvaluateHand(GameContextFullHouse);
        }
        
        [Benchmark]
        public void ScoreSimplePair()
        {
            ScoringCalculation.EvaluateHand(GameContextPair);
        }
    }
}