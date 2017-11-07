#region Version Info Header
/*
 * $Id$
 * $HeadURL$
 * Last modified by $Author$
 * Last modified at $Date$
 * $Revision$
 */
#endregion

using System.Data.Common;
using JetBrains.Annotations;
using TorSteroids.Common;
using TorSteroids.Storage.Data;

// ReSharper disable CheckNamespace
namespace System.Data
// ReSharper restore CheckNamespace
{
    public static class FrameworkExtensions
	{
		#region Extend DbCommand

		public static DbParameter AddParameter(this DbCommand command)
	    {
		    var p = command.CreateParameter();
		    command.Parameters.Add(p);
		    return p;
	    }

	    public static DbCommand Text(this DbCommand command)
	    {
		    command.CommandType = CommandType.Text;
		    return command;
	    }

	    public static DbCommand TextFormat(this DbCommand command, string commandTextFormat,
	                                       params object[] formatArguments)
	    {
		    command.CommandType = CommandType.Text;
		    command.CommandText = (commandTextFormat == null ? null : String.Format(commandTextFormat, formatArguments));
		    return command;
	    }

	    #endregion

		#region Extend DbConnection

		public static DbTransaction BeginTransaction(this DbConnection connection, bool useTransaction)
	    {
		    if (useTransaction)
			    return connection.BeginTransaction();

		    return new DbTransactionEmpty(connection);
	    }

	    #endregion

		#region Extend IDataReader

		public static FieldIndexLookup FieldIndexLookup(this IDataReader reader)
	    {
		    return TorSteroids.Storage.Data.FieldIndexLookup.FromDataReader(reader);
	    }


	    public static T GetSafe<T>(this IDataReader reader, [NotNull] string fieldName, T defaultValueIfNull = default(T))
		    where T : struct
	    {
		    return Db.ParseValue(Db.WrappedGetValueByName(reader, fieldName), defaultValueIfNull);
	    }
		public static T GetSafe<T>(this IDataReader reader, int fieldIndex, T defaultValueIfNull = default(T))
			where T : struct
		{
			return Db.ParseValue(Db.WrappedGetValueByIndex(reader, fieldIndex), defaultValueIfNull);
		}

	    public static string GetSafe(this IDataReader reader, [NotNull] string fieldName, string defaultValueIfNull = null)
	    {
		    return Db.ParseValue(Db.WrappedGetValueByName(reader, fieldName), defaultValueIfNull);
	    }
		public static string GetSafe(this IDataReader reader, int fieldIndex, string defaultValueIfNull = null)
		{
			return Db.ParseValue(Db.WrappedGetValueByIndex(reader, fieldIndex), defaultValueIfNull);
		}

		/// <summary>
		/// Gets a DateTime by combining the two database fields used to store 
		/// a database date and database time.
		/// </summary>
		/// <param name="reader">The reader.</param>
		/// <param name="dateFieldName">Name of the date field.</param>
		/// <param name="timeFieldIndex">Name of the time field.</param>
		/// <param name="defaultValueIfNull">The default value if null.</param>
		/// <returns></returns>
		public static DateTime GetSafe(this IDataReader reader, [NotNull] string dateFieldName, [NotNull] string timeFieldIndex, DateTime defaultValueIfNull)
		{
			// the database (should) always store a date/time in UTC. So the default should be of the same Kind:
			var utcDefault = defaultValueIfNull.ToUniversalTime();
			
			var dbDate = Db.Parse(reader, dateFieldName, (DbDate)utcDefault);
			var dbTime = Db.Parse(reader, timeFieldIndex, (DbTime)utcDefault);
			
			// re-combine the read field values:
			return DbDateTime.ToDateTime(dbDate, dbTime);
		}

		/// <summary>
		/// Gets a DateTime by combining the two database fields used to store 
		/// a database date and database time.
		/// </summary>
		/// <param name="reader">The reader.</param>
		/// <param name="dateFieldIndex">Index of the date field.</param>
		/// <param name="timeFieldIndex">Name of the time field.</param>
		/// <param name="defaultValueIfNull">The default value if null.</param>
		/// <returns></returns>
		public static DateTime GetSafe(this IDataReader reader, int dateFieldIndex, int timeFieldIndex, DateTime defaultValueIfNull)
		{
			// the database (should) always store a date/time in UTC. So the default should be of the same Kind:
			var utcDefault = defaultValueIfNull.ToUniversalTime();

			var dbDate = Db.Parse(reader, dateFieldIndex, (DbDate)utcDefault);
			var dbTime = Db.Parse(reader, timeFieldIndex, (DbTime)utcDefault);

			// re-combine the read field values:
			return DbDateTime.ToDateTime(dbDate, dbTime);
		}



	    #endregion

	}
}

// ReSharper disable CheckNamespace
namespace System
// ReSharper restore CheckNamespace
{
	public static class FrameworkExtensions
	{
		#region Extend DateTime

		/// <summary>
		/// Transform te instance to a DbDate (Utc).
		/// </summary>
		/// <param name="instance">The instance.</param>
		/// <returns></returns>
		public static DbDate ToDbDate(this DateTime instance)
		{
			return (DbDate)instance.ToUniversalTime();
		}

		/// <summary>
		/// Transform te instance to a DbTime (Utc).
		/// </summary>
		/// <param name="instance">The instance.</param>
		/// <returns></returns>
		public static DbTime ToDbTime(this DateTime instance)
		{
			return (DbTime)instance.ToUniversalTime();
		}


		#endregion
	}
}
