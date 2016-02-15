using System;

namespace Server.Items
{
	[Furniture]
	[Flipable(0xB32, 0xB33)]
	public class Throne : BaseCraftableItem
	{
		[Constructable]
		public Throne() : base(0xB33)
		{
			Weight = 1.0;
			Dyable = true;
		}

		public override Type DyeType{ get{ return typeof(FurnitureDyeTub); } }
		public override bool DisplayDyable{ get{ return false; } }

		public Throne(Serial serial) : base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write((int) 1);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = ( InheritsItem ? OldVersion : reader.ReadInt() ); //Required for BaseCraftableItem insertion

			if ( Weight == 6.0 )
				Weight = 1.0;

			if ( version < 1 )
				Dyable = true;
		}
	}

	[Furniture]
	[Flipable( 0xB2E, 0xB2F, 0xB31, 0xB30 )]
	public class WoodenThrone : BaseCraftableItem
	{
		[Constructable]
		public WoodenThrone() : base(0xB2F)
		{
			Weight = 15.0;
			Dyable = true;
		}

		public override Type DyeType{ get{ return typeof(FurnitureDyeTub); } }
		public override bool DisplayDyable{ get{ return false; } }

		public WoodenThrone(Serial serial) : base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write((int) 1);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = ( InheritsItem ? OldVersion : reader.ReadInt() ); //Required for BaseCraftableItem insertion

			if ( Weight == 6.0 )
				Weight = 15.0;

			if ( version == 0 )
				Dyable = true;
		}
	}
}