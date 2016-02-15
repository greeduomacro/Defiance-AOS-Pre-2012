using System;
using Server;
using Server.Multis;
using Server.Targeting;

namespace Server.Items
{
	public class PigmentofTokuno : Item
	{
		public class PigmentEntry
		{
			public int m_hue;
			public int m_cliloc;
			public PigmentEntry( int hue, int cliloc )
			{
				m_hue = hue;
				m_cliloc = cliloc;
			}
		}

		private PigmentEntry[] m_PigmentTypes = new PigmentEntry[]
		{
			new PigmentEntry( 0x501,	1070987 ), // Paragon Gold
			new PigmentEntry( 0x486,	1070988 ), // Violet Courage Purple
			new PigmentEntry( 0x4F2,	1070989 ), // Invulnerability Blue
			new PigmentEntry( 0x47E,	1070990 ), // Luna White
			new PigmentEntry( 1167,		1070991 ), // Dryad Green
			new PigmentEntry( 0x455,	1070992 ), // Shadow Dancer Black
			new PigmentEntry( 0x21,		1070993 ), // Berserker Red
			new PigmentEntry( 0x58C,	1070994 ), // Nox Green
			new PigmentEntry( 1645,		1070995 ), // Rum Red
			new PigmentEntry( 0x489,	1070996 )  // Fire Orange
		};

		public override int LabelNumber{ get{ return 1070933; } } // Pigments of Tokuno

		private int m_Charges;
		[CommandProperty( AccessLevel.GameMaster )]
		public int Charges
		{
			get{ return m_Charges; }
			set{ m_Charges = value; InvalidateProperties(); }
		}

		private int m_iCliloc;
		[CommandProperty( AccessLevel.GameMaster )]
		public int Cliloc
		{
			get{ return m_iCliloc; }
			set{ m_iCliloc = value; InvalidateProperties(); }
		}

//		[Constructable]
		public PigmentofTokuno() : this( 5 )
		{
		}

//		[Constructable]
		public PigmentofTokuno( int charges ) : base( 0xE27 )
		{
			PigmentEntry entry = m_PigmentTypes[Utility.Random(m_PigmentTypes.Length)];
			Hue = entry.m_hue;
			m_iCliloc = entry.m_cliloc;
			Weight = 1.0;
			m_Charges = charges;
		}

		public PigmentofTokuno( Serial serial ) : base( serial )
		{
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );

			list.Add( m_iCliloc );
			list.Add( 1060584, m_Charges.ToString() ); // uses remaining: ~1_val~
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( IsChildOf( from.Backpack ) )
			{
				from.SendLocalizedMessage( 1070929 ); // Select the artifact or enhanced magic item to dye.
				from.Target = new InternalTarget( this );
			}
			else
				from.SendLocalizedMessage( 1042010 ); // You must have the object in your backpack to use it.
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 ); // version
			writer.Write( (int) m_Charges );
			writer.Write( (int) m_iCliloc );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
			switch ( version )
			{
				case 1:
				{
					m_Charges = reader.ReadInt();
					m_iCliloc = reader.ReadInt();
					break;
				}
			}
		}

		private class InternalTarget : Target
		{
			private PigmentofTokuno m_Pigment;

			public InternalTarget( PigmentofTokuno pigment ) : base( 1, false, TargetFlags.None )
			{
				m_Pigment = pigment;
			}

			protected override void OnTarget( Mobile from, object targeted )
			{
				if ( targeted is Item )
				{
					Item i = (Item)targeted;

					if( i == null )
						from.SendLocalizedMessage( 1070931 ); // You can only dye artifacts and enhanced magic items with this tub.
					else if( !from.InRange( i.GetWorldLocation(), 3 ) || !m_Pigment.IsAccessibleTo( from ) )
						from.SendLocalizedMessage( 502436 ); // That is not accessible.
					else if( from.Items.Contains( i ) )
						from.SendLocalizedMessage( 1070930 ); // Can't dye artifacts or enhanced magic items that are being worn.
					else if( i.IsLockedDown )
						from.SendLocalizedMessage( 1070932 ); // You may not dye artifacts and enhanced magic items which are locked down.
					else if( i is PigmentofTokuno )
						from.SendLocalizedMessage( 1042417 ); // You cannot dye that.
					else if( !PigmentsOfTokuno.IsValidItem( i ) || ( (i is BodySash) && (i.Layer == Layer.MiddleTorso) ) )
						from.SendLocalizedMessage( 1070931 ); // You can only dye artifacts and enhanced magic items with this tub.
					else
					{
						i.Hue = m_Pigment.Hue;
						from.PlaySound( 0x23F );
						from.SendLocalizedMessage( 1062623 ); // You dye the item.

						if( m_Pigment.Charges > 1 )
							m_Pigment.Charges--;
						else
						{
							m_Pigment.Delete();
							from.SendLocalizedMessage( 500858 ); // You used up the dye.
						}
					}
				}
			}
		}
	}
}