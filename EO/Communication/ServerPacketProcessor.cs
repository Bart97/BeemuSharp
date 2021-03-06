using System;

namespace EOHax.EO.Communication
{
	/// <summary>
	/// Packet processor for the server side
	/// </summary>
	public class ServerPacketProcessor : PacketProcessor
	{
		public override void Encode(ref byte[] original)
		{
			if (SendMulti == 0 || original[1] == (byte)PacketFamily.Init)
				return;

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
