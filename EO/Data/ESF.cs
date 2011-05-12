using System;
using System.Collections.Generic;

namespace EOHax.EO.Data
{
	/// <summary>
	/// Endless Spell File reader/writer
	/// </summary>
	public class ESF : Pub
	{
		/// <summary>
		/// Target type of the spell.
		/// Determines if you have to be in a PK map to target players
		/// </summary>
		public enum TargetType : byte
		{
			Any,
			Attackable,
			Bard
		}

		/// <summary>
		/// Restricts the possible targets for the spell
		/// </summary>
		public enum TargetRestrict : byte
		{
			Any,
			Friendly,
			Opponent
		}

		/// <summary>
		/// Specifies who the spell effects
		/// </summary>
		public enum Target : byte
		{
			Normal,
			Self,
			Unknown,
			Group
		}

		/// <summary>
		/// Returns the magic file type header
		/// </summary>
		public override string FileType { get { return "ESF"; } }

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
		public ESF() : base() { }

		/// <summary>
		/// Load from a pub file
		/// </summary>
		/// <param name="fileName">File to read the data from</param>
		public ESF(string fileName) : base(fileName) { }
		
		/// <summary>
		/// Class data entry
		/// </summary>
		public class Entry : IPubEntry
		{
			public string name = String.Empty;
			public string shout = String.Empty;

			public short icon;
			public short graphic;

			public short tp;
			public short sp;

			public byte castTime;

			public byte unknown1;

			public TargetType targetType;

			public byte unknown2;
			public byte unknown3;
			public byte unknown4;
			public byte unknown5;
			public byte unknown6;
			public byte unknown7;

			public TargetRestrict targetRestrict;
			public Target target;

			public byte unknown8;
			public byte unknown9;
			public byte unknown10;
			public byte unknown11;

			public short minDamage;
			public short maxDamage;
			public short accuracy;

			public byte unknown12;
			public byte unknown13;
			public byte unknown14;
			public byte unknown15;
			public byte unknown16;

			public short hp;

			public byte unknown17;
			public byte unknown18;
			public byte unknown19;
			public byte unknown20;
			public byte unknown21;
			public byte unknown22;
			public byte unknown23;
			public byte unknown24;
			public byte unknown25;
			public byte unknown26;
			public byte unknown27;
			public byte unknown28;
			public byte unknown29;
			public byte unknown30;
			public byte unknown31;
		}
	}
}
