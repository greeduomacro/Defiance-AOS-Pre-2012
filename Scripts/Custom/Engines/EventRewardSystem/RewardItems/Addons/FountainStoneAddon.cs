using System;
using Server;

namespace Server.Items
{
	public class FountainStoneAddon : BaseAddon
	{
		public override BaseAddonDeed Deed { get { return new FountainStoneAddonDeed(); } }

		[Constructable]
		public FountainStoneAddon()
		{
			int itemID = 0x1731;

			AddComponent(new AddonComponent(itemID++), -2, +1, 0);
			AddComponent(new AddonComponent(itemID++), -1, +1, 0);
			AddComponent(new AddonComponent(itemID++), +0, +1, 0);
			AddComponent(new AddonComponent(itemID++), +1, +1, 0);

			AddComponent(new AddonComponent(itemID++), +1, +0, 0);
			AddComponent(new AddonComponent(itemID++), +1, -1, 0);
			AddComponent(new AddonComponent(itemID++), +1, -2, 0);

			AddComponent(new AddonComponent(itemID++), +0, -2, 0);
			AddComponent(new AddonComponent(itemID++), +0, -1, 0);
			AddComponent(new AddonComponent(itemID++), +0, +0, 0);

			AddComponent(new AddonComponent(itemID++), -1, +0, 0);
			AddComponent(new AddonComponent(itemID++), -2, +0, 0);

			AddComponent(new AddonComponent(itemID++), -2, -1, 0);
			AddComponent(new AddonComponent(itemID++), -1, -1, 0);

			AddComponent(new AddonComponent(itemID++), -1, -2, 0);
			AddComponent(new AddonComponent(++itemID), -2, -2, 0);
		}

		public FountainStoneAddon(Serial serial)
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

	public class FountainStoneAddonDeed : BaseAddonDeed
	{
		public override BaseAddon Addon{ get{ return new FountainStoneAddon(); } }
		public override int LabelNumber { get { return 1026629; } } // fountain

		[Constructable]
		public FountainStoneAddonDeed()
		{
		}

		public FountainStoneAddonDeed( Serial serial ) : base( serial )
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