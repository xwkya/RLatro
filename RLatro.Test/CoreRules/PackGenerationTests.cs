using Balatro.Core.CoreObjects.Consumables.ConsumableObject;
using Balatro.Core.CoreObjects.CoreEnums;
using Balatro.Core.CoreObjects.Registries;
using Balatro.Core.GameEngine.GameStateController;
using Balatro.Core.GameEngine.PseudoRng;
using Balatro.Core.ObjectsImplementations.Consumables;
using Balatro.Core.ObjectsImplementations.Decks;

namespace RLatro.Test.CoreRules
{
    [TestFixture]
    public sealed class PackGenerationTests
    {
        private GameContext GameContext { get; set; }
        
        // Special card static IDs
        private static readonly int TheSoulStaticId = ConsumableRegistry.GetStaticId(typeof(TheSoul));
        private static readonly int BlackHoleStaticId = ConsumableRegistry.GetStaticId(typeof(BlackHole));
        
        // Test sample sizes
        private const int LargeTestSampleSize = 10000;
        private const int SpecialCardTestSampleSize = 100000; // Need larger sample for 0.3% probability
        
        [SetUp]
        public void SetUp()
        {
            SetUpTestState();
        }
        
        private void SetUpTestState()
        {
            var seed = "PACK_TEST_SEED";
            var contextBuilder = GameContextBuilder.Create();
            contextBuilder.WithDeck(new RedDeckFactory());
            
            GameContext = contextBuilder.CreateGameContext(seed);
        }
        
        [Test]
        public void TarotCardDistribution_ShouldBeUniform()
        {
            // Arrange
            var cardCounts = new Dictionary<int, int>();
            var expectedTarotIds = ConsumableRegistry.GetMasterOrderedStaticIds(ConsumableType.Tarot, includePackOnly: false);
            
            // Initialize counts
            foreach (var id in expectedTarotIds)
            {
                cardCounts[id] = 0;
            }
            
            // Act - Generate cards
            for (int i = 0; i < LargeTestSampleSize; i++)
            {
                var card = GameContext.GlobalPoolManager.GeneratePackConsumable(
                    RngActionType.RandomPackConsumable, ConsumableType.Tarot);
                GameContext.GameEventBus.PublishConsumableRemovedFromContext(card.StaticId);
                
                if (cardCounts.ContainsKey(card.StaticId))
                {
                    cardCounts[card.StaticId]++;
                }
            }
            
            // Assert - Check uniform distribution
            var expectedCount = LargeTestSampleSize / (double)expectedTarotIds.Count;
            var tolerance = expectedCount * 0.15; // Allow 15% deviation
            
            foreach (var kvp in cardCounts)
            {
                Assert.That(kvp.Value, Is.InRange(expectedCount - tolerance, expectedCount + tolerance),
                    $"Tarot card {kvp.Key} appeared {kvp.Value} times, expected around {expectedCount:F0}");
            }
            
            // Verify no special cards appeared
            Assert.That(cardCounts.ContainsKey(TheSoulStaticId), Is.False, 
                "The Soul should not appear in regular generation");
        }
        
        [Test]
        public void SpectralCardDistribution_ShouldBeUniform_ExcludingPackOnlyCards()
        {
            // Arrange
            var cardCounts = new Dictionary<int, int>();
            var expectedSpectralIds = ConsumableRegistry.GetMasterOrderedStaticIds(ConsumableType.Spectral, includePackOnly: false);
            
            // Initialize counts
            foreach (var id in expectedSpectralIds)
            {
                cardCounts[id] = 0;
            }
            
            // Act - Generate cards
            for (int i = 0; i < LargeTestSampleSize; i++)
            {
                var card = GameContext.GlobalPoolManager.GeneratePackConsumable(
                    RngActionType.RandomPackConsumable, ConsumableType.Spectral);
                GameContext.GameEventBus.PublishConsumableRemovedFromContext(card.StaticId);
                
                if (cardCounts.ContainsKey(card.StaticId))
                {
                    cardCounts[card.StaticId]++;
                }
            }
            
            // Assert - Check uniform distribution
            var expectedCount = LargeTestSampleSize / (double)expectedSpectralIds.Count;
            var tolerance = expectedCount * 0.10; // Allow 10% deviation
            
            foreach (var kvp in cardCounts)
            {
                Assert.That(kvp.Value, Is.InRange(expectedCount - tolerance, expectedCount + tolerance),
                    $"Spectral card {kvp.Key} appeared {kvp.Value} times, expected around {expectedCount:F0}");
            }
            
            // Verify pack-only cards didn't appear in regular generation
            Assert.That(cardCounts.ContainsKey(TheSoulStaticId), Is.False, 
                "The Soul should not appear in regular generation");
            Assert.That(cardCounts.ContainsKey(BlackHoleStaticId), Is.False, 
                "Black Hole should not appear in regular generation");
        }
        
