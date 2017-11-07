using System;
using System.Data;
using System.Globalization;
using JetBrains.Annotations;
using TorSteroids.Storage.Resources;

namespace TorSteroids.Storage.Data
{
    /// <summary>
    /// Database Helper/Utility class to convert <see cref="IDataReader"/> values,
    /// and other helpers.
    /// </summary>
    public sealed class Db
    {
        #region Parse(string)
        /// <summary>
        /// Handle the DBNull instance values returned by a <see cref="IDataReader"/> properly.
        /// </summary>
        /// <param name="reader">Provide the <see cref="IDataReader"/> instance</param>
        /// <param name="fieldName">Provide the database field name</param>
        /// <param name="defaultValueIfNull">Provide the value you want to get back, if the original
        /// value is null or a instance of <seealso cref="DBNull">DBNull</seealso>.</param>
        /// <returns>Either the provided value parameter itself or the default value.</returns>
        /// <exception cref="ArgumentNullException">If reader or <paramref name="fieldName"/> is null/empty</exception>
        /// <exception cref="ArgumentOutOfRangeException">If the <paramref name="fieldName"/> is not member of the resultset</exception>
		public static string Parse([NotNull] IDataReader reader, [NotNull] string fieldName, string defaultValueIfNull = null)
        {
            reader.ExceptionIfNull("reader");
            fieldName.ExceptionIfNullOrEmpty("fieldName");
            
            var theValue = WrappedGetValueByName(reader, fieldName);
			var nullable = DbObjectValueParser.SafeParseString(theValue, fieldName);
	        
			return nullable ?? defaultValueIfNull;
        }

        /// <summary>
        /// Handle the DBNull instance values returned by a <see cref="IDataReader"/> properly.
        /// </summary>
        /// <param name="reader">Provide the <see cref="IDataReader"/> instance</param>
        /// <param name="fieldIndex">Provide the database field index</param>
        /// <param name="defaultValueIfNull">Provide the value you want to get back, if the original
        /// value is null or a instance of <seealso cref="DBNull">DBNull</seealso>.</param>
        /// <returns>Either the provided value parameter itself or the default value.</returns>
        /// <exception cref="ArgumentNullException">If reader is null</exception>
        /// <exception cref="IndexOutOfRangeException">If the <paramref name="fieldIndex"/> is out of range</exception>
		public static string Parse([NotNull] IDataReader reader, int fieldIndex, string defaultValueIfNull = null)
        {
            reader.ExceptionIfNull("reader");

			var theValue = WrappedGetValueByIndex(reader, fieldIndex);
			var nullable = DbObjectValueParser.SafeParseString(theValue, fieldIndex);

			return nullable ?? defaultValueIfNull;
        }

        #endregion

		#region Parse(struct) / generic

		public static T Parse<T>([NotNull] IDataReader reader, [NotNull] string fieldName, T defaultValueIfNull = default(T)) where T : struct
		{
			reader.ExceptionIfNull("reader");
			fieldName.ExceptionIfNullOrEmpty("fieldName");

			object theValue = WrappedGetValueByName(reader, fieldName);
			return SwitchParserByType(theValue, fieldName, defaultValueIfNull);
		}


		public static T Parse<T>([NotNull] IDataReader reader, int fieldIndex, T defaultValueIfNull = default(T)) where T : struct
		{
			reader.ExceptionIfNull("reader");

			object theValue = WrappedGetValueByIndex(reader, fieldIndex);
			return SwitchParserByType(theValue, fieldIndex.ToString(Invariant), defaultValueIfNull);
		}
		
		#endregion

        #region generic value parsing

        /// <summary>
        /// Parses the specified value for string content.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="defaultValueIfNull">The default value.</param>
        /// <returns></returns>
		public static string ParseValue(object value, string defaultValueIfNull = null)
        {
			return DbObjectValueParser.SafeParseString(value, defaultValueIfNull);
        }

        /// <summary>
        /// Parses the specified value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <param name="defaultValueIfNull">The default value if null.</param>
        /// <returns></returns>
        /// <exception cref="DataException">On any conversion exception</exception>
        public static T ParseValue<T>(object value, T defaultValueIfNull) where T : struct
        {
			return SwitchParserByType(value, null, defaultValueIfNull);
        }

        #endregion

        #region other parse functions

        #region ToDatabaseDate()

        /// <summary>
        /// Converts a Local DateTime to a Database Date. No conversion to UniversalTime will happen.
        /// </summary>
        /// <param name="date">Local DateTime</param>
        /// <returns>integer</returns>
        /// <remarks>Remember to provide a LOCAL DateTime instance!</remarks>
        public static int ToDatabaseDate(DateTime date)
        {
            return ToDatabaseDate(date, false);
        }

