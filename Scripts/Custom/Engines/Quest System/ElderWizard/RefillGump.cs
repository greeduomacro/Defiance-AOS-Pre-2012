using System;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Mobiles;

namespace Server.Gumps
{
	public class RefillGump : Gump
	{
		private BaseVendor m_Vendor;
		private QuestReagentBag m_Bag;
		private int m_Price;
		private List<RefillEntry> m_RefillEntryList;

		public RefillGump(BaseVendor vendor, Mobile from, QuestReagentBag bag, List<RefillEntry> refillEntryList)
			: base(50, 50)
		{
			from.CloseGump(typeof(RefillGump));

			m_Vendor = vendor;
			m_Bag = bag;
			m_RefillEntryList = refillEntryList;

			this.Closable = true;
			this.Disposable = true;
			this.Dragable = true;
			this.Resizable = false;

			this.AddPage(0);

			this.AddBackground(0, 0, 453, 285, 2620);
			this.AddAlphaRegion(4, 7, 443, 271);
			this.AddLabel(bag.Type == QuestReagentBag.ReagentBagType.Mage ? 160 : 140, 13, 955, string.Format("Refill {0}", bag.Name));
			this.AddLabel(22, 45, 955, @"Reagents");
			this.AddLabel(106, 45, 955, @"Amount to refill");
			this.AddLabel(371, 45, 955, @"Total Cost");
			this.AddLabel(239, 45, 955, @"Cost per reagent");
			this.AddButton(410, 250, 4017, 4018, (int)Buttons.Cancel, GumpButtonType.Reply, 0);

			m_Price = ShowReagentCostList(refillEntryList);

			if (m_Price > 0)
			{
				this.AddLabel(100, 229, 955, string.Format("Do you want to refill your bag for {0} gp?", m_Price));
				this.AddButton(210, 250, 4023, 4024, (int)Buttons.OK, GumpButtonType.Reply, 0);
			}
		}

		private int ShowReagentCostList(List<RefillEntry> refillEntryList)
		{
			int totalCost = 0;

			for (int i = 0; i < refillEntryList.Count; ++i)
			{
				RefillEntry entry = (RefillEntry)refillEntryList[i];
				totalCost += entry.TotalCost;

				if (entry.ItemType != null)
					this.AddLabel(22, 70 + i * 20, 955, entry.ItemType.Name);

				if (entry.HasVendorGotItem)
				{
					this.AddLabel(147, 70 + i * 20, 955, entry.AmountToRefill.ToString());
					this.AddLabel(277, 70 + i * 20, 955, string.Format("{0} gp", entry.PriceOnVendor));
					this.AddLabel(380, 70 + i * 20, 955, string.Format("{0} gp", entry.TotalCost));
				}
				else
				{
					this.AddLabel(147, 70 + i * 20, 955, "*Vendor does not sell this item*");
				}
			}

			return totalCost;
		}

		public enum Buttons
		{
			Cancel,
			OK,
		}

		public static bool ChargePlayer(Mobile player, int amount)
		{
			Container backPack = player.Backpack;

			if (backPack != null && backPack.ConsumeTotal(typeof(Gold), amount))
			{
				player.SendMessage("{0} gp has been removed from your backpack.", amount);
				return true;
			}
			else if (Banker.Withdraw(player, amount))
			{
				player.SendLocalizedMessage(1060398, amount.ToString()); //~1_AMOUNT~ gold has been withdrawn from your bank box.
				return true;
			}

			return false;
		}

		public override void OnResponse(Server.Network.NetState sender, RelayInfo info)
		{
			switch (info.ButtonID)
			{
				case (int)Buttons.OK:
					if (sender.Mobile.InRange(m_Vendor, 10))
					{
						List<RefillEntry> refillEntryList = ERefillUtility.Refill(m_Bag, m_Bag.ReagentTypes, m_Vendor, sender.Mobile, false, m_Bag.BagRefillAmount);
						int curCost = 0;

						foreach (RefillEntry entry in refillEntryList)
							curCost += entry.TotalCost;

						if (m_Price != curCost)
						{
							m_Vendor.SayTo(sender.Mobile, "Seems like you have changed the reagents in your bag!");
							m_Vendor.SayTo(sender.Mobile, "I will not refill your bag to that price anymore!");
						}
						else
						{
							if (ChargePlayer(sender.Mobile, curCost))
							{
								ERefillUtility.Refill(m_Bag, m_Bag.ReagentTypes, m_Vendor, sender.Mobile, true, m_Bag.BagRefillAmount);
							}
							else
								m_Vendor.SayTo(sender.Mobile, 500192); // Begging thy pardon, but thou casnt afford that.
						}
					}
					else
						sender.Mobile.SendLocalizedMessage(500295); // You are too far away to do that.
					break;
			}
		}
	}
}