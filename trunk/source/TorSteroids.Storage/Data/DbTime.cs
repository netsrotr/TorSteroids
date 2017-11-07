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
using System.Diagnostics;
using System.Globalization;
using TorSteroids.Storage.Resources;

namespace TorSteroids.Storage.Data
{
	/// <summary>
	/// Struct DbTime.
	/// </summary>
	[Serializable]
	[DebuggerDisplay("{DebuggerDisplay,nq}")] // "nq" == "no quotes"; see also http://blogs.msdn.com/b/jaredpar/archive/2011/03/18/debuggerdisplay-attribute-best-practices.aspx
	public struct DbTime : IEquatable<DbTime>, IConvertible
	{
		public static DbTime MinValue = new DbTime(MinTicks, DateTimeKind.Unspecified);
		public static DbTime MaxValue = new DbTime(MaxTicks, DateTimeKind.Unspecified);

		private const int MinTicks = 0;
		private const int MaxTicks = 235959999;

		private const int UnixEpochDay = 1;
		private const int UnixEpochMonth = 1;
		private const int UnixEpochYear = 1970;

		private readonly int _ticks;
		private readonly DateTimeKind _kind;

		#region ctor's

		internal DbTime(int ticks, DateTimeKind kind)
		{
			ticks.ExceptionIfOutOfRange(MinTicks, MaxTicks, "ticks");
			
			if ((kind < DateTimeKind.Unspecified) || (kind > DateTimeKind.Local))
			{
				throw new ArgumentException(SR.ArgumentOutOfRangeInvalidDateTimeKind, "kind");
			}
			_ticks = ticks;
			_kind = kind;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DbTime"/> structure.
		/// </summary>
		/// <param name="hour">The hour.</param>
		/// <param name="minute">The minute.</param>
		/// <param name="second">The second.</param>
		/// <param name="kind">The kind.</param>
		public DbTime(int hour, int minute, int second, DateTimeKind kind) :
			this(hour, minute, second, 0, kind)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DbTime"/> structure.
		/// </summary>
		/// <param name="hour">The hour.</param>
		/// <param name="minute">The minute.</param>
		/// <param name="second">The second.</param>
		/// <param name="millisecond">The millisecond.</param>
		/// <param name="kind">The kind.</param>
		/// <exception cref="System.ArgumentException">kind</exception>
		public DbTime(int hour, int minute, int second, int millisecond, DateTimeKind kind)
		{
			millisecond.ExceptionIfOutOfRange(0, 999, "millisecond");
			
			if ((kind < DateTimeKind.Unspecified) || (kind > DateTimeKind.Local))
			{
				throw new ArgumentException(SR.ArgumentOutOfRangeInvalidDateTimeKind, "kind");
			}
			
			int num =  TimeToTicks(hour, minute, second);

			_ticks = num + millisecond;
			_kind = kind;
		}

		#endregion

		#region public properties

		/// <summary>
		/// Gets the ticks (time ticks).
		/// </summary>
		/// <value>The ticks.</value>
		public int Ticks
		{
			[DebuggerStepThrough]
			get { return _ticks; }
		}

		/// <summary>
		/// Gets the original kind (at creation time).
		/// </summary>
		/// <value>The kind.</value>
		public DateTimeKind Kind { get { return _kind; } }

		/// <summary>
		/// Gets the hour.
		/// </summary>
		/// <value>The hour.</value>
		public int Hour 
		{
			[DebuggerStepThrough]
			get { return _ticks / 10000000; }
		}

		/// <summary>
		/// Gets the minute.
		/// </summary>
		/// <value>The minute.</value>
		public int Minute
		{
			[DebuggerStepThrough]
			get { return (_ticks % 10000000) / 100000; }
		}

		/// <summary>
		/// Gets the second.
		/// </summary>
		/// <value>The second.</value>
		public int Second
		{
			[DebuggerStepThrough]
			get { return (_ticks % 100000) / 1000; }
		}

		/// <summary>
		/// Gets the millisecond.
		/// </summary>
		/// <value>The millisecond.</value>
		public int Millisecond
		{
			[DebuggerStepThrough]
			get { return _ticks % 1000; }
		}

		#endregion

		#region public methods

		/// <summary>
		/// Converts the instance to the DateTime.
		/// </summary>
		/// <returns>DateTime.</returns>
		public DateTime ToDateTime()
		{
			if (MinValue.Equals(this))
				return DateTime.MinValue;
			if (MaxValue.Equals(this))
				return DateTime.MaxValue;

			return new DateTime(UnixEpochYear, UnixEpochMonth, UnixEpochDay, Hour, Minute, Second, Millisecond, Kind);
		}

		/// <summary>
		/// Returns a <see cref="System.String" /> that represents this instance.
		/// </summary>
		/// <returns>A <see cref="System.String" /> that represents this instance.</returns>
		public override string ToString()
		{
			var t = (DateTime)this;
			return t.ToString(DateTimeFormatInfo.CurrentInfo);
		}

		/// <summary>
		/// Returns a <see cref="System.String" /> that represents this instance.
		/// </summary>
		/// <param name="provider">An <see cref="T:System.IFormatProvider" /> interface implementation that supplies culture-specific formatting information.</param>
		/// <returns>A <see cref="System.String" /> that represents this instance.</returns>
		public string ToString(IFormatProvider provider)
		{
			return ((DateTime)this).ToString(provider);
		}

		/// <summary>
		/// Returns a <see cref="System.String" /> that represents this instance.
		/// </summary>
		/// <param name="format">The date format string.</param>
		/// <returns>A <see cref="System.String" /> that represents this instance.</returns>
		public string ToString(string format)
		{
			return ((DateTime)this).ToString(format);
		}

		/// <summary>
		/// Returns a <see cref="System.String" /> that represents this instance.
		/// </summary>
		/// <param name="format">The date format string.</param>
		/// <param name="provider">The provider.</param>
		/// <returns>A <see cref="System.String" /> that represents this instance.</returns>
		public string ToString(string format, IFormatProvider provider)
		{
			return ((DateTime)this).ToString(format, provider);
		}

		#endregion

		#region private

		private static int TimeToTicks(int hour, int minute, int second)
		{
			hour.ExceptionIfOutOfRange(0, 23, "hour");
			minute.ExceptionIfOutOfRange(0, 59, "minute");
			second.ExceptionIfOutOfRange(0, 59, "second");
			
			return hour * 10000000 + minute * 100000 + second * 1000;
		}

		// do not remove, it is used within DebuggerDisplayAttribute at the class
		private string DebuggerDisplay
		{
			get { return ToString("T"); }
		}

		#endregion

		#region  IEquatable<DbTime>

		public bool Equals(DbTime other)
		{
			return _ticks == other._ticks && _kind == other._kind;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((DbTime) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (_ticks*397) ^ (int) _kind;
			}
		}

		public static bool operator ==(DbTime left, DbTime right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(DbTime left, DbTime right)
		{
			return !Equals(left, right);
		}

		#endregion

		#region conversion / operator overloading

		/// <summary>
		/// We can safely implicit cast to a DateTime.
		/// </summary>
		/// <param name="time">The value.</param>
		/// <returns></returns>
		public static implicit operator DateTime(DbTime time)
		{
			return time.ToDateTime();
		}

		/// <summary>
		/// We must implicit cast to a DbTime, because the date info gets lost by this conversion / downcast.
		/// </summary>
		/// <param name="dateTime">The DateTime value.</param>
		/// <returns></returns>
		public static explicit operator DbTime(DateTime dateTime)
		{
			if (dateTime == DateTime.MinValue)
				return DbTime.MinValue;

			return new DbTime(dateTime.Hour, dateTime.Minute, dateTime.Second, dateTime.Millisecond, dateTime.Kind);
		}


		#endregion

		#region IConvertible

		TypeCode IConvertible.GetTypeCode()
		{
			// we implement a part of:
			return TypeCode.DateTime;
		}

		bool IConvertible.ToBoolean(IFormatProvider provider)
		{
			throw new InvalidCastException(SR.InvalidCast_FromTo.FormatWith("DbTime", "Boolean"));
		}

		char IConvertible.ToChar(IFormatProvider provider)
		{
			throw new InvalidCastException(SR.InvalidCast_FromTo.FormatWith("DbTime", "Char"));
		}

		sbyte IConvertible.ToSByte(IFormatProvider provider)
		{
			throw new InvalidCastException(SR.InvalidCast_FromTo.FormatWith("DbTime", "SByte"));
		}

		byte IConvertible.ToByte(IFormatProvider provider)
		{
			throw new InvalidCastException(SR.InvalidCast_FromTo.FormatWith("DbTime", "Byte"));
		}

		short IConvertible.ToInt16(IFormatProvider provider)
		{
			throw new InvalidCastException(SR.InvalidCast_FromTo.FormatWith("DbTime", "Int16"));
		}

		ushort IConvertible.ToUInt16(IFormatProvider provider)
		{
			throw new InvalidCastException(SR.InvalidCast_FromTo.FormatWith("DbTime", "UInt16"));
		}

		int IConvertible.ToInt32(IFormatProvider provider)
		{
			return this.Ticks;
		}

		uint IConvertible.ToUInt32(IFormatProvider provider)
		{
			return (uint)this.Ticks;
		}

		long IConvertible.ToInt64(IFormatProvider provider)
		{
			return this.Ticks;
		}

		ulong IConvertible.ToUInt64(IFormatProvider provider)
		{
			return (ulong)this.Ticks;
		}

		float IConvertible.ToSingle(IFormatProvider provider)
		{
			throw new InvalidCastException(SR.InvalidCast_FromTo.FormatWith("DbTime", "Single"));
		}

		double IConvertible.ToDouble(IFormatProvider provider)
		{
			throw new InvalidCastException(SR.InvalidCast_FromTo.FormatWith("DbTime", "Double"));
		}

		decimal IConvertible.ToDecimal(IFormatProvider provider)
		{
			throw new InvalidCastException(SR.InvalidCast_FromTo.FormatWith("DbTime", "Decimal"));
		}

		DateTime IConvertible.ToDateTime(IFormatProvider provider)
		{
			return this;
		}

		object IConvertible.ToType(Type conversionType, IFormatProvider provider)
		{
			IConvertible t = (DateTime)this;
			return t.ToType(conversionType, provider);
		}

		#endregion

	}
}