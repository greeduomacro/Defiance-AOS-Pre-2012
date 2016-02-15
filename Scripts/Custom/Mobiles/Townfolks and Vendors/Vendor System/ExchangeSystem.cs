using System;
using System.Collections.Generic;
using Server.Items;
using Server.Mobiles;

namespace Server.Exchange
{
	public static class ExchangeSystem
	{
		public static List<ExchangeCategory> CategoryList = new List<ExchangeCategory>(Config.Categories);

		public static Dictionary<Mobile, List<TransactionInfo>> TransactionHistory = new Dictionary<Mobile, List<TransactionInfo>>();

		public static void Configure()
		{
			CustomSaving.AddSaveModule(new SaveData(new DC.SaveMethod(Serialize), new DC.LoadMethod(Deserialize)), "Exchange System");
			Timer.DelayCall(TimeSpan.FromHours(13.0), TimeSpan.FromHours(13.0), new TimerCallback(CleanUp));
			EventSink.Login += new LoginEventHandler(OnLogin);
		}

		public static void OnLogin(LoginEventArgs e)
		{
			SendHistoryTo(e.Mobile);
		}

		public static void SendHistoryTo(Mobile m)
		{
			if (m != null && m.NetState != null)
			{
				List<TransactionInfo> lti;

				if (TransactionHistory.TryGetValue(m, out lti))
					m.SendGump(new TransactionConfirmationGump(lti));
			}
		}

		#region CleanUp
		public static void CleanUp()
		{
			//split serverload
			CleanBuyInfo();

			//split serverload
			Timer.DelayCall(TimeSpan.FromMinutes(5.0), new TimerCallback(CleanSellInfo));

			//split serverload
			Timer.DelayCall(TimeSpan.FromMinutes(10.0), new TimerCallback(CleanTransactionHistory));
		}

		private static void CleanTransactionHistory()
		{
			List<Mobile> toremove = new List<Mobile>();

			DateTime removetime = DateTime.Now - Config.HistoryCleanTime;
			if (TransactionHistory.Count > Config.HistoryCleanAmount)
			{
				foreach(KeyValuePair<Mobile, List<TransactionInfo>> kvp in TransactionHistory)
				{
					for (int i = kvp.Value.Count;  i >= 0; i--)
					{
						if(kvp.Value[i].Date < removetime)
							kvp.Value.RemoveAt(i);

						}
						if (kvp.Value.Count == 0)
							toremove.Add(kvp.Key);
						}
			}

			foreach (Mobile m in toremove)
				TransactionHistory.Remove(m);
		}

		private static void CleanBuyInfo()
		{
			DateTime removetime = DateTime.Now - Config.BidTime;
			List<BuyInfo> toremove = new List<BuyInfo>();

			foreach (ExchangeCategory ec in CategoryList)
			{
				foreach (ExchangeTypeInfo eti in ec.InfoList)
				{
					foreach (BuyInfo bi in eti.BuyInfoList)
					{
						if(bi.Date < removetime)
							toremove.Add(bi);
					}
				}
			}

			foreach (BuyInfo bi in toremove)
				bi.Deactivate();
		}

		private static void CleanSellInfo()
		{
			DateTime removetime = DateTime.Now - Config.SellTime;
			List<SellInfo> toremove = new List<SellInfo>();

			foreach (ExchangeCategory ec in CategoryList)
			{
				foreach (ExchangeTypeInfo eti in ec.InfoList)
				{
					foreach (SellInfo si in eti.SellInfoList)
					{
						if (si.Date < removetime)
							toremove.Add(si);
					}
				}
			}

			foreach (SellInfo si in toremove)
				si.Deactivate();
		}
		#endregion

		#region Ser/Deser
		public static void Serialize(GenericWriter writer)
		{
			writer.Write(0);//version

			int count = CategoryList.Count;
			writer.Write(count);
			for (int i = 0; i < count; i++)
			{
				writer.Write(CategoryList[i].ID);
				CategoryList[i].Serialize(writer);
			}

			count = TransactionHistory.Count;
			writer.Write(count);
			foreach(KeyValuePair<Mobile, List<TransactionInfo>> kvp in TransactionHistory)
			{
				writer.Write(kvp.Key);

				count = kvp.Value.Count;
				writer.Write(count);
				foreach (TransactionInfo ti in kvp.Value)
					ti.Serialize(writer);
			}
		}

		public static void Deserialize(GenericReader reader)
		{
			int version = reader.ReadInt();

			int count = reader.ReadInt();
			for (int i = 0; i < count; i++)
			{
				ExchangeCategory cat = null;
				int id = reader.ReadInt();
				foreach (ExchangeCategory ec in CategoryList)
					if (ec.ID == id)
					{
						cat = ec;
						break;
					}

				if (cat == null)
					cat = new ExchangeCategory(0, "readerror", 0);

				cat.Deserialize(reader);
			}

			count = reader.ReadInt();
			for (int i = 0; i < count; i++)
			{
				Mobile m = reader.ReadMobile();

				List<TransactionInfo> lti = new List<TransactionInfo>();
				int countlti = reader.ReadInt();
				for (int j = 0; j < countlti; j++)
					lti.Add(new TransactionInfo(reader));

				if (m != null && !m.Deleted)
					TransactionHistory.Add(m, lti);
			}
		}
		#endregion
	}

}