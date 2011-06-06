using System;
using System.Collections;
using System.Collections.Generic;

namespace EOHax.EO.Data
{
	/// <summary>
	/// Endless Item File reader/writer
	/// </summary>
	public class EIF : Pub
	{
		/// <summary>
		/// Type of the item.
		/// Determines it's generic behaviour.
		/// </summary>
		public enum Type : byte
		{
			Static,
			Unknown,
			Money,
			Heal,
			Teleport,
			Spell,
			EXPReward,
			StatReward,
			SkillReward,
			Key,
			Weapon,
			Shield,
			Armor,
			Hat,
			Boots,
			Gloves,
			Charm,
			Belt,
			Necklace,
			Ring,
			Armlet,
			Bracer,
			Beer,
			EffectPotion,
			HairDye,
			CureCurse
		}

		/// <summary>
		/// Sub-type of the item.
		/// Determines type-specific behaviour of the item.
		/// </summary>
		public enum SubType : byte
		{
			None,
			Ranged,
			Arrows,
			Wings
		}

		/// <summary>
		/// Special property the item carries.
		/// Effects the color of the item name in the inventory.
		/// </summary>
		public enum Special : byte
		{
			Normal,
			Rare,
			Unknown,
			Unique,
			Lore,
			Cursed
		}

		/// <summary>
		/// Fixed item size constants
		/// </summary>
		public enum Size : byte
		{
			Size1x1,
			Size1x2,
			Size1x3,
			Size1x4,
			Size2x1,
			Size2x2,
			Size2x3,
			Size2x4
		}

		/// <summary>
		/// Returns the width component of the Size passed
		/// </summary>
		/// <param name="size">Size member of the item</param>
		public static int SizeWidth(Size size)
		{
			switch (size)
			{
				case Size.Size1x1: return 1;
				case Size.Size1x2: return 1;
				case Size.Size1x3: return 1;
				case Size.Size1x4: return 1;
				case Size.Size2x1: return 2;
				case Size.Size2x2: return 2;
				case Size.Size2x3: return 2;
				case Size.Size2x4: return 2;
				default:           return 0;
			}
		}
		
		/// <summary>
		/// Returns the height component of the Size passed
		/// </summary>
		/// <param name="size">Size member of the item</param>
		public static int SizeHeight(Size size)
		{
			switch (size)
			{
				case Size.Size1x1: return 1;
				case Size.Size1x2: return 2;
				case Size.Size1x3: return 3;
				case Size.Size1x4: return 4;
				case Size.Size2x1: return 1;
				case Size.Size2x2: return 2;
				case Size.Size2x3: return 3;
				case Size.Size2x4: return 4;
				default:           return 0;
			}
		}

		/// <summary>
		/// Returns the total number of tiles in the inventory taken
		/// </summary>
		/// <param name="size">Size member of the item</param>
		public static int SizeTiles(Size size)
		{
			switch (size)
			{
				case Size.Size1x1: return 1;
				case Size.Size1x2: return 2;
				case Size.Size1x3: return 3;
				case Size.Size1x4: return 4;
				case Size.Size2x1: return 2;
				case Size.Size2x2: return 4;
				case Size.Size2x3: return 6;
				case Size.Size2x4: return 8;
				default:           return 0;
			}
		}

		/// <summary>
		/// Returns the magic file type header
		/// </summary>
		public override string FileType { get { return "EIF"; } }

        /// <summary>
        /// Returns a element of a key
        /// </summary>
        /// <param name="key">The key that you want the element of</param>
        /// <returns>Entry of that key</returns>
        public Entry this[ushort id]
        {
            get
            {
                if (id <= Count) return (Entry)data[id];
                else return null;
            }
        }

		/// <summary>
		/// Return a new class-specific data entry
		/// </summary>
		public override IPubEntry EntryFactory()
		{
			return new Entry();
		}

		/// <summary>
		/// Creates an empty data set
		/// </summary>
		public EIF() : base() { }

		/// <summary>
		/// Load from a pub file
		/// </summary>
		/// <param name="fileName">File to read the data from</param>
		public EIF(string fileName) : base(fileName) { }
		
		/// <summary>
		/// Return the index of the keynum-th Key type item
		/// </summary>
		/// <param name="keynum">The key ID</param>
		/// <returns>Key index or -1 on failure</returns>
		public int GetKey(int keynum)
		{
			int current = 0;

			for (int i = 0; i < data.Count; ++i)
			{
				if (((EIF.Entry)data[i]).type == Type.Key)
				{
					if (++current == i)
						return i;
				}
			}
			
			return -1;
		}

		/// <summary>
		/// Item data entry
		/// </summary>
		public class Entry : IPubEntry
		{
			public string name = String.Empty;
			public short graphic;
			public Type type;
			public SubType subtype;
			public Special special;

			public short hp;
			public short tp;
			public short minDamage;
			public short maxDamage;
			public short accuracy;
			public short evade;
			public short armor;

			public byte unknown1;

			public byte strength;
			public byte intelligence;
			public byte wisdom;
			public byte agility;
			public byte constitution;
			public byte charisma;

			public byte light;
			public byte dark;
			public byte earth;
			public byte air;
			public byte water;
			public byte fire;

			[ThreeByte] public int special1;
			public byte special2;
			public byte special3;

			public short levelRequirement;
			public short classRequirement;

			public short strRequirement;
			public short intRequirement;
			public short wisRequirement;
			public short agiRequirement;
			public short conRequirement;
			public short chaRequirement;

			public byte unknown2;
			public byte unknown3;

			public byte weight;

			public byte unknown4;

			public Size size;
		}
	}
}
