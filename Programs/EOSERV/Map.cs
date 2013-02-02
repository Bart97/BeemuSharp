using System;
using System.Collections;
using System.Collections.Generic;
using EOHax.EO;
using EOHax.EO.Communication;
using EOHax.EOSERV.Data;

namespace EOHax.Programs.EOSERV
{
	public class Map : IMap, IMessageTarget
	{
		private HashSet<IMapObject> objects = new HashSet<IMapObject>();

		public ICollection<IMapObject> Objects
		{
			get { return objects; }
		}

		public MapData Data { get; private set; }

		public Map(MapData data)
		{
			this.Data = data;
		}

        public void SpawnNpcs(IServer server)
        {
            foreach (MapData.Tile tile in Data.Tiles)
            {
                if (tile.npc != null)
                {
                    Enter(new NPC(server, (short)tile.npc.Value.id, Data.Id, tile.npc.Value.x, tile.npc.Value.y), WarpAnimation.None);
                }
            }
        }
        
        /// <summary>
		/// Checks if the provided coordinates are in bounds of the map
		/// </summary>
		/// <param name="x">X coordinate</param>
		/// <param name="y">Y coordinate</param>
		public bool InBounds(byte x, byte y)
		{
			return (x < Data.Width && y < Data.Height);
		}

		/// <summary>
		/// Returns a MapObject at a point on the map
		/// </summary>
		/// <param name="x">X coordinate</param>
		/// <param name="y">Y coordinate</param>
		public T ObjectOnTile<T>(byte x, byte y)
			where T : IMapObject
		{
			foreach (IMapObject obj in objects)
			{
				if (typeof(T).IsInstanceOfType(obj) && obj.X == x && obj.Y == y)
					return (T)obj;
			}

			return default(T);
		}

		/// <summary>
		/// Returns the MapObjects at a point on the map
		/// </summary>
		/// <param name="x">X coordinate</param>
		/// <param name="y">Y coordinate</param>
		public IEnumerable<T> ObjectsOnTile<T>(byte x, byte y)
			where T : IMapObject
		{
			var localObjects = new List<T>();

			foreach (IMapObject obj in objects)
			{
				if (typeof(T).IsInstanceOfType(obj) && obj.X == x && obj.Y == y)
					localObjects.Add((T)obj);
			}

			return localObjects;
		}

		/// <summary>
		/// Returns the MapObjects in range of a point on the map
		/// </summary>
		/// <param name="x">X coordinate to begin searching</param>
		/// <param name="y">Y coordinate to begin searching</param>
		/// <param name="distance">Range in tiles to be scanned</param>
		public IEnumerable<T> ObjectsInRange<T>(byte x, byte y, int distance = -1)
			where T : IMapObject
		{
			var localObjects = new List<T>();

			// TODO: Get default distance from config
			if (distance == -1)
				distance = 11;

			foreach (IMapObject obj in objects)
			{
				if (typeof(T).IsInstanceOfType(obj) && Math.Abs(obj.X - x) < distance && Math.Abs(obj.Y - y) < distance)
					localObjects.Add((T)obj);
			}
			
			return localObjects;
		}

		public ushort GenerateObjectID<T>() where T : IMapObject
		{
			ushort lastId = 0;
			foreach (IMapObject obj in objects)
			{
				if (typeof(T).IsInstanceOfType(obj))
					if (obj.Id >= lastId)
						lastId = (ushort)((short)obj.Id + 1);
			}
			return lastId;
		}

		public T GetObjectByID<T>(ushort id) where T : IMapObject
		{
			foreach (IMapObject obj in objects)
			{
				if (typeof(T).IsInstanceOfType(obj) && obj.Id == id)
					return (T)obj;
			}

			return default(T);
		}

		/// <summary>
		/// Sends a packet to all Character MapObjects in view range of a point on the map
		/// </summary>
		/// <param name="packet">Packet to be sent</param>
		/// <param name="x">X coordinate to begin searching</param>
		/// <param name="y">Y coordinate to begin searching</param>
		public void SendInRange(Packet packet, byte x, byte y, MapObject exclude = null)
		{
			foreach (IMapObject obj in ObjectsInRange<IMapObject>(x, y))
			{
				if (obj == exclude) continue;
				obj.Client.Send(packet);
			}
		}

		public void Enter(MapObject obj, WarpAnimation animation, MapObject exclude = null)
		{
			if (!objects.Add(obj))
				return;

			obj.SendInRange(obj.AddToViewBuilder(animation), false, exclude);
		}

		public void Leave(MapObject obj, WarpAnimation animation, MapObject exclude = null)
		{
			if (!objects.Remove(obj))
				return;

			obj.SendInRange(obj.DeleteFromViewBuilder(animation), false, exclude);
		}

		public void ObjectBuilder(ref Packet packet)
		{

		}

		public void RecieveMsg(IMessageSource source, Message message)
		{
			if (typeof(IMapObject).IsInstanceOfType(source))
			{
				Packet packet = new Packet(PacketFamily.Talk, PacketAction.Player);
				packet.AddShort((short)((MapObject)source).Id);
				packet.AddString(message.MessageString);

				SendInRange(packet, ((MapObject)source).X, ((MapObject)source).Y, (MapObject)source);
			}
		}
	}
}
