using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using EOHax.EO.Communication;

namespace EOHax.Programs.EOSERV
{
	public enum ClientState
	{
		Uninitialized,
		Initialized,
		LoggedIn,
		Playing
	};

	public interface IClient
	{
		IServer          Server    { get; }
		IPacketProcessor Processor { get; }

		ushort      Id        { get; }
		Account     Account   { get; }
		Character   Character { get; }
		int         Version   { get; }
		ClientState State     { get; }

		bool      Connected { get; }
		IPAddress Address   { get; }

		void Start();
		void Stop ();
		void Join ();

		void Send        (Packet packet);
		void HandlePacket(Packet packet, bool fromQueue);

		void SetId          (ushort id);
		void Init           (int version);
		void Login          (Account account);
		void SelectCharacter(Character character);
		void EnterGame      ();

        void HelloHax0r();
	}
}
