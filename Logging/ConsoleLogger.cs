using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;

namespace EOHax.Logging
{
	public class ConsoleLoggerLevelConfig
	{
		public TextWriter Stream             { get; set; }
		public bool ShowException            { get; set; }
		public bool ShowStackTrace           { get; set; }
		public ConsoleColor? ForegroundColor { get; set; }
		public ConsoleColor? BackgroundColor { get; set; }

		public ConsoleLoggerLevelConfig()
		{
			Stream = Console.Out;
			ShowException = false;
			ShowStackTrace = false;
			ForegroundColor = null;
			BackgroundColor = null;
		}
	}

	public class ConsoleLogger : Logger
	{
		public string Format { get; set; }

		public Dictionary<LogLevel, ConsoleLoggerLevelConfig> LevelConfig { get; private set; }

		public ConsoleLogger()
		{
			Format = "[{0:G}] [{1:8}] {2}";
			LevelConfig = new Dictionary<LogLevel, ConsoleLoggerLevelConfig>();

			foreach (LogLevel level in Enum.GetValues(typeof(LogLevel)))
			{
				LevelConfig[level] = new ConsoleLoggerLevelConfig();
			}

            LevelConfig[LogLevel.Info].ForegroundColor = ConsoleColor.White;

            LevelConfig[LogLevel.Success].ForegroundColor = ConsoleColor.Green;

			LevelConfig[LogLevel.Warning].ForegroundColor = ConsoleColor.Yellow;
			LevelConfig[LogLevel.Warning].BackgroundColor = ConsoleColor.Black;

			LevelConfig[LogLevel.Error].ShowException = true;
			LevelConfig[LogLevel.Error].ForegroundColor = ConsoleColor.Red;
			LevelConfig[LogLevel.Error].BackgroundColor = null;

			LevelConfig[LogLevel.Fatal].ShowException = true;
			LevelConfig[LogLevel.Fatal].ShowStackTrace = true;
			LevelConfig[LogLevel.Fatal].ForegroundColor = ConsoleColor.Red;
			LevelConfig[LogLevel.Fatal].BackgroundColor = null;
		}

		public override void Log(LogLevel logLevel, string message, Exception ex = null)
		{
			if (logLevel > LogLevel)
				return;

			ConsoleLoggerLevelConfig levelConfig = LevelConfig[logLevel];

			if (levelConfig.ForegroundColor.HasValue)
				Console.ForegroundColor = levelConfig.ForegroundColor.Value;

			if (levelConfig.BackgroundColor.HasValue)
				Console.BackgroundColor = levelConfig.BackgroundColor.Value;

			Console.WriteLine(Format, DateTime.Now, logLevel.ToString(), message);

			if (ex != null && levelConfig.ShowException)
			{
				if (levelConfig.ShowStackTrace)
					Console.WriteLine(ex);
				else
					Console.WriteLine("{0}: {1}", ex.GetType().Name, ex.Message);
			}

			Console.ResetColor();
		}
	}
}
