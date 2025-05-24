namespace Balatro.Core.CoreObjects.Jokers.Joker
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class JokerStaticDescriptionAttribute : Attribute
    {
        /// <summary>
        /// A unique static identifier for this Joker type, used for registration and pooling.
        /// Also defines the default sort order within its rarity unless overridden.
        /// </summary>
        public int StaticId { get; }
        public JokerRarity Rarity { get; }
        public int BasePrice { get; set; }
        public string Description { get; set; } // Optional

        public JokerStaticDescriptionAttribute(int staticId, JokerRarity rarity, int basePrice)
        {
            StaticId = staticId; // This 'order' is now the primary key.
            Rarity = rarity;
        }
    }
}