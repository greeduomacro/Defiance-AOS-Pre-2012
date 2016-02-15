using System;
using System.Collections.Generic;
using Server.Engines.Quests;
using Server.Mobiles;
using Server.Items;

namespace Server.Engines.RewardSystem
{
	[TypeAlias( "Server.RewardSystem.EventRewardNPC" )]
	public class EventRewardNPC : BaseQuester
	{
		[Constructable]
		public EventRewardNPC()
			: base("The Event Reward Vendor")
		{
			InitStats(100, 100, 25);

			Hue = Utility.RandomSkinHue();

			Female = false;
			Direction = Direction.Down;
			Body = 0x190;

			AddItem(new Tunic(0x48D));
			AddItem(new LongPants(0x48D));
			AddItem(new SkullCap(0x48D));
			AddItem(new Boots());

			Item hair = new Item(Utility.RandomList(0x203B, 0x203C, 0x203D, 0x2044, 0x2045, 0x2047, 0x2049, 0x204A));
			hair.Hue = Utility.RandomHairHue();
			hair.Layer = Layer.Hair;
			hair.Movable = false;
			AddItem(hair);
		}

		public override bool ClickTitle { get { return true; } }
		public override void InitBody() { }
		public override void InitOutfit() { }

		public override void OnTalk(PlayerMobile talker, bool contextMenu)
		{
			talker.CloseGump(typeof(OpeningGump));
			talker.SendGump(new OpeningGump(talker));
		}

		public EventRewardNPC(Serial serial)
			: base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
		}
	}
}