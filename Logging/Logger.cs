using System;
using System.Collections.Generic;
using System.Text;

namespace EOHax.Logging
{
	public abstract class Logger : ILogger
	{
		public LogLevel LogLevel { get; set; }

		public Logger()
		{
			LogLevel = LogLevel./*Info*/Debug;
		}

		public abstract void Log(LogLevel logLevel, string message, Exception ex = null);

		public void LogDebug(string message, Exception ex = null)
		{
			Log(LogLevel.Debug, message, ex);
		}

		public void LogInfo(string message, Exception ex = null)
		{
			Log(LogLevel.Info, message, ex);
		}

        public void LogSuccess(string message, Exception ex = null)
        {
            Log(LogLevel.Success, message, ex);
        }

		public void LogWarning(string message, Exception ex = null)
		{
			Log(LogLevel.Warning, message, ex);
		}

		public void LogError(string message, Exception ex = null)
		{
			Log(LogLevel.Error, message, ex);
		}

		public void LogFatal(string message, Exception ex = null)
		{
			Log(LogLevel.Fatal, message, ex);
		}

	}
}
