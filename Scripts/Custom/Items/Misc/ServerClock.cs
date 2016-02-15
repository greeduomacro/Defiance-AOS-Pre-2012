using System;
using Server;

namespace Server.Items
{
	[Flipable( 0x104B, 0x104C )]
	public class ServerClock : Clock
	{

		[Constructable]
		public ServerClock() : base()
		{
		}

		public ServerClock( Serial serial ) : base( serial )
		{
		}


		public override void OnDoubleClick( Mobile from )
		{
			from.SendMessage("The current Server time is: {0}", DateTime.Now.ToString() );
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

			if ( Weight == 2.0 )
				Weight = 3.0;
		}
	}

}