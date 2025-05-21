using Balatro.Core.CoreObjects.Consumables.ConsumableObject;
using Balatro.Core.CoreObjects.Registries;
using Balatro.Core.GameEngine.GameStateController;
using Balatro.Core.GameEngine.GameStateController.EventBus;
using Balatro.Core.GameEngine.PseudoRng;

namespace Balatro.Core.CoreObjects.Pools
{
    public class ConsumablePoolManager : IEventBusSubscriber
    {
        private readonly GameContext GameContext;

        /// <summary>
        /// Tracks the number of instances of each consumable StaticId currently "in context" (e.g., in shop or player inventory).
        /// ConsumableType -> { StaticId -> CountInContext }
        /// </summary>
        private readonly Dictionary<ConsumableType, Dictionary<int, int>> NumberOfConsumablesInContextByType = new();

        // Default static ids for each consumable type.
        // TODO: These should be set via the registry once they have been implemented
        private static readonly int DefaultTarotStaticId = 1;
        private static readonly int DefaultPlanetStaticId = 1;
        private static readonly int DefaultSpectralStaticId = 1;

        public ConsumablePoolManager(GameContext context)
        {
            GameContext = context;
        }

        public void InitializePool()
        {
            foreach (ConsumableType cType in Enum.GetValues(typeof(ConsumableType)))
            {
                NumberOfConsumablesInContextByType[cType] = new Dictionary<int, int>();
                foreach (var staticId in ConsumableRegistry.GetMasterOrderedStaticIds(cType))
                {
                    NumberOfConsumablesInContextByType[cType][staticId] = 0; // Initialize with zero owned/in context
                }
            }

            // Update for any consumables already in the player's inventory at game start
            foreach (var consumable in GameContext.ConsumableContainer.Consumables) // Assuming ConsumableContainer
            {
                int staticId = consumable.StaticId;
                var attribute = ConsumableRegistry.GetAttribute(staticId);

                NumberOfConsumablesInContextByType[attribute.Type][staticId]++;
            }
        }

        public void Subscribe(GameEventBus eventBus)
        {
            throw new NotImplementedException("Subscribe to relevant consumable events.");
        }

        // Event handler methods
        public void ConsumableRemovedFromContext(int staticId) // e.g. sold from shop, used by player
        {
            var attribute = ConsumableRegistry.GetAttribute(staticId);
            if (NumberOfConsumablesInContextByType.TryGetValue(attribute.Type, out var counts) &&
                counts.ContainsKey(staticId))
            {
                if (counts[staticId] > 0) counts[staticId]--;
            }
        }

        public void ConsumableAddedToContext(int staticId) // e.g. added to shop, obtained by player
        {
            var attribute = ConsumableRegistry.GetAttribute(staticId);
            if (NumberOfConsumablesInContextByType.TryGetValue(attribute.Type, out var counts))
            {
                if (!counts.ContainsKey(staticId)) counts[staticId] = 0; // Should be initialized, but safety
                counts[staticId]++;
            }
        }

        public int GetRandomStaticId(ConsumableType type, RngController rng, RngActionType actionType)
        {
            var masterStaticIdList = ConsumableRegistry.GetMasterOrderedStaticIds(type);
            
            if (GameContext.JokerContainer.Showman())
            {
                var index = rng.RandomInt(0, masterStaticIdList.Count - 1, actionType);
                return masterStaticIdList[index];
            }

            Span<int> candidatesSpan = stackalloc int[masterStaticIdList.Count];
            int count = 0;

            foreach (int staticId in masterStaticIdList)
            {
                bool isUnavailable = NumberOfConsumablesInContextByType[type][staticId] > 0;
                if (isUnavailable)
                {
                    continue;
                }

                var attribute = ConsumableRegistry.GetAttribute(staticId);
                if (attribute.SoftLockRank.HasValue)
                {
                    if (GameContext.PersistentState.HandTracker.GetHandPlayedCount(attribute.SoftLockRank.Value) == 0)
                    {
                        continue; // Skip if the soft lock condition is not met
                    }
                }
                
                candidatesSpan[count++] = staticId;
            }

            if (count == 0) return GetDefaultStaticIdForType(type);

            int randomIndex =
                rng.RandomInt(0, count - 1, actionType);
            return candidatesSpan[randomIndex];
        }

        private int GetDefaultStaticIdForType(ConsumableType type)
        {
            return type switch
            {
                ConsumableType.Planet => DefaultPlanetStaticId,
                ConsumableType.Tarot => DefaultTarotStaticId,
                ConsumableType.Spectral => DefaultSpectralStaticId,
                _ => DefaultTarotStaticId, // Fallback to Tarot if type is unknown
            };
        }
    }
}