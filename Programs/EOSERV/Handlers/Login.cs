using System;
using Db4objects.Db4o.Linq;
using EOHax.EO;
using EOHax.EO.Communication;

namespace EOHax.Programs.EOSERV.Handlers
{
	static class LoginHandler
	{
		// Logging in to an account
		[HandlerState(ClientState.Initialized)]
		public static void HandleRequest(Packet packet, IClient client, bool fromQueue)
		{
			string username = packet.GetBreakString();
			string password = packet.GetBreakString();
			
			var accounts = from Account a in client.Server.Database.Container
			               where a.username == username
			               select a;

			Packet reply = new Packet(PacketFamily.Login, PacketAction.Reply);

			if (accounts.Count() == 0)
			{
				reply.AddShort((short)LoginReply.WrongUser);
				client.Send(reply);
				return;
			}

			Account account = null;

			foreach (Account temp in accounts)
			{
				account = temp;
				break;
			}

			account.Activate(client.Server, client);

			if (!account.CheckPassword(password))
			{
				reply.AddShort((short)LoginReply.WrongUserPass);
				client.Send(reply);
				return;
			}

			reply.AddShort((short)LoginReply.OK);
			reply.AddChar((byte)account.Characters.Count);
			reply.AddByte(2);
			reply.AddBreak();

			// TODO: Some kind of character list packet builder
			int i = 0;
			foreach (Character character in account.Characters)
			{
				reply.AddBreakString(character.Name);
				reply.AddInt(i++); // ID
				reply.AddChar(character.Level);
				reply.AddChar((byte)character.Gender);
				reply.AddChar((byte)character.HairStyle);
				reply.AddChar((byte)character.HairColor);
				reply.AddChar((byte)character.Skin);
				reply.AddChar(4); // Admin Level
                reply.AddShort((short)(character.Boots  != null ? character.Boots.Data.special1  : 0));
                reply.AddShort((short)(character.Armor  != null ? character.Armor.Data.special1  : 0));
                reply.AddShort((short)(character.Hat    != null ? character.Hat.Data.special1    : 0));
                reply.AddShort((short)(character.Shield != null ? character.Shield.Data.special1 : 0));
                reply.AddShort((short)(character.Weapon != null ? character.Weapon.Data.special1 : 0));
				reply.AddBreak();
			}
			
			client.Send(reply);

			client.Login(account);
		}
	}
}
