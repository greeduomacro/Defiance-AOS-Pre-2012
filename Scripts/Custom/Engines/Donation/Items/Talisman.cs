namespace Server.Items
{
	public class Talisman : Item
	{
		public override string DefaultName{ get{ return "Defiance Talisman"; } }

		private string m_ForumAccount;

		[CommandProperty( AccessLevel.GameMaster )]
		public string ForumAccount
		{
			get{ return m_ForumAccount; }
			set{ m_ForumAccount = value; }
		}

		[Constructable]
		public Talisman() : base( 0x2F5B )
		{
			Movable = true;
			LootType = LootType.Blessed;
			Layer = Layer.Talisman;
		}

		public Talisman( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( (int) 0 ); // version
			writer.Write( (string) m_ForumAccount);
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
			m_ForumAccount = reader.ReadString();
		}
	}
}