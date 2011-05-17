using System;
using System.Net;
using System.Reflection;
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
            Console.WriteLine(
@"
 ______   ____     _____ ______ _______      __ _  _   
|  ____| / __ \   / ____|  ____|  __ \ \    / /| || |_ 
| |__   | |  | | | (___ | |__  | |__) \ \  / /_  __  _|
|  __|  | |  | |  \___ \|  __| |  _  / \ \/ / _| || |_ 
| |____ | |__| |  ____) | |____| | \ \  \  / |_  __  _|
|______| \____/  |_____/|______|_|  \_\  \/    |_||_|  ");
            Console.Title = "EOSERV# build ";
            Console.Title += Assembly.GetExecutingAssembly().GetName().Version.Revision;

			server = new Server(IPAddress.Any, 8078);
			server.Start();
		}
	}
}
