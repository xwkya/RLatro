namespace Balatro.Core.GameEngine.GameStateController.PersistentStates
{
    public sealed class AppearanceRates
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
        
        public int TarotRate { get; private set; }
        public int JokerRate { get; private set; }
        public int PlanetRate { get; private set; }
        public int SpectralRate { get; private set; }
        public int PlayingCardRate { get; private set; }

        public float CommonRarityRate => CommonRate;
        public float UncommonRarityRate => UncommonRate;
        public float RareRarityRate => RareRate;
    }
}