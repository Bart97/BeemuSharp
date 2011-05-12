using System;

namespace EOHax.Logging
{
	public enum LogLevel
	{
		Fatal   = 0,
		Error   = 1,
		Warning = 2,
		Info    = 3,
		Debug   = 4
	};

	public interface ILogger
	{
		LogLevel LogLevel { get; set; }

		void Log(LogLevel logLevel, string message, Exception ex = null);

		void LogDebug  (string message, Exception ex = null);
		void LogInfo   (string message, Exception ex = null);
		void LogWarning(string message, Exception ex = null);
		void LogError  (string message, Exception ex = null);
		void LogFatal  (string message, Exception ex = null);
	}
}
