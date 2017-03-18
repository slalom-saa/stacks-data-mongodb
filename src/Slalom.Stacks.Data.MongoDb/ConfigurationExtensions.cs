using System;
using Autofac;
using Slalom.Stacks.Configuration;
using Slalom.Stacks.Validation;

namespace Slalom.Stacks.Data.MongoDb
{
    /// <summary>
    /// Contains extension methods to add MongoDB Data blocks.
    /// </summary>
    public static class ConfigurationExtensions
    {
        /// <summary>
        /// Adds the MongoDB Repositories block.
        /// </summary>
        /// <param name="instance">The container instance.</param>
        /// <param name="configuration">The configuration routine.</param>
        /// <returns>Returns the container instance for method chaining.</returns>
        public static Stack UseMongoDbRepositories(this Stack instance, Action<MongoDbRepositoriesOptions> configuration = null)
        {
            Argument.NotNull(instance, nameof(instance));

            var options = new MongoDbRepositoriesOptions();
            configuration?.Invoke(options);

            instance.Use(builder =>
            {
                builder.RegisterModule(new MongoDbRepositoriesModule(options));
            });
            
            return instance;
        }
    }
}
