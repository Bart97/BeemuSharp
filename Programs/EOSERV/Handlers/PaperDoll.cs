using System;
using EOHax.EO;
using EOHax.EO.Communication;

namespace EOHax.Programs.EOSERV.Handlers
{
    class PaperDollHandler
    {
        // Player opening a paperdoll
        [HandlerState(ClientState.Playing)]
        public static void HandleRequest(Packet packet, IClient client, bool fromQueue)
        {
            short id = packet.GetShort();

            Character target = client.Character.Map.GetObjectByID<Character>((ushort)id);

            if (target == null)
            {
                target = client.Character;
            }

            Packet reply = new Packet(PacketFamily.PaperDoll, PacketAction.Reply);
            reply.AddBreakString(target.Name);
            reply.AddBreakString("Home");
            reply.AddBreakString("Partner");
            reply.AddBreakString(target.Title ?? "");
            reply.AddBreakString("Guild name");
            reply.AddBreakString("Guild rank");
            reply.AddShort((short)target.Id);
            reply.AddChar((byte)target.ClassId);
            reply.AddChar(0); // Wut?

            reply.AddShort(target.boots     != null ? target.boots.Id     : (short)0);
            reply.AddShort(target.accessory != null ? target.accessory.Id : (short)0);
            reply.AddShort(target.gloves    != null ? target.gloves.Id    : (short)0);
            reply.AddShort(target.belt      != null ? target.belt.Id      : (short)0);
            reply.AddShort(target.armor     != null ? target.armor.Id     : (short)0);
            reply.AddShort(target.necklace  != null ? target.necklace.Id  : (short)0);
            reply.AddShort(target.hat       != null ? target.hat.Id       : (short)0);
            reply.AddShort(target.shield    != null ? target.shield.Id    : (short)0);
            reply.AddShort(target.weapon    != null ? target.weapon.Id    : (short)0);
            reply.AddShort(target.ring1     != null ? target.ring1.Id     : (short)0);
            reply.AddShort(target.ring2     != null ? target.ring2.Id     : (short)0);
            reply.AddShort(target.armlet1   != null ? target.armlet1.Id   : (short)0);
            reply.AddShort(target.armlet2   != null ? target.armlet2.Id   : (short)0);
            reply.AddShort(target.bracer1   != null ? target.bracer1.Id   : (short)0);
            reply.AddShort(target.bracer2   != null ? target.bracer2.Id   : (short)0);

            // TODO: Right icon
            reply.AddChar((byte)PaperdollIcon.Normal);
            client.Send(reply);
        }
    }
}
