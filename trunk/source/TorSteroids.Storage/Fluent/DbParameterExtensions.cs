#region Version Info Header
/*
 * $Id$
 * $HeadURL$
 * Last modified by $Author$
 * Last modified at $Date$
 * $Revision$
 */
#endregion

// ReSharper disable CheckNamespace
namespace System.Data.Common
// ReSharper restore CheckNamespace
{

    public static class DbParameterExtensions
    {
        public static FluentDbParameterSetPropertyAndEnumValue AsEnum(this DbParameter instance, string parameterName)
        {
            instance.ParameterName = parameterName;
            instance.SourceColumn = parameterName;
            instance.Direction = ParameterDirection.Input;
            instance.DbType = DbType.Int32;
            instance.Value = DBNull.Value;
            return new FluentDbParameterSetPropertyAndEnumValue(instance);
        }

        public static FluentDbParameterSetPropertyAndStringValue AsString(this DbParameter instance, string parameterName)
        {
            instance.ParameterName = parameterName;
            instance.SourceColumn = parameterName;
            instance.Direction = ParameterDirection.Input;
            instance.Size = -1;
            instance.DbType = DbType.String;
            instance.Value = DBNull.Value;
            return new FluentDbParameterSetPropertyAndStringValue(instance);
        }

        public static FluentDbParameterSetPropertyAndInt32Value AsInt32(this DbParameter instance, string parameterName)
        {
            instance.ParameterName = parameterName;
            instance.SourceColumn = parameterName;
            instance.Direction = ParameterDirection.Input;
            instance.DbType = DbType.Int32;
            instance.Value = DBNull.Value;
            return new FluentDbParameterSetPropertyAndInt32Value(instance);
        }

    }
}
