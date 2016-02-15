
namespace Server.Items
{
	[Flipable( 0x9A9, 0xE7E )]
	public class RentalSmallCrate : RentalChest
	{
		public override string DefaultName{ get{ return "a small rental crate"; } }

		[Constructable]
		public RentalSmallCrate() : base( )
		{
			ItemID = 0x9A9;
		}

		public RentalSmallCrate( Serial serial ) : base( serial )
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

	[Flipable( 0xE3F, 0xE3E )]
	public class RentalMediumCrate : RentalChest
	{
		public override string DefaultName{ get{ return "a medium rental crate"; } }

		[Constructable]
		public RentalMediumCrate() : base( )
		{
			ItemID = 0xE3F;
		}

		public RentalMediumCrate( Serial serial ) : base( serial )
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

	[Flipable( 0xE3D, 0xE3C )]
	public class RentalLargeCrate : RentalChest
	{
		public override string DefaultName{ get{ return "a large rental crate"; } }

		[Constructable]
		public RentalLargeCrate() : base( )
		{
			ItemID = 0xE3D;
		}

		public RentalLargeCrate( Serial serial ) : base( serial )
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

	[Flipable( 0x9AB, 0xE7C )]
	public class RentalMetalChest : RentalChest
	{
		public override string DefaultName{ get{ return "a metal rental crate"; } }

		[Constructable]
		public RentalMetalChest() : base( )
		{
			ItemID = 0x9AB;
		}

		public RentalMetalChest( Serial serial ) : base( serial )
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