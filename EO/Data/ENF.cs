using System;
using System.Collections.Generic;

namespace EOHax.EO.Data
{
	/// <summary>
	/// Endless NPC File reader/writer
	/// </summary>
	public class ENF : Pub
	{
		/// <summary>
		/// Type of the npc.
		/// Determines it's generic behaviour.
		/// </summary>
		public enum Type : short
		{
			NPC,
			Passive,
			Aggressive,
			Unknown1,
			Unknown2,
			Unknown3,
			Shop,
			Inn,
			Unknown4,
			Bank,
			Barber,
			Guild,
			Priest,
			Law,
			Skills,
			Quest
		}

		/// <summary>
		/// Returns the magic file type header
		/// </summary>
		public override string FileType { get { return "ENF"; } }

        /// <summary>
        /// Returns a element of a key
        /// </summary>
        /// <param name="key">The key that you want the element of</param>
        /// <returns>Entry of that key</returns>
        public Entry this[ushort id]
        {
            get
            {
                if (id > data.Count) return (Entry)data[id];
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
		public ENF() : base() { }

		/// <summary>
		/// Load from a pub file
		/// </summary>
		/// <param name="fileName">File to read the data from</param>
		public ENF(string fileName) : base(fileName) { }
		
		/// <summary>
		/// NPC data entry
		/// </summary>
		public class Entry : IPubEntry
		{
			public string name = String.Empty;
			public short graphic;

			public byte unknown1;

			public short boss;
			public short child;
			public Type type;

			public byte unknown2;
			public byte unknown3;

			[ThreeByte] public int hp;

			public byte unknown4;
			public byte unknown5;

			public short minDamage;
			public short maxDamage;
			public short accuracy;
			public short evade;
			public short armor;

			public byte unknown6;
			public byte unknown7;
			public byte unknown8;
			public byte unknown9;
			public byte unknown10;
			public byte unknown11;
			public byte unknown12;
			public byte unknown13;
			public byte unknown14;
			public byte unknown15;

			public ushort exp;

			public byte unknown16;
		}
	}
}
