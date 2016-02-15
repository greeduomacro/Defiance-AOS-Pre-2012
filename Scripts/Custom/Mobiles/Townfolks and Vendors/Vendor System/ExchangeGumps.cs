using System;
using System.Collections.Generic;
using Server.Gumps;
using Server.Network;
using Server.Targeting;
using Server.Items;
using Server.Mobiles;

namespace Server.Exchange
{
	public class EntryGump : AdvGump
	{
		private ExchangeCategory m_Category;

		public EntryGump(ExchangeCategory cat)
			: base(true)
		{
			m_Category = cat;

			int infos = cat.InfoList.Count;
			int infolength = 18 * infos;
			int totlength = infolength + 240;
			AddBackground(0, 0, 600, totlength, 2600);
			AddBackground(150, 30, 300, 90, 2600);
			AddHtml(0, 50, 600, 18, Center("West Britain Exchange"), false, false);
			AddHtml(0, 70, 600, 18, Center(cat.Name), false, false);
			//AddBackground(195, 48, 160, 50, 3000);

			int[] columns = new int[] { 35, 35, 170, 90, 90, 90 };
			string[] firstline = new string[] { "Bid", "Sell", "Commodity", "Bids", "Offers", "Average Price" };

			List<string> data = new List<string>(firstline);

			//150
			int count = 0;
			foreach (ExchangeTypeInfo il in cat.InfoList)
			{
				AddButton(45, 168 + 18 * count, 4005, 4006, 100 + count, GumpButtonType.Reply, 0);
				AddButton(80, 168 + 18 * count, 4005, 4006, 200 + count, GumpButtonType.Reply, 0);
				data.Add("");
				data.Add("");
				data.Add(il.Name);
				data.Add(il.BuyInfoList.Count.ToString());
				data.Add(il.SellInfoList.Count.ToString());
				data.Add(il.AveragePrice.ToString());
				count++;
			}

			AddTable(45, 150, columns, data);

			AddButton(55, totlength - 45, 4011, 4012, 1, GumpButtonType.Reply, 0);
			AddLabel(94, totlength - 45, 0, "Information");
			AddButton(520, totlength - 45, 4011, 4012, 2, GumpButtonType.Reply, 0);
			AddLabel(360, totlength - 45, 0, "View My Offers/Biddings");
		}

		public override void OnResponse(NetState sender, RelayInfo info)
		{
			Mobile m = sender.Mobile;
			int button = info.ButtonID;

			if (button == 1)
				m.SendGump(new ExchangeInfoGump());

			else if (button == 2)
			{
				List<BuySellInfo> bsil = new List<BuySellInfo>();

				foreach (ExchangeTypeInfo eti in m_Category.InfoList)
				{
					foreach (BuyInfo bi in eti.BuyInfoList)
						if (bi.Mobile == m)
							bsil.Add(bi);

					foreach (SellInfo si in eti.SellInfoList)
						if (si.Mobile == m)
							bsil.Add(si);
				}

				if (bsil.Count > 0)
					m.SendGump(new ViewMyBidsGump(bsil));

				else
					m.SendMessage("I am sorry i couldn't find any bids/offers from you.");
			}

			else if (button >= 100 && button - 100 < m_Category.InfoList.Count)
				m.SendGump(new BuyGump(new BuyInfo(m, m_Category.InfoList[button - 100])));

			else if (button >= 200 && button - 200 < ExchangeSystem.CategoryList.Count)
			{
				m.Target = new InternalTarget(m_Category);
				m.SendMessage("Please target the commodity deed you wish to sell.");
			}
		}

		private class InternalTarget : Target
		{
			private ExchangeCategory m_Category;

			public InternalTarget(ExchangeCategory cat)
				: base(12, false, TargetFlags.None)
			{
				m_Category = cat;
			}

			protected override void OnTarget(Mobile m, object targeted)
			{
				if (targeted is CommodityDeed)
				{
					CommodityDeed deed = (CommodityDeed)targeted;
					if (deed.IsChildOf(m.Backpack))
					{
						if (deed.Commodity != null)
						{
							if (deed.Commodity.Amount >= 25)
							{
								Type type = deed.Commodity.GetType();
								ExchangeTypeInfo einfo = null;

								foreach (ExchangeTypeInfo info in m_Category.InfoList)
								{
									if (info.Type == type)
									{
										einfo = info;
										break;
									}
								}

								if (einfo != null)
									m.SendGump(new SellGump(new SellInfo(deed, m, einfo)));

								else
									m.SendMessage("I am sorry i do not exchange that kind of goods, you may want to try it at my collegues.");
							}
							else
								m.SendMessage("We only accept deeds that contain at least 25 goods.");
						}
						else
							m.SendMessage("We only accept deeds that are already filled.");
					}
					else
						m.SendMessage("Please make sure your deed is in your backpack.");
				}
				else
					m.SendMessage("I am sorry we only accept deeded commodities.");
			}
		}
	}

