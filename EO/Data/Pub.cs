using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace EOHax.EO.Data
{
	/// <summary>
	/// Pub file data entry
	/// </summary>
	public interface IPubEntry { }

	/// <summary>
	/// Base class for "pub" files
	/// </summary>
	public abstract class Pub : IEnumerable<IPubEntry>
	{
		/// <summary>
		/// Marks a field as a three byte integer
		/// </summary>
		[AttributeUsage(AttributeTargets.Field)]
		protected sealed class ThreeByteAttribute : Attribute { }

		/// <summary>
		/// Returns the magic file type header
		/// </summary>
		public abstract string FileType { get; }

		/// <summary>
		/// A unique number for the version of the file.
		/// This is set after Write() is called for new files.
		/// </summary>
		/// <remarks>
		/// EOHax defacto standard is to set this to the CRC32 of the entire file output with this field to 0x00000000
		/// 0x00 bytes are represented in 0x01 to avoid problems with file transfer
		/// </remarks>
		public byte[] revisionId = new byte[4];

		/// <summary>
		/// File data entries
		/// </summary>
		public List<IPubEntry> data;

		/// <summary>
		/// Return a new class-specific data entry
		/// </summary>
		public abstract IPubEntry EntryFactory();

        /// <summary>
        /// Ammount of entries
        /// </summary>
        public int Count
        {
            get { return data.Count; }
        }

		/// <summary>
		/// Returns an enumerator for the pub file entries
		/// </summary>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return data.GetEnumerator();
		}

		/// <summary>
		/// Returns an enumerator for the pub file entries
		/// </summary>
		IEnumerator<IPubEntry> IEnumerable<IPubEntry>.GetEnumerator()
		{
			return data.GetEnumerator();
		}

		/// <summary>
		/// Creates an empty data set
		/// </summary>
		public Pub()
		{
			data = new List<IPubEntry>();
		}

		/// <summary>
		/// Load from a pub file
		/// </summary>
		/// <param name="fileName">File to read the data from</param>
		public Pub(string fileName)
		{
			Read(fileName);
		}

		/// <summary>
		/// Read a pub file
		/// </summary>
		/// <param name="fileName">File to read the data from</param>
		public void Read(string fileName)
		{
			using (EOFile file = new EOFile(File.Open(fileName, FileMode.Open, FileAccess.Read)))
			{
				if (file.GetFixedString(3) != FileType)
					throw new Exception("Corrupt or not a " + FileType + " file");

				uint revisionId = (uint)file.GetInt();

				this.revisionId = new byte[4] {
					(byte)((revisionId & 0xFF000000) >> 24),
					(byte)((revisionId & 0xFF0000) >> 16),
					(byte)((revisionId & 0xFF00) >> 8),
					(byte)(revisionId & 0xFF)
				};

				int count = file.GetShort();
				file.Skip(1); // TODO: What is this?

				data = new List<IPubEntry>(count);
			
				for (int i = 0; i < count; ++i)
				{
					IPubEntry entry = EntryFactory();
					GetEntry(file, ref entry);
					data.Add(entry);
				}
			}
		}

		/// <summary>
		/// Writes the full pub file to disk
		/// </summary>
		/// <param name="fileName">Name of the file to write to</param>
		public void Write(string fileName)
		{
			using (EOFile file = new EOFile(File.Open(fileName, FileMode.Create, FileAccess.Write)))
			{
				file.AddString(FileType);
				file.AddInt(0);
				file.AddShort((short)data.Count);
				file.AddChar(0); // TODO: What is this?

				foreach (IPubEntry entry in data)
				{
					AddEntry(file, entry);
				}

				revisionId = file.WriteHash(3);
			}
		}

		/// <summary>
		/// Loads a single entry from the pub file in to an IPubEntry object
		/// </summary>
		/// <param name="file">EOFile object to read</param>
		/// <param name="entry">Entry object to read in to</param>
		private void GetEntry(EOFile file, ref IPubEntry entry)
		{
			List<byte> stringLengths = new List<byte>();
			int i = 0;

			FieldInfo[] fields = entry.GetType().GetFields();

			foreach (FieldInfo member in fields)
			{
				if (member.FieldType == typeof(string))
					stringLengths.Add(file.GetChar());
			}

			foreach (FieldInfo member in fields)
			{
				Type memberType = member.FieldType;

				if (memberType.IsEnum)
					memberType = memberType.GetEnumUnderlyingType();

				if (memberType == typeof(char))
				{
					member.SetValue(entry, (char)file.GetChar());
				}
				else if (memberType == typeof(byte))
				{
					member.SetValue(entry, file.GetChar());
				}
				else if (memberType == typeof(short))
				{
					member.SetValue(entry, file.GetShort());
				}
				else if (memberType == typeof(ushort))
				{
					member.SetValue(entry, (ushort)file.GetShort());
				}
				else if (memberType == typeof(int))
				{
					if (member.GetCustomAttributes(typeof(ThreeByteAttribute), false).Length == 0)
						member.SetValue(entry, file.GetInt());
					else
						member.SetValue(entry, file.GetThree());
				}
				else if (memberType == typeof(uint))
				{
					if (member.GetCustomAttributes(typeof(ThreeByteAttribute), false).Length == 0)
						member.SetValue(entry, (uint)file.GetInt());
					else
						member.SetValue(entry, (uint)file.GetThree());
				}
				else if (memberType == typeof(string))
				{
					member.SetValue(entry, file.GetFixedString(stringLengths[i++]));
				}
				else
				{
					throw new Exception("Cannot represent " + memberType + " in pub file");
				}
			}
		}

		/// <summary>
		/// Adds an entry to the pub file
		/// </summary>
		/// <param name="file">EOFile object to write to</param>
		/// <param name="entry">Entry to append to the file</param>
		private void AddEntry(EOFile file, IPubEntry entry)
		{
			FieldInfo[] fields = entry.GetType().GetFields();

			foreach (FieldInfo member in fields)
			{
				if (member.FieldType == typeof(string))
					file.AddChar((byte)((string)member.GetValue(entry)).Length);
			}

			foreach (FieldInfo member in fields)
			{
				Type memberType = member.FieldType;

				if (memberType.IsEnum)
					memberType = memberType.GetEnumUnderlyingType();

				if (memberType == typeof(char))
				{
					file.AddChar((byte)(char)member.GetValue(entry));
				}
				else if (memberType == typeof(byte))
				{
					file.AddChar((byte)member.GetValue(entry));
				}
				else if (memberType == typeof(short))
				{
					file.AddShort((short)member.GetValue(entry));
				}
				else if (memberType == typeof(ushort))
				{
					file.AddShort((short)(ushort)member.GetValue(entry));
				}
				else if (memberType == typeof(int))
				{
					if (member.GetCustomAttributes(typeof(ThreeByteAttribute), false).Length == 0)
						file.AddInt((int)member.GetValue(entry));
					else
						file.AddThree((int)member.GetValue(entry));
				}
				else if (memberType == typeof(uint))
				{
					if (member.GetCustomAttributes(typeof(ThreeByteAttribute), false).Length == 0)
						file.AddInt((int)(uint)member.GetValue(entry));
					else
						file.AddThree((int)(uint)member.GetValue(entry));
				}
				else if (memberType == typeof(string))
				{
					file.AddString((string)member.GetValue(entry));
				}
				else
				{
					throw new Exception("Cannot represent " + memberType.Name + " in pub file");
				}
			}
		}
	}
}
