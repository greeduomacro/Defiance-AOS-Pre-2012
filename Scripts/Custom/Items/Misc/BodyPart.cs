using System;
using Server.Items;
using Server.Network;

namespace Server.Items
{
	public class BodyPart : Item
	{
		[Constructable]
		public BodyPart() : base( Utility.RandomMinMax( 0x1CDD, 0x1CF0 ) )
		{
			Weight = 0.1;
		}

		public BodyPart( Serial serial ) : base( serial )
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