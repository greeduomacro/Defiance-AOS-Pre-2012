using System;

namespace Server.Items
{
	public class PoisonFungus : Food
	{
		[Constructable]
		public PoisonFungus() : this( 1 )
		{
		}

		[Constructable]
		public PoisonFungus( int amount ) : base( 0x26B7 )
		{
			Name = "Poison Fungus";
			Hue = 0x558;
			Weight = 0.1;
			Stackable = true;
			Amount = amount;
			FillFactor = 1;
			Poison = Poison.Deadly;
		}

		public PoisonFungus(Serial serial) : base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write((int) 0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();
		}
	}
}