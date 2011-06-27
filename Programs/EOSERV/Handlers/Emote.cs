using System;
using EOHax.EO;
using EOHax.EO.Communication;

namespace EOHax.Programs.EOSERV.Handlers
{
	class EmoteHandler
	{
		// Player sending an emote
		[HandlerState(ClientState.Playing)]
		public static void HandleReport(Packet packet, IClient client, bool fromQueue)
		{
			Emote emote = (Emote)packet.GetChar();
			if ((emote >= (Emote)0 && emote <= Emote.Embarassed) || emote == Emote.Trade || emote == Emote.Playful)
			{
				client.Character.Emote(emote, false);
			}
		}
	}
}
