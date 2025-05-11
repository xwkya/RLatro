using System.Reflection;
using Balatro.Core.CoreObjects.CoreEnums;
using Balatro.Core.CoreObjects.Jokers.Joker;

namespace Balatro.Core.CoreObjects.Registries
{
        public static class JokerRegistry
    {
        private static readonly Dictionary<JokerRarity, IReadOnlyList<int>> MasterOrderedStaticIdsByRarity = new();
        private static readonly Dictionary<int, JokerStaticDescriptionAttribute> AttributesByStaticId = new();

        // Compiled constructors for JokerObject: Func<staticId, runtimeId, edition, JokerObject>
        private static readonly Dictionary<int, Func<int, uint, Edition, JokerObject>> ConstructorsByStaticId = new();

        // For reverse lookup
        private static readonly Dictionary<Type, int> TypeToStaticId = new(); // No 'new Dictionary' needed, initialized below
        private static readonly Dictionary<int, Type> StaticIdToType = new(); // No 'new Dictionary' needed, initialized below

        static JokerRegistry()
        {
            var tempStaticIdsByRarity = new Dictionary<JokerRarity, List<int>>();
            // Initialize TypeToStaticId and StaticIdToType here if they are not assigned elsewhere before use.
            // However, they are populated within the loop below, which is fine.

            var types = Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => t.IsSubclassOf(typeof(JokerObject)) && !t.IsAbstract);

            foreach (var type in types)
            {
                var attr = type.GetCustomAttribute<JokerStaticDescriptionAttribute>();
                if (attr == null)
                {
                    throw new ArgumentException($"Type {type.Name} does not have a {nameof(JokerStaticDescriptionAttribute)}.");
                }

                if (AttributesByStaticId.ContainsKey(attr.StaticId))
                {
                    var existingType = StaticIdToType.TryGetValue(attr.StaticId, out var et) ? et.Name : "Unknown";
                    throw new ArgumentException(
                        $"Duplicate StaticId {attr.StaticId} found. Existing: {existingType}, New: {type.Name}. StaticIds must be unique.");
                }
                
                AttributesByStaticId[attr.StaticId] = attr;
                TypeToStaticId[type] = attr.StaticId; // Populate here
                StaticIdToType[attr.StaticId] = type;   // Populate here


                if (!tempStaticIdsByRarity.TryGetValue(attr.Rarity, out var list))
                {
                    list = new List<int>();
                    tempStaticIdsByRarity[attr.Rarity] = list;
                }
                list.Add(attr.StaticId);

                // Constructor now takes (int staticId, uint runtimeId, Edition edition)
                var ctorInfo = type.GetConstructor(new[] { typeof(int), typeof(uint), typeof(Edition) });
                if (ctorInfo != null)
                {
                    // Define parameters for the lambda expression
                    var staticIdParam = System.Linq.Expressions.Expression.Parameter(typeof(int), "staticIdArg");
                    var runtimeIdParam = System.Linq.Expressions.Expression.Parameter(typeof(uint), "runtimeIdArg");
                    var editionParam = System.Linq.Expressions.Expression.Parameter(typeof(Edition), "editionArg");
                    
                    // Create the 'new' expression, passing the lambda parameters to the constructor
                    var newExpr = System.Linq.Expressions.Expression.New(ctorInfo, staticIdParam, runtimeIdParam, editionParam);
                    
                    // Compile the lambda
                    var lambda = System.Linq.Expressions.Expression.Lambda<Func<int, uint, Edition, JokerObject>>(
                        newExpr, staticIdParam, runtimeIdParam, editionParam);
                    
                    ConstructorsByStaticId[attr.StaticId] = lambda.Compile();
                }
                else
                {
                    // This error should guide the user to fix their Joker class constructors
                    throw new ArgumentException(
                        $"Joker {type.Name} (StaticId: {attr.StaticId}) does not have a public constructor matching signature (int staticId, uint runtimeId, Edition edition).");
                }
            }

            foreach (var rarity in tempStaticIdsByRarity.Keys)
            {
                var sortedList = tempStaticIdsByRarity[rarity];
                sortedList.Sort(); // Sort by StaticId (which also serves as Order here)
                MasterOrderedStaticIdsByRarity[rarity] = sortedList.AsReadOnly();
            }
        }

        public static IReadOnlyList<int> GetMasterOrderedStaticIds(JokerRarity rarity) => 
            MasterOrderedStaticIdsByRarity.TryGetValue(rarity, out var list) ? list : Array.Empty<int>();

        public static JokerStaticDescriptionAttribute GetAttribute(int staticId) =>
            AttributesByStaticId.TryGetValue(staticId, out var attr) ? attr : 
            throw new KeyNotFoundException($"No JokerAttribute found for StaticId {staticId}.");
        
        public static Type GetType(int staticId) =>
            StaticIdToType.TryGetValue(staticId, out var type) ? type : 
            throw new KeyNotFoundException($"No Joker Type found for StaticId {staticId}.");

        public static int GetStaticId(Type type) =>
             TypeToStaticId.TryGetValue(type, out var id) ? id : 
             throw new KeyNotFoundException($"No StaticId found for Joker Type {type.Name}. Is it registered with a JokerStaticDescriptionAttribute?");

        public static JokerObject CreateInstance(int staticId, uint runtimeId, Edition ed = Edition.None)
        {
            if (ConstructorsByStaticId.TryGetValue(staticId, out var constructor))
            {
                // Call the compiled constructor, passing the staticId along with runtimeId and edition
                return constructor(staticId, runtimeId, ed);
            }
            
            throw new ArgumentException($"No Joker constructor found for StaticId {staticId}.");
        }

        public static JokerObject CreateInstance<T>(uint runtimeId, Edition ed = Edition.None)
            where T : JokerObject
        {
            var staticId = GetStaticId(typeof(T));
            if (ConstructorsByStaticId.TryGetValue(staticId, out var constructor))
            {
                // Call the compiled constructor, passing the staticId along with runtimeId and edition
                return constructor(staticId, runtimeId, ed);
            }
            
            throw new ArgumentException($"No Joker constructor found for StaticId {staticId}.");
        }
    }
}