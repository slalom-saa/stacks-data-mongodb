using System;
using Autofac;
using Slalom.Stacks.Reflection;
using Slalom.Stacks.Validation;

namespace Slalom.Stacks.Data.MongoDb
{
    /// <summary>
    /// An Autofac module for the MongoDB Repositories module.
    /// </summary>
    /// <seealso cref="Autofac.Module" />
    public class MongoDbRepositoriesModule : Module
    {
        private readonly MongoDbRepositoriesOptions _options = new MongoDbRepositoriesOptions();

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoDbRepositoriesModule"/> class.
        /// </summary>
        public MongoDbRepositoriesModule()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoDbRepositoriesModule"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public MongoDbRepositoriesModule(Action<MongoDbRepositoriesOptions> configuration)
        {
            Argument.NotNull(configuration, nameof(configuration));

            configuration(_options);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoDbRepositoriesModule" /> class.
        /// </summary>
        /// <param name="options">The options to use.</param>
        public MongoDbRepositoriesModule(MongoDbRepositoriesOptions options)
        {
            Argument.NotNull(options, nameof(options));

            _options = options;
        }

        /// <summary>
        /// Override to add registrations to the container.
        /// </summary>
        /// <param name="builder">The builder through which components can be
        /// registered.</param>
        /// <remarks>Note that the ContainerBuilder parameter is unique to this module.</remarks>
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.Register(c => new MongoMappingsManager(c.Resolve<IDiscoverTypes>()))
                   .As<MongoMappingsManager>()
                   .SingleInstance()
                   .AutoActivate()
                   .OnActivated(e =>
                   {
                       e.Instance.EnsureInitialized();
                   });

            builder.Register(c => new MongoDbEntityContext(_options))
                   .AsImplementedInterfaces()
                   .AsSelf()
                   .SingleInstance()
                   .OnPreparing(e =>
                   {
                       // TODO: Configuration
                   });
        }
    }
}