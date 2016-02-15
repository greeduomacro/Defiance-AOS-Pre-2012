using System;
using Server;
using Server.Mobiles;

namespace Server.Items
{
	public class PlaguedSkull : Item, ICarvable
	{
		private SpawnTimer m_Timer;
		public double breakChance = 0.3;

		[Constructable]
		public PlaguedSkull() : base( Utility.Random( 0x1AE0, 5 ) )
		{
			Movable = false;
			Hue = 0x55C;

			m_Timer = new SpawnTimer( this );
			m_Timer.Start();
		}

		public void Carve( Mobile from, Item item )
		{
			Effects.PlaySound( GetWorldLocation(), Map, 0x48F );
			Effects.SendLocationEffect( GetWorldLocation(), Map, 0x3728, 10, 10, 0, 0 );

			if ( breakChance > Utility.RandomDouble() )
			{
				from.SendMessage( "You destroy the skull." );

				if( Utility.RandomDouble() < 0.1 )
				{
					Gold gold = new Gold( 750, 1000 );
					gold.MoveToWorld( GetWorldLocation(), Map );
				}

				Delete();

				m_Timer.Stop();
			}
			else
			{
				from.SendMessage( "You damage the skull." );
				breakChance += Utility.RandomDouble() * 0.15 + 0.15;
			}
		}

		public PlaguedSkull( Serial serial ) : base( serial )
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

			m_Timer = new SpawnTimer( this );
			m_Timer.Start();
		}

		private class SpawnTimer : Timer
		{
			private Item m_Item;

			public SpawnTimer( Item item ) : base( TimeSpan.FromSeconds( 10 ) )
			{
				Priority = TimerPriority.FiftyMS;

				m_Item = item;
			}

			protected override void OnTick()
			{
				if ( m_Item.Deleted )
					return;

				BaseCreature spawn;

				if( Utility.RandomDouble() < 0.7 )
					spawn = new Slime();
				else
					spawn = new PlagueBeast();

				spawn.IsPlagued = true;
				spawn.IgnorePlagueLoot = true;
				spawn.MoveToWorld( m_Item.Location, m_Item.Map );

				m_Item.Delete();
			}
		}
	}
}