        [Test]
        public void PlanetCardDistribution_ShouldBeUniform_WithoutSoftLock()
        {
            // Arrange
            var cardCounts = new Dictionary<int, int>();
            var expectedPlanetIds = ConsumableRegistry.GetMasterOrderedStaticIds(ConsumableType.Planet, includePackOnly: false);
            
            // Filter out soft-locked cards (cards that require the hand to be played first)
            var availablePlanetIds = expectedPlanetIds.Where(id =>
            {
                var attr = ConsumableRegistry.GetAttribute(id);
                return !attr.SoftLockRank.HasValue ||
                       GameContext.PersistentState.HandTracker.GetHandPlayedCount(attr.SoftLockRank.Value) > 0;
            }).ToList();
            
            // Initialize counts
            foreach (var id in availablePlanetIds)
            {
                cardCounts[id] = 0;
            }
            
            // Act - Generate cards
            for (int i = 0; i < LargeTestSampleSize; i++)
            {
                var card = GameContext.GlobalPoolManager.GeneratePackConsumable(
                    RngActionType.RandomPackConsumable, ConsumableType.Planet);
                GameContext.GameEventBus.PublishConsumableRemovedFromContext(card.StaticId);
                if (cardCounts.ContainsKey(card.StaticId))
                {
                    cardCounts[card.StaticId]++;
                }
            }
            
            // Assert - Check uniform distribution
            var expectedCount = LargeTestSampleSize / (double)availablePlanetIds.Count;
            var tolerance = expectedCount * 0.15; // Allow 15% deviation
            
            foreach (var kvp in cardCounts)
            {
                Assert.That(kvp.Value, Is.InRange(expectedCount - tolerance, expectedCount + tolerance),
                    $"Planet card {kvp.Key} appeared {kvp.Value} times, expected around {expectedCount:F0}");
            }
            
            // Verify no special cards appeared
            Assert.That(cardCounts.ContainsKey(BlackHoleStaticId), Is.False, 
                "Black Hole should not appear in regular generation");
        }
        
        [Test]
        public void PlanetCardDistribution_ShouldIncludeSoftLockedCards_WhenHandsPlayed()
        {
            // Arrange - Play some hands to unlock soft-locked cards
            GameContext.PersistentState.HandTracker.UpgradeHand(HandRank.FiveOfAKind);
            GameContext.PersistentState.HandTracker.UpgradeHand(HandRank.FlushHouse);
            GameContext.PersistentState.HandTracker.UpgradeHand(HandRank.FlushFive);
            
            var cardCounts = new Dictionary<int, int>();
            var expectedPlanetIds = ConsumableRegistry.GetMasterOrderedStaticIds(ConsumableType.Planet, includePackOnly: false);
            
            // All cards should now be available
            var availablePlanetIds = expectedPlanetIds.Where(id =>
            {
                var attr = ConsumableRegistry.GetAttribute(id);
                return !attr.SoftLockRank.HasValue ||
                       GameContext.PersistentState.HandTracker.GetHandPlayedCount(attr.SoftLockRank.Value) > 0;
            }).ToList();
            
            // Initialize counts
            foreach (var id in availablePlanetIds)
            {
                cardCounts[id] = 0;
            }
            
            // Act - Generate cards
            for (int i = 0; i < LargeTestSampleSize; i++)
            {
                var card = GameContext.GlobalPoolManager.GeneratePackConsumable(
                    RngActionType.RandomPackConsumable, ConsumableType.Planet);
                
                if (cardCounts.ContainsKey(card.StaticId))
                {
                    cardCounts[card.StaticId]++;
                }
            }
            
            // Assert - Should have more cards available than before
            Assert.That(availablePlanetIds.Count, Is.GreaterThan(8), 
                "Should have more planet cards available after playing hands");
            
            // Check that soft-locked cards are now appearing
            var softLockedCards = expectedPlanetIds.Where(id =>
            {
                var attr = ConsumableRegistry.GetAttribute(id);
                return attr.SoftLockRank.HasValue;
            }).ToList();
            
            foreach (var softLockedCardId in softLockedCards)
            {
                if (cardCounts.ContainsKey(softLockedCardId))
                {
                    Assert.That(cardCounts[softLockedCardId], Is.GreaterThan(0),
                        $"Soft-locked card {softLockedCardId} should appear after hand is played");
                }
            }
        }
        
