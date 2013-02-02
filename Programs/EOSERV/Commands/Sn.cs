using EOHax.EO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EOHax.Programs.EOSERV.Commands
{
	static class SnCommand
	{
		[ArgumentAmount(1)]
		[AccessLevel(AccessLevel.GM)]
		public static void CommandMain(CommandParameters parameters, ICommandExecutor executor)
		{
			short DataId = (short)parameters.Get<int>(0);

			NPC npc = new NPC(executor.Server, DataId, ((Character)executor).MapId, ((Character)executor).X, ((Character)executor).Y);
			((Character)executor).Map.Enter(npc, WarpAnimation.None);
		}
	}
}
