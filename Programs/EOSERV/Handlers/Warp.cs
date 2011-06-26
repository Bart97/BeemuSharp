using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EOHax.EO;
using EOHax.EO.Communication;

namespace EOHax.Programs.EOSERV.Handlers
{
    class WarpHandler
    {
        [HandlerState(ClientState.Playing)]
        public static void HandleAccept(Packet packet, IClient client, bool fromQueue)
        {
            if (!client.Character.Warping) throw new Exception("Unexpected warp");
            ushort map = (ushort)packet.GetShort();
            WarpAnimation animation = client.Character.WarpAnimation;

            Packet reply = new Packet(PacketFamily.Warp, PacketAction.Agree);
            reply.AddChar(2); // WarpReply?
            reply.AddShort((short)map);
            reply.AddChar((byte)animation);

            IEnumerable<Character> characters = client.Character.GetInRange<Character>();
            IEnumerable<MapItem> items = client.Character.GetInRange<MapItem>();
			
			reply.AddChar((byte)characters.Count());
			reply.AddBreak();

			// Characters
			// {
			foreach (Character character in characters)
			{
				character.InfoBuilder(ref reply);
				reply.AddBreak();
			}
			// }

			// NPCs
			reply.AddBreak();

			// Items
            foreach (MapItem item in items)
            {
                item.InfoBuilder(ref reply);
            }
            client.Send(reply);
            client.Character.Warping = false;
        }

        // Player requesting a copy of the map he's being warped to
        [HandlerState(ClientState.Playing)]
        public static void HandleTake(Packet packet, IClient client, bool fromQueue)
        {
            FileType fileType = (FileType)packet.GetChar();

            string fileName = client.Character.Map.Data.GetPubFile("./data/");

			Packet reply = new Packet(PacketFamily.Init, PacketAction.Init);
			reply.AddChar((byte)InitReply.Banned);

			reply.AddBytes(File.ReadAllBytes(fileName));
			
			client.Send(reply);
        }
    }
}
