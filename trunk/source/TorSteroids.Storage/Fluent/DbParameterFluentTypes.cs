using System.Data.SqlClient;
using System.Globalization;
using TorSteroids.Common.Fluent;

// ReSharper disable CheckNamespace
namespace System.Data.Common
// ReSharper restore CheckNamespace
{
    
    public class FluentDbParameterSetPropertyAndStringValue : FluentDbParameterSetProperty<FluentDbParameterSetPropertyAndStringValue>
    {
        public FluentDbParameterSetPropertyAndStringValue(DbParameter parameter) :
            base(parameter)
        {
            ValueProvider = this;
        }

        public DbParameter SetValue(string value)
        {
            Context.DbType = DbType.String;
            Context.Value = DBNull.Value;
            if (value != null)
                Context.Value = value;
            return Context;
        }
    }

    public class FluentDbParameterSetPropertyAndInt32Value : FluentDbParameterSetProperty<FluentDbParameterSetPropertyAndInt32Value>
    {
        public FluentDbParameterSetPropertyAndInt32Value(DbParameter parameter) :
            base(parameter)
        {
            ValueProvider = this;
        }

        public DbParameter SetValue(Int32 value)
        {
            Context.DbType = DbType.Int32;
            Context.Value = DBNull.Value;
            Context.Value = value;
            return Context;
        }
    }

    public class FluentDbParameterSetPropertyAndEnumValue : FluentDbParameterSetProperty<FluentDbParameterSetPropertyAndEnumValue>
    {
        public FluentDbParameterSetPropertyAndEnumValue(DbParameter parameter) :
            base(parameter)
        {
            ValueProvider = this;
        }

        public override DbParameter SetNull()
        {
            // restore initial DbType, that might be changed by a previous SetValue call: 
            Context.DbType = DbType.Int32;
            return base.SetNull();
        }

        public DbParameter SetValue(Enum value)
        {
            Context.DbType = DbType.Int32;
            string theEnumVal = Enum.Format(value.GetType(), value, "d");
            Type t = Enum.GetUnderlyingType(value.GetType());

            // most common first
            if (t == typeof(Int32))
            {
                Context.DbType = DbType.Int32;
                Context.Value = Int32.Parse(theEnumVal, NumberStyles.Integer);
                return Context;
            }

            if (t == typeof(UInt32))
            {
                // MS SQL Client fails on UInt32 DbType, so we map to the next higher size:
                if (Context is SqlParameter)
                    Context.DbType = DbType.Int64;
                else
                    Context.DbType = DbType.UInt32;
                Context.Value = UInt32.Parse(theEnumVal, NumberStyles.Integer);
                return Context;
            }

            if (t == typeof (Byte))
            {
                Context.DbType = DbType.Byte;
                Context.Value = Byte.Parse(theEnumVal, NumberStyles.Integer);
                return Context;
            }

            if (t == typeof(SByte))
            {
                // MS SQL Client fails on SByte DbType, so we map to the next higher size:
                if (Context is SqlParameter)
                    Context.DbType = DbType.Int16;
                else
                    Context.DbType = DbType.SByte;
                Context.Value = SByte.Parse(theEnumVal, NumberStyles.Integer);
                return Context;
            }

            if (t == typeof(Int64))
            {
                Context.DbType = DbType.Int64;
                Context.Value = Int64.Parse(theEnumVal, NumberStyles.Integer);
                return Context;
            }

            if (t == typeof(UInt64))
            {
                // MS SQL Client fails on UInt64 DbType, so we map to the possibly way too small size (?):
                if (Context is SqlParameter)
                    Context.DbType = DbType.Int64;
                else
                    Context.DbType = DbType.UInt64;
                Context.Value = UInt64.Parse(theEnumVal, NumberStyles.Integer);
                return Context;
            }

            if (t == typeof(Int16))
            {
                Context.DbType = DbType.Int16;
                Context.Value = Int16.Parse(theEnumVal, NumberStyles.Integer);
                return Context;
            }

            if (t == typeof(UInt16))
            {
                // MS SQL Client fails on UInt16 DbType, so we map to the next higher size:
                if (Context is SqlParameter)
                    Context.DbType = DbType.Int32;
                else
                    Context.DbType = DbType.UInt16;
                Context.Value = UInt16.Parse(theEnumVal, NumberStyles.Integer);
                return Context;
            }
            
	        throw new NotImplementedException(String.Format( "Enum underlying type '{0}' of parameter '{1}' is not implemented.", t, Context.ParameterName));
        }
    }

    public class FluentDbParameterSetProperty<T> : FluentContext<DbParameter> where T : class
    {
        
        public FluentDbParameterSetProperty(DbParameter parameter) :
            base(parameter)
        {
        }

        protected T ValueProvider { set; get; }

        public virtual DbParameter SetNull()
        {
            Context.Value = DBNull.Value;
            return Context;
        }

        public T SourceColumn(string columnName)
        {
            Context.SourceColumn = columnName;
            return ValueProvider;
        }

        public T Size(int size)
        {
            Context.Size = size;
            return ValueProvider;
        }

    }

}
