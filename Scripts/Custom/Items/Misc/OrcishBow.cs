using System;
using Server;

namespace Server.Items
{
	[FlipableAttribute( 0x13B2, 0x13B1 )]
	public class OrcishBow : Bow
	{
		[Constructable]
		public OrcishBow()
		{
			Hue = 0x497;
			Attributes.WeaponDamage = 25;
			Name = "Orcish Bow";
			Slayer = SlayerName.Repond;
		}

		public OrcishBow( Serial serial ) : base( serial )
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