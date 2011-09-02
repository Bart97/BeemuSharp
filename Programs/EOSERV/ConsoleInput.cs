using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace EOHax.Programs.EOSERV
{
	class ConsoleInput : ICommandExecutor
	{
		private IServer server;
		private Thread thread;

		public AccessLevel AccessLevel
		{
			get { return AccessLevel.Console; }
		}

		public IServer Server
		{
			get { return server; }
			private set { server = value; }
		}

		public ConsoleInput(IServer server)
		{
			Server = server;
			thread = new Thread(Read);
			Start();
		}

		public void Start()
		{
			if (!thread.IsAlive)
				thread.Start();
		}

		public void Stop()
		{
			thread.Abort();
		}

		public void Join()
		{
			thread.Join();
		}

		public void Read()
		{
			while (true)
			{
				String input = Console.ReadLine();
				try
				{
					CommandParser.Parse(input, this);
					/*String[] param = input.Split(' ');
					String command = param[0];
					CommandArguments args = new CommandArguments(input);
					param = param.Where((ValueType, idx) => idx != 0).ToArray();
					Program.Logger.LogDebug(String.Format("Command: {0}", command));
					CommandHandler.Handle(command, param, this);*/
				}
				catch (Exception ex)
				{
					Program.Logger.LogError("Exception was thrown when executing command.", ex);
				}
			}
		}
	}
}
