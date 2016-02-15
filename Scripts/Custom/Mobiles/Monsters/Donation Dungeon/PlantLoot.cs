using System;

namespace Server.Items
{
	public class PlantRare : Item
	{
		public PlantRare()
		{
			Weight = 20;
			Name = "A Tiny Plant";
			double plant = Utility.RandomDouble();
			if (plant > 0.80)
			{
				ItemID = 10460;
			}
			else if (plant <= 0.80 && plant > 0.60)
			{
				ItemID = 10461;
			}
			else if (plant <= 0.60 && plant > 0.40)
			{
				ItemID = 10462;
			}
			else if (plant <= 0.40 && plant > 0.25)
			{
				ItemID = 10463;
			}
			else if (plant <= 0.25 && plant > 0.10)
			{
				ItemID = 10464;
			}
			else if (plant <= 0.10 && plant > 0.06)
			{
				ItemID = 10465;
			}
			else if (plant <= 0.06 && plant > 0.005)
			{
				ItemID = 10467;
			}
			else
			{
				ItemID = 10466;
			}
		}

		public PlantRare(Serial serial)
			: base(serial)
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