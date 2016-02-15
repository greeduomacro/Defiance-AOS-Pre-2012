using System;
using System.Collections;
using System.Collections.Generic;
using Server.ContextMenus;

namespace Server.Items
{
	public class LockedFruitBasket : Item
	{
		public override bool Decays{get{return false;}}

		[Constructable]
		public LockedFruitBasket() : base( 0x993 )
		{
			Weight = 2.0;
			Stackable = false;
			Movable = false;
		}

		public LockedFruitBasket( Serial serial ) : base( serial )
		{
		}

		public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
		{
			base.GetContextMenuEntries( from, list );

			if ( from.Alive )
				list.Add( new EatEntry( from, this ) );
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( from.InRange( this.GetWorldLocation(), 1 ) )
			{
				// Fill the Mobile with FillFactor
				if ( Food.FillHunger( from, 5 ) )
				{
					// Play a random "eat" sound
					from.PlaySound( Utility.Random( 0x3A, 3 ) );

					if ( from.Body.IsHuman && !from.Mounted )
						from.Animate( 34, 5, 1, true, false, 0 );

					new Basket().MoveToWorld( this.Location, this.Map );
					Consume();
				}
			}
		}

		private class EatEntry : ContextMenuEntry
		{
			private Mobile m_From;
			private Item m_Basket;

			public EatEntry( Mobile from, Item basket ) : base( 6135, 1 )
			{
				m_From = from;
				m_Basket = basket;
			}

			public override void OnClick()
			{
				if ( m_Basket.Deleted || !m_From.CheckAlive() )
					return;

				m_Basket.OnDoubleClick( m_From );
			}
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
		}
	}
}