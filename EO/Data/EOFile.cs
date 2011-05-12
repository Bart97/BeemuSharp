using System;
using System.IO;
using System.Text;

namespace EOHax.EO.Data
{
	/// <summary>
	/// Handles conversions on file IO
	/// </summary>
	public class EOFile : IDisposable
	{
		/// <summary>
		/// Raw FileStream object
		/// </summary>
		private FileStream stream;

		/// <summary>
		/// Keeps track of the CRC32 hash of the written file
		/// </summary>
		private CRC32 hasher;

		private void WriteWrapper(byte[] array, int offset, int count)
		{
			stream.Write(array, offset, count);
			hasher.TransformBlock(array, offset, count, null, 0);
		}

		/// <summary>
		/// Value written by AddBreak
		/// </summary>
		public const byte Break = 0xFF;

		/// <summary>
		/// Constants for the maximum number that can be stored in a number of bytes in "EO Format"
		/// </summary>
		public static int[] Max = { 253, 64009, 16194277 };

		/// <summary>
		/// Encodes a number in to "EO Format"
		/// </summary>
		/// <param name="number">The number to be encoded</param>
		/// <param name="size">The size of the bte array returned</param>
		/// <returns>A byte array as large as size containing the encoded number</returns>
		public static byte[] EncodeNumber(int number, int size)
		{
			byte[] b = new byte[size];
			uint unumber = (uint)number;

			for (int i = 3; i >= 1; --i)
			{
				if (i >= b.Length)
				{
					if (unumber >= Max[i - 1])
						unumber = unumber % (uint)Max[i - 1];
				}
				else if (number >= Max[i - 1])
				{
					b[i] = (byte)(unumber / Max[i - 1] + 1);
					unumber = unumber % (uint)Max[i - 1];
				}
				else
				{
					b[i] = 254;
				}
			}

			b[0] = (byte)(unumber + 1);

			return b;
		}

		/// <summary>
		/// Decodes an "EO Format" number
		/// </summary>
		/// <param name="b">The byte array to decode</param>
		/// <returns>Returns the decoded number</returns>
		public static int DecodeNumber(byte[] b)
		{
			for (int i = 0; i < b.Length; ++i)
			{
				if (b[i] == 0 || b[i] == 254)
					b[i] = 0;
				else
					--b[i];
			}

			int a = 0;

			for (int i = b.Length - 1; i >= 1; --i)
			{
				a += b[i] * Max[i - 1];
			}

			return a + b[0];
		}

		/// <summary>
		/// Creates a wrapper around a FileStream object
		/// </summary>
		/// <param name="stream">FileStream object to wrap</param>
		public EOFile(FileStream stream)
		{
			this.stream = stream;
			hasher = new CRC32();
		}

		/// <summary>
		/// Adds a byte to the data stream (raw data).
		/// Uses 1 byte.
		/// </summary>
		public void AddByte(byte b) { WriteWrapper(new byte[] { b }, 0, 1); }

		/// <summary>
		/// Adds a "break" byte to the stream.
		/// Uses 1 byte.
		/// </summary>
		/// <see>Break</see>
		public void AddBreak() { AddByte(Break); }

		/// <summary>
		/// Adds a byte to the stream in "EO format".
		/// Uses 1 byte.
		/// </summary>
		public void AddChar(byte c) { WriteWrapper(EncodeNumber((int)c, 1), 0, 1); }

		/// <summary>
		/// Adds a short to the stream in "EO format".
		/// Uses 2 bytes.
		/// </summary>
		public void AddShort(short s) { WriteWrapper(EncodeNumber((int)s, 2), 0, 2); }

		/// <summary>
		/// Adds an integer to the stream in "EO format".
		/// Uses 3 bytes.
		/// </summary>
		public void AddThree(int t) { WriteWrapper(EncodeNumber((int)t, 3), 0, 3); }

		/// <summary>
		/// Adds an integer to the stream in "EO format".
		/// Uses 4 bytes.
		/// </summary>
		public void AddInt(int i) { WriteWrapper(EncodeNumber(i, 4), 0, 4); }

		/// <summary>
		/// Adds an array of bytes to the stream
		/// </summary>
		public void AddBytes(byte[] b) { WriteWrapper(b, 0, b.Length); }

		/// <summary>
		/// Adds a string encoded as ASCII to the stream
		/// </summary>
		public void AddString(string s)
		{
			byte[] b = ASCIIEncoding.ASCII.GetBytes(s);

			AddBytes(b);
		}

		/// <summary>
		/// Adds a string encoded as ASCII to the stream then adds a Break character
		/// </summary>
		public void AddBreakString(string s)
		{
			byte[] b = ASCIIEncoding.ASCII.GetBytes(s);

			for (int i = 0; i < b.Length; ++i)
			{
				if (b[i] == Break)
					b[i] = (byte)'y';
			}

			AddBytes(b);
			AddBreak();
		}

		/// <summary>
		/// Reads a byte from the data stream (raw data).
		/// Reads 1 byte
		/// </summary>
		public byte GetByte() { return (byte)stream.ReadByte(); }

		/// <summary>
		/// Reads an "EO format" byte from the stream.
		/// Reads 1 byte.
		/// </summary>
		public byte GetChar() { return (byte)DecodeNumber(GetBytes(1)); }

		/// <summary>
		/// Reads an "EO format" short from the stream.
		/// Reads 2 bytes.
		/// </summary>
		public short GetShort() { return (short)DecodeNumber(GetBytes(2)); }

		/// <summary>
		/// Reads an "EO format" integer from the stream.
		/// Reads 3 bytes.
		/// </summary>
		public int GetThree() { return DecodeNumber(GetBytes(3)); }

		/// <summary>
		/// Reads an "EO format" integer from the stream.
		/// Reads 4 bytes.
		/// </summary>
		public int GetInt() { return DecodeNumber(GetBytes(4)); }

		/// <summary>
		/// Reads a fixed amount of bytes from the stream
		/// </summary>
		/// <param name="length">Number of bytes to read</param>
		public byte[] GetBytes(int length)
		{
			byte[] b = new byte[length];
			stream.Read(b, 0, length);
			return b;
		}

		/// <summary>
		/// Reads a fixed amount of bytes from the stream and returns it as a string
		/// </summary>
		/// <param name="length">Number of bytes to read</param>
		public string GetFixedString(int length) { return ASCIIEncoding.ASCII.GetString(GetBytes(length)); }

		/// <summary>
		/// Skips a fixed number of bytes without returning any data
		/// </summary>
		/// <param name="bytes">Number of bytes to skip</param>
		public void Skip(int bytes)
		{
			stream.Seek(bytes, SeekOrigin.Current);
		}

		/// <summary>
		/// Writes the CRC32 hash of the file to the specified location in the file
		/// </summary>
		public byte[] WriteHash(long offset, SeekOrigin origin = SeekOrigin.Begin)
		{
			hasher.TransformFinalBlock(new byte[0], 0, 0);
			byte[] hashBytes = hasher.Hash;
			long pos = stream.Position;

			for (int i = 0; i < 4; ++i)
			{
				hashBytes[i] = (byte)(hashBytes[i] | 0x01);
			}

			stream.Seek(offset, origin);
			stream.Write(hashBytes, 0, 4);
			stream.Seek(pos, SeekOrigin.Begin);
				
			return hashBytes;
		}

		/// <summary>
		/// Closes the file
		/// </summary>
		public void Dispose()
		{
			hasher.Dispose();
			stream.Close();
		}
	}
}
