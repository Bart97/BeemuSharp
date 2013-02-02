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
	public class Character : MapObject, IMessageSource, IMessageTarget, ICommandExecutor
	{
		// These have to be public to allow queries to work
		public string     name;
		public string     title;
		public short      classId;
		public Gender     gender;
		public Skin       skin;
		public AdminLevel admin;
		public byte       hairStyle;
		public byte       hairColor;
		public SitState   sitState = SitState.Stand;
		public byte       level;
		public int        exp;
		public short      strength;
		public short      intelligence;
		public short      wisdom;
		public short      agility;
		public short      constitution;
		public short      charisma;
		public short      statPoints;
		public short      skillPoints;
		public short      karma;
		public int        usage;
		public byte       weight;
		public byte       maxWeight;
		bool              warping;
		WarpAnimation     warpAnimation = WarpAnimation.None;
		private short     adjStrength;
		private short     adjIntelligence;
		private short     adjWisdom;
		private short     adjAgility;
		private short     adjConstruction;
		private short     adjCharisma;

		public List<ItemStack> items;
		//public Citizenship citizenship = null;
		//public Marriage marriage = null;
		//public BankAccount bank = null;
		//public GuildMembership guildMembership = null;

#region Paperdoll
		public Item boots;
		public Item charm;
		public Item gloves;
		public Item belt;
		public Item armor;
		public Item necklace;
		public Item hat;
		public Item shield;
		public Item weapon;
		public Item ring1;
		public Item ring2;
		public Item armlet1;
		public Item armlet2;
		public Item bracer1;
		public Item bracer2;
#endregion

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
			set { title = value; }
		}

		public short ClassId
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

		public AdminLevel Admin
		{
			get { return admin; }
			set { admin = value; }
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
			get { return (short)(strength + adjStrength); }
			private set { strength = value; }
		}

		public short Intelligence
		{
			get { return (short)(intelligence + adjIntelligence); }
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

		public byte Weight
		{
			get { return (weight > 250 ? (byte)250 : weight); }
			private set { weight = value; }
		}

		public byte MaxWeight
		{ // TODO: Limits
			get { return maxWeight; }
			private set { maxWeight = value; }
		}

		public List<ItemStack> Items
		{
			get { return items; }
			private set { items = value; }
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

#region Inventory Null Accessors
		public Item Boots
		{
			get { return boots; }
			private set { boots = value; }
		}
		public Item Charm
		{
			get { return charm; }
			private set { charm = value; }
		}
		public Item Gloves
		{
			get { return gloves; }
			private set { gloves = value; }
		}
		public Item Belt
		{
			get { return belt; }
			private set { belt = value; }
		}
		public Item Armor
		{
			get { return armor; }
			private set { armor = value; }
		}
		public Item Necklace
		{
			get { return necklace; }
			private set { necklace = value; }
		}
		public Item Hat
		{
			get { return hat; }
			private set { hat = value; }
		}
		public Item Shield
		{
			get { return shield; }
			private set { shield = value; }
		}
		public Item Weapon
		{
			get { return weapon; }
			private set { weapon = value; }
		}
		public Item Ring1
		{
			get { return ring1; }
			private set { ring1 = value; }
		}
		public Item Ring2
		{
			get { return ring2; }
			private set { ring2 = value; }
		}
		public Item Armlet1
		{
			get { return armlet1; }
			private set { armlet1 = value; }
		}
		public Item Armlet2
		{
			get { return armlet2; }
			private set { armlet2 = value; }
		}
		public Item Bracer1
		{
			get { return bracer1; }
			private set { bracer1 = value; }
		}
		public Item Bracer2
		{
			get { return bracer2; }
			private set { bracer2 = value; }
		}
#endregion
		public short MaxSp
		{
			get { return maxSp; }
			private set { maxSp = value; }
		}

		public bool Warping
		{
			get { return warping; }
			set
			{
				warping = value;
				warpAnimation = WarpAnimation.None;
			}
		}

		public WarpAnimation WarpAnimation
		{
			get { return warpAnimation; }
			private set { warpAnimation = value; }
		}
#endregion

		public bool Online
		{
			get { return Client != null; }
		}

		public ECF.Entry Class
		{
			get { return Server.ClassData[(ushort)classId]; }
		}

		public AccessLevel AccessLevel
		{
			get { return (AccessLevel)Admin; }
		}

		public Character(IServer server, IClient client, string name, Gender gender, byte hairStyle, byte hairColor, Skin skin) : base(server, client)
		{
			Client = client;

			Name = name;
			Gender = gender;
			HairStyle = hairStyle;
			HairColor = hairColor;
			Skin = skin;
			Items = new List<ItemStack>();

			MapId = Properties.Settings.Default.StartMap;
			X = Properties.Settings.Default.StartX;
			Y = Properties.Settings.Default.StartY;
		}
#region Database
		public void Activate(IServer server, IClient client)
		{
			base.Activate(server);

			Client = client;
			SafeActivate(items);
			
			//SafeActivate(citizenship);
			//SafeActivate(marriage);
			//SafeActivate(bank);
			//SafeActivate(guildMembership);
			foreach (ItemStack item in items)
			{
				item.Activate(server);
			}
			if (boots != null)
				boots.Activate(server);
			if (charm != null)
				charm.Activate(server);
			if (gloves != null)
				gloves.Activate(server);
			if (belt != null)
				belt.Activate(server);
			if (armor != null)
				armor.Activate(server);
			if (necklace != null)
				necklace.Activate(server);
			if (hat != null)
				hat.Activate(server);
			if (shield != null)
				shield.Activate(server);
			if (weapon != null)
				weapon.Activate(server);
			if (ring1 != null)
				ring1.Activate(server);
			if (ring2 != null)
				ring2.Activate(server);
			if (armlet1 != null)
				armlet1.Activate(server);
			if (armlet2 != null)
				armlet2.Activate(server);
			if (bracer1 != null)
				bracer1.Activate(server);
			if (bracer2 != null)
				bracer2.Activate(server);

			if (!Server.MapData.Exists(MapId))
			{
				MapId = Properties.Settings.Default.StartMap;
				X = Properties.Settings.Default.StartX;
				Y = Properties.Settings.Default.StartY;
			}
		}

		public new void Store()
		{
			base.Store();

			SafeStore(items);
			//SafeStore(citizenship);
			//SafeStore(marriage);
			//SafeStore(bank);
			//SafeStore(guildMembership);

			SafeStore(boots);
			SafeStore(charm);
			SafeStore(gloves);
			SafeStore(belt);
			SafeStore(armor);
			SafeStore(necklace);
			SafeStore(hat);
			SafeStore(shield);
			SafeStore(weapon);
			SafeStore(ring1);
			SafeStore(ring2);
			SafeStore(armlet1);
			SafeStore(armlet2);
			SafeStore(bracer1);
			SafeStore(bracer2);

			Server.Database.Commit();
		}
#endregion
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
			Weight = 0;
			MaxWeight = 70;
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

		public override void Warp(ushort map, byte x, byte y, WarpAnimation animation = WarpAnimation.None)
		{
			lock (this)
			lock (Map)
			{
				this.Warping = true;
				this.WarpAnimation = animation;
				Packet packet = new Packet(PacketFamily.Warp, PacketAction.Request);
				if (this.MapId == map)
				{
					packet.AddChar((byte)WarpReply.Local);
					packet.AddShort((short)map);
					packet.AddChar(x);
					packet.AddChar(y);
				}
				else
				{
					packet.AddChar((byte)WarpReply.Switch);
					packet.AddShort((short)map);
					packet.AddBytes(Map.Data.RevisionID);
					packet.AddThree((int)Map.Data.PubFileLength);
					packet.AddChar(0); // ?
					packet.AddChar(0); // ?
				}
				base.Warp(map, x, y, animation);
				Client.Send(packet);
			}
		}

		public void Refresh()
		{

		}

		public void Emote(Emote emote, bool echo = false)
		{
			Packet packet = new Packet(PacketFamily.Emote, PacketAction.Player);
			packet.AddShort((short)Client.Id);
			packet.AddChar((byte)emote);
			SendInRange(packet, echo);
		}

		public void Sit(SitState state)
		{
			SitState = state;

			Packet packet = new Packet((state == SitState.Chair) ? PacketFamily.Chair : PacketFamily.Sit, PacketAction.Player);
			packet.AddShort((short)Client.Id);
			packet.AddChar(X);
			packet.AddChar(Y);
			packet.AddChar((byte)Direction);
			packet.AddChar(0); // ?

			SendInRange(packet);
		}

		public void SitChair(byte x, byte y)
		{
			X = x;
			Y = y;
			Sit(SitState.Chair);
		}

		public void Stand()
		{
			bool chair = (SitState == SitState.Chair);
			if (chair)
			{
				switch (Direction)
				{
					case Direction.Up:
						--Y;
						break;

					case Direction.Right:
						++X;
						break;

					case Direction.Down:
						++Y;
						break;

					case Direction.Left:
						--X;
						break;
				}

				Packet cPacket = new Packet(PacketFamily.Chair, PacketAction.Close);
				cPacket.AddShort((short)Client.Id);
				cPacket.AddChar(X);
				cPacket.AddChar(Y);
				Client.Send(cPacket);
			}
			SitState = SitState.Stand;

			Packet packet = new Packet(PacketFamily.Sit, PacketAction.Remove);
			packet.AddShort((short)Client.Id);
			packet.AddChar(X);
			packet.AddChar(Y);
			SendInRange(packet, !chair);
		}

		public void SendMsg(IMessageTarget target, Message message)
		{
			target.RecieveMsg(this, message);
		}

		public void RecieveMsg(IMessageSource source, Message message)
		{
			if (typeof(MessageServer).IsInstanceOfType(source))
			{
				Packet packet = new Packet(PacketFamily.Talk, PacketAction.Server);
				packet.AddString(message.MessageString);
				Client.Send(packet);
			}
		}
#region Paperdoll
		public void Equip(short item, byte subloc)
		{
			if (item == 0) throw new ArgumentException("Tried to equip not existing item.");
			if (subloc < 0 || subloc > 1) throw new ArgumentOutOfRangeException("Subloc out of range.");
			switch (Server.ItemData[(ushort)(item - 1)].type)
			{
				case EIF.Type.Charm:
					GenericEquip(ref charm, item);
					break;

				case EIF.Type.Gloves:
					GenericEquip(ref gloves, item);
					break;

				case EIF.Type.Belt:
					GenericEquip(ref belt, item);
					break;

				case EIF.Type.Armor:
					GenericEquip(ref armor, item);
					break;

				case EIF.Type.Necklace:
					GenericEquip(ref necklace, item);
					break;

				case EIF.Type.Hat:
					GenericEquip(ref hat, item);
					break;

				case EIF.Type.Shield:
					GenericEquip(ref shield, item);
					break;

				case EIF.Type.Weapon:
					GenericEquip(ref weapon, item);
					break;

				case EIF.Type.Ring:
					if (subloc == 0)
						GenericEquip(ref ring1, item);
					else
						GenericEquip(ref ring2, item);
					break;

				case EIF.Type.Armlet:
					if (subloc == 0)
						GenericEquip(ref armlet1, item);
					else
						GenericEquip(ref armlet2, item);
					break;

				case EIF.Type.Bracer:
					if (subloc == 0)
						GenericEquip(ref bracer1, item);
					else
						GenericEquip(ref bracer2, item);
					break;

				default:
					return;
			}

			Packet packet = new Packet(PacketFamily.PaperDoll, PacketAction.Agree);
			packet.AddShort((short)Id);
			packet.AddChar((byte)AvatarSlot.Clothes);
			packet.AddChar(0); // wut?
			packet.AddShort((short)(Boots  != null ? Boots.Data.special1  : 0));
			packet.AddShort((short)(Armor  != null ? Armor.Data.special1  : 0));
			packet.AddShort((short)(Hat    != null ? Hat.Data.special1    : 0));
			packet.AddShort((short)(Weapon != null ? Weapon.Data.special1 : 0));
			packet.AddShort((short)(Shield != null ? Shield.Data.special1 : 0));
			packet.AddShort(item);
			packet.AddShort((short)HasItem(item));
			packet.AddChar(subloc);
			packet.AddShort(MaxHp);
			packet.AddShort(MaxTp);
			packet.AddShort(Strength);
			packet.AddShort(Intelligence);
			packet.AddShort(Wisdom);
			packet.AddShort(Agility);
			packet.AddShort(Constitution);
			packet.AddShort(Charisma);
			packet.AddShort(MinDamage);
			packet.AddShort(MaxDamage);
			packet.AddShort(Accuracy);
			packet.AddShort(Evade);
			packet.AddShort(Defence);
			Client.Send(packet);

			Packet _packet = new Packet(PacketFamily.Avatar, PacketAction.Agree);
			_packet.AddShort((short)Id);
			_packet.AddChar((byte)AvatarSlot.Clothes);
			_packet.AddChar(subloc);
			_packet.AddShort((short)(Boots  != null ? Boots.Data.special1  : 0));
			_packet.AddShort((short)(Armor  != null ? Armor.Data.special1  : 0));
			_packet.AddShort((short)(Hat    != null ? Hat.Data.special1    : 0));
			_packet.AddShort((short)(Weapon != null ? Weapon.Data.special1 : 0));
			_packet.AddShort((short)(Shield != null ? Shield.Data.special1 : 0));
			SendInRange(_packet, false);
		}

		private void GenericEquip(ref Item item, short id)
		{
			DelItem(id, 1, false);
			item = new Item(Server, id);
			CalculateStats();
			Store();
		}

		public void Unequip(short item, byte subloc)
		{
			if (item == 0) throw new ArgumentException("Tried to unequip not existing item.");
			if (subloc < 0 || subloc > 1) throw new ArgumentOutOfRangeException("Subloc out of range.");
			switch (Server.ItemData[(ushort)(item - 1)].type)
			{
				case EIF.Type.Charm:
					GenericUnequip(ref charm);
					break;

				case EIF.Type.Gloves:
					GenericUnequip(ref gloves);
					break;

				case EIF.Type.Belt:
					GenericUnequip(ref belt);
					break;

				case EIF.Type.Armor:
					GenericUnequip(ref armor);
					break;

				case EIF.Type.Necklace:
					GenericUnequip(ref necklace);
					break;

				case EIF.Type.Hat:
					GenericUnequip(ref hat);
					break;

				case EIF.Type.Shield:
					GenericUnequip(ref shield);
					break;

				case EIF.Type.Weapon:
					GenericUnequip(ref weapon);
					break;

				case EIF.Type.Ring:
					if (subloc == 0)
						GenericUnequip(ref ring1);
					else
						GenericUnequip(ref ring2);
					break;

				case EIF.Type.Armlet:
					if (subloc == 0)
						GenericUnequip(ref armlet1);
					else
						GenericUnequip(ref armlet2);
					break;

				case EIF.Type.Bracer:
					if (subloc == 0)
						GenericUnequip(ref bracer1);
					else
						GenericUnequip(ref bracer2);
					break;

				default:
					return;
			}

			Packet packet = new Packet(PacketFamily.PaperDoll, PacketAction.Remove);
			packet.AddShort((short)Id);
			packet.AddChar((byte)AvatarSlot.Clothes);
			packet.AddChar(0); // wut?
			packet.AddShort((short)(Boots  != null ? Boots.Data.special1  : 0));
			packet.AddShort((short)(Armor  != null ? Armor.Data.special1  : 0));
			packet.AddShort((short)(Hat    != null ? Hat.Data.special1    : 0));
			packet.AddShort((short)(Weapon != null ? Weapon.Data.special1 : 0));
			packet.AddShort((short)(Shield != null ? Shield.Data.special1 : 0));
			packet.AddShort(item);
			packet.AddChar(subloc);
			packet.AddShort(MaxHp);
			packet.AddShort(MaxTp);
			packet.AddShort(Strength);
			packet.AddShort(Intelligence);
			packet.AddShort(Wisdom);
			packet.AddShort(Agility);
			packet.AddShort(Constitution);
			packet.AddShort(Charisma);
			packet.AddShort(MinDamage);
			packet.AddShort(MaxDamage);
			packet.AddShort(Accuracy);
			packet.AddShort(Evade);
			packet.AddShort(Defence);
			Client.Send(packet);

			Packet _packet = new Packet(PacketFamily.Avatar, PacketAction.Agree);
			_packet.AddShort((short)Id);
			_packet.AddChar((byte)AvatarSlot.Clothes);
			_packet.AddChar(subloc);
			_packet.AddShort((short)(Boots  != null ? Boots.Data.special1  : 0));
			_packet.AddShort((short)(Armor  != null ? Armor.Data.special1  : 0));
			_packet.AddShort((short)(Hat    != null ? Hat.Data.special1    : 0));
			_packet.AddShort((short)(Weapon != null ? Weapon.Data.special1 : 0));
			_packet.AddShort((short)(Shield != null ? Shield.Data.special1 : 0));
			SendInRange(_packet, false);
		}

		private void GenericUnequip(ref Item item)
		{
			AddItem(item.Id, 1, false);
			item = null;
			CalculateStats();
			Store();
		}
#endregion

#region Item Related Functions
		public void AddItem(short id, int amount, bool sendPacket = true)
		{
			foreach (ItemStack item in items)
			{
				if (item.Id == id)
				{
					item.Amount += amount;
					item.Store();
					Store();
					goto packet;
				}
			}

			ItemStack newItem = new ItemStack(Client.Server, id, amount);
			Items.Add(newItem);
			newItem.Store();
			Store();
			Server.Database.Commit();
		packet:
			if (!sendPacket) return;

			Packet packet = new Packet(PacketFamily.Item, PacketAction.Obtain);
			packet.AddShort(id);
			packet.AddThree(amount);
			packet.AddChar(Weight);
			Client.Send(packet);
		}

		public bool HasItem(short id, int amount)
		{
			foreach (ItemStack item in Items)
			{
				if (item.Id == id && item.Amount >= amount)
					return true;
			}
			return false;
		}

		public int HasItem(short id)
		{
			foreach (ItemStack item in Items)
			{
				if (item.Id == id)
					return item.Amount;
			}
			return 0;
		}

		public bool DelItem(short id, int amount, bool sendPacket = true)
		{
			foreach (ItemStack item in Items)
			{
				if (item.Id == id)
				{
					if (item.Amount < amount)
						return false;
					else if (item.Amount == amount)
						Items.Remove(item);
					else
						item.amount -= amount;
					if (!sendPacket)
						return true;
					Packet packet = new Packet(PacketFamily.Item, PacketAction.Kick);
					packet.AddShort(id);
					packet.AddInt(item.Amount);
					packet.AddChar(Weight);
					Client.Send(packet);
					return true;
				}
			}
			return false;
		}

		public void PickItem(ushort id)
		{
			// TODO: Item distance check
			MapItem item = Map.GetObjectByID<MapItem>(id);

			if (item.Owner != this && item.UnprotectTime > DateTime.Now)
				throw new Exception("Item is ptotected"); ;

			AddItem(item.ItemId, item.Amount, false);

			Map.Leave(item, WarpAnimation.None, this);

			Packet packet = new Packet(PacketFamily.Item, PacketAction.Get);
			packet.AddShort((short)item.Id);
			packet.AddShort(item.ItemId);
			packet.AddThree(item.Amount);
			packet.AddChar(Weight);
			packet.AddChar(MaxWeight);
			Client.Send(packet);
		}

		public void DropItem(short id, int amount, byte x, byte y)
		{
			// TODO: Drop distance
			if (!Walkable(x, y) && Admin == AdminLevel.Player)
				return;

			if (HasItem(id, amount))
			{
				if (!DelItem(id, amount, false))
					return;
				MapItem item = new MapItem(Server, id, amount, this, MapId, x, y);
				Map.Enter(item, WarpAnimation.None, this);
				
				Packet packet = new Packet(PacketFamily.Item, PacketAction.Drop);
				packet.AddShort(id);
				packet.AddThree(amount);
				packet.AddInt(HasItem(id));
				packet.AddShort((short)item.Id);
				packet.AddChar(item.X);
				packet.AddChar(item.Y);
				packet.AddChar(Weight);
				packet.AddChar(MaxWeight);
				Client.Send(packet);
			}
		}
#endregion

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
			packet.AddShort((short)Map.Data.Id);
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
			packet.AddShort((short)(Boots != null ? Boots.Data.special1 : 0));
			packet.AddShort(0);
			packet.AddShort(0);
			packet.AddShort(0);
			packet.AddShort((short)(Armor != null ? Armor.Data.special1 : 0));
			packet.AddShort(0);
			packet.AddShort((short)(Hat != null ? Hat.Data.special1 : 0));
			packet.AddShort((short)(Shield != null ? Shield.Data.special1 : 0));
			packet.AddShort((short)(Weapon != null ? Weapon.Data.special1 : 0));
			packet.AddChar((byte)sitState);
			packet.AddChar(0); // Hidden
		}
#endregion
	}
}
