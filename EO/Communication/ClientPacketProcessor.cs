using System;

namespace EOHax.EO.Communication
{
	/// <summary>
	/// Packet processor for the client side
	/// </summary>
	public class ClientPacketProcessor : PacketProcessor
	{
		public byte SequenceStart { get; set; }
		public byte SequenceValue { get; set; }
		
		public byte GetSequence()
		{
			byte val = SequenceValue;

			if (++SequenceValue == 10)
				SequenceValue = 0;

			return (byte)(SequenceStart + val);
		}

		public void AddSequenceByte(ref byte[] original)
		{
			byte[] newPacket = new byte[original.Length + 1];
			Array.Copy(original, 0, newPacket, 0, 2);
			newPacket[2] = Packet.EncodeNumber(GetSequence(), 1)[2];
			Array.Copy(original, 2, newPacket, 3, original.Length - 2);
			original = newPacket;
		}

		public override void Encode(ref byte[] original)
		{
			if (SendMulti == 0 || original[1] == (byte)PacketFamily.Init)
				return;

			AddSequenceByte(ref original);
			SwapMultiples(ref original, SendMulti);
			Interleave(ref original);
			FlipMSB(ref original);
		}

		public override void Decode(ref byte[] original)
		{
			if (RecvMulti == 0 || original[1] == (byte)PacketFamily.Init)
				return;

			FlipMSB(ref original);
			Deinterleave(ref original);
			SwapMultiples(ref original, RecvMulti);
		}
	}
}
