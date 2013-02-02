using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EOHax.EO;
using EOHax.EO.Communication;
using EOHax.EO.Data;

namespace EOHax.Programs.EOSERV
{
	class NPC : MapObject
	{
		public short id;

#region Null Accessors
		public short DataId
		{
			get { return id; }
			private set { id = value; }
		}
#endregion

		public NPC(IServer server, short id, ushort mapId, byte x, byte y)
			: base(server, null)
		{
			DataId = id;
			MapId = mapId;
			Id = Map.GenerateObjectID<NPC>();
			X = x;
			Y = y;
		}
#region Database
		public new void Activate(IServer server)
		{
			base.Activate(server);
		}
#endregion
		public new void Store()
		{
			base.Store();
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
			Packet packet = new Packet(PacketFamily.Appear, PacketAction.Reply);
			packet.AddChar(0); // ?
			packet.AddBreak();
			packet.AddChar((byte)Id);
			packet.AddShort(DataId);
			packet.AddChar(X);
			packet.AddChar(Y);
			packet.AddChar((byte)Direction);
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
			packet.AddByte((byte)Id);
			packet.AddShort(DataId);
			packet.AddChar(X);
			packet.AddChar(Y);
			packet.AddByte((byte)Direction);
		}
#endregion
	}
}
