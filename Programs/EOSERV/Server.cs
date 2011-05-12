using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using EOHax.EO.Communication;
using EOHax.EOSERV.Data;
using EOHax.Scripting;

namespace EOHax.Programs.EOSERV
{
	public class Server : IServer
	{
		private Random rng = new Random();
		private TcpListener socket;
		private Thread thread;
		private Dictionary<ushort, IClient> clients;
		private Dictionary<string, IMap> maps;

		public ItemDataSet ItemData { get; private set; }
		public NpcDataSet NpcData { get; private set; }
		public ClassDataSet ClassData { get; private set; }
		public SpellDataSet SpellData { get; private set; }
		public MapDataSet MapData { get; private set; }
		public Database Database { get; private set; }

		public ScriptHost ScriptHost { get; private set; }

		public IDictionary<ushort, IClient> Clients
		{
			get { return clients; }
		}

		public IDictionary<string, IMap> Maps
		{
			get { return maps; }
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
			ScriptHost = ScriptHost.Create();

			socket = new TcpListener(addr, port);
			thread = new Thread(AcceptClients);
			Database = new Database("eoserv.db4o");
			clients = new Dictionary<ushort, IClient>();
			maps = new Dictionary<string, IMap>();

			ItemData = new ItemDataSet("./data/items/");
			NpcData = new NpcDataSet("./data/npcs/");
			ClassData = new ClassDataSet("./data/classes/");
			SpellData = new SpellDataSet("./data/spells/");
			MapData = new MapDataSet("./data/maps/");

			ItemData.GetPubFile("./tmp/");
			NpcData.GetPubFile("./tmp/");
			ClassData.GetPubFile("./tmp/");
			SpellData.GetPubFile("./tmp/");

			foreach (KeyValuePair<string, MapData> entry in MapData)
			{
				entry.Value.GetPubFile("./tmp/");
				maps.Add(entry.Key, new Map(entry.Value));
			}
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
						logString += "/" + client.Character.Name;
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

		public void Shutdown(int time)
		{
			throw new NotImplementedException();
		}
	}
}
