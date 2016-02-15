using System;
using Server.Items;
using Server.Network;
using Server.ContextMenus;
using Server.Spells;
using System.Collections.Generic;


namespace Server.Mobiles
{
	public class CriminalBanker : Banker
	{
		[Constructable]
		public CriminalBanker()
			: base()
		{
		}

		public CriminalBanker(Serial serial)
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