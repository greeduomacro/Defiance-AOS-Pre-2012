using System;
using Server;
using Server.Network;

namespace Server.Items
{
	public class MandrakeQuestReagent : BaseQuestReagent
	{
		public static int GetCropID()
		{
			return Utility.RandomBool() ? 0x18DF : 0x18E0;
		}

		public override int GetPickedID()
		{
			return 3254;
		}

		[Constructable]
		public MandrakeQuestReagent()
			: base(GetCropID())
		{
		}

		public MandrakeQuestReagent(Serial serial)
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

	public class GarlicQuestReagent : BaseQuestReagent
	{
		public static int GetCropID()
		{
			return Utility.RandomBool() ? 0x18E1 : 0x18E2;
		}

		public override int GetPickedID()
		{
			return 3254;
		}

		[Constructable]
		public GarlicQuestReagent()
			: base(GetCropID())
		{
		}

		public GarlicQuestReagent(Serial serial)
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

	public class NightShadeQuestReagent : BaseQuestReagent
	{
		public static int GetCropID()
		{
			return Utility.RandomBool() ? 0x18E5 : 0x18E6;
		}

		public override int GetPickedID()
		{
			return 3254;
		}

		[Constructable]
		public NightShadeQuestReagent()
			: base(GetCropID())
		{
		}

		public NightShadeQuestReagent(Serial serial)
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

	public class GinsengQuestReagent : BaseQuestReagent
	{
		public static int GetCropID()
		{
			return Utility.RandomBool() ? 0x18E9 : 0x18EA;
		}

		public override int GetPickedID()
		{
			return 3254;
		}

		[Constructable]
		public GinsengQuestReagent()
			: base(GetCropID())
		{
		}

		public GinsengQuestReagent(Serial serial)
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