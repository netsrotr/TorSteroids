#region Version Info Header
/*
 * $Id: ExceptionExtensions.cs 85556 2013-03-13 13:33:33Z unknown $
 * $HeadURL: https://torsteroids.svn.codeplex.com/svn/trunk/source/TorSteroids.Common/Extensions/ExceptionExtensions.cs $
 * Last modified by $Author: unknown $
 * Last modified at $Date: 2013-03-13 14:33:33 +0100 (Mi, 13 Mrz 2013) $
 * $Revision: 85556 $
 */
#endregion


using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Text;
using TorSteroids.Common.Resources;

// ReSharper disable CheckNamespace
namespace System
// ReSharper restore CheckNamespace
{
	/// <summary>
	/// Extensions for Exception class and parameter check throw helpers
	/// </summary>
	public static class ExceptionExtensions
	{
		/// <summary>
		/// Used to preserve stack traces on rethrow
		/// </summary>
		private static readonly MethodInfo PreserveException = typeof (Exception).GetRuntimeMethod(
			"InternalPreserveStackTrace", new Type[] {});//, BindingFlags.Instance | BindingFlags.NonPublic);


		/// <summary>
		/// Calls the Exception's internal method to preserve its stack trace prior to rethrow
		/// </summary>
		/// <remarks>
		/// See http://weblogs.asp.net/fmarguerie/archive/2008/01/02/rethrowing-exceptions-and-preserving-the-full-call-stack-trace.aspx 
		/// for more info.
		/// </remarks>
		/// <param name="e"></param>
		public static void PreserveExceptionStackTrace(this Exception e)
		{
			if (e == null)
				throw new ArgumentNullException("e");

			PreserveException.Invoke(e, null);
		}

		/// <summary>
		/// Breaks and logs Exception and all its inner exceptions into one neatly formatted string.
		/// </summary>
		public static string ToDescriptiveString(this Exception ex)
		{
			var infoBuilder = new StringBuilder();

			if (ex.InnerException != null)
			{
				infoBuilder.Append(ex.InnerException.ToDescriptiveString());
				infoBuilder.Append(Environment.NewLine + Environment.NewLine +
				                   "- Nested Exception --------------------------------------" + Environment.NewLine +
				                   Environment.NewLine);
			}

			infoBuilder.AppendFormat(CultureInfo.InvariantCulture, "Exception:     {0}" + Environment.NewLine,
			                         ex.GetType());
			infoBuilder.AppendFormat(CultureInfo.InvariantCulture, "Message:       {0}" + Environment.NewLine,
			                         ex.Message);
			infoBuilder.AppendFormat(CultureInfo.InvariantCulture, "Data:          {0}" + Environment.NewLine + "{1}",
			                         ex.Data, ex.StackTrace);

			if (ex is ReflectionTypeLoadException)
			{
				foreach (var ex1 in (ex as ReflectionTypeLoadException).LoaderExceptions)
				{
					infoBuilder.Append(Environment.NewLine + Environment.NewLine +
					                   "- Loader Exception --------------------------------------" + Environment.NewLine +
					                   Environment.NewLine);
					infoBuilder.Append(ex1.ToDescriptiveString());
				}
			}

			return infoBuilder.ToString();
		}


		/// <summary>
		/// Throws an <see cref="ArgumentNullException"/> if the value is null.
		/// </summary>
		/// <typeparam name="T">The type of the value.</typeparam>
		/// <param name="value">The value to check for null.</param>
		/// <param name="argumentName">The name of the argument the value represents.</param>
		/// <exception cref="ArgumentNullException">The value is null.</exception>
		[DebuggerStepThrough]
		public static void ExceptionIfNull<T>(this T value, string argumentName) where T : class
		{
			if (value == null)
			{
				throw new ArgumentNullException(argumentName);
			}
		}

		/// <summary>
		/// Throws an <see cref="ArgumentNullException"/> if the value is null.
		/// </summary>
		/// <typeparam name="T">The type of the value.</typeparam>
		/// <param name="value">The value to check for null.</param>
		/// <param name="argumentName">The name of the argument the value represents.</param>
		/// <param name="message">A message for the exception.</param>
		/// <exception cref="ArgumentNullException">The value is null.</exception>
		[DebuggerStepThrough]
		public static void ExceptionIfNull<T>(this T value, string argumentName, string message) where T : class
		{
			if (value == null)
			{
				throw new ArgumentNullException(argumentName, message);
			}
		}

		/// <summary>
		/// Throws an <see cref="ArgumentNullException"/> if the specified value is null or
		/// throws an <see cref="ArgumentOutOfRangeException"/> if the specified value is an
		/// empty string.
		/// </summary>
		/// <param name="value">The string to check for null or empty.</param>
		/// <param name="argumentName">The name of the argument the value represents.</param>
		/// <exception cref="ArgumentNullException">The value is null.</exception>
		/// <exception cref="ArgumentOutOfRangeException">The value is an empty string.</exception>
		[DebuggerStepThrough]
		public static void ExceptionIfNullOrEmpty(this string value,
		                                          string argumentName)
		{
			value.ExceptionIfNull("value");

			if (value.Length == 0)
			{
				throw new ArgumentOutOfRangeException(argumentName, 
					SR.ArgumentOutOfRangeStringLengthExceptionMessage.FormatWith(argumentName ?? string.Empty));
			}
		}

		/// <summary>
		/// Throws an <see cref="ArgumentNullException"/> if the specified value is null or
		/// throws an <see cref="ArgumentOutOfRangeException"/> if the specified value is an
		/// empty string.
		/// </summary>
		/// <param name="value">The string to check for null or empty.</param>
		/// <param name="argumentName">The name of the argument the value represents.</param>
		/// <exception cref="ArgumentNullException">The value is null.</exception>
		/// <exception cref="ArgumentOutOfRangeException">The value is an empty string or only contains spaces.</exception>
		[DebuggerStepThrough]
		public static void ExceptionIfNullOrTrimmedEmpty(this string value,
			string argumentName)
		{
			value.ExceptionIfNullOrEmpty("value");

			if (value.Trim().Length == 0)
			{
				throw new ArgumentOutOfRangeException(argumentName, 
					SR.ArgumentOutOfRangeStringSpacesExceptionMessage.FormatWith( argumentName ?? string.Empty));
			}
		}

		/// <summary>
		/// Throws an <see cref="ArgumentOutOfRangeException"/> if the specified value is not within the range
		/// specified by the <paramref name="lowerBound"/> and <paramref name="upperBound"/> parameters.
		/// </summary>
		/// <typeparam name="T">The type of the value.</typeparam>
		/// <param name="value">The value to check that it's not out of range.</param>
		/// <param name="lowerBound">The lowest value that's considered being within the range.</param>
		/// <param name="upperBound">The highest value that's considered being within the range.</param>
		/// <param name="argumentName">The name of the argument the value represents.</param>
		/// <exception cref="ArgumentOutOfRangeException">The value is not within the given range.</exception>
		[DebuggerStepThrough]
		public static void ExceptionIfOutOfRange<T>(this T value,
		                                            T lowerBound, T upperBound, string argumentName)
			where T : IComparable<T>
		{
			if (value.CompareTo(lowerBound) < 0 || value.CompareTo(upperBound) > 0)
			{
				throw new ArgumentOutOfRangeException(argumentName, 
					SR.ArgumentOutOfRangeExceptionMessage.FormatWith(argumentName, lowerBound, upperBound));
			}
		}


		/// <summary>
		/// Throws an <see cref="ArgumentOutOfRangeException"/> if the specified value is not within the range
		/// specified by the <paramref name="lowerBound"/> and <paramref name="upperBound"/> parameters.
		/// </summary>
		/// <typeparam name="T">The type of the value.</typeparam>
		/// <param name="value">The value to check that it's not out of range.</param>
		/// <param name="lowerBound">The lowest value that's considered being within the range.</param>
		/// <param name="upperBound">The highest value that's considered being within the range.</param>
		/// <param name="argumentName">The name of the argument the value represents.</param>
		/// <param name="message">A message for the exception.</param>
		/// <exception cref="ArgumentOutOfRangeException">The value is not within the given range.</exception>
		[DebuggerStepThrough]
		public static void ExceptionIfOutOfRange<T>(this T value,
		                                            T lowerBound, T upperBound, string argumentName, string message)
			where T : IComparable<T>
		{
			if (value.CompareTo(lowerBound) < 0 || value.CompareTo(upperBound) > 0)
			{
				throw new ArgumentOutOfRangeException(argumentName, message);
			}
		}

		/// <summary>
		/// Throws an <see cref="IndexOutOfRangeException"/> if the specified index is not within the range
		/// specified by the <paramref name="lowerBound"/> and <paramref name="upperBound"/> parameters.
		/// </summary>
		/// <param name="index">The index to check that it's not out of range.</param>
		/// <param name="lowerBound">The lowest value considered being within the range.</param>
		/// <param name="upperBound">The highest value considered being within the range.</param>
		/// <param name="argumentName">The name of the argument the value represents.</param>
		/// <exception cref="IndexOutOfRangeException">The index is not within the given range.</exception>
		[DebuggerStepThrough]
		public static void ExceptionIfIndexOutOfRange(this int index,
		                                              int lowerBound, int upperBound, string argumentName)
		{
			if (index < lowerBound || index > upperBound)
			{
				throw new IndexOutOfRangeException(SR.IndexOutOfRangeExceptionMessage.FormatWith(argumentName, lowerBound, upperBound));
			}
		}

		/// <summary>
		/// Throws an <see cref="IndexOutOfRangeException"/> if the specified index is not within the range
		/// specified by the <paramref name="lowerBound"/> and <paramref name="upperBound"/> parameters.
		/// </summary>
		/// <param name="index">The index to check that it's not out of range.</param>
		/// <param name="lowerBound">The lowest value considered being within the range.</param>
		/// <param name="upperBound">The highest value considered being within the range.</param>
		/// <param name="argumentName">The name of the argument the value represents.</param>
		/// <exception cref="IndexOutOfRangeException">The index is not within the given range.</exception>
		[DebuggerStepThrough]
		public static void ExceptionIfIndexOutOfRange(this long index,
		                                              long lowerBound, long upperBound, string argumentName)
		{
			if (index < lowerBound || index > upperBound)
			{
				throw new IndexOutOfRangeException(SR.IndexOutOfRangeExceptionMessage.FormatWith(argumentName, lowerBound, upperBound));
			}
		}

	}
}