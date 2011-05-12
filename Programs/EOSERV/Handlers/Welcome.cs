using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EOHax.EO;
using EOHax.EO.Communication;

namespace EOHax.Programs.EOSERV.Handlers
{
	static class WelcomeHandler
	{
		// Selected a character
		[HandlerState(ClientState.LoggedIn)]
		public static void HandleRequest(Packet packet, IClient client, bool fromQueue)
		{
			int id = packet.GetInt();

			if (id < 0 || id > client.Account.Characters.Count)
				throw new ArgumentOutOfRangeException("Login character ID out of range");

			client.SelectCharacter(client.Account.Characters[id]);
			
			Packet reply = new Packet(PacketFamily.Welcome, PacketAction.Reply);
			
			reply.AddShort((short)WelcomeReply.CharacterInfo);
			reply.AddShort((short)client.Id);
			reply.AddInt(id);
			reply.AddShort((short)client.Character.Map.Data.PubId);

			reply.AddBytes(client.Character.Map.Data.RevisionID);
			reply.AddThree((int)client.Character.Map.Data.PubFileLength);

			reply.AddBytes(client.Server.ItemData.RevisionID);
			reply.AddShort((short)client.Server.ItemData.Count);

			reply.AddBytes(client.Server.NpcData.RevisionID);
			reply.AddShort((short)client.Server.NpcData.Count);

			reply.AddBytes(client.Server.SpellData.RevisionID);
			reply.AddShort((short)client.Server.SpellData.Count);

			reply.AddBytes(client.Server.ClassData.RevisionID);
			reply.AddShort((short)client.Server.ClassData.Count);

			reply.AddBreakString(client.Character.Name);
			reply.AddBreakString(client.Character.Title ?? "");
			
			reply.AddBreakString("Guild Name");
			reply.AddBreakString("Guild Rank");

			reply.AddChar(0); // Class
			reply.AddString("TAG"); // Guild tag
			reply.AddChar((byte)AdminLevel.HGM);

			reply.AddChar(client.Character.Level); // Level
			reply.AddInt(client.Character.Exp); // Exp
			reply.AddInt(client.Character.Usage); // Usage

			reply.AddShort(client.Character.Hp); // HP
			reply.AddShort(client.Character.MaxHp); // MaxHP
			reply.AddShort(client.Character.Tp); // TP
			reply.AddShort(client.Character.MaxTp); // MaxTP
			reply.AddShort(client.Character.MaxSp); // MaxSP
			reply.AddShort(client.Character.StatPoints); // StatPts
			reply.AddShort(client.Character.SkillPoints); // SkillPts
			reply.AddShort(client.Character.Karma); // Karma
			reply.AddShort(client.Character.MinDamage); // MinDam
			reply.AddShort(client.Character.MaxDamage); // MaxDam
			reply.AddShort(client.Character.Accuracy); // Accuracy
			reply.AddShort(client.Character.Evade); // Evade
			reply.AddShort(client.Character.Defence); // Armor

			reply.AddShort(client.Character.Strength); // Str
			reply.AddShort(client.Character.Wisdom); // Wis
			reply.AddShort(client.Character.Intelligence); // Int
			reply.AddShort(client.Character.Agility); // Agi
			reply.AddShort(client.Character.Constitution); // Con
			reply.AddShort(client.Character.Charisma); // Cha

			// Inventory
			reply.AddBreak();

			reply.AddChar(1); // Guild Rank

			reply.AddShort(2); // Jail map
			reply.AddShort(4); // ?
			reply.AddChar(24); // ?
			reply.AddChar(24); // ?
			reply.AddShort(10); // ?
			reply.AddShort(10); // ?
			reply.AddShort(0); // Admin command flood rate
			reply.AddShort(2); // ?
			reply.AddChar(0); // Login warning message
			reply.AddBreak();

			client.Send(reply);
		}

		// Welcome message after you login.
		[HandlerState(ClientState.LoggedIn)]
		public static void HandleMessage(Packet packet, IClient client, bool fromQueue)
		{
			Packet reply = new Packet(PacketFamily.Welcome, PacketAction.Reply);

			client.EnterGame();
			client.Character.Map.Enter(client.Character, WarpAnimation.Admin);

			reply.AddShort((short)WelcomeReply.WorldInfo);
			reply.AddBreak();

			for (int i = 0; i < 9; ++i)
			{
				reply.AddBreakString("A");
			}
			
			reply.AddChar(10); // Weight
			reply.AddChar(70); // Max Weight
			
			// Inventory
			reply.AddBreak();

			// Spells
			reply.AddBreak();

			IEnumerable<IMapObject> characters = client.Character.GetInRange<IMapObject>();
			
			reply.AddChar((byte)characters.Count());
			reply.AddBreak();

			// Characters
			// {
			foreach (Character character in characters)
			{
				character.InfoBuilder(ref reply);
				reply.AddBreak();
			}
			// }

			// NPCs
			reply.AddBreak();

			// Items
			/* ... */

			client.Send(reply);
		}

		// Client wants a file
		[HandlerState(ClientState.LoggedIn)]
		public static void HandleAgree(Packet packet, IClient client, bool fromQueue)
		{
			FileType fileType = (FileType)packet.GetChar();

			string fileName;
			InitReply replyCode;

			switch (fileType)
			{
				case FileType.Map:
					fileName = client.Character.Map.Data.GetPubFile("./tmp/");
					replyCode = InitReply.FileMap;
					break;

				case FileType.Item:
					fileName = client.Server.ItemData.GetPubFile("./tmp/");
					replyCode = InitReply.FileEIF;
					break;

				case FileType.NPC:
					fileName = client.Server.NpcData.GetPubFile("./tmp/");
					replyCode = InitReply.FileENF;
					break;

				case FileType.Spell:
					fileName = client.Server.SpellData.GetPubFile("./tmp/");
					replyCode = InitReply.FileESF;
					break;

				case FileType.Class:
					fileName = client.Server.ClassData.GetPubFile("./tmp/");
					replyCode = InitReply.FileECF;
					break;

				default:
					throw new ArgumentOutOfRangeException("Invalid file type request");
			}

			Packet reply = new Packet(PacketFamily.Init, PacketAction.Init);
			reply.AddChar((byte)replyCode);

			if (fileType != FileType.Map)
				reply.AddChar(1); // File ID

			reply.AddBytes(File.ReadAllBytes(fileName));
			
			client.Send(reply);
		}
	}
}
