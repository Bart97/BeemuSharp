using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EOHax.Programs.EOSERV.Commands
{
	static class StCommand
	{
		[ArgumentAmount(1)]
		public static void CommandMain(CommandParameters parameters, ICommandExecutor executor)
		{
			var targets = from Character c in executor.Server.Characters where c.Name == parameters.Get<string>(0) select c;
			if (targets.Count() == 0)
				throw new Exception("Target not found");
			Character target = targets.First<Character>();

			String title = parameters.Get<String>(1, "");
			target.Title = title;
		}
	}
}
