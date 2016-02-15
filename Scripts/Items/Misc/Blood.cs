using System;
using Server;

namespace Server.Items
{
	public class Blood : Item
	{
		public static int DefaultItemID{ get{ return Utility.RandomBool() ? 0x1645 : Utility.Random( 0x122A , 6 ); } }

		[Constructable]
		public Blood() : this( DefaultItemID, 0, false )
		{
		}

		[Constructable]
		public Blood( bool Long ) : this( DefaultItemID, 0, Long )
		{
		}

		[Constructable]
		public Blood( int itemID ) : this( itemID, 0, false )
		{
		}

		[Constructable]
		public Blood( int itemID, int hue ) : this( itemID, hue, false )
		{
		}

		[Constructable]
		public Blood( int itemID, bool Long ) : this( itemID, 0, Long )
		{
		}

		[Constructable]
		public Blood( int itemID, int hue, bool Long ) : base( itemID )
		{
			Hue = hue;
			Movable = false;

			new InternalTimer( this, Long ).Start();
		}

		public Blood( Serial serial ) : base( serial )
		{
			new InternalTimer( this, false ).Start();
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

		private class InternalTimer : Timer
		{
			private Item m_Blood;

			public InternalTimer( Item blood, bool longduration ) : base( TimeSpan.FromSeconds( 3.0 + (Utility.RandomDouble() * (longduration ? 25.0 : 3.0) ) ) )
			{
				Priority = TimerPriority.OneSecond;

				m_Blood = blood;
			}

			protected override void OnTick()
			{
				m_Blood.Delete();
			}
		}
	}
}