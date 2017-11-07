#region Version Info Header
/*
 * $Id: Bit.cs 85556 2013-03-13 13:33:33Z unknown $
 * $HeadURL: https://torsteroids.svn.codeplex.com/svn/trunk/source/TorSteroids.Common/Bit.cs $
 * Last modified by $Author: unknown $
 * Last modified at $Date: 2013-03-13 14:33:33 +0100 (Mi, 13 Mrz 2013) $
 * $Revision: 85556 $
 */
#endregion

using System;
using System.Globalization;

namespace TorSteroids.Common
{
    /// <summary>
    /// Helper class for bit manipulation operations
    /// </summary>
    public static class Bit
    {

        #region UnSet

        /// <summary>
        /// Unset one or more flags in [Flags]<paramref name="flaggedEnum"/>.
        /// </summary>
        /// <param name="flaggedEnum">The flagged <see cref="Enum"/>.</param>
        /// <param name="flags">The flags to clear (unset).</param>
        /// <returns>The new <see cref="Enum"/> with the flags cleared</returns>
        /// <remarks>If you need the more speedy version of this bit manipulations,
        /// please use the overload(s) with long or int parameters!</remarks>
        public static Enum UnSet(Enum flaggedEnum, params Enum[] flags)
        {
            long w = Int64.Parse(flaggedEnum.ToString("d"), NumberStyles.Integer);
            foreach (Enum f in flags)
                w = UnSet(w, Int64.Parse(f.ToString("d"), NumberStyles.Integer));

            return (Enum)Enum.Parse(flaggedEnum.GetType(), w.ToString(CultureInfo.InvariantCulture), true);
        }

        /// <summary>
        /// Unset one or more flags in <paramref name="flagVariable"/>.
        /// </summary>
        /// <param name="flagVariable">The flag variable.</param>
        /// <param name="flags">The flags to clear (unset).</param>
        /// <returns>The new long with the flags set</returns>
        public static long UnSet(long flagVariable, params long[] flags)
        {
            long v = flagVariable;
            foreach (long f in flags)
                v &= ~f;
            return v;
        }

        /// <summary>
        /// Unset one or more flags in <paramref name="flagVariable"/>.
        /// </summary>
        /// <param name="flagVariable">The flag variable.</param>
        /// <param name="flags">The flags to clear (unset).</param>
        /// <returns>The new long with the flags set</returns>
        public static int UnSet(int flagVariable, params int[] flags)
        {
            int v = flagVariable;
            foreach (int f in flags)
                v &= ~f;
            return v;
        }

        /// <summary>
        /// Unset one or more flags in <paramref name="flagVariable"/>.
        /// </summary>
        /// <param name="flagVariable">The flag variable.</param>
        /// <param name="flags">The flags to clear (unset).</param>
        /// <returns>The new long with the flags set</returns>
        public static byte UnSet(byte flagVariable, params int[] flags)
        {
            byte v = flagVariable;
            foreach (int f in flags)
                v &= (byte)~f;
            return v;
        }
        #endregion

        #region Set

        /// <summary>
        /// Set one or more flags in [Flags]<paramref name="flaggedEnum"/>.
        /// </summary>
        /// <param name="flaggedEnum">The flagged <see cref="Enum"/>.</param>
        /// <param name="flags">The flags to set.</param>
        /// <returns>The new <see cref="Enum"/> with the flags set</returns>
        /// <remarks>If you need the more speedy version of this bit manipulations,
        /// please use the overload(s) with long or int parameters!</remarks>
        public static Enum Set(Enum flaggedEnum, params Enum[] flags)
        {
            long w = Int64.Parse(flaggedEnum.ToString("d"), NumberStyles.Integer);
            foreach (Enum f in flags)
                w = Set(w, Int64.Parse(f.ToString("d"), NumberStyles.Integer));

			return (Enum)Enum.Parse(flaggedEnum.GetType(), w.ToString(CultureInfo.InvariantCulture), true);
        }

        /// <summary>
        /// Set one or more flags in <paramref name="flagVariable"/>.
        /// </summary>
        /// <param name="flagVariable">The flag variable.</param>
        /// <param name="flags">The flags.</param>
        /// <returns></returns>
        public static long Set(long flagVariable, params long[] flags)
        {
            long v = flagVariable;
            foreach (long f in flags)
                v |= f;
            return v;
        }

        /// <summary>
        /// Set one or more flags in <paramref name="flagVariable"/>.
        /// </summary>
        /// <param name="flagVariable">The flag variable.</param>
        /// <param name="flags">The flags.</param>
        /// <returns></returns>
        public static int Set(int flagVariable, params int[] flags)
        {
            int v = flagVariable;
            foreach (int f in flags)
                v |= f;
            return v;
        }

