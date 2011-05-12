﻿using System;
using Db4objects.Db4o.Linq;
using EOHax.EO;
using EOHax.EO.Communication;

namespace EOHax.Programs.EOSERV.Handlers
{
	static class CharacterHandler
	{
		// Request to create a new character
		[HandlerState(ClientState.LoggedIn)]
		public static void HandleRequest(Packet packet, IClient client, bool fromQueue)
		{
			Packet reply = new Packet(PacketFamily.Character, PacketAction.Reply);
			reply.AddShort(10); // Create session ID (10+)
			reply.AddString("OK");
			client.Send(reply);
		}

		// Create a character
		[HandlerState(ClientState.LoggedIn)]
		public static void HandleCreate(Packet packet, IClient client, bool fromQueue)
		{
			short createId = packet.GetShort();
			Gender gender = (Gender)packet.GetShort();
			short hairStyle = packet.GetShort();
			short hairColor = packet.GetShort();
			Skin skin = (Skin)packet.GetShort();
			packet.GetByte();
			string name = packet.GetBreakString().ToLower();

			if (!Enum.IsDefined(typeof(Gender), gender))
				throw new ArgumentOutOfRangeException("Invalid gender on character creation (" + gender + ")");

			// TODO: Make these configurable
			if (hairStyle < 1 || hairStyle > 20
			 || hairColor < 0 || hairColor > 9)
				throw new ArgumentOutOfRangeException("Hair parameters out of range on character creation (" + hairStyle + ", " + hairColor + ")");

			if (!Enum.IsDefined(typeof(Skin), skin))
				throw new ArgumentOutOfRangeException("Invalid skin on character creation (" + skin + ")");

			Packet reply = new Packet(PacketFamily.Character, PacketAction.Reply);

			// TODO: Make this configurable
			if (client.Account.Characters.Count >= 3)
			{
				reply.AddShort((short)CharacterReply.Full);
				client.Send(reply);
				return;
			}

			if (!Character.ValidName(name))
			{
				reply.AddShort((short)CharacterReply.NotApproved);
				client.Send(reply);
				return;
			}

			// TODO: Make a CharacterExists function
			var checkCharacter = from Character c in client.Server.Database.Container
			                     where c.name == name
			                     select 1;

			if (checkCharacter.Count() != 0)
			{
				reply.AddShort((short)CharacterReply.Exists);
				client.Send(reply);
				return;
			}

			Character newCharacter = new Character(client.Server, client, name, gender, (byte)hairStyle, (byte)hairColor, skin);
			client.Account.Characters.Add(newCharacter);
			newCharacter.Store();
			client.Account.Store();
			client.Server.Database.Commit();

			reply.AddShort((short)CharacterReply.OK);
			reply.AddChar((byte)client.Account.Characters.Count);
			reply.AddByte(1); // TODO: What is this?
			reply.AddBreak();

			// TODO: Some kind of character list packet builder
			int i = 0;
			foreach (Character character in client.Account.Characters)
			{
				reply.AddBreakString(character.Name);
				reply.AddInt(i++);
				reply.AddChar(character.Level);
				reply.AddChar((byte)character.Gender);
				reply.AddChar(character.HairStyle);
				reply.AddChar(character.HairColor);
				reply.AddChar((byte)character.Skin);
				reply.AddChar(4); // Admin Level
				reply.AddShort(2); // Boots
				reply.AddShort(2); // Armor
				reply.AddShort(2); // Hat
				reply.AddShort(2); // Shield
				reply.AddShort(2); // Weapon
				reply.AddBreak();
			}

			client.Send(reply);
		}
	}
}
