using System;
using Server.Items;
using Server.Network;
using Server.Gumps;

namespace Server.Events.Duel
{
	[Flipable(0x27A0, 0x27EB)]
	public class DuelBelt : BaseWaist
	{
		private Mobile m_MobileLock;
		private string m_BeltName;

		[CommandProperty(AccessLevel.GameMaster)]
		public Mobile MobileLock
		{
			get { return m_MobileLock; }
			set { m_MobileLock = value; }
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public string BeltName
		{
			get { return m_BeltName; }
			set { m_BeltName = value; }
		}

		[Constructable]
		public DuelBelt()
			: base(0x27A0)
		{
			Weight = 1.0;
			Name = "Duel Belt";
			LootType = LootType.Blessed;
		}

		public override void GetProperties(ObjectPropertyList list)
		{
			list.Add(1070722, m_BeltName);
			base.GetProperties(list);
		}

		public override bool OnEquip(Mobile from)
		{
			if (from == m_MobileLock)
				return true;

			from.SendMessage("Only the champion can wear this belt!");
			return false;
		}

		public override void OnDoubleClick(Mobile from)
		{
			if (IsChildOf(from.Backpack))
			{
				from.CloseGump(typeof(DuelBeltGump));
				from.SendGump(new DuelBeltGump(this));
			}
			else
			{
				from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
			}
		}

		public DuelBelt(Serial serial) : base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write((int)0); // version
			writer.Write(m_MobileLock);
			writer.Write(m_BeltName);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();
			m_MobileLock = reader.ReadMobile();
			m_BeltName = reader.ReadString();
		}
	}

	public class DuelBeltGump : Gump
	{
		private DuelBelt m_Belt;

		private class BeltGumpEntry
		{
			public int m_hue;
			public string m_name;
			public string m_htmlhue;
			public BeltGumpEntry(int hue, string name, string htmlhue)
			{
				m_hue = hue;
				m_name = name;
				m_htmlhue = htmlhue;
			}
		}

		private BeltGumpEntry[] m_BeltGumpHues = new BeltGumpEntry[]
		{
			new BeltGumpEntry( 0x501,	"Paragon Gold",			 "#ffd600" ), //
			new BeltGumpEntry( 0x486,	"Violet Courage Purple",	"#94088c" ), //
			new BeltGumpEntry( 0x4F2,	"Invulnerability Blue",	 "#00e7ff" ), //
			new BeltGumpEntry( 0x47E,	"Luna White",			   "#ffffff" ), //
			new BeltGumpEntry( 1167,	"Dryad Green",			  "#d6efd6" ), //
			new BeltGumpEntry( 0x455,	"Shadow Dancer Black",	  "#4a4a5a" ), //
			new BeltGumpEntry( 0x21,	"Berserker Red",			"#de0031" ), //
			new BeltGumpEntry( 0x58C,	"Nox Green",				"#8cc6a5" ), //
			new BeltGumpEntry( 1645,	"Rum Red",				  "#c63131" ), //
			new BeltGumpEntry( 0x489,	"Fire Orange",			  "#e7de00" )  //
		};

		public DuelBeltGump(DuelBelt belt)
			: base(0, 0)
		{
			m_Belt = belt;

			AddPage(0);
			AddBackground(150, 60, 350, 358, 2600);
			AddHtml(230, 75, 200, 20, "Belt Color Selection Menu", false, false);
			AddHtml(235, 380, 300, 20, "Dye my belt this color!", false, false);
			AddButton(200, 380, 0xFA5, 0xFA7, 1, GumpButtonType.Reply, 0);		// dye belt

			for (int i = 0; i < m_BeltGumpHues.Length; ++i)
			{
				BeltGumpEntry entry = m_BeltGumpHues[i];
				//AddLabel(275, 115 + (i * 18), entry.m_hue, "*****");
				AddHtml(275, 115 + (i * 23), 150, 20, String.Format("<BASEFONT COLOR={0}>{1}</BASEFONT>", entry.m_htmlhue, entry.m_name), false, false);
				AddRadio(235, 115 + (i * 23), 210, 211, false, i + 10);
			}
		}

		public override void OnResponse(NetState from, RelayInfo info)
		{
			if (m_Belt.Deleted)
				return;

			Mobile m = from.Mobile;

			if (!m_Belt.IsChildOf(m.Backpack))
			{
				m.SendLocalizedMessage(1042010); //You must have the object in your backpack to use it.
				return;
			}

			if (info.ButtonID != 0 && info.Switches.Length > 0)
			{
				int entryIndex = info.Switches[0] - 10;

				if (entryIndex >= 0 && entryIndex < m_BeltGumpHues.Length)
				{
					BeltGumpEntry entry = m_BeltGumpHues[entryIndex];
					m_Belt.Hue = entry.m_hue;
				}
			}
		}
	}
}