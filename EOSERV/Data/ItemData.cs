using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using EOHax.EO;
using EOHax.EO.Data;

namespace EOHax.EOSERV.Data
{
	public enum ItemEquipSlot : byte
	{
		None,
		Armor,
		Weapon,
		Shield,
		Wings,
		Arrows,
		Hat,
		Boots,
		Gloves,
		Charm,
		Belt,
		Necklace,
		Ring,
		Armlet,
		Bracer
	}

	public class ItemData : PubData<EIF.Entry>
	{
		
		public string Name { get; set; }
		public ItemEquipSlot EquipSlot { get; set; }
		public short Graphic { get; set; }
		public short DollGraphic { get; set; }

		public Gender Gender { get; set; }
		public EIF.Size Shape { get; set; }
		public byte Weight { get; set; }

		/// <summary>
		/// Generates the information a client needs to know about the items
		/// </summary>
		public override EIF.Entry GetEntry()
		{
			EIF.Entry entry = new EIF.Entry();

			entry.name = Name;
			entry.graphic = Graphic;
			entry.size = Shape;

			entry.type = EIF.Type.Heal;

			switch (EquipSlot)
			{
				case ItemEquipSlot.Armor:    entry.type = EIF.Type.Armor;    break;
				case ItemEquipSlot.Weapon:   entry.type = EIF.Type.Weapon;   break;
				case ItemEquipSlot.Shield:   entry.type = EIF.Type.Shield;   break;
				case ItemEquipSlot.Hat:      entry.type = EIF.Type.Hat;      break;
				case ItemEquipSlot.Boots:    entry.type = EIF.Type.Boots;    break;
				case ItemEquipSlot.Gloves:   entry.type = EIF.Type.Gloves;   break;
				case ItemEquipSlot.Charm:    entry.type = EIF.Type.Charm;    break;
				case ItemEquipSlot.Belt:     entry.type = EIF.Type.Belt;     break;
				case ItemEquipSlot.Necklace: entry.type = EIF.Type.Necklace; break;
				case ItemEquipSlot.Ring:     entry.type = EIF.Type.Ring;     break;
				case ItemEquipSlot.Armlet:   entry.type = EIF.Type.Armlet;   break;
				case ItemEquipSlot.Bracer:   entry.type = EIF.Type.Bracer;   break;
				case ItemEquipSlot.Wings:    entry.type = EIF.Type.Shield; entry.subtype = EIF.SubType.Wings;  break;
				case ItemEquipSlot.Arrows:   entry.type = EIF.Type.Shield; entry.subtype = EIF.SubType.Arrows; break;
			}

			entry.weight = Weight;

			return entry;
		}
	}

	public class ItemDataSet : PubDataSet<ItemData, EIF, EIF.Entry>
	{
		public override string PubFileNameBase { get { return "dat001.eif"; } }

		protected override ItemData ProcessXmlEntry(XElement xml)
		{
			ItemData entry = new ItemData();

			entry.Id = xml.Attribute("id").Value;

			foreach (XElement xmlInfo in xml.Elements())
			{
				switch (xmlInfo.Name.LocalName)
				{
					case "name":
						entry.Name = xmlInfo.Value;
						break;

					case "shape":
						entry.Shape = (EIF.Size)Enum.Parse(typeof(EIF.Size), "Size" + xmlInfo.Value, true);
						break;

					case "gender":
						entry.Gender = (Gender)Enum.Parse(typeof(Gender), xmlInfo.Value, true);
						break;

					case "equipslot":
						entry.EquipSlot = (ItemEquipSlot)Enum.Parse(typeof(ItemEquipSlot), xmlInfo.Value, true);
						break;

					case "graphic":
						entry.Graphic = short.Parse(xmlInfo.Value);
						break;

					case "dollgraphic":
						entry.DollGraphic = short.Parse(xmlInfo.Value);
						break;

					case "weight":
						entry.Weight = byte.Parse(xmlInfo.Value);
						break;

					/*default:
						throw new Exception("Unknown item data: " + xmlInfo.Name.LocalName);*/
				}
			}

			return entry;
		}

		/// <summary>
		/// Loads item data from a directory
		/// </summary>
		/// <param name="directory">Directory to scan for item XML files</param>
		public ItemDataSet(string directory) : base(directory) { }
	}
}
