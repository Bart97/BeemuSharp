using System;
using EOHax.EO;
using EOHax.EO.Communication;

namespace EOHax.Programs.EOSERV.Handlers
{
	static class ConnectionHandler
	{
		// Reply from clients after initialization completes
		[HandlerState(ClientState.Initialized)]
		public static void HandleAccept(Packet packet, IClient client, bool fromQueue)
		{
			
		}

		// Ping reply
		public static void HandlePing(Packet packet, IClient client, bool fromQueue)
		{
			client.Pinged = false;
		}
	}
}
