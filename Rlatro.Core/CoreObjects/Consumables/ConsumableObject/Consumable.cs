using Balatro.Core.CoreObjects.Contracts.Objects.Consumables;
using Balatro.Core.GameEngine.GameStateController;

namespace Balatro.Core.CoreObjects.Consumables.ConsumableObject
{
    public class Consumable
    {
        public ConsumableDef Definition { get; }
        private ushort BonusValue { get; set; }
        
        public ushort BaseSellValue => Definition.BaseSellValue;
        public IEffect Effect => Definition.Effect;
        public uint SellValue => (uint)(Definition.BaseSellValue + BonusValue);
        public bool IsNegative { get; } // For Perkeo
        public bool IsUsable(GameContext context) => Definition.UsageCondition.IsUsable(context);
        public Consumable(ConsumableDef def, bool isNegative = false)
        {
            Definition = def;
            BonusValue = 0;
            IsNegative = isNegative;
        }
    }
}