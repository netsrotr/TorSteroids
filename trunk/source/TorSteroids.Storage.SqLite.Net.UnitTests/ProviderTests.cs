#region Copyright PROCOS AG; All rights reserved
// Code copyright by PROCOS AG; FL-9490 Vaduz; All rights reserved
#endregion

using System;
using System.Data;
using NUnit.Framework;

namespace TorSteroids.Storage.SqLite.Net.UnitTests
{
	/// <summary>
	/// DbParse Tests
	/// </summary>
	[TestFixture]
	public class ProviderTests
	{
        
        public static long GetTicksTimestamp(DateTime value)
        {
            return value.ToUniversalTime().Ticks;
        }

        
        public static DateTime FromTicksTimestamp(long value)
        {
            return new DateTime(value, DateTimeKind.Utc);
        }

        

	    [Test]
	    public void GetAndParseTimeStamps()
	    {
            var now = DateTime.UtcNow;
        
            var v = GetTicksTimestamp(now);
            Console.WriteLine("{0} :: {1}", now, v);

            var d = FromTicksTimestamp(v);
            Console.WriteLine("{0} :: {1}", now, d);
            Assert.AreEqual(now, d);
            Assert.AreEqual(v, d.Ticks);
	    }
	}
}
