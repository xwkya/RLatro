namespace Balatro.Core.CoreObjects.Jokers.Joker
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class JokerStaticDescriptionAttribute : Attribute
    {
        public JokerRarity Rarity { get; }
        public int Order { get; } // For reproducible pool ordering
        public string Description { get; set; } // Optional

        public JokerStaticDescriptionAttribute(JokerRarity rarity, int order)
        {
            Rarity = rarity;
            Order = order;
        }
    }
}