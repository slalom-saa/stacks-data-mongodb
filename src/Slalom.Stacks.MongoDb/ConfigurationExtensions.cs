/* 
 * Copyright (c) Stacks Contributors
 * 
 * This file is subject to the terms and conditions defined in
 * the LICENSE file, which is part of this source code package.
 */

using System;
using Autofac;
using Slalom.Stacks.Validation;

namespace Slalom.Stacks.MongoDb
{
    /// <summary>
    /// Contains extension methods to add MongoDB Data blocks.
    /// </summary>
    public static class ConfigurationExtensions
    {
        /// <summary>
        /// Adds the MongoDB block.
        /// </summary>
        /// <param name="instance">The container instance.</param>
        /// <param name="configuration">The configuration routine.</param>
        /// <returns>Returns the container instance for method chaining.</returns>
        public static Stack UseMongoDb(this Stack instance, Action<MongoDbOptions> configuration = null)
        {
            Argument.NotNull(instance, nameof(instance));

            var options = new MongoDbOptions();
            configuration?.Invoke(options);

            instance.Use(builder => { builder.RegisterModule(new MongoDbRepositoriesModule(instance, options)); });

            return instance;
        }
    }
}