#region Version Info Header
/*
 * $Id: FlaggedEnumExtensions.cs 85556 2013-03-13 13:33:33Z unknown $
 * $HeadURL: https://torsteroids.svn.codeplex.com/svn/trunk/source/TorSteroids.Common/Extensions/FlaggedEnumExtensions.cs $
 * Last modified by $Author: unknown $
 * Last modified at $Date: 2013-03-13 14:33:33 +0100 (Mi, 13 Mrz 2013) $
 * $Revision: 85556 $
 */
#endregion

using System.Globalization;

// ReSharper disable CheckNamespace
namespace System
// ReSharper restore CheckNamespace
{
	public static class FlaggedEnumExtensions
	{
		/// <summary>
		/// Clears the flag.
		/// </summary>
		/// <typeparam name="T">Value type: Enum</typeparam>
		/// <param name="extendee">The extendee.</param>
		/// <param name="flags">The flags.</param>
		/// <returns></returns>
		public static T ClearFlag<T>(this Enum extendee, params T[] flags) where T : struct
		{
			ulong w = UInt64.Parse(extendee.ToString("d"), NumberStyles.Integer);
			foreach (object f in flags)
			{
				if (extendee.GetType() != f.GetType())
					throw new ArgumentException("Enum types does not match!");
				ulong vf = UInt64.Parse(((Enum)f).ToString("d"), NumberStyles.Integer);
				w &= ~vf;
			}

			object rr = Enum.Parse(extendee.GetType(), w.ToString(), true);

			return (T)rr;
		}

		/// <summary>
		/// Sets the flag.
		/// </summary>
		/// <typeparam name="T">Value type: Enum</typeparam>
		/// <param name="extendee">The extendee.</param>
		/// <param name="flags">The flags.</param>
		/// <returns></returns>
		public static T SetFlag<T>(this Enum extendee, params T[] flags) where T : struct
		{
			ulong w = UInt64.Parse(extendee.ToString("d"), NumberStyles.Integer);
			foreach (object f in flags)
			{
				if (extendee.GetType() != f.GetType())
					throw new ArgumentException("Enum types does not match!");
				ulong vf = UInt64.Parse(((Enum)f).ToString("d"), NumberStyles.Integer);
				w |= vf;
			}

			object rr = Enum.Parse(extendee.GetType(), w.ToString(), true);

			return (T)rr;
		}

		/// <summary>
		/// Set or unset one or more flags in [Flags]<paramref name="extendee"/>
		/// depending on the predicate value.
		/// </summary>
		/// <typeparam name="T">Value type: Enum</typeparam>
		/// <param name="extendee">The extendee.</param>
		/// <param name="predicate">The condition how to set the flags.</param>
		/// <param name="flags">The flags to toggle.</param>
		/// <returns></returns>
		public static T ToggleFlag<T>(this Enum extendee, bool predicate, params T[] flags) where T : struct
		{
			if (predicate)
				return SetFlag(extendee, flags);
			return ClearFlag(extendee, flags);
		}

	}
}