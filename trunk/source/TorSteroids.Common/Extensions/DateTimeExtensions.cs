#region Version Info Header
/*
 * $Id: DateTimeExtensions.cs 89862 2016-07-19 12:20:09Z unknown $
 * $HeadURL: https://torsteroids.svn.codeplex.com/svn/trunk/source/TorSteroids.Common/Extensions/DateTimeExtensions.cs $
 * Last modified by $Author: unknown $
 * Last modified at $Date: 2016-07-19 14:20:09 +0200 (Di, 19 Jul 2016) $
 * $Revision: 89862 $
 */
#endregion

// ReSharper disable CheckNamespace
namespace System
// ReSharper restore CheckNamespace
{
	public static class DateTimeExtensions
	{
		/// <summary>
		/// Returns the date part as integer in the format YYYYMMDD.
		/// </summary>
		/// <param name="dateTime">DateTime (only date part is relevant)</param>
		/// <returns>Int32</returns>
		public static int DateToInteger(this DateTime dateTime)
		{
			return dateTime.Year * 10000 + dateTime.Month * 100 + dateTime.Day;
		}

		/// <summary>
		/// Returns the time part as an integer in the format HHmmssfff.
		/// </summary>
		/// <param name="dateTime">DateTime (only time part is relevant)</param>
		/// <param name="convertToUniversalTime">Set this to true, if the provided time
		/// is not already a UTC time, else false.
		/// If it is true, a conversion to UniversalTime will happen! Default is: false</param>
		/// <returns>integer</returns>
		public static int TimeToInteger(this DateTime dateTime, bool convertToUniversalTime = false)
		{
			if (convertToUniversalTime)
				dateTime = dateTime.ToUniversalTime();
			return dateTime.Hour * 10000000 + dateTime.Minute * 100000 + dateTime.Second * 1000 + dateTime.Millisecond;
		}

		static DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

		/// <summary>
		/// Converts a Unix time expressed as the number of seconds that have elapsed 
		/// since 1970-01-01T00:00:00Z to a DateTime value.
		/// </summary>
		/// <param name="unixTimeSeconds">A Unix time, expressed as the number of seconds that have elapsed 
		/// since 1970-01-01T00:00:00Z (January 1, 1970, at 12:00 AM UTC). 
		/// For Unix times before this date, its value is negative.</param>
		/// <returns>DateTime. A date and time value that represents the same moment in time as the Unix time</returns>
		public static DateTime FromUnixTimeSeconds(long unixTimeSeconds)
		{
			var dtDateTime = UnixEpoch.AddSeconds(unixTimeSeconds).ToUniversalTime();
			return dtDateTime;
		}

		/// <summary>
		/// Converts a Unix time expressed as the number of seconds that have elapsed 
		/// since 1970-01-01T00:00:00Z to a DateTime value.
		/// </summary>
		/// <param name="unixTimeSeconds">A Unix time, expressed as the number of seconds that have elapsed 
		/// since 1970-01-01T00:00:00Z (January 1, 1970, at 12:00 AM UTC). 
		/// For Unix times before this date, its value is negative.</param>
		/// <returns>DateTime. A date and time value that represents the same moment in time as the Unix time</returns>
		public static DateTime FromUnixTimeSeconds(this DateTime dateTime, long unixTimeSeconds)
		{
			return FromUnixTimeSeconds(unixTimeSeconds);
		}

		/// <summary>
		/// Returns the number of seconds that have elapsed since 1970-01-01T00:00:00Z.
		/// </summary>
		/// <param name="dateTime">The date time to convert.</param>
		/// <returns>System.Int64. The number of seconds that have elapsed since 1970-01-01T00:00:00Z.</returns>
		/// <remarks>
		/// <para>
		/// Unix time represents the number of seconds that have elapsed since 1970-01-01T00:00:00Z (January 1, 1970, at 12:00 AM UTC). 
		/// It does not take leap seconds into account.
		/// </para>
		/// <para>
		/// This method first converts the current instance to UTC before returning its Unix time.
		/// For date and time values before 1970-01-01T00:00:00Z, this method returns a negative value.
		/// </para>
		/// </remarks>
		public static long ToUnixTimeSeconds(this DateTime dateTime)
		{
			return Convert.ToInt64((dateTime.ToUniversalTime() - UnixEpoch).TotalSeconds);
		}

	}
}