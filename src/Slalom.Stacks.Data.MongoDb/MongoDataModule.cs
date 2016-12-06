using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Slalom.Stacks.Reflection;

namespace Slalom.Stacks.Data.MongoDb
{
    public class MongoDataModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.Register(c => new MongoMappingsManager(c.Resolve<IDiscoverTypes>()))
                  .As<MongoMappingsManager>()
                  .SingleInstance();

            builder.Register(c => new MongoEntityContext())
                   .SingleInstance();
        }
    }
}
