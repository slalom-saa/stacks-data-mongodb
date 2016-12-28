using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Slalom.Stacks.Domain;

namespace Slalom.Stacks.Data.MongoDb
{
    /// <summary>
    /// A MongoDB <see cref="IEntityContext"/> implementation.
    /// </summary>
    /// <seealso cref="Slalom.Stacks.Domain.IEntityContext" />
    public class MongoDbEntityContext : IEntityContext
    {
        private readonly MongoDbRepositoriesOptions _options;

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoDbEntityContext"/> class.
        /// </summary>
        /// <param name="options">The options.</param>
        public MongoDbEntityContext(MongoDbRepositoriesOptions options)
        {
            _options = options;
        }

        /// <summary>
        /// Gets the configured <see cref="IConfiguration" />.
        /// </summary>
        /// <value>The configured <see cref="IConfiguration" />.</value>
        protected IConfiguration Configuration { get; set; }

        /// <summary>
        /// Adds the specified instances.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="instances">The instances to update.</param>
        /// <returns>A task for asynchronous programming.</returns>
        public virtual Task AddAsync<TEntity>(TEntity[] instances) where TEntity : IAggregateRoot
        {
            return this.GetCollection<TEntity>().InsertManyAsync(instances);
        }

        /// <summary>
        /// Clears all instances.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <returns>A task for asynchronous programming.</returns>
        public Task ClearAsync<TEntity>() where TEntity : IAggregateRoot
        {
            return this.GetCollection<TEntity>().DeleteManyAsync(e => true);
        }

        /// <summary>
        /// find as an asynchronous operation.
        /// </summary>
        /// <typeparam name="TEntity">The type of the t entity.</typeparam>
        /// <param name="id">The instance identifier.</param>
        /// <returns>A task for asynchronous programming.</returns>
        public virtual async Task<TEntity> FindAsync<TEntity>(string id) where TEntity : IAggregateRoot
        {
            var result = await this.GetCollection<TEntity>().FindAsync(e => e.Id == id);

            return result.FirstOrDefault();
        }

        /// <summary>
        /// find as an asynchronous operation.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="expression">The expression to filter with.</param>
        /// <returns>A task for asynchronous programming.</returns>
        public async Task<IEnumerable<TEntity>> FindAsync<TEntity>(Expression<Func<TEntity, bool>> expression) where TEntity : IAggregateRoot
        {
            var result = await this.GetCollection<TEntity>().FindAsync(expression);

            return result.ToList();
        }

        /// <summary>
        /// Removes the specified instances.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="instances">The instances to remove.</param>
        /// <returns>A task for asynchronous programming.</returns>
        public virtual Task RemoveAsync<TEntity>(TEntity[] instances) where TEntity : IAggregateRoot
        {
            var ids = instances.Select(e => e.Id).ToList();
            return this.GetCollection<TEntity>().DeleteManyAsync(e => ids.Contains(e.Id));
        }

        /// <summary>
        /// Updates the specified instances.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="instances">The instances to update.</param>
        /// <returns>A task for asynchronous programming.</returns>
        public virtual Task UpdateAsync<TEntity>(TEntity[] instances) where TEntity : IAggregateRoot
        {
            return Task.WhenAll(
                instances.ToList().Select(e =>
                {
                    return this.GetCollection<TEntity>().ReplaceOneAsync(x => x.Id == e.Id, e, new UpdateOptions { IsUpsert = true });
                }));
        }

        private IMongoCollection<TEntity> GetCollection<TEntity>()
        {
            return this.GetDatabase().GetCollection<TEntity>(typeof(TEntity).Name);
        }

        private IMongoDatabase GetDatabase()
        {
            var client = !string.IsNullOrWhiteSpace(_options.Connection) ? new MongoClient(_options.Connection)
                             : new MongoClient();

            return client.GetDatabase(_options.Collection ?? "local");
        }
    }
}