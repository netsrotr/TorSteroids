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
using System.Data;
using System.Globalization;
using JetBrains.Annotations;
using TorSteroids.Storage.Resources;

namespace TorSteroids.Storage.Data
{
	internal static class DbObjectValueParser
	{
		/// <summary>
		/// Shortcut to the invariant and number format used to parse 
		/// </summary>
		private static readonly IFormatProvider Invariant = CultureInfo.InvariantCulture;
		private static readonly IFormatProvider InvariantNumberInfo = CultureInfo.InvariantCulture.NumberFormat;

		private const string IndexFieldInfo = "[{0}]";

		/// <summary>
		/// Parses the specified value for string content and handle the DBNull instance or null values properly: 
		/// null is returned in these cases.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="fieldInfo">The field info used in exceptional cases.</param>
		/// <returns></returns>
		/// <exception cref="System.Data.DataException"></exception>
		public static string SafeParseString(object value, object fieldInfo = null)
		{
			if (value != null && !(value is DBNull))
			{
				try
				{
					return value.ToString();
				}
				catch (Exception ex)
				{
					throw FormattedException(ex, value, value.GetType(), typeof(string), fieldInfo);
				}
			}

			return null;
		}

		public static decimal? SafeParseNullableDecimal(object value, object fieldInfo = null)
		{
			return SafeParseNullableNumber(value, Convert.ToDecimal, fieldInfo);
		}

		public static float? SafeParseNullableSingle(object value, object fieldInfo = null)
		{
			return SafeParseNullableNumber(value, Convert.ToSingle, fieldInfo);
		}

		public static double? SafeParseNullableDouble(object value, object fieldInfo = null)
		{
			return SafeParseNullableNumber(value, Convert.ToDouble, fieldInfo);
		}

		public static bool? SafeParseNullableBoolean(object value, object fieldInfo = null)
		{
			if (value != null && !(value is DBNull))
			{
				switch (Type.GetTypeCode(value.GetType()))
				{
					case TypeCode.Empty:
						return null;

					case TypeCode.Boolean:
						return (bool)value;

					case TypeCode.String:
						{
							// handle the empty case (usually this cannot be converted to numbers)
							string s = value as string;
							if (String.IsNullOrEmpty(s))
								return null;

							// handle possible "true/false" strings 
							bool b;
							if (bool.TryParse(s, out b))
								return b;

							// handle any numbers
							int n;
							if (Int32.TryParse(s, NumberStyles.Any, InvariantNumberInfo, out n))
							{
								if (n == 0) 
									return false;
								return true;
							}

							try
							{
								// our last try for strings:
								return Convert.ToBoolean(value);
							}
							catch (Exception ex)
							{
								throw FormattedException(ex, value, value.GetType(), typeof(bool), fieldInfo);
							}
						}

					default:

						try
						{
							if (Convert.ToInt32(value) == 0)
								return false;
							return true;
						}
						catch (Exception ex)
						{
							throw FormattedException(ex, value, value.GetType(), typeof(bool), fieldInfo);
						}

				}
			}
			return null;
		}

		public static long? SafeParseNullableInt64(object value, object fieldInfo = null)
		{
			return SafeParseNullableNumber(value, Convert.ToInt64, fieldInfo);
		}

		public static ulong? SafeParseNullableUInt64(object value, object fieldInfo = null)
		{
			return SafeParseNullableNumber(value, Convert.ToUInt64, fieldInfo);
		}

		public static int? SafeParseNullableInt32(object value, object fieldInfo = null)
		{
			return SafeParseNullableNumber(value, Convert.ToInt32, fieldInfo);
		}

		public static uint? SafeParseNullableUInt32(object value, object fieldInfo = null)
		{
			return SafeParseNullableNumber(value, Convert.ToUInt32, fieldInfo);
		}

		public static short? SafeParseNullableInt16(object value, object fieldInfo = null)
		{
			return SafeParseNullableNumber(value, Convert.ToInt16, fieldInfo);
		}

