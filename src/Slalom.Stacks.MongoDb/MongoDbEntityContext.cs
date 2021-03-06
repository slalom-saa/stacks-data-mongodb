﻿/* 
 * Copyright (c) Stacks Contributors
 * 
 * This file is subject to the terms and conditions defined in
 * the LICENSE file, which is part of this source code package.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Authentication;
using System.Threading.Tasks;
using MongoDB.Driver;
using Slalom.Stacks.Domain;

namespace Slalom.Stacks.MongoDb
{
    /// <summary>
    /// A MongoDB <see cref="IEntityContext" /> implementation.
    /// </summary>
    /// <seealso cref="Slalom.Stacks.Domain.IEntityContext" />
    public class MongoDbEntityContext : IEntityContext
    {
        private readonly MongoDbOptions _options;

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoDbEntityContext" /> class.
        /// </summary>
        /// <param name="options">The options.</param>
        public MongoDbEntityContext(MongoDbOptions options)
        {
            _options = options;
        }

        /// <summary>
        /// Adds the specified instances.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="instances">The instances to update.</param>
        /// <returns>A task for asynchronous programming.</returns>
        public virtual Task Add<TEntity>(TEntity[] instances) where TEntity : class, IAggregateRoot
        {
            return this.GetCollection<TEntity>().InsertManyAsync(instances);
        }

        /// <summary>
        /// Clears all instances.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <returns>A task for asynchronous programming.</returns>
        public Task Clear<TEntity>() where TEntity : class, IAggregateRoot
        {
            return this.GetCollection<TEntity>().DeleteManyAsync(e => true);
        }

        /// <summary>
        /// find as an asynchronous operation.
        /// </summary>
        /// <typeparam name="TEntity">The type of the t entity.</typeparam>
        /// <param name="id">The instance identifier.</param>
        /// <returns>A task for asynchronous programming.</returns>
        public virtual async Task<TEntity> Find<TEntity>(string id) where TEntity : class, IAggregateRoot
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
        public async Task<IEnumerable<TEntity>> Find<TEntity>(Expression<Func<TEntity, bool>> expression) where TEntity : class, IAggregateRoot
        {
            var result = await this.GetCollection<TEntity>().FindAsync(expression);

            return result.ToList();
        }

        /// <summary>
        /// Finds all instances of the specified type.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <returns>A task for asynchronous programming.</returns>
        public async Task<IEnumerable<TEntity>> Find<TEntity>() where TEntity : class, IAggregateRoot
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
        public virtual Task Remove<TEntity>(TEntity[] instances) where TEntity : class, IAggregateRoot
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
        public virtual Task Update<TEntity>(TEntity[] instances) where TEntity : class, IAggregateRoot
        {
            var requests = new List<ReplaceOneModel<TEntity>>(instances.Count());
            foreach (var entity in instances)
            {
                var filter = new FilterDefinitionBuilder<TEntity>().Where(m => m.Id == entity.Id);
                requests.Add(new ReplaceOneModel<TEntity>(filter, entity));
            }
            return this.GetCollection<TEntity>().BulkWriteAsync(requests);
        }

        public async Task<bool> Exists<TEntity>(Expression<Func<TEntity, bool>> expression) where TEntity : class, IAggregateRoot
        {
            var result = await this.Find(expression);

            return result.Any();
        }

        public async Task<bool> Exists<TEntity>(string id) where TEntity : class, IAggregateRoot
        {
            var result = await this.Find<TEntity>(id);

            return result != null;
        }

        private IMongoCollection<TEntity> GetCollection<TEntity>()
        {
            return this.GetDatabase().GetCollection<TEntity>(typeof(TEntity).Name);
        }

        private IMongoDatabase GetDatabase()
        {
            MongoClient client;
            if (!string.IsNullOrWhiteSpace(_options.ConnectionString))
            {
                var settings = MongoClientSettings.FromUrl(new MongoUrl(_options.ConnectionString));
                settings.SslSettings = new SslSettings
                {
                    EnabledSslProtocols = SslProtocols.Tls12,
                    ServerCertificateValidationCallback = (a, b, c, d) => true
                };

                client = new MongoClient(settings);
            }
            else
            {
                client = new MongoClient();
            }

            return client.GetDatabase(_options.Database ?? "local");
        }
    }
}