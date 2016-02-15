using System;
using Server.Network;

namespace Server.Items
{


	public class SerpentCrestAoS : Item
	{
		[Constructable]
		public SerpentCrestAoS() : base( 0x1514 )
		{
			Movable = true;
			Name = "a donation serpent crest";
		}

		public SerpentCrestAoS( Serial serial ) : base( serial )
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
		}
	}

}