using System;

namespace EOHax.Logging
{
	public enum LogLevel
	{
		Fatal   = 0,
		Error   = 1,
		Warning = 2,
        Success = 3,
		Info    = 4,
		Debug   = 5
	};

	public interface ILogger
	{
		LogLevel LogLevel { get; set; }

		void Log(LogLevel logLevel, string message, Exception ex = null);

		void LogDebug  (string message, Exception ex = null);
		void LogInfo   (string message, Exception ex = null);
        void LogSuccess(string message, Exception ex = null);
		void LogWarning(string message, Exception ex = null);
		void LogError  (string message, Exception ex = null);
		void LogFatal  (string message, Exception ex = null);
	}
}
