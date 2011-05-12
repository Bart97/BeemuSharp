using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using EOHax.EO;
using EOHax.EO.Data;

namespace EOHax.EOSERV.Data
{
	public class NpcData : PubData<ENF.Entry>
	{
		public override ENF.Entry GetEntry()
		{
			ENF.Entry entry = new ENF.Entry();

			return entry;
		}
	}

	public class NpcDataSet : PubDataSet<NpcData, ENF, ENF.Entry>
	{
		public override string PubFileNameBase { get { return "dtn001.enf"; } }

		protected override NpcData ProcessXmlEntry(XElement xml)
		{
			NpcData entry = new NpcData();

			entry.Id = xml.Attribute("id").Value;

			foreach (XElement xml_iteminfo in xml.Elements())
			{
				switch (xml_iteminfo.Name.LocalName)
				{
					//default:
						//throw new Exception("Unknown npc data: " + xml_iteminfo.Name.LocalName);
				}					
			}

			return entry;
		}

		/// <summary>
		/// Loads npc data from a directory
		/// </summary>
		/// <param name="directory">Directory to scan for npc XML files</param>
		public NpcDataSet(string directory) : base(directory) { }
	}
}
