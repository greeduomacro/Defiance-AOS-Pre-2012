using System;
using Server.Network;

namespace Server.Items
{
	public class BigMushroom1AoS : Item
	{
		[Constructable]
		public BigMushroom1AoS() : base( 0x222E )
		{
			Movable = true;
		}

		public BigMushroom1AoS( Serial serial ) : base( serial )
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

	public class BigMushroom2AoS : Item
	{
		[Constructable]
		public BigMushroom2AoS() : base( 0x222F )
		{
			Movable = true;
		}

		public BigMushroom2AoS( Serial serial ) : base( serial )
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

	public class BigMushroom3AoS : Item
	{
		[Constructable]
		public BigMushroom3AoS() : base( 0x2230 )
		{
			Movable = true;
		}

		public BigMushroom3AoS( Serial serial ) : base( serial )
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

	public class BigMushroom4AoS : Item
	{
		[Constructable]
		public BigMushroom4AoS() : base( 0x2231 )
		{
			Movable = true;
		}

		public BigMushroom4AoS( Serial serial ) : base( serial )
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