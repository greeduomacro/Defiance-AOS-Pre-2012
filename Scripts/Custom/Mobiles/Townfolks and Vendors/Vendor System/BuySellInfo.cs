using System;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Items;

namespace Server.Exchange
{
	public class BuySellInfo
	{
		public double Price;
		public int Quantity;
		public Mobile Mobile;
		public DateTime Date;
		public ExchangeTypeInfo Info;
		public bool Active;

		public BuySellInfo(Mobile mobile, ExchangeTypeInfo info):this(mobile, info, 0)
		{
		}

		public BuySellInfo(Mobile mobile, ExchangeTypeInfo info, int quantity)
		{
			Quantity = quantity;
			Mobile = mobile;
			Info = info;
			Date = DateTime.Now;
		}

		public virtual void Activate()
		{
			Active = true;
		}

		public virtual void Deactivate()
		{
			Active = false;
		}

		public void CreateTransactionHistory(string name, Mobile buyer, Mobile seller, int quantity, double price)
		{
			List<TransactionInfo> lti;

			if (!ExchangeSystem.TransactionHistory.TryGetValue(buyer, out lti))
			{
				lti = new List<TransactionInfo>();
				lti.Add(new TransactionInfo(name, price, quantity, true));
				ExchangeSystem.TransactionHistory[buyer] = lti;
			}

			else
				lti.Add(new TransactionInfo(name, price, quantity, true));

			if (!ExchangeSystem.TransactionHistory.TryGetValue(seller, out lti))
			{
				lti = new List<TransactionInfo>();
				lti.Add(new TransactionInfo(name, price, quantity, false));
				ExchangeSystem.TransactionHistory[seller] = lti;
			}

			else
				lti.Add(new TransactionInfo(name, price, quantity, false));
		}

		public bool VerifyOwner { get { return Mobile != null && !Mobile.Deleted; } }

		#region Ser/Deser
		public virtual void Serialize(GenericWriter writer)
		{
			writer.Write(0);//version

			writer.Write(Price);
			writer.Write(Quantity);
			writer.Write(this.Mobile);
			writer.Write(Date);
			writer.Write(Active);
			//Info set by the collections this belongs to
		}

		public BuySellInfo(GenericReader reader)
		{
			int version = reader.ReadInt();

			Price = reader.ReadDouble();
			Quantity = reader.ReadInt();
			this.Mobile = reader.ReadMobile();
			Date = reader.ReadDateTime();
			Active = reader.ReadBool();
			//Info set by the collections this belongs to
		}
		#endregion
	}

	public class BuyInfo : BuySellInfo, IComparable
	{
		public BuyInfo(Mobile mobile, ExchangeTypeInfo info):base(mobile, info)
		{
		}

		public int CompareTo(object o)
		{
			if (o is BuyInfo)
			{
				BuyInfo ae = (BuyInfo)o;
				return (int)Math.Ceiling(((ae.Price - Price)));
			}
			else
				throw new ArgumentException("object is not BuyInfo");
		}

		public void ReturnMoney()
		{
			GiveMoneyTo(Quantity, Mobile);
		}

		public void GiveMoneyTo(int amount, Mobile m)
		{
			if (m != null && !m.Deleted)
			{
				int money = (int)(Price * amount);

				while (true)
				{
					if (money > 2000)
					{
						int tocheck = Math.Min(money, 1000000);
						m.BankBox.DropItem(new BankCheck(tocheck));
						money -= tocheck;
					}

					else
					{
						m.BankBox.DropItem(new Gold(money));
						break;
					}
				}
			}

			Quantity -= amount;
		}

