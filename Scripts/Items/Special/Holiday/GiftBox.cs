using System;
using Server.Items;
using Server.Network;

namespace Server.Items
{
	[Furniture]
	[Flipable( 0x232A, 0x232B )]
	public class GiftBox : BaseContainer
	{
		[Constructable]
		public GiftBox() : this( Utility.RandomDyedHue() )
		{
		}

		[Constructable]
		public GiftBox( int hue ) : base( Utility.Random( 0x232A, 2 ) )
		{
			Weight = 2.0;
			Hue = hue;
			Dyable = true;
		}

		public override Type DyeType{ get{ return typeof(FurnitureDyeTub); } }
		public override bool DisplayDyable{ get{ return false; } }

		public GiftBox( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 1 ); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();

			if ( version == 0 )
				Dyable = true;
		}
	}
}