using System;
using Server;
using Server.Multis;
using Server.Targeting;

namespace Server.Items
{
	public class LesserPigmentofTokuno : Item
	{
		public override int LabelNumber{ get{ return 1070933; } } // Pigments of Tokuno
		public virtual int MaxCharges{ get{ return 10; } }

		private int m_Charges;
		[CommandProperty( AccessLevel.GameMaster )]
		public int Charges
		{
			get{ return m_Charges; }
			set{ m_Charges = value; InvalidateProperties(); }
		}

//		[Constructable]
		public LesserPigmentofTokuno() : base( 0xE27 )
		{
			Weight = 1.0;
			m_Charges = MaxCharges;
		}

		public LesserPigmentofTokuno( Serial serial ) : base( serial )
		{
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );

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
					break;
				}
				case 0:
				{
					m_Charges = reader.ReadInt();
					bool m_Repig = reader.ReadBool();
					int m_PigHue = reader.ReadInt();
					break;
				}
			}
		}

		private class InternalTarget : Target
		{
			private LesserPigmentofTokuno m_Pigment;

			public InternalTarget( LesserPigmentofTokuno pigment ) : base( 1, false, TargetFlags.None )
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
					else if( !PigmentsOfTokuno.IsValidItem( i ) )
						from.SendLocalizedMessage( 1070931 ); // You can only dye artifacts and enhanced magic items with this tub.
					else
					{
						i.Hue = 0;
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