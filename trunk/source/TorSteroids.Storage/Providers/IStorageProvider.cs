#region Version Info Header
/*
 * $Id$
 * $HeadURL$
 * Last modified by $Author$
 * Last modified at $Date$
 * $Revision$
 */
#endregion

using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using JetBrains.Annotations;

namespace TorSteroids.Storage.Providers
{
    public interface IStorageProvider
    {
        /// <summary>
        /// Just creates the connection instance.
        /// </summary>
        /// <returns></returns>
        DbConnection CreateConnection();
        
        /// <summary>
        /// Gets a fully initialized connection ready for open and usage.
        /// </summary>
        /// <returns></returns>
        DbConnection GetConnection();

        DbCommand CreateCommand();
        DbParameter CreateParameter();

        int Insert([NotNull] string tableName, DbParameter identity, [NotNull] List<DbParameter> insertColumns, bool useTransactions = true);
        int Update([NotNull] string tableName, [NotNull] DbParameter primaryKey, [NotNull] List<DbParameter> updateColumns, bool useTransactions = true);
        int Delete([NotNull] string tableName, [NotNull] DbParameter rowSelector, bool useTransactions = true);

        IDataReader ExecuteReader([NotNull] DbCommand command);
        int ExecuteNonQuery([NotNull] DbCommand command, bool useTransactions = true);
        object ExecuteScalar([NotNull] DbCommand command);

        string QuoteName(string objectName);
        string SqlVariable(string objectName);
    }
}
