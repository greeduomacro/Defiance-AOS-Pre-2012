//Opticon
using System;
using Server;

namespace Server.Items
{
	public class YetisPads : FurBoots
	{
		//public override int LabelNumber{ get{ return 1075048; } } // Pads of the Cu Sidhe
		public override int ArtifactRarity{ get{ return 11; } }

		//public override int InitMinHits{ get{ return 255; } }
		//public override int InitMaxHits{ get{ return 255; } }

		[Constructable]
		public YetisPads() : base( 0x47E )
		{
			Name = "Yeti's Pads";
			Hue = 0x481;
			Resistances.Cold = 3;

		}

		public YetisPads( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}
}