using Balatro.Core.CoreObjects.CoreEnums;

namespace Balatro.Core.CoreObjects.Consumables.ConsumableObject
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class ConsumableStaticDescriptionAttribute : Attribute
    {
        private const int TarotCardCost = 3;
        private const int PlanetCardCost = 3;
        private const int SpectralCardCost = 4;
        
        /// <summary>
        /// A unique static identifier for this Consumable type across ALL consumables.
        /// Also defines the default sort order within its type unless overridden.
        /// </summary>
        public int StaticId { get; }
        public ConsumableType Type { get; }
        public int BasePrice { get; init; }
        public string Description { get; set; }
        public HandRank? SoftLockRank { get; set; } = null; // For planet cards that require a hand to be played

        public ConsumableStaticDescriptionAttribute(int staticId, ConsumableType type)
        {
            StaticId = staticId;
            Type = type;
            BasePrice = GetConsumableBaseCost(type);
        }

        private static int GetConsumableBaseCost(ConsumableType type)
        {
            switch (type)
            {
                case ConsumableType.Tarot:
                    return TarotCardCost;
                case ConsumableType.Planet:
                    return PlanetCardCost;
                case ConsumableType.Spectral:
                    return SpectralCardCost;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}