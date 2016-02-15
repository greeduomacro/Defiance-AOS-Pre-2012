using System;
using Server.Items;
using Server.Network;

namespace Server.Items
{
	public class WhisperingRose : Item
	{
		[Constructable]
		public WhisperingRose(String name) : base( 6377 + Utility.Random(2) )
		{
			Name = "Whispering Rose from " + name;
			LootType = LootType.Blessed;
			if (Utility.Random(3) == 0)
			{
				ItemID = 9035;
				Hue = 33;
			}
		}

		public WhisperingRose( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.WriteEncodedInt( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadEncodedInt();
		}
	}
}