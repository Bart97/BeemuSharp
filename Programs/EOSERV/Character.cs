using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using EOHax.EO;
using EOHax.EO.Communication;
using EOHax.EOSERV.Data;

namespace EOHax.Programs.EOSERV
{
	public class Character : MapObject
	{
		// These have to be public to allow queries to work
		public string   name;
		public string   title;
		public string   classId;
		public Gender   gender;
		public Skin     skin;
		public byte     hairStyle;
		public byte     hairColor;
		public SitState sitState = SitState.Stand;
		public byte     level;
		public int      exp;
		public short    strength;
		public short    intelligence;
		public short    wisdom;
		public short    agility;
		public short    constitution;
		public short    charisma;
		public short    statPoints;
		public short    skillPoints;
		public short    karma;
		public int      usage;

		//public Citizenship citizenship = null;
		//public Marriage marriage = null;
		//public BankAccount bank = null;
		//public GuildMembership guildMembership = null;

		[NonSerialized] private short maxSp;

#region Null Accessors
		public string Name
		{
			get { return name; }
			private set { name = value; }
		}

		public string Title
		{
			get { return title; }
			private set { title = value; }
		}

		public string ClassId
		{
			get { return classId; }
			private set { classId = value; }
		}

		public Gender Gender
		{
			get { return gender; }
			private set { gender = value; }
		}

		public Skin Skin
		{
			get { return skin; }
			private set { skin = value; }
		}

		public byte HairStyle
		{
			get { return hairStyle; }
			private set { hairStyle = value; }
		}

		public byte HairColor
		{
			get { return hairColor; }
			private set { hairColor = value; }
		}

		public SitState SitState
		{
			get { return sitState; }
			private set { sitState = value; }
		}

		public byte Level
		{
			get { return level; }
			private set { level = value; }
		}

		public int Exp
		{
			get { return exp; }
			private set { exp = value; }
		}

		public short Strength
		{
			get { return strength; }
			private set { strength = value; }
		}

		public short Intelligence
		{
			get { return intelligence; }
			private set { intelligence = value; }
		}

		public short Wisdom
		{
			get { return wisdom; }
			private set { wisdom = value; }
		}

		public short Agility
		{
			get { return agility; }
			private set { agility = value; }
		}

		public short Constitution
		{
			get { return constitution; }
			private set { constitution = value; }
		}

		public short Charisma
		{
			get { return charisma; }
			private set { charisma = value; }
		}

		public short StatPoints
		{
			get { return statPoints; }
			private set { statPoints = value; }
		}

		public short SkillPoints
		{
			get { return skillPoints; }
			private set { skillPoints = value; }
		}

		public short Karma
		{
			get { return karma; }
			private set { karma = value; }
		}

		public int Usage
		{
			get { return usage; }
			private set { usage = value; }
		}

		/*public Citizenship Citizenship
		{
			get { return citizenship; }
			private set { citizenship = value; }
		}

		public Marriage Marriage
		{
			get { return marriage; }
			private set { marriage = value; }
		}

		public BankAccount Bank
		{
			get { return bank; }
			private set { bank = value; }
		}

		public GuildMembership GuildMembership
		{
			get { return guildMembership; }
			private set { guildMembership = value; }
		}*/

		public short MaxSp
		{
			get { return maxSp; }
			private set { maxSp = value; }
		}
#endregion

		public bool Online
		{
			get { return Client != null; }
		}

		public ClassData Class
		{
			get { return Server.ClassData[classId]; }
		}

		public Character(IServer server, IClient client, string name, Gender gender, byte hairStyle, byte hairColor, Skin skin) : base(server, client)
		{
			Client = client;

			Name = name;
			Gender = gender;
			HairStyle = hairStyle;
			HairColor = hairColor;
			Skin = skin;

			// TODO: Get default from server
			MapId = "SAUSAGE_CASTLE_OUTSIDE";
			X = 10;
			Y = 10;
		}

		public void Activate(IServer server, IClient client)
		{
			base.Activate(server);

			Client = client;

			//SafeActivate(citizenship);
			//SafeActivate(marriage);
			//SafeActivate(bank);
			//SafeActivate(guildMembership);
		}

		public new void Store()
		{
			base.Store();

			//SafeStore(citizenship);
			//SafeStore(marriage);
			//SafeStore(bank);
			//SafeStore(guildMembership);
		}

		public static bool ValidName(string name)
		{
			return new Regex("[a-z]{4,12}").Match(name).Success;
		}

