using Balatro.Core.CoreObjects.Cards.CardObject;
using Balatro.Core.CoreObjects.CoreEnums;
using Balatro.Core.GameEngine.GameStateController;
using Balatro.Core.GameEngine.GameStateController.PhaseActions;
using Balatro.Core.GameEngine.GameStateController.PhaseStates;
using Balatro.Core.ObjectsImplementations.Decks;

namespace RLatro.Test.CoreRules
{
    [TestFixture]
    public sealed class HandEvaluationTest
    {
        private GameContext GameContext { get; set; }
        private RoundState RoundState { get; set; }
        [SetUp]
        public void SetUp()
        {
            var seed = "ABCDEF";
            var contextBuilder = GameContextBuilder.Create(seed);
            contextBuilder.WithDeck(new DefaultDeckFactory());
            contextBuilder.WithHand(
            [
                Card64.Create(100, Rank.Ace, Suit.Club, Enhancement.Glass),
                Card64.Create(100, Rank.King, Suit.Club),
                Card64.Create(100, Rank.Queen, Suit.Club),
                Card64.Create(100, Rank.Jack, Suit.Club),
                Card64.Create(100, Rank.Ten, Suit.Club),
            ]);

            GameContext = contextBuilder.CreateGameContext();
            RoundState = new RoundState(GameContext);
        }
        
        [Test]
        public void ScoreStraight()
        {
            RoundState.HandleAction(new RoundAction()
            {
                ActionIntent = RoundActionIntent.Play,
                CardIndexes = [0, 1, 2, 3, 4],
            });
        }
    }
}