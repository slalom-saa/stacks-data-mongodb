using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Slalom.Stacks.Search;

namespace Slalom.Stacks.Data.MongoDb
{
    public class MongoDbReader<TItem> : IEntityReader<TItem>
    {
        private readonly MongoDbOptions _options;
        private string _collection;

        public MongoDbReader(MongoDbOptions options)
        {
            _options = options;

            if (!BsonClassMap.IsClassMapRegistered(typeof(TItem)))
            {
                BsonClassMap.RegisterClassMap<TItem>(e =>
                {
                    e.AutoMap();
                    e.SetIgnoreExtraElements(true);
                });
            }
        }

        public MongoDbReader(MongoDbOptions options, string collection) : this(options)
        {
            _collection = collection;
        }

        public IQueryable<TItem> Read()
        {
            return this.GetCollection<TItem>().AsQueryable();
        }

        private IMongoCollection<TEntity> GetCollection<TEntity>()
        {
            return this.GetDatabase().GetCollection<TEntity>(_collection ?? Inflector.Inflector.Pluralize(typeof(TEntity).Name));
        }

        private IMongoDatabase GetDatabase()
        {
            var client = !string.IsNullOrWhiteSpace(_options.Connection) ? new MongoClient(_options.Connection)
                : new MongoClient();

            return client.GetDatabase(_options.Database ?? "local");
        }
    }
}
