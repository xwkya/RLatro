using Balatro.Core.CoreObjects.CoreEnums;

namespace Balatro.Core.CoreObjects.Consumables.ConsumableObject
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class ConsumableDefAttribute : Attribute
    {
        public int Order { get; }
        public ConsumableType Type { get; } // Tarot, Planet, Spectral
        public string Key { get; }

        // For Planet Soft Locks
        public HandRank? SoftLockRank { get; set; } = null;

        public ConsumableDefAttribute(string key, ConsumableType type, int order)
        {
            Key = key;
            Type = type;
            Order = order;
        }
    }
}