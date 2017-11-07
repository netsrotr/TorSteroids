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
using System.Data.Common;
using System.Data.SQLite;
using System.IO;
using JetBrains.Annotations;
using TorSteroids.Storage.Providers;
using TorSteroids.Storage.SqLite.Net.Resources;

namespace TorSteroids.Storage.SqLite.Net
{
    /// <summary>
    /// SQLite data engine
    /// </summary>
    public class SqLiteStorageProvider: StorageProviderBase
    {
        internal const string DefaultStorageFileExtension= "db3";

		private const string QuoteCharacterPrefix = "[";
		private const string QuoteCharacterPostfix = "]";
		private const string VariablePrefix = "@";
		private const string CommandDelimiter = ";";
	    private const string ModOperator = "%";

        //const string SqlCountFunctionFormatString = "count({0})";

        /// <summary>
        /// Initializes a new instance of the <see cref="SqLiteStorageProvider"/> class.
        /// </summary>
        public SqLiteStorageProvider()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqLiteStorageProvider" /> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public SqLiteStorageProvider([NotNull]string connectionString):
            base(connectionString)
        {
        }

        public override string BuildConnectionString(string databasePath, string databaseName, string secret = null)
        {
            databasePath.ExceptionIfNullOrEmpty("databasePath");
            databaseName.ExceptionIfNullOrEmpty("databaseName");

            SQLiteConnectionStringBuilder b = new SQLiteConnectionStringBuilder
                {
                    DataSource = BuildDatabaseFileName(databasePath, databaseName)
                };

            if (!String.IsNullOrEmpty(secret))
                b.Password = secret;
            b.FailIfMissing = true;

            return  b.ConnectionString;
        }

        public override string DefaultDatabaseFileExtension
        {
            get { return "db3"; }
        }

        public override string GetDatabaseFileName(string connectionString)
        {
            connectionString.ExceptionIfNullOrEmpty("connectionString");
            SQLiteConnectionStringBuilder b = new SQLiteConnectionStringBuilder(connectionString);
            return b.DataSource;
        }

        public override void CreateDatabase(string connectionString, bool overwriteExistingDatabaseFile)
        {
            connectionString.ExceptionIfNullOrEmpty("connectionString");
            SQLiteConnectionStringBuilder b = new SQLiteConnectionStringBuilder(connectionString);

            string tempName = null;
            if (File.Exists(b.DataSource))
            {
                if (!overwriteExistingDatabaseFile)
                    throw new IOException(
                        String.Format(SR.DatabaseYetExistsException, b.DataSource));
                string path = "" + Path.GetDirectoryName(b.DataSource);
                tempName = Path.Combine(path, Guid.NewGuid().ToString("N"));
                File.Move(b.DataSource, tempName);
            }
        
            try
            {
                SQLiteConnection.CreateFile(b.DataSource);
            }
            catch (Exception)
            {
                if (File.Exists(b.DataSource))
                {
                    // db file created, but with error(s)
                    File.Delete(b.DataSource);
                    // restore old file:
                    if (tempName != null)
                        File.Move(tempName, b.DataSource);
                }
                
                throw;
            }

            if (tempName != null)
            {
                // success: remove safe/old copy:
                File.Delete(tempName);
            }
        }

        public override void CreateSchema([NotNull] string connectionString, string createSqlSchemaScript)
        {
            if (String.IsNullOrWhiteSpace(createSqlSchemaScript))
                return;

            var commands = createSqlSchemaScript.Split(new[] { CommandDelimiter }, StringSplitOptions.RemoveEmptyEntries);
            ExecuteSqlNonQueryBatch(connectionString, commands);
        }

        public override void UpdateSchema([NotNull] string connectionString, string updateSqlSchemaScript)
        {
            if (String.IsNullOrWhiteSpace(updateSqlSchemaScript))
                return;
            
            var commands = updateSqlSchemaScript.Split(new[] { CommandDelimiter }, StringSplitOptions.RemoveEmptyEntries);
            ExecuteSqlNonQueryBatch(connectionString, commands);
        }

        public override IStorageProvider CreateInstance([NotNull]string connectionString)
        {
            return new SqLiteStorageProvider(connectionString);
        }

        public override DbConnection CreateConnection() { return SQLiteFactory.Instance.CreateConnection(); }
        public override DbCommand CreateCommand() {  return SQLiteFactory.Instance.CreateCommand();  }
        public override DbParameter CreateParameter() {  return SQLiteFactory.Instance.CreateParameter();  }

        protected override string GetRecentSqlIdentityId(string tableName, string column)
        {
            return "SELECT last_insert_rowid() as " + QuoteName("Identity_ID");
        }

		protected override string SqlModOperator { get { return ModOperator; } }
    }
}
