using Balatro.Core.CoreObjects.Consumables.ConsumableObject;
using Balatro.Core.GameEngine.GameStateController;

namespace Balatro.Core.ObjectsImplementations.Consumables
{
    [ConsumableStaticDescription(1, ConsumableType.Tarot)]
    public class TheFool : Consumable
    {
        public TheFool(int staticId, uint runtimeId, bool isNegative = false) : base(staticId, runtimeId, isNegative)
        {
        }

        public override void Apply(GameContext context, byte[] targetCards)
        {
            throw new NotImplementedException();
        }

        public override bool IsUsable(GameContext ctx, byte[] targetCards)
        {
            return ctx.PersistentState.TheFoolStorage is not null;
        }

        public override int BaseCost => 3;
    }
}