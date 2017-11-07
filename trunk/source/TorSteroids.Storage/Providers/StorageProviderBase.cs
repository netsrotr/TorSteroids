#region Version Info Header
/*
 * $Id$
 * $HeadURL$
 * Last modified by $Author$
 * Last modified at $Date$
 * $Revision$
 */
#endregion

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Security;
using System.Text;
using JetBrains.Annotations;
using TorSteroids.Storage.Data;
using TorSteroids.Storage.Diagnostics;
using TorSteroids.Storage.Resources;

namespace TorSteroids.Storage.Providers
{
    /// <summary>
    /// Data storage provider base class
    /// </summary>
    public abstract class StorageProviderBase :IStorageInitializer, IStorageProvider
    {
        #region ivars

        private readonly string _connectionString;

        #endregion

        #region ctor's
        
        /// <summary>
        /// Initializes a new instance of the <see cref="StorageProviderBase" /> class. Used by unit tests and currently by StorageFactory
        /// </summary>
        protected internal StorageProviderBase()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StorageProviderBase" /> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="connectionString"/> is null</exception>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="connectionString"/> is empty</exception>
        protected StorageProviderBase([NotNull] string connectionString)
        {
            connectionString.ExceptionIfNullOrEmpty("connectionString");
            _connectionString = connectionString;    
        }

        #endregion

        #region IStorageInitializer

        /// <summary>
        /// Override to build a valid connection string.
        /// </summary>
        /// <param name="databasePath">The database base path.</param>
        /// <param name="databaseName">Name of the database without extension.</param>
        /// <param name="secret">The secret.</param>
        /// <returns>Database connection string</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="databasePath"/> or <paramref name="databaseName"/> is null</exception>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="databasePath"/> or <paramref name="databaseName"/> is empty</exception>
        public abstract string BuildConnectionString([NotNull] string databasePath, [NotNull] string databaseName,
                                                     string secret = null);

        /// <summary>
        /// Override to get the full name of the database file inclusive path. If the storage provider does not support a single file storage, return null.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">If <paramref name="connectionString"/> is null</exception>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="connectionString"/> is an empty string</exception>
        public abstract string GetDatabaseFileName([NotNull] string connectionString);

        
        /// <summary>
        /// Creates the database.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="overwriteExistingDatabaseFile">if set to <c>true</c>, it overwrites a possibly existing database file.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="connectionString"/> is null</exception>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="connectionString"/> is empty</exception>
        /// <exception cref="IOException">On file operations</exception>
        /// <exception cref="SecurityException">On missing file I/O permissions</exception>
        public abstract void CreateDatabase([NotNull] string connectionString, bool overwriteExistingDatabaseFile);

        /// <summary>
        /// Creates the database schema with the provided <paramref name="createSqlSchemaScript" />.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="createSqlSchemaScript">The create SQL schema script.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="connectionString"/> is null</exception>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="connectionString"/> is empty</exception>
        public abstract void CreateSchema([NotNull] string connectionString, string createSqlSchemaScript);


        /// <summary>
        /// Update the database schema.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="updateSqlSchemaScript">The update SQL schema script.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="connectionString"/> is null</exception>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="connectionString"/> is empty</exception>
        public abstract void UpdateSchema([NotNull] string connectionString, string updateSqlSchemaScript);

        /// <summary>
        /// Override to create a concrete storage provider instance.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <returns></returns>
        [NotNull]
        public abstract IStorageProvider CreateInstance([NotNull] string connectionString);

        #endregion

        #region IStorageProvider

        /// <summary>
        /// Override to create the DB connection.
        /// </summary>
        public abstract DbConnection CreateConnection();

        public virtual DbConnection GetConnection()
        {
            DbConnection conn = CreateConnection();
            conn.ConnectionString = ConnectionString;
            return conn;
        }

        /// <summary>
        /// Override to create a DbCommand.
        /// </summary>
        /// <returns></returns>
        public abstract DbCommand CreateCommand();

