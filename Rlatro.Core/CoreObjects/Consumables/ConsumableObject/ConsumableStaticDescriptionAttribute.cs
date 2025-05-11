using Balatro.Core.CoreObjects.CoreEnums;

namespace Balatro.Core.CoreObjects.Consumables.ConsumableObject
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class ConsumableStaticDescriptionAttribute : Attribute
    {
        /// <summary>
        /// A unique static identifier for this Consumable type across ALL consumables.
        /// Also defines the default sort order within its type unless overridden.
        /// </summary>
        public int StaticId { get; }
        public ConsumableType Type { get; } // Tarot, Planet, Spectral
        public string Description { get; set; } // Optional
        public HandRank? SoftLockRank { get; set; } = null; // For planet cards that require a hand to be played

        public ConsumableStaticDescriptionAttribute(int staticId, ConsumableType type)
        {
            StaticId = staticId;
            Type = type;
        }
    }
}