using Server.Gumps;
using Server.Items;
using Server.Network;
using Server.Mobiles;
using System;
using System.IO;
using System.Collections.Generic;

namespace Server.Engines.RewardSystem
{
	public class LLItemGump : AdvGump
	{
		private List<ItemInfo> m_List;

		public LLItemGump(List<ItemInfo> list)
			: base(60, 60)
		{
			m_List = list;

			Closable = true;
			Disposable = true;
			Dragable = true;
			Resizable = false;
			AddBackground(50, 30, 350, 467, 9260);
			AddBackground(125, 30, 200, 55, 9260);
			AddImageTiled(81, 30, 271, 11, 9261);
			AddHtml(50, 50, 350, 16, Colorize(Center("Available Items"), "ffffff"), false, false);
			AddImage(0, 0, 10440);
			AddImage(365, 0, 10441);
			AddLabel(110, 100, 1152, "Select an item to buy for Verite Gems.");
			AddLabel(110, 135, 1152, "Item:");
			AddLabel(335, 135, 1152, "Price:");
			AddButton(200, 450, 242, 241, 0, GumpButtonType.Reply, 0); //CANCEL

			AddPage(1);

			int page = 1;
			int idx = 0;
			for (int i = 0; i < list.Count; i++)
			{
				ItemInfo info = list[i];

				if (idx >= 9)
				{
					page++;
					AddButton(360, 450, 5601, 5605, 0, GumpButtonType.Page, page);
					idx = 0;
					AddPage(page);
					AddButton(80, 450, 5603, 5607, 0, GumpButtonType.Page, page - 1);
				}
				AddButton(75, 165 + idx * 30, 4005, 4006, 1000 + i, GumpButtonType.Reply, 0);
				AddItem(100, 165 + idx * 30, info.ItemID);
				AddLabel(145, 165 + idx * 30, 1152, info.Name);
				AddLabel(335, 165 + idx * 30, 1152, CalculatePrice(info.Price).ToString());

				idx++;
			}
		}

		public override void OnResponse(NetState sender, RelayInfo info)
		{
			Mobile m = sender.Mobile;
			int button = info.ButtonID;
			ItemInfo m_Info;

			if(button == 0)
				m.SendGump(new CategoriesGump(m));

			else if (button > 999)
			{
				int id = button - 1000;
				if (id >= 0 && id < m_List.Count)
				{
					m_Info = m_List[id];
					if (m.Backpack != null && m.Backpack.ConsumeTotal(typeof(VeriteGem), CalculatePrice(m_Info.Price)))
						LLVendorItems.CreateItem(m_Info, m);
					else
						m.SendMessage("You do not have enough Verite Gems in your backpack.");
				}
			}
			else if (button == 0)
				m.SendGump(new CategoriesGump(m));
		}

		private int CalculatePrice(int goldPrice)
		{
			return (int)Math.Ceiling((double)( goldPrice * 3 / 50.0));
		}
	}

	public class CategoriesGump : AdvGump
	{
		public CategoriesGump(Mobile m)
			: base(60,60)
		{
			Closable = true;
			Disposable = true;
			Dragable = true;
			Resizable = false;
			AddPage(0);
			AddBackground(50, 30, 350, 467, 9260);
			AddImage(0, 0, 10440);
			AddImage(365, 0, 10441);
			AddBackground(125, 30, 200, 55, 9260);
			AddImageTiled(81, 30, 271, 11, 9261);

			AddHtml(50, 50, 350, 16, Colorize(Center("Item Categories"), "ffffff"), false, false);
			AddLabel(125, 110, 1152, "Category:");
			AddButton(85, 140, 4005, 4006, 1, GumpButtonType.Reply, 0);
			AddLabel(125, 140, 1152, "Mage Reagents");

			AddButton(85, 170, 4005, 4006, 2, GumpButtonType.Reply, 0);
			AddLabel(125, 170, 1152, "Necro Reagents");

			AddButton(85, 200, 4005, 4006, 3, GumpButtonType.Reply, 0);
			AddLabel(125, 200, 1152, "Potions");

			AddButton(85, 230, 4005, 4006, 4, GumpButtonType.Reply, 0);
			AddLabel(125, 230, 1152, "Bandages");

			AddButton(85, 260, 4005, 4006, 5, GumpButtonType.Reply, 0);
			AddLabel(125, 260, 1152, "Arrows / Bolts");

			AddButton(85, 290, 4005, 4006, 6, GumpButtonType.Reply, 0);
			AddLabel(125, 290, 1152, "Weapons with random bonuses");

			AddButton(85, 320, 4005, 4006, 7, GumpButtonType.Reply, 0);
			AddLabel(125, 320, 1152, "Armors with random bonuses");

			AddButton(85, 350, 4005, 4006, 8, GumpButtonType.Reply, 0);
			AddLabel(125, 350, 1152, "Misc");

			AddButton(200, 450, 247, 248, 0, GumpButtonType.Reply, 0);
		}

		public override void OnResponse(NetState sender, RelayInfo info)
		{
			Mobile m = sender.Mobile;
			int button = info.ButtonID;

			if (button > 0 && button < 9)
			{
				m.CloseGump(typeof(LLItemGump));
				m.SendGump(new LLItemGump(LLVendorItems.GetList(button)));
			}
		}
	}
}