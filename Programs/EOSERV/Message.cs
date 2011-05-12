using System;

namespace EOHax.Programs.EOSERV
{
	public interface IMessageSource
	{
		void SendMsg(IMessageTarget target, Message message);
	}

	public interface IMessageTarget
	{
		void RecieveMsg(IMessageSource source, Message message);
	}

	public class Message
	{
		public IMessageSource Source { get; private set; }
		public string MessageString { get; private set; }

		public Message(IMessageSource source, string messageString)
		{
			Source = source;
			MessageString = messageString;
			byte[] x = new byte[10];
		}
	}

	public class MessageLocal : Message
	{
		public MessageLocal(IMessageSource source, string message) : base(source, message) { }
	}

	public class MessageGlobal : Message
	{
		public MessageGlobal(IMessageSource source, string message) : base(source, message) { }
	}

	public class MessageParty : Message
	{
		public MessageParty(IMessageSource source, string message) : base(source, message) { }
	}

	public class MessageGuild : Message
	{
		public MessageGuild(IMessageSource source, string message) : base(source, message) { }
	}

	public class MessageAdmin : Message
	{
		public MessageAdmin(IMessageSource source, string message) : base(source, message) { }
	}

	public class MessageAnnounce : Message
	{
		public MessageAnnounce(IMessageSource source, string message) : base(source, message) { } 
	}
}
