using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using EOHax.EO;
using EOHax.EO.Communication;

namespace EOHax.Programs.EOSERV
{
	public class Client : IClient, IDisposable
	{
		private TcpClient client;
		private NetworkStream stream;
		private Thread thread;

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
			get { return client.Connected; }
		}

		public IPAddress Address
		{
			get { return ((IPEndPoint)client.Client.RemoteEndPoint).Address; }
		}

		public Client(IServer server, TcpClient client)
		{
			Server = server;
			Processor = new ServerPacketProcessor();
			this.client = client;
			stream = client.GetStream();
			thread = new Thread(Main);
		}

		public void Start()
		{
			if (!thread.IsAlive)
				thread.Start();
		}

		public void Stop()
		{
			thread.Abort();
		}

		public void Join()
		{
			thread.Join();
		}

		public void Send(Packet packet)
		{
			byte[] data = packet.Get();
			Processor.Encode(ref data);
			stream.Write(Packet.EncodeNumber(data.Length, 2), 0, 2);
			stream.Write(data, 0, data.Length);
		}

		private void Main()
		{
			try
			{
				while (Connected)
				{
					try
					{
						HandlePacket(ReadPacket(), false);
					}
					catch (NotSupportedException ex)
					{
						Program.Logger.LogWarning(ex.Message);
					}
				}
			}
			catch (IOException) { }
			catch (Exception ex)
			{
				HelloHax0r();
				Program.Logger.LogError("Client triggered an exception and was killed", ex);
			}
			finally
			{
				Server.RemoveClient(this);

				this.Dispose();
			}
		}

		public void HandlePacket(Packet packet, bool fromQueue)
		{
			// TODO: Check sequence value
			if (packet.Family != PacketFamily.Init)
				packet.GetChar();

			PacketHandler.Handle(packet, this, fromQueue);
		}

		private Packet ReadPacket()
		{
			byte[] data = new byte[2];
			int length;
			int readTotal = 0;

			if (stream.Read(data, 0, 2) == 0)
				throw new IOException("Read returned 0 bytes");

			length = Packet.DecodeNumber(data);

			if (length < 2)
				throw new Exception("Packet with length < 2 recieved");

			data = new byte[length];

			while (readTotal != length)
			{
				int read = stream.Read(data, 0, length);

				if (read == 0)
					throw new IOException("Read returned 0 bytes");

				readTotal += read;
			}
			
			Processor.Decode(ref data);

			return new Packet(data);
		}

		public void SetId(ushort id)
		{
			if (Id != 0)
				throw new InvalidOperationException("Id has already been set");

			Id = id;
		}

		public void Init(int version)
		{
			if (State >= ClientState.Initialized)
				throw new InvalidOperationException("Client is already initialized");

			State = ClientState.Initialized;
		}

		public void Login(Account account)
		{
			if (State >= ClientState.LoggedIn)
				throw new InvalidOperationException("Client is already logged in");

			Account = account;
			State = ClientState.LoggedIn;
		}

		public void SelectCharacter(Character character)
		{
			if (State != ClientState.LoggedIn)
				throw new InvalidOperationException("Client has already entered the game");

			Character = character;
		}

		public void EnterGame()
		{
			if (State >= ClientState.Playing)
				throw new InvalidOperationException("Client has already entered the game");

			State = ClientState.Playing;
			Server.Characters.Add(Character);
		}

		public void HelloHax0r()
		{
			Packet packet = new Packet(PacketFamily.StatSkill, PacketAction.Player);
			for (int i = 0; i < 32; ++i)
			{
				packet.AddChar(0xFF);
			}
			Send(packet);
		}

		public void Dispose()
		{
			if (Character != null && Character.Map != null)
				Character.Map.Leave(Character, WarpAnimation.None);

			if (Character != null)
				Character.Store();

			if (client.Connected)
				stream.Dispose();
		}
	}
}