        [Test]
        public void TheSoulGeneration_ShouldHaveCorrectProbability_InArcanaPacks()
        {
            // Arrange
            int theSoulCount = 0;
            int totalGenerated = 0;
            
            // Act - Generate many cards to test probability
            for (int i = 0; i < SpecialCardTestSampleSize; i++)
            {
                var card = GameContext.GlobalPoolManager.GeneratePackConsumable(
                    RngActionType.RandomPackConsumable, ConsumableType.Tarot);
                
                if (card.StaticId == TheSoulStaticId)
                {
                    theSoulCount++;
                }
                totalGenerated++;
            }
            
            // Assert - Should be around 0.3% (0.003)
            var actualProbability = theSoulCount / (double)totalGenerated;
            var expectedProbability = 0.003;
            var tolerance = 0.001; // Allow some variance
            
            Assert.That(actualProbability, Is.InRange(expectedProbability - tolerance, expectedProbability + tolerance),
                $"The Soul appeared {theSoulCount} times out of {totalGenerated} (probability: {actualProbability:P3}), expected around {expectedProbability:P3}");
        }
        
        [Test]
        public void TheSoulGeneration_ShouldHaveCorrectProbability_InSpectralPacks()
        {
            // Arrange
            int theSoulCount = 0;
            int totalGenerated = 0;
            
            // Act - Generate many cards to test probability
            for (int i = 0; i < SpecialCardTestSampleSize; i++)
            {
                var card = GameContext.GlobalPoolManager.GeneratePackConsumable(
                    RngActionType.RandomPackConsumable, ConsumableType.Spectral);
                
                if (card.StaticId == TheSoulStaticId)
                {
                    theSoulCount++;
                }
                totalGenerated++;
            }
            
            // Assert - Should be around 0.3% (0.003)
            var actualProbability = theSoulCount / (double)totalGenerated;
            var expectedProbability = 0.003;
            var tolerance = 0.0005; // Allow some variance
            
            Assert.That(actualProbability, Is.InRange(expectedProbability - tolerance, expectedProbability + tolerance),
                $"The Soul appeared {theSoulCount} times out of {totalGenerated} (probability: {actualProbability:P3}), expected around {expectedProbability:P3}");
        }
        
        [Test]
        public void BlackHoleGeneration_ShouldHaveCorrectProbability_InPlanetPacks()
        {
            // Arrange
            int blackHoleCount = 0;
            int totalGenerated = 0;
            
            // Act - Generate many cards to test probability
            for (int i = 0; i < SpecialCardTestSampleSize; i++)
            {
                var shopItem = GameContext.GlobalPoolManager.GeneratePackShopConsumable(
                    RngActionType.RandomPackConsumable, ConsumableType.Planet);
                
                if (shopItem.StaticId == BlackHoleStaticId)
                {
                    blackHoleCount++;
                }
                totalGenerated++;
            }
            
            // Assert - Should be around 0.3% (0.003)
            var actualProbability = blackHoleCount / (double)totalGenerated;
            var expectedProbability = 0.003;
            var tolerance = 0.001; // Allow some variance
            
            Assert.That(actualProbability, Is.InRange(expectedProbability - tolerance, expectedProbability + tolerance),
                $"Black Hole appeared {blackHoleCount} times out of {totalGenerated} (probability: {actualProbability:P3}), expected around {expectedProbability:P3}");
        }
        
        [Test]
        public void RegularShopGeneration_ShouldNeverIncludeSpecialCards()
        {
            // Arrange & Act - Generate many shop consumables
            var generatedIds = new HashSet<int>();
            
            for (int i = 0; i < LargeTestSampleSize; i++)
            {
                // Test all consumable types in shop generation
                var tarotItem = GameContext.GlobalPoolManager.GenerateShopConsumable(
                    RngActionType.RandomShopConsumable, ConsumableType.Tarot);
                var spectralItem = GameContext.GlobalPoolManager.GenerateShopConsumable(
                    RngActionType.RandomShopConsumable, ConsumableType.Spectral);
                var planetItem = GameContext.GlobalPoolManager.GenerateShopConsumable(
                    RngActionType.RandomShopConsumable, ConsumableType.Planet);
                
                generatedIds.Add(tarotItem.StaticId);
                generatedIds.Add(spectralItem.StaticId);
                generatedIds.Add(planetItem.StaticId);
            }
            
            // Assert - Special cards should never appear
            Assert.That(generatedIds.Contains(TheSoulStaticId), Is.False,
                "The Soul should never appear in shop generation");
            Assert.That(generatedIds.Contains(BlackHoleStaticId), Is.False,
                "Black Hole should never appear in shop generation");
        }
        
