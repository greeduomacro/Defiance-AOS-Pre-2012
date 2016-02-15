using System;
using Server.Items;

namespace Server.Engines.RewardSystem
{
	public class TempRobe : Robe, ITempItem
	{
		private DateTime m_RemovalTime;
		private string m_PropertyString;

		public DateTime RemovalTime { get { return m_RemovalTime; } }
		public string PropertyString { get { return m_PropertyString; } set { m_PropertyString = value; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public int DaysLeft
		{
			get { return (int)(m_RemovalTime - DateTime.Now).TotalDays; }
			set
			{
				m_RemovalTime = DateTime.Now + TimeSpan.FromDays(Math.Min(value, 365));
				TemporaryItemSystem.Verify(this);
			}
		}

		[Constructable]
		public TempRobe()
			: this(30)
		{
		}

		[Constructable]
		public TempRobe(int days)
			: base()
		{
			m_RemovalTime = DateTime.Now + TimeSpan.FromDays(Math.Min(days, 365));
			TemporaryItemSystem.Verify(this);
		}

		public override void GetProperties(ObjectPropertyList list)
		{
			base.GetProperties(list);
			list.Add(m_PropertyString);
		}

		public TempRobe(Serial serial)
			: base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(m_RemovalTime);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			m_RemovalTime = reader.ReadDateTime();
			TemporaryItemSystem.Verify(this);
		}
	}
}