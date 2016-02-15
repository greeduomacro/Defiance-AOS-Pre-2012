using Server.Accounting;

namespace Server.Items
{
	public class SubscriberEarrings : BaseEarrings
	{
		private string m_Account = string.Empty;

		[CommandProperty(AccessLevel.GameMaster)]
		public Mobile Account_Set
		{
			get { return null; }

			set
			{
				if (value == null)
					m_Account = string.Empty;

				else
					m_Account = value.Account.Username;
				InvalidateProperties();
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public string Account_Get
		{
			get { return m_Account; }
		}

		[Constructable]
		public SubscriberEarrings() : base( 0x1087 )
		{
			Weight = 0.1;
			Hue = 1161;
			Name = "Subscriber Earrings";
			LootType = LootType.Blessed;
		}

		public SubscriberEarrings(Serial serial)
			: base(serial)
		{
		}

		public override void GetProperties(ObjectPropertyList list)
		{
			base.GetProperties(list);

			list.Add(1041522, "\t{0}\t\t", "Bound to Account");
		}

		public override bool OnEquip(Mobile from)
		{
			if (m_Account != from.Account.Username)
			{
				from.SendMessage("This item is bound to a different account, thus you cannot wear it.");
				return false;
			}

			return base.OnEquip(from);
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
			writer.Write(m_Account);
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
			m_Account = reader.ReadString();
		}
	}
}