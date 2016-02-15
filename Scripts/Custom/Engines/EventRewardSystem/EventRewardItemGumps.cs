using Server.Gumps;
using Server.Network;
using Server.Mobiles;
using System;
using System.IO;
using System.Collections.Generic;

namespace Server.Engines.RewardSystem
{
	public class ItemGump : AdvGump
	{
		private EventRewardInfo m_Info;

		public ItemGump(EventRewardInfo info)
			: base(60, 60)
		{
			m_Info = info;

			Closable = true;
			Disposable = true;
			Dragable = true;
			Resizable = false;
			AddPage(0);

			AddBackground(50, 30, 350, 429, 9260);
			AddImage(0, 0, 10440);
			AddImage(365, 0, 10441);
			AddBackground(125, 30, 200, 55, 9260);
			AddImageTiled(81, 30, 271, 11, 9261);
			AddHtml(50, 50, 350, 16, Colorize(Center("Event Reward Details"), "ffffff"), false, false);
			AddButton(85, 407, 4005, 4006, 1, GumpButtonType.Reply, 0);
			AddButton(300, 410, 242, 241, 0, GumpButtonType.Reply, 0);
			AddBackground(85, 250, 280, 150, 9260);
			AddBackground(175, 120, 100, 100, 9200);
			AddItem(info.X,info.Y,info.ItemID);
			AddHtml(50, 196, 350, 18, Colorize(Center("Bars: " + info.Price.ToString()), "ffffff"), false, false);
			AddHtml(50, 100, 350, 18, Colorize(Center(info.Name), "ffffff"), false, false);
			AddHtml(50, 230, 350, 18, Colorize(Center("Explanation:"), "ffffff"), false, false);
			AddHtml(100, 265, 250, 120, Colorize(info.Info, "ffffff"), false, false);
			AddLabel(120, 410, 1152, "Order this item");
			AddImage(183, 143, 5182);
		}

		public override void OnResponse(NetState sender, RelayInfo info)
		{
			Mobile m = sender.Mobile;
			int button = info.ButtonID;

			if (button == 1)
			{
				if (m.Backpack != null && m.Backpack.ConsumeTotal(typeof(CopperBar), m_Info.Price))
					EventRewardSystem.CreateReward(m_Info, m);

				else
					m.SendMessage("You do not have enough copper bars in your backpack.");
			}

			else if (button == 0)
				m.SendGump(new IndexGump(EventRewardSystem.GetList((int)m_Info.RType)));
		}
	}
	public class IndexGump : AdvGump
	{
		private List<EventRewardInfo> m_List;

		public IndexGump(List<EventRewardInfo> list)
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
			AddHtml(50, 50, 350, 16, Colorize(Center("Available Rewards"), "ffffff"), false, false);
			AddImage(0, 0, 10440);
			AddImage(365, 0, 10441);
			AddLabel(110, 100, 1152, "Select an item to see full description.");
			AddLabel(110, 135, 1152, "Reward:");
			AddLabel(300, 135, 1152, "Price:");
			AddButton(200, 450, 242, 241, 0, GumpButtonType.Reply, 0); //CANCEL

			AddPage(1);

			int page = 1;
			int idx = 0;
			for (int i = 0; i < list.Count; i++)
			{
				EventRewardInfo info = list[i];

				if (idx >= 10)
				{
					page++;
					AddButton(360, 450, 5601, 5605, 0, GumpButtonType.Page, page);
					idx = 0;
					AddPage(page);
					AddButton(80, 450, 5603, 5607, 0, GumpButtonType.Page, page - 1);
				}
				AddButton(75, 165 + idx * 25, 4005, 4006, 1000 + i, GumpButtonType.Reply, 0);
				AddLabel(110, 165 + idx * 25, 1152, info.Name);
				AddLabel(300, 165 + idx * 25, 1152, info.Price.ToString());

				idx++;
			}
		}

