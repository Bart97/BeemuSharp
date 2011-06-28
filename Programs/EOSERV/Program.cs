using System;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using EOHax.Logging;
using Microsoft.WindowsAPICodePack.Taskbar;

namespace EOHax.Programs.EOSERV
{
	public static class Program
	{
		private static IServer server;

		public static ILogger Logger { get; private set; }

		public static TaskbarManager Taskbar { get; private set; }

		public static void Main(string[] args)
		{
			Logger = new ConsoleLogger();
#if DEBUG
			Logger.LogLevel = LogLevel.Debug;
#endif
			Logger.LogDebug("Test");
			try
			{
				Taskbar = TaskbarManager.Instance;
			}
			catch (PlatformNotSupportedException)
			{
				// Skip the taskbar stuff
			}
			Console.WriteLine(
@"Thanks to: ____    ___    ___     ________    _   _      __   __     
Apollo    /  _ \  / _ \  / _ \   /  _   _ \  / / / / ___/ /__/ /__  Based on
Sausage  / /_/ / /  __/ /  __/  / / / / / / / \_/ / /__/ /__/ /__/  EOSERV#
        /  _ <   \___/  \___/  /_/ /_/ /_/  \__,_/ ___/ /__/ /__      by
       / /_/ /  Beemu# version 1.0.0.0            /__/ /__/ /__/    Sausage
      /_____/   by Bart <bart-networks.eu>          /_/  /_/    <tehsausage.com>");
			// TODO: Make the "Basesd on EOSERV# by Sausage <tehsausage.com> look better. Probably move it to a new line.

			Console.Title = String.Format("Beemu# version {0}", Assembly.GetExecutingAssembly().GetName().Version.ToString());
#if DEBUG
			Console.Title += " debug";
#endif

			server = new Server(IPAddress.Any, 8078);
			server.Start();

			SetConsoleCtrlHandler(new HandlerRoutine(ConsoleClose), true);
		}
		private static bool ConsoleClose(CtrlTypes ctrlType) 
		{
			server.Shutdown(5);
			return true;
		}

#region SetConsoleCtrlHandler
		[DllImport("Kernel32")]
		public static extern bool SetConsoleCtrlHandler(HandlerRoutine Handler, bool Add);

		// A delegate type to be used as the handler routine
		// for SetConsoleCtrlHandler.
		public delegate bool HandlerRoutine(CtrlTypes CtrlType);

		// An enumerated type for the control messages
		// sent to the handler routine.
		public enum CtrlTypes
		{
			CTRL_C_EVENT = 0,
			CTRL_BREAK_EVENT,
			CTRL_CLOSE_EVENT,
			CTRL_LOGOFF_EVENT = 5,
			CTRL_SHUTDOWN_EVENT
		}
#endregion
	}
}
