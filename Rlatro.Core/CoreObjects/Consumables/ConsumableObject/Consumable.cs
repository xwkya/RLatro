using Balatro.Core.Contracts.Consumables;
using Balatro.Core.GameEngine.GameStateController;

namespace Balatro.Core.CoreObjects.Consumables.ConsumableObject
{
    public class Consumable
    {
        public ConsumableDef Definition { get; }
        private ushort BonusValue { get; set; }
        public uint Id { get; private set; }
        
        public ushort BaseSellValue => Definition.BaseSellValue;
        public IConsumableEffect ConsumableEffect => Definition.ConsumableEffect;
        public uint SellValue => (uint)(Definition.BaseSellValue + BonusValue);
        public bool IsNegative { get; } // For Perkeo

        public bool IsUsable(GameContext context, byte[] cardIndexes)
        {
            return Definition.UsageCondition.IsUsable(context, cardIndexes);
        }
        public void ApplyEffect(GameContext context, byte[] targetCards)
        {
            ConsumableEffect.Apply(context, targetCards);
        }
        public Consumable(uint id, ConsumableDef def, bool isNegative = false)
        {
            Id = id;
            Definition = def;
            BonusValue = 0;
            IsNegative = isNegative;
        }
    }
}