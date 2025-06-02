using Balatro.Core.CoreObjects.Jokers.Joker;
using Balatro.Core.CoreObjects.Registries;
using Balatro.Core.GameEngine.GameStateController;
using Balatro.Core.GameEngine.GameStateController.EventBus;
using Balatro.Core.GameEngine.PseudoRng;

namespace Balatro.Core.CoreObjects.Pools
{
    public class JokerPoolManager : IEventBusSubscriber
    {
        private readonly GameContext GameContext;

        /// <summary>
        /// Dictionary to keep track of the number of jokers owned by rarity.
        /// Rarity -> {StaticId -> Number of instance owned}
        /// </summary>
        private readonly Dictionary<JokerRarity, Dictionary<int, int>> NumberOfJokersInContextByRarity = new();

        private static readonly int DefaultCommonJokerStaticId = 1; // Example, must match an actual Joker's StaticId

        public JokerPoolManager(GameContext context)
        {
            GameContext = context;
        }

        public void InitializePool()
        {
            // Initialize the dictionary for each rarity.
            foreach (JokerRarity rarity in Enum.GetValues(typeof(JokerRarity)))
            {
                NumberOfJokersInContextByRarity[rarity] = new Dictionary<int, int>();
                foreach (var joker in JokerRegistry.GetMasterOrderedStaticIds(rarity))
                {
                    NumberOfJokersInContextByRarity[rarity][joker] = 0; // Initialize with zero owned
                }
            }
            
            // Update the pool for each joker owned
            foreach (var joker in GameContext.JokerContainer.Jokers)
            {
                var staticId = joker.StaticId;
                var attribute = JokerRegistry.GetAttribute(staticId);
                
                NumberOfJokersInContextByRarity[attribute.Rarity][staticId] += 1;
            }
        }
        
        public void Subscribe(GameEventBus eventBus)
        {
            eventBus.SubscribeToJokerAddedToContext(JokerAddedToContext);
            eventBus.SubscribeToJokerRemovedFromContext(JokerRemovedFromContext);
        }

        private void JokerRemovedFromContext(int staticId)
        {
            var rarity = JokerRegistry.GetAttribute(staticId).Rarity;
            NumberOfJokersInContextByRarity[rarity][staticId] -= 1;
        }

        private void JokerAddedToContext(int staticId)
        {
            var rarity = JokerRegistry.GetAttribute(staticId).Rarity;
            NumberOfJokersInContextByRarity[rarity][staticId] += 1;
        }

        public int GetRandomStaticId(JokerRarity rarity, RngController rng, RngActionType actionType)
        {
            var masterStaticIdList = JokerRegistry.GetMasterOrderedStaticIds(rarity);
            if (GameContext.JokerContainer.Showman())
            {
                var index = rng.RandomInt(0, masterStaticIdList.Count - 1, actionType);
                return masterStaticIdList[index];
            }

            Span<int> candidatesSpan = stackalloc int[masterStaticIdList.Count];
            int count = 0;
            
            foreach (int staticId in masterStaticIdList)
            {
                bool isUnavailable = NumberOfJokersInContextByRarity[rarity][staticId] > 0;
                if (!isUnavailable)
                {
                    candidatesSpan[count++] = staticId;
                }
            }

            if (count == 0) return DefaultCommonJokerStaticId;

            int randomIndex = rng.RandomInt(0, count - 1, actionType);
            return candidatesSpan[randomIndex];
        }
    }
}