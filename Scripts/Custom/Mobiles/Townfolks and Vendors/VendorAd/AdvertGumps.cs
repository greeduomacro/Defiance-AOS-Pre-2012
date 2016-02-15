using System;
using System.Collections.Generic;
using Server.Gumps;
using Server.Network;
using Server.Targeting;
using Server.Mobiles;

namespace Server.Misc
{
	public class InfoGump : AdvGump
	{
		public InfoGump()
			: base(true)
		{
			AddBackground(0, 0, 400, 410, 3500);
			AddBackground(100, 20, 200, 50, 9350);
			AddHtml(0, 25, 400, 20, Center("Advertisement System"), false, false);
			AddHtml(0, 45, 400, 20, Center("Information"), false, false);
			AddBackground(50, 90, 300, 270, 9350);
			AddLabel(59, 95, 0, "Explanation:");
			AddHtml(60, 120, 280, 230, Colorize(
				"Welcome to the advertisement system. This system provides you with an opportunity to advertise your vendors.<br>"+
				"In exchange for this service you have to pay a small fee. The fee is based on a daily fee of 50gp, and a fee based on the usage of your advertisement which you can determine yourself.<br>"+
				"The higher you set the price per usage the higher it will be in the ranks when someone views the advertisements. The fee is substracted per usage, and has a maximum of once per 12 hours per account. So if the same person uses it twice in a small timeframe you only pay once.<br>"+
				"There are a number of commands that you can shout when you stand near an advertiser.<br><br>"+
				"Commands:<br>"+
				"Go (to go to the current advertisement)<br>"+
				"Show adds<br>"+
				"I wish to place a new advertisement<br>"+
				"I wish to review my advertisements<br><br>"+
				"Advertisements that are used while the owner is online only cost half the fee.<br>"+
				"The advertisers shout out a random advertisement every 5 minutes, players near them can say \"Go\" to go to the vendor."+
				"Players can also click on the advertisers or say \"show adds\" to view all adds in a categorized way. By clicking on the designated arrows they automatically go to your vendor.<br>"+
				"Advertisements can be altered or removed at any moment. This is done by clicking the message or cross button while viewing advertisements. Your own advertisements can either be found while reviewing all existing advertisements or by saying \"I wish to review my advertisements\".<br>" +
				"If you do not have enough money to keep your advertisement campaign running, your advertisement will be removed."+
				"When adding an advertisement please keep it clean or we will see it our duty to take actions upon you."
				, "333333"), false, true);
			AddButton(168, 370, 247, 248, 0, GumpButtonType.Reply, 0);
		}
	}

	public class AddVendorGump : AdvGump
	{
		private AdvertEntry m_Entry;

		public AddVendorGump(Mobile m)
			: this(new AdvertEntry(m))
		{ }

		public AddVendorGump(AdvertEntry entry)
			: base(true)
		{
			m_Entry = entry;

			int length = AdvertSystem.Categories.Length;
			AddBackground(240, 0, 350, 80 + 20 * length, 3500);

			AddLabel(444, 16, 0, "Category:");
			for (int i = 0; i < length; i++)
			{
				Category cat = AdvertSystem.Categories[i];
				AddButton(416, 40+i*20, cat == m_Entry.Category ?209:208, 209, 1000 + i, GumpButtonType.Reply, 0); AddLabel(449, 40 +i *20, 0, cat.Name);
			}

			AddBackground(0, 0, 400, 360, 3500);
			AddBackground(100, 20, 200, 50, 9350);
			AddHtml(0, 25, 400, 20, Center("Advertisement System"), false, false);
			AddHtml(0, 45, 400, 20, Center("Add an Advertisement"), false, false);
			AddButton(30, 88, 4005, 4006, 2, GumpButtonType.Reply, 0); AddLabel(73, 89, 0, m_Entry.Vendor == null ? "No vendor assigned" : m_Entry.Vendor.Name);
			AddBackground(72, 123, 275, 100, 9350);
			AddButton(30, 122, 4005, 4006, 3, GumpButtonType.Reply, 0); AddTextEntry(83, 131, 252, 85, 0, 0, m_Entry.Description == "" ? "No description entered" : m_Entry.Description);
			AddBackground(72, 230, 161, 26, 9350);
			AddButton(29, 232, 4005, 4006, 4, GumpButtonType.Reply, 0);
			AddTextEntry(80, 235, 94, 17, 0, 1, m_Entry.Price.ToString());
			AddLabel(180, 233, 0, "gp/use"); if (m_Entry.Price < AdvertSystem.Minimumprice) AddLabel(244, 235, 0, String.Format("Minimum price is {0}", AdvertSystem.Minimumprice));

			AddLabel(74, 266, 0, "Rank with this price:");

			if (m_Entry.Price < AdvertSystem.Minimumprice)
				AddLabel(220, 267, 0, "Price is too low");

			else
			{
				if (m_Entry.Category != null)
				{
					int rank = 1;
					foreach (AdvertEntry ent in m_Entry.Category.Entries)
					{
						if (ent.Price < m_Entry.Price)
							break;
						rank++;
					}
					AddLabel(220, 267, 0, rank.ToString());
				}

				else
					AddLabel(220, 267, 0, "No category selected");
			}

			AddButton(271, 307, 247, 248, 1, GumpButtonType.Reply, 0);
			AddButton(78, 310, 241, 242, 0, GumpButtonType.Reply, 0);
		}

