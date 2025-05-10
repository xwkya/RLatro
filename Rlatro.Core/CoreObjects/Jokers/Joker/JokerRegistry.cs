using System.Reflection;
using Balatro.Core.CoreObjects.CoreEnums;

namespace Balatro.Core.CoreObjects.Jokers.Joker
{
    public static class JokerRegistry
    {
        private static readonly Dictionary<JokerRarity, List<Type>> JokersByRarity = new();
        private static readonly Dictionary<Type, JokerStaticDescriptionAttribute> JokerMetadata = new();

        // Precompiled constructors for better performance than Activator.CreateInstance every time
        private static readonly Dictionary<Type, Func<uint, Edition, JokerObject>> JokerConstructors = new();

        static JokerRegistry()
        {
            Initialize();
        }

        private static void Initialize()
        {
            // Scan the assembly Joker classes are defined
            var jokerTypes = Assembly.GetAssembly(typeof(JokerObject))
                .GetTypes()
                .Where(t => t.IsSubclassOf(typeof(JokerObject)) && !t.IsAbstract &&
                            t.GetCustomAttribute<JokerStaticDescriptionAttribute>() != null);

            foreach (var type in jokerTypes)
            {
                var attr = type.GetCustomAttribute<JokerStaticDescriptionAttribute>();
                if (attr == null) continue; // Safety check

                if (!JokersByRarity.ContainsKey(attr.Rarity))
                {
                    JokersByRarity[attr.Rarity] = new List<Type>();
                }

                JokersByRarity[attr.Rarity].Add(type);
                JokerMetadata[type] = attr;

                // Pre-compile constructor
                var constructorInfo = type.GetConstructor(new[] { typeof(uint), typeof(Edition) });
                if (constructorInfo != null)
                {
                    // Using expression trees for optimized delegate creation
                    // /!\ Any modification to the constructor signature will break this.
                    var idParam = System.Linq.Expressions.Expression.Parameter(typeof(uint), "id");
                    var editionParam = System.Linq.Expressions.Expression.Parameter(typeof(Edition), "edition");
                    var newExpr = System.Linq.Expressions.Expression.New(constructorInfo, idParam, editionParam);
                    var lambda =
                        System.Linq.Expressions.Expression.Lambda<Func<uint, Edition, JokerObject>>(newExpr, idParam,
                            editionParam);
                    JokerConstructors[type] = lambda.Compile();
                }
                else
                {
                    Console.WriteLine(
                        $"Warning: Joker type {type.Name} does not have the expected constructor (uint, Edition).");
                }
            }
        }

        public static JokerObject CreateJoker(Type jokerType, uint id, Edition edition = Edition.None)
        {
            if (JokerConstructors.TryGetValue(jokerType, out var constructor))
            {
                return constructor(id, edition);
            }

            // Fallback if delegate wasn't created
            throw new ArgumentException(
                $"Cannot create instance of {jokerType.Name}. Constructor delegate not found or type not registered correctly.");
        }

        public static JokerStaticDescriptionAttribute GetJokerMetadata(Type jokerType)
        {
            JokerMetadata.TryGetValue(jokerType, out var metadata);
            return metadata;
        }

        public static JokerStaticDescriptionAttribute GetJokerMetadata(JokerObject jokerInstance)
        {
            if (jokerInstance == null) return null;
            return GetJokerMetadata(jokerInstance.GetType());
        }

        public static List<Type> GetJokersOfType(JokerRarity rarity)
        {
            if (JokersByRarity.TryGetValue(rarity, out var jokerList))
            {
                return new List<Type>(jokerList); // Return a copy
            }

            return new List<Type>();
        }
    }
}