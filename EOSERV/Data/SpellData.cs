using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using EOHax.EO;
using EOHax.EO.Data;

namespace EOHax.EOSERV.Data
{
	public class SpellData : PubData<ESF.Entry>
	{
		public string Name { get; set; }

		public SpellData()
		{
			Name = String.Empty;
		}

		public override ESF.Entry GetEntry()
		{
			ESF.Entry entry = new ESF.Entry();

			return entry;
		}
	}
	
	public class SpellDataSet : PubDataSet<SpellData, ESF, ESF.Entry>
	{
		public override string PubFileNameBase { get { return "dsl001.esf"; } }

		protected override SpellData ProcessXmlEntry(XElement xml)
		{
			SpellData entry = new SpellData();

			entry.Id = xml.Attribute("id").Value;

			foreach (XElement xml_info in xml.Elements())
			{
				switch (xml_info.Name.LocalName)
				{
					case "name":
						entry.Name = xml_info.Value;
						break;

					//default:
						//throw new Exception("Unknown npc data: " + xml_info.Name.LocalName);
				}					
			}

			return entry;
		}

		/// <summary>
		/// Loads npc data from a directory
		/// </summary>
		/// <param name="directory">Directory to scan for npc XML files</param>
		public SpellDataSet(string directory) : base(directory) { }
	}
}
