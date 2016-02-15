using System;
using Server;

namespace Server.Items
{
	public class GlovesOfTheLepracon : LeatherGloves
	{
		public override int ArtifactRarity{ get{ return 11; } }

		public override int InitMinHits{ get{ return 255; } }
		public override int InitMaxHits{ get{ return 255; } }

		[Constructable]
		public GlovesOfTheLepracon()
		{
			Name = "gloves of the leprechaun";
			Hue = 0x84F;

			Attributes.BonusDex = 10;
			Attributes.LowerRegCost = 20;
			Attributes.Luck = 140;
			ArmorAttributes.SelfRepair = 3;

			PhysicalBonus = 10;
			FireBonus = 5;
			PoisonBonus = 5;
		}

		public GlovesOfTheLepracon( Serial serial ) : base( serial )
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