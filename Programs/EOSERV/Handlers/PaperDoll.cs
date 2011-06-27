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
			ushort id = (ushort)packet.GetShort();

			Character target = client.Character.Map.GetObjectByID<Character>(id);

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
			reply.AddChar((byte)target.Gender);
			reply.AddChar(0); // Wut?

			reply.AddShort(target.Boots    != null ? target.Boots.Id    : (short)0);
			reply.AddShort(target.Charm    != null ? target.Charm.Id    : (short)0);
			reply.AddShort(target.Gloves   != null ? target.Gloves.Id   : (short)0);
			reply.AddShort(target.Belt     != null ? target.Belt.Id     : (short)0);
			reply.AddShort(target.Armor    != null ? target.Armor.Id    : (short)0);
			reply.AddShort(target.Necklace != null ? target.Necklace.Id : (short)0);
			reply.AddShort(target.Hat      != null ? target.Hat.Id      : (short)0);
			reply.AddShort(target.Shield   != null ? target.Shield.Id   : (short)0);
			reply.AddShort(target.Weapon   != null ? target.Weapon.Id   : (short)0);
			reply.AddShort(target.Ring1    != null ? target.Ring1.Id    : (short)0);
			reply.AddShort(target.Ring2    != null ? target.Ring2.Id    : (short)0);
			reply.AddShort(target.Armlet1  != null ? target.Armlet1.Id  : (short)0);
			reply.AddShort(target.Armlet2  != null ? target.Armlet2.Id  : (short)0);
			reply.AddShort(target.Bracer1  != null ? target.Bracer1.Id  : (short)0);
			reply.AddShort(target.Bracer2  != null ? target.Bracer2.Id  : (short)0);

			// TODO: Right icon
			reply.AddChar((byte)PaperdollIcon.Normal);
			client.Send(reply);
		}

		// Player unequipping an item
		[HandlerState(ClientState.Playing)]
		public static void HandleRemove(Packet packet, IClient client, bool fromQueue)
		{
			short item = packet.GetShort();
			byte subloc = packet.GetChar();

			if (client.Server.ItemData[(ushort)item].special == EO.Data.EIF.Special.Cursed)
			{
				return;
			}

			client.Character.Unequip(item, subloc);
		}

		// Player equipping an item
		[HandlerState(ClientState.Playing)]
		public static void HandleAdd(Packet packet, IClient client, bool fromQueue)
		{
			short item = packet.GetShort();
			byte subloc = packet.GetChar();

			client.Character.Equip(item, subloc);
		}
	}
}
