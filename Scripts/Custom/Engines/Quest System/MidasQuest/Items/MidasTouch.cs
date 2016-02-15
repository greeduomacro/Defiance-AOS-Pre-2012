using System;
using Server;
using Server.Engines.Craft;

namespace Server.Items
{
	[FlipableAttribute( 0x13E4, 0x13E3 )]
	public class MidasTouch : AncientSmithyHammer
	{
		[Constructable]
		public MidasTouch() : base(300, 1)
		{
			Name = "Midas touch";
			Weight = 8.0;
			Layer = Layer.OneHanded;
			Hue = 0x501;
		}


		public MidasTouch( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version

			//writer.Write( (int) m_Bonus );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 0:
				{
					//m_Bonus = reader.ReadInt();
					break;
				}
			}

			if ( Hue == 0 )
				Hue = 0x482;
		}
	}
}