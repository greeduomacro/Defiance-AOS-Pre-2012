using System;
using System.Collections.Generic;
using Server.Mobiles;

namespace Server.Items
{
	public class BankWithdrawStone : Item
	{
		#region CommandProperties
		private Dictionary<Mobile, int> m_AmountList = new Dictionary<Mobile,int>();

		private bool m_Active;
		[CommandProperty(AccessLevel.GameMaster)]
		public bool Active
		{
			get { return m_Active; }
			set { m_Active = value; InvalidateProperties(); }
		}

		private bool m_UseLimit;
		[CommandProperty(AccessLevel.GameMaster)]
		public bool UseLimit
		{
			get { return m_UseLimit; }
			set { m_UseLimit = value; UpdateName(); InvalidateProperties(); }
		}

		private int m_Limit;
		[CommandProperty(AccessLevel.GameMaster)]
		public int Limit
		{
			get { return m_Limit; }
			set { m_Limit = value; InvalidateProperties(); }
		}

		private int m_GoldAmount;
		[CommandProperty(AccessLevel.GameMaster)]
		public int GoldAmount
		{
			get { return m_GoldAmount; }
			set { m_GoldAmount = value; InvalidateProperties(); }
		}
		#endregion

		[Constructable]
		public BankWithdrawStone()
			: base(0xED4)
		{
			Movable = false;
			Hue = 0x2D1;

			m_Active = true;
			m_UseLimit = true;
			m_Limit = 1;
			UpdateName();
		}

		private void UpdateName()
		{
			string name = m_UseLimit ? "a limited " : "a ";

			Name = name + String.Format("{0} gp withdrawal stone", m_GoldAmount);
		}

		public override void GetProperties(ObjectPropertyList list)
		{
			base.GetProperties(list);

			if (m_Active)
				list.Add(1060742); // active
			else
				list.Add(1060743); // inactive
		}

		public override void OnDoubleClick(Mobile from)
		{
			if (!m_Active || m_GoldAmount <= 0)
			{
				from.SendMessage("This stone is disabled.");
				return;
			}

			if (!from.InRange(GetWorldLocation(), 2))
			{
				from.SendLocalizedMessage(500446); // That is too far away.
				return;
			}

			if (m_UseLimit && m_AmountList.ContainsKey(from) && m_AmountList[from] >= m_Limit)
			{
				from.SendMessage("You cannot take anymore use this stone another time.");
				return;
			}

			if (Banker.Withdraw(from, m_GoldAmount))
			{
				from.SendMessage(string.Format("Your new bank balance is: {0}gp.", Banker.GetBalance( from ).ToString()));
				if (m_GoldAmount < 1000)
					from.AddToBackpack(new Gold(m_GoldAmount));

				else
					from.AddToBackpack(new BankCheck(m_GoldAmount));

				if (m_UseLimit)
				{
					if (m_AmountList.ContainsKey(from))
						m_AmountList[from]++;
					else
						m_AmountList.Add(from, 1);
				}
			}

			else
			{
				from.SendMessage("You do not have enough money in your bank to withdraw this amount.");
			}
		}

		public BankWithdrawStone(Serial serial)
			: base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.WriteEncodedInt((int)0); // version

			// Version 0
			writer.Write(m_Limit);
			writer.Write(m_UseLimit);
			writer.Write(m_GoldAmount);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadEncodedInt();

			m_Limit = reader.ReadInt();
			m_UseLimit = reader.ReadBool();
			m_GoldAmount = reader.ReadInt();
			UpdateName();
		}
	}
}