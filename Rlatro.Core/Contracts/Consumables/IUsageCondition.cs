using Balatro.Core.GameEngine.GameStateController;

namespace Balatro.Core.Contracts.Consumables
{
    public interface IUsageCondition
    {
        bool IsUsable(GameContext context, int[] cardIndexes);
    }
}