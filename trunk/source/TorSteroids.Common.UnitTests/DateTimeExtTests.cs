using System;
using NUnit.Framework;

namespace TorSteroids.Common.UnitTests
{
	/// <summary/> 
	[TestFixture]
	public class DateTimeExtTests
	{

		/// <summary/> 
		[Test, ExpectedException(typeof(ArgumentNullException))]
		public void ParseIso8601DateTimeNullStringExpectArgumentNullException()
		{
			DateTimeExt.ParseIso8601DateTime(null);
		}

		/// <summary/> 
		[Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void ParseIso8601DateTimeEmptyStringExpectArgumentOutOfRangeException()
		{
			DateTimeExt.ParseIso8601DateTime("");
		}

		
		/// <summary/> 
		[Test]
		public void ParseIso8601DateTimeString()
		{
			// to make them comparable with .Equals(), we define it this way (throw millisecs and higher ticks resolution):
			DateTime myLocalDt = new DateTime(1966, 6, 30, 10, 45, 24, DateTimeKind.Utc);
			string xmlString = myLocalDt.ToString("s");
			DateTime result = DateTimeExt.ParseIso8601DateTime(xmlString);
			Assert.IsTrue(myLocalDt.Equals(result), "DateTimeExt.ParseISO8601DateTime('{0}') failed", xmlString);
			
			DateTime ahead4 = myLocalDt.AddHours(4);
			xmlString = ahead4.ToString("s");
			xmlString = xmlString + "+04:00:00";
			result = DateTimeExt.ParseIso8601DateTime(xmlString);
			Assert.IsTrue(myLocalDt.Equals(result), "DateTimeExt.ParseIso8601DateTime('{0}') failed", xmlString);
			
			DateTime behind4 = myLocalDt.AddHours(-4);
			xmlString = behind4.ToString("s");
			xmlString = xmlString + "-04:00:00";
			result = DateTimeExt.ParseIso8601DateTime(xmlString);
			Assert.IsTrue(myLocalDt.Equals(result), "DateTimeExt.ParseISO8601DateTime('{0}') failed", xmlString);
			
			DateTime dtUtcTime = new DateTime(2004, 6, 5, 5, 20, 9, 0, DateTimeKind.Utc);
			Assert.AreEqual(dtUtcTime, DateTimeExt.ParseIso8601DateTime("2004-06-05T06:20:09+01:00"));
			Assert.AreEqual(dtUtcTime, DateTimeExt.ParseIso8601DateTime("2004-06-05T05:20:09+00:00"));
			Assert.AreEqual(dtUtcTime, DateTimeExt.ParseIso8601DateTime("2004-06-05T05:20:09"));

			// might be with millisecs (not in the SPEC, but who knows...)
			dtUtcTime = new DateTime(2013, 6, 5, 5, 20, 9, 123, DateTimeKind.Utc);
			Assert.AreEqual(dtUtcTime, DateTimeExt.ParseIso8601DateTime("2013-06-05T06:20:09.123+01:00"));
			Assert.AreEqual(dtUtcTime, DateTimeExt.ParseIso8601DateTime("2013-06-05 06:20:09.123+01:00"));
			Assert.AreEqual(dtUtcTime, DateTimeExt.ParseIso8601DateTime("2013-06-05T05:20:09.123+00:00"));
			Assert.AreEqual(dtUtcTime, DateTimeExt.ParseIso8601DateTime("2013-06-05T04:20:09.123-01:00"));
			Assert.AreEqual(dtUtcTime, DateTimeExt.ParseIso8601DateTime("2013-06-05T05:20:09.123-00:00"));
			Assert.AreEqual(dtUtcTime, DateTimeExt.ParseIso8601DateTime("2013-06-05T06:20:09.123+0100"));
			Assert.AreEqual(dtUtcTime, DateTimeExt.ParseIso8601DateTime("2013-06-05T05:20:09.123+0000"));
			Assert.AreEqual(dtUtcTime, DateTimeExt.ParseIso8601DateTime("2013-06-05T04:20:09.123-0100"));
			Assert.AreEqual(dtUtcTime, DateTimeExt.ParseIso8601DateTime("2013-06-05T05:20:09.123-0000"));
			Assert.AreEqual(dtUtcTime, DateTimeExt.ParseIso8601DateTime("2013-06-05T05:20:09.123Z"));
			Assert.AreEqual(dtUtcTime, DateTimeExt.ParseIso8601DateTime("2013-06-05 05:20:09.123Z"));
			Assert.AreEqual(dtUtcTime, DateTimeExt.ParseIso8601DateTime("2013-06-05T05:20:09.123"));
			Assert.AreEqual(dtUtcTime, DateTimeExt.ParseIso8601DateTime("2013-06-05T05:20:09 123"));

			// tests from http://www.w3.org/TR/NOTE-datetime 
			dtUtcTime = new DateTime(1994, 11, 5, 13, 15, 30, 0, DateTimeKind.Utc);

			// November 5, 1994, 8:15:30 am, US Eastern Standard Time.
			Assert.AreEqual(dtUtcTime, DateTimeExt.ParseIso8601DateTime("1994-11-05T08:15:30-05:00"));
			Assert.AreEqual(dtUtcTime, DateTimeExt.ParseIso8601DateTime("1994-11-05T13:15:30Z"));

		}


		/// <summary/> 
		[Test, ExpectedException(typeof(ArgumentNullException))]
		public void ParseRfc2822DateTimeNullStringExpectArgumentNullException()
		{
			DateTimeExt.ParseRfc2822DateTime(null);
		}

		/// <summary/> 
		[Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void ParseRfc2822DateTimeEmptyStringExpectArgumentOutOfRangeException()
		{
			DateTimeExt.ParseRfc2822DateTime("");
		}

		/// <summary/> 
		[Test, ExpectedException(typeof(FormatException))]
		public void ParseRfc2822DateTimeNotMatch()
		{
			DateTimeExt.ParseRfc2822DateTime("ABC");
		}

		/// <summary/> 
		[Test]
		public void ParseRfc2822DateTimeLazy()
		{
			// this is a ISO8601 datetime:
			var got = DateTimeExt.ParseRfc2822DateTime("2013-06-05T04:20:09.123-0100");
			Assert.AreEqual(new DateTime(2013,06,05, 05,20,09,123), got);
		}


		/// <summary/> 
		[Test]
		public void ParseRfc2822DateTimeGMTStrings()
		{
			DateTime dtUtcTime = new DateTime(2013, 2, 20, 5, 21, 0, 0, DateTimeKind.Utc);

			// returns the UTC time
			Assert.AreEqual(dtUtcTime, DateTimeExt.ParseRfc2822DateTime("Wed, 20 Feb 13 05:21:00 GMT"));
			Assert.AreEqual(dtUtcTime, DateTimeExt.ParseRfc2822DateTime("Wed, 20 Feb 13 05:21:00 Z"));
			Assert.AreEqual(dtUtcTime, DateTimeExt.ParseRfc2822DateTime("Wed, 20 Feb 13 05:21:00 UT"));
			Assert.AreEqual(dtUtcTime, DateTimeExt.ParseRfc2822DateTime("Wed, 20 Feb 13 05:21:00 +00:00"));
			Assert.AreEqual(dtUtcTime, DateTimeExt.ParseRfc2822DateTime("Wed, 20 Feb 13 05:21:00 +0000"));
			Assert.AreEqual(dtUtcTime, DateTimeExt.ParseRfc2822DateTime("Wed, 20 Feb 13 05:21:00 -00:00"));
			Assert.AreEqual(dtUtcTime, DateTimeExt.ParseRfc2822DateTime("Wed, 20 Feb 13 05:21:00 -0000"));
			Assert.AreEqual(dtUtcTime, DateTimeExt.ParseRfc2822DateTime("Wed, 20 Feb 13 05:21:00 +000"));
			Assert.AreEqual(dtUtcTime, DateTimeExt.ParseRfc2822DateTime("Wed, 20 Feb 13 05:21:00 +00"));
			Assert.AreEqual(dtUtcTime, DateTimeExt.ParseRfc2822DateTime("Wed, 20 Feb 13 05:21:00 +0"));
			Assert.AreEqual(dtUtcTime, DateTimeExt.ParseRfc2822DateTime("Wed, 20 Feb 13 05:21:00 -000"));
			Assert.AreEqual(dtUtcTime, DateTimeExt.ParseRfc2822DateTime("Wed, 20 Feb 13 05:21:00 -00"));
			Assert.AreEqual(dtUtcTime, DateTimeExt.ParseRfc2822DateTime("Wed, 20 Feb 13 05:21:00 -0"));

			Assert.AreEqual(dtUtcTime, DateTimeExt.ParseRfc2822DateTime("Wed, 20 Feb 13 06:21:00 +1"));
			Assert.AreEqual(dtUtcTime, DateTimeExt.ParseRfc2822DateTime("Wed, 20 Feb 13 06:21:00 +01"));
			Assert.AreEqual(dtUtcTime, DateTimeExt.ParseRfc2822DateTime("Wed, 20 Feb 13 05:31:00 +010"));
			Assert.AreEqual(dtUtcTime, DateTimeExt.ParseRfc2822DateTime("Wed, 20 Feb 13 15:21:00 +10"));
			Assert.AreEqual(dtUtcTime, DateTimeExt.ParseRfc2822DateTime("Wed, 20 Feb 13 06:21:00 +100"));
			Assert.AreEqual(dtUtcTime, DateTimeExt.ParseRfc2822DateTime("Wed, 20 Feb 13 06:21:00 +0100"));
			Assert.AreEqual(dtUtcTime, DateTimeExt.ParseRfc2822DateTime("Wed, 20 Feb 13 06:21:00 +01:00"));

			Assert.AreEqual(dtUtcTime, DateTimeExt.ParseRfc2822DateTime("Wed, 20 Feb 13 04:21:00 -1"));
			Assert.AreEqual(dtUtcTime, DateTimeExt.ParseRfc2822DateTime("Wed, 20 Feb 13 04:21:00 -01"));
			Assert.AreEqual(dtUtcTime, DateTimeExt.ParseRfc2822DateTime("Wed, 20 Feb 13 05:11:00 -010"));
			Assert.AreEqual(dtUtcTime, DateTimeExt.ParseRfc2822DateTime("Tue, 19 Feb 13 19:21:00 -10"));
			Assert.AreEqual(dtUtcTime, DateTimeExt.ParseRfc2822DateTime("Wed, 20 Feb 13 04:21:00 -100"));
			Assert.AreEqual(dtUtcTime, DateTimeExt.ParseRfc2822DateTime("Wed, 20 Feb 13 04:21:00 -0100"));
			Assert.AreEqual(dtUtcTime, DateTimeExt.ParseRfc2822DateTime("Wed, 20 Feb 13 04:21:00 -01:00"));

		}

		/// <summary/> 
		[Test]
		public void ParseRfc2822DateTimeInvalidZoneStrings()
		{
			try
			{
				DateTimeExt.ParseRfc2822DateTime("Wed, 20 Feb 13 05:21:00 0000");
				Assert.Fail("Expected FormatException not raised");
			}
			catch (FormatException)
			{
				// OK
			}

			try
			{
				DateTimeExt.ParseRfc2822DateTime("Tue, 20 Feb 13 05:21:00 J"); // not recognized by regex and fallback fails to parse
				Assert.Fail("Expected FormatException not raised on unused military timezone 'J'");
			}
			catch (FormatException)
			{
				// OK
			}
			
		}

		/// <summary/> 
		[Test]
		public void ParseRfc2822DateTimeZoneStrings()
		{
			DateTime dtUtcTime = new DateTime(2013, 2, 20, 5, 21, 0, 0, DateTimeKind.Utc);

			// returns the UTC time
			Assert.AreEqual(dtUtcTime, DateTimeExt.ParseRfc2822DateTime("Wed, 20 Feb 13 05:21:00 GMT"));
			
			Assert.AreEqual(dtUtcTime, DateTimeExt.ParseRfc2822DateTime("Wed, 20 Feb 13 06:21:00 A")); // +1h
			Assert.AreEqual(dtUtcTime, DateTimeExt.ParseRfc2822DateTime("Wed, 20 Feb 13 07:21:00 B"));
			Assert.AreEqual(dtUtcTime, DateTimeExt.ParseRfc2822DateTime("Wed, 20 Feb 13 08:21:00 C"));
			Assert.AreEqual(dtUtcTime, DateTimeExt.ParseRfc2822DateTime("Wed, 20 Feb 13 09:21:00 D"));
			Assert.AreEqual(dtUtcTime, DateTimeExt.ParseRfc2822DateTime("Wed, 20 Feb 13 10:21:00 E")); // +5h
			Assert.AreEqual(dtUtcTime, DateTimeExt.ParseRfc2822DateTime("Wed, 20 Feb 13 11:21:00 F")); // we are lazy about weekday...
			Assert.AreEqual(dtUtcTime, DateTimeExt.ParseRfc2822DateTime("Tue, 20 Feb 13 12:21:00 G"));
			Assert.AreEqual(dtUtcTime, DateTimeExt.ParseRfc2822DateTime("Tue, 20 Feb 13 13:21:00 H"));
			Assert.AreEqual(dtUtcTime, DateTimeExt.ParseRfc2822DateTime("Tue, 20 Feb 13 14:21:00 I")); // +9h
			// J not used
			Assert.AreEqual(dtUtcTime, DateTimeExt.ParseRfc2822DateTime("Tue, 20 Feb 13 15:21:00 K")); // +10h
			Assert.AreEqual(dtUtcTime, DateTimeExt.ParseRfc2822DateTime("Tue, 20 Feb 13 16:21:00 L"));
			Assert.AreEqual(dtUtcTime, DateTimeExt.ParseRfc2822DateTime("Tue, 20 Feb 13 17:21:00 M")); // +12h
			
			Assert.AreEqual(dtUtcTime, DateTimeExt.ParseRfc2822DateTime("Wed, 20 Feb 13 04:21:00 N")); // -1h
			Assert.AreEqual(dtUtcTime, DateTimeExt.ParseRfc2822DateTime("Wed, 20 Feb 13 03:21:00 O"));
			Assert.AreEqual(dtUtcTime, DateTimeExt.ParseRfc2822DateTime("Wed, 20 Feb 13 02:21:00 P"));
			Assert.AreEqual(dtUtcTime, DateTimeExt.ParseRfc2822DateTime("Wed, 20 Feb 13 01:21:00 Q")); // -4h
			Assert.AreEqual(dtUtcTime, DateTimeExt.ParseRfc2822DateTime("Wed, 20 Feb 13 00:21:00 R"));
			Assert.AreEqual(dtUtcTime, DateTimeExt.ParseRfc2822DateTime("Wed, 19 Feb 13 23:21:00 S"));
			Assert.AreEqual(dtUtcTime, DateTimeExt.ParseRfc2822DateTime("Wed, 19 Feb 13 22:21:00 T"));
			Assert.AreEqual(dtUtcTime, DateTimeExt.ParseRfc2822DateTime("Wed, 19 Feb 13 21:21:00 U")); // + 8h
			Assert.AreEqual(dtUtcTime, DateTimeExt.ParseRfc2822DateTime("Wed, 19 Feb 13 20:21:00 V"));
			Assert.AreEqual(dtUtcTime, DateTimeExt.ParseRfc2822DateTime("Wed, 19 Feb 13 19:21:00 W"));
			Assert.AreEqual(dtUtcTime, DateTimeExt.ParseRfc2822DateTime("Wed, 19 Feb 13 18:21:00 X"));
			Assert.AreEqual(dtUtcTime, DateTimeExt.ParseRfc2822DateTime("Wed, 19 Feb 13 17:21:00 Y")); // +12h
			Assert.AreEqual(dtUtcTime, DateTimeExt.ParseRfc2822DateTime("Wed, 20 Feb 13 05:21:00 Z")); // == UTC
			
			Assert.AreEqual(dtUtcTime, DateTimeExt.ParseRfc2822DateTime("Wed, 20 Feb 13 05:21:00 UT")); // == UTC
			Assert.AreEqual(dtUtcTime, DateTimeExt.ParseRfc2822DateTime("Wed, 20 Feb 13 01:21:00 EDT")); 
			Assert.AreEqual(dtUtcTime, DateTimeExt.ParseRfc2822DateTime("Wed, 20 Feb 13 00:21:00 EST")); 
			Assert.AreEqual(dtUtcTime, DateTimeExt.ParseRfc2822DateTime("Wed, 20 Feb 13 00:21:00 CDT")); 
			Assert.AreEqual(dtUtcTime, DateTimeExt.ParseRfc2822DateTime("Tue, 19 Feb 13 23:21:00 CST")); 
			Assert.AreEqual(dtUtcTime, DateTimeExt.ParseRfc2822DateTime("Tue, 19 Feb 13 23:21:00 MDT")); 
			Assert.AreEqual(dtUtcTime, DateTimeExt.ParseRfc2822DateTime("Tue, 19 Feb 13 22:21:00 MST")); 
			Assert.AreEqual(dtUtcTime, DateTimeExt.ParseRfc2822DateTime("Tue, 19 Feb 13 22:21:00 PDT")); 
			Assert.AreEqual(dtUtcTime, DateTimeExt.ParseRfc2822DateTime("Tue, 19 Feb 13 21:21:00 PST"));

			
		}

		/// <summary/> 
		[Test]
		public void ParseRfc2822DateTimeZoneAbbreviationsStrings()
		{
			DateTime dtUtcTime = new DateTime(2013, 2, 20, 5, 21, 0, 0, DateTimeKind.Utc);

			// returns the UTC time
			Assert.AreEqual(dtUtcTime, DateTimeExt.ParseRfc2822DateTime("Wed, 20 Feb 13 05:21:00 GMT")); // ==  UTC

			Assert.AreEqual(dtUtcTime, DateTimeExt.ParseRfc2822DateTime("Wed, 20 Feb 13 06:21:00 CET"));  // UTC+1
			Assert.AreEqual(dtUtcTime, DateTimeExt.ParseRfc2822DateTime("Wed, 20 Feb 13 11:06:00 NPT"));  // Nepal Time / Asia UTC + 5:45 hours 
			Assert.AreEqual(dtUtcTime, DateTimeExt.ParseRfc2822DateTime("Wed, 20 Feb 13  9:21:00 GST"));  // Dubai (United Arab Emirates - Dubai) UTC + 4 hours 

			Assert.AreEqual(new DateTime(2013, 3, 25, 3, 44, 58, 0, DateTimeKind.Utc), DateTimeExt.ParseRfc2822DateTime("Mo, 25 Mär 2013 04:44:58 CET"));
			Assert.AreEqual(new DateTime(2013, 3, 26, 5, 50, 42, 0, DateTimeKind.Utc), DateTimeExt.ParseRfc2822DateTime("Di, 26 Mär 2013 06:50:42 CET"));
			Assert.AreEqual(new DateTime(2013, 5,  6, 22, 8, 18, 0, DateTimeKind.Utc), DateTimeExt.ParseRfc2822DateTime("Di, 07 Mai 2013 00:08:18 CEST"));
			Assert.AreEqual(new DateTime(2013, 3,  8, 17, 21, 8, 0, DateTimeKind.Utc), DateTimeExt.ParseRfc2822DateTime("Fr, 08 Mär 2013 18:21:08 CET"));
			
		}


		/// <summary/> 
		[Test]
		public void ParseRfc2822DateTimeCaseIndependenceStrings()
		{
			DateTime dtUtcTime = new DateTime(2013, 2, 20, 5, 21, 0, 0, DateTimeKind.Utc);

			// returns the UTC time
			Assert.AreEqual(dtUtcTime, DateTimeExt.ParseRfc2822DateTime("wEd, 20 Feb 13 05:21:00 GMT"));
			Assert.AreEqual(dtUtcTime, DateTimeExt.ParseRfc2822DateTime("Wed, 20 fEb 13 06:21:00 A")); // +1h
			Assert.AreEqual(dtUtcTime, DateTimeExt.ParseRfc2822DateTime("Wed, 20 Feb 13 06:21:00 a")); // +1h
		}

		/// <summary/> 
		[Test]
		public void ParseRfc2822DateTimeOptionalStrings()
		{
			DateTime dtUtcTime = new DateTime(2013, 2, 20, 5, 21, 0, 0, DateTimeKind.Utc);

			// Optional: week day
			Assert.AreEqual(dtUtcTime, DateTimeExt.ParseRfc2822DateTime("20 Feb 13 05:21:00 GMT"));
			Assert.AreEqual(dtUtcTime, DateTimeExt.ParseRfc2822DateTime("20 Feb 13 06:21:00 A")); // +1h

			// Optional: seconds in time
			Assert.AreEqual(dtUtcTime, DateTimeExt.ParseRfc2822DateTime("Wed, 20 Feb 2013 05:21:00 GMT"));
			Assert.AreEqual(dtUtcTime, DateTimeExt.ParseRfc2822DateTime("Wed, 20 Feb 2013 06:21 A")); // +1h

			// Two digit year edge cases/rules:
			// namely 00 to 49 are equivalent to 2000 to 2049, 
			// and 50 to 99 are equivalent to 1950 to 1999:

			Assert.AreEqual(new DateTime(2000, 2, 20, 5, 21, 0, 0, DateTimeKind.Utc), 
				DateTimeExt.ParseRfc2822DateTime("20 Feb 00 05:21:00 GMT"));
			Assert.AreEqual(new DateTime(2049, 2, 20, 5, 21, 0, 0, DateTimeKind.Utc),
				DateTimeExt.ParseRfc2822DateTime("20 Feb 49 05:21:00 GMT"));
			Assert.AreEqual(new DateTime(1950, 2, 20, 5, 21, 0, 0, DateTimeKind.Utc),
				DateTimeExt.ParseRfc2822DateTime("20 Feb 50 05:21:00 GMT"));
			Assert.AreEqual(new DateTime(1999, 2, 20, 5, 21, 0, 0, DateTimeKind.Utc),
				DateTimeExt.ParseRfc2822DateTime("20 Feb 99 05:21:00 GMT"));
		}

		/// <summary/> 
		[Test]
		public void ParseRfc2822DateTimeWithCommentsStrings()
		{
			DateTime dtUtcTime = new DateTime(2013, 2, 20, 5, 21, 0, 0, DateTimeKind.Utc);

			//  RFC 2822: comment are enclosed in "(" / ")" pairs, can be nested and/or can contain masked comment chars "\(" or "\)"
			Assert.AreEqual(dtUtcTime, DateTimeExt.ParseRfc2822DateTime(@"Wed, 20 Feb 13 05:21:00 +0000 (this is GMT)"));
			Assert.AreEqual(dtUtcTime, DateTimeExt.ParseRfc2822DateTime(@"Wed, 20(th) Feb 13 05:21:00 +0000"));
			Assert.AreEqual(dtUtcTime, DateTimeExt.ParseRfc2822DateTime(@"Wed, 20 Feb 13()05:21:00 +0000"));
			Assert.AreEqual(dtUtcTime, DateTimeExt.ParseRfc2822DateTime(@"Wed, 20 (Mar)Feb(escaped \( comment\) 2005) 13 05:21:00 +0000"));
			Assert.AreEqual(dtUtcTime, DateTimeExt.ParseRfc2822DateTime(@"Wed, 20 Feb 13(comment with
 newline)05:21:00 +0000"));
			Assert.AreEqual(dtUtcTime, DateTimeExt.ParseRfc2822DateTime(@"(This is RFC comment at start)Wed, 20 Feb 13 05:21:00 +0000"));
		}

		/// <summary/> 
		[Test]
		public void ParseRfc2822DateTimeWithTabsStrings()
		{
			DateTime dtUtcTime = new DateTime(2013, 2, 20, 5, 21, 0, 0, DateTimeKind.Utc);

			Assert.AreEqual(dtUtcTime, DateTimeExt.ParseRfc2822DateTime("Wed, 20 Feb\t 13 05:21:00 +0000"));
		}

		/// <summary/> 
		[Test]
		public void ParseRfc2822DateTimeWithNewlineStrings()
		{
			DateTime dtUtcTime = new DateTime(2013, 2, 20, 5, 21, 0, 0, DateTimeKind.Utc);

			Assert.AreEqual(dtUtcTime, DateTimeExt.ParseRfc2822DateTime("Wed, 20 Feb \r\n13 05:21:00 +0000"));
		}

		/// <summary/> 
		[Test]
		public void ParseRfc2822DateTimeWithMultiSpacesStrings()
		{
			DateTime dtUtcTime = new DateTime(2013, 2, 20, 5, 21, 0, 0, DateTimeKind.Utc);

			Assert.AreEqual(dtUtcTime, DateTimeExt.ParseRfc2822DateTime("Wed, 20 Feb   13 05:21:00 +0000"));
			Assert.AreEqual(dtUtcTime, DateTimeExt.ParseRfc2822DateTime("Wed, 20 Feb   13 05:21:00+0000"));
			Assert.AreEqual(dtUtcTime, DateTimeExt.ParseRfc2822DateTime("Wed, 20 Feb   13 05:21:00+00:00"));
			Assert.AreEqual(dtUtcTime, DateTimeExt.ParseRfc2822DateTime("Wed, 20 Feb 2013 06:21:00   A")); // +1h
			Assert.AreEqual(dtUtcTime, DateTimeExt.ParseRfc2822DateTime("Wed, 20   Feb 13 06:21:00 A")); // +1h
			Assert.AreEqual(dtUtcTime, DateTimeExt.ParseRfc2822DateTime("Wed, 20   Feb 13 06:21:00A")); // +1h
		}
	}
}