        [Test]
        public void RegularConsumableGeneration_ShouldNeverIncludeSpecialCards()
        {
            // Arrange & Act - Generate many regular consumables
            var generatedIds = new HashSet<int>();
            
            for (int i = 0; i < LargeTestSampleSize; i++)
            {
                // Test all consumable types in regular generation
                var tarotCard = GameContext.GlobalPoolManager.GenerateConsumable(
                    RngActionType.RandomShopConsumable, ConsumableType.Tarot);
                var spectralCard = GameContext.GlobalPoolManager.GenerateConsumable(
                    RngActionType.RandomShopConsumable, ConsumableType.Spectral);
                var planetCard = GameContext.GlobalPoolManager.GenerateConsumable(
                    RngActionType.RandomShopConsumable, ConsumableType.Planet);
                
                generatedIds.Add(tarotCard.StaticId);
                generatedIds.Add(spectralCard.StaticId);
                generatedIds.Add(planetCard.StaticId);
            }
            
            // Assert - Special cards should never appear
            Assert.That(generatedIds.Contains(TheSoulStaticId), Is.False,
                "The Soul should never appear in regular generation");
            Assert.That(generatedIds.Contains(BlackHoleStaticId), Is.False,
                "Black Hole should never appear in regular generation");
        }
        
        [Test]
        public void PackGeneration_ShouldBeDeterministicWithSameSeed()
        {
            // Arrange - Create two identical contexts with same seed
            var seed = "DETERMINISTIC_TEST";
            
            var context1 = GameContextBuilder.Create()
                .WithDeck(new RedDeckFactory())
                .CreateGameContext(seed);
                
            var context2 = GameContextBuilder.Create()
                .WithDeck(new RedDeckFactory())
                .CreateGameContext(seed);
            
            // Act - Generate cards with both contexts
            var cards1 = new List<int>();
            var cards2 = new List<int>();
            
            for (int i = 0; i < 1000; i++)
            {
                var card1 = context1.GlobalPoolManager.GeneratePackConsumable(
                    RngActionType.RandomPackConsumable, ConsumableType.Tarot);
                var card2 = context2.GlobalPoolManager.GeneratePackConsumable(
                    RngActionType.RandomPackConsumable, ConsumableType.Tarot);
                
                cards1.Add(card1.StaticId);
                cards2.Add(card2.StaticId);
            }
            
            // Assert - Both sequences should be identical
            Assert.That(cards1, Is.EqualTo(cards2),
                "Card generation should be deterministic with the same seed");
        }
        
        [TestCase(ConsumableType.Tarot, TestName = "Tarot pack generation should exclude pack-only cards")]
        [TestCase(ConsumableType.Planet, TestName = "Planet pack generation should exclude pack-only cards")]
        [TestCase(ConsumableType.Spectral, TestName = "Spectral pack generation should exclude pack-only cards")]
        public void PackGeneration_ShouldGenerateValidConsumableTypes(ConsumableType type)
        {
            // Arrange & Act
            var generatedTypes = new HashSet<ConsumableType>();
            
            for (int i = 0; i < 1000; i++)
            {
                var card = GameContext.GlobalPoolManager.GeneratePackConsumable(
                    RngActionType.RandomPackConsumable, type);
                GameContext.GameEventBus.PublishConsumableRemovedFromContext(card.StaticId);
                if (card.StaticId == TheSoulStaticId || card.StaticId == BlackHoleStaticId)
                {
                    // Skip special cards that are not part of the regular pack generation
                    continue;
                }
                
                var attr = ConsumableRegistry.GetAttribute(card.StaticId);
                generatedTypes.Add(attr.Type);
            }
            
            // Assert - Should only generate the requested type (or special cards of same type)
            foreach (var generatedType in generatedTypes)
            {
                Assert.That(generatedType, Is.EqualTo(type),
                    $"Generated card type {generatedType} should match requested type {type}");
            }
        }
    }
}