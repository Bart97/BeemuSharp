using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Linq;
using EOHax.EO;
using EOHax.EO.Data;

namespace EOHax.EOSERV.Data
{
	public struct MapWarp
	{
		private ushort door;

		public string mapId;
		public byte x;
		public byte y;
		public byte levelRequirement;

		public bool Door
		{
			get { return door > 0; }

			set
			{
				if (value)
					door = (door > 0) ? door : (ushort)1;
				else
					door = 0;
			}
		}

		public bool Locked
		{
			get { return door > 1; }

			set
			{
				if (value)
					door = (door > 1) ? door : (ushort)2;
				else
					door = (door > 0) ? (ushort)1 : (ushort)0;
			}
		}

		public ushort Key
		{
			get { return (door > 1) ? (ushort)(door - 1) : (ushort)0; }

			set
			{
				door = value;
			}
		}
	}

	public enum MapTileSpec
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
		Board = 20,
		Jukebox = 28,
		Jump = 29,
		Water = 30,
		Arena = 32,
		AmbientSource = 33,
		Spikes = 34,
		SpikesTrap = 35,
		SpikesTimed = 36
	}

	public class MapData
	{
		private string pubFileName;

		public long PubFileLength { get; private set; }
		public bool PubGenerated { get; private set; }

		public string Id { get; private set; }
		public string Name { get; private set; }
		public ushort PubId { get; private set; }

		private EMF emf;

		private MapTileSpec ConvertTileSpec(EMF.TileSpec spec)
		{
			if (spec >= EMF.TileSpec.Board1 && spec <= EMF.TileSpec.Board8)
				return MapTileSpec.Board;

			return (MapTileSpec)spec;
		}

		public struct Tile
		{
			public MapTileSpec? spec;
			public MapWarp? warp;
		}

		public Tile[,] Tiles { get; private set; }

		private void ResetTiles()
		{
			Tile[,] newTiles = new Tile[Width, Height];

			for (byte y = 0; y < Width; ++y)
			{
				for (byte x = 0; x < Height; ++x)
				{
					newTiles[y, x] = Tiles[y, x];
				}
			}

			Tiles = newTiles;
		}

		public byte Width
		{
			get { return emf.width; }
		}

		public byte Height
		{
			get { return emf.height; }
		}

		public void Resize(byte width, byte height)
		{
			emf.width = width;
			emf.height = height;
			ResetTiles();
		}

		private MapData(string id)
		{
			SetId(id);
		}

		public MapData(string id, EMF emf) : this(id)
		{
			SetEMF(emf);
		}

		private void SetId(string id)
		{
			Id = id;

			using (CRC32 hash = new CRC32())
			{
				byte[] hashResult = hash.ComputeHash(ASCIIEncoding.ASCII.GetBytes(Id));
				PubId = (ushort)(hashResult[0] * 256 + hashResult[1]);
			}
		}

		private void SetEMF(EMF emf)
		{
			this.emf = emf;

			Tiles = new Tile[emf.height, emf.width];

			foreach (EMF.TileRow row in emf.tileRows)
			{
				foreach (EMF.Tile tile in row.tiles)
				{
					Tiles[row.y, tile.x].spec = ConvertTileSpec(tile.spec);
				}
			}

			foreach (EMF.WarpRow row in emf.warprows)
			{
				foreach (EMF.Warp tile in row.tiles)
				{
					MapWarp warp = new MapWarp()
					{
						Door = (tile.door > 0),
						Locked = (tile.door > 1)
					};

					if (warp.Locked)
						warp.Key = (ushort)(tile.door - 1);

					Tiles[row.y, tile.x].warp = warp;
				}
			}
		}

		public byte[] RevisionID { get; private set; }

		public static MapData ProcessXmlEntry(XElement xml, string directory)
		{
			MapData entry = new MapData(xml.Attribute("id").Value);

			foreach (XElement xmlInfo in xml.Elements())
			{
				switch (xmlInfo.Name.LocalName)
				{
					case "name":
						entry.Name = xmlInfo.Value;
						break;

					case "emf":
						entry.SetEMF(new EMF(Path.Combine(directory, xmlInfo.Value)));
						break;

					/*default:
						throw new Exception("Unknown map data: " + xmlInfo.Name.LocalName);*/
				}
			}

			return entry;
		}

		/// <summary>
		/// Generates a pub file from the data and returns its filename
		/// </summary>
		/// <param name="directory">Directory to generate the file to if neccessary</param>
		public string GetPubFile(string directory)
		{
			if (PubGenerated)
				return pubFileName;

			string fileNameBase = String.Format("{0:00000}.emf", PubId);

			EMF emf = new EMF()
			{
				type         = this.emf.type,
				music        = this.emf.music,
				width        = this.emf.width,
				height       = this.emf.height,
				fillTile     = this.emf.fillTile,
				mapAvailable = this.emf.mapAvailable,
				canScroll    = this.emf.canScroll,
				gfxRows      = this.emf.gfxRows,
				signs        = this.emf.signs
			};

			PubGenerated = true;
			pubFileName = directory + fileNameBase;

			emf.Write(pubFileName);

			RevisionID = emf.revisionId;

			FileInfo info = new FileInfo(pubFileName);
			PubFileLength = info.Length;

			return pubFileName;
		}
	}

	public class MapDataSet : IEnumerable<KeyValuePair<string, MapData>>
	{
		private Dictionary<string, MapData> data = new Dictionary<string, MapData>();

		IEnumerator<KeyValuePair<string, MapData>> IEnumerable<KeyValuePair<string, MapData>>.GetEnumerator()
		{
			return data.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return data.GetEnumerator();
		}

		/// <summary>
		/// Creates an empty data set
		/// </summary>
		public MapDataSet()
		{

		}

		/// <summary>
		/// Loads data from a directory
		/// </summary>
		/// <param name="directory">Directory to scan for XML files</param>
		public MapDataSet(string directory) : this()
		{
			Load(directory);
		}

		/// <summary>
		/// Loads data from a directory
		/// </summary>
		/// <param name="directory">Directory to scan for XML files</param>
		public void Load(string directory)
		{
			foreach (string filename in Directory.EnumerateFiles(directory, "*.xml", SearchOption.AllDirectories))
			{
				XElement xml = XElement.Load(filename);

				foreach (XElement xmlItem in xml.Elements())
				{
					var entry = MapData.ProcessXmlEntry(xmlItem, Path.GetDirectoryName(filename));
					data.Add(entry.Id, entry);
				}
			}
		}
	}
}
