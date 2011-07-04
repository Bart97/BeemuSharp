using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using EOHax.EO.Communication;

namespace EOHax.Programs.EOSERV
{
	public class NullClient : IClient
	{
		public IServer Server { get; private set; }
		public IPacketProcessor Processor { get; private set; }

		public ushort Id { get; private set; }
		public Account Account { get; private set; }
		public Character Character { get; private set; }
		public int Version { get; private set; }
		public ClientState State { get; private set; }
		public bool Pinged { get; set; }

		public bool Connected
		{
			get { return false; }
		}

		public IPAddress Address
		{
			get { return IPAddress.Loopback; }
		}

		public NullClient(IServer server)
		{
			Server = server;
			Processor = new ServerPacketProcessor();
		}

		public void Start() { }
		public void Stop() { }
		public void Join() { }
		public void Send(Packet packet) { }
		public void HandlePacket(Packet packet, bool fromQueue) { }

		public void SetId(ushort id)
		{
			if (Id != 0)
				throw new InvalidOperationException("Id has already been set");

			Id = id;
		}
		public void Init(int version) { }
		public void Login(Account account) { }
		public void SelectCharacter(Character character) { }
		public void EnterGame() { }

        public void HelloHax0r() { }
	}
}
