#region Version Info Header
/*
 * $Id$
 * $HeadURL$
 * Last modified by $Author$
 * Last modified at $Date$
 * $Revision$
 */
#endregion

using JetBrains.Annotations;

namespace TorSteroids.Storage.Providers
{
    public interface IStorageInitializer
    {
        /// <summary>
        /// Builds the connection string to be used with the <see cref="IStorageProvider"/> instance.
        /// </summary>
        /// <param name="databasePath">The database path. Optional</param>
        /// <param name="databaseName">Name of the database. Required</param>
        /// <param name="secret">The secret (password).</param>
        /// <returns></returns>
        string BuildConnectionString(string databasePath, [NotNull] string databaseName, string secret = null);

        /// <summary>
        /// Gets the name of the database file out of a connection string, if supported.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <returns>Null, if not found or not supported</returns>
        string GetDatabaseFileName([NotNull] string connectionString);

        /// <summary>
        /// Creates the storage (database).
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="overwriteExistingDatabaseFile">if set to <c>true</c> an existing database file gets overwritten.</param>
        void CreateDatabase([NotNull] string connectionString, bool overwriteExistingDatabaseFile);

        /// <summary>
        /// Creates the storage schema (database schema).
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="createSchemaScript">The create schema script.</param>
        void CreateSchema([NotNull] string connectionString, string createSchemaScript);
        
        /// <summary>
        /// Updates an existing schema to a new one.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="updateSchemaScript">The update schema script.</param>
        void UpdateSchema([NotNull] string connectionString, string updateSchemaScript);

        /// <summary>
        /// Creates the <see cref="IStorageProvider"/> instance.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <returns>Concrete <see cref="IStorageProvider"/> implementation</returns>
        IStorageProvider CreateInstance([NotNull] string connectionString);
    }
}
