using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using EOHax.EO;
using EOHax.EO.Communication;
using EOHax.EO.Data;

namespace EOHax.Programs.EOSERV
{
	class MapItem : MapObject
	{
		public short id;
		public int amount;
		public Character owner;
		public DateTime unprotectTime = DateTime.Now.AddSeconds(5); // TODO: Config?

#region Null Accessors
		public short ItemId
		{
			get { return id; }
			private set { id = value; }
		}
		public int Amount
		{
			get { return amount; }
			private set { amount = value; }
		}
		public Character Owner
		{
			get { return owner; }
			private set { owner = value; }
		}
		public DateTime UnprotectTime
		{
			get { return unprotectTime; }
			private set { unprotectTime = value; }
		}
#endregion

		public int OwnerID
		{
			get { return (Owner != null ? Owner.Client.Id : 0); }
		}

		public MapItem(IServer server, short id, int amount, Character owner, ushort mapId, byte x, byte y)
			: base(server, null)
		{
			ItemId = id;
			Amount = amount;
			Owner = owner;
			MapId = mapId;
			Id = Map.GenerateObjectID<MapItem>();
			X = x;
			Y = y;
		}
#region Database
		public new void Activate(IServer server)
		{
			base.Activate(server);

			SafeActivate(Owner);
		}
#endregion
		public new void Store()
		{
			base.Store();

			SafeStore(Owner);
		}

		public override void CalculateStats()
		{
			MaxHp = 1;
			MaxTp = 0;
			MinDamage = 0;
			MaxDamage = 0;
			Accuracy = 0;
			Evade = 0;
			Defence = 0;
		}

		public override void Effect(int effect, bool echo = true)
		{
			// TODO: Implement this?
		}


#region Packet Builders
		public override Packet AddToViewBuilder(WarpAnimation animation = WarpAnimation.None)
		{
			Packet packet = new Packet(PacketFamily.Item, PacketAction.Add);
			packet.AddShort(ItemId);
			packet.AddShort((short)Id);
			packet.AddThree(Amount);
			packet.AddChar(X);
			packet.AddChar(Y);
			return packet;
		}
		public override Packet DeleteFromViewBuilder(WarpAnimation animation = WarpAnimation.None)
		{
			Packet packet = new Packet(PacketFamily.Item, PacketAction.Remove);
			packet.AddShort((short)Id);
			return packet;
		}
		public override Packet WalkBuilder()
		{
			// TODO: Implement this
			return null;
		}
		public override Packet FaceBuilder()
		{
			// TODO: Implement this
			return null;
		}
		public override void InfoBuilder(ref Packet packet)
		{
			packet.AddShort((short)Id);
			packet.AddShort(ItemId);
			packet.AddChar(X);
			packet.AddChar(Y);
			packet.AddThree(Amount);
		}
#endregion
	}
}
