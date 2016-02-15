using System;
using Server;
using Server.Mobiles;

namespace Server.Items
{
	public class CCSummoningAltar : BaseSummoningAltar
	{
		public override Type ChampionType { get { return typeof(Hephaestus); } }
		public override int HueActive { get { return 0x21; } }
		public override int HueInactive { get { return 0x472; } }

		[Constructable]
		public CCSummoningAltar()
		{
		}

		public CCSummoningAltar(Serial serial)
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