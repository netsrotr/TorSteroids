#region Version Info Header
/*
 * $Id: DateTimeExt.cs 87189 2013-08-29 13:21:12Z unknown $
 * $HeadURL: https://torsteroids.svn.codeplex.com/svn/trunk/source/TorSteroids.Common/DateTimeExt.cs $
 * Last modified by $Author: unknown $
 * Last modified at $Date: 2013-08-29 15:21:12 +0200 (Do, 29 Aug 2013) $
 * $Revision: 87189 $
 */
#endregion

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using TorSteroids.Common.Resources;

namespace TorSteroids.Common
{
	/// <summary>
	/// Supports DateTime to be able to parse ISO 8601 / RFC2822/RFC822 formatted dates, and more
	/// </summary>
	public static class DateTimeExt
	{
		#region ParseIso8601DateTime(string)

		/// <summary>
		/// Converts an ISO 8601 date to a DateTime object. Helper method needed to
		/// deal with timezone offset since they are unsupported by the (older or other targeted)
		/// .NET Framework(s).
		/// </summary>
		/// <param name="dateTime">DateTime string</param>
		/// <returns>
		/// DateTime instance with time converted to Universal Time (UTC)
		/// </returns>
		/// <exception cref="FormatException">On format errors parsing the <paramref name="dateTime" /></exception>
		/// <exception cref="ArgumentNullException">If <paramref name="dateTime" /> is null</exception>
		/// <exception cref="ArgumentOutOfRangeException">If <paramref name="dateTime" /> is empty</exception>
		/// <remarks>
		/// See also W3C note at: http://www.w3.org/TR/NOTE-datetime
		/// </remarks>
		public static DateTime ParseIso8601DateTime([NotNull] string dateTime)
		{
			dateTime.ExceptionIfNullOrEmpty("dateTime");

			const int notFound = -1;
			var original = dateTime;

			DateTime toReturn;

			//strip trailing 'Z' since we assume UTC
			if (dateTime.EndsWithOrdinal("Z"))
			{
				if (DateTime.TryParseExact(dateTime, "o", CultureInfo.InvariantCulture,
				                           DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.AssumeUniversal, out toReturn))
					return toReturn;

				//strip trailing 'Z' since we assume UTC
				dateTime = dateTime.Substring(0, dateTime.Length - 1);
			}

			var timeZoneOffset = new TimeSpan(0);
			int timeIndex = dateTime.IndexOfOrdinal(":");

			if (timeIndex != notFound)
			{
				string offsetOp = "-";

				int tzoneIndex = dateTime.IndexOfOrdinal(offsetOp, timeIndex);

				if (tzoneIndex == notFound)
				{
					offsetOp = "+";
					tzoneIndex = dateTime.IndexOfOrdinal(offsetOp, timeIndex);

					if (tzoneIndex == notFound)
					{
						offsetOp = null;
					}
				}

				if (offsetOp != null)
				{
					timeZoneOffset = GetTimeZoneOffset(offsetOp, dateTime, tzoneIndex, original);
					dateTime = dateTime.Substring(0, tzoneIndex);
				}
			}

			string[] masks = new[]
				{
					@"yyyy-MM-dd\THH:mm",
					@"yyyy-MM-dd\THH:mm:ss",
					@"yyyy-MM-dd\THH:mm:ss.fff",
					@"yyyy-MM-dd\THH:mm:ss fff"
				};

			if (DateTime.TryParseExact(dateTime, masks, CultureInfo.InvariantCulture,
			                           DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.RoundtripKind, out toReturn))
				return toReturn.Add(timeZoneOffset);

			return DateTime.Parse(dateTime, CultureInfo.InvariantCulture,
			                      DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.RoundtripKind)
			               .Add(timeZoneOffset);


		}

		#region private

