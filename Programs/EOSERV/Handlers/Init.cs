using System;
using EOHax.EO;
using EOHax.EO.Communication;

namespace EOHax.Programs.EOSERV.Handlers
{
	static class InitHandler
	{
		private static System.Random rng = new System.Random();

		/// <summary>
		/// "secret" hash function the client uses to verify the server
		/// </summary>
		private static uint StupidHash(uint i)
		{
			++i;
			return 110905 + (i % 9 + 1) * ((11092004 - i) % ((i % 11 + 1) * 119)) * 119 + i % 2004;
		}

		// Initialization
		[HandlerState(ClientState.Uninitialized)]
		public static void HandleInit(Packet packet, IClient client, bool fromQueue)
		{
			Packet reply = new Packet(PacketFamily.Init, PacketAction.Init);

			uint challenge = (uint)packet.GetThree();
			uint response = StupidHash(challenge);
			
			packet.Skip(2);
			int version = packet.GetByte();
			
			client.Processor.SetMulti((byte)rng.Next(6, 12), (byte)rng.Next(6, 12));

			reply.AddByte((byte)InitReply.OK);
			reply.AddByte(10);
			reply.AddByte(10);

			reply.AddByte((byte)client.Processor.SendMulti);
			reply.AddByte((byte)client.Processor.RecvMulti);

			reply.AddShort((short)client.Id);

			reply.AddThree((int)response);

			client.Send(reply);
			client.Init(version);
		}
	}
}
