//******************************************************
// Name: ERefillUtility
// Desc: Written by Eclipse
//******************************************************
using System;
using Server;
using Server.Items;
using Server.Mobiles;
using System.Collections.Generic;

namespace Server
{
	public class ERefillUtility
	{
		public static List<RefillEntry> Refill(Container cont, Type[] itemTypes, BaseVendor vendor, Mobile from, bool Refill, int amount)
		{
			List<RefillEntry> refillEntryList = new List<RefillEntry>();
			RefillEntry refillEntry;

			foreach (Type itemType in itemTypes)
			{
				bool foundReagentInBag = false;

				refillEntry = new RefillEntry();
				refillEntry.ItemType = itemType;

				for (int i = 0; i < cont.Items.Count; i++)
				{
					Item item = (Item)cont.Items[i];
					if (itemType == item.GetType())
					{
						// Add amount to refill to entry
						int amountToRefill = amount - item.Amount;
						refillEntry.AmountToRefill = amountToRefill;

						// Add price on vendor to entry
						int priceOnVendor = GetPriceFromVendor(vendor, itemType);
						refillEntry.PriceOnVendor = priceOnVendor;

						if (priceOnVendor > 0)
						{
							refillEntry.HasVendorGotItem = true;

							if (Refill)
								item.Amount += amountToRefill;
						}
						else
						{
							// Vendor does not have this item
						}

						foundReagentInBag = true;
						break;
					}
				}

				// We could not find the item in the players bag, create the item
				// and set the amount.
				if (!foundReagentInBag)
				{
					// Add amount to refill to entry
					refillEntry.AmountToRefill = amount;

					// Add price on vendor to entry
					int priceOnVendor = GetPriceFromVendor(vendor, itemType);
					refillEntry.PriceOnVendor = priceOnVendor;

					if (priceOnVendor > 0)
					{
						refillEntry.HasVendorGotItem = true;

						if (Refill)
							DirectRefillType(cont, itemType, amount);
					}
					else
					{
						// Vendor does not have this item
					}
				}

				// Add the entry
				refillEntryList.Add(refillEntry);
			}

			return refillEntryList;
		}

		public static int GetPriceFromVendor(BaseVendor vendor, Type itemType)
		{
			IBuyItemInfo[] buyInfo = vendor.GetBuyInfo();

			for (int i = 0; i < buyInfo.Length; ++i)
			{
				IBuyItemInfo buyItem = (IBuyItemInfo)buyInfo[i];
				GenericBuyInfo gbi = (GenericBuyInfo)buyItem;

				if (gbi.Type == itemType)
					return buyItem.Price;
			}

			return 0;
		}

		/// <summary>
		/// Refills all the items
		/// </summary>
		/// <param name="amount"></param>
		public static void DirectRefill(Container cont, Type[] itemTypes, int amount)
		{
			foreach (Type itemType in itemTypes)
				DirectRefillType(cont, itemType, amount);
		}

		/// <summary>
		/// Refills the bag with the item given.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="amount"></param>
		public static void DirectRefillType(Container cont, Type type, int amount)
		{
			Item item = Activator.CreateInstance(type) as Item;
			item.Amount = amount;
			cont.DropItem(item);
		}
	}

	#region RefillEntry
	public class RefillEntry
	{
		private bool m_bHasVendorGotItem;
		public bool HasVendorGotItem
		{
			get { return m_bHasVendorGotItem; }
			set { m_bHasVendorGotItem = value; }
		}

		private int m_iAmountToRefill;
		public int AmountToRefill
		{
			get { return m_iAmountToRefill; }
			set { m_iAmountToRefill = value; }
		}

		private int m_iPriceOnVendor;
		public int PriceOnVendor
		{
			get { return m_iPriceOnVendor; }
			set { m_iPriceOnVendor = value; }
		}

		public int TotalCost
		{
			get
			{
				if (m_bHasVendorGotItem && m_iPriceOnVendor > 0)
					return m_iAmountToRefill * m_iPriceOnVendor;
				else return 0;
			}
		}

		public bool CanBeRefilled
		{
			get { return m_iAmountToRefill > 0; }
		}

		private Type m_ItemType;
		public Type ItemType
		{
			get { return m_ItemType; }
			set { m_ItemType = value; }
		}

		public RefillEntry()
		{
		}
	}
	#endregion
}