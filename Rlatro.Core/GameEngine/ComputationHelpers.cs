using Balatro.Core.GameEngine.GameStateController;

namespace Balatro.Core.GameEngine
{
    public static class ComputationHelpers
    {
        public static ushort ComputeSellValue(GameContext ctx, byte baseSellValue, ushort bonusValue) =>
            (ushort)(baseSellValue + bonusValue);
    }
}