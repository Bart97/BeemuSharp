using System;
using EOHax.EO;
using EOHax.EO.Communication;

namespace EOHax.Programs.EOSERV.Handlers
{
	static class FaceHandler
	{
		// Player changing direction
		[HandlerState(ClientState.Playing)]
		public static void HandlePlayer(Packet packet, IClient client, bool fromQueue)
		{
			Direction direction = (Direction)packet.GetChar();

			if (!Enum.IsDefined(typeof(Direction), direction))
				throw new ArgumentOutOfRangeException("Face direction out of range");

			client.Character.Face(direction, false);
		}
	}
}
