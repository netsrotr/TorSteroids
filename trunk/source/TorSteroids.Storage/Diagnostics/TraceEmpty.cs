using System;
using System.Data.Common;

namespace TorSteroids.Storage.Diagnostics
{
	public class TraceEmpty : ITrace
	{
		public Guid GetMarker()
		{
			return Guid.Empty;
		}

		public void Write(Guid marker, DbCommand cmd) { }
		public void Write(Guid marker, string format, params object[] data) { }
		public void Separator() { }
	}
}
