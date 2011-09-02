using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EOHax.EO.Communication;

namespace EOHax.Programs.EOSERV
{
	public class GlobalChat : IMessageTarget
	{
		public void RecieveMsg(IMessageSource source, Message message)
		{
			if (typeof(Character).IsInstanceOfType(source))
			{
				Packet packet = new Packet(PacketFamily.Talk, PacketAction.Message);
				packet.AddBreakString(((Character)source).Name);
				packet.AddBreakString(message.MessageString);
				foreach (Character character in ((Character)source).Server.Characters)
				{
					if (character != (Character)source)
						character.Client.Send(packet);
				}
			}
		}
	}
}
