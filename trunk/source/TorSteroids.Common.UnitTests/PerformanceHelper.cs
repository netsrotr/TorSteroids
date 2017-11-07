using System;
using System.Runtime.InteropServices;

namespace TorSteroids.Common.UnitTests
{
	/// <summary>
	/// Helper class to profile/measure some long running parts 
	/// of an application.
	/// </summary>
	/// <example> The following code example measure a part of long running code.
	/// The call to <see cref="StopMeasureString">StopMeasureString</see> returns 
	/// a formatted string with this format: <code>{0:0.###} sec(s)</code>.
	/// <code>
	/// long measureVar = 0;
	/// PerformanceHelper.StartMeasure(ref measureVar);
	/// ... // long running part of the code
	/// Trace.WriteLine("Long running part needs "+PerformanceHelper.StopMeasureString(measureVar));
	/// </code>
	/// </example>
	public static class PerformanceHelper
	{

		private static readonly long seqFreq;

		/// <summary>
		/// Static class Initializer.
		/// </summary>
		static PerformanceHelper()
		{
			QueryPerformance.SafeNativeMethods.QueryPerformanceFrequency(ref seqFreq);
		}

		/// <summary>
		/// Start the measure process by calling QueryPerformanceCounter() API
		/// function.
		/// </summary>
		/// <param name="secStart">Reference parameter to store the starting info</param>
		public static void StartMeasure(ref long secStart)
		{
			QueryPerformance.SafeNativeMethods.QueryPerformanceCounter(ref secStart);
		}

		/// <summary>
		/// Return the calculated time in seconds as a double.
		/// </summary>
		/// <param name="secStart">The variable you provided to 
		/// <see cref="StartMeasure">StartMeasure</see>.</param>
		/// <returns>Calculated time in seconds</returns>
		public static double StopMeasure(long secStart)
		{
			long secTiming = 0;
			QueryPerformance.SafeNativeMethods.QueryPerformanceCounter(ref secTiming);
			if (seqFreq == 0) return 0.0;   // Handle no high-resolution timer
			return (secTiming - secStart) / (double)seqFreq;
		}

		/// <summary>
		/// Return the calculated time in seconds as a formatted string.
		/// </summary>
		/// <param name="secStart">The variable you provided to 
		/// <see cref="StartMeasure">StartMeasure</see>.</param>
		/// <returns>Calculated time in seconds as a formatted string</returns>
		/// <remarks>The returned string is formatted with this mask: 
		/// <code>{0:0.###} sec(s)</code>.</remarks>
		public static string StopMeasureString(long secStart)
		{
			return String.Format("{0:0.###} sec(s)", StopMeasure(secStart));
		}

		internal static class QueryPerformance
		{
			internal static class SafeNativeMethods
			{
				[DllImport("KERNEL32")]
				[return: MarshalAs(UnmanagedType.Bool)]
				public static extern bool QueryPerformanceCounter(ref long lpPerformanceCount);

				[DllImport("KERNEL32")]
				[return: MarshalAs(UnmanagedType.Bool)]
				public static extern bool QueryPerformanceFrequency(ref long lpFrequency);
			}
		}
	}
}
