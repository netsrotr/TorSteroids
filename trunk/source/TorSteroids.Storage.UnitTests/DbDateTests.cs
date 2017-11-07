using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using TorSteroids.Storage.Data;

namespace TorSteroids.Storage.UnitTests
{
	/// <summary>
	/// DbDate Tests
	/// </summary>
	[TestFixture]
	public class DbDateTests
	{

		[Test]
		public void MinValue()
		{
			DbDate date = DbDate.MinValue;
			Assert.AreEqual(0, date.Ticks);
		}

		[Test]
		public void MaxValue()
		{
			DbDate date = DbDate.MaxValue;
			Assert.AreEqual(99991231, date.Ticks);
		}

		[Test]
		public void CtorAndProperties()
		{
			DbDate date = new DbDate();
			Assert.AreEqual(DbDate.MinValue, date);

			date = new DbDate(9999, 12, 31);
			Assert.AreEqual(DbDate.MaxValue, date);

			date = new DbDate(1, 2, 3);
			Assert.AreEqual(1, date.Year);
			Assert.AreEqual(2, date.Month);
			Assert.AreEqual(3, date.Day);
		}

		[Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void CtorFailOnYearLowerBound()
		{
			DbDate date = new DbDate(0, 1, 1);
		}

		[Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void CtorFailOnYearUpperBound()
		{
			DbDate date = new DbDate(10000, 1, 1);
		}

		[Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void CtorFailOnMonthLowerBound()
		{
			DbDate date = new DbDate(1, 0, 1);
		}

		[Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void CtorFailOnMonthUpperBound()
		{
			DbDate date = new DbDate(2, 13, 1);
		}

		[Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void CtorFailOnDayLowerBound()
		{
			DbDate date = new DbDate(2000, 1, 0);
		}

		[Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void CtorFailOnDayUpperBound()
		{
			DbDate date = new DbDate(9999, 1, 32);
		}

		[Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void CtorFailOnInconsistentDayUpperBound()
		{
			// april has only 30 days:
			DbDate date = new DbDate(2001, 4, 31);
		}

		[Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void CtorFailOnInvalidLeapYearDayUpperBound()
		{
			// this was not a leap year:
			DbDate date = new DbDate(2001, 2, 29);
		}

		[Test]
		public void ImplicitCastToDateTime()
		{
			DbDate date = new DbDate(2013, 2, 13);
			DateTime target = date;
			// expected zeros on the unused time part of DateTime:
			Assert.AreEqual(0, target.Hour);
			Assert.AreEqual(0, target.Minute);
			Assert.AreEqual(0, target.Second);
			Assert.AreEqual(0, target.Millisecond);

			Assert.AreEqual(2013, target.Year);
			Assert.AreEqual(2, target.Month);
			Assert.AreEqual(13, target.Day);
			
			Assert.AreEqual(DateTimeKind.Utc, target.Kind);
		}

		[Test]
		public void ExplicitCastToDbDate()
		{
			DateTime date = new DateTime(2013, 2, 20, 1, 2, 3, 99, DateTimeKind.Utc);
			// yes, we like to explicitly "loose" infos:
			DbDate target = (DbDate)date;
			
			Assert.AreEqual(2013, target.Year);
			Assert.AreEqual(2, target.Month);
			Assert.AreEqual(20, target.Day);
		}

		[Test]
		public void ConvertibleCasts()
		{
			object self = new DbDate(2013, 11, 5);

			DbDate selfCasted = (DbDate) self;
			Assert.AreEqual(self, selfCasted);

			Assert.AreEqual(self, GetDbDateByObjectT((DbDate)self));
			Assert.AreEqual(self, GetDbDateByIConvertibleT((DbDate)self));

		}
		
		private DbDate GetDbDateByObjectT<T>(T value)
		{
			object intermediate = value;
			return (DbDate)intermediate;
		}

		private DbDate GetDbDateByIConvertibleT<T>(T value)
		{
			IConvertible intermediate = (IConvertible)value;
			return (DbDate)intermediate;
		}

		[Test, ExpectedException(typeof(InvalidCastException))]
		public void ConvertibleInvalidCastToBoolean()
		{
			var self = (IConvertible) new DbDate(2013, 11, 5);
			self.ToBoolean(null);
		}

		[Test]
		public void ConvertibleValidCastToIntAndLong()
		{
			var self = (IConvertible)new DbDate(2013, 11, 5);
			Assert.AreEqual(20131105 ,self.ToInt32(null));
			Assert.AreEqual(20131105u ,self.ToUInt32(null));
			Assert.AreEqual(20131105L, self.ToInt64(null));
			Assert.AreEqual(20131105ul, self.ToUInt64(null));
		}

		[Test]
		public void ConvertibleValidCastToDateTime()
		{
			var self = (IConvertible) new DbDate(2013, 11, 5);
			Assert.AreEqual(new DateTime(2013,11,5, 0,0,0,DateTimeKind.Utc), self.ToDateTime(null));
		}
	}
}
