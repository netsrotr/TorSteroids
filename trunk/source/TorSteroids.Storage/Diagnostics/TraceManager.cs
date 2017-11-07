using System;
using System.Configuration;
using System.Data.Common;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;

namespace TorSteroids.Storage.Diagnostics
{
	public class TraceManager : ITrace
	{
		private static ITrace _instance;
		private static readonly object TraceLocker = new object();
		private static bool _tracingEnabled = false;
		private const string STORAGE_ENABLE_TRACING = "TorSteroids.Storage::EnableTracing";

		#region Constructors

		static TraceManager()
		{
			bool tracingEnabled = false;

            if (bool.TryParse(ConfigurationManager.AppSettings[STORAGE_ENABLE_TRACING], out tracingEnabled))
			{
				_tracingEnabled = tracingEnabled;
			}
		}

		public TraceManager()
		{
			try
			{
				Initialize();
			}
			catch
			{
				_tracingEnabled = false;
				_instance = null;
			}
		}

		#endregion

		#region private void Initialize()

		private void Initialize()
		{
			AssemblyName asmName = Assembly.GetExecutingAssembly().GetName();
			string logFileName = String.Format("TorSteroids.Storage.{0}.log", asmName.Version);
		    string currentPath = Environment.CurrentDirectory;

			// enable tracing for this process.
			string logFilePath = Path.Combine(Path.GetTempPath(), logFileName);

			FileStream fs = new FileStream(logFilePath, FileMode.OpenOrCreate | FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
			StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);

			TextWriterTraceListener txtListener = new TextWriterTraceListener(sw, "txt_listener");

			Trace.Listeners.Add(txtListener);
			Trace.AutoFlush = true;

			Separator();

			Trace.WriteLine(asmName.FullName);

			Trace.Write("Application Domain Path: ");
			Trace.WriteLine(currentPath);

			Trace.Write("Started from: ");
			Trace.WriteLine(logFilePath);

			Trace.Write("Starting at: ");
			Trace.WriteLine(DateTime.Now.ToString("s"));

			Trace.Write("Executing Command Line: ");
			Trace.WriteLine(Environment.CommandLine);

			Trace.Write("Machine Name: ");
			Trace.WriteLine(Environment.MachineName);

			Trace.Write("OS Version: ");
			Trace.WriteLine(Environment.OSVersion);

			Trace.Write("Dot.Net Version: ");
			Trace.WriteLine(Environment.Version);

			Separator();
		}

		#endregion

		#region public static ITrace Instance

		public static ITrace Instance
		{
			get
			{
				lock ( TraceLocker )
				{
				    return _instance ??
				           (_instance = (_tracingEnabled) ? new TraceManager() : (ITrace) (new TraceEmpty()));
				}
			}
		}

		#endregion

		#region private string GetCurrentDateTimeString()

		private string GetCurrentDateTimeString()
		{
			return DateTime.Now.ToString( "yyyy-MM-dd hh:mm:ss.ffffff" ) + " - ";
		}

		#endregion

		#region public Guid GetMarker()

		public Guid GetMarker()
		{
			return Guid.NewGuid();
		}

		#endregion

		#region public void Write(...) overloads

		public void Write(Guid marker, DbCommand cmd)
		{
			string sql = cmd.CommandText;
			StringBuilder sb = new StringBuilder();

			sb.Append( GetCurrentDateTimeString() )
				.Append("Marker: ")
				.Append(marker.ToString("D"))
				.Append(", Current SQL hash: ")
				.AppendLine(sql.GetHashCode().ToString(CultureInfo.InvariantCulture))
				.Append("\tStatement: ")
				.AppendLine(sql);
			if ( cmd.Parameters.Count > 0 )
			{
				sb.AppendLine("\tParameters:");
				foreach( DbParameter p in cmd.Parameters )
				{
					try
					{
						object o = ( p.Value is DBNull ) ? "NULL" : (p.Value ?? "null");
						sb.AppendLine(string.Format("\t\tName = {0}, Type = {1}, Value = {2}", p.ParameterName, p.DbType, o));
					}
					catch (Exception e)
					{
						Trace.WriteLine( string.Format("Error thrown: {0}{1}SQL statement: {2}", e, Environment.NewLine, sql) );
					}
				}
			}
			Trace.Write(sb.ToString());
		}

		void WriteLine(Guid marker, string data)
		{
			StringBuilder sb = new StringBuilder();
			sb.Append( GetCurrentDateTimeString() )
				.Append("Marker: ")
				.Append(marker.ToString("D"))
				.Append(", Data: ")
				.Append(data);
			Trace.WriteLine(sb.ToString());
		}

		public void Write(Guid marker, string format, params object [] data)
		{
			WriteLine(marker, string.Format(format, data));
		}

		#endregion

		#region public void Separator()

		public void Separator()
		{
			Trace.WriteLine(new System.String('*', 76));
		}

		#endregion
	}
}
