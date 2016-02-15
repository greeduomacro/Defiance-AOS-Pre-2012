using System;
using Server;
using Server.Mobiles;

namespace Server.Items
{
	public class SolenEggSack : Item, ICarvable
	{
		private SolenEggTimer m_Timer;
		private AntEggType m_EggType;

		[Constructable]
		public SolenEggSack( AntEggType eggType ) : base( 0x10D9 )
		{
			Movable = false;
			m_EggType = eggType;

			m_Timer = new SolenEggTimer( this );
			m_Timer.Start();
		}

		public void Carve( Mobile from, Item item )
		{
			from.SendMessage( "You destroy the egg sack." );
			m_Timer.Stop();
			Delete();
		}

		public SolenEggSack( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version

			writer.Write( (int) m_EggType );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();

			m_EggType = (AntEggType) reader.ReadInt();

			m_Timer = new SolenEggTimer( this );
			m_Timer.Start();
		}

		private class SolenEggTimer : Timer
		{
			private SolenEggSack m_EggSack;

			public SolenEggTimer( SolenEggSack eggSack ) : base( TimeSpan.FromSeconds( 2.5 + (Utility.RandomDouble() * 2.5) ) )
			{
				Priority = TimerPriority.FiftyMS;
				m_EggSack = eggSack;
			}

			protected override void OnTick()
			{
				if ( m_EggSack.Deleted )
					return;

				Mobile spawn;

				int number = Utility.Random( 2 );
				if( m_EggSack.m_EggType == AntEggType.BlackEgg )
					number+=2;

				switch( number )
				{
					default:
					case 0: spawn = new RedSolenWarrior(); break;
					case 1: spawn = new RedSolenWorker(); break;
					case 2: spawn = new BlackSolenWarrior(); break;
					case 3: spawn = new BlackSolenWorker(); break;
				}

				spawn.Map = m_EggSack.Map;
				spawn.Location = m_EggSack.Location;

				m_EggSack.Delete();
			}
		}
	}
}