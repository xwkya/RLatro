using Balatro.Core.CoreObjects.Cards.CardObject;
using Balatro.Core.CoreObjects.Consumables.ConsumableObject;
using Balatro.Core.CoreObjects.CoreEnums;
using Balatro.Core.CoreObjects.Jokers.Joker;
using Balatro.Core.CoreObjects.Registries;
using Balatro.Core.CoreObjects.Shop.ShopObjects;

namespace Balatro.Core.CoreObjects
{
    /// <summary>
    /// Responsible for creating and allocating ids to core objects
    /// </summary>
    public class CoreObjectsFactory
    {
        private uint NextId = 0;
        public Card64 CreateCard(
            Rank rank,
            Suit suit, 
            Enhancement enhancement = Enhancement.None,
            Seal seal = Seal.None,
            Edition edition = Edition.None)
        {
            var card = Card64.Create(NextId, rank, suit, enhancement, seal, edition);
            NextId++;
            return card;
        }

        public uint GetNextRuntimeId()
        {
            var id = NextId;
            NextId++;
            return id;
        }
        
        public JokerObject CreateJoker(int jokerStaticId, Edition edition = Edition.None)
        {
            var joker = JokerRegistry.CreateInstance(jokerStaticId, NextId, edition);
            NextId++;
            return joker;
        }

        public static JokerObject CreateJoker(ShopItem item)
        {
            var joker = JokerRegistry.CreateInstance(item.StaticId, item.Id, item.Edition);
            return joker;
        }

        public Consumable CreateConsumable(int consumableStaticId)
        {
            var consumable = ConsumableRegistry.CreateInstance(consumableStaticId, NextId);
            NextId++;
            return consumable;
        }

        public static Consumable CreateConsumable(ShopItem item)
        {
            var consumable = ConsumableRegistry.CreateInstance(item.StaticId, item.Id);
            return consumable;
        }
    }
}