	public class BuyGump : BuySellGump
	{
		private BuyInfo m_Info;
		private bool m_WasActive;

		public BuyGump(BuyInfo info)
			: base(false, info.Info)
		{
			if (info.Active)
			{
				m_WasActive = true;
				info.Deactivate();
			}
			m_Info = info;

			AddLabel(230, 360, 0, "Commodity:");
			AddLabel(300, 360, 0, info.Info.Name);

			AddLabel(230, 380, 0, "Price:");
			AddTextEntry(300, 380, 1202, 20, 0, 1, info.Price.ToString());
			AddButton(420, 380, 4029, 248, 1, GumpButtonType.Reply, 0);

			AddLabel(230, 400, 0, "Quantity:");
			AddTextEntry(300, 400, 100, 20, 0, 2, info.Quantity.ToString());
			AddButton(420, 400, 4029, 4030, 2, GumpButtonType.Reply, 0);

			int acq = 0;
			foreach (SellInfo si in info.Info.SellInfoList)
				if (si.Price <= info.Price)
					acq += si.Quantity;

			acq = Math.Min(info.Quantity, acq);

			AddLabel(230, 420, 0, "Immediate acquisitions: " + acq.ToString());

			AddLabel(260, 440, 0, "Commit");
			AddButton(230, 440, 4011, 4012, 5, GumpButtonType.Reply, 0);

			AddLabel(360, 440, 0, info.Active ? "Remove" : "Cancel");
			AddButton(330, 440, info.Active ? 4017 : 4020, info.Active ? 4018 : 4021, 0, GumpButtonType.Reply, 0);
		}

		public override void OnResponse(NetState sender, RelayInfo info)
		{
			Mobile m = sender.Mobile;

			switch (info.ButtonID)
			{
				case 0:
					if (m_WasActive)
						m.SendGump(new VerifyActionGump(m_Info));

					else
					{
						m.SendMessage("Your bid has not been added to the exchange board.");
						m.SendGump(new EntryGump(m_Info.Info.Category));
					}
					break;

				case 1:
					try
					{
						m_Info.Price = Math.Round(Convert.ToDouble(info.GetTextEntry(1).Text), 2);
					}
					catch { }
					m.SendGump(new BuyGump(m_Info));
					break;

				case 2:
					try
					{
						m_Info.Quantity = Convert.ToInt32(info.GetTextEntry(2).Text);
					}
					catch { }
					m.SendGump(new BuyGump(m_Info));
					break;

				case 5:
					int totalprice = (int)(m_Info.Quantity * m_Info.Price);
					bool resend = true;
					string message = "";

					if (m_Info.Price <= 0)
						message = "Please enter a valid price.";

					else if (m_Info.Quantity < 25)
						message = "We do not accept bids of under 25 goods.";

					else if (!Banker.Withdraw(m, totalprice + (m_WasActive ? 0 : Config.BuyCosts)))
						message = "You would not be able to afford such a transaction.";

					else
					{
						m_Info.Activate();
						resend = false;
					}

					if (resend)
					{
						m.SendMessage(message);
						m.SendGump(new BuyGump(m_Info));
					}
					break;
			}
		}
	}

	public class SellGump : BuySellGump
	{
		private SellInfo m_Info;

		public SellGump(SellInfo info)
			: base(true, info.Info)
		{
			m_Info = info;

			AddLabel(230, 360, 0, "Commodity:");
			AddLabel(300, 360, 0, info.Info.Name);

			AddLabel(230, 380, 0, "Price:");
			AddTextEntry(300, 380, 120, 20, 0, 1, info.Price.ToString());
			AddButton(420, 380, 4029, 4030, 1, GumpButtonType.Reply, 0);

			AddLabel(230, 400, 0, "Quantity:");
			AddLabel(300, 400, 0, info.Quantity.ToString());

			int acq = 0;
			foreach (BuyInfo bi in info.Info.BuyInfoList)
				if (bi.Price >= info.Price)
					acq += bi.Quantity;

			acq = Math.Min(info.Quantity, acq);

			AddLabel(230, 420, 0, "Immediate sales: " + acq.ToString());

			AddLabel(260, 440, 0, "Commit");
			AddButton(230, 440, 4011, 4012, 5, GumpButtonType.Reply, 0);

			AddLabel(360, 440, 0, info.Active ? "Remove" : "Cancel");
			AddButton(330, 440, info.Active ? 4017 : 4020, info.Active ? 4018 : 4021, 0, GumpButtonType.Reply, 0);
		}

