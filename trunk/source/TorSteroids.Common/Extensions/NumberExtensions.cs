#region Version Info Header
/*
 * $Id: NumberExtensions.cs 85556 2013-03-13 13:33:33Z unknown $
 * $HeadURL: https://torsteroids.svn.codeplex.com/svn/trunk/source/TorSteroids.Common/Extensions/NumberExtensions.cs $
 * Last modified by $Author: unknown $
 * Last modified at $Date: 2013-03-13 14:33:33 +0100 (Mi, 13 Mrz 2013) $
 * $Revision: 85556 $
 */
#endregion

// ReSharper disable CheckNamespace
namespace System
// ReSharper restore CheckNamespace
{
	public static class NumberExtensions
	{
		[CLSCompliant(false)]
		public static UInt32 R(this UInt32 instance, int pos)
		{
			return Rr(instance, pos);
		}
		[CLSCompliant(false)]
		public static UInt16 R(this UInt16 instance, int pos)
		{
			return Rr(instance, pos);
		}
		public static Byte R(this Byte instance, int pos)
		{
			return Rr(instance, pos);
		}


		[CLSCompliant(false)]
		public static UInt32 L(this UInt32 instance, int pos)
		{
			return Rl(instance, pos);
		}
		[CLSCompliant(false)]
		public static UInt16 L(this UInt16 instance, int pos)
		{
			return Rl(instance, pos);
		}
		public static Byte L(this Byte instance, int pos)
		{
			return Rl(instance, pos);
		}

		private static UInt32 Rl(UInt32 i, int r)
		{
			r = r % 32;
			return ((i << r) | (i >> 32 - r));
		}
		private static UInt16 Rl(UInt16 i, int r)
		{
			r = r % 16;
			return (UInt16)((i << r) | (i >> 16 - r));
		}
		private static Byte Rl(Byte i, int r)
		{
			r = r % 8;
			return (Byte)((i << r) | (i >> 8 - r));
		}

		private static UInt32 Rr(UInt32 i, int r)
		{
			r = r % 32;
			return ((i >> r) | (i << 32 - r));
		}
		private static UInt16 Rr(UInt16 i, int r)
		{
			r = r % 16;
			return (UInt16)((i >> r) | (i << 16 - r));
		}
		private static Byte Rr(Byte i, int r)
		{
			r = r % 8;
			return (Byte)((i >> r) | (i << 8 - r));
		}
	}
}