using System;
using Slalom.Stacks.Validation;

namespace Slalom.Stacks.Data.MongoDb
{
    /// <summary>
    /// Options for the MongoDB module.
    /// </summary>
    public class MongoDbOptions
    {
        /// <summary>
        /// Gets the connection.
        /// </summary>
        /// <value>The connection.</value>
        internal string Connection { get; private set; } = "";

        /// <summary>
        /// Gets the database.
        /// </summary>
        /// <value>The database.</value>
        internal string Database { get; private set; }

        /// <summary>
        /// Sets the connection to use.
        /// </summary>
        /// <param name="connection">The connection to use.</param>
        /// <returns>Returns this instance for chaining.</returns>
        public MongoDbOptions WithConnection(string connection)
        {
            Argument.NotNull(connection, nameof(connection));

            this.Connection = connection;

            return this;
        }

        /// <summary>
        /// Sets the database to use.
        /// </summary>
        /// <param name="database">The database to use.</param>
        /// <returns>Returns this instance for chaining.</returns>
        public MongoDbOptions WithDatabase(string database)
        {
            Argument.NotNull(database, nameof(database));

            this.Database = database;

            return this;
        }
    }
}