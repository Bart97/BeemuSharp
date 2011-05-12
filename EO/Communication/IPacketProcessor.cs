using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EOHax.EO.Communication
{
	public interface IPacketProcessor
	{
		/// <summary>
		/// Multiple used by the SwapMultiples algorithm.
		/// If set to 255 decoding is disabled.
		/// </summary>
		/// <remarks>
		/// Keep in mind this is equal to SendMulti on the other side.
		/// </remarks>
		byte RecvMulti { get; }
		
		/// <summary>
		/// Multiple used by the SwapMultiples algorithm
		/// Keep in mind this is equal to RecvMulti on the other side.
		/// If set to 255 encoding is disabled.
		/// </summary>
		/// <remarks>
		/// Keep in mind this is equal to RecvMulti on the other side.
		/// </remarks>
		byte SendMulti { get; }

		/// <summary>
		/// Sets RecvMulti and SendMulti
		/// </summary>
		/// <param name="recvMulti">Value for RecvMulti</param>
		/// <param name="sendMulti">Value for SendMulti</param>
		void SetMulti(byte recvMulti, byte sendMulti);

		/// <summary>
		/// Encodes a byte array.
		/// If SendMulti is -1 this is a no-op.
		/// </summary>
		/// <param name="original">Array of bytes to be encoded</param>
		void Encode(ref byte[] original);

		/// <summary>
		/// Decodes a byte array.
		/// If RecvMulti is -1 this is a no-op.
		/// </summary>
		/// <param name="original">Array of bytes to be decoded</param>
		void Decode(ref byte[] original);
	}
}
