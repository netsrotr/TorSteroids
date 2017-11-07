using System;
using System.Globalization;
using System.Linq;
using TorSteroids.Common;
using TorSteroids.Storage.Resources;

namespace TorSteroids.Storage.Data
{
	public enum DbTimeStyle
	{
		Full,
		WithoutMillisecond
	}

    public static class DbDateTime
	{

		#region public static Parse...()

		/// <summary>
		/// Able to parse a date string with these formats:
		///   'YYYYMMDD', 'YYYMMDD', 'YYMMDD' and 'YMMDD'.
		/// </summary>
		/// <param name="dateString">The string to be parsed</param>
		/// <returns>DbDate instance</returns>
		/// <remarks>
		/// If it's length is lower/equal 4, DbDate.MinValue is returned.
		/// Special case: 'YMMDD' as of 10101 will return DbDate.MinValue.
		/// </remarks>
		/// <exception cref="ArgumentNullException">If <paramref name="dateString"/> is null</exception>
		/// <exception cref="ArgumentOutOfRangeException">If <paramref name="dateString"/> is empty</exception>
		/// <exception cref="FormatException">If <paramref name="dateString"/> cannot be parsed as a DbDate</exception>
		public static DbDate ParseDate(string dateString)
		{
			dateString.ExceptionIfNullOrTrimmedEmpty("dateString");
			
			string dts = dateString.Trim();

			if (!dts.All(Char.IsDigit))
				throw new FormatException(SR.DbDateTimeAcceptsDigitsOnlyFormatException);

			if (dts.Length < 5) // Minimum Db Date is: YMMDD
				return DbDate.MinValue;
			
			if (dts == "10101") // year 1, 1th jan is Minimum Db Date 
				return DbDate.MinValue;

			dts = dts.PadLeft(8, '0');	// fill in missing leading zeros

			if (dts.Length == 8) // Db Date: YYYYMMDD
			{
				try
				{
					return new DbDate(Int32.Parse(dts.Substring(0, 4)),
									  Int32.Parse(dts.Substring(4, 2)),
									  Int32.Parse(dts.Substring(6, 2)));
				}
				catch (Exception ex)
				{
					throw new FormatException(ex.Message, ex);
				}
			}

			throw new FormatException(SR.DbDateInvalidFormatException);
		}

		/// <summary>
		/// Parses the specified time string with these formats:
		/// 'HMMss', 'HHMMss', 'HMMssfff' and 'HHMMssfff'.
		/// </summary>
		/// <param name="timeString">The time string.</param>
		/// <param name="kind">The <see cref="DateTimeKind" /> kind. Default: Utc</param>
		/// <param name="style">The <see cref="DbTimeStyle"/> style. Default: Full</param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException">If <paramref name="timeString"/> is null</exception>
		/// <exception cref="ArgumentOutOfRangeException">If <paramref name="timeString"/> is empty</exception>
		/// <exception cref="FormatException">If <paramref name="timeString"/> cannot be parsed as a DbDate</exception>
		public static DbTime ParseTime(string timeString,  
			DateTimeKind kind = DateTimeKind.Utc, DbTimeStyle style = DbTimeStyle.Full)
	    {
			timeString.ExceptionIfNullOrTrimmedEmpty("timeString");
			
			string dts = timeString.Trim();

			if (!dts.All(Char.IsDigit))
				throw new FormatException(SR.DbDateTimeAcceptsDigitsOnlyFormatException);

			// always fill with leading zeros to get around the missing digits parsing an int.
			// HMMSS is the parsed minimum
			dts = dts.PadLeft(6, '0'); 
			
			if (style != DbTimeStyle.Full) // HHMMss
				dts = dts.PadRight(9, '0');
			
			dts = dts.PadLeft(9, '0');	// fill in missing leading zeros

		    if (dts.Length == 9)
			{
				// Time: HHMMSSmmm 
				int hour = Int32.Parse(dts.Substring(0, 2));
				int minute = Int32.Parse(dts.Substring(2, 2));
				int second = Int32.Parse(dts.Substring(4, 2));
				int millisecond = Int32.Parse(dts.Substring(6, 3));
				try
				{
					return new DbTime(hour, minute, second, millisecond, kind);
				}
				catch (Exception ex)
				{
					throw new FormatException(ex.Message, ex);
				}
			}

			throw new FormatException(SR.DbTimeInvalidFormatException);
		}


		/// <summary>
		/// Parses a date time string.
		/// 
		/// Is able to parse a date string with these formats:
		///   - 'YYYYMMDD', 'YYYMMDD', 'YYMMDD' and 'YMMDD' (<seealso cref="ParseDate(string)"/>,
		///   - CLR DateTime formats,
		///   - and ISO 8601 formats.
		/// </summary>
		/// <param name="dateTimeString">The date time string.</param>
		/// <param name="kind">The <see cref="DateTimeKind"/> we expect to be parsed.</param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException">If <paramref name="dateTimeString"/> is null</exception>
		/// <exception cref="ArgumentOutOfRangeException">If <paramref name="dateTimeString"/> is empty</exception>
		/// <exception cref="FormatException">If <paramref name="dateTimeString"/> cannot be parsed as a DateTime</exception>
		public static DateTime ParseDateTime(string dateTimeString,
			DateTimeKind kind = DateTimeKind.Utc)
		{
			dateTimeString.ExceptionIfNullOrTrimmedEmpty("dateTimeString");

			string dts = dateTimeString.Trim();

			if (dts == "0")
				return DateTime.MinValue;

			if (dts.Length <= 8 && dts.All(Char.IsDigit)) // Full DbDate format: YYYYMMDD
			{
				return ParseDate(dateTimeString).ToDateTime(kind);
			}

			var styles = DateTimeStyles.AllowWhiteSpaces;
			if (kind == DateTimeKind.Utc)
				styles |= DateTimeStyles.AssumeUniversal;

			DateTime clrDateTime;
			if (DateTime.TryParse(dts, CultureInfo.InvariantCulture, styles, out clrDateTime))
			{
				return clrDateTime;
			}

			// last try: the common ISO 8601:
			return DateTimeExt.ParseIso8601DateTime(dateTimeString);
		}

	    #endregion

		/// <summary>
		/// Combine the date and time to a DateTime.
		/// </summary>
		/// <param name="date">The date.</param>
		/// <param name="time">The time.</param>
		/// <returns></returns>
		public static DateTime ToDateTime(DbDate date, DbTime time)
		{
			if (date.Equals(DbDate.MinValue))
			{
				var minDate = new DateTime(DateTime.MinValue.Year, DateTime.MinValue.Month, DateTime.MinValue.Day,
						time.Hour, time.Minute, time.Second, time.Millisecond, time.Kind);
				return minDate;
			} 

			var result = new DateTime(date.Year, date.Month, date.Day,
					time.Hour, time.Minute, time.Second, time.Millisecond, time.Kind);

			return result;
		}

		
    }
}
