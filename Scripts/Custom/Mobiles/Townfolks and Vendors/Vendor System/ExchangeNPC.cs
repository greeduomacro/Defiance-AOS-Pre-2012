using System;
using System.Collections.Generic;
using Server.Gumps;
using Server.ContextMenus;
using Server.Items;

namespace Server.Exchange
{
	public class ExchangeNPC : Mobile
	{
		public ExchangeCategory ECategory;
		private Config.NPCCategories m_Category;
		private ItemHues m_ItemHue;

		[CommandProperty(AccessLevel.GameMaster)]
		public Config.NPCCategories Category
		{
			get { return m_Category; }
			set
			{
				int nr = (int)value;
				if (nr >= 0 && nr < ExchangeSystem.CategoryList.Count)
				{
					m_Category = value;
					ECategory = ExchangeSystem.CategoryList[nr];
				}
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public ItemHues ItemHue
		{
			get { return m_ItemHue; }
			set { m_ItemHue = value; SetAllItemHues((int)value); }
		}

		[Constructable]
		public ExchangeNPC()
			: base()
		{
			Category = Config.NPCCategories.BlackSmith;
			m_ItemHue = ItemHues.Turquoise;
			InitStats(100, 100, 25);

			Title = "the trader";
			Hue = Utility.RandomSkinHue();
			Body = 0x190;
			Hue = 33770;
			Name = NameList.RandomName("male");
			HairItemID = 8252;
			Blessed = true;
			HairHue = 2125;
			FacialHairItemID = 8267;
			FacialHairHue = 2125;
			Item sash = new BodySash();
			sash.Hue = 1195;
			sash.Name = "I can make thee rich!";
			sash.Layer = Layer.Talisman;
			AddItem(sash);

			Equip(new Tunic());
			Equip(new FurBoots());
			Equip(new FurCape());
			Equip(new Kilt());
			Equip(new LongPants());
			Equip(new Necklace());
			Equip(new GoldRing());
			Equip(new SilverBracelet());
			Direction = Direction.Down;
		}

		public enum ItemHues
		{
			Green,
			GreenBlue,
			DarkTurquoise,
			AquaGreen,
			Turquoise,
			Purple,
			DarkPurple,
			BloodRed,
			NeonBlue,
			NeonPurple,
			NeonPink,
			NeonYellow,
			NeonGreen,
			RedBlue,
			DarkBlue,
			CharCoal,
			Fire,
			IceGreen,
			IceBlue,
			IceWhite
		}

		private static int[] m_Hues = new int[]
			{
				 0x483, 0x48C, 0x488, 0x48A,
				 0x495, 0x48B, 0x486, 0x485,
				 0x48D, 0x490, 0x48E, 0x491,
				 0x48F, 0x494, 0x484, 0x497,
				 0x489, 0x47F, 0x482, 0x47E
			};

		private void Equip(Item item)
		{
			AddItem(item);
			if(item is BaseClothing)
				((BaseClothing)item).CustomPropName = "property of WBE Traders & co";
			item.Movable = false;
			item.Hue = 2125;
		}

		public void SetAllItemHues(int index)
		{
			foreach (Item item in Items)
				if(!(item is BodySash))
					item.Hue = m_Hues[index];

			HairHue = m_Hues[index];
			FacialHairHue = m_Hues[index];
		}

		public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
		{
			base.GetContextMenuEntries(from, list);

			if (from.Alive)
				list.Add(new TalkEntry(from, this));
		}

		public ExchangeNPC(Serial serial)
			: base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(0); // version;
			writer.Write(ECategory.ID);

		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();
			int id = reader.ReadInt();
			foreach (ExchangeCategory ec in ExchangeSystem.CategoryList)
			{
				if (ec.ID == id)
				{
					ECategory = ec;
					m_Category = (Config.NPCCategories)id;
					break;
				}
			}

			if (ECategory == null)
				Delete();
		}

		private class TalkEntry : ContextMenuEntry
		{
			private Mobile m_From;
			private ExchangeNPC m_NPC;

			public TalkEntry(Mobile from, ExchangeNPC npc)
				: base(6146, 12)
			{
				m_From = from;
				m_NPC = npc;
			}

			public override void OnClick()
			{
				if (!m_From.HasGump(typeof(EntryGump)) && !m_From.HasGump(typeof(BuyGump)) && !m_From.HasGump(typeof(SellGump)) && !m_From.HasGump(typeof(ViewMyBidsGump)) && !m_From.HasGump(typeof(VerifyActionGump)))
					m_From.SendGump(new EntryGump(m_NPC.ECategory));
			}
		}
	}
}