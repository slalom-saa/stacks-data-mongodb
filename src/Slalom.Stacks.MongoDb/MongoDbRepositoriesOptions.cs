/* 
 * Copyright (c) Stacks Contributors
 * 
 * This file is subject to the terms and conditions defined in
 * the LICENSE file, which is part of this source code package.
 */

using Slalom.Stacks.Validation;

namespace Slalom.Stacks.MongoDb
{
    /// <summary>
    /// Options for the MongoDB module.
    /// </summary>
    public class MongoDbOptions
    {
        /// <summary>
        /// Gets or sets the connection string.
        /// </summary>
        /// <value>The connection string.</value>
        public string ConnectionString { get; set; } = "";

        /// <summary>
        /// Gets or sets the database name.
        /// </summary>
        /// <value>The database name.</value>
        public string Database { get; set; } = "Stacks";

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