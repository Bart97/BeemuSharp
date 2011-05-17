using System;
using EOHax.EO;
using EOHax.EO.Communication;
using EOHax.Scripting;

namespace EOHax.Programs.EOSERV.Handlers
{
    static class ItemHandler
    {
        // Player changing direction
        [HandlerState(ClientState.Playing)]
        public static void HandleDrop(Packet packet, IClient client, bool fromQueue)
        {
            short id = packet.GetShort();
            int amount = (packet.Length == 10 ? packet.GetThree() : packet.GetInt());
            byte x = packet.GetChar();
            byte y = packet.GetChar();
            if (x == 254 && y == 254)
            {
                x = client.Character.X;
                y = client.Character.Y;
            }
            client.Character.DropItem(id, amount, x, y);
        }

        // Player picking up an item
        [HandlerState(ClientState.Playing)]
        public static void HandleGet(Packet packet, IClient client, bool fromQueue)
        {
            client.Character.PickItem((ushort)packet.GetShort());
        }
    }
}
