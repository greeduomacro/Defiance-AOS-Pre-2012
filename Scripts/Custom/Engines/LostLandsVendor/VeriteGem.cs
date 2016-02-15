using System;

namespace Server.Items
{
	public class VeriteGem : Item
	{
		public override string DefaultName{ get{ return "verite gem"; } }
		public override double DefaultWeight
		{
			get { return 0.02; }
		}

		[Constructable]
		public VeriteGem() : this( 1 )
		{
		}

		[Constructable]
		public VeriteGem( int amount ) : base( 0x3191 )
		{
			Stackable = true;
			Amount = ((amount > 0) ? amount : 1);
			Hue = 2210;
		}

		public VeriteGem( Serial serial ) : base( serial )
		{
		}

		public override int GetDropSound()
		{
			return 0x2E4;
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