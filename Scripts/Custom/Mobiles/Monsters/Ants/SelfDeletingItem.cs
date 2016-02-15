using System;
using Server;

namespace Server.Items
{
	public class SelfDeletingItem : Static
	{
		private TimeSpan m_tsDelay;

		[Constructable]
		public SelfDeletingItem( string name, int hue, int itemid, TimeSpan delay ) : this( hue, itemid, delay )
		{
			Name = name;
		}

		[Constructable]
		public SelfDeletingItem( int hue, int itemid, TimeSpan delay ) : base( itemid )
		{
			Movable = false;
			Hue = hue;
			m_tsDelay = delay;

			Timer.DelayCall( m_tsDelay, new TimerCallback( DeleteItem ) );
		}

		public SelfDeletingItem( Serial serial ) : base( serial )
		{
		}

		private void DeleteItem()
		{
			this.Delete();
		}

		public override void Serialize( GenericWriter writer )
		{
		}

		public override void Deserialize( GenericReader reader )
		{
		}
	}
}