        /// <summary>
        /// Override to create a DbParameter.
        /// </summary>
        /// <returns></returns>
        public abstract DbParameter CreateParameter();

        public virtual int Insert([NotNull] string tableName, DbParameter identity, [NotNull] List<DbParameter> insertColumns, bool useTransactions = true)
        {
            tableName.ExceptionIfNullOrEmpty("tableName");
            insertColumns.ExceptionIfNull("parameters");

            Guid marker = Trace.GetMarker();
            StringBuilder sb = new StringBuilder();

            using (var connection = this.GetConnection())
            {
                try
                {
                    connection.Open();
                    Trace.Write(marker, "Insert() Connection Open");

                    using (var transaction = connection.BeginTransaction(useTransactions))
                    {
                        if (useTransactions) Trace.Write(marker, "Insert() transaction started");

                        using (var command = connection.CreateCommand().Text())
                        {
                            command.Transaction = transaction;

                            sb.AppendFormat("INSERT INTO {0} (", QuoteName(tableName));

                            bool isFirst = true;
                            foreach (var p in insertColumns)
                            {
                                if (identity != null && p.SourceColumn != identity.SourceColumn)
                                {
                                    sb.AppendFormat(" {0} {1}", isFirst ? string.Empty : ", ", QuoteName(p.SourceColumn));
                                    isFirst = false;
                                }
                            }

                            sb.Append(") VALUES (");
                            isFirst = true;

                            foreach (var p in insertColumns)
                            {
                                if (identity != null && p.SourceColumn != identity.SourceColumn)
                                {
                                    sb.AppendFormat(" {0} {1}", isFirst ? string.Empty : ", ",
                                                    SqlVariable(p.ParameterName));
                                    command.Parameters.Add(p);
                                    isFirst = false;
                                }
                            }

                            try
                            {
                                if (identity != null && identity.DbType != DbType.String)
                                {
                                    command.CommandText = sb.AppendFormat("); {0}",
                                        GetRecentSqlIdentityId(tableName, identity.SourceColumn)).ToString();
                                    object obj = command.ExecuteScalar();
                                    Trace.Write(marker, "Insert() completed");
                                    transaction.Commit();
                                    if (useTransactions) Trace.Write(marker, "Insert() transaction commit");

                                    return Db.ParseValue(obj, 0);
                                }

                                command.CommandText = sb.AppendFormat(")").ToString();
                                command.ExecuteNonQuery();
                                Trace.Write(marker, "Insert() without identity completed");
                                transaction.Commit();
                                if (useTransactions) Trace.Write(marker, "Insert() transaction commit");

                                return -1;
                            }
                            catch (Exception ex)
                            {
                                Trace.Write(marker, "Insert() failed: {0}", ex);
                                transaction.Rollback();
                                if (useTransactions) Trace.Write(marker, "Insert() transaction rollback");
                                throw new StorageException(SR.StorageInsertException.FormatWith(tableName, ex.Message), ex);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Trace.Write(marker, "Insert() failed at {0}: {1}", tableName, ex);
                    throw new StorageException(SR.StorageInsertException.FormatWith(tableName, ex.Message), ex);
                }
            }
        }

        public virtual int Update([NotNull]string tableName, DbParameter primaryKey, [NotNull]List<DbParameter> updateColumns, bool useTransactions = true)
        {
            tableName.ExceptionIfNullOrEmpty("tableName");
            updateColumns.ExceptionIfNull("parameters");

            Guid marker = Trace.GetMarker();
            StringBuilder sb = new StringBuilder();

            using (var connection = this.GetConnection())
            {
                try
                {
                    connection.Open();
                    Trace.Write(marker, "Update() Connection Open");

                    using (var transaction = connection.BeginTransaction(useTransactions))
                    {
                        if (useTransactions) Trace.Write(marker, "Update() transaction started");

                        using (var command = connection.CreateCommand().Text())
                        {
                            command.Transaction = transaction;

                            sb.AppendFormat("UPDATE {0} SET ", QuoteName(tableName));

                            bool isFirst = true;
                            foreach (var p in updateColumns)
                            {
                                if (primaryKey != null && p.SourceColumn != primaryKey.SourceColumn)
                                {
                                    sb.AppendFormat(" {0} {1} = {2} ", isFirst ? string.Empty : ", ",
                                                    QuoteName(p.SourceColumn), SqlVariable(p.ParameterName));
                                    command.Parameters.Add(p);
                                    isFirst = false;
                                }
                            }

                            if (primaryKey != null)
                            {
                                sb.AppendFormat(" WHERE {0} = {1}", QuoteName(primaryKey.SourceColumn),
                                                SqlVariable(primaryKey.ParameterName));
                                command.Parameters.Add(primaryKey);
                            }

                            command.CommandText = sb.ToString();

                            try
                            {
                                var result = command.ExecuteNonQuery();
                                Trace.Write(marker, "Update() completed");
                                transaction.Commit();
                                if (useTransactions) Trace.Write(marker, "Update() transaction commit");
                                return result;
                            }
                            catch (Exception ex)
                            {
                                Trace.Write(marker, "Update() failed: {0}", ex);
                                transaction.Rollback();
                                if (useTransactions) Trace.Write(marker, "Update() transaction rollback");
                                throw new StorageException(
                                    SR.StorageUpdateException.FormatWith(tableName, ex.Message), ex);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Trace.Write(marker, "Update() failed at {0}: {1}", tableName, ex);
                    throw new StorageException(SR.StorageUpdateException.FormatWith(tableName, ex.Message), ex);
                }
            }
        }

        #region Delete

        public virtual int Delete([NotNull]string tableName, [NotNull]DbParameter rowSelector, bool useTransactions=true)
        {
            tableName.ExceptionIfNullOrEmpty("tableName");
            rowSelector.ExceptionIfNull("rowSelector");

            Guid marker = Trace.GetMarker();

            using (var connection = this.GetConnection())
            {
                try
                {
                    connection.Open();
                    Trace.Write(marker, "Delete() Connection Open");

                    using (var transaction = connection.BeginTransaction(useTransactions))
                    {
                        if (useTransactions) Trace.Write(marker, "Delete() transaction started");

                        using (var command = connection.CreateCommand().Text())
                        {
                            command.Transaction = transaction;

                            command.CommandText = string.Format(
                                "DELETE FROM {0} WHERE {1} = {2}"
                                , QuoteName(tableName)
                                , QuoteName(rowSelector.SourceColumn)
                                , SqlVariable(rowSelector.ParameterName)
                                );

                            command.Parameters.Add(rowSelector);

                            try
                            {
                                var result = command.ExecuteNonQuery();
                                Trace.Write(marker, "Delete() completed");
                                transaction.Commit();
                                if (useTransactions) Trace.Write(marker, "Delete() transaction commit");
                                return result;
                            }
                            catch (Exception ex)
                            {
                                Trace.Write(marker, "Delete() failed: {0}", ex);
                                transaction.Rollback();
                                if (useTransactions) Trace.Write(marker, "Delete() transaction rollback");
                                throw new StorageException(
                                    SR.StorageDeleteException.FormatWith(tableName, ex.Message), ex);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Trace.Write(marker, "Delete() failed at {0}: {1}", tableName, ex);
                    throw new StorageException(SR.StorageDeleteException.FormatWith(tableName, ex.Message), ex);
                }
            }
        }

        #endregion

        #region Commands

        public virtual IDataReader ExecuteReader(DbCommand command)
        {
            Guid marker = Trace.GetMarker();
            command = CheckCommandConnection(marker, command);
            try
            {
                EnsureOpenConnection(command.Connection);
                Trace.Write(marker, "ExecuteReader() Connection Open");
                IDataReader dr = command.ExecuteReader(CommandBehavior.CloseConnection);
                Trace.Write(marker, "ExecuteReader() IDataReader returned");
                return dr;
            }
            catch (Exception ex)
            {
                Trace.Write(marker, "ExecuteReader() failed: {0}", ex);
                throw new StorageException(SR.StorageExecuteReaderException.FormatWith(ex.Message), ex);
            }
        }
        
        public virtual int ExecuteNonQuery(DbCommand command, bool useTransactions = true)
        {
            Guid marker = Trace.GetMarker();
            command = CheckCommandConnection(marker, command);

            using (DbConnection connection = command.Connection)
            {
                try
                {
                    connection.Open();
                    Trace.Write(marker, "ExecuteNonQuery() Connection Open");

                    using (var transaction = connection.BeginTransaction(useTransactions))
                    {
                        if (useTransactions) Trace.Write(marker, "ExecuteNonQuery() transaction started");
                        command.Transaction = transaction;

                        try
                        {
                            int rowCount = command.ExecuteNonQuery();
                            Trace.Write(marker, "ExecuteNonQuery() completed");

                            transaction.Commit();

                            if (useTransactions) Trace.Write(marker, "ExecuteNonQuery() transaction commit");

                            connection.Close();
                            return rowCount;
                        }
                        catch (Exception ex)
                        {
                            Trace.Write(marker, "ExecuteNonQuery() failed: {0}", ex);
                            transaction.Rollback();
                            if (useTransactions) Trace.Write(marker, "ExecuteNonQuery() transaction rollback");
                            throw new StorageException(SR.StorageExecuteNonQueryException.FormatWith(ex.Message), ex);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Trace.Write(marker, "ExecuteNonQuery() failed: {0}", ex);
					throw new StorageException(SR.StorageExecuteNonQueryException.FormatWith(ex.Message), ex);
                }
            }
        }

        public virtual object ExecuteScalar(DbCommand command)
        {
            Guid marker = Trace.GetMarker();
            command = CheckCommandConnection(marker, command);

            using (DbConnection conn = command.Connection)
            {
                try
                {
                    conn.Open();
                    Trace.Write(marker, "ExecuteScalar() Connection Open");
                    object obj = command.ExecuteScalar();
                    Trace.Write(marker, "ExecuteScalar() completed");
                    conn.Close();
                    return obj;
                }
                catch (Exception ex)
                {
                    Trace.Write(marker, "ExecuteScalar() failed: {0}", ex);
                    throw new StorageException(SR.StorageExecuteScalarException.FormatWith(ex.Message), ex);
                }
            }
        }

        #endregion

        public virtual string QuoteName(string objectName)
        {
            if (objectName == null) objectName = string.Empty;
            return String.Concat(
                SqlQuoteCharacterPrefix
                , objectName.Replace(SqlQuoteCharacterPostfix, String.Concat(SqlQuoteCharacterPostfix, SqlQuoteCharacterPostfix))
                , SqlQuoteCharacterPostfix);
        }

        public virtual string SqlVariable(string objectName)
        {
            if (objectName == null) objectName = string.Empty;
            return String.Concat(SqlVariablePrefix, objectName);
        }

        #endregion

        #region protected common support functions

        /// <summary>
        /// Override to get the database file extension (exclusive ".")
        /// </summary>
        /// <value>The database file extension.</value>
        public abstract string DefaultDatabaseFileExtension { get; }

        /// <summary>
        /// Gets the connection string, if initialized; else null.
        /// </summary>
        /// <value>
        /// The connection string.
        /// </value>
        protected string ConnectionString
        {
            get { return _connectionString; }
        }

        /// <summary>
        /// Builds the full name of the database file and path (inclusive file extension).
        /// </summary>
        /// <param name="databasePath">The path.</param>
        /// <param name="baseFileName">Name of the base file.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">If baseFileName is null or empty string</exception>
        protected string BuildDatabaseFileName(string databasePath, [NotNull] string baseFileName)
        {
            baseFileName.ExceptionIfNullOrEmpty("baseFileName");
            if (databasePath == null)
                databasePath = String.Empty;

	        if (String.IsNullOrEmpty(Path.GetExtension(baseFileName)))
	        {
		        string fileExtension = DefaultDatabaseFileExtension;

		        if (
			        !String.Equals(Path.GetExtension(baseFileName), "." + fileExtension,
			                       StringComparison.InvariantCultureIgnoreCase))
		        {
			        if (baseFileName.EndsWith("."))
				        baseFileName += fileExtension;
			        else
				        baseFileName += "." + fileExtension;
		        }
	        }

	        return Path.Combine(databasePath, baseFileName);
        }



        protected virtual DbCommand CheckCommandConnection(DbCommand command, DbConnection connection = null)
        {
            return CheckCommandConnection(Trace.GetMarker(), command, connection);
        }

        protected virtual DbCommand CheckCommandConnection(Guid marker, DbCommand command, DbConnection connection = null)
        {
            if (command.Connection == null)
                command.Connection = connection ?? GetConnection();
            Trace.Write(marker, command);
            return command;
        }

        internal static bool EnsureOpenConnection([NotNull]DbConnection connection)
        {
            bool shouldCloseConnection = false;

            try
            {
                if ((connection.State & ConnectionState.Broken) == ConnectionState.Broken)
                {
                    connection.Close();
                    connection.Open();
                    shouldCloseConnection = true;
                }
                else if ((connection.State & ConnectionState.Open) != ConnectionState.Open)
                {
                    connection.Open();
                    shouldCloseConnection = true;
                }
            }
            catch (System.Threading.ThreadAbortException)
            {
                // ignore
            }
            catch (Exception ex)
            {
                throw new StorageException(String.Format(SR.StorageOpenException, ex.Message), ex);
            }

            return shouldCloseConnection;
        }

        /// <summary>
        /// Override to get the recent SQL identity id statement used for basic Insert method implementation 
        /// to return the most recent used identity id with only one operation.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="column">The column.</param>
        /// <returns></returns>
        protected abstract string GetRecentSqlIdentityId(string tableName, string column);

        protected virtual string SqlQuoteCharacterPrefix { get { return "["; } }
        protected virtual string SqlQuoteCharacterPostfix { get { return "]"; } }
        protected virtual string SqlVariablePrefix { get { return "@"; } }
        protected virtual string SqlModOperator { get { return "MOD"; } }
        protected virtual string SqlCountFunctionFormatString { get { return "count({0})"; } }

        /// <summary>
        /// Executes the SQL non query batch.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="commands">The commands.</param>
        /// <param name="useTransactions">if set to <c>true</c> [use transactions].</param>
        /// <returns></returns>
        /// <exception cref="StorageException"></exception>
        /// <exception cref="StorageException">On any data related exception</exception>
        protected virtual int ExecuteSqlNonQueryBatch(string connectionString, IList<string> commands, bool useTransactions = true)
        {
            int modifications = 0;

            if (commands == null || commands.Count == 0)
                return modifications;

            using (var connection = this.CreateConnection())
            {
                connection.ConnectionString = connectionString;
                connection.Open();

                using (var transaction = connection.BeginTransaction(useTransactions))
                {
                    using (var command = connection.CreateCommand().Text())
                    {
                        command.Transaction = transaction;
                        
                        foreach (string cmd in commands)
                        {
                            if (!String.IsNullOrWhiteSpace(cmd))
                            {
                                command.CommandText = cmd;
                         
                                try
                                {
                                    modifications += command.ExecuteNonQuery();
                                }
                                catch (Exception ex)
                                {
                                    transaction.Rollback();
                                    throw new StorageException(String.Format("Exception while executing SQL command '{0}': {1}", cmd, ex.Message), ex);
                                }
                            }
                        }
                    }

                    transaction.Commit();
                }
            }

            return modifications;
        }

        #region protected virtual ITrace DbTrace

        
        //TODO
        /// <summary>
        /// Use 
        /// <code>
        ///		<add key="DataBuddy::EnableTracing" value="true"/>
        /// </code>
        /// to enable the tracing
        /// </summary>
        public virtual ITrace Trace
        {
            get { return TraceManager.Instance; }
        }

        #endregion
        #endregion

    }
}
