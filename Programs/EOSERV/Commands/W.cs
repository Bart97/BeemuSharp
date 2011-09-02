using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EOHax.Programs.EOSERV.Commands
{
	static class WCommand
	{
		[ArgumentAmount(3)]
		[AccessLevel(AccessLevel.Guardian)]
		[PlayerOnly()]
		public static void CommandMain(CommandParameters parameters, ICommandExecutor executor)
		{
			if (parameters.Get<int>(0) > 0)
			{
				((Character)executor).Warp((ushort)parameters.Get<int>(1), (byte)parameters.Get<int>(2), (byte)parameters.Get<int>(3), EO.WarpAnimation.Admin);
			}
		}
	}
}
