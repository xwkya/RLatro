using System.Reflection;
using Balatro.Core.CoreObjects.Consumables.ConsumableObject;
using Balatro.Core.CoreObjects.CoreEnums;

namespace Balatro.Core.CoreObjects.Registries
{
    public static class ConsumableRegistry
    {
        // ConsumableType -> Ordered List of StaticIds for that type
        private static readonly Dictionary<ConsumableType, IReadOnlyList<int>> MasterOrderedStaticIdsByType = new();

        private static readonly Dictionary<int, ConsumableStaticDescriptionAttribute> AttributesByStaticId = new();

        // Constructor: takes staticId, runtimeId, isNegative
        private static readonly Dictionary<int, Func<int, uint, bool, Consumable>> ConstructorsByStaticId = new();

        private static readonly Dictionary<Type, int> TypeToStaticId = new Dictionary<Type, int>();
        private static readonly Dictionary<int, Type> StaticIdToType = new Dictionary<int, Type>();
        
        private static readonly Dictionary<HandRank, int> HandRankToStaticId = new Dictionary<HandRank, int>();

        static ConsumableRegistry()
        {
            var tempStaticIdsByType = new Dictionary<ConsumableType, List<int>>();

            var types = Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => t.IsSubclassOf(typeof(Consumable)) && !t.IsAbstract);

            foreach (var type in types)
            {
                var attr = type.GetCustomAttribute<ConsumableStaticDescriptionAttribute>();
                if (attr == null)
                {
                    throw new ArgumentException(
                        $"Type {type.Name} does not have a {nameof(ConsumableStaticDescriptionAttribute)}.");
                }

                if (AttributesByStaticId.ContainsKey(attr.StaticId))
                {
                    var existingType = StaticIdToType[attr.StaticId];
                    throw new ArgumentException(
                        $"Duplicate StaticId {attr.StaticId} found for consumables. Types: {existingType.Name} and {type.Name}. StaticIds must be unique across all consumables.");
                }

                AttributesByStaticId[attr.StaticId] = attr;
                TypeToStaticId[type] = attr.StaticId;
                StaticIdToType[attr.StaticId] = type;

                if (attr.UpgradedRank.HasValue)
                {
                    HandRankToStaticId[attr.UpgradedRank.Value] = attr.StaticId;
                }

                if (!tempStaticIdsByType.TryGetValue(attr.Type, out var list))
                {
                    list = new List<int>();
                    tempStaticIdsByType[attr.Type] = list;
                }

                list.Add(attr.StaticId);

                var ctorInfo = type.GetConstructor(new[] { typeof(int), typeof(uint), typeof(bool) });
                if (ctorInfo != null)
                {
                    var staticIdParam = System.Linq.Expressions.Expression.Parameter(typeof(int), "staticId");
                    var runtimeIdParam = System.Linq.Expressions.Expression.Parameter(typeof(uint), "runtimeId");
                    var isNegativeParam = System.Linq.Expressions.Expression.Parameter(typeof(bool), "isNegative");

                    var newExpr =
                        System.Linq.Expressions.Expression.New(ctorInfo, staticIdParam, runtimeIdParam,
                            isNegativeParam);
                    var lambda = System.Linq.Expressions.Expression.Lambda<Func<int, uint, bool, Consumable>>(
                        newExpr, staticIdParam, runtimeIdParam, isNegativeParam);
                    ConstructorsByStaticId[attr.StaticId] = lambda.Compile();
                }
                else
                {
                    throw new ArgumentException(
                        $"Consumable {type.Name} (StaticId: {attr.StaticId}) does not have a valid constructor with parameters (int staticId, uint runtimeId, bool isNegative).");
                }
            }

            foreach (var cType in tempStaticIdsByType.Keys)
            {
                var sortedList = tempStaticIdsByType[cType];
                sortedList.Sort(); // Sort by StaticId
                MasterOrderedStaticIdsByType[cType] = sortedList.AsReadOnly();
            }
        }

        public static IReadOnlyList<int> GetMasterOrderedStaticIds(ConsumableType type) =>
            MasterOrderedStaticIdsByType.TryGetValue(type, out var list) ? list : Array.Empty<int>();

        public static ConsumableStaticDescriptionAttribute GetAttribute(int staticId) =>
            AttributesByStaticId.TryGetValue(staticId, out var attr)
                ? attr
                : throw new KeyNotFoundException($"No ConsumableAttribute found for StaticId {staticId}.");

        public static Type GetType(int staticId) =>
            StaticIdToType.TryGetValue(staticId, out var type)
                ? type
                : throw new KeyNotFoundException($"No Consumable Type found for StaticId {staticId}.");

        public static int GetStaticId(Type type) =>
            TypeToStaticId.TryGetValue(type, out var id)
                ? id
                : throw new KeyNotFoundException(
                    $"No StaticId found for Consumable Type {type.Name}. Is it registered with an attribute?");
        
        public static int GetHandRankPlanetStaticId(HandRank rank) =>
            HandRankToStaticId.TryGetValue(rank, out var staticId)
                ? staticId
                : throw new KeyNotFoundException($"No StaticId found for HandRank {rank}.");

        public static Consumable CreateInstance(int staticId, uint runtimeId, bool isNegative = false)
        {
            if (ConstructorsByStaticId.TryGetValue(staticId, out var constructor))
                return constructor(staticId, runtimeId, isNegative);
            throw new ArgumentException($"No Consumable constructor for StaticId {staticId}.");
        }
    }
}