		public override void OnResponse(NetState sender, RelayInfo info)
		{
			Mobile m = sender.Mobile;
			int button = info.ButtonID;

			if(button == 0)
				m.SendGump(new OpeningGump(m));

			else if (button > 999)
			{
				int id = button - 1000;
				if (id >= 0 && id < m_List.Count)
					m.SendGump(new ItemGump(m_List[id]));
			}
		}
	}

	public class OpeningGump : AdvGump
	{
		public OpeningGump(Mobile m)
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

			AddHtml(50, 50, 350, 16, Colorize(Center("Event Reward Categories"), "ffffff"), false, false);
			AddHtml(100, 110, 250, 175, ( m.Female? "Looking good today m'lady,":"Hey there young lad," ) +
			"<br><br>I have everything you are looking for, just choose a category" +
			" and i will be glad to show you more. <br><br>Just make sure you have enough copper bars to spare."
			, true, false);
			AddLabel(125, 310, 1152, "Category:");
			AddLabel(285, 310, 1152, "Entries:");
			AddButton(85, 340, 4005, 4006, 1, GumpButtonType.Reply, 0);
			AddLabel(125, 340, 1152, "Game Items");
			AddLabel(285, 340, 1152, EventRewardSystem.GetList(1).Count.ToString());

			AddButton(85, 370, 4005, 4006, 2, GumpButtonType.Reply, 0);
			AddLabel(125, 370, 1152, "House Decorations");
			AddLabel(285, 370, 1152, EventRewardSystem.GetList(2).Count.ToString());

			AddButton(85, 400, 4005, 4006, 3, GumpButtonType.Reply, 0);
			AddLabel(125, 400, 1152, "Fancy Items");
			AddLabel(285, 400, 1152, EventRewardSystem.GetList(3).Count.ToString());

			AddButton(200, 450, 247, 248, 0, GumpButtonType.Reply, 0);
		}

		public override void OnResponse(NetState sender, RelayInfo info)
		{
			Mobile m = sender.Mobile;
			int button = info.ButtonID;

			if (button > 0 && button < 4)
			{
				m.CloseGump(typeof(IndexGump));
				m.SendGump(new IndexGump(EventRewardSystem.GetList(button)));
			}
		}
	}

	public class RequestCopperBarsGump : AdvGump
	{
		public RequestCopperBarsGump(Mobile m)
			: base(60, 60)
		{
			Closable = true;
			Disposable = true;
			Dragable = true;
			Resizable = false;
			AddPage(0);
			AddBackground(0, 0, 550, 335, 9200);
			AddImageTiled(8, 8, 204, 22, 2624); AddAlphaRegion(8, 8, 204, 22); AddLabel(12, 11, 1152, "GM Event Copper Bar Request");
			AddImageTiled(10, 42, 204, 22, 2624); AddAlphaRegion(10, 42, 204, 22); AddLabel(11, 44, 1152, "Requested Bars:");
			AddImageTiled(9, 70, 84, 22, 2624); AddAlphaRegion(9, 70, 84, 22); AddTextEntry(10, 72, 80, 20, 1152, 0, "");
			AddImageTiled(10, 98, 204, 22, 2624); AddAlphaRegion(10, 98, 204, 22); AddLabel(16, 100, 1152, "Event Name:");
			AddImageTiled(9, 126, 204, 22, 2624); AddAlphaRegion(9, 126, 204, 22); AddTextEntry(12, 127, 199, 20, 1152, 1, "");
			AddImageTiled(10, 159, 204, 22, 2624); AddAlphaRegion(10, 159, 204, 22); AddLabel(17, 163, 1152, "Further Explanation:");
			AddImageTiled(10, 186, 204, 130, 2624); AddAlphaRegion(10, 186, 205, 135); AddTextEntry(12, 189, 201, 130, 1152, 2, "");

			AddImageTiled(272, 12, 246, 273, 2624); AddAlphaRegion(272, 12, 246, 273);
			AddLabel(283, 17, 1152, "Information:");
			AddHtml(280, 40, 233, 239, Colorize("Usual requests are 6 copper bars per event, or if it takes longer than an hour 6 extra copper bars per hour, so a 2 hour event would be 12 copper bars.<br>Make sure you enter all fields, or your request will be denied.<br>Under further explanation you can enter who will receive the rewards ie 1st place, 2nd place etc. Everything you enter is logged, and reviewed by senior staffmembers.", "ffffff"), false, false);
			AddButton(280, 296, 241, 242, 0, GumpButtonType.Reply, 0);
			AddButton(429, 295, 247, 248, 1, GumpButtonType.Reply, 0);
		}

		public override void OnResponse(NetState sender, RelayInfo info)
		{
			Mobile m = sender.Mobile;
			bool handed = false;
			bool canceled = false;
			int amount = 0;
			string eventname = "";
			string explanation = "";

			try
			{
				amount = Convert.ToInt32(info.TextEntries[0].Text);
				eventname = info.TextEntries[1].Text;
				explanation = info.TextEntries[2].Text;
			}

			catch
			{
				m.SendMessage("Something went wrong, please contact a dev.");
				return;
			}

			if (info.ButtonID == 1)
			{
				int oldamount = 0;
				EventRewardSystem.CopperHandoutDictionary.TryGetValue(m, out oldamount);

				if (m.AccessLevel >= AccessLevel.Administrator || oldamount + amount <= EventRewardSystem.MaxCopper)
				{
					EventRewardSystem.CopperHandoutDictionary[m] = oldamount + amount;

					if (m.Backpack != null)
						m.AddToBackpack(new CopperBar(amount));

					m.SendMessage("The copper bars have been created, good luck with your event!");
				}

				else
					m.SendMessage("You have reached the daily limit of copper requests for this day.");
			}

			else
				canceled = true;

			StreamWriter writer = new StreamWriter("Logs\\CopperBarLogging.log",true);
			string status = canceled ? "Canceled" : handed ? "Allowed" : "Disallowed";
			string output =
String.Format
(
@"

{5} - {0}
Name: {1}
Amount: {2}
EventName: {3}
Further Explanation: {4}
", status, m.Name, amount.ToString(), eventname, explanation, DateTime.Now.ToString( "MM/dd/yyyy" )
);
			writer.Write(output);
			writer.Close();
		}
	}
}