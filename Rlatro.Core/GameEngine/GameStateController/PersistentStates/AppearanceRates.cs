using Balatro.Core.CoreObjects.BoosterPacks;
using Balatro.Core.CoreObjects.Vouchers;
using Balatro.Core.GameEngine.GameStateController.EventBus;

namespace Balatro.Core.GameEngine.GameStateController.PersistentStates
{
    public sealed class AppearanceRates : IEventBusSubscriber
    {
        // Shop base appearance rates
        private const int BaseTarotRate = 4;
        private const int BaseJokerRate = 20;
        private const int BasePlanetRate = 4;
        private const int BaseSpectralRate = 0;
        private const int BasePlayingCardRate = 0;
        
        // Rarity rates
        private const float CommonRate = 0.7f;
        private const float UncommonRate = 0.25f;
        private const float RareRate = 0.05f;
        
        // Joker edition rates
        private const float BaseNegativeJokerRate = 0.003f;
        private const float BasePolychromeJokerRate = 0.003f;
        private const float BaseHoloJokerRate = 0.014f;
        private const float BaseFoilJokerRate = 0.02f;
        
        // Playing Cards edition rates
        private const float BasePolychromePlayingCardRate = 0.012f;
        private const float BaseHoloPlayingCardRate = 0.028f;
        private const float BaseFoilPlayingCardRate = 0.04f;
        
        // Playing cards enhancement rates
        private const float EnhancementChance = 0.4f;
        private const float SealChance = 0.2f;
        
        public int JokerRate { get; private set; }
        public int PlanetRate { get; private set; }
        public int SpectralRate { get; private set; }
        public int PlayingCardRate { get; private set; }
        public int TarotRate { get; private set; }
        
        public int NegativeJokerRate { get; private set; }
        public float PolychromeJokerRate { get; private set; }
        public float HoloJokerRate { get; private set; }
        public float FoilJokerRate { get; private set; }
        
        public float PolychromePlayingCardRate { get; private set; }
        public float HoloPlayingCardRate { get; private set; }
        public float FoilPlayingCardRate { get; private set; }
        
        private static readonly Dictionary<BoosterPackType, float> BoosterPackWeights = new()
        {
            // Standard packs
            { BoosterPackType.StandardNormal, 4f },
            { BoosterPackType.StandardJumbo, 2f },
            { BoosterPackType.StandardMega, 0.5f },
            
            // Arcana packs
            { BoosterPackType.ArcanaNormal, 4f },
            { BoosterPackType.ArcanaJumbo, 2f },
            { BoosterPackType.ArcanaMega, 0.5f },
            
            // Celestial packs
            { BoosterPackType.CelestialNormal, 4f },
            { BoosterPackType.CelestialJumbo, 2f },
            { BoosterPackType.CelestialMega, 0.5f },
            
            // Spectral packs
            { BoosterPackType.SpectralNormal, 0.6f },
            { BoosterPackType.SpectralJumbo, 0.3f },
            { BoosterPackType.SpectralMega, 0.07f },
            
            // Buffoon packs
            { BoosterPackType.BuffoonNormal, 1.2f },
            { BoosterPackType.BuffoonJumbo, 0.6f },
            { BoosterPackType.BuffoonMega, 0.15f }
        };

        private static Dictionary<BoosterPackType, float> BoosterPackNormalizedWeights = new();
        

        public float CommonRarityRate => CommonRate;
        public float UncommonRarityRate => UncommonRate;
        public float RareRarityRate => RareRate;
        public float EnhancementChanceRate => EnhancementChance;
        public float SealChanceRate => SealChance;
        
        private PersistentState PersistentState { get; }
        
        static AppearanceRates()
        {
            // Calculate normalized weights for booster packs
            CalculateNormalizedBoosterPackWeights();
        }

        public AppearanceRates(PersistentState persistentState)
        {
            PersistentState = persistentState;
            RecomputeAppearanceRates(0); // Initialization of the rates
        }

        public void Reset()
        {
            RecomputeAppearanceRates(0);
        }
        
        public void Subscribe(GameEventBus eventBus)
        {
            eventBus.SubscribeToVoucherBought(RecomputeAppearanceRates);
        }
        
        /// <summary>
        /// Gets the weight for a specific booster pack type for weighted random selection
        /// </summary>
        public float GetBoosterPackWeight(BoosterPackType packType)
        {
            return BoosterPackNormalizedWeights.GetValueOrDefault(packType, 1f);
        }

        /// <summary>
        /// Gets all available booster pack types with their weights
        /// </summary>
        public IReadOnlyDictionary<BoosterPackType, float> GetAllBoosterPackWeights()
        {
            return BoosterPackNormalizedWeights;
        }
        
        private static float GetTotalBoosterPackWeight()
        {
            return BoosterPackWeights.Values.Sum();
        }

        private static void CalculateNormalizedBoosterPackWeights()
        {
            float totalWeight = GetTotalBoosterPackWeight();

            BoosterPackNormalizedWeights = BoosterPackWeights.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value / totalWeight);
        }

        private void RecomputeAppearanceRates(VoucherType _)
        {
            var editionMultiplier = PersistentState.OwnedVouchers[(int)VoucherType.GlowUp] ? 4 :
                PersistentState.OwnedVouchers[(int)VoucherType.Hone] ? 2 : 1;

            PolychromeJokerRate = BasePolychromeJokerRate * editionMultiplier;
            HoloJokerRate = BaseHoloJokerRate * editionMultiplier;
            FoilJokerRate = BaseFoilJokerRate * editionMultiplier;

            PolychromePlayingCardRate = BasePolychromePlayingCardRate * editionMultiplier;
            HoloPlayingCardRate = BaseHoloPlayingCardRate * editionMultiplier;
            FoilPlayingCardRate = BaseFoilPlayingCardRate * editionMultiplier;
            
            // Planet voucher
            var planetRateMultiplier = PersistentState.OwnedVouchers[(int)VoucherType.PlanetTycoon] ? 4 :
                PersistentState.OwnedVouchers[(int)VoucherType.PlanetMerchant] ? 2 : 1;
            
            PlanetRate = BasePlanetRate * planetRateMultiplier;
            
            // Tarot voucher
            var tarotRateMultiplier = PersistentState.OwnedVouchers[(int)VoucherType.TarotTycoon] ? 4 :
                PersistentState.OwnedVouchers[(int)VoucherType.TarotMerchant] ? 2 : 1;
            
            TarotRate = BaseTarotRate * tarotRateMultiplier;
        
            // Omen globe
            if (PersistentState.OwnedVouchers[(int)VoucherType.OmenGlobe])
            {
                SpectralRate = 4;
            }
            
            // Playing cards appearance rate
            var playingCardBonusRate = PersistentState.OwnedVouchers[(int)VoucherType.MagicTrick] ? 8 : 0;

            PlayingCardRate = playingCardBonusRate;
        }
    }
}