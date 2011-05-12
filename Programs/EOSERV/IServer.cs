using System;
using System.Collections.Generic;
using EOHax.EOSERV.Data;
using EOHax.Scripting;

namespace EOHax.Programs.EOSERV
{
	public interface IServer
	{
		ItemDataSet  ItemData  { get; }
		NpcDataSet   NpcData   { get; }
		ClassDataSet ClassData { get; }
		SpellDataSet SpellData { get; }
		MapDataSet   MapData   { get; }
		Database     Database  { get; }

		ScriptHost ScriptHost { get; }

		IDictionary<ushort, IClient> Clients { get; }
		IDictionary<string, IMap>    Maps    { get; }

		void Start();
		void Stop ();
		void Join ();

		void AddClient   (IClient client);
		void RemoveClient(IClient client);

		void Shutdown(int time);
	}
}
