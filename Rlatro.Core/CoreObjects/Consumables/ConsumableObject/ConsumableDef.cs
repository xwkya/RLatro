using Balatro.Core.Contracts.Consumables;

namespace Balatro.Core.CoreObjects.Consumables.ConsumableObject
{
    public sealed record ConsumableDef(
        ushort BaseSellValue,
        ushort BaseCost,
        IUsageCondition UsageCondition,
        IConsumableEffect ConsumableEffect)
    {
        public Consumable CreateInstance(uint id) => new(id, this);
    }
}