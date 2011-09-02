using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EOHax.Programs.EOSERV
{
	public interface ICommandExecutor
	{
		AccessLevel AccessLevel { get; }
		IServer Server{ get; }
	}
}
