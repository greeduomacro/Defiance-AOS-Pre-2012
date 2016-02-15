using System;
using Server;

namespace Server.Items
{
	public class Hay : Item
	{
		public override bool Decays{get{return false;} }

		[Constructable]
		public Hay() : base( 0xF34 )
		{
			Name = "hay";
			Weight = 1.0;
		}

		public Hay( Serial serial ) : base( serial )
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

	public class Rock : Item
	{
		public override bool Decays{get{return false;} }

		[Constructable]
		public Rock() : base( 0x1368 )
		{
			Name = "a rock";
			Weight = 1.0;
		}

		public Rock( Serial serial ) : base( serial )
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

	public class Rocks : Item
	{
		public override bool Decays{get{return false;} }

		[Constructable]
		public Rocks() : base( 0x1367 )
		{
			Name = "rocks";
			Weight = 1.0;
		}

		public Rocks( Serial serial ) : base( serial )
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

	public class NClosedBarrel : BaseContainer
	{
		public override bool Decays{get{return false;} }

		public override int DefaultGumpID{ get{ return 0x3E; } }
		public override int DefaultDropSound{ get{ return 0x42; } }

		public override Rectangle2D Bounds
		{
			get{ return new Rectangle2D( 33, 36, 109, 112 ); }
		}

		[Constructable]
		public NClosedBarrel() : base( 0xFAE )
		{
			Name = "a closed barrel";
			Weight = 25.0;
		}

		public NClosedBarrel( Serial serial ) : base( serial )
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

	public class BrokenChair : Item
	{
		public override bool Decays{get{return false;} }

		[Constructable]
		public BrokenChair() : base( 0xC1A )
		{
			Weight = 1.0;
		}

		public BrokenChair( Serial serial ) : base( serial )
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

	public class ToolKit : Item
	{
		public override bool Decays{get{return false;} }

		[Constructable]
		public ToolKit() : base( 0x1EBA )
		{
			Weight = 1.0;
		}

		public ToolKit( Serial serial ) : base( serial )
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