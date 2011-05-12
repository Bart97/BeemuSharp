using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using EOHax.EO;
using EOHax.EO.Data;

// TODO: Completely separate this from EOHax.EO.Data so applications don't need to link to it

namespace EOHax.EOSERV.Data
{
	public abstract class PubData<TEntry>
		where TEntry : IPubEntry
	{
		public abstract TEntry GetEntry();

		public string Id { get; set; }
	}

	public abstract class PubDataSet<T, TPub, TEntry> : IEnumerable<KeyValuePair<string, T>>
		where T : PubData<TEntry>
		where TPub : Pub, new()
		where TEntry : IPubEntry, new()
	{
		private string pubFileName;
		private Dictionary<string, T> data = new Dictionary<string, T>();

		protected abstract T ProcessXmlEntry(XElement xml);

		IEnumerator<KeyValuePair<string, T>> IEnumerable<KeyValuePair<string, T>>.GetEnumerator()
		{
			return data.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return data.GetEnumerator();
		}

		public abstract string PubFileNameBase { get; }
		public long PubFileLength { get; private set; }
		public bool PubGenerated { get; private set; }
		public byte[] RevisionID { get; private set; }

		/// <summary>
		/// Creates an empty data set
		/// </summary>
		public PubDataSet() { }

		/// <summary>
		/// Loads data from a directory
		/// </summary>
		/// <param name="directory">Directory to scan for XML files</param>
		public PubDataSet(string directory)
		{
			Load(directory);
		}

		/// <summary>
		/// Index access operator for pub data sets
		/// </summary>
		/// <param name="id">Pub data index</param>
		public T this[string id]
		{
			get { return data[id]; }
			private set { data[id] = value; }
		}

		/// <summary>
		/// Number of entries in the pub file
		/// </summary>
		public int Count
		{
			get { return data.Count; }
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

				foreach (XElement xml_item in xml.Elements())
				{
					var entry = ProcessXmlEntry(xml_item);
					data.Add(entry.Id, entry);
				}
			}
		}

		/// <summary>
		/// Generates a pub file from the data and returns its filename
		/// </summary>
		/// <param name="directory">Directory to generate the file to if neccessary</param>
		public string GetPubFile(string directory)
		{
			if (PubGenerated)
				return pubFileName;

			TPub pub = new TPub();

			foreach (KeyValuePair<string, T> entry in data)
			{
				pub.data.Add(entry.Value.GetEntry());
			}

			PubGenerated = true;
			pubFileName = directory + PubFileNameBase;

			pub.Write(pubFileName);

			RevisionID = pub.revisionId;

			FileInfo fileInfo = new FileInfo(pubFileName);
			PubFileLength = fileInfo.Length;

			return pubFileName;
		}
	}
}
