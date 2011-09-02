using System;
using Db4objects.Db4o.Linq;
using EOHax.EO;
using EOHax.EO.Communication;

namespace EOHax.Programs.EOSERV.Handlers
{
	static class PlayersHandler
	{
		public static void HandleRequest(Packet packet, IClient client, bool fromQueue)
		{
			short count = (short)client.Server.Characters.Count;
			// TODO: Don't list hidden admins

			Packet reply = new Packet(PacketFamily.Init, PacketAction.Init);
			reply.AddChar((byte)EO.InitReply.Players);
			reply.AddShort(count);
			reply.AddBreak();

			foreach (Character character in client.Server.Characters)
			{
				reply.AddBreakString(character.Name);
				reply.AddBreakString(character.Title ?? "");
				reply.AddChar(0); // What's this?
				reply.AddChar((byte)EO.PaperdollIcon.HGM);
				reply.AddChar((byte)character.ClassId);
				reply.AddString("TAG");
				reply.AddBreak();
			}
			client.Send(reply);
		}
	}
}
