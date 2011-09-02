using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EOHax.EO;

namespace EOHax.Programs.EOSERV.Commands
{
	static class AdminCommand
	{
		[ArgumentAmount(2)]
		[AccessLevel(AccessLevel.Console)]
		public static void CommandMain(CommandParameters parameters, ICommandExecutor executor)
		{
			var targets = from Character c in executor.Server.Characters where c.Name == parameters.Get<String>(0) select c;
			if (targets.Count() == 0)
				throw new Exception("Target not found");
			Character target = targets.First<Character>();

			AdminLevel level = (AdminLevel)parameters.Get<int>(1);
			if (!Enum.IsDefined(typeof(AdminLevel), level))
				throw new Exception(String.Format("Admin level out of range ({0})", (int)level));
			target.Admin = level;
		}
	}
}
