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
        public virtual Task Add<TEntity>(TEntity[] instances) where TEntity : IAggregateRoot
        {
            return this.GetCollection<TEntity>().InsertManyAsync(instances);
        }

        /// <summary>
        /// Clears all instances.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <returns>A task for asynchronous programming.</returns>
        public Task Clear<TEntity>() where TEntity : IAggregateRoot
        {
            return this.GetCollection<TEntity>().DeleteManyAsync(e => true);
        }

        /// <summary>
        /// find as an asynchronous operation.
        /// </summary>
        /// <typeparam name="TEntity">The type of the t entity.</typeparam>
        /// <param name="id">The instance identifier.</param>
        /// <returns>A task for asynchronous programming.</returns>
        public virtual async Task<TEntity> Find<TEntity>(string id) where TEntity : IAggregateRoot
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
        public async Task<IEnumerable<TEntity>> Find<TEntity>(Expression<Func<TEntity, bool>> expression) where TEntity : IAggregateRoot
        {
            var result = await this.GetCollection<TEntity>().FindAsync(expression);

            return result.ToList();
        }

        /// <summary>
        /// Finds all instances of the specified type.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <returns>A task for asynchronous programming.</returns>
        public async Task<IEnumerable<TEntity>> Find<TEntity>() where TEntity : IAggregateRoot
        {
            var result = await this.GetCollection<TEntity>().AsQueryable().ToListAsync();

            return result;
        }

        /// <summary>
        /// Removes the specified instances.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="instances">The instances to remove.</param>
        /// <returns>A task for asynchronous programming.</returns>
        public virtual Task Remove<TEntity>(TEntity[] instances) where TEntity : IAggregateRoot
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
        public virtual Task Update<TEntity>(TEntity[] instances) where TEntity : IAggregateRoot
        {
            var requests = new List<ReplaceOneModel<TEntity>>(instances.Count());
            foreach (var entity in instances)
            {
                var filter = new FilterDefinitionBuilder<TEntity>().Where(m => m.Id == entity.Id);
                requests.Add(new ReplaceOneModel<TEntity>(filter, entity));
            }
            return this.GetCollection<TEntity>().BulkWriteAsync(requests);
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

        public async Task<bool> Exists<TEntity>(Expression<Func<TEntity, bool>> expression) where TEntity : IAggregateRoot
        {
            var result = await this.Find<TEntity>(expression);

            return result.Any();
        }

        public async Task<bool> Exists<TEntity>(string id) where TEntity : IAggregateRoot
        {
            var result = await this.Find<TEntity>(id);

            return result != null;
        }
    }
}