		public override void OnResponse(NetState sender, RelayInfo info)
		{
			Mobile m = sender.Mobile;

			if (info.ButtonID > 999 && info.ButtonID < AdvertSystem.Categories.Length + 1000)
			{
				m_Entry.Category = AdvertSystem.Categories[info.ButtonID - 1000];
				m.SendGump(new AddVendorGump(m_Entry));
			}

			else
				switch (info.ButtonID)
				{
					case 1:
						if (m_Entry.Price >= AdvertSystem.Minimumprice && m_Entry.Vendor != null && m_Entry.Description != string.Empty && m_Entry.Category != null)
						{
							AdvertSystem.AddEntry(m_Entry);
							m.SendMessage("Your advertisement has been successfully added.");
						}

						else
						{
							m.SendMessage("Some of the required information was missing.");
							m.SendGump(new AddVendorGump(m_Entry));
						}
						break;

					case 2:
						m.Target = new InternalTarget(m_Entry);
						m.SendMessage("Please target your vendor.");
						break;

					case 3:
						m_Entry.Description = info.GetTextEntry(0).Text;
						m.SendGump(new AddVendorGump(m_Entry));
						break;

					case 4:
						int price;
						if(Int32.TryParse(info.GetTextEntry(1).Text, out price))
							 m_Entry.Price = price;
						m.SendGump(new AddVendorGump(m_Entry));
						break;
				}
		}

		private class InternalTarget : Target
		{
			private AdvertEntry m_Entry;

			public InternalTarget(AdvertEntry entry)
				: base(18, false, TargetFlags.None)
			{
				m_Entry = entry;
			}

			protected override void OnTarget(Mobile from, object targeted)
			{
				if (targeted is PlayerVendor)
				{
					PlayerVendor vendor = (PlayerVendor)targeted;
					//if (vendor.Owner == from)
					//{
						m_Entry.Vendor = vendor;
						from.SendGump(new AddVendorGump(m_Entry));
					//}

					//else
					//{
					//	from.SendMessage("You can't add an advertisement for a vendor that is not employed by you.");
					//	from.SendGump(new AddVendorGump(m_Entry));
					//}
				}

				else
				{
					from.SendMessage("That is not an eligible vendor.");
					from.SendGump(new AddVendorGump(m_Entry));
				}
			}
		}
	}

	public class AdvertCategoriesGump : AdvGump
	{
		private AdvertEntry m_Entry;

		public AdvertCategoriesGump()
			: base(true)
		{
			int cats = AdvertSystem.Categories.Length;
			AddBackground(0, 0, 400, 300 + cats * 20, 3500);
			AddBackground(100, 20, 200, 50, 9350);
			AddHtml(0, 25, 400, 20, Center("Advertisement System"), false, false);
			AddHtml(0, 45, 400, 20, Center("Choose your category"), false, false);
			AddLabel(69, 98, 0, "Categories:");
			AddLabel(296, 98, 0, "Entries:");
			for (int i = 0; i < cats; i++)
			{
				Category cat = AdvertSystem.Categories[i];
				AddButton(30, 120 + i * 25, 4005, 4006, 100 + i, GumpButtonType.Reply, 0);
				AddLabel(69, 120 + i * 25, 0, cat.Name);
				AddLabel(300, 120 + i * 25, 0, cat.Entries.Count.ToString());
			}

			List<Category> list = new List<Category>();
			foreach (Category cat in AdvertSystem.Categories)
				if (cat.Entries.Count > 0)
					list.Add(cat);

			if (list.Count > 0)
			{
				Category cat = list[Utility.Random(list.Count)];
				m_Entry = cat.Entries[Utility.Random(cat.Entries.Count)];
				AddButton(32, 245, 4005, 4006, 1, GumpButtonType.Reply, 0);
				AddBackground(68, 246, 275, 100, 9350);
				AddLabel(80, 252, 0, m_Entry.Vendor.Name);
				AddHtml(79, 273, 253, 65, Colorize(m_Entry.Description, "333333"), false, true);
			}
		}

		public override void OnResponse(NetState sender, RelayInfo info)
		{
			Mobile m = sender.Mobile;

			if (info.ButtonID > 99 && info.ButtonID < AdvertSystem.Categories.Length + 100)
				m.SendGump(new AdvertEntriesGump(AdvertSystem.Categories[info.ButtonID - 100], 0,m));

			else if (info.ButtonID == 1)
			{
				if (m_Entry != null)
					AdvertSystem.UseAdvert(m, m_Entry);
			}
		}
	}