		private static TimeSpan GetTimeZoneOffset(string offsetOp, string datetime, int tzoneIndex, string original)
		{
			string[] offset = datetime.Substring(tzoneIndex + 1).Split(new[] {':'});

			// just fix a common issue with timezone specified
			// as "-0300", not as defined by http://www.w3.org/TR/NOTE-datetime format 
			// TZD  = time zone designator (Z or +hh:mm or -hh:mm)
			if (offset.Length == 1 && offset[0].Length == 4)
			{
				string[] offset2 = new string[2];
				offset2[0] = offset[0].Substring(0, 2);
				offset2[1] = offset[0].Substring(2);
				offset = offset2;
			}

			try
			{
				TimeSpan toReturn = new TimeSpan(Int32.Parse(offset[0]), Int32.Parse(offset[1]), 0);

				if (offsetOp == "+")
					return toReturn.Negate();

				return toReturn;
			}
			catch (IndexOutOfRangeException)
			{
				throw new FormatException(SR.RFC2822InvalidTimezoneFormatExceptionMessage.FormatWith(original));
			}
		}

		#endregion

		#endregion

		#region ParseRfc2822DateTime(string)
		
		/// <summary>
		/// Parse is able to parse RFC2822/RFC822 formatted dates.
		/// DateTime instance with date and time converted to Universal Time (UTC)
		/// is returned on success.
		/// </summary>
		/// <param name="dateTime">DateTime String to parse</param>
		/// <returns>
		/// DateTime instance with time converted to Universal Time (UTC)
		/// </returns>
		/// <exception cref="FormatException">On format errors parsing the <paramref name="dateTime"/> if it match RFC 2822 but another unexpected error occurs, or the <paramref name="dateTime"/> could not be parsed</exception>
		/// <exception cref="ArgumentNullException">If <paramref name="dateTime"/> is null</exception>
		/// <exception cref="ArgumentOutOfRangeException">If <paramref name="dateTime"/> is empty</exception>
		public static DateTime ParseRfc2822DateTime([NotNull] string dateTime)
		{
			dateTime.ExceptionIfNullOrEmpty("dateTime");

			Match match = Rfc2822.Match(dateTime);
			if (!match.Success)
			{
				// try sanitized:
				var sanitized = StripRfcComments(StripTabsAndNewlines(dateTime));
				match = Rfc2822.Match(sanitized);
				if (!match.Success)
				{
					// try german:
					match = Rfc2822_de.Match(dateTime);
					if (!match.Success)
					{
						// try sanitized german:
						match = Rfc2822_de.Match(sanitized);
					}
				}
			}

			if (match.Success)
			{
				try
				{
					var dd = Int32.Parse(match.Groups[1].Value);
					
					// allow to be case insensitive. We also test for german names (they are common here ;-)
					var monthStr = match.Groups[2].Value.ToUpper();
					int mth;
					if (!Months.TryGetValue(monthStr, out mth))
						mth = Months_De[monthStr];

					var yy = Int32.Parse(match.Groups[3].Value);
					// following year completion is compliant with RFC 2822.
					yy = (yy < 50 ? 2000 + yy : (yy < 1000 ? 1900 + yy : yy));
					var hh = Int32.Parse(match.Groups[4].Value, CultureInfo.InvariantCulture);
					var mm = Int32.Parse(match.Groups[5].Value, CultureInfo.InvariantCulture);
					var ss = Int32.Parse("0" + match.Groups[6].Value, CultureInfo.InvariantCulture); // optional (may get lenght zero)
					var zone = match.Groups[7].Value;

					var xd = new DateTime(yy, mth, dd, hh, mm, ss, DateTimeKind.Utc);
					var zoneValue = RfcTimeZoneToGMTBias(zone);
					return xd.Add(zoneValue);
				}
				catch (Exception e)
				{
					throw new FormatException(SR.RFC2822ParseGroupsExceptionMessage.FormatWith(e.GetType().Name, dateTime), e);
				}
			}

			try
			{
				// fallback, if regex does not match:
				return DateTime.Parse(dateTime, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal);
			}
			catch (Exception ex)
			{
				throw new FormatException(SR.RFC2822GeneralParseDateTimeExceptionMessage.FormatWith(ex.Message, dateTime), ex);
			}
		}