		public static ushort? SafeParseNullableUInt16(object value, object fieldInfo = null)
		{
			return SafeParseNullableNumber(value, Convert.ToUInt16, fieldInfo);
		}

		public static byte? SafeParseNullableByte(object value, object fieldInfo = null)
		{
			return SafeParseNullableNumber(value, Convert.ToByte, fieldInfo);
		}

		public static sbyte? SafeParseNullableSByte(object value, object fieldInfo = null)
		{
			return SafeParseNullableNumber(value, Convert.ToSByte, fieldInfo);
		}
		
		public static Enum SafeParseEnum(object value, Enum defaultValueIfNull, object fieldInfo = null, bool throwOnUndefinedEnum = false)
		{
			if (value != null && !(value is DBNull))
			{
				Type typeOfDefault = defaultValueIfNull.GetType();
				
				switch (Type.GetTypeCode(value.GetType()))
				{
					// we try to cast what we get...
					case TypeCode.Empty:	// should be equal to test for "xx == null"
						return defaultValueIfNull;
					
					case TypeCode.String:
						var strval = (string)value;
						if (throwOnUndefinedEnum && !Enum.IsDefined(typeOfDefault, strval))
							throw FormattedException(new InvalidCastException(SR.InvalidEnumCastException.FormatWith(strval, typeOfDefault)),
								strval, value.GetType(), typeOfDefault, fieldInfo);
							
						return (Enum)Enum.Parse(typeOfDefault, strval);
					
					default:
						try
						{
							var val = Convert.ToInt64(value).ToString(InvariantNumberInfo);
							var parsed = (Enum)Enum.Parse(typeOfDefault, val);
							if (throwOnUndefinedEnum && !Enum.IsDefined(typeOfDefault, parsed))
								throw new InvalidCastException(SR.InvalidEnumCastException.FormatWith(value, typeOfDefault));
							
							return parsed;
						}
						catch (Exception ex)
						{
							throw FormattedException(ex, value, value.GetType(), typeOfDefault, fieldInfo);
						}

				}
			}
			return defaultValueIfNull;
		}

		public static DateTime? SafeParseNullableDateTime(object value, object fieldInfo = null,
			DateTimeKind kind = DateTimeKind.Utc)
		{
			if (value != null && !(value is DBNull))
			{
				try
				{
					string dateTimeString;

					switch (Type.GetTypeCode(value.GetType()))
					{
						case TypeCode.Empty:
							return null;

						case TypeCode.String:
							dateTimeString = (string)value;
							break;
						case TypeCode.Int32:
							dateTimeString = Convert.ToInt32(value).ToString(Invariant);
							break;
						case TypeCode.UInt32:
							dateTimeString = Convert.ToUInt32(value).ToString(Invariant);
							break;
						case TypeCode.Int64:
							return new DateTime(Convert.ToInt64(value), kind);
						case TypeCode.UInt64:
							return new DateTime(Convert.ToInt64(value), kind);

						case TypeCode.DateTime:
							//TODO: review if we have to use DateTimeKind to transform - this info is DB provider specific stored or not...
							return (DateTime)value;

						default:
							dateTimeString = value.ToString();
							break;

					}

					dateTimeString = dateTimeString.Trim();
					if (String.IsNullOrEmpty(dateTimeString))
						return null;

					if (dateTimeString == "0")
						return DateTime.MinValue;

					return DbDateTime.ParseDateTime(dateTimeString, kind);

				}
				catch (Exception ex)
				{
					throw FormattedException(ex, value, value.GetType(), typeof(DateTime), fieldInfo);
				}

			}

			return null;
		}

