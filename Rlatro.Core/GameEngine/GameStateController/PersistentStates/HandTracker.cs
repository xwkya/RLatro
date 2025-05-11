using Balatro.Core.CoreObjects.CoreEnums;
using Balatro.Core.CoreRules.CanonicalViews;
using Balatro.Core.CoreRules.Scoring;
using Balatro.Core.GameEngine.GameStateController.EventBus;

namespace Balatro.Core.GameEngine.GameStateController.PersistentStates
{
    public record HandStatistics
    {
        public int PlayedCount { get; set; }
        public int Level { get; set; }
        public int Chips { get; set; }
        public int Mult { get; set; }
    }

    public class HandTracker : IEventBusSubscriber
    {
        private static readonly Dictionary<HandRank, (int mult, int chips)> Scaling = new()
        {
            { HandRank.HighCard, new(1, 10) },
            { HandRank.Pair, new(1, 15) },
            { HandRank.TwoPair, new(1, 20) },
            { HandRank.ThreeOfAKind, new(2, 20) },
            { HandRank.Straight, new(3, 30) },
            { HandRank.Flush, new(2, 15) },
            { HandRank.FullHouse, new(2, 25) },
            { HandRank.FourOfAKind, new(3, 30) },
            { HandRank.StraightFlush, new(4, 40) },
            { HandRank.FiveOfAKind, new(3, 35) },
            { HandRank.FlushHouse, new(4, 40) },
            { HandRank.FlushFive, new(3, 50) },
        };
        
        private static readonly Dictionary<HandRank, (int mult, int chips)> InitialValues = new()
        {
            { HandRank.HighCard, new(1, 5) },
            { HandRank.Pair, new(2, 10) },
            { HandRank.TwoPair, new(2, 20) },
            { HandRank.ThreeOfAKind, new(3, 30) },
            { HandRank.Straight, new(4, 30) },
            { HandRank.Flush, new(4, 35) },
            { HandRank.FullHouse, new(4, 40) },
            { HandRank.FourOfAKind, new(7, 60) },
            { HandRank.StraightFlush, new(8, 100) },
            { HandRank.FiveOfAKind, new(12, 120) },
            { HandRank.FlushHouse, new(14, 140) },
            { HandRank.FlushFive, new(16, 160) },
        };

        private Dictionary<HandRank, HandStatistics> HandsStatistics { get; init; }
        
        public void Subscribe(GameEventBus eventBus)
        {
            eventBus.SubscribeToHandPlayed(OnHandPlayed);
        }
        
        public HandTracker()
        {
            HandsStatistics = new Dictionary<HandRank, HandStatistics>();
            foreach (var handRank in Enum.GetValues(typeof(HandRank)).Cast<HandRank>())
            {
                HandsStatistics[handRank] = new HandStatistics
                {
                    PlayedCount = 0,
                    Level = 1,
                    Chips = InitialValues[handRank].chips,
                    Mult = InitialValues[handRank].mult,
                };
            }
        }
        
        public int GetHandPlayedCount(HandRank handRank)
        {
            return HandsStatistics.TryGetValue(handRank, out var handStatistics) ? handStatistics.PlayedCount : 0;
        }

        public ScoreContext GetHandScore(HandRank rank)
        {
            return new ScoreContext
            {
                HandRank = rank,
                Chips = (uint)HandsStatistics[rank].Chips,
                MultNumerator = (uint)HandsStatistics[rank].Mult,
                MultDenominator = 1,
            };
        }

        private void OnHandPlayed(ReadOnlySpan<CardView> playedCardsViews, HandRank handRank)
        {
            if (HandsStatistics.TryGetValue(handRank, out var handStatistics))
            {
                handStatistics.PlayedCount++;
            }
        }
        
        // TODO: Do the same when a consumable is used
    }
}