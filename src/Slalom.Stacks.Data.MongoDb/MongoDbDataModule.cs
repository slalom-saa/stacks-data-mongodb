using System;
using Autofac;
using Slalom.Stacks.Reflection;
using Slalom.Stacks.Validation;

namespace Slalom.Stacks.Data.MongoDb
{
    /// <summary>
    /// An Autofac module for the MongoDB Data module.
    /// </summary>
    /// <seealso cref="Autofac.Module" />
    public class MongoDbDataModule : Module
    {
        private readonly MongoDbDataOptions _options = new MongoDbDataOptions();

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoDbDataModule"/> class.
        /// </summary>
        public MongoDbDataModule()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoDbDataModule"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public MongoDbDataModule(Action<MongoDbDataOptions> configuration)
        {
            Argument.NotNull(() => configuration);

            configuration(_options);
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
                   .SingleInstance();
        }
    }
}