		#region private

		
		private static readonly Dictionary<string, int> Months = new Dictionary<string, int>
			(12, StringComparer.Ordinal)
			{  
				{"JAN", 1}, {"FEB", 2}, {"MAR", 3}, {"APR", 4}, {"MAY", 5}, {"JUN", 6},
				{"JUL", 7}, {"AUG", 8}, {"SEP", 9}, {"OCT",10}, {"NOV",11}, {"DEC",12} 
			};

		private static readonly Dictionary<string, int> Months_De = new Dictionary<string, int>
			(12, StringComparer.Ordinal)
			{  
				{"JAN", 1}, {"FEB", 2}, {"MÄR", 3}, {"APR", 4}, {"MAI", 5}, {"JUN", 6},
				{"JUL", 7}, {"AUG", 8}, {"SEP", 9}, {"OKT",10}, {"NOV",11}, {"DEZ",12} 
			};

		private const int MaxTimeZones = 35;

		// http://www.w3.org/Protocols/rfc822/#z28 or http://www.faqs.org/rfcs/rfc822.html
		// http://www.ietf.org/rfc/rfc2822.txt?number=2822

		/// <summary>
		/// Time zone to bias hours map
		/// </summary>
		private static readonly Dictionary<string, int> ZoneBias = new Dictionary<string, int>
			(MaxTimeZones, StringComparer.Ordinal)
			{
				{"GMT", 0}, {"UT", 0}, {"EST", -5}, {"EDT", -4}, {"CST", -6}, {"CDT", -5},
				{"MST", -7}, {"MDT", -6}, {"PST", -8}, {"PDT", -7},
				// old military: problematic - see http://tools.ietf.org/html/rfc1123#page-55
				// these are correct: http://www.timeanddate.com/library/abbreviations/timezones/
				{"Z", 0}, {"A", 1}, {"B", 2}, {"C", 3}, {"D", 4}, {"E", 5}, {"F", 6}, 
				{"G", 7}, {"H", 8}, {"I", 9}, /* J is not used */ {"K", 10}, {"L", 11}, {"M", 12}, 
				{"N", -1}, {"O", -2}, {"P", -3}, {"Q", -4}, {"R", -5},{"S", -6}, 
				{"T", -7}, {"U", -8}, {"V", -9},{"W", -10}, {"X", -11},{"Y", -12}
			};
		