		public override void OnResponse(NetState sender, RelayInfo info)
		{
			Mobile m = sender.Mobile;

			switch (info.ButtonID)
			{
				case 0:
					if (m_Info.Active)
						m.SendGump(new VerifyActionGump(m_Info));
					else
					{
						m.SendMessage("Your offer has not been added to the exchange board, your deed has been placed back into your backpack.");
						m.Backpack.DropItem(m_Info.GetDeed());
						m.SendGump(new EntryGump(m_Info.Info.Category));
					}
					break;

				case 1:
					try
					{
						m_Info.Price = Math.Round(Convert.ToDouble(info.GetTextEntry(1).Text), 2);
					}
					catch { }
					m.SendGump(new SellGump(m_Info));
					break;

				case 5:
					bool resend = true;
					string message = "";

					if (m_Info.Price <= 0)
						message = "Please enter a valid price.";

					else
					{
						m_Info.Activate();
						resend = false;
					}

					if (resend)
						m.SendGump(new SellGump(m_Info));

					m.SendMessage(message);

					break;
			}
		}
	}

	public class BuySellGump : AdvGump
	{
		public BuySellGump(bool selling, ExchangeTypeInfo info)
			: base(true)
		{
			AddBackground(0, 0, 800, 495, 9250);
			AddBackground(275, 15, 250, 76, 9250);
			AddHtml(0, 32, 800, 18, Center("West Britain Exchange"), false, false);
			AddHtml(0, 53, 800, 18, Center(selling ? "Selling " : "Buying " + info.Name), false, false);

			AddBackground(30, 100, 175, 80, 9250);
			AddHtml(30, 114, 175, 20, Center("Bids"), false, false);
			AddBackground(15, 135, 200, 210, 9250);
			AddBackground(30, 150, 170, 180, 9250);
			string[] firstline = new string[] { "Price", "Quantity" };
			int[] columns = new int[] { 70, 70 };
			List<string> data = new List<string>(firstline);
			for (int i = 0; i < 5 && i < info.BuyInfoList.Count; i++)
			{
				data.Add(info.BuyInfoList[i].Price.ToString());
				data.Add(info.BuyInfoList[i].Quantity.ToString());
			}

			AddTable(45, 165, columns, data);

			AddBackground(600, 100, 175, 80, 9250);
			AddHtml(600, 112, 175, 20, Center("Offers"), false, false);
			AddBackground(585, 135, 200, 210, 9250);
			AddBackground(600, 148, 170, 180, 9250);

			data = new List<string>(firstline);
			for (int i = 0; i < 5 && i < info.SellInfoList.Count; i++)
			{
				data.Add(info.SellInfoList[i].Price.ToString());
				data.Add(info.SellInfoList[i].Quantity.ToString());
			}
			AddTable(615, 165, columns, data);

			AddBackground(15, 345, 200, 135, 9250);

			columns = new int[] { 70, 100 };
			data = new List<string>(info.GetSalesInfo1);
			AddTable(30, 360, columns, data);

			AddBackground(585, 345, 200, 135, 9250);

			data = new List<string>(info.GetSalesInfo2);
			AddTable(600, 360, columns, data);

			AddBackground(215, 345, 370, 135, 9250);

			AddLabel(400, 320, 0, "Days");
			AddLabel(250, 310, 0, "0");
			AddLabel(560, 310, 0, "20");

			AddLabel(245, 140, 0, "P");
			AddLabel(220, 96, 0, info.HighestDayPrice == 0 ? "N/A" : info.HighestDayPrice.ToString());
			AddLabel(220, 185, 0, info.LowestDayPrice == double.MaxValue ? "N/A" : info.LowestDayPrice.ToString());
			AddLabel(245, 252, 0, "Q");
			string str = "";
			if (info.HighestDayQuantity == 0)
				str = "N/A";
			else if (info.HighestDayQuantity > 1000000)
				str = (info.HighestDayQuantity / 1000000).ToString() + "M";
			else if (info.HighestDayQuantity > 1000)
				str = (info.HighestDayQuantity / 1000).ToString() + "K";
			else
				str = info.HighestDayQuantity.ToString();
			AddLabel(220, 206, 0, str);

			AddImageTiled(265, 100, 3, 100, 2624);
			AddImageTiled(265, 150, 300, 1, 2624);
			AddImageTiled(265, 200, 300, 3, 2624);

			double conversion = 95 / (info.HighestDayPrice - info.LowestDayPrice);
			int locx = 0;
			foreach (ExchangeDay day in info.ExchangeDayList)
			{
				int height = (int)Math.Floor((day.Average - info.LowestDayPrice) * conversion);
				AddImage(265 + 12 + locx, 195 - height, 2103);
				locx += 15;
			}

			AddImageTiled(265, 210, 3, 100, 2624);
			AddImageTiled(265, 260, 300, 1, 2624);
			AddImageTiled(265, 310, 300, 3, 2624);

			conversion = 100 / (double)info.HighestDayQuantity;

			locx = 0;
			foreach (ExchangeDay day in info.ExchangeDayList)
			{
				int height = (int)Math.Floor(day.TotalQuantity * conversion);
				AddImageTiled(265 + locx, 310 - height, 15, height, 2624);
				locx += 15;
			}
		}
	}

