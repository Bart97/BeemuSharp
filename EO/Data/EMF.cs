using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace EOHax.EO.Data
{
	/// <summary>
	/// Endless Map File reader/writer
	/// </summary>
	public class EMF
	{
		/// <summary>
		/// Map type
		/// </summary>
		public enum Type : byte
		{
			Default = 0,
			PK = 3
		}
		
		/// <summary>
		/// Timed map effect
		/// </summary>
		public enum Effect : byte
		{
			None = 0,
			HPDrain = 1,
			TPDrain = 2,
			Quake = 3
		}

		/// <summary>
		/// Special property of a tile
		/// </summary>
		public enum TileSpec : byte
		{
			Wall = 0,
			ChairDown = 1,
			ChairLeft = 2,
			ChairRight = 3,
			ChairUp = 4,
			ChairDownRight = 5,
			ChairUpLeft = 6,
			ChairAll = 7,
			JammedDoor = 8,
			Chest = 9,
			BankVault = 16,
			NPCBoundary = 17,
			MapEdge = 18,
			FakeWall = 19,
			Board1 = 20,
			Board2 = 21,
			Board3 = 22,
			Board4 = 23,
			Board5 = 24,
			Board6 = 25,
			Board7 = 26,
			Board8 = 27,
			Jukebox = 28,
			Jump = 29,
			Water = 30,
			Arena = 32,
			AmbientSource = 33,
			Spikes = 34,
			SpikesTrap = 35,
			SpikesTimed = 36
		}
		
		/// <summary>
		/// NPC spawn entry
		/// </summary>
		public struct NPC
		{
			/// <summary>
			/// X coordinate NPC spawns near
			/// </summary>
			public byte x;
			
			/// <summary>
			/// Y coordinate NPC spawns near
			/// </summary>
			public byte y;
			
			/// <summary>
			/// NPC's ENF ID
			/// </summary>
			public ushort id;
			
			/// <summary>
			/// 
			/// </summary>
			public byte spawnType;
			public ushort spawnTime;
			public byte amount;
		}
		
		public struct Unknown
		{
			public byte[] data;
		}
		
		/// <summary>
		/// Chest item spawn entry
		/// </summary>
		public struct Chest
		{
			public byte x;
			public byte y;
			public ushort key;
			public byte slot;
			public ushort item;
			public ushort time;
			public uint amount;
		}

		/// <summary>
		/// Tile special property entry
		/// </summary>
		public struct Tile
		{
			public byte x;
			public TileSpec spec;
		}

		/// <summary>
		/// Tile row
		/// </summary>
		public struct TileRow
		{
			public byte y;
			public List<Tile> tiles;
		}

		/// <summary>
		/// Tile warp entry
		/// </summary>
		public struct Warp
		{
			public byte x;
			public ushort warpMap;
			public byte warpX;
			public byte warpY;
			public byte levelRequirement;
			public ushort door;
		}

		/// <summary>
		/// Warp row
		/// </summary>
		public struct WarpRow
		{
			public byte y;
			public List<Warp> tiles;
		}
		
		/// <summary>
		/// Tile graphic entry
		/// </summary>
		public class GFX
		{
			public byte x;
			public ushort tile;
		}
		
		/// <summary>
		/// Graphic row
		/// </summary>
		public struct GFXRow
		{
			public byte y;
			public List<GFX> tiles;
		}
		
		/// <summary>
		/// Sign entry
		/// </summary>
		public struct Sign
		{
			public byte x;
			public byte y;
			public string title;
			public string message;
		}

		/// <summary>
		/// Special key value meaning the warp has no door
		/// </summary>
		public const ushort NoDoor = 0;
		
		/// <summary>
		/// Special key value meaning there is an unlocked door
		/// </summary>
		public const ushort Door = 1;
		
		/// <summary>
		/// Number of graphic layers a map has
		/// </summary>
		public const int GFXLayers = 9;

		/// <summary>
		/// A unique number for the version of the file.
		/// </summary>
		/// <remarks>
		/// EOHax defacto standard is to set this to the CRC32 of the entire file output with this field to 0x00000000
		/// 0x00 bytes are represented in 0x01 to avoid problems with file transfer
		/// </remarks>
		public byte[] revisionId = new byte[4];

		/// <summary>
		/// Name of the map
		/// </summary>
		public string name = String.Empty;
		
		/// <summary>
		/// Map type
		/// </summary>
		public Type type;

		/// <summary>
		/// Timed map effect
		/// </summary>
		public Effect effect;
		
		/// <summary>
		/// MFX ID to play
		/// </summary>
		public byte music;
		
		/// <summary>
		/// Background music playing instructions
		/// </summary>
		public byte musicExtra; // TODO: Find these out
		
		/// <summary>
		/// SFX ID that ambient noise tiles play
		/// </summary>
		public ushort ambientNoise;
		
		/// <summary>
		/// Width of the map in tiles
		/// </summary>
		public byte width;
		
		/// <summary>
		/// Height of the map in tiles
		/// </summary>
		public byte height;
		
		/// <summary>
		/// Tile graphic ID that fills tiles with no graphic set
		/// </summary>
		public ushort fillTile;
		
		/// <summary>
		/// Whether a player can view the mini-map
		/// </summary>
		public byte mapAvailable;
		
		/// <summary>
		/// Whether a player can teleport out of the map
		/// </summary>
		public byte canScroll;
		
		/// <summary>
		/// X coordinate a player is moved to on logging in to the map.
		/// If relogX and relogY are both 0 the player is not moved.
		/// </summary>
		public byte relogX;
		
		/// <summary>
		/// Y coordinate a player is move to on logging in to the map.
		/// If relogX and relogY are both 0 the player is not moved.
		/// </summary>
		public byte relogY;

		public byte unknown;

		/// <summary>
		/// NPC spawn entry list
		/// </summary>
		public List<NPC> npcs = new List<NPC>();
		
		public List<Unknown> unknowns = new List<Unknown>();
		
		/// <summary>
		/// Chest item spawn entry list
		/// </summary>
		public List<Chest> chests = new List<Chest>();
		
		/// <summary>
		/// List of tile property rows
		/// </summary>
		public List<TileRow> tileRows = new List<TileRow>();
		
		/// <summary>
		/// List of tile warp entry rows
		/// </summary>
		public List<WarpRow> warprows = new List<WarpRow>();
		
		/// <summary>
		/// Tile graphic information map
		/// </summary>
		public List<GFXRow>[] gfxRows = new List<GFXRow>[GFXLayers];

		/// <summary>
		/// Sign entry list
		/// </summary>
		public List<Sign> signs = new List<Sign>();

		/// <summary>
		/// Encodes a string to EMF format
		/// </summary>
		/// <param name="s">String to be encoded</param>
		public byte[] EncodeString(string s)
		{
			return ASCIIEncoding.ASCII.GetBytes(DecodeString(ASCIIEncoding.ASCII.GetBytes(s)));
		}
		
		/// <summary>
		/// Decodes an EMF format string
		/// </summary>
		/// <param name="chars">EMF encoded string</param>
		public string DecodeString(byte[] chars)
		{
			Array.Reverse(chars);

			bool flippy = (chars.Length % 2) == 1;

			for (int i = 0; i < chars.Length; ++i)
			{
				byte c = chars[i];

				if (flippy)
				{
					if (c >= 0x22 && c <= 0x4F)
						c = (byte)(0x71 - c);
					else if (c >= 0x50 && c <= 0x7E)
						c = (byte)(0xCD - c);
				}
				else
				{
					if (c >= 0x22 && c <= 0x7E)
						c = (byte)(0x9F - c);
				}

				chars[i] = c;
				flippy = !flippy;
			}

			return ASCIIEncoding.ASCII.GetString(chars);
		}

		/// <summary>
		/// Create an empty EMF file
		/// </summary>
		public EMF()
		{
			for (int layer = 0; layer < GFXLayers; ++layer)
			{
				gfxRows[layer] = new List<GFXRow>();
			}
		}

		/// <summary>
		/// Load from an EMF file
		/// </summary>
		/// <param name="fileName">File to read the EMF data from</param>
		public EMF(string fileName)
		{
			Read(fileName);
		}

		/// <summary>
		/// Read an EMF file
		/// </summary>
		/// <param name="fileName">File to read the EMF data from</param>
		public void Read(string fileName)
		{
			int outersize;
			int innersize;

			using (EOFile file = new EOFile(File.Open(fileName, FileMode.Open, FileAccess.Read)))
			{
				if (file.GetFixedString(3) != "EMF")
					throw new Exception("Corrupt or not an EMF file");

				revisionId = file.GetBytes(4);
				byte[] rawname = file.GetBytes(24);

				for (int i = 0; i < 24; ++i)
				{
					if (rawname[i] == 0xFF)
					{
						Array.Resize(ref rawname, i);
						break;
					}
				}

				name = DecodeString(rawname);

				type = (Type)file.GetChar();
				effect = (Effect)file.GetChar();
				music = file.GetChar();
				musicExtra = file.GetChar();
				ambientNoise = (ushort)file.GetShort();
				width = file.GetChar();
				height = file.GetChar();
				fillTile = (ushort)file.GetShort();
				mapAvailable = file.GetChar();
				canScroll = file.GetChar();
				relogX = file.GetChar();
				relogY = file.GetChar();
				unknown = file.GetChar();

				outersize = file.GetChar();

				for (int i = 0; i < outersize; ++i)
				{
					npcs.Add(new NPC()
					{
						x = file.GetChar(),
						y = file.GetChar(),
						id = (ushort)file.GetShort(),
						spawnType = file.GetChar(),
						spawnTime = (ushort)file.GetShort(),
						amount = file.GetChar()
					});
				}

				outersize = file.GetChar();

				for (int i = 0; i < outersize; ++i)
				{
					unknowns.Add(new Unknown()
					{
						data = file.GetBytes(5)
					});
				}

				outersize = file.GetChar();

				for (int i = 0; i < outersize; ++i)
				{
					chests.Add(new Chest()
					{
						x = file.GetChar(),
						y = file.GetChar(),
						key = (ushort)file.GetShort(),
						slot = file.GetChar(),
						item = (ushort)file.GetShort(),
						time = (ushort)file.GetShort(),
						amount = (uint)file.GetThree()
					});
				}

				outersize = file.GetChar();

				for (int i = 0; i < outersize; ++i)
				{
					byte y = file.GetChar();
					innersize = file.GetChar();
					
					TileRow row = new TileRow()
					{
						y = y,
						tiles = new List<Tile>(innersize)
					};

					for (int ii = 0; ii < innersize; ++ii)
					{
						row.tiles.Add(new Tile()
						{
							x = file.GetChar(),
							spec = (TileSpec)file.GetChar()
						});
					}

					tileRows.Add(row);
				}

				outersize = file.GetChar();

				for (int i = 0; i < outersize; ++i)
				{
					byte y = file.GetChar();
					innersize = file.GetChar();

					WarpRow row = new WarpRow()
					{
						y = y,
						tiles = new List<Warp>(innersize)
					};

					for (int ii = 0; ii < innersize; ++ii)
					{
						row.tiles.Add(new Warp()
						{
							x = file.GetChar(),
							warpMap = (ushort)file.GetShort(),
							warpX = file.GetChar(),
							warpY = file.GetChar(),
							levelRequirement = file.GetChar(),
							door = (ushort)file.GetShort()
						});
					}

					warprows.Add(row);
				}
			
				for (int layer = 0; layer < GFXLayers; ++layer)
				{
					outersize = file.GetChar();
					gfxRows[layer] = new List<GFXRow>(outersize);

					for (int i = 0; i < outersize; ++i)
					{
						byte y = file.GetChar();
						innersize = file.GetChar();

						GFXRow row = new GFXRow()
						{
							y = y,
							tiles = new List<GFX>(innersize)
						};

						row.tiles = new List<GFX>(innersize);

						for (int ii = 0; ii < innersize; ++ii)
						{
							row.tiles.Add(new GFX()
							{
								x = file.GetChar(),
								tile = (ushort)file.GetShort()
							});
						}

						gfxRows[layer].Add(row);
					}
				}

				outersize = file.GetChar();

				for (int i = 0; i < outersize; ++i)
				{
					Sign sign = new Sign();

					sign.x = file.GetChar();
					sign.y = file.GetChar();
					int msgLength = file.GetShort() - 1;
					string data = DecodeString(file.GetBytes(msgLength));
					int titleLength = file.GetChar();
					sign.title = data.Substring(0, titleLength);
					sign.message = data.Substring(titleLength);

					signs.Add(sign);
				}
			}
		}

		public void Write(string fileName)
		{
			using (EOFile file = new EOFile(File.Open(fileName, FileMode.Create, FileAccess.Write)))
			{
				file.AddString("EMF");
				file.AddInt(0);

				byte[] padName = new byte[24];

				for (int i = 0; i < 24; ++i)
					padName[i] = 0xFF;

				byte[] encName = EncodeString(name);

				Array.Resize(ref encName, Math.Min(24, encName.Length));

				Array.Copy(encName, 0, padName, 24 - encName.Length, encName.Length);

				file.AddBytes(padName);
				file.AddChar((byte)type);
				file.AddChar((byte)effect);
				file.AddChar(music);
				file.AddChar(musicExtra);
				file.AddShort((short)ambientNoise);
				file.AddChar(width);
				file.AddChar(height);
				file.AddShort((short)fillTile);
				file.AddChar(mapAvailable);
				file.AddChar(canScroll);
				file.AddChar(relogX);
				file.AddChar(relogY);
				file.AddChar(unknown);

				file.AddChar((byte)npcs.Count);

				foreach (NPC npc in npcs)
				{
					file.AddChar(npc.x);
					file.AddChar(npc.y);
					file.AddShort((short)npc.id);
					file.AddChar(npc.spawnType);
					file.AddShort((short)npc.spawnTime);
					file.AddChar(npc.amount);
				}

				file.AddChar((byte)unknowns.Count);

				foreach (Unknown unknown_ in unknowns)
				{
					file.AddBytes(unknown_.data);
				}

				file.AddChar((byte)chests.Count);

				foreach (Chest chest in chests)
				{
					file.AddChar(chest.x);
					file.AddChar(chest.y);
					file.AddShort((short)chest.key);
					file.AddChar(chest.slot);
					file.AddShort((short)chest.item);
					file.AddShort((short)chest.time);
					file.AddThree((int)chest.amount);
				}

				file.AddChar((byte)tileRows.Count);

				foreach (TileRow row in tileRows)
				{
					file.AddChar(row.y);
					file.AddChar((byte)row.tiles.Count);

					foreach (Tile tile in row.tiles)
					{
						file.AddChar(tile.x);
						file.AddChar((byte)tile.spec);
					}
				}

				file.AddChar((byte)warprows.Count);

				foreach (WarpRow row in warprows)
				{
					file.AddChar(row.y);
					file.AddChar((byte)row.tiles.Count);

					foreach (Warp warp in row.tiles)
					{
						file.AddChar(warp.x);
						file.AddShort((short)warp.warpMap);
						file.AddChar(warp.warpX);
						file.AddChar(warp.warpY);
						file.AddChar(warp.levelRequirement);
						file.AddShort((short)warp.door);
					}
				}

				for (int layer = 0; layer < GFXLayers; ++layer)
				{
					file.AddChar((byte)gfxRows[layer].Count);

					foreach (GFXRow row in gfxRows[layer])
					{
						file.AddChar(row.y);
						file.AddChar((byte)row.tiles.Count);

						foreach (GFX gfx in row.tiles)
						{
							file.AddChar(gfx.x);
							file.AddShort((short)gfx.tile);
						}
					}
				}

				file.AddChar((byte)signs.Count);

				foreach (Sign sign in signs)
				{
					file.AddChar(sign.x);
					file.AddChar(sign.y);
					file.AddShort((short)(sign.title.Length + sign.message.Length + 1));
					file.AddBytes(EncodeString(sign.title + sign.message));
					file.AddChar((byte)sign.title.Length);
				}

				revisionId = file.WriteHash(3);
			}
		}
	}
}