		public static DbDate? SafeParseNullableDate(object value, object fieldInfo = null)
		{
			if (value != null && !(value is DBNull))
			{
				try
				{
					string dateString;

					switch (Type.GetTypeCode(value.GetType()))
					{
						case TypeCode.Empty:
							return null;

						case TypeCode.String:
							dateString = (string)value;
							break;

						case TypeCode.Int32:
							return new DbDate(Convert.ToInt32(value));
						case TypeCode.Int64:
							return new DbDate(Convert.ToInt32(value));
						case TypeCode.UInt32:
							return new DbDate(Convert.ToInt32(value));
						case TypeCode.UInt64:
							return new DbDate(Convert.ToInt32(value));

						case TypeCode.DateTime:
							var date = (DateTime)value;
							return (DbDate)date;

						default:
							dateString = (string)value;
							break;

					}

					dateString = dateString.Trim();
					if (String.IsNullOrEmpty(dateString))
						return null;

					if (dateString == "0")
						return DbDate.MinValue;

					return DbDateTime.ParseDate(dateString.Trim());

				}
				catch (Exception ex)
				{
					throw FormattedException(ex, value, value.GetType(), typeof(DbDate), fieldInfo);
				}
			}

			return null;
		}

		public static DbTime? SafeParseNullableTime(object value, object fieldInfo = null,
			DateTimeKind kind = DateTimeKind.Utc, DbTimeStyle style = DbTimeStyle.Full)
		{
			if (value != null && !(value is DBNull))
			{
				try
				{
					string timeString;

					switch (Type.GetTypeCode(value.GetType()))
					{
						case TypeCode.Empty:
							return null;

						case TypeCode.String:
							timeString = (string)value;
							break;
						
						case TypeCode.Int32:
							return new DbTime(Convert.ToInt32(value), kind);
						case TypeCode.Int64:
							return new DbTime(Convert.ToInt32(value), kind);
						case TypeCode.UInt32:
							return new DbTime(Convert.ToInt32(value), kind);
						case TypeCode.UInt64:
							return new DbTime(Convert.ToInt32(value), kind);

						case TypeCode.DateTime:
							var dt = (DateTime)value;
							return (DbTime)dt;

						default:
							timeString = value.ToString();
							break;
					}

					timeString = timeString.Trim();
					if (String.IsNullOrEmpty(timeString))
						return null;

					if (timeString == "0")
						return DbTime.MinValue;

					return DbDateTime.ParseTime(timeString, kind, style);

				}
				catch (Exception ex)
				{
					throw FormattedException(ex, value, value.GetType(), typeof(DbDate), fieldInfo);
				}
			}

			return null;
		}


		#region private

		private delegate T Conversion<out T>(object value, IFormatProvider formatProvider);

		private static T? SafeParseNullableNumber<T>(object value, Conversion<T> convert, object fieldInfo) where T:struct
		{
			if (value != null && !(value is DBNull))
			{
				switch (Type.GetTypeCode(value.GetType()))
				{
					case TypeCode.Empty:
						return null;

					case TypeCode.Boolean:
						// handle hwo we do interpret the boolean
						if ((bool)value)
							return convert(1, InvariantNumberInfo);
						return convert(0, InvariantNumberInfo);

					case TypeCode.String:
						// handle the empty case (usually this cannot be converted to numbers)
						string s = value as string;
						if (String.IsNullOrEmpty(s))
							return null;
						break;
				}

				try
				{
					return convert(value, InvariantNumberInfo);
				}
				catch (Exception ex)
				{
					throw FormattedException(ex, value, value.GetType(), typeof(T), fieldInfo);
				}

			}

			return null;
		}

		private static Exception FormattedException([NotNull]Exception ex, object value, Type sourceType, Type targetType, object fieldInfo)
		{
			if (fieldInfo == null)
				return new DataException(SR.InvalidDbTypeCastException
					.FormatWith(value, sourceType, targetType, ex.Message), ex);

			if (fieldInfo is int)
				return new DataException(SR.InvalidDbTypeCastExceptionAtField
					.FormatWith(IndexFieldInfo.FormatWith(fieldInfo), value, sourceType, targetType, ex.Message), ex);

			return new DataException(SR.InvalidDbTypeCastExceptionAtField
				.FormatWith(fieldInfo, value, sourceType, targetType, ex.Message), ex);
		}

		#endregion
	}
}
