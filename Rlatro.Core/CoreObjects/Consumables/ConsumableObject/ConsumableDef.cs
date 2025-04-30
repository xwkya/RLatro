using Balatro.Core.CoreObjects.Contracts.Objects.Consumables;

namespace Balatro.Core.CoreObjects.Consumables.ConsumableObject
{
    public sealed record ConsumableDef(
        ushort BaseSellValue,
        ushort BaseCost,
        IUsageCondition UsageCondition,
        IEffect Effect)
    {
        public Consumable CreateInstance() => new(this);
    }
}