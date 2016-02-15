using System;
using System.Collections;
using Server.Items;
using Server.Targeting;

namespace Server.Mobiles
{
	[CorpseName( "a krakens corpse" )]
	public class SpecialNetKraken : Kraken
	{
		[Constructable]
		public SpecialNetKraken() : base()
		{
			SetResistance( ResistanceType.Fire, 50, 60 );

			Fame = 14000;
			Karma = -14000;
		}

		public override bool HasBreath{ get{ return true; } }

		public SpecialNetKraken( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}