		/// <summary>
		/// Time zone abbreviations to bias hours map
		/// </summary>
		/// <remarks>
		/// We do NOT consider non-uique time zone abbreviations, like "ADT", "AMST" or "AMT"...
		/// except "CST" / "EDT" - where we use North/Central American Time and Eastern Daylight Time 
		/// - that are mostly common;
		/// see http://www.timeanddate.com/library/abbreviations/timezones/ !!!
		/// </remarks>
		private static readonly Dictionary<string, double> ZoneAbbrBias = new Dictionary<string, double>
			(201, StringComparer.Ordinal)
			{
				{"ACDT", 10.5},  {"ACST", 9.5},  {"AEDT", 11},  {"AEST", 10},  {"AFT", 4.5},  {"AKDT", -8},      {"AKST", -9},
				{"ALMT", 6},     {"ANAST", 12},  {"ANAT", 12},  {"AQTT", 5},   {"ART", -3},   {"AWDT", 9},       {"AWST", 8},
				{"AZOST", 0},    {"AZOT", -1},   {"AZST", 5},   {"AZT", 4},
				{"BNT", 8},      {"BOT", -4},    {"BRST", -2},  {"BRT", -3},   {"BTT", 6},
				{"CAST", 8},     {"CAT", 2},     {"CCT", 6.5},  {"CEST", 2},   {"CET", 1},    {"CHADT", 13.75},  {"CHAST", 12.75},
				{"CKT", -10},    {"CLST", -3},   {"CLT", -4},   {"COT", -5},   {"CVT", -1},   {"CXT", 7},        {"CHST", 10},
				{"DAVT", 7},     {"EASST", -5},  {"EAST", -6},  {"EAT", 3},    {"ECT", -5},   {"EEST", 3},
				{"EET", 2},      {"EGST", 0},    {"EGT", -1},   {"ET", -5}, 
				{"FJST", 13},    {"FJT", 12},    {"FKST", -3},  {"FKT", -4},   {"FNT", -2}, 
				{"GALT", -6},    {"GAMT", -9},   {"GET", 4},    {"GFT", -3},   {"GILT", 12},  {"GST", 4},        {"GYT", -4}, 
				{"HAA", -3},     {"HAC", -5},    {"HADT", -9},  {"HAE", -4},   {"HAP", -7},   {"HAR", -6},       {"HAST", -10},     {"HAT", -2.5}, 
				{"HAY", -8},     {"HKT", 8},     {"HLV", -4.5}, {"HNA", -4},   {"HNC", -6},   {"HNE", -5},       {"HNP", -8},       {"HNR", -7}, 
				{"HNT", -3.5},   {"HNY", -9},    {"HOVT", 7},   {"ICT", 7},    {"IDT", 3},    {"IOT", 6},        {"IRDT", 4.5},     {"IRKST", 9}, 
				{"IRKT", 9},     {"IRST", 3.5},  {"JST", 9},    {"KGT", 6},    {"KRAST", 8},  {"KRAT", 8},       {"KST", 9},        {"KUYT", 4}, 
				{"LHDT", 11},    {"LHST", 10.5}, {"LINT", 14},  {"MAGST", 12}, {"MAGT", 12},  {"MART", -9.5},    {"MAWT", 5},      
				{"MESZ", 2},     {"MEZ", 1},     {"MHT", 12},   {"MMT", 6.5},  {"MSD", 4},    {"MSK", 4},        {"MUT", 4}, 
				{"MVT", 5},      {"MYT", 8},     {"NCT", 11},   {"NDT", -2.5}, {"NFT", 11.5}, {"NOVST", 7},      {"NOVT", 6},       {"NPT", 5.75}, 
				{"NST", -3.5},   {"NUT", -11},   {"NZDT", 13},  {"NZST", 12},  {"OMSST", 7},  {"OMST", 7}, 
				{"PET", -5},     {"PETST", 12},  {"PETT", 12},  {"PGT", 10},   {"PHOT", 13},  {"PHT", 8},        {"PKT", 5}, 
				{"PMDT", -2},    {"PMST", -3},   {"PONT", 11},  {"PT", -8},    {"PWT", 9},    {"PYST", -3},      {"PYT", -4}, 
				{"RET", 4},      {"SAMT", 4},    {"SAST", 2},   {"SBT", 11},   {"SCT", 4},    {"SGT", 8},        {"SRT", -3},       {"SST", -11}, 
				{"TAHT", -10},   {"TFT", 5},     {"TJT", 5},    {"TKT", 13},   {"TLT", 9},    {"TMT", 5},        {"TVT", 12},    
				{"ULAT", 8},     {"UYST", -2},   {"UYT", -3},   {"UZT", 5},    {"VET", -4.5}, {"VLAST", 11},     {"VLAT", 11},      {"VUT", 11}, 
				{"WAST", 2},     {"WAT", 1},     {"WEST", 1},   {"WESZ", 1},   {"WET", 0},    {"WEZ", 0},        {"WFT", 12},       {"WGST", -2}, 
				{"WGT", -3},     {"WIB", 7},     {"WIT", 9},    {"WITA", 8},   {"WT", 0},     
				{"YAKST", 10},   {"YAKT", 10},   {"YAPT", 10},  {"YEKST", 6},  {"YEKT", 6} 
			};