		public override void CalculateStats()
		{
			MaxHp = 120;
			MaxTp = 80;
			MinDamage = 15;
			MaxDamage = 25;
			Accuracy = 40;
			Evade = 30;
			Defence = 50;
		}

		public override bool Walkable(byte x, byte y)
		{
			lock (Map)
			{
				return true;
			}
		}

		public override void Effect(int effect, bool echo = true)
		{

		}

		public override void Face(Direction direction, bool echo = true)
		{
			lock (Map)
			{
				base.Face(direction);
				SendInRange(FaceBuilder(), false);
			}
		}

		public override bool Walk(Direction direction, WalkType type = WalkType.Normal, bool echo = true)
		{
			lock (this)
			lock (Map)
			{
				List<IMapObject> exitView = new List<IMapObject>();
				List<IMapObject> enterView = new List<IMapObject>();
				IEnumerable<IMapObject> oldObjects = GetInRange<IMapObject>();

				if (base.Walk(direction, type, echo))
				{
					IEnumerable<IMapObject> newObjects = GetInRange<IMapObject>();

					// NOTE: This is rather slow, in exchange for clarity
					IEnumerable<IMapObject> addedObjects = newObjects.Except(oldObjects);
					IEnumerable<IMapObject> deletedObjects = oldObjects.Except(newObjects);
					IEnumerable<IMapObject> staticObjects = newObjects.Intersect(oldObjects);

					foreach (IMapObject obj in deletedObjects)
					{
						Client.Send(obj.DeleteFromViewBuilder());
						obj.Client.Send(DeleteFromViewBuilder());
					}

					var x = GetInRange<Character>();

					foreach (IMapObject obj in addedObjects)
					{
						Client.Send(obj.AddToViewBuilder());
						obj.Client.Send(AddToViewBuilder());
					}

					Packet walkPacket = WalkBuilder();

					foreach (IMapObject obj in newObjects)
					{
						if (obj != Client.Character)
							obj.Client.Send(walkPacket);
					}

					return true;
				}
				else
				{
					return false;
				}
			}
		}

		public void Refresh()
		{

		}

#region Packet Builders
		public override Packet AddToViewBuilder(WarpAnimation animation = WarpAnimation.None)
		{
			Packet packet = new Packet(PacketFamily.Players, PacketAction.Agree);
			packet.AddBreak();
			InfoBuilder(ref packet);
			packet.AddChar((byte)animation);
			packet.AddBreak();
			packet.AddChar(1); // ?
			return packet;
		}

		public override Packet DeleteFromViewBuilder(WarpAnimation animation = WarpAnimation.None)
		{
			Packet packet = new Packet(PacketFamily.Avatar, PacketAction.Remove);
			packet.AddShort((short)Client.Id);
			packet.AddChar((byte)animation);
			return packet;
		}

		public override Packet WalkBuilder()
		{
			Packet packet = new Packet(PacketFamily.Walk, PacketAction.Player);
			packet.AddShort((short)Client.Id);
			packet.AddChar((byte)Direction);
			packet.AddChar(X);
			packet.AddChar(Y);
			return packet;
		}

		public override Packet FaceBuilder()
		{
			Packet packet = new Packet(PacketFamily.Face, PacketAction.Player);
			packet.AddShort((short)Client.Id);
			packet.AddChar((byte)Direction);
			return packet;
		}

		public override void InfoBuilder(ref Packet packet)
		{
			packet.AddBreakString(Name);
			packet.AddShort((short)Client.Id);
			packet.AddShort((short)Map.Data.PubId);
			packet.AddShort(X);
			packet.AddShort(Y);
			packet.AddChar((byte)Direction);
			packet.AddChar(6);
			packet.AddString("TAG");
			packet.AddChar(level);
			packet.AddChar((byte)gender);
			packet.AddChar(HairStyle);
			packet.AddChar(HairColor);
			packet.AddChar((byte)Skin);
			packet.AddShort(MaxHp);
			packet.AddShort(Hp);
			packet.AddShort(MaxTp);
			packet.AddShort(Tp);
			packet.AddShort(2); // paperdoll
			packet.AddShort(0);
			packet.AddShort(0);
			packet.AddShort(0);
			packet.AddShort(2);
			packet.AddShort(0);
			packet.AddShort(2);
			packet.AddShort(2);
			packet.AddShort(2); // /paperdoll
			packet.AddChar((byte)sitState);
			packet.AddChar(0); // Hidden
		}
#endregion
	}
}
