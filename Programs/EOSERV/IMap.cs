using System;
using System.Collections.Generic;
using EOHax.EO;
using EOHax.EO.Communication;
using EOHax.EOSERV.Data;

namespace EOHax.Programs.EOSERV
{
	public class MapWalkEventArgs : EventArgs
	{
		public IMapObject obj;
	}

	public interface IMap
	{
		ICollection<IMapObject> Objects { get; }
		MapData                 Data    { get; }

		bool InBounds(byte x, byte y);

		T              ObjectOnTile<T>  (byte x, byte y)                    where T : IMapObject;
		IEnumerable<T> ObjectsOnTile<T> (byte x, byte y)                    where T : IMapObject;
		IEnumerable<T> ObjectsInRange<T>(byte x, byte y, int distance = -1) where T : IMapObject;

        ushort GenerateObjectID<T>() where T : IMapObject;
        T GetObjectByID<T>(ushort id) where T : IMapObject;

        void SendInRange(Packet packet, byte x, byte y, MapObject exclude = null);

        void Enter(MapObject obj, WarpAnimation animation, MapObject exclude = null);
        void Leave(MapObject obj, WarpAnimation animation, MapObject exclude = null);

		void ObjectBuilder(ref Packet packet);
	}
}