		public override void Activate()
		{
			base.Activate();
			List<SellInfo> toremove = new List<SellInfo>();
			List<Mobile> sendhistory = new List<Mobile>();

			bool add = true;

			foreach (SellInfo si in Info.SellInfoList)
			{
				if (!si.VerifyOwner)
				{
					toremove.Add(si);
					continue;
				}

				if (!add || si.Price > Price)
					break;

				int acq = Math.Min(Quantity, si.Quantity);
				int payment = (int)(Price * acq);

				if (si.Quantity == acq)
					toremove.Add(si);

				Mobile.Backpack.DropItem(si.GetDeed(acq));
				GiveMoneyTo(acq, si.Mobile);

				Info.ActivateExchange(acq, Price);
				CreateTransactionHistory(Info.Name, Mobile, si.Mobile, acq, Price);

				if (!sendhistory.Contains(si.Mobile))
					sendhistory.Add(si.Mobile);

				if (!sendhistory.Contains(Mobile))
					sendhistory.Add(Mobile);

				if (Quantity == 0)
					add = false;
			}

			if (add)
			{
				Info.BuyInfoList.Add(this);
				Info.BuyInfoList.Sort();
				Mobile.SendMessage("Your order has been added to the exchange board.");
			}

			else
				Mobile.SendMessage("Your order has been executed.");

			foreach (SellInfo si in toremove)
				Info.SellInfoList.Remove(si);

			foreach (Mobile m in sendhistory)
				ExchangeSystem.SendHistoryTo(m);
		}

		public override void Deactivate()
		{
			base.Deactivate();
			ReturnMoney();
			Info.BuyInfoList.Remove(this);
		}

		#region Ser/Deser
		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write(0);//version
		}

		public BuyInfo(GenericReader reader):base(reader)
		{
			int version = reader.ReadInt();
		}
		#endregion
	}

	public class SellInfo : BuySellInfo, IComparable
	{
		public SellInfo(CommodityDeed deed, Mobile mobile, ExchangeTypeInfo info):base(mobile, info,deed.Commodity.Amount)
		{
			deed.Delete();
		}

		public CommodityDeed GetDeed()
		{
			return GetDeed(Quantity);
		}

		public CommodityDeed GetDeed(int amount)
		{
			Item item = (Item)Activator.CreateInstance(Info.Type);
			item.Amount = amount;
			Quantity -= amount;
			return new CommodityDeed(item);
		}

		public override void Activate()
		{
			base.Activate();

			List<BuyInfo> toremove = new List<BuyInfo>();
			List<Mobile> sendhistory = new List<Mobile>();

			bool add = true;

			foreach (BuyInfo bi in Info.BuyInfoList)
			{
				if (!bi.VerifyOwner)
				{
					toremove.Add(bi);
					continue;
				}

				if (!add || bi.Price < Price)
					break;

				double price = bi.Price;
				int acq = Math.Min(Quantity, bi.Quantity);
				int payment = (int)(price * acq);

				if (bi.Quantity == acq)
					toremove.Add(bi);

				bi.Mobile.Backpack.DropItem(GetDeed(acq));
				bi.GiveMoneyTo(acq,Mobile);

				Info.ActivateExchange(acq, price);
				CreateTransactionHistory(Info.Name, bi.Mobile, Mobile, acq, price);

				if (!sendhistory.Contains(bi.Mobile))
					sendhistory.Add(bi.Mobile);

				if (!sendhistory.Contains(Mobile))
					sendhistory.Add(Mobile);

				if (Quantity == 0)
					add = false;
			}

			if (add)
			{
				Info.SellInfoList.Add(this);
				Info.SellInfoList.Sort();
				Mobile.SendMessage("Your order has been added to the exchange board.");
			}

			else
				Mobile.SendMessage("Your order has been executed.");

			foreach (BuyInfo bi in toremove)
				Info.BuyInfoList.Remove(bi);

			foreach (Mobile m in sendhistory)
				ExchangeSystem.SendHistoryTo(m);
		}

		public override void Deactivate()
		{
			base.Deactivate();

			if (VerifyOwner)
			{
				int seizure = Math.Max((int)(Quantity * Config.SellGoodsSeizure), 1);
				Quantity -= seizure;

				Mobile.Backpack.DropItem(GetDeed());
			}

			Info.SellInfoList.Remove(this);
		}

		public int CompareTo(object o)
		{
			if (o is SellInfo)
			{
				SellInfo ae = (SellInfo)o;
				return (int)Math.Floor((Price - ae.Price));
			}
			else
				throw new ArgumentException("object is not SellInfo");
		}

		#region Ser/Deser
		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write(0);//version
		}

		public SellInfo(GenericReader reader):base(reader)
		{
			int version = reader.ReadInt();
		}
		#endregion
	}
}