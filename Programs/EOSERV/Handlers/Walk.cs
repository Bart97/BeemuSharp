using System;
using EOHax.EO;
using EOHax.EO.Communication;

namespace EOHax.Programs.EOSERV.Handlers
{
	static class WalkHandler
	{
		private static void WalkCommon(Packet packet, IClient client, bool fromQueue, WalkType walkType)
		{
			Direction direction = (Direction)packet.GetChar();
			//TODO: SpeedTimestamp
			//SpeedTimestamp timestamp = new SpeedTimestamp(packet.GetThree());
			int timestamp = packet.GetThree();
			byte x = packet.GetChar();
			byte y = packet.GetChar();
			
			client.Character.Walk(direction, walkType, true);

			if (client.Character.X != x || client.Character.Y != y)
				client.Character.Refresh();
		}

		// Player walking
		[HandlerState(ClientState.Playing)]
		public static void HandlePlayer(Packet packet, IClient client, bool fromQueue)
		{
			WalkCommon(packet, client, fromQueue, WalkType.Normal);
		}

		// Player walking (ghost)
		[HandlerState(ClientState.Playing)]
		public static void HandleSpec(Packet packet, IClient client, bool fromQueue)
		{
			WalkCommon(packet, client, fromQueue, WalkType.Ghost);
		}

		// Player walking (#nowall)
		[HandlerState(ClientState.Playing)]
		public static void HandleAdmin(Packet packet, IClient client, bool fromQueue)
		{
			WalkCommon(packet, client, fromQueue, WalkType.Admin);
		}
	}
}
