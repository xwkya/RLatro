using Balatro.Core.CoreObjects.Cards.CardObject;
using Balatro.Core.CoreObjects.Consumables.ConsumableObject;
using Balatro.Core.CoreObjects.CoreEnums;
using Balatro.Core.CoreObjects.Jokers.Joker;
using Balatro.Core.CoreObjects.Registries;

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
        
        public JokerObject CreateJoker(int jokerStaticId)
        {
            var joker = JokerRegistry.CreateInstance(jokerStaticId, NextId);
            NextId++;
            return joker;
        }

        public Consumable CreateConsumable(int consumableStaticId)
        {
            var consumable = ConsumableRegistry.CreateInstance(consumableStaticId, NextId);
            NextId++;
            return consumable;
        }
    }
}