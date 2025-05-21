using Balatro.Core.GameEngine.GameStateController;

namespace Balatro.Core.Contracts.Consumables
{
    public interface IConsumableEffect
    {
        void Apply(GameContext context, int[] targetCards);
    }
}