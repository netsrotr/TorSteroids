using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace TorSteroids.Common.Extensions
{
	public static class AsyncExtensions
	{
		public static async Task<T> TimeoutAfter<T>(this Task<T> task, TimeSpan timeout, CancellationTokenSource cancellationTokenSource = null)
		{
			var timeoutCancellationTokenSource = new CancellationTokenSource();

			var completedTask = await Task.WhenAny(task, Task.Delay(timeout, timeoutCancellationTokenSource.Token));
			if (completedTask == task)
			{
				timeoutCancellationTokenSource.Cancel();
				return await task;  // Very important in order to propagate exceptions
			}

			if (cancellationTokenSource != null)
				cancellationTokenSource.Cancel();

			throw new TimeoutException();
		}
	}
}
