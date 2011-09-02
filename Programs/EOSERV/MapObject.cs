using System;
using System.Collections.Generic;
using EOHax.EO;
using EOHax.EO.Communication;
using EOHax.EOSERV.Data;

namespace EOHax.Programs.EOSERV
{
	public abstract class MapObject : DatabaseObject, IMapObject
	{
		// These have to be public to allow queries to work
		public ushort    mapId;
		public byte      x;
		public byte      y;
		public Direction direction = Direction.Down;
		public short     hp;
		public short     tp;

		[NonSerialized] private IClient client;
		[NonSerialized] private short   maxHp;
		[NonSerialized] private short   maxTp;
		[NonSerialized] private short   minDamage;
		[NonSerialized] private short   maxDamage;
		[NonSerialized] private short   accuracy;
		[NonSerialized] private short   evade;
		[NonSerialized] private short   defence;
#region Null Accessors
		public ushort MapId
		{
			get { return mapId; }
			protected set { mapId = value; }
		}

		public byte X
		{
			get { return x; }
			protected set { x = value; }
		}

		public byte Y
		{
			get { return y; }
			protected set { y = value; }
		}

		public Direction Direction
		{
			get { return direction; }
			protected set { direction = value; }
		}

		public short Hp
		{
			get { return hp; }
			protected set { hp = value; }
		}

		public short Tp
		{
			get { return tp; }
			protected set { tp = value; }
		}

		public IClient Client
		{
			get { return client; }
			protected set { client = value; }
		}

		public short MaxHp
		{
			get { return maxHp; }
			protected set { maxHp = value; }
		}

		public short MaxTp
		{
			get { return maxTp; }
			protected set { maxTp = value; }
		}

		public short MinDamage
		{
			get { return minDamage; }
			protected set { minDamage = value; }
		}

		public short MaxDamage
		{
			get { return maxDamage; }
			protected set { maxDamage = value; }
		}

		public short Accuracy
		{
			get { return accuracy; }
			protected set { accuracy = value; }
		}

		public short Evade
		{
			get { return evade; }
			protected set { evade = value; }
		}

		public short Defence
		{
			get { return defence; }
			protected set { defence = value; }
		}
#endregion

		public IMap Map
		{
			get { return Server.Maps[mapId]; }
		}

		public ushort Id
		{
			get { return Client.Id; }
			protected set { Client.SetId(value); }
		}

		private void Init()
		{
			if (Client == null)
				Client = new NullClient(Server);

			this.CalculateStats();
		}

		public MapObject(IServer server, IClient client) : base(server)
		{
			Init();
		}
#region Database
		public new void Activate(IServer server)
		{
			base.Activate(server);

			Init();
		}

		public new void Store()
		{
			base.Store();
		}
#endregion
		public IEnumerable<T> GetInRange<T>(int distance = -1)
			where T : IMapObject
		{
			return Map.ObjectsInRange<T>(X, Y, distance);
		}

		public void SendInRange(Packet packet, bool echo = true, MapObject exclude = null)
		{
			foreach (IMapObject obj in Map.ObjectsInRange<IMapObject>(X, Y))
			{
				if ((echo || obj != this) && obj != exclude)
					obj.Client.Send(packet);
			}
		}

		public abstract void CalculateStats();

		public virtual bool Walkable(byte x, byte y)
		{
			switch (Map.Data.Tiles[y, x].spec)
			{
				case MapTileSpec.Wall:
				case MapTileSpec.ChairDown:
				case MapTileSpec.ChairLeft:
				case MapTileSpec.ChairRight:
				case MapTileSpec.ChairUp:
				case MapTileSpec.ChairDownRight:
				case MapTileSpec.ChairUpLeft:
				case MapTileSpec.ChairAll:
				case MapTileSpec.Chest:
				case MapTileSpec.BankVault:
				case MapTileSpec.MapEdge:
				case MapTileSpec.Board:
				case MapTileSpec.Jukebox:
					return false;
			}

			return true;
		}

		public abstract void Effect(int effect, bool echo = true);

		public virtual void Face(Direction direction, bool echo = true)
		{
			Direction = direction;
		}

		public virtual bool Walk(Direction direction, WalkType type = WalkType.Normal, bool echo = true)
		{
			byte offsetX = 0;
			byte offsetY = 0;

			if (Enum.IsDefined(typeof(Direction), direction))
				client.Character.Direction = direction;

			switch (direction)
			{
				case Direction.Up:    
					offsetY -= 1; break;
				case Direction.Right: offsetX += 1; break;
				case Direction.Down:  offsetY += 1; break;
				case Direction.Left:  offsetX -= 1; break;
			}

			byte targetX = (byte)(X + offsetX);
			byte targetY = (byte)(Y + offsetY);
			
			if (Walkable(targetX, targetY))
			{
				if (Map.Data.Tiles[targetY, targetX].warp != null)
				{
					Warp(Map.Data.Tiles[targetY, targetX].warp.Value.mapId,
						 Map.Data.Tiles[targetY, targetX].warp.Value.x, Map.Data.Tiles[targetY, targetX].warp.Value.y);
					return false;
				}
				X = targetX;
				Y = targetY;
				Direction = direction;

				return true;
			}

			return false;
		}

		public virtual void Warp(ushort map, byte x, byte y, WarpAnimation animation = WarpAnimation.None)
		{
			IMap target;
			// TODO: A better way to check if a map exists
			try
			{
				target = Server.Maps[map];
			}
			catch (Exception)
			{
				return;
			}

			Map.Leave(this, animation);
			
			MapId = map;
			X = x;
			Y = y;
			Map.Enter(this, animation);
		}

		public abstract Packet AddToViewBuilder(WarpAnimation animation = WarpAnimation.None);
		public abstract Packet DeleteFromViewBuilder(WarpAnimation animation = WarpAnimation.None);
		public abstract Packet WalkBuilder();
		public abstract Packet FaceBuilder();
		public abstract void InfoBuilder(ref Packet packet);
	}
}
