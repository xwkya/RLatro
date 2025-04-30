using Balatro.Core.GameEngine.GameStateController;

namespace Balatro.Core.CoreObjects.Contracts.Objects.Consumables
{
    public interface IEffect
    {
        void Apply(GameContext context, byte[] targetCards);
    }
}