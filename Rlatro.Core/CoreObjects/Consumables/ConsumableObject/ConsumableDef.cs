using Balatro.Core.Contracts.Consumables;

namespace Balatro.Core.CoreObjects.Consumables.ConsumableObject
{
    public enum ConsumableType
    {
        Planet,
        Tarot,
        Spectral
    }
    
    public sealed record ConsumableDef(
        ushort BaseCost,
        IUsageCondition UsageCondition,
        IConsumableEffect ConsumableEffect,
        ConsumableType Type)
    {
        public Consumable CreateInstance(uint id) => new(id, this);
    }
}