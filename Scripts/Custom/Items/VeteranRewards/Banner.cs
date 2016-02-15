using System;
using Server;
using Server.Gumps;
using Server.Network;

namespace Server.Items
{
	public class Banner : BaseAddon
	{
		public override BaseAddonDeed Deed{ get{ return new BannerDeed(); } }

		[Constructable]
		public Banner( int itemID )
		{
			AddComponent( new AddonComponent( itemID ), 0, 0, 0 );
		}

		public Banner( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}

	public class BannerDeed : BaseAddonDeed
	{
		private int m_ItemID;

		public override BaseAddon Addon{ get{ return new Banner( m_ItemID ); } }
		public override int LabelNumber{ get{ return 1041007; } } // A Banner Deed

		[Constructable]
		public BannerDeed()
		{
			LootType = LootType.Blessed;
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( IsChildOf( from.Backpack ) )
			{
				from.CloseGump( typeof( BannerGump ) );
				from.CloseGump( typeof( BannerFaceGump ) );
				from.SendGump( new BannerGump( this, 0 ) );
			}
			else
			{
				from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
			}
		}

		private void SendTarget( Mobile m )
		{
			base.OnDoubleClick( m );
		}

		public class BannerGump : Gump
		{
			private BannerDeed m_Deed;
			public int m_iLoc;
			public virtual int AmountToGet{ get{ return 8; } }
			public virtual int FirstItemID{ get{ return 5550; } }
			public virtual int LastItemID{ get{ return 5621; } }

			public BannerGump( BannerDeed deed, int loc ) : base( 150, 50 )
			{
				m_Deed = deed;
				m_iLoc = loc;

				AddPage(0);
				AddBackground( 0, 0, 500, 230, 2600);
				AddLabel( 45, 15, 1152, "Choose a Banner:");

				if( loc+FirstItemID+(AmountToGet*2) < LastItemID )
					AddButton(430, 190, 5601, 5605, (int)Buttons.Next, GumpButtonType.Reply, 0);

				if( loc+FirstItemID > FirstItemID )
					AddButton(50, 190, 5603, 5607, (int)Buttons.Prev, GumpButtonType.Reply, 0);

				for ( int i = 0; i < AmountToGet*2; i+=2 )
				{
					if( i+loc+FirstItemID > LastItemID )
						break;

					int positionLoc = i > 0 ? positionLoc = i/2 : positionLoc = 0;

					AddButton( 30 + (60*positionLoc), 50, 2117, 2118, i+loc+FirstItemID, GumpButtonType.Reply, 0 );
					AddItem( 15 + (60*positionLoc), 70, i+loc+FirstItemID );
				}
			}

			public enum Buttons
			{
				CloseButton,
				Next,
				Prev
			}

			public virtual void OnRequestGump( Mobile from, BannerDeed deed, int loc )
			{
				from.SendGump( new BannerGump( deed, loc ) );
			}

			public override void OnResponse( Server.Network.NetState sender, RelayInfo info )
			{
				Mobile from = sender.Mobile;

				if ( info.ButtonID >= FirstItemID )
				{
					from.CloseGump( typeof ( BannerGump ) );
					from.CloseGump( typeof ( BannerFaceGump ) );
					from.SendGump( new BannerFaceGump( info.ButtonID, m_Deed ) );
					return;
				}

				switch ( info.ButtonID )
				{
					case (int)Buttons.CloseButton:
						return;
					case (int)Buttons.Next:
						OnRequestGump( from, m_Deed, m_iLoc + AmountToGet*2 );
						return;
					case (int)Buttons.Prev:
						OnRequestGump( from, m_Deed, m_iLoc - AmountToGet*2 );
						return;
				}
			}
		}

		public class BannerFaceGump : Gump
		{
			private int m_Id;
			private BannerDeed m_Deed;

			public BannerFaceGump( int id, BannerDeed deed ) : base( 150, 50 )
			{
				m_Id = id;
				m_Deed = deed;

				AddPage(0);
				AddBackground( 0, 2, 300, 150, 2600 );
				AddButton( 50, 40, 2151, 2153, 2, GumpButtonType.Reply, 0 );
				AddItem( 90, 35, id+1 );
				AddButton( 150, 40, 2151, 2153, 1, GumpButtonType.Reply, 0 );
				AddItem( 180, 35, id );
			}

			public override void OnResponse( Server.Network.NetState sender, RelayInfo info )
			{
				if ( m_Deed.Deleted || info.ButtonID == 0 )
					return;

				m_Deed.m_ItemID = m_Id+(info.ButtonID-1);
				m_Deed.SendTarget( sender.Mobile );
			}
		}

		public BannerDeed( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}
}