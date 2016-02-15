using System;
using Server;
using Server.Items;

namespace Server.Multis
{
	public class TentCampGreen : BaseMulti
	{
		[Constructable]
		public TentCampGreen()
			: base(0x72 | 0x4000)
		{
		}

		public TentCampGreen(Serial serial)
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
}