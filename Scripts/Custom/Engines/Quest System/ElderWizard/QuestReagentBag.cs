using System;
using System.Collections.Generic;
using Server.Mobiles;

namespace Server.Items
{
	public class QuestReagentBag : Bag
	{
		public enum ReagentBagType
		{
			Mage,
			Necro
		}

		public static Type[] m_MageReagentTypes = new Type[]
		{
			typeof( BlackPearl ),
			typeof( Bloodmoss ),
			typeof( Garlic ),
			typeof( Ginseng ),
			typeof( MandrakeRoot ),
			typeof( Nightshade ),
			typeof( SulfurousAsh ),
			typeof( SpidersSilk )
		};

		public static Type[] m_NecroReagentTypes = new Type[]
		{
			typeof( BatWing ),
			typeof( GraveDust ),
			typeof( DaemonBlood ),
			typeof( NoxCrystal ),
			typeof( PigIron )
		};

		private ReagentBagType m_Type;
		[CommandProperty(AccessLevel.Administrator)]
		public ReagentBagType Type
		{
			get { return m_Type; }
			set
			{
				switch (value)
				{
					default:
					case ReagentBagType.Mage:
						Name = "Mage Reagent Bag";
						Hue = 0x1AF;
						break;
					case ReagentBagType.Necro:
						Name = "Necromancer Reagent Bag";
						Hue = 0x482;
						break;
				}

				m_Type = value;
			}
		}

		public Type[] ReagentTypes
		{
			get
			{
				switch (m_Type)
				{
					default:
					case ReagentBagType.Mage:
						return m_MageReagentTypes;
					case ReagentBagType.Necro:
						return m_NecroReagentTypes;
				}
			}
		}

		private int m_iBagRefillAmount;
		[CommandProperty(AccessLevel.Administrator)]
		public int BagRefillAmount
		{
			get { return m_iBagRefillAmount; }
			set { m_iBagRefillAmount = value; }
		}

		[Constructable]
		public QuestReagentBag()
			: this(ReagentBagType.Mage)
		{
		}

		public QuestReagentBag(ReagentBagType bagType)
			: this(bagType, QuestReagentBag.GetRandomRefillAmount())
		{
		}

		public QuestReagentBag(ReagentBagType bagType, int refillAmount)
		{
			Type = bagType;
			m_iBagRefillAmount = refillAmount;
			ERefillUtility.DirectRefill(this, ReagentTypes, BagRefillAmount);
		}

		private bool CheckItem(Mobile from, Item item)
		{
			if (item is BaseReagent)
				return true;

			from.SendLocalizedMessage(1042474); // The bag rejects that item.
			return false;
		}

		private static int GetRandomRefillAmount()
		{
			return 60 + (Utility.Random(7) * 10);
		}

		public override void GetProperties(ObjectPropertyList list)
		{
			base.GetProperties(list);

			list.Add(1070722, "(Refillable)");
			list.Add(1060660, "refill amount\t{0}", m_iBagRefillAmount); // ~1_val~: ~2_val~
		}

		public override bool OnDragDropInto(Mobile from, Item item, Point3D p)
		{
			if (CheckItem(from, item))
				return base.OnDragDrop(from, item);

			return false;
		}

		public override bool OnDragDrop(Mobile from, Item dropped)
		{
			if (CheckItem(from, dropped))
				return base.OnDragDrop(from, dropped);

			return false;
		}

		public QuestReagentBag(Serial serial)
			: base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write((int)2); // version

			// Version 2
			writer.Write((int)m_iBagRefillAmount);

			// Version 1
			writer.Write((int)m_Type);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();

			switch (version)
			{
				case 2:
					m_iBagRefillAmount = reader.ReadInt();
					goto case 1;
				case 1:
					m_Type = (ReagentBagType)reader.ReadInt();
					break;
			}

			if (version < 2)
				m_iBagRefillAmount = QuestReagentBag.GetRandomRefillAmount();
		}
	}
}