using System;
using System.IO;
using Server.Mobiles;

namespace Server.Engines.RewardSystem
{
	[TypeAlias( "Server.RewardSystem.CopperBar" )]
	public class CopperBar : Item
	{
		public override double DefaultWeight { get { return 0.3; } }
		public override string DefaultName{ get{ return "Copper Bar"; } }

		public CopperBar()
			: this(1)
		{
		}

		public CopperBar(int amount)
			: base(0x1BE3)
		{
			Stackable = true;
			Amount = amount;
			LootType = LootType.Cursed;
		}

		[Constructable]
		public CopperBar(int amount, bool willAdd) //some weird constructor so that GMs won't know about it and will use [createcopperbars
			: base(0x1BE3)
		{
			Stackable = true;
			Amount = amount;
			LootType = LootType.Cursed;
		}

		public CopperBar(Serial serial)
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