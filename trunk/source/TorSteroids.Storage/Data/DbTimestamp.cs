using System;
using System.Globalization;
using System.Threading;
using TorSteroids.Common;

namespace TorSteroids.Storage.Data
{
    /// <summary>
    /// Database Time Stamp class.
    /// </summary>
    /// <remarks>Works internally with the help of a <see cref="DateTime"/> class and always UTC.</remarks>
    public sealed class DbTimeStamp
    {
        #region static creation

        private static long _lastTimeStamp = DateTime.UtcNow.Ticks;

        public static DbTimeStamp Now
        {
            get
            {
                long orig, newval;
                do
                {
                    orig = _lastTimeStamp;
                    long now = DateTime.UtcNow.Ticks;
                    newval = Math.Max(now, orig + 1);
                } while (Interlocked.CompareExchange
                             (ref _lastTimeStamp, newval, orig) != orig);

                return new DbTimeStamp(new DateTime(newval, DateTimeKind.Utc));
            }
        }

        #endregion

	    private const string Rfc2822DateTimePattern = "ddd, dd MMM yyyy HH':'mm':'ss zzz";

        private readonly DateTimeOffset _current;

        #region ctor's

		/// <summary>
		/// Initializes a new instance of the <see cref="DbTimeStamp"/> class.
		/// </summary>
		/// <param name="dateTimeOffset">The date time offset.</param>
		public DbTimeStamp(DateTimeOffset dateTimeOffset)
		{
			_current = dateTimeOffset;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DbTimeStamp"/> class.
		/// </summary>
		/// <param name="dateTimeString">
		/// The date-time string in the  format ddd, dd MMM yyyy HH':'mm':'ss zzz as specified by Rfc2822.
		/// See also http://www.ietf.org/rfc/rfc2822.txt?number=2822.
		/// </param>
		/// <exception cref="FormatException">If the <paramref name="dateTimeString"/> is not a parsable date-time format</exception>
	    public DbTimeStamp(string dateTimeString)
		{
			dateTimeString.ExceptionIfNullOrEmpty("dateTimeString");
			
			var parsed = DateTimeExt.ParseRfc2822DateTime(dateTimeString);
			_current = new DateTimeOffset(parsed.ToLocalTime());
		}

        private DbTimeStamp(DateTime dateTime)
        {
	        var local = dateTime.Kind == DateTimeKind.Utc ? dateTime.ToLocalTime() : dateTime;
			
			_current = new DateTimeOffset(local);
        }

        #endregion

        #region public properties

		public DateTimeOffset DateTimeOffset
		{
			get { return _current; }
		}

        #endregion

        #region public methods

        /// <summary>
        /// Returns a <see cref="System.String" /> representation of the DbTimeStamp in the 
		/// format ddd, dd MMM yyyy HH':'mm':'ss zzz as specified by Rfc2822.
		/// See also http://www.ietf.org/rfc/rfc2822.txt?number=2822.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
			return _current.ToString(Rfc2822DateTimePattern, CultureInfo.InvariantCulture);
        }

        #endregion

    }
}
