using System;
using Server.Mobiles;
using Server.Spells;

namespace Server.Engines.RewardSystem
{
	public class TempHorseEthereal : EtherealHorse, ITempItem
	{
		private DateTime m_RemovalTime;
		private string m_PropertyString;

		public DateTime RemovalTime { get { return m_RemovalTime; } }
		public string PropertyString { get { return m_PropertyString; } set { m_PropertyString = value; } }

		[Constructable]
		public TempHorseEthereal(int days)
			: base()
		{
			Name = "No age ethereal horse";
			m_RemovalTime = DateTime.Now + TimeSpan.FromDays(days);
			TemporaryItemSystem.Verify(this);
		}

		public override void GetProperties(ObjectPropertyList list)
		{
			base.GetProperties(list);
			list.Add(m_PropertyString);
		}

		public TempHorseEthereal(Serial serial)
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