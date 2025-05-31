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

        /// <summary>
        /// Upgrades a hand by increasing its level and associated chips/mult values
        /// </summary>
        /// <param name="handRank">The hand rank to upgrade</param>
        public void UpgradeHand(HandRank handRank)
        {
            if (HandsStatistics.TryGetValue(handRank, out var handStatistics))
            {
                handStatistics.Level++;
                
                // Add the scaling values for this hand rank
                var scaling = Scaling[handRank];
                handStatistics.Chips += scaling.chips;
                handStatistics.Mult += scaling.mult;
            }
        }

        /// <summary>
        /// Upgrades a hand by a specific number of levels
        /// </summary>
        /// <param name="handRank">The hand rank to upgrade</param>
        /// <param name="levels">Number of levels to upgrade</param>
        public void UpgradeHand(HandRank handRank, int levels)
        {
            for (int i = 0; i < levels; i++)
            {
                UpgradeHand(handRank);
            }
        }

        /// <summary>
        /// Gets the current level of a hand
        /// </summary>
        /// <param name="handRank">The hand rank to check</param>
        /// <returns>The current level of the hand</returns>
        public int GetHandLevel(HandRank handRank)
        {
            return HandsStatistics.TryGetValue(handRank, out var handStatistics) ? handStatistics.Level : 1;
        }

        private void OnHandPlayed(ReadOnlySpan<CardView> playedCardsViews, HandRank handRank)
        {
            if (HandsStatistics.TryGetValue(handRank, out var handStatistics))
            {
                handStatistics.PlayedCount++;
            }
        }
    }
}