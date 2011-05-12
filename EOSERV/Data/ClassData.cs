using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using EOHax.EO;
using EOHax.EO.Data;

namespace EOHax.EOSERV.Data
{
	public class ClassData : PubData<ECF.Entry>
	{
		public string Name { get; set; }

		public ClassData()
		{
			Name = String.Empty;
		}

		public override ECF.Entry GetEntry()
		{
			ECF.Entry entry = new ECF.Entry();

			entry.name = Name;

			return entry;
		}
	}

	public class ClassDataSet : PubDataSet<ClassData, ECF, ECF.Entry>
	{
		public override string PubFileNameBase { get { return "dat001.ecf"; } }

		protected override ClassData ProcessXmlEntry(XElement xml)
		{
			ClassData entry = new ClassData();

			entry.Id = xml.Attribute("id").Value;

			foreach (XElement xmlInfo in xml.Elements())
			{
				switch (xmlInfo.Name.LocalName)
				{
					case "name":
						entry.Name = xmlInfo.Value;
						break;

					//default:
						//throw new Exception("Unknown class data: " + xmlInfo.Name.LocalName);
				}					
			}

			return entry;
		}

		/// <summary>
		/// Loads class data from a directory
		/// </summary>
		/// <param name="directory">Directory to scan for class XML files</param>
		public ClassDataSet(string directory) : base(directory) { }
	}
}
