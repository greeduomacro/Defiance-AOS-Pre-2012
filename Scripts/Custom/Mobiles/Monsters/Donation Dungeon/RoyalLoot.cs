using System;

namespace Server.Items
{
	public class RoyalLoot : Item
	{
		public RoyalLoot()
		{
			Weight = 20;
			double royalLoot = Utility.RandomDouble();
			if (royalLoot > 0.60)
			{
				ItemID = 10296;
				Name = "A Royal Butterfly";
			}
			else if (royalLoot <= 0.60 && royalLoot > 0.25)
			{
				ItemID = 10297;
				Name = "A Royal Swan";
			}
			else if (royalLoot <= 0.25 && royalLoot > 0.08)
			{
				ItemID = 10298;
				Name = "A Squashed Frog";
			}
			else
			{
				ItemID = 10299;
				Hue = 1151;
				Name = "A Royal Star";
			}
		}

		public RoyalLoot( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}