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
        

        public float CommonRarityRate => CommonRate;
        public float UncommonRarityRate => UncommonRate;
        public float RareRarityRate => RareRate;
        public float EnhancementChanceRate => EnhancementChance;
        public float SealChanceRate => SealChance;
        
        private PersistentState PersistentState { get; }

        public AppearanceRates(PersistentState persistentState)
        {
            PersistentState = persistentState;
            RecomputeAppearanceRates(0); // Initialization of the rates
        }
        
        public void Subscribe(GameEventBus eventBus)
        {
            eventBus.SubscribeToVoucherBought(RecomputeAppearanceRates);
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
            var playingCardBonusRate = PersistentState.OwnedVouchers[(int)VoucherType.MagicTrick] ? 8 :
                PersistentState.OwnedVouchers[(int)VoucherType.Illusion] ? 4 : 0;

            PlayingCardRate = playingCardBonusRate;
        }
    }
}