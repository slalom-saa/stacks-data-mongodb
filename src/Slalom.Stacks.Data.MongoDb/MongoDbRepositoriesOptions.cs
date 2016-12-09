﻿using System;
using Slalom.Stacks.Validation;

namespace Slalom.Stacks.Data.MongoDb
{
    /// <summary>
    /// Options for the MongoDB Repositories module.
    /// </summary>
    public class MongoDbRepositoriesOptions
    {
        /// <summary>
        /// Gets the connection.
        /// </summary>
        /// <value>The connection.</value>
        internal string Connection { get; private set; } = "";

        /// <summary>
        /// Gets the collection.
        /// </summary>
        /// <value>The collection.</value>
        internal string Collection { get; private set; }

        /// <summary>
        /// Sets the connection to use.
        /// </summary>
        /// <param name="connection">The connection to use.</param>
        /// <returns>Returns this instance for chaining.</returns>
        public MongoDbRepositoriesOptions WithConnection(string connection)
        {
            Argument.NotNullOrWhiteSpace(() => connection);

            this.Connection = connection;

            return this;
        }

        /// <summary>
        /// Sets the collection to use.
        /// </summary>
        /// <param name="collection">The collection to use.</param>
        /// <returns>Returns this instance for chaining.</returns>
        public MongoDbRepositoriesOptions WithCollection(string collection)
        {
            Argument.NotNullOrWhiteSpace(() => collection);

            this.Collection = collection;

            return this;
        }
    }
}