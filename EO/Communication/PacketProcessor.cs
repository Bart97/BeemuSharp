﻿using System;

namespace EOHax.EO.Communication
{
	/// <summary>
	/// Handles packet encoding and decoding state
	/// </summary>
	public abstract class PacketProcessor : IPacketProcessor
	{
		public byte RecvMulti { get; private set; }
		public byte SendMulti { get; private set; }

		public void SetMulti(byte recvMulti, byte sendMulti)
		{
			if (RecvMulti != 0 || SendMulti != 0)
				throw new ApplicationException("PacketProcessor multiples already set");

			RecvMulti = recvMulti;
			SendMulti = sendMulti;
		}
		
		/// <summary>
		/// Internal packet encoding algorithm.
		/// "Interleaves" the byte array.
		/// </summary>
		/// <param name="b">Byte array to perform the operation on</param>
		public static void Interleave(ref byte[] b)
		{
			byte[] newstr = new byte[b.Length];
			int i = 0;
			int ii = 0;

			for (; i < b.Length; i += 2)
				newstr[i] = b[ii++];

			--i;

			if (b.Length % 2 != 0)
				i -= 2;

			for (; i >= 0; i -= 2)
				newstr[i] = b[ii++];

			newstr.CopyTo(b, 0);
		}

		/// <summary>
		/// Internal packet encoding algorithm.
		/// Reverses the effect of the Interleave function.
		/// </summary>
		/// <param name="b">Byte array to perform the operation on</param>
		public static void Deinterleave(ref byte[] b)
		{
			byte[] newstr = new byte[b.Length];
			int i = 0;
			int ii = 0;

			for (; i < b.Length; i += 2)
				newstr[ii++] = b[i];

			--i;

			if (b.Length % 2 != 0)
				i -= 2;

			for (; i >= 0; i -= 2)
				newstr[ii++] = b[i];

			newstr.CopyTo(b, 0);
		}

		/// <summary>
		/// Internal packet encoding algorithm.
		/// Flips the MSB of all bytes.
		/// </summary>
		/// <param name="b">Byte array to perform the operation on</param>
		public static void FlipMSB(ref byte[] b)
		{
			for (int i = 0; i < b.Length; ++i)
			{
				b[i] = (byte)(b[i] ^ 0x80);
			}
		}

		/// <summary>
		/// Internal packet encoding algorithm.
		/// Flips sequences of numbers of which multi are a divisor.
		/// </summary>
		/// <param name="b">Byte array to perform the operation on</param>
		/// <param name="multi">Number to test for divisibility</param>
		public static void SwapMultiples(ref byte[] b, int multi)
		{
			int sequenceLength = 0;

			if (multi <= 0)
				return;

			for (int i = 0; i <= b.Length; ++i)
			{
				if (i != b.Length && b[i] % multi == 0)
				{
					++sequenceLength;
				}
				else
				{
					if (sequenceLength > 1)
					{
						for (int ii = 0; ii < sequenceLength / 2; ++ii)
						{
							byte temp = b[i - sequenceLength + ii];
							b[i - sequenceLength + ii] = b[i - ii - 1];
							b[i - ii - 1] = temp;
						}
					}

					sequenceLength = 0;
				}
			}
		}

		public abstract void Encode(ref byte[] original);
		public abstract void Decode(ref byte[] original);
	}
}
