using System;

namespace Server.Exchange
{
	public class ExchangeDay
	{
		public double HighestPrice;
		public double LowestPrice;
		public long TotalQuantity;
		public long TotalRevenue;
		public double Average;
		public int Day;

		public ExchangeDay(int day, double price, int quantity, long revenue)
		{
			Day = day;
			LowestPrice = double.MaxValue;
			AddExchange(price, quantity, revenue);
		}

		public void AddExchange(double price, int quantity, long revenue)
		{
			TotalRevenue += revenue;
			TotalQuantity += quantity;
			HighestPrice = Math.Max(HighestPrice, price);
			LowestPrice = Math.Min(LowestPrice, price);
			Average = Math.Round((double)TotalRevenue / TotalQuantity,2);
		}

		#region Ser/Deser
		public void Serialize(GenericWriter writer)
		{
			writer.Write(0);//version

			writer.Write(HighestPrice);
			writer.Write(LowestPrice);
			writer.Write(TotalQuantity);
			writer.Write(TotalRevenue);
			writer.Write(Average);
			writer.Write(Day);
		}

		public ExchangeDay(GenericReader reader)
		{
			int version = reader.ReadInt();

			HighestPrice = reader.ReadDouble();
			LowestPrice = reader.ReadDouble();
			TotalQuantity = reader.ReadLong();
			TotalRevenue = reader.ReadLong();
			Average = reader.ReadDouble();
			Day = reader.ReadInt();
		}
		#endregion
	}
}