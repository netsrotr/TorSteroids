#region Version Info Header
/*
 * $Id: StringExtensions.cs 87185 2013-08-29 11:20:20Z unknown $
 * $HeadURL: https://torsteroids.svn.codeplex.com/svn/trunk/source/TorSteroids.Common/Extensions/StringExtensions.cs $
 * Last modified by $Author: unknown $
 * Last modified at $Date: 2013-08-29 13:20:20 +0200 (Do, 29 Aug 2013) $
 * $Revision: 87185 $
 */
#endregion

// ReSharper disable CheckNamespace
namespace System
// ReSharper restore CheckNamespace
{
	public static class StringExtensions
	{
		#region Format

		/// <summary>
		/// Formats the string with the provided placeholder arguments.
		/// </summary>
		/// <param name="format">The format.</param>
		/// <param name="args">The arguments.</param>
		/// <returns></returns>
		public static string FormatWith(this string format, params object[] args)
		{
			return String.Format(format, args);
		}

		#endregion

		#region EndsWith

		public static bool EndsWithOrdinal(this String instance, String other)
		{
			return instance.EndsWith(other, StringComparison.Ordinal);
		}

		public static bool EndsWithOrdinalIgnoreCase(this String instance, String other)
		{
			return instance.EndsWith(other, StringComparison.OrdinalIgnoreCase);
		}

		#endregion

		#region Equals

		public static bool EqualsOrdinal(this String instance, String other)
		{
			return String.Equals(instance, other, StringComparison.Ordinal);
		}

		public static bool EqualsOrdinalIgnoreCase(this String instance, String other)
		{
			return String.Equals(instance, other, StringComparison.OrdinalIgnoreCase);
		}

		public static bool EqualsCurrentCulture(this String instance, String other)
		{
			return String.Equals(instance, other, StringComparison.CurrentCulture);
		}

		public static bool EqualsCurrentCultureIgnoreCase(this String instance, String other)
		{
			return String.Equals(instance, other, StringComparison.CurrentCultureIgnoreCase);
		}

		#endregion

		#region IndexOf
		
		public static int IndexOfOrdinal(this String instance, String other, int startIndex = 0)
		{
			return instance.IndexOf(other, startIndex, StringComparison.Ordinal);
		}

		public static int IndexOfOrdinalIgnoreCase(this String instance, String other, int startIndex = 0)
		{
			return instance.IndexOf(other, startIndex, StringComparison.OrdinalIgnoreCase);
		}

		#endregion
	}
}