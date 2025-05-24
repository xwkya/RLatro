using Balatro.Core.Contracts.Consumables;
using Balatro.Core.GameEngine.GameStateController;

namespace Balatro.Core.CoreObjects.Consumables.ConsumableObject
{
    public abstract class Consumable : IConsumableEffect, IUsageCondition
    {
        public ConsumableType Type;
        public int BonusValue { get; set; }
        
        public int StaticId { get; } // Static definition ID
        public uint Id { get; private set; }
        
        public bool IsNegative { get; } // For Perkeo
        
        public abstract void Apply(GameContext context, int[] targetCards);
        public abstract bool IsUsable(GameContext ctx, int[] targetCards);
        
        public Consumable(int staticId, uint id, bool isNegative = false)
        {
            Id = id;
            StaticId = staticId;
            BonusValue = 0;
            IsNegative = isNegative;
        }
    }
}