        /// <summary>
        /// Set one or more flags in <paramref name="flagVariable"/>.
        /// </summary>
        /// <param name="flagVariable">The flag variable.</param>
        /// <param name="flags">The flags.</param>
        /// <returns></returns>
        public static byte Set(byte flagVariable, params int[] flags)
        {
            byte v = flagVariable;
            foreach (var f in flags)
                v |= (byte)f;
            return v;
        }

        #endregion

        #region Toggle

        /// <summary>
        /// Set or unset one or more flags in [Flags]<paramref name="flaggedEnum"/>
        /// depending on the predicate value.
        /// </summary>
        /// <param name="predicate">if set to <c>true</c> [predicate].</param>
        /// <param name="flaggedEnum">The flagged <see cref="Enum"/>.</param>
        /// <param name="flags">The flags to set.</param>
        /// <returns>The new <see cref="Enum"/> with the flags set</returns>
        /// <remarks>If you need the more speedy version of this bit manipulations,
        /// please use the overload(s) with long or int parameters!</remarks>
        public static Enum Toggle(bool predicate, Enum flaggedEnum, params Enum[] flags)
        {
            if (predicate)
                return Set(flaggedEnum, flags);
            return UnSet(flaggedEnum, flags);
        }

        /// <summary>
        /// Set or unset one or more flags in <paramref name="flagVariable"/>,
        /// depending on the predicate value.
        /// </summary>
        /// <param name="predicate">if set to <c>true</c> [predicate].</param>
        /// <param name="flagVariable">The flag variable.</param>
        /// <param name="flags">The flags.</param>
        /// <returns></returns>
        public static long Toggle(bool predicate, long flagVariable, params long[] flags)
        {
            if (predicate)
                return Set(flagVariable, flags);
            return UnSet(flagVariable, flags);
        }

        /// <summary>
        /// Set or unset one or more flags in <paramref name="flagVariable"/>,
        /// depending on the predicate value.
        /// </summary>
        /// <param name="predicate">if set to <c>true</c> [predicate].</param>
        /// <param name="flagVariable">The flag variable.</param>
        /// <param name="flags">The flags.</param>
        /// <returns></returns>
        public static int Toggle(bool predicate, int flagVariable, params int[] flags)
        {
            if (predicate)
                return Set(flagVariable, flags);
            return UnSet(flagVariable, flags);
        }

        /// <summary>
        /// Set or unset one or more flags in <paramref name="flagVariable"/>,
        /// depending on the predicate value.
        /// </summary>
        /// <param name="predicate">if set to <c>true</c> [predicate].</param>
        /// <param name="flagVariable">The flag variable.</param>
        /// <param name="flags">The flags.</param>
        /// <returns></returns>
        public static byte Toggle(bool predicate, byte flagVariable, params int[] flags)
        {
            if (predicate)
                return Set(flagVariable, flags);
            return UnSet(flagVariable, flags);
        }

        #endregion

        #region IsSet

        /// <summary>
        /// Determines whether the specified <paramref name="flag"/> 
        /// in the [Flags]<paramref name="flaggedEnum"/> <see cref="Enum"/> is set.
        /// </summary>
        /// <param name="flaggedEnum">The flagged <see cref="Enum"/>.</param>
        /// <param name="flag">The flag.</param>
        /// <returns>
        /// 	<c>true</c> if the specified flagged <see cref="Enum"/> is set; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsSet(Enum flaggedEnum, Enum flag)
        {
            long v = Int64.Parse(flag.ToString("d"), NumberStyles.Integer);
            long w = Int64.Parse(flaggedEnum.ToString("d"), NumberStyles.Integer);
            return IsSet(w, v);
        }

        /// <summary>
        /// Determines whether the specified <paramref name="flag"/> 
        /// in the <paramref name="flagVariable"/> is set.
        /// </summary>
        /// <param name="flagVariable">The flag variable.</param>
        /// <param name="flag">The flag.</param>
        /// <returns>
        /// 	<c>true</c> if the specified flag variable is set; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsSet(long flagVariable, long flag)
        {
            return ((flagVariable & flag) == flag);
        }

        /// <summary>
        /// Determines whether the specified <paramref name="flag"/> 
        /// in the <paramref name="flagVariable"/> is set.
        /// </summary>
        /// <param name="flagVariable">The flag variable.</param>
        /// <param name="flag">The flag.</param>
        /// <returns>
        /// 	<c>true</c> if the specified flag variable is set; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsSet(int flagVariable, int flag)
        {
            return ((flagVariable & flag) == flag);
        }

        #endregion

    }
}
