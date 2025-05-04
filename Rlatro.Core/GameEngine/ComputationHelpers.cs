using Balatro.Core.GameEngine.GameStateController;

namespace Balatro.Core.GameEngine
{
    public static class ComputationHelpers
    {
        public static ushort ComputeSellValue(GameContext ctx, int baseSellValue, int bonusValue) =>
            (ushort)(baseSellValue + bonusValue);
    }
}