using Balatro.Core.GameEngine.GameStateController;

namespace Balatro.Core.CoreObjects.Contracts.Objects.Consumables
{
    public interface IUsageCondition
    {
        bool IsUsable(GameContext context);
    }
}