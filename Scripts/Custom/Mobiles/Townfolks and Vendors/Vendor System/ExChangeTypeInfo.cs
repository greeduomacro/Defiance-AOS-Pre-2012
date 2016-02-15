using System;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Items;

namespace Server.Exchange
{
	public class ExchangeTypeInfo
	{
		public Type Type;
		public string Name;

		public ExchangeCategory Category;

		public int LastQuantity;
		public double LastPrice;
		public DateTime LastExchangeTime;

		public double AveragePrice;

		public long HighestDayQuantity;
		public double HighestDayPrice;
		public double LowestDayPrice;

		public double HighestPrice;
		public double LowestPrice;
		public long TotalQuantity;
		public long TotalRevenue;

		public List<ExchangeDay> ExchangeDayList = new List<ExchangeDay>();
		public ExchangeDay CurrentDay;

		public List<BuyInfo> BuyInfoList = new List<BuyInfo>();
		public List<SellInfo> SellInfoList = new List<SellInfo>();

		private string[] m_SalesInfo1 = new string[]
				{
					"Last:", "N/A",
					"Last-P:", "N/A",
					"Last-Q:", "N/A",
				};
		private string[] m_SalesInfo2 = new string[]
				{
					"Highest-P:", "N/A",
					"Lowest-P:", "N/A",
					"Average-P:", "N/A",
					"Total-Q:", "N/A",
					"Total-R:", "N/A"
				};

		public string[] GetSalesInfo1
		{
			get
			{
				TimeSpan span = DateTime.Now - LastExchangeTime;
				string time = String.Format("±{0} h ago", (int)Math.Ceiling(span.TotalHours));
				m_SalesInfo1[1] = (LastExchangeTime == DateTime.MinValue) ? "N/A" : time;
				return m_SalesInfo1;
			}
		}
		public string[] GetSalesInfo2 { get { return m_SalesInfo2; } }


		public ExchangeTypeInfo(Type type, string name)
		{
			Type = type;
			Name = name;
			LowestDayPrice = double.MaxValue;
			LowestPrice = double.MaxValue;
		}

		public void ActivateExchange(int quantity, double price)
		{
			long revenue = (int)(price * quantity);
			DateTime now = DateTime.Now;

			if (CurrentDay == null || now.Day != CurrentDay.Day)
			{
				CurrentDay = new ExchangeDay(now.Day, price, quantity, revenue);
				ExchangeDayList.Add(CurrentDay);

				if (ExchangeDayList.Count > 20)
				{
					bool hdq = false;
					bool lp = false;
					bool hp = false;
					ExchangeDay oldday = ExchangeDayList[0];

					TotalQuantity -= oldday.TotalQuantity;
					TotalRevenue -= oldday.TotalRevenue;

					if (oldday.TotalQuantity == HighestDayQuantity)
					{
						hdq = true;
						HighestDayQuantity = 0;
					}

					if (oldday.HighestPrice == HighestDayPrice)
					{
						hp = true;
						HighestDayPrice = 0;
					}

					if (oldday.LowestPrice == LowestPrice)
					{
						lp = true;
						LowestPrice = double.MaxValue;
					}

					ExchangeDayList.RemoveAt(0);

					foreach (ExchangeDay ed in ExchangeDayList)
					{
						if (hdq)
							HighestDayQuantity = Math.Max(HighestDayQuantity, ed.TotalQuantity);
						if (hp)
							HighestPrice = Math.Max(HighestPrice, ed.HighestPrice);
						if (lp)
							LowestPrice = Math.Min(LowestPrice, ed.LowestPrice);
					}
				}
			}
			else
				CurrentDay.AddExchange(price, quantity, revenue);

			LastPrice = price;
			LastQuantity = quantity;
			LastExchangeTime = now;

			TotalQuantity += quantity;
			TotalRevenue += revenue;
			HighestPrice = Math.Max(HighestPrice, price);
			LowestPrice = Math.Min(LowestPrice, price);
			AveragePrice = Math.Round((double)TotalRevenue / TotalQuantity,2);

			HighestDayPrice = 0;
			LowestDayPrice = double.MaxValue;
			foreach (ExchangeDay ed in ExchangeDayList)
			{
				HighestDayPrice = Math.Max(HighestDayPrice, ed.Average);
				LowestDayPrice = Math.Min(LowestDayPrice, ed.Average);
			}

			HighestDayQuantity = Math.Max(HighestDayQuantity, CurrentDay.TotalQuantity);

			m_SalesInfo1 = new string[]
				{
					"Last:", "",
					"Last-P:",LastPrice.ToString(),
					"Last-Q:", LastQuantity.ToString()
				};

			m_SalesInfo2 = new string[]
				{
					"Highest-P:", HighestPrice.ToString(),
					"Lowest-P:", LowestPrice.ToString(),
					"Average-P:", AveragePrice.ToString(),
					"Total-Q:", TotalQuantity.ToString(),
					"Total-R:", TotalRevenue.ToString()
				};
		}

		#region Ser/Deser
		public void Serialize(GenericWriter writer)
		{
			writer.Write(0);//version

			//Category set by the collections this belongs to

			writer.Write(LastExchangeTime);
			writer.Write(LastPrice);
			writer.Write(LastQuantity);

			writer.Write(AveragePrice);

			writer.Write(HighestDayQuantity);
			writer.Write(HighestDayPrice);
			writer.Write(LowestDayPrice);

			writer.Write(HighestPrice);
			writer.Write(LowestPrice);
			writer.Write(TotalQuantity);

			int count = ExchangeDayList.Count;
			writer.Write(count);
			for (int i = 0; i < count; i++)
				ExchangeDayList[i].Serialize(writer);

			count = BuyInfoList.Count;
			writer.Write(count);
			for (int i = 0; i < count; i++)
				BuyInfoList[i].Serialize(writer);

			count = SellInfoList.Count;
			writer.Write(count);
			for (int i = 0; i < count; i++)
				SellInfoList[i].Serialize(writer);

			CustomSaving.SerializeStringArray(m_SalesInfo1, writer);
			CustomSaving.SerializeStringArray(m_SalesInfo2, writer);
		}

		public void Deserialize(GenericReader reader)
		{
			int version = reader.ReadInt();

			//Category set by the collections this belongs to
			LastExchangeTime = reader.ReadDateTime();
			LastPrice = reader.ReadDouble();
			LastQuantity = reader.ReadInt();

			AveragePrice = reader.ReadDouble();

			HighestDayQuantity = reader.ReadLong();
			HighestDayPrice = reader.ReadDouble();
			LowestDayPrice = reader.ReadDouble();

			HighestPrice = reader.ReadDouble();
			LowestPrice = reader.ReadDouble();
			TotalQuantity = reader.ReadLong();

			int count = reader.ReadInt();
			for (int i = 0; i < count; i++)
			{
				ExchangeDay ed = new ExchangeDay(reader);
				ExchangeDayList.Add(ed);

				if (i == count - 1)
					CurrentDay = ed;
			}

			count = reader.ReadInt();
			for (int i = 0; i < count; i++)
			{
				BuyInfo bi = new BuyInfo(reader);
				BuyInfoList.Add(bi);
				bi.Info = this;
			}

			count = reader.ReadInt();
			for (int i = 0; i < count; i++)
			{
				SellInfo si= new SellInfo(reader);
				SellInfoList.Add(si);
				si.Info = this;
			}

			m_SalesInfo1 = CustomSaving.DeserializeStringArray(reader);
			m_SalesInfo2 = CustomSaving.DeserializeStringArray(reader);
		}
		#endregion
	}
}