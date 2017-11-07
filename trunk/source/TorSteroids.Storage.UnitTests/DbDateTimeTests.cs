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
using NUnit.Framework;
using TorSteroids.Storage.Data;

namespace TorSteroids.Storage.UnitTests
{
	/// <summary>
	/// DbDateTime Tests
	/// </summary>
	[TestFixture]
	public class DbDateTimeTests
	{
		#region Parse Date Strings

		[Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void ParseDateStringsSpacesShouldFail()
		{
			DbDateTime.ParseDate(" ");
		}
		
		[Test, ExpectedException(typeof(FormatException))]
		public void ParseDateStringsAlphaNumericInputShouldFail()
		{
			DbDateTime.ParseDate("hä?");
		}

		[Test, ExpectedException(typeof(FormatException))]
		public void ParseDateStringsNegativeNumberInputShouldFail()
		{
			DbDateTime.ParseDate("-123");
		}

		[Test, ExpectedException(typeof(FormatException))]
		public void ParseDateStringsInvalidDateNumberInputShouldFail()
		{
			DbDateTime.ParseDate("19994523"); // 45 months
		}

		[Test, ExpectedException(typeof(FormatException))]
		public void ParseDateStringsInvalidDateLeapYearDayInputShouldFail()
		{
			DbDateTime.ParseDate("20010229"); // Feb., 29th 2001 ???
		}

		[Test]
		public void ParseDateStringsValidDateLeapYearDayInput()
		{
			Assert.AreEqual(new DbDate(2000,2,29), DbDateTime.ParseDate("20000229")); // Feb., 29th 2000 !!!
		}

		[Test]
		public void ParseDateStrings()
		{
			Assert.AreEqual(DbDate.MinValue, DbDateTime.ParseDate("0")); // 0 - gets minimum value
			Assert.AreEqual(DbDate.MinValue, DbDateTime.ParseDate("1")); // 1 - gets minimum value
			Assert.AreEqual(DbDate.MinValue, DbDateTime.ParseDate("423")); // dito
			Assert.AreEqual(DbDate.MinValue, DbDateTime.ParseDate("10101")); // dito
			Assert.AreEqual(10102, DbDateTime.ParseDate("10102").Ticks);
			Assert.AreEqual(19660630, DbDateTime.ParseDate("19660630").Ticks); // 
			Assert.AreEqual(770401, DbDateTime.ParseDate("770401").Ticks); // April fool at 77 a.D. 

			var dbTime = DbDateTime.ParseDate("19660630");
			Assert.AreEqual(1966, dbTime.Year);
			Assert.AreEqual(6, dbTime.Month);
			Assert.AreEqual(30, dbTime.Day);

			dbTime = DbDateTime.ParseDate("770401");
			Assert.AreEqual(77, dbTime.Year);
			Assert.AreEqual(4, dbTime.Month);
			Assert.AreEqual(1, dbTime.Day);
		}

		#endregion

		#region Parse Time Strings

		[Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void ParseTimeStringsSpacesShouldFail()
		{
			DbDateTime.ParseTime(" ");
		}

		[Test, ExpectedException(typeof (FormatException))]
		public void ParseTimeStringsAlphaNumericInputShouldFail()
		{
			DbDateTime.ParseTime("hä?");
		}

		[Test, ExpectedException(typeof (FormatException))]
		public void ParseTimeStringsNegativeNumberInputShouldFail()
		{
			DbDateTime.ParseTime("-123");
		}

		[Test, ExpectedException(typeof(FormatException))]
		public void ParseTimeStringsInvalidTimeNumberInputShouldFail()
		{
			DbDateTime.ParseTime("9739123"); // 00:97:39.123
		}

		[Test]
		public void ParseTimeStringsFullStyle()
		{
			Assert.AreEqual(1, DbDateTime.ParseTime("1").Ticks); // 1 msec
			Assert.AreEqual(423, DbDateTime.ParseTime("423").Ticks);
			Assert.AreEqual(423, DbDateTime.ParseTime("0423").Ticks);
			Assert.AreEqual(57423, DbDateTime.ParseTime("57423").Ticks); // 57 sec, 423 msecs
			Assert.AreEqual(110423, DbDateTime.ParseTime("110423").Ticks); // 1 min, 10 sec, 423 msecs

			var dbTime = DbDateTime.ParseTime("110423");
			Assert.AreEqual(0, dbTime.Hour);
			Assert.AreEqual(1, dbTime.Minute);
			Assert.AreEqual(10, dbTime.Second);
			Assert.AreEqual(423, dbTime.Millisecond);
			Assert.AreEqual(DateTimeKind.Utc, dbTime.Kind);

			dbTime = DbDateTime.ParseTime("50110423", kind: DateTimeKind.Local);
			Assert.AreEqual(5, dbTime.Hour);
			Assert.AreEqual(1, dbTime.Minute);
			Assert.AreEqual(10, dbTime.Second);
			Assert.AreEqual(423, dbTime.Millisecond);
			Assert.AreEqual(DateTimeKind.Local, dbTime.Kind);
		}

		[Test]
		public void ParseTimeStringsWithoutMillisecsStyle()
		{
			var style = DbTimeStyle.WithoutMillisecond;

			Assert.AreEqual(1000, DbDateTime.ParseTime("1", style: style).Ticks); // 1 sec
			Assert.AreEqual(423000, DbDateTime.ParseTime("423", style: style).Ticks); // 4 min, 23 sec
			Assert.AreEqual(423000, DbDateTime.ParseTime("0423", style: style).Ticks);
			Assert.AreEqual(70423000, DbDateTime.ParseTime("70423", style: style).Ticks); // 7 h, 4 min, 23 secs
			Assert.AreEqual(110423000, DbDateTime.ParseTime("110423", style: style).Ticks); // 11 h, 4 min, 23 secs

			var dbTime = DbDateTime.ParseTime("110423", style: style);
			Assert.AreEqual(11, dbTime.Hour);
			Assert.AreEqual(4, dbTime.Minute);
			Assert.AreEqual(23, dbTime.Second);
			Assert.AreEqual(0, dbTime.Millisecond);
			Assert.AreEqual(DateTimeKind.Utc, dbTime.Kind);

			dbTime = DbDateTime.ParseTime("0", style: style, kind: DateTimeKind.Unspecified);
			Assert.AreEqual(0, dbTime.Hour);
			Assert.AreEqual(0, dbTime.Minute);
			Assert.AreEqual(0, dbTime.Second);
			Assert.AreEqual(0, dbTime.Millisecond);
			Assert.AreEqual(DateTimeKind.Unspecified, dbTime.Kind);
		}

		

		#endregion

		[Test]
		public void ToDateTime()
		{
			var utcDt = DateTime.MinValue.ToUniversalTime();
			Assert.AreEqual(utcDt, DbDateTime.ToDateTime(DbDate.MinValue, DbTime.MinValue));

			utcDt = new DateTime(2013, 2, 20, 13, 24, 56, 0, DateTimeKind.Utc);
			Assert.AreEqual(utcDt, DbDateTime.ToDateTime(new DbDate(2013, 2, 20), new DbTime(13, 24, 56, 0, DateTimeKind.Utc)));

			// using DateTime.Now here get non-Equal results (internal datedata are different, but the ticks and kind are equal)
			var localDt = new DateTime(2013, 2, 20, 14, 24, 56, 0, DateTimeKind.Local);
			
			// provide a local datetime
			var res = DbDateTime.ToDateTime(
				new DbDate(localDt.Year, localDt.Month, localDt.Day),
				new DbTime(localDt.Hour, localDt.Minute, localDt.Second, localDt.Millisecond, localDt.Kind));
			
			Assert.AreEqual(localDt, res);

			Assert.AreEqual(localDt, DbDateTime.ToDateTime((DbDate)localDt, (DbTime)localDt));

		}

		[Test]
		public void ToDbDateExtension()
		{
			var utcDt = DateTime.MinValue.ToUniversalTime();
			Assert.AreEqual(DbDate.MinValue, utcDt.ToDbDate());

			// force a timezone offset of one hour ahead:
			var localDt = new DateTimeOffset(2013, 2, 27, 0, 30, 55, TimeSpan.FromHours(1)).DateTime;
			// this should now be Utc (the day before):
			Assert.AreEqual(new DbDate(2013, 2, 26), localDt.ToDbDate());

		}

		[Test]
		public void ToDbTimeExtension()
		{
			var utcDt = DateTime.MinValue.ToUniversalTime();
			Assert.AreEqual(DbTime.MinValue, utcDt.ToDbTime());

			// force a timezone offset of one hour ahead:
			var localDt = new DateTimeOffset(2013, 2, 27, 0, 30, 55, TimeSpan.FromHours(1)).DateTime;
			// this should now be Utc (the day/hour before):
			Assert.AreEqual(new DbTime(23, 30, 55, DateTimeKind.Utc), localDt.ToDbTime());
		}
	}
}