	public class VerifyActionGump : AdvGump
	{
		private BuySellInfo m_Info;

		public VerifyActionGump(BuyInfo info)
			: base(true)
		{
			m_Info = info;

			Closable = false;
			Disposable = false;

			AddBackground(0, 0, 500, 350, 2600);
			AddBackground(125, 15, 250, 76, 9250);
			AddHtml(0, 32, 500, 18, Center("West Britain Exchange"), false, false);
			AddHtml(0, 53, 500, 18, Center("Selling"), false, false);

			AddBackground(75, 150, 350, 100, 3000);
			AddHtml(80, 155, 340, 90, "Warning!<br>Canceling this offer will remove it from the exchange board, the incurred costs will not be returned", false, false);

			AddButton(92, 294, 247, 248, 0, GumpButtonType.Reply, 0);
			AddButton(338, 292, 241, 242, 1, GumpButtonType.Reply, 0);
		}

		public VerifyActionGump(SellInfo info)
			: base(true)
		{
			m_Info = info;

			Closable = false;
			Disposable = false;

			int seizure = Math.Max((int)(m_Info.Quantity * Config.SellGoodsSeizure), 1);

			AddBackground(0, 0, 500, 350, 2600);
			AddBackground(125, 22, 250, 83, 2600);
			AddHtml(0, 32, 500, 18, Center("West Britain Exchange"), false, false);
			AddHtml(0, 53, 500, 18, Center("Selling"), false, false);

			AddBackground(75, 150, 350, 100, 3000);
			AddHtml(80, 155, 340, 90, String.Format("Warning!<br>Canceling this offer will result in the removal of {0} units of your goods.", seizure), false, false);

			AddButton(92, 294, 247, 248, 0, GumpButtonType.Reply, 0);
			AddButton(338, 292, 241, 242, 1, GumpButtonType.Reply, 0);
		}

		public override void OnResponse(NetState sender, RelayInfo info)
		{
			Mobile m = sender.Mobile;

			if (info.ButtonID == 0)
				m_Info.Deactivate();

			else
			{
				if (m_Info is SellInfo)
					m.SendGump(new SellGump((SellInfo)m_Info));

				else
					m.SendGump(new BuyGump((BuyInfo)m_Info));
			}
		}
	}

	public class ViewMyBidsGump : AdvGump
	{
		private List<BuySellInfo> m_List;

		public ViewMyBidsGump(List<BuySellInfo> list)
			: base(true)
		{
			m_List = list;

			int length = list.Count * 18;
			int totallength = length + 250;
			AddBackground(0, 0, 600, totallength, 2600);
			AddBackground(175, 22, 250, 83, 2600);
			AddHtml(0, 45, 600, 18, Center("West Britain Exchange"), false, false);
			AddHtml(0, 65, 600, 18, Center("Reviewing"), false, false);

			int[] columns = new int[] { 100, 60, 70, 70, 70, 70 };
			string[] firstline = new string[] { "Commodity", "Action", "Price", "Quantity", "Revenue", "DaysLeft" };

			List<string> data = new List<string>(firstline);

			int count = 0;
			DateTime now = DateTime.Now;
			foreach (BuySellInfo bsi in list)
			{
				bool buying = bsi is BuyInfo;
				int daysleft = (int)(buying ? Config.BidTime : Config.SellTime - (now - bsi.Date)).TotalDays;
				AddButton(25, 138 + 18 * count, 4005, 4006, 100 + count, GumpButtonType.Reply, 0);
				data.Add(bsi.Info.Name);
				data.Add(buying ? "Buying" : "Selling");
				data.Add(bsi.Price.ToString());
				data.Add(bsi.Quantity.ToString());
				data.Add(((int)(bsi.Quantity * bsi.Price)).ToString());
				data.Add(daysleft.ToString());
				count++;
			}
			AddTable(55, 120, columns, data);

			AddButton(275, totallength - 40, 247, 248, 0, GumpButtonType.Reply, 0);
		}

