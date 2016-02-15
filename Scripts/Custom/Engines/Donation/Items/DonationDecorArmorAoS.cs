using System;
using Server.Network;

namespace Server.Items
{
	public class DonationDecorArmor1AoS : Item
	{
		[Constructable]
		public DonationDecorArmor1AoS() : base( 0x1508 )
		{
			Movable = true;
		}

		public DonationDecorArmor1AoS( Serial serial ) : base( serial )
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

	public class DonationDecorArmor2AoS : Item
	{
		[Constructable]
		public DonationDecorArmor2AoS() : base( 0x151C )
		{
			Movable = true;
		}

		public DonationDecorArmor2AoS( Serial serial ) : base( serial )
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

	public class DonationDecorArmor3AoS : Item
	{
		[Constructable]
		public DonationDecorArmor3AoS() : base( 0x151A )
		{
			Movable = true;
		}

		public DonationDecorArmor3AoS( Serial serial ) : base( serial )
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

	public class DonationDecorArmor4AoS : Item
	{
		[Constructable]
		public DonationDecorArmor4AoS() : base( 0x1512 )
		{
			Movable = true;
		}

		public DonationDecorArmor4AoS( Serial serial ) : base( serial )
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