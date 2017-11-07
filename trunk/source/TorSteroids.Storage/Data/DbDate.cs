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
	/// Struct DbDate.
	/// </summary>
	[Serializable]
	[DebuggerDisplay("{DebuggerDisplay,nq}")] // "nq" == "no quotes"; see also http://blogs.msdn.com/b/jaredpar/archive/2011/03/18/debuggerdisplay-attribute-best-practices.aspx
	public struct DbDate : IEquatable<DbDate>, IConvertible
	{
		public static DbDate MinValue = new DbDate(MinTicks);
		public static DbDate MaxValue = new DbDate(MaxTicks);

		private const int MinTicks = 0;
		private const int MaxTicks = 99991231;

		private readonly int _ticks;
		
		#region ctor's

		internal DbDate(int ticks)
		{
			ticks.ExceptionIfOutOfRange(MinTicks, MaxTicks,"ticks");
			
			_ticks = ticks;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DbDate"/> structure.
		/// </summary>
		/// <param name="year">The year.</param>
		/// <param name="month">The month.</param>
		/// <param name="day">The day.</param>
		public DbDate(int year, int month, int day)
		{
			// also checks leap years etc.:
			var validator = new DateTime(year, month, day, 0, 0, 0, DateTimeKind.Unspecified);
			_ticks = DateToTicks(validator.Year, validator.Month, validator.Day);
		}

		#endregion

		#region public properties

		public int Ticks
		{
			get { return _ticks; }
		}

		public int Year
		{
			[DebuggerStepThrough]
			get { return _ticks / 10000; }
		}

		public int Month
		{
			[DebuggerStepThrough]
			get { return (_ticks % 10000) / 100; }
		}

		public int Day
		{
			[DebuggerStepThrough]
			get { return _ticks % 100; }
		}

		#endregion

		#region public methods

		/// <summary>
		/// Transform the instance to a date time. 
		/// </summary>
		/// <param name="kind">The <see cref="DateTimeKind"/>. This is how we should interpret the stored ticks.</param>
		/// <returns></returns>
		public DateTime ToDateTime(DateTimeKind kind = DateTimeKind.Utc)
		{
			if (MinValue.Equals(this))
				return new DateTime(DateTime.MinValue.Year, DateTime.MinValue.Month, DateTime.MinValue.Day, 0, 0, 0, 0, kind);
			if (MaxValue.Equals(this))
				return new DateTime(DateTime.MaxValue.Year, DateTime.MaxValue.Month, DateTime.MaxValue.Day, 0, 0, 0, 0, kind);

			return new DateTime(Year, Month, Day, 0, 0, 0, 0, kind);
		}

		/// <summary>
		/// Returns a <see cref="System.String" /> that represents this instance.
		/// </summary>
		/// <returns>A <see cref="System.String" /> that represents this instance.</returns>
		public override string ToString()
		{
			var t = (DateTime) this;
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

		#region private / internal

		private static int DateToTicks(int year, int month, int day)
		{
			return year * 10000 + month * 100 + day ;
		}

		// do not remove, it is used within DebuggerDisplayAttribute at the class
		private string DebuggerDisplay
		{
			get { return ToString("d"); }
		}

		#endregion

		#region IEquatable<DbDate>

		public bool Equals(DbDate other)
		{
			return _ticks == other._ticks;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((DbDate) obj);
		}

		public override int GetHashCode()
		{
			return _ticks;
		}

		public static bool operator ==(DbDate left, DbDate right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(DbDate left, DbDate right)
		{
			return !Equals(left, right);
		}

		#endregion

		#region conversion / operator overloading

		/// <summary>
		/// We can safely implicit cast to a DateTime.
		/// </summary>
		/// <param name="date">The value.</param>
		/// <returns>The DateTime as UTC</returns>
		public static implicit operator DateTime(DbDate date)
		{
			return new DateTime(date.Year, date.Month, date.Day,
				0, 0, 0, 0, DateTimeKind.Utc);
		}

		/// <summary>
		/// We must implicit cast to a DbDate, because the time and Kind info gets lost by this conversion / downcast.
		/// </summary>
		/// <param name="dateTime">The DateTime value.</param>
		/// <returns>DbDate that always contain a UTC version of the provided <paramref name="dateTime"/></returns>
		public static explicit operator DbDate(DateTime dateTime)
		{
			if (dateTime == DateTime.MinValue)
				return DbDate.MinValue;

			var utc = dateTime.ToUniversalTime();
			return new DbDate(utc.Year, utc.Month, utc.Day);
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
			throw new InvalidCastException(SR.InvalidCast_FromTo.FormatWith("DbDate", "Boolean"));
		}

		char IConvertible.ToChar(IFormatProvider provider)
		{
			throw new InvalidCastException(SR.InvalidCast_FromTo.FormatWith("DbDate", "Char"));
		}

		sbyte IConvertible.ToSByte(IFormatProvider provider)
		{
			throw new InvalidCastException(SR.InvalidCast_FromTo.FormatWith("DbDate", "SByte"));
		}

		byte IConvertible.ToByte(IFormatProvider provider)
		{
			throw new InvalidCastException(SR.InvalidCast_FromTo.FormatWith("DbDate", "Byte"));
		}

		short IConvertible.ToInt16(IFormatProvider provider)
		{
			throw new InvalidCastException(SR.InvalidCast_FromTo.FormatWith("DbDate", "Int16"));
		}

		ushort IConvertible.ToUInt16(IFormatProvider provider)
		{
			throw new InvalidCastException(SR.InvalidCast_FromTo.FormatWith("DbDate", "UInt16"));
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
			throw new InvalidCastException(SR.InvalidCast_FromTo.FormatWith("DbDate", "Single"));
		}

		double IConvertible.ToDouble(IFormatProvider provider)
		{
			throw new InvalidCastException(SR.InvalidCast_FromTo.FormatWith("DbDate", "Double"));
		}

		decimal IConvertible.ToDecimal(IFormatProvider provider)
		{
			throw new InvalidCastException(SR.InvalidCast_FromTo.FormatWith("DbDate", "Decimal"));
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