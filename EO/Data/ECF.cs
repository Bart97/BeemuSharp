using System;
using System.Collections.Generic;

namespace EOHax.EO.Data
{
	/// <summary>
	/// Endless Class File reader/writer
	/// </summary>
	public class ECF : Pub
	{
		/// <summary>
		/// Type of the class.
		/// Determines the stat calculations used.
		/// </summary>
		public enum Type : byte
		{
			Melee,
			Rogue,
			Magician,
			Archer,
			Peasant
		}

		/// <summary>
		/// Returns the magic file type header
		/// </summary>
		public override string FileType { get { return "ECF"; } }

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
		public ECF() : base() { }

		/// <summary>
		/// Load from a pub file
		/// </summary>
		/// <param name="fileName">File to read the data from</param>
		public ECF(string fileName) : base(fileName) { }
		
		/// <summary>
		/// Class data entry
		/// </summary>
		public class Entry : IPubEntry
		{
			public string name = String.Empty;

			public byte baseClass;
			public Type type;

			public short strength;
			public short intelligence;
			public short wisdom;
			public short agility;
			public short constitution;
			public short charisma;
		}
	}
}
