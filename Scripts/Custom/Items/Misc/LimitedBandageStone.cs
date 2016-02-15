using System;
using Server.Items;
using System.Collections;

namespace Server.Items
{
	public class LimitedBandageStone : Item
	{
		private ArrayList m_alNameList;

		[Constructable]
		public LimitedBandageStone() : base( 0xED4 )
		{
			Movable = false;
			Hue = 0x2D1;
			Name = "a limited bandage stone";
			m_alNameList = new ArrayList();
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( !from.InRange( GetWorldLocation(), 2 ) )
			{
				from.SendLocalizedMessage( 500446 ); // That is too far away.
				return;
			}

			foreach( string str in m_alNameList )
				if( str == from.Account.ToString() )
				{
					from.SendMessage( "You may not take anymore bandages." );
					return;
				}

			if ( from.AddToBackpack( new Bandage( 200 ) ) )
				m_alNameList.Add( from.Account.ToString() );
		}

		public LimitedBandageStone( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
			m_alNameList = new ArrayList();
		}
	}
}