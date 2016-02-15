using System;
using Server;

namespace Server.Items
{
	public class CrystalPedestalAddon : BaseAddon
	{
		public override BaseAddonDeed Deed { get { return new CrystalPedestalAddonDeed(); } }

		[Constructable]
		public CrystalPedestalAddon()
		{
			AddComponent(new AddonComponent(0x2FD4), 0, 0, 0);
		}

		public CrystalPedestalAddon(Serial serial)
			: base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.WriteEncodedInt(0); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadEncodedInt();
		}
	}

	public class CrystalPedestalAddonDeed : BaseAddonDeed
	{
		public override BaseAddon Addon { get { return new CrystalPedestalAddon(); } }
		public override int LabelNumber { get { return 1032244; } } // pedestal with crystal

		[Constructable]
		public CrystalPedestalAddonDeed()
		{
		}

		public CrystalPedestalAddonDeed(Serial serial)
			: base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.WriteEncodedInt(0); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadEncodedInt();
		}
	}
}