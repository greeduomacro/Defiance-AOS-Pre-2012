namespace Server.Items
{
	public class DonationItem : Item
	{
		private bool m_Donation;

		[CommandProperty(AccessLevel.Administrator)]
		public bool Donation
		{
			get { return m_Donation; }
			set { m_Donation = value; InvalidateProperties(); }
		}

		public DonationItem()
			: base()
		{
		}

		public DonationItem(int itemID)
			: this(itemID, false)
		{
		}

		public DonationItem(int itemID, bool donated)
			: base(itemID)
		{
			m_Donation = donated;
		}

		public override void GetProperties(ObjectPropertyList list)
		{
			base.GetProperties(list);

			if (m_Donation)
				list.Add(1050039, "\t{0}\t", "Donation Item");
		}

		public DonationItem(Serial serial)
			: base(serial)
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
			writer.Write( m_Donation );
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();
			m_Donation = reader.ReadBool();
		}
	}
}