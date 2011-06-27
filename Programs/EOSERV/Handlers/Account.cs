using System;
using Db4objects.Db4o.Linq;
using EOHax.EO;
using EOHax.EO.Communication;

namespace EOHax.Programs.EOSERV.Handlers
{
	static class AccountHandler
	{
		static private Random rng = new Random();

		// Check if a character exists
		[HandlerState(ClientState.Initialized)]
		public static void HandleRequest(Packet packet, IClient client, bool fromQueue)
		{
			string username = packet.GetBreakString().ToLower();

			Packet reply = new Packet(PacketFamily.Account, PacketAction.Reply);

			if (!Account.ValidUsername(username))
			{
				reply.AddShort((short)AccountReply.NotApproved);
				reply.AddString("NO");
				client.Send(reply);
				return;
			}

			// TODO: AccountExists functions
			var account = from Account a in client.Server.Database.Container
						  where a.username == username
						  select 1;

			if (account.Count() != 0)
			{
				reply.AddShort((short)AccountReply.Exists);
				reply.AddString("NO");
				client.Send(reply);
				return;
			}

			reply.AddShort((short)rng.Next()); // Account creation "session ID"
			reply.AddString("AOK");
			client.Send(reply);
		}

		// Account creation
		[HandlerState(ClientState.Initialized)]
		public static void HandleCreate(Packet packet, IClient client, bool fromQueue)
		{
			packet.GetShort(); // Account creation "session ID"
			packet.GetByte(); // ?

			string username = packet.GetBreakString().ToLower();
			string password = packet.GetBreakString();
			string fullname = packet.GetBreakString();
			string location = packet.GetBreakString();
			string email = packet.GetBreakString();
			string computer = packet.GetBreakString();
			uint hdid = UInt32.Parse(packet.GetBreakString());

			Packet reply = new Packet(PacketFamily.Account, PacketAction.Reply);

			if (!Account.ValidUsername(username))
			{
				reply.AddShort((short)AccountReply.NotApproved);
				reply.AddString("NO");
				client.Send(reply);
				return;
			}
			
			// TODO: AccountExists functions
			var checkAccount = from Account a in client.Server.Database.Container
							   where a.username == username
							   select 1;

			if (checkAccount.Count() != 0)
			{
				reply.AddShort((short)AccountReply.Exists);
				reply.AddString("NO");
				client.Send(reply);
				return;
			}

			Account newAccount = new Account(client.Server, client, client.Address, username, password, location, email, computer, hdid);
			newAccount.Store();
			client.Server.Database.Commit();

			reply.AddShort((short)AccountReply.Created);
			reply.AddString("OK");
			client.Send(reply);
		}
	}
}
