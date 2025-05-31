using Balatro.Core.GameEngine.PseudoRng;

namespace Balatro.Core.CoreObjects.CoreEnums
{
    public enum Enhancement : byte
    {
        None,
        Bonus,
        Mult,
        Wild,
        Glass,
        Steel,
        Stone,
        Gold,
        Lucky,
    }

    public static class EnhancementExtensions
    {
        public static readonly Enhancement[] SpectralEnhancementChoices =
        [
            Enhancement.Bonus,
            Enhancement.Mult,
            Enhancement.Wild,
            Enhancement.Glass,
            Enhancement.Steel,
            Enhancement.Stone,
            Enhancement.Gold,
            Enhancement.Lucky
        ];
        
        public static Enhancement GetRandomEnhancement(RngController rng, RngActionType actionType)
        {
            var randomEnhancementIndex =
                rng.RandomInt(0, EnhancementExtensions.SpectralEnhancementChoices.Length - 1, actionType);
            return EnhancementExtensions.SpectralEnhancementChoices[randomEnhancementIndex];
        }
    }
}