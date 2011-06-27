using System;
using System.Collections.Generic;
using EOHax.EO.Data;
using EOHax.EOSERV.Data;
using EOHax.Scripting;

namespace EOHax.Programs.EOSERV
{
	public interface IServer
	{
		EIF        ItemData  { get; }
		ENF        NpcData   { get; }
		ECF        ClassData { get; }
		ESF        SpellData { get; }
		MapDataSet MapData   { get; }
		Database   Database  { get; }

		ScriptHost ScriptHost { get; }

		IDictionary<ushort, IClient> Clients    { get; }
		IDictionary<ushort, IMap>    Maps       { get; }
		List<Character>              Characters { get; }

		void Start();
		void Stop ();
		void Join ();

		void AddClient   (IClient client);
		void RemoveClient(IClient client);

		void Shutdown(int time);
	}
}
