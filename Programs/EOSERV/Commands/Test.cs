using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EOHax.Programs.EOSERV.Commands
{
	static class TestCommand
	{
		public static void CommandMain(CommandParameters parameters, ICommandExecutor executor)
		{
			Program.Logger.LogInfo("Hello world!");
		}
	}
}
