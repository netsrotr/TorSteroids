using System;
using System.Data.Common;

// ReSharper disable CheckNamespace
namespace TorSteroids.Storage
// ReSharper restore CheckNamespace
{
	public interface ITrace
	{
		Guid GetMarker();
		void Write(Guid marker, DbCommand cmd);
		void Write(Guid marker, string format, params object[] data);
		void Separator();
	}
}