		public override void OnResponse(NetState sender, RelayInfo info)
		{
			Mobile m = sender.Mobile;
			int button = info.ButtonID;

			if (button >= 100 && button - 100 < m_List.Count)
			{
				BuySellInfo bsi = m_List[button - 100];
				bool buying = bsi is BuyInfo;
				if (buying)
				{
					BuyInfo bi = (BuyInfo)bsi;

					if (!m.HasGump(typeof(BuyGump)) && !m.HasGump(typeof(SellGump)) && !m.HasGump(typeof(VerifyActionGump)))
					{
						if (bi.Active)
							m.SendGump(new BuyGump(bi));

						else
							m.SendMessage("We could not find your bid, it might have been executed by now.");
					}

					else
						m.SendMessage("You already have an exchange gump open.");
				}

				else
				{
					SellInfo si = (SellInfo)bsi;

					if (!m.HasGump(typeof(BuyGump)) && !m.HasGump(typeof(SellGump)) && !m.HasGump(typeof(VerifyActionGump)))
					{
						if (si.Info.SellInfoList.Remove(si))
							m.SendGump(new SellGump(si));

						else
							m.SendMessage("We could not find your offer, it might have been executed by now.");
					}
					else
						m.SendMessage("You already have an exchange gump open.");

				}
			}
		}
	}

	public class TransactionConfirmationGump : AdvGump
	{
		private List<TransactionInfo> m_List;

		public TransactionConfirmationGump(List<TransactionInfo> list)
			: base(true)
		{
			m_List = list;

			Closable = false;
			Disposable = false;
			Dragable = false;

			int length = list.Count * 18;
			int totlength = length + 220;
			AddBackground(0, 0, 500, totlength, 2600);
			AddBackground(125, 22, 250, 83, 2600);
			AddHtml(0, 43, 500, 18, Center("West Britain Exchange"), false, false);
			AddHtml(0, 65, 500, 18, Center("Transaction History"), false, false);

			int[] columns = new int[] { 100, 70, 70, 70, 100 };
			string[] firstline = new string[] { "Commodity", "Action", "Price", "Quantity", "Revenue" };

			List<string> data = new List<string>(firstline);

			int count = 0;
			DateTime now = DateTime.Now;
			foreach (TransactionInfo ti in list)
			{
				data.Add(ti.Name);
				data.Add(ti.Buyer ? "Bought" : "Sold");
				data.Add(ti.Price.ToString());
				data.Add(ti.Quantity.ToString());
				data.Add(((int)(ti.Price * ti.Quantity)).ToString());
				count++;
			}
			AddTable(75, 125, columns, data);

			AddButton(225, totlength - 40, 247, 248, 1, GumpButtonType.Reply, 0);
		}

		public override void OnResponse(NetState sender, RelayInfo info)
		{
			Mobile m = sender.Mobile;

			if (info.ButtonID == 1)
				ExchangeSystem.TransactionHistory.Remove(m);
		}
	}

	public class ExchangeInfoGump : AdvGump
	{
		public ExchangeInfoGump()
			: base(true)
		{
			AddBackground(0, 0, 500, 550, 2600);
			AddBackground(125, 22, 250, 83, 2600);
			AddButton(209, 502, 247, 248, 0, GumpButtonType.Reply, 0);
			AddHtml(0, 43, 500, 18, Center("West Britain Exchange"), false, false);
			AddHtml(0, 65, 500, 18, Center("Information"), false, false);

			AddHtml(50, 115, 400, 380,
				"Welcome to the West Britain Commodities Exchange!<br><br>" +
				"We trade just about every commodity.<br>"+
				"If you own a commodity you have to fill it into a commodity deed and than contact the designated trader that is able to exchange your goods." +
				"If you want to buy a commodity you can just fill out a bid at a trader." +
				"All bids and sale-entries are removed after a period of 5 days. All downpayment will be returned and goods will be returned minus the non-sale expenses as explained below.<br><br>"+
				"Costs inclined:<br>"+
				"As a bidder you have to pay 150 gp per bid, so in small bids this is a lot of money on  large bid it isn't much.<br>"+
				String.Format("Sellers don't have to pay anything in the first place but if the goods havn't been sold within 5 days {0}% will be held back to pay for the expenses.", Config.SellGoodsSeizure * 100) +
				"As you are free to edit your prices at any time this never has to happen.<br><br>" +
				"We hope you will enjoy our services!"
				, true, true);
		}
	}
}