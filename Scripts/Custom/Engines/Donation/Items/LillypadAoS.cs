using System;
using Server.Network;

namespace Server.Items
{
	public class LillyPad1AoS : Item
	{
		[Constructable]
		public LillyPad1AoS() : base( 0xDBC )
		{
			Movable = true;
		}

		public LillyPad1AoS( Serial serial ) : base( serial )
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

	public class LillyPad2AoS : Item
	{
		[Constructable]
		public LillyPad2AoS() : base( 0xDBD )
		{
			Movable = true;
		}

		public LillyPad2AoS( Serial serial ) : base( serial )
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

	public class LillyPad3AoS : Item
	{
		[Constructable]
		public LillyPad3AoS() : base( 0xDBE )
		{
			Movable = true;
		}

		public LillyPad3AoS( Serial serial ) : base( serial )
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