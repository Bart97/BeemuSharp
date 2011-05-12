using System;
using System.Net;
using EOHax.Logging;

namespace EOHax.Programs.EOSERV
{
	public static class Program
	{
		private static IServer server;

		public static ILogger Logger { get; private set; }

		public static void Main(string[] args)
		{
			Logger = new ConsoleLogger();
			server = new Server(IPAddress.Any, 8078);
			server.Start();
		}
	}
}
