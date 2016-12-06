using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Slalom.Stacks.Domain;
using Slalom.Stacks.Reflection;

namespace Slalom.Stacks.Data.MongoDb
{
    public class MongoEntityContext : IEntityContext
    {
        private readonly string _connection;
        private readonly string _collection;

        public MongoEntityContext()
        {
        }

        public MongoEntityContext(string connection) : this()
        {
            _connection = connection;
        }

        protected MongoEntityContext(string connection, string collection) : this()
        {
            _connection = connection;
            _collection = collection;
        }

        protected IConfiguration Configuration { get; set; }

        public MongoMappingsManager Mappings { get; set; }


        private IMongoDatabase GetDatabase(string connection)
        {
            Mappings.EnsureInitialized();

            var client = !string.IsNullOrWhiteSpace(this.Configuration["Mongo:Connection"]) ? new MongoClient(this.Configuration["Mongo:Connection"])
                             : new MongoClient();

            return client.GetDatabase(connection ?? "local");
        }

        private IMongoCollection<TEntity> GetCollection<TEntity>(string collection, string connection = null)
        {
            return this.GetDatabase(connection).GetCollection<TEntity>(collection);
        }

        private IMongoCollection<TEntity> GetCollection<TEntity>()
        {
            return this.GetCollection<TEntity>(_collection ?? typeof(TEntity).Name, _connection);
        }

        public Task ClearAsync<TEntity>() where TEntity : IAggregateRoot
        {
            return this.GetCollection<TEntity>().DeleteManyAsync(e => true);
        }


        public IQueryable<TEntity> CreateQuery<TEntity>() where TEntity : IAggregateRoot
        {
            return this.GetCollection<TEntity>().AsQueryable();
        }

        public virtual Task RemoveAsync<TEntity>(TEntity[] instances) where TEntity : IAggregateRoot
        {
            var ids = instances.Select(e => e.Id).ToList();
            return this.GetCollection<TEntity>().DeleteManyAsync(e => ids.Contains(e.Id));
        }

        public virtual async Task<TEntity> FindAsync<TEntity>(Guid id) where TEntity : IAggregateRoot
        {
            var result = await this.GetCollection<TEntity>().FindAsync(e => e.Id == id);

            return result.FirstOrDefault();
        }

        public virtual Task AddAsync<TEntity>(TEntity[] instances) where TEntity : IAggregateRoot
        {
            return this.GetCollection<TEntity>().InsertManyAsync(instances);
        }

        public virtual Task UpdateAsync<TEntity>(TEntity[] instances) where TEntity : IAggregateRoot
        {
            return Task.WhenAll(
                instances.ToList().Select(e =>
                {
                    return this.GetCollection<TEntity>().ReplaceOneAsync(x => x.Id == e.Id, e, new UpdateOptions { IsUpsert = true });
                }));
        }

    }
}