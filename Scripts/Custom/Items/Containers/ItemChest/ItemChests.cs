//******************************************************
// Name: ItemChests
// Desc: Written by Eclipse
//******************************************************
namespace Server.Items
{
	[FlipableAttribute( 0xe43, 0xe42 )]
	public class EICWooden : BaseItemChest
	{
		public override int DefaultGumpID{ get{ return 0x49; } }
		public override int DefaultDropSound{ get{ return 0x42; } }

		public override Rectangle2D Bounds
		{
			get{ return new Rectangle2D( 20, 105, 150, 180 ); }
		}

		[Constructable]
		public EICWooden() : base( 4, 0xe43 )
		{
		}

		public EICWooden( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}

	[FlipableAttribute(0xe41, 0xe40)]
	public class EICMetalGoldenRed : BaseItemChest
	{
		public override int DefaultGumpID { get { return 0x42; } }
		public override int DefaultDropSound { get { return 0x42; } }

		public override Rectangle2D Bounds
		{
			get { return new Rectangle2D(20, 105, 150, 180); }
		}

		[Constructable]
		public EICMetalGoldenRed()
			: base(7, 0xe41)
		{
			Hue = 0x89B;
		}

		public EICMetalGoldenRed(Serial serial)
			: base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write((int)0); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();
		}
	}

	[FlipableAttribute( 0xe41, 0xe40 )]
	public class EICMetalGolden : BaseItemChest
	{
		public override int DefaultGumpID{ get{ return 0x42; } }
		public override int DefaultDropSound{ get{ return 0x42; } }

		public override Rectangle2D Bounds
		{
			get{ return new Rectangle2D( 20, 105, 150, 180 ); }
		}

		[Constructable]
		public EICMetalGolden() : base( 6, 0xe41 )
		{
		}

		public EICMetalGolden( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}

	[FlipableAttribute( 0x9ab, 0xe7c )]
	public class EICMetal : BaseItemChest
	{
		public override int DefaultGumpID{ get{ return 0x4A; } }
		public override int DefaultDropSound{ get{ return 0x42; } }

		public override Rectangle2D Bounds
		{
			get{ return new Rectangle2D( 20, 105, 150, 180 ); }
		}

		[Constructable]
		public EICMetal() : base( 5, 0x9ab )
		{
		}

		public EICMetal( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}

	[Flipable( 0xE3D, 0xE3C )]
	public class EICLargeCrate : BaseItemChest
	{
		public override int DefaultGumpID{ get{ return 0x44; } }
		public override int DefaultDropSound{ get{ return 0x42; } }

		public override Rectangle2D Bounds
		{
			get{ return new Rectangle2D( 20, 10, 150, 90 ); }
		}

		[Constructable]
		public EICLargeCrate() : base( 3, 0xE3D )
		{
			Weight = 1.0;
		}

		public EICLargeCrate( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
	[Flipable( 0xE3F, 0xE3E )]
	public class EICMediumCrate : BaseItemChest
	{
		public override int DefaultGumpID{ get{ return 0x44; } }
		public override int DefaultDropSound{ get{ return 0x42; } }

		public override Rectangle2D Bounds
		{
			get{ return new Rectangle2D( 20, 10, 150, 90 ); }
		}

		[Constructable]
		public EICMediumCrate() : base( 2, 0xE3F )
		{
			Weight = 2.0;
		}

		public EICMediumCrate( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	[Flipable( 0x9A9, 0xE7E )]
	public class EICSmallCrate : BaseItemChest
	{
		public override int DefaultGumpID{ get{ return 0x44; } }
		public override int DefaultDropSound{ get{ return 0x42; } }

		public override Rectangle2D Bounds
		{
			get{ return new Rectangle2D( 20, 10, 150, 90 ); }
		}

		[Constructable]
		public EICSmallCrate() : base( 1, 0x9A9 )
		{
			Weight = 2.0;
		}

		public EICSmallCrate( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}