		// we do NOT consider non-uique time zone abbreviations, like "ADT", "AMST" or "AMT"...
		// see http://www.timeanddate.com/library/abbreviations/timezones/ !!!
		private static readonly Regex Rfc2822 =
			new Regex(
				@"\s*(?:(?:Mon|Tue|Wed|Thu|Fri|Sat|Sun)\s*,\s*)?(\d{1,2})" +
				@"\s+(Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep|Oct|Nov|Dec)\s+(\d{2,})\s+(\d{1,2})\s*:\s*(\d{2})\s*(?::\s*(\d{2}))?" +
				@"\s*([+\-]\d{4}|" +
				String.Join("|", ZoneAbbrBias.Keys.ToArray()) + "|" +
				String.Join("|", ZoneBias.Keys.ToArray()) +
				")",
				RegexOptions.Singleline | RegexOptions.IgnoreCase);

		private static readonly Regex Rfc2822_de =
			new Regex(
				@"\s*(?:(?:Mo|Di|Mi|Do|Fr|Sa|So)\s*,\s*)?(\d{1,2})" +
				@"\s+(Jan|Feb|Mär|Apr|Mai|Jun|Jul|Aug|Sep|Okt|Nov|Dez)\s+(\d{2,})\s+(\d{1,2})\s*:\s*(\d{2})\s*(?::\s*(\d{2}))?" +
				@"\s*([+\-]\d{4}|" +
				String.Join("|", ZoneAbbrBias.Keys.ToArray()) + "|" +
				String.Join("|", ZoneBias.Keys.ToArray()) +
				")",
				RegexOptions.Singleline | RegexOptions.IgnoreCase);


		private static TimeSpan RfcTimeZoneToGMTBias(string zoneString)
		{
			string zone = zoneString.Trim();

			if (zone.IndexOfAny(new[] { '+', '-' }) == 0)
			{
				// +hhmm format
				bool toNegate = zone.Substring(0, 1) == "+";
				zone = zone.Substring(1).PadRight(4, '0');
				var hh = Math.Min(23, Int32.Parse(zone.Substring(0, 2), CultureInfo.InvariantCulture));
				var mm = Math.Min(59, Int32.Parse(zone.Substring(2, 2), CultureInfo.InvariantCulture));

				if (toNegate)
					return new TimeSpan(hh, mm, 0).Negate();
				return new TimeSpan(hh, mm, 0);
			}

			// named format

			int biasHours;
			if (ZoneBias.TryGetValue(zone.ToUpper(), out biasHours))
				return TimeSpan.FromHours(biasHours).Negate();

			double biasHourMinutes;
			if (ZoneAbbrBias.TryGetValue(zone.ToUpper(), out biasHourMinutes))
				return TimeSpan.FromHours(biasHourMinutes).Negate();

			return new TimeSpan(0, 0, 0);
		}

		private static string StripTabsAndNewlines(string value)
		{
			return value.Replace("\t", " ").Replace("\r", "").Replace("\n", " ");
		}

		private static string StripRfcComments(string value)
		{
			if (!value.Contains("("))
				return value;

			int comments = 0;
			bool ignoreNext = false, previousWasSpace = false;
			StringBuilder sb = new StringBuilder(value.Length);

			var length = value.Length;

			for (int i = 0; i < length; i++)
			{
				if (ignoreNext)
				{
					ignoreNext = false;
					continue;
				}

				var c = value[i];

				if (Equals(c, '\\'))
				{
					if ( (length > i + 1) && (Equals(value[i + 1], '(') || Equals(value[i + 1], ')')))
					{
						ignoreNext = true;
					}

					continue;
				}

				if (Equals(c, '('))
				{
					comments++;
					continue;
				}

				if (Equals(c, ')'))
				{
					comments--;
					if (comments == 0)
					{
						// http://www.faqs.org/rfcs/rfc822.html
						// When a comment acts as the delimiter  between a sequence of two lexical symbols, such as two atoms, 
						// it is lex-ically equivalent with a single SPACE,  for  the  purposes  ofregenerating  the  sequence
						if (!previousWasSpace && ((length > i + 1) && !Equals(value[i + 1], ' ')))
							sb.Append(" ");
					}

					continue;
				}

				if (comments == 0)
				{
					previousWasSpace = (i > 0 && Equals(value[i - 1], ' '));
					sb.Append(c);
				}
			}

			return sb.ToString();
		}

		#endregion

		#endregion

	}	
}
