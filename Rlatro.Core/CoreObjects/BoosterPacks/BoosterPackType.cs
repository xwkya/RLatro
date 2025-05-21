namespace Balatro.Core.CoreObjects.BoosterPacks
{
    public enum BoosterPackType
    {
        ArcanaNormal,
        ArcanaJumbo,
        ArcanaMega,
        CelestialNormal,
        CelestialJumbo,
        CelestialMega,
        SpectralNormal,
        SpectralJumbo,
        SpectralMega,
        StandardNormal,
        StandardJumbo,
        StandardMega,
        BuffoonNormal,
        BuffoonJumbo,
        BuffoonMega,
    }
    
    public static class BoosterPackTypeExtensions
    {
        private static Dictionary<BoosterPackType, (int size, int choice)> PackSizesAndChoices = new()
        {
            { BoosterPackType.ArcanaNormal, (3, 1) },
            { BoosterPackType.ArcanaJumbo, (5, 1) },
            { BoosterPackType.ArcanaMega, (5, 2) },
            { BoosterPackType.CelestialNormal, (3, 1) },
            { BoosterPackType.CelestialJumbo, (5, 1) },
            { BoosterPackType.CelestialMega, (5, 2) },
            { BoosterPackType.SpectralNormal, (2, 1) },
            { BoosterPackType.SpectralJumbo, (4, 1) },
            { BoosterPackType.SpectralMega, (4, 2) },
            { BoosterPackType.StandardNormal, (3, 1) },
            { BoosterPackType.StandardJumbo, (5, 1) },
            { BoosterPackType.StandardMega, (5, 2) },
            { BoosterPackType.BuffoonNormal, (2, 1) },
            { BoosterPackType.BuffoonJumbo, (4, 1) },
            { BoosterPackType.BuffoonMega, (4, 2) }
        };
        
        public static (int size, int choice) GetPackSizeAndChoice(this BoosterPackType packType)
        {
            if (PackSizesAndChoices.TryGetValue(packType, out var values))
            {
                return values;
            }
            throw new ArgumentOutOfRangeException(nameof(packType), $"Unknown BoosterPackType: {packType}");
        }
        
        public static int GetPackPrice(this BoosterPackType packType)
        {
            switch (packType)
            {
                case BoosterPackType.ArcanaNormal:
                case BoosterPackType.CelestialNormal:
                case BoosterPackType.SpectralNormal:
                case BoosterPackType.StandardNormal:
                case BoosterPackType.BuffoonNormal:
                    return 4;
                
                case BoosterPackType.ArcanaJumbo:
                case BoosterPackType.CelestialJumbo:
                case BoosterPackType.SpectralJumbo:
                case BoosterPackType.StandardJumbo:
                case BoosterPackType.BuffoonJumbo:
                    return 6;
                
                case BoosterPackType.ArcanaMega:
                case BoosterPackType.CelestialMega:
                case BoosterPackType.SpectralMega:
                case BoosterPackType.StandardMega:
                case BoosterPackType.BuffoonMega:
                    return 8;
                
                default:
                    throw new ArgumentOutOfRangeException(nameof(packType), $"Unknown BoosterPackType: {packType}");
            }
        }
    }
}