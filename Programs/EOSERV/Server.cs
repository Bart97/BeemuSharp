using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using EOHax.EO.Communication;
using EOHax.EO.Data;
using EOHax.EOSERV.Data;
using EOHax.Scripting;
using Microsoft.WindowsAPICodePack.Taskbar;

namespace EOHax.Programs.EOSERV
{
	public class Server : IServer
	{
		private Random rng = new Random();
		private TcpListener socket;
		private Thread thread;
		private SLNClient slnClient;
		private ConsoleInput consoleInput;
		private Dictionary<ushort, IClient> clients;
		private Dictionary<ushort, IMap> maps;
		private List<Character> characters;

        private Timer pingTimer;
        private Timer npcRespawnTimer;

		public EIF ItemData { get; private set; }
		public ENF NpcData { get; private set; }
		public ECF ClassData { get; private set; }
		public ESF SpellData { get; private set; }
		public MapDataSet MapData { get; private set; }
		public Database Database { get; private set; }

		public ScriptHost ScriptHost { get; private set; }

		public GlobalChat Global { get; private set; }

        /*public delegate void WalkEventHandler(MapWalkEventArgs we);
        public event WalkEventHandler WalkEvent;*/

		public IDictionary<ushort, IClient> Clients
		{
			get { return clients; }
		}

		public IDictionary<ushort, IMap> Maps
		{
			get { return maps; }
		}

		public List<Character> Characters
		{
			get { return characters; }
		}

		private ushort GenerateClientID()
		{
			ushort i = (ushort)rng.Next(1, 60000);
			ushort start = i;

			while (clients.ContainsKey(i))
			{
				++i;

				if (i > 60000)
					i = 1;

				if (i == start)
					throw new ApplicationException("No more client IDs are available");
			}

			return i;
		}

		public Server(IPAddress addr, ushort port)
		{
			if (Program.Taskbar != null)
				Program.Taskbar.SetProgressState(TaskbarProgressBarState.Indeterminate);
			ScriptHost = ScriptHost.Create();

			socket = new TcpListener(addr, port);
			Program.Logger.LogInfo(String.Format("Listening on {0}:{1}", addr, port));
			thread = new Thread(AcceptClients);

			pingTimer = new Timer(new TimerCallback(this.PingClients), new AutoResetEvent(false), 60000, 60000);
            npcRespawnTimer = new Timer(new TimerCallback(this.RespawnNPCs), new AutoResetEvent(false), 2000, 1000);

			Database = new Database("eoserv.db4o");
			Program.Logger.LogSuccess("Database loaded.");
			Global = new GlobalChat();
			clients = new Dictionary<ushort, IClient>();
			maps = new Dictionary<ushort, IMap>();
			characters = new List<Character>();

			ItemData = new EIF("./data/dat001.eif");
			Program.Logger.LogSuccess(String.Format("Loaded {0} items.", ItemData.Count));
			NpcData = new ENF("./data/dtn001.enf");
			Program.Logger.LogSuccess(String.Format("Loaded {0} NPCs.", NpcData.Count));
			ClassData = new ECF("./data/dat001.ecf");
			Program.Logger.LogSuccess(String.Format("Loaded {0} classes.", ClassData.Count));
			SpellData = new ESF("./data/dsl001.esf");
			Program.Logger.LogSuccess(String.Format("Loaded {0} spells.", SpellData.Count));
			MapData = new MapDataSet("./data/maps/");
			Program.Logger.LogSuccess(String.Format("Loaded {0} maps.", MapData.Count));

			/*ItemData.GetPubFile("./tmp/");
			NpcData.GetPubFile("./tmp/");
			ClassData.GetPubFile("./tmp/");
			SpellData.GetPubFile("./tmp/");*/

			foreach (KeyValuePair<ushort, MapData> entry in MapData)
			{
				entry.Value.GetPubFile("./tmp/");
				maps.Add(entry.Key, new Map(entry.Value));
                maps[entry.Key].SpawnNpcs(this);
			}

			if (Program.Taskbar != null)
				Program.Taskbar.SetProgressState(TaskbarProgressBarState.NoProgress);
			
			slnClient = new SLNClient();
			consoleInput = new ConsoleInput(this);

			Program.Logger.LogSuccess("Server started");
		}

		public void Start()
		{
			socket.Start();

			if (!thread.IsAlive)
				thread.Start();
		}

		public void Stop()
		{
			socket.Stop();
			thread.Abort();
		}

		public void Join()
		{
			thread.Join();
		}

		public void AddClient(IClient client)
		{
			lock (clients)
			{
				client.SetId(GenerateClientID());
				clients.Add(client.Id, client);

				Program.Logger.LogInfo(String.Format("New client: {0} ({1})", client.Address, client.Id));
			}
		}
		
		public void RemoveClient(IClient client)
		{
			lock (clients)
			{
				string logString = String.Format("Client closed: {0} ({1})", client.Address, client.Id);

				if (client.Account != null)
				{
					logString += " " + client.Account.Username;

					if (client.Character != null)
					{
						Characters.Remove(client.Character);
						logString += "/" + client.Character.Name;
					}
				}

				Program.Logger.LogInfo(logString);

				clients.Remove(client.Id);
			}
		}

		private void AcceptClients()
		{
			while (true)
			{
				Client new_client = new Client(this, socket.AcceptTcpClient());
				AddClient(new_client);
				new_client.Start();
				new_client = null;
			}
		}

		private void PingClients(Object stateInfo)
		{
			Program.Logger.LogDebug("Pinging clients.");

			Packet packet = new Packet(PacketFamily.Connection, PacketAction.Player);
			packet.AddShort(0);
			packet.AddChar(0);

			foreach (KeyValuePair<ushort, IClient> client in clients)
			{
				if (client.Value.Pinged)
				{
					Program.Logger.LogError("Client ping timed out.");

					RemoveClient(client.Value);
				}
				else
				{
					client.Value.Send(packet);

					client.Value.Pinged = true;
				}
			}
		}

        private void RespawnNPCs(Object stateInfo)
        {

        }

		public void Shutdown(int time)
		{
			/*throw new NotImplementedException();*/
			Program.Logger.LogInfo("Server shutting down");
			foreach (Character character in characters)
			{
				character.Store();
			}
			Database.Commit();
			Stop();
		}

		public void Send(Packet packet, ClientState state)
		{
			foreach (KeyValuePair<ushort, IClient> client in clients)
			{
				if (client.Value.State == state)
				{
					client.Value.Send(packet);
				}
			}
		}
	}
}
