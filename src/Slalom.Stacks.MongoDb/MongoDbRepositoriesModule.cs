/* 
 * Copyright (c) Stacks Contributors
 * 
 * This file is subject to the terms and conditions defined in
 * the LICENSE file, which is part of this source code package.
 */

using System.Linq;
using System.Reflection;
using Autofac;
using Slalom.Stacks.Reflection;
using Slalom.Stacks.Validation;
using Module = Autofac.Module;

namespace Slalom.Stacks.MongoDb
{
    /// <summary>
    /// An Autofac module for the MongoDB Repositories module.
    /// </summary>
    /// <seealso cref="Autofac.Module" />
    public class MongoDbRepositoriesModule : Module
    {
        private readonly Stack _stack;
        private readonly MongoDbOptions _options;

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoDbRepositoriesModule" /> class.
        /// </summary>
        /// <param name="stack"></param>
        /// <param name="options">The options to use.</param>
        public MongoDbRepositoriesModule(Stack stack, MongoDbOptions options)
        {
            Argument.NotNull(options, nameof(options));

            _stack = stack;
            _options = options;
        }

        /// <summary>
        /// Override to add registrations to the container.
        /// </summary>
        /// <param name="builder">
        /// The builder through which components can be
        /// registered.
        /// </param>
        /// <remarks>Note that the ContainerBuilder parameter is unique to this module.</remarks>
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.Register(c => new MongoMappingsManager(c.Resolve<IDiscoverTypes>()))
                .As<MongoMappingsManager>()
                .SingleInstance()
                .AutoActivate()
                .OnActivated(e => { e.Instance.EnsureInitialized(); });

            builder.Register(c => new MongoDbEntityContext(_options))
                .AsImplementedInterfaces()
                .AsSelf()
                .SingleInstance()
                .OnPreparing(e =>
                {
                    // TODO: Configuration
                });

            builder.Register(c => _options).AsSelf();

            builder.RegisterGeneric(typeof(MongoDbReader<>))
                .WithParameter("options", _options)
                .AsImplementedInterfaces();

            builder.RegisterAssemblyTypes(_stack.Assemblies.ToArray())
                .Where(e => e.GetBaseAndContractTypes().Contains(typeof(MongoDbReader<>)))
                .AsSelf()
                .AsImplementedInterfaces();

            _stack.Assemblies.CollectionChanged += (sender, e) =>
            {
                builder.RegisterAssemblyTypes(e.NewItems.OfType<Assembly>().ToArray())
                    .Where(x => x.GetBaseAndContractTypes().Contains(typeof(MongoDbReader<>)))
                    .AsSelf()
                    .AsImplementedInterfaces();
            };
        }
    }
}