	public class AdvertEntriesGump : AdvGump
	{
		private Category m_Category;
		private int m_Started;
		private List<AdvertEntry> m_Entries;

		public AdvertEntriesGump(Category cat, int start, Mobile m)
			: base(true)
		{
			bool access = m.AccessLevel >= AccessLevel.GameMaster;
			m_Category = cat;
			m_Started = start;
			if (cat == null)
			{
				m_Entries = new List<AdvertEntry>();
				foreach (Category cato in AdvertSystem.Categories)
					foreach (AdvertEntry entry in cato.Entries)
						if (AdvertSystem.MobileExists(entry.Vendor) && entry.Owner == m)
							m_Entries.Add(entry);
			}
			else
				m_Entries = cat.Entries;
			int bottom = 0;
			int rightside = 0;

			if (start <= m_Entries.Count)
			{
				int show = Math.Min(m_Entries.Count - start, 8);
				int ver = Math.Min(show, 4);
				int hor = Math.Max((int)Math.Ceiling(show / 4.0), 1);
				bottom = 150 + ver * 110;
				rightside = hor == 1? 400: 700;
				AddBackground(0, 0, rightside, bottom, 3500);
				AddBackground((rightside - 200) / 2, 20, 200, 50, 9350);
				AddHtml(0, 25, rightside, 20, Center("Advertisement System"), false, false);
				AddHtml(0, 45, rightside, 20, Center("Find your shop"), false, false);
				for (int i = 0; i < show; i++)
				{
					AdvertEntry entry = m_Entries[start + i];
					bool owner = entry.Owner == m;
					if (i < 4)
					{
						AddButton(30, 85 + 110 * i, 4005, 4006, 100 + i, GumpButtonType.Reply, 0);
						if (owner || access)
						{
							AddButton(30, 110 + 110 * i, 4011, 4012, 1000 + i, GumpButtonType.Reply, 0);
							AddButton(30, 135 + 110 * i, 4017, 4018, 1100 + i, GumpButtonType.Reply, 0);
						}
						AddBackground(70, 85 + 110 * i, 275, 100, 9350);
						AddLabel(80, 90 + 110 * i, 0, entry.Vendor.Name);
						AddHtml(80, 115 + 110 * i, 253, 65, Colorize(entry.Description, "333333"), false, true);
					}

					else
					{
						AddButton(630, 85 + 110 * (i - 4), 4014, 4015, 100 + i, GumpButtonType.Reply, 0);
						if (owner || access)
						{
							AddButton(630, 110 + 110 * (i - 4), 4011, 4012, 1000 + i, GumpButtonType.Reply, 0);
							AddButton(630, 135 + 110 * (i - 4), 4017, 4018, 1100 + i, GumpButtonType.Reply, 0);
						}
						AddBackground(350, 85 + 110 * (i - 4), 275, 100, 9350);
						AddLabel(360, 85 + 110 * (i - 4), 0, entry.Vendor.Name);
						AddHtml(360, 115 + 110 * (i - 4), 253, 65, Colorize(entry.Description, "333333"), false, true);
					}
				}

				if (start > 0)
					AddButton(29, bottom - 40, 4014, 4015, 10, GumpButtonType.Reply, 0);

				if (start + show < m_Entries.Count)
					AddButton(rightside - 70, bottom - 40, 4005, 4006, 20, GumpButtonType.Reply, 0);
			}
		}

		public override void OnResponse(NetState sender, RelayInfo info)
		{
			Mobile m = sender.Mobile;

			if (info.ButtonID >= 1100)
			{
				int loc = info.ButtonID - 1100 + m_Started;
				if (loc < m_Entries.Count)
				{
					AdvertEntry entry = m_Entries[loc];
					if (m.AccessLevel >= AccessLevel.GameMaster ||(AdvertSystem.MobileExists(entry.Vendor) && entry.Owner == m))
						entry.Category.Entries.Remove(entry);
				}
			}

			else if (info.ButtonID >= 1000)
			{
				int loc = info.ButtonID - 1000 + m_Started;
				if (loc < m_Entries.Count)
				{
					AdvertEntry entry = m_Entries[loc];
					if (m.AccessLevel >= AccessLevel.GameMaster || (AdvertSystem.MobileExists(entry.Vendor) && entry.Owner == m))
						m.SendGump(new AddVendorGump(entry));
				}
			}

			else if (info.ButtonID > 99)
			{
				int loc = info.ButtonID - 100 + m_Started;
				if (loc < m_Entries.Count)
					AdvertSystem.UseAdvert(m, m_Entries[loc]);
			}

			else
				switch (info.ButtonID)
				{
					case 0: m.SendGump(new AdvertCategoriesGump()); break;
					case 10: m.SendGump(new AdvertEntriesGump(m_Category, Math.Max(m_Started - 6, 0),m));break;
					case 20: m.SendGump(new AdvertEntriesGump(m_Category, m_Started + 6,m)); break;
				}
		}
	}
}