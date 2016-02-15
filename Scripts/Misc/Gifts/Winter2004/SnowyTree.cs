using System;
using Server;
using Server.Network;

namespace Server.Items
{
	public class SnowyTree : Item
	{
		string m_Label;

		[CommandProperty( AccessLevel.GameMaster )]
		public string Label{ get{ return m_Label; } set{ m_Label = value; } }

		[Constructable]
		public SnowyTree() : base( 0x2377 )
		{
			Weight = 1.0;
			LootType = LootType.Blessed;
			m_Label = "December 2007";
		}

		public SnowyTree( Serial serial ) : base( serial )
		{
		}

		public override void OnSingleClick( Mobile from )
		{
			base.OnSingleClick( from );

			LabelTo( from, m_Label );
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );

			list.Add( m_Label  );
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 ); // version
			writer.Write( (string) m_Label );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 1:
				{
					m_Label = reader.ReadString();
					break;
				}
				case 0:
				{
					m_Label = "December 2007";
					break;
				}
			}
		}
	}
}