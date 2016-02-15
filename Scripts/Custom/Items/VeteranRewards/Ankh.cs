using System;
using Server;
using Server.Gumps;
using Server.Network;

namespace Server.Items
{
	public class Ankh : BaseAddon
	{
		public override BaseAddonDeed Deed{ get{ return new AnkhDeed(); } }

		[Constructable]
		public Ankh( bool east )
		{
			if ( east )
			{
				AddLightComponent( new AddonComponent( 0x4 ), 0, 0, 0 );
				AddLightComponent( new AddonComponent( 0x5 ), 1, 0, 0 );
			}
			else
			{
				AddLightComponent( new AddonComponent( 0x3 ), 0, 0, 0 );
				AddLightComponent( new AddonComponent( 0x2 ), 0, 1, 0 );
			}
		}

		private void AddLightComponent( AddonComponent component, int x, int y, int z )
		{
			component.Light = LightType.Circle150;

			AddComponent( component, x, y, z );
		}

		public Ankh( Serial serial ) : base( serial )
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

	public class AnkhDeed : BaseAddonDeed
	{
		private bool m_East;

		public override BaseAddon Addon{ get{ return new Ankh( m_East ); } }

		public override int LabelNumber{ get{ return 1049773; } }

		[Constructable]
		public AnkhDeed()
		{
			LootType = LootType.Blessed;
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( IsChildOf( from.Backpack ) )
			{
				from.CloseGump( typeof( InternalGump ) );
				from.SendGump( new InternalGump( this ) );
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

		private class InternalGump : Gump
		{
			private AnkhDeed m_Deed;

			public InternalGump( AnkhDeed deed ) : base( 150, 50 )
			{
				m_Deed = deed;

				AddBackground( 0, 0, 350, 250, 0xA28 );

				AddItem( 90, 35, 0x2 );
				AddItem( 112, 35, 0x3 );
				AddButton( 70, 35, 0x868, 0x869, 1, GumpButtonType.Reply, 0 );

				AddItem( 220, 35, 0x4 );
				AddItem( 242, 35, 0x5 );
				AddButton( 185, 35, 0x868, 0x869, 2, GumpButtonType.Reply, 0 );
			}

			public override void OnResponse( NetState sender, RelayInfo info )
			{
				if ( m_Deed.Deleted || info.ButtonID == 0 )
					return;

				m_Deed.m_East = (info.ButtonID != 1);
				m_Deed.SendTarget( sender.Mobile );
			}
		}

		public AnkhDeed( Serial serial ) : base( serial )
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