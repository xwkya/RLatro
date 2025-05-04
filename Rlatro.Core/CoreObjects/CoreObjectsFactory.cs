using Balatro.Core.CoreObjects.Cards.CardObject;
using Balatro.Core.CoreObjects.Consumables.ConsumableObject;
using Balatro.Core.CoreObjects.CoreEnums;

namespace Balatro.Core.CoreObjects
{
    /// <summary>
    /// Responsible for creating and allocating ids to core objects
    /// </summary>
    public class CoreObjectsFactory
    {
        private uint NextId = 0;
        public Card64 CreateCard(Rank rank, Suit suit)
        {
            var card = Card64.Create(NextId, rank, suit, Enhancement.None, Seal.None, Edition.None);
            NextId++;
            return card;
        }
        
        public Consumable CreateConsumable(ConsumableDef def, bool isNegative = false)
        {
            var consumable = def.CreateInstance(NextId);
            NextId++;
            return consumable;
        }
    }
}