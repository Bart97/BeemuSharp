using System;
using EOHax.EO;
using EOHax.EO.Communication;

namespace EOHax.Programs.EOSERV.Handlers
{
	class SitHandler
	{
		// Player sitting on the flor / stanging
		[HandlerState(ClientState.Playing)]
		public static void HandleRequest(Packet packet, IClient client, bool fromQueue)
		{
			SitCommand command = (SitCommand)packet.GetChar();

			if (command == SitCommand.Sitting)
			{
				if (client.Character.SitState != SitState.Stand) throw new InvalidOperationException("Not standing.");
				client.Character.Sit(SitState.Floor);
			}
			else if (command == SitCommand.Standing)
			{
				if (client.Character.SitState != SitState.Floor) throw new InvalidOperationException("Not sitting on the floor.");
				client.Character.Stand();
			}
			else throw new ArgumentException("Invalid sit command");
		}
	}
}
