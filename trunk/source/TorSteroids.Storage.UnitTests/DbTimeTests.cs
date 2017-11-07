using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using TorSteroids.Storage.Data;

namespace TorSteroids.Storage.UnitTests
{
	/// <summary>
	/// DbTime Tests
	/// </summary>
	[TestFixture]
	public class DbTimeTests
	{

		[Test]
		public void MinValue()
		{
			DbTime time = DbTime.MinValue;
			Assert.AreEqual(0, time.Ticks);
			Assert.AreEqual(DateTimeKind.Unspecified, time.Kind);
		}

		[Test]
		public void MaxValue()
		{
			DbTime time = DbTime.MaxValue;
			Assert.AreEqual(235959999, time.Ticks);
			Assert.AreEqual(DateTimeKind.Unspecified, time.Kind);
		}

		[Test]
		public void CtorAndProperties()
		{
			DbTime time = new DbTime();
			Assert.AreEqual(DbTime.MinValue, time);

			time = new DbTime(23, 59, 59, 999, DateTimeKind.Unspecified);
			Assert.AreEqual(DbTime.MaxValue, time);

			time = new DbTime(1, 2, 3, DateTimeKind.Local);
			Assert.AreEqual(1, time.Hour);
			Assert.AreEqual(2, time.Minute);
			Assert.AreEqual(3, time.Second);
			Assert.AreEqual(0, time.Millisecond);
			Assert.AreEqual(DateTimeKind.Local, time.Kind);

			time = new DbTime(4, 5, 6, 10, DateTimeKind.Utc);
			Assert.AreEqual(4, time.Hour);
			Assert.AreEqual(5, time.Minute);
			Assert.AreEqual(6, time.Second);
			Assert.AreEqual(10, time.Millisecond);
			Assert.AreEqual(DateTimeKind.Utc, time.Kind);
		}

		[Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void CtorFailOnHourLowerBoundNegative()
		{
			DbTime time = new DbTime(-1, 1,1, DateTimeKind.Unspecified);
		}

		[Test]
		public void CtorNotFailOnHourUpperBound()
		{
			DbTime time = new DbTime(23, 1, 1, DateTimeKind.Unspecified);
		}

		[Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void CtorFailOnHourUpperBound()
		{
			DbTime time = new DbTime(24, 1, 1, DateTimeKind.Unspecified);
		}

		[Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void CtorFailOnMinuteLowerBound()
		{
			DbTime time = new DbTime(0, -1, 1, DateTimeKind.Unspecified);
		}

		[Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void CtorFailOnMinuteUpperBound()
		{
			DbTime time = new DbTime(0, 60, 1, DateTimeKind.Unspecified);
		}

		[Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void CtorFailOnSecondLowerBound()
		{
			DbTime time = new DbTime(0, 0, -1, DateTimeKind.Unspecified);
		}

		[Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void CtorFailOnSecondUpperBound()
		{
			DbTime time = new DbTime(0, 0, 60, DateTimeKind.Unspecified);
		}

		[Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void CtorFailOnMilliSecondLowerBound()
		{
			DbTime time = new DbTime(0, 0, 0, -1, DateTimeKind.Unspecified);
		}

		[Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void CtorFailOnMilliSecondUpperBound()
		{
			DbTime time = new DbTime(0, 0, 0, 1000, DateTimeKind.Unspecified);
		}

		[Test]
		public void ImplicitCastToDateTime()
		{
			DbTime time = new DbTime(1, 2, 3, 99, DateTimeKind.Utc);
			DateTime target = time;
			// expected unix epoch on the unused date part of DateTime:
			Assert.AreEqual(1970, target.Year);
			Assert.AreEqual(1, target.Month);
			Assert.AreEqual(1, target.Day);
			
			Assert.AreEqual(1, target.Hour);
			Assert.AreEqual(2, target.Minute);
			Assert.AreEqual(3, target.Second);
			Assert.AreEqual(99, target.Millisecond);
			Assert.AreEqual(DateTimeKind.Utc, target.Kind);
		}

		[Test]
		public void ExplicitCastToDbTime()
		{
			DateTime time = new DateTime(2013, 2, 20, 1, 2, 3, 99, DateTimeKind.Utc);
			// yes, we like to explicitly "loose" infos:
			DbTime target = (DbTime)time;
			
			Assert.AreEqual(1, target.Hour);
			Assert.AreEqual(2, target.Minute);
			Assert.AreEqual(3, target.Second);
			Assert.AreEqual(99, target.Millisecond);
			Assert.AreEqual(DateTimeKind.Utc, target.Kind);
		}

		[Test]
		public void ConvertibleCasts()
		{
			object self = new DbTime(15, 36, 11, DateTimeKind.Utc);

			DbTime selfCasted = (DbTime)self;
			Assert.AreEqual(self, selfCasted);

			Assert.AreEqual(self, GetDbDateByObjectT((DbTime)self));
			Assert.AreEqual(self, GetDbDateByIConvertibleT((DbTime)self));

		}

		private DbTime GetDbDateByObjectT<T>(T value)
		{
			object intermediate = value;
			return (DbTime)intermediate;
		}

		private DbTime GetDbDateByIConvertibleT<T>(T value)
		{
			IConvertible intermediate = (IConvertible)value;
			return (DbTime)intermediate;
		}

		[Test, ExpectedException(typeof(InvalidCastException))]
		public void ConvertibleInvalidCastToBoolean()
		{
			var self = (IConvertible)new DbTime(15, 36, 11, DateTimeKind.Utc);
			self.ToBoolean(null);
		}

		[Test]
		public void ConvertibleValidCastToIntAndLong()
		{
			var self = (IConvertible)new DbTime(15, 36, 11, DateTimeKind.Utc);
			Assert.AreEqual(153611000, self.ToInt32(null));
			Assert.AreEqual(153611000u, self.ToUInt32(null));
			Assert.AreEqual(153611000L, self.ToInt64(null));
			Assert.AreEqual(153611000ul, self.ToUInt64(null));
		}

		[Test]
		public void ConvertibleValidCastToDateTime()
		{
			var self = (IConvertible) new DbTime(15, 36, 11, DateTimeKind.Utc);
			Assert.AreEqual(new DateTime(1970, 1, 1, 15, 36, 11, 0, DateTimeKind.Utc), self.ToDateTime(null));
		}
	}
}
