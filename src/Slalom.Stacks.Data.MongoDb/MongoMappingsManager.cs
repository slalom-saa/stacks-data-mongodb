using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using MongoDB.Bson.Serialization;
using Slalom.Stacks.Communication;
using Slalom.Stacks.Domain;
using Slalom.Stacks.Reflection;
using Slalom.Stacks.Serialization;
using Slalom.Stacks.Validation;

namespace Slalom.Stacks.Data.MongoDb
{
    /// <summary>
    /// Builds and maintains mappings for the MongoDB Data module.
    /// </summary>
    internal class MongoMappingsManager
    {
        private readonly IDiscoverTypes _types;
        private bool _initialized;

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoMappingsManager"/> class.
        /// </summary>
        /// <param name="types">The types.</param>
        public MongoMappingsManager(IDiscoverTypes types)
        {
            Argument.NotNull(() => types);

            _types = types;
        }

        /// <summary>
        /// Ensures that the maps are initialized.
        /// </summary>
        public void EnsureInitialized()
        {
            if (!_initialized)
            {
                _initialized = true;

                this.CreateKnownMaps();

                this.CreateDynamicMaps();
            }
        }

        private void CreateDynamicMaps()
        {
            _types.Find<Event>().ToList().ForEach(BuildMap);

            _types.Find<Entity>().ToList().ForEach(BuildMap);
        }

        private void CreateKnownMaps()
        {
            if (!BsonClassMap.IsClassMapRegistered(typeof(Entity)))
            {
                BsonClassMap.RegisterClassMap<Entity>(x =>
                {
                    x.AutoMap();
                    x.SetIsRootClass(true);
                    x.MapIdField(e => e.Id);
                });
            }
            if (!BsonClassMap.IsClassMapRegistered(typeof(Event)))
            {
                BsonClassMap.RegisterClassMap<Event>(x =>
                {
                    x.AutoMap();
                    x.SetIsRootClass(true);
                    x.MapProperty(e => e.EventName);
                    x.MapProperty(e => e.TimeStamp);
                });
            }
        }

        /// <summary>
        /// Dynamically builds a map for the specified type.
        /// </summary>
        /// <param name="target">The target.</param>
        public static void BuildMap(Type target)
        {
            // First check that the type is not already registered to prevent errors.
            if (!BsonClassMap.IsClassMapRegistered(target))
            {
                var map = new BsonClassMap(target);

                var properties = target.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                var propertyNames = properties.Select(x => x.Name);

                MapStandardProperties(target, properties, map);

                MapEnumerableProperties(target, properties, map);

                MapConstructors(target, propertyNames, map);

                BsonClassMap.RegisterClassMap(map);
            }
        }

        private static void MapConstructors(Type target, IEnumerable<string> propertyNames, BsonClassMap map)
        {
            var constructors = target.GetConstructors();
            foreach (var constructorInfo in constructors)
            {
                var parameters = constructorInfo.GetParameters();

                if (parameters.All(e => propertyNames.Contains(char.ToUpper(e.Name[0]) + e.Name.Remove(0, 1))))
                {
                    var parameterExpressions = parameters.Select(e => Expression.Parameter(e.ParameterType, e.Name)).ToList();

                    var expression = Expression.Lambda(Expression.New(constructorInfo, parameterExpressions),
                        parameterExpressions);

                    map.MapCreator(expression.Compile(), parameters.Select(e => char.ToUpper(e.Name[0]) + e.Name.Remove(0, 1)).ToArray());
                }
            }
        }

        private static void MapEnumerableProperties(Type target, PropertyInfo[] properties, BsonClassMap map)
        {
            var fields = target.GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var info in fields.Where(e => e.DeclaringType == target))
            {
                if (info.FieldType.GetTypeInfo().IsGenericType && info.FieldType.GetGenericTypeDefinition() == typeof(List<>))
                {
                    var name = char.ToUpper(info.Name[1]) + info.Name.Substring(2);
                    if (properties.Any(e => e.Name == name))
                    {
                        map.UnmapProperty(name);
                        map.MapField(info.Name).SetElementName(name);
                    }
                }
            }
        }

        private static void MapStandardProperties(Type target, PropertyInfo[] properties, BsonClassMap map)
        {
            foreach (var info in properties.Where(e => e.DeclaringType == target && !e.GetCustomAttributes<SecureAttribute>().Any()))
            {
                map.MapProperty(info.Name);
            }
        }
    }
}