        /// <summary>
        /// Converts a Date to a Database Date.
        /// </summary>
        /// <param name="date">DateTime</param>
        /// <param name="convertToUniversalTime">Set this to true, if the provided date
        /// is not already a UTC date, else false.
        /// If it is true, a conversion to UniversalTime will happen!</param>
        /// <returns>integer</returns>
        public static int ToDatabaseDate(DateTime date, bool convertToUniversalTime)
        {
            if (!convertToUniversalTime)
                return (10000 * date.Year + 100 * date.Month + date.Day);
            DateTime d = date.ToUniversalTime();
            return (10000 * d.Year + 100 * d.Month + d.Day);
        }

        /// <summary>
        /// Converts a Local Time a Database Time. No conversion to UniversalTime will happen.
        /// </summary>
        /// <param name="time">Local DateTime (Time part only)</param>
        /// <returns>integer</returns>
        public static int ToDatabaseTime(DateTime time)
        {
            return ToDatabaseTime(time, false);
        }

        /// <summary>
        /// Converts a Time to a Database Time.
        /// </summary>
        /// <param name="time">DateTime (Time part only)</param>
        /// <param name="convertToUniversalTime">Set this to true, if the provided time
        /// is not already a UTC time, else false.
        /// If it is true, a conversion to UniversalTime will happen!</param>
        /// <returns>integer</returns>
        public static int ToDatabaseTime(DateTime time, bool convertToUniversalTime)
        {
            if (convertToUniversalTime)
                time = time.ToUniversalTime();
            return time.Hour * 10000000 + time.Minute * 100000 + time.Second * 1000 + time.Millisecond;
        }

        #endregion

        #region FromDatabaseDate()

        

        /// <summary>
        /// Converts a Database Date to a DateTime object (Local).
        /// </summary>
        /// <param name="date">int</param>
        /// <param name="convertToLocalTime">Set this to true, if the provided date
        /// is a UTC date, else false. If it is true, a conversion to LocalTime will happen!</param>
        /// <returns>DateTime (Local)</returns>
        /// <remarks>Just the same as calling Parse(int, 0, bool)</remarks>
        public static DateTime FromDatabaseDate(int date, bool convertToLocalTime)
        {
            return Parse(date, 0, convertToLocalTime);
        }

        /// <summary>
        /// Converts a database date and time (UTC or not) to a DateTime object (Local)
        /// </summary>
        /// <param name="date">int</param>
        /// <param name="time">int</param>
        /// <param name="convertToLocalTime">Set this to true, if the provided date
        /// is a UTC date, else false. If it is true, a conversion to LocalTime will happen!</param>
        /// <returns>DateTime (Local)</returns>
        /// <remarks>Just the same as calling Parse(int, int, bool)</remarks>
        public static DateTime FromDatabaseDate(int date, int time, bool convertToLocalTime)
        {
            return Parse(date, time, convertToLocalTime);
        }


        /// <summary>
        /// Parse the provided integer for valid Database Date/Period
        /// (UTC or Local Time).
        /// </summary>
        /// <param name="dateValue">The date integer to be parsed</param>
        /// <param name="timeValue">The time integer to be parsed</param>
        /// <param name="convertToLocalTime">Set this to true, if the provided date/time
        /// is a UTC date, else false. 
        /// If it is true, a conversion to LocalTime will happen!</param>
        /// <returns>DateTime object instance (Local), that defaults to DateTime.UtcNow
        /// on any parse failure.</returns>
        /// <remarks>
        /// If the dateValue parameter is lower/equal zero, the current date will be
        /// returned. If it's length is lower/equal 8, Database Date and Periods are
        /// handled: 'YYYYMMDD', 'YYYYMM' and 'YYYY'.
        /// Special case: 'YMMDD' as of 10101 will return 1.1.0001.
        /// If the timeValue parameter is lower/equal zero, the returned DateTime will
        /// use zeros to initialize the time.
        /// timeValue is a integer of this format: 'HHMMSSmmm' with
        /// HH == Hours, MM = Minutes, SS == Seconds and mmm == Miliseconds.
        /// If it is stored as an integer, the leading zero(s) will be missing
        /// and added back again while parsing!
        /// </remarks>
        public static DateTime Parse(int dateValue, int timeValue, bool convertToLocalTime)
        {
            return Parse(dateValue, timeValue, convertToLocalTime, DateTime.UtcNow);
        }

        /// <summary>
        /// Parse the provided integer for valid Database Date/Period
        /// (UTC or Local Time).
        /// </summary>
        /// <param name="dateValue">The date integer to be parsed</param>
        /// <param name="timeValue">The time integer to be parsed</param>
        /// <param name="convertToLocalTime">Set this to true, if the provided date/time
        /// is a UTC date, else false.
        /// If it is true, a conversion to LocalTime will happen!</param>
        /// <param name="defaultOnFailure">The default value to return on any parse failure.</param>
        /// <returns>DateTime object instance (Local)</returns>
        /// <remarks>
        /// If the dateValue parameter is lower/equal zero, the current date will be
        /// returned. If it's length is lower/equal 8, Database Date and Periods are
        /// handled: 'YYYYMMDD', 'YYYYMM' and 'YYYY'.
        /// Special case: 'YMMDD' as of 10101 will return 1.1.0001.
        /// If the timeValue parameter is lower/equal zero, the returned DateTime will
        /// use zeros to initialize the time.
        /// timeValue is a integer of this format: 'HHMMSSmmm' with
        /// HH == Hours, MM = Minutes, SS == Seconds and mmm == Miliseconds.
        /// If it is stored as an integer, the leading zero(s) will be missing
        /// and added back again while parsing!
        /// </remarks>
        public static DateTime Parse(int dateValue, int timeValue, bool convertToLocalTime, DateTime defaultOnFailure)
        {
            DateTime result = defaultOnFailure;
            // dtPart inits to {Year,Month,Day,Hour,Minute,Second,Milisecond}
            var dtPart = new[]{result.Year,result.Month,result.Day,
									result.Hour,result.Minute,result.Second,result.Millisecond};

            if (dateValue > 1000000)
            {// Database Date: YYYYMMDD 

                dtPart[0] = dateValue / 10000;
                dtPart[1] = (dateValue - (dtPart[0] * 10000)) / 100;
                dtPart[2] = dateValue - (dtPart[0] * 10000) - (dtPart[1] * 100);
                dtPart[3] = 0;
                dtPart[4] = 0;
                dtPart[5] = 0;
                dtPart[6] = 0;
            }
            else
            {
                string dateString = dateValue.ToString(Invariant).Trim();
                if (dateString.Length == 6)
                {	// Database Date: YYYYMM 
                    dtPart[0] = Int32.Parse(dateString.Substring(0, 4));
                    dtPart[1] = Int32.Parse(dateString.Substring(4, 2));
                }

                if (dateString.Length == 5 && dateString == "10101")
                {	// Database Date: MinimumDate 1. Jan. 0001
                    dtPart[0] = 1;
                    dtPart[1] = 1;
                    dtPart[2] = 1;
                }
                if (dateString.Length == 4)
                {	// Database Date: YYYY
                    dtPart[0] = Int32.Parse(dateString.Substring(0, 4));
                }
            }

            if (timeValue > 0)
            {
                // always fill with leading zeros to get around the missing digits parsing an int
                string timeString = timeValue.ToString("000000000").Trim();

                if (timeString.Length == 9)
                {	// Database Time: HHMMSSmmm 
                    dtPart[3] = Int32.Parse(timeString.Substring(0, 2)); // HH
                    dtPart[4] = Int32.Parse(timeString.Substring(2, 2));	// MM
                    dtPart[5] = Int32.Parse(timeString.Substring(4, 2));	// SS
                    dtPart[6] = Int32.Parse(timeString.Substring(6, 3));	// mmm
                }
                else
                {
                    throw new InvalidOperationException("Cannot parse timeValue: invalid length.");
                }
            }

            result = new DateTime(dtPart[0], dtPart[1], dtPart[2],
                    dtPart[3], dtPart[4], dtPart[5], dtPart[6]);

            if (convertToLocalTime)
                return result.ToLocalTime();
            return result;
        }

        #endregion

		#endregion

		#region private

		/// <summary>
		/// Wrap the access to the value by name to sanitize/unify the DB provider specific exceptions.
		/// </summary>
		/// <param name="reader">The reader.</param>
		/// <param name="fieldName">Name of the field.</param>
		/// <returns></returns>
		/// <exception cref="System.ArgumentOutOfRangeException"></exception>
        internal static object WrappedGetValueByName(IDataRecord reader, string fieldName)
        {
            try
            {
                return reader[fieldName];
            }
            catch (Exception ex)
            {
                throw new ArgumentOutOfRangeException(SR.AccessToUnknownTableFieldException.FormatWith(fieldName), ex);
            }
        }

		/// <summary>
		/// Wrap the access to the value by index to sanitize/unify the DB provider specific exceptions.
		/// </summary>
		/// <param name="reader">The reader.</param>
		/// <param name="fieldIndex">Index of the field.</param>
		/// <returns></returns>
		/// <exception cref="System.IndexOutOfRangeException"></exception>
		internal static object WrappedGetValueByIndex(IDataRecord reader, int fieldIndex)
		{
			try
			{
				return reader.GetValue(fieldIndex);
			}
			catch (Exception ex)
			{
				throw new IndexOutOfRangeException(SR.AccessToUnknownTableFieldIndexException.FormatWith(fieldIndex), ex);
			}
		}

		private static T SwitchParserByType<T>(object value, object fieldInfo, T defaultValueIfNull) where T : struct
		{

			object dv = defaultValueIfNull;

			// most common types first:
			switch (Type.GetTypeCode(typeof(T)))
			{
				case TypeCode.Int32:
					{
						var nullable = DbObjectValueParser.SafeParseNullableInt32(value, fieldInfo);
						return nullable.HasValue ? (T)(IConvertible)nullable.Value : defaultValueIfNull;
					}
				case TypeCode.Int64:
					{
						var nullable = DbObjectValueParser.SafeParseNullableInt64(value, fieldInfo);
						return nullable.HasValue ? (T)(IConvertible)nullable.Value : defaultValueIfNull;
					}
				case TypeCode.Double:
					{
						var nullable = DbObjectValueParser.SafeParseNullableDouble(value, fieldInfo);
						return nullable.HasValue ? (T)(IConvertible)nullable.Value : defaultValueIfNull;
					}
				case TypeCode.Single:
					{
						var nullable = DbObjectValueParser.SafeParseNullableSingle(value, fieldInfo);
						return nullable.HasValue ? (T)(IConvertible)nullable.Value : defaultValueIfNull;
					}
				case TypeCode.Decimal:
					{
						var nullable = DbObjectValueParser.SafeParseNullableDecimal(value, fieldInfo);
						return nullable.HasValue ? (T)(IConvertible)nullable.Value : defaultValueIfNull;
					}
				case TypeCode.Boolean:
					{
						var nullable = DbObjectValueParser.SafeParseNullableBoolean(value, fieldInfo);
						return nullable.HasValue ? (T)(IConvertible)nullable.Value : defaultValueIfNull;
					}
				case TypeCode.UInt64:
					{
						var nullable = DbObjectValueParser.SafeParseNullableUInt64(value, fieldInfo);
						return nullable.HasValue ? (T)(IConvertible)nullable.Value : defaultValueIfNull;
					}
				case TypeCode.UInt32:
					{
						var nullable = DbObjectValueParser.SafeParseNullableUInt32(value, fieldInfo);
						return nullable.HasValue ? (T)(IConvertible)nullable.Value : defaultValueIfNull;
					}
				case TypeCode.Int16:
					{
						var nullable = DbObjectValueParser.SafeParseNullableInt16(value, fieldInfo);
						return nullable.HasValue ? (T)(IConvertible)nullable.Value : defaultValueIfNull;
					}
				case TypeCode.UInt16:
					{
						var nullable = DbObjectValueParser.SafeParseNullableUInt16(value, fieldInfo);
						return nullable.HasValue ? (T)(IConvertible)nullable.Value : defaultValueIfNull;
					}
				case TypeCode.Byte:
					{
						var nullable = DbObjectValueParser.SafeParseNullableByte(value, fieldInfo);
						return nullable.HasValue ? (T)(IConvertible)nullable.Value : defaultValueIfNull;
					}
				case TypeCode.SByte:
					{
						var nullable = DbObjectValueParser.SafeParseNullableSByte(value, fieldInfo);
						return nullable.HasValue ? (T)(IConvertible)nullable.Value : defaultValueIfNull;
					}
				default:
					{
#pragma warning disable 184
						if (typeof(T) is Enum)
#pragma warning restore 184
						{
							// ReSharper disable HeuristicUnreachableCode
							return (T)(IConvertible)DbObjectValueParser.SafeParseEnum(value, (Enum)dv, fieldInfo, false);
							// ReSharper restore HeuristicUnreachableCode
						}

						if (typeof(T) == typeof(DateTime))
						{
							var nullable = DbObjectValueParser.SafeParseNullableDateTime(value, fieldInfo);
							return nullable != null ? (T)(IConvertible)nullable : defaultValueIfNull;
						}

						//TODO:??
						if (typeof(T) == typeof(DbDate))
						{
							var nullable = DbObjectValueParser.SafeParseNullableDate(value, fieldInfo);
							return nullable.HasValue ? (T)(object)nullable.Value : defaultValueIfNull;
						}

						if (typeof(T) == typeof(DbTime))
						{
							var nullable = DbObjectValueParser.SafeParseNullableTime(value, fieldInfo);
							return nullable.HasValue ? (T)(object)nullable.Value : defaultValueIfNull;
						}

						throw new NotImplementedException("SwitchParserByType<" + typeof(T).Name + ">(value) not (yet) implemented");
					}
			}
		}


		/// <summary>
		/// Shortcut to the invariant and number format used to parse double
		/// </summary>
		private static readonly IFormatProvider Invariant = CultureInfo.InvariantCulture;

        #endregion
    }
}
