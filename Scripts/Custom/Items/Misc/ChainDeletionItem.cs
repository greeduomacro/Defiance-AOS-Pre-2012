using System;

namespace Server.Items
{
	public class ChainDeletionItem : Static
	{
		private Item m_ChainedItem1, m_ChainedItem2, m_ChainedItem3, m_ChainedItem4;
		private Mobile m_ChainedMobile1, m_ChainedMobile2;

		[CommandProperty( AccessLevel.GameMaster )]
		public Item ChainedItem1
		{
			get{ return m_ChainedItem1; }
			set{ m_ChainedItem1 = value; InvalidateProperties(); }
		}
		[CommandProperty( AccessLevel.GameMaster )]
		public Item ChainedItem2
		{
			get{ return m_ChainedItem2; }
			set{ m_ChainedItem2 = value; InvalidateProperties(); }
		}
		[CommandProperty( AccessLevel.GameMaster )]
		public Item ChainedItem3
		{
			get{ return m_ChainedItem3; }
			set{ m_ChainedItem3 = value; InvalidateProperties(); }
		}
		[CommandProperty( AccessLevel.GameMaster )]
		public Item ChainedItem4
		{
			get{ return m_ChainedItem4; }
			set{ m_ChainedItem4 = value; InvalidateProperties(); }
		}
		[CommandProperty( AccessLevel.GameMaster )]
		public Mobile ChainedMobile1
		{
			get{ return m_ChainedMobile1; }
			set{ m_ChainedMobile1 = value; InvalidateProperties(); }
		}
		[CommandProperty( AccessLevel.GameMaster )]
		public Mobile ChainedMobile2
		{
			get{ return m_ChainedMobile2; }
			set{ m_ChainedMobile2 = value; InvalidateProperties(); }
		}

		[Constructable]
		public ChainDeletionItem() : base( 4112 )
		{
			LootType = LootType.Blessed;
		}

		public override void OnDelete()
		{
			if( m_ChainedItem1 != null && !m_ChainedItem1.Deleted )
				m_ChainedItem1.Delete();

			if( m_ChainedItem2 != null && !m_ChainedItem2.Deleted )
				m_ChainedItem2.Delete();

			if( m_ChainedItem3 != null && !m_ChainedItem3.Deleted )
				m_ChainedItem3.Delete();

			if( m_ChainedItem4 != null && !m_ChainedItem4.Deleted )
				m_ChainedItem4.Delete();

			if( m_ChainedMobile1 != null && !m_ChainedMobile1.Deleted )
				m_ChainedMobile1.Kill();

			if( m_ChainedMobile2 != null && !m_ChainedMobile2.Deleted )
				m_ChainedMobile2.Kill();

			base.OnDelete();
		}

		public ChainDeletionItem( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );

			writer.Write( (Item) m_ChainedItem1 );
			writer.Write( (Item) m_ChainedItem2 );
			writer.Write( (Item) m_ChainedItem3 );
			writer.Write( (Item) m_ChainedItem4 );
			writer.Write( (Mobile) m_ChainedMobile1 );
			writer.Write( (Mobile) m_ChainedMobile2 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			m_ChainedItem1 = reader.ReadItem();
			m_ChainedItem2 = reader.ReadItem();
			m_ChainedItem3 = reader.ReadItem();
			m_ChainedItem4 = reader.ReadItem();
			m_ChainedMobile1 = reader.ReadMobile();
			m_ChainedMobile2 = reader.ReadMobile();
		}
	}
}