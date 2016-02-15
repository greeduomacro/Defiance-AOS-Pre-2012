using System;
using System.Collections.Generic;

namespace Server.Exchange
{
	public class TransactionInfo
	{
		public double Price;
		public int Quantity;
		public string Name;
		public DateTime Date;
		public bool Buyer;

		public TransactionInfo(string name, double price, int quantity, bool buyer)
		{
			Price = price;
			Quantity= quantity;
			Name = name;
			Buyer = buyer;
			Date = DateTime.Now;
		}

		#region Ser/Deser
		public virtual void Serialize(GenericWriter writer)
		{
			writer.Write(0);//version

			writer.Write(Price);
			writer.Write(Quantity);
			writer.Write(Name);
			writer.Write(Buyer);
			writer.Write(Date);
		}

		public TransactionInfo(GenericReader reader)
		{
			int version = reader.ReadInt();

			Price = reader.ReadDouble();
			Quantity = reader.ReadInt();
			Name = reader.ReadString();
			Buyer = reader.ReadBool();
			Date = reader.ReadDateTime();
		}
		#endregion
	}
}