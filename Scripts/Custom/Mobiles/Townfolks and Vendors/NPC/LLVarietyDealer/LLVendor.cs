using System;
using System.Collections;
using System.Collections.Generic;
using Server.ContextMenus;
using Server.Engines.Quests;
using Server.Items;
using Server.Mobiles;
using Server.Engines.RewardSystem;

namespace Server.Engines.RewardSystem
{
	[TypeAlias( "Server.RewardSystem.LLVendor" )]
	public class LLVendor : BaseVendor
	{
		[Constructable]
		public LLVendor()
			: base("the Variety Dealer")
		{
			InitStats(100, 100, 25);

			Hue = 34869;

			Female = false;
			Direction = Direction.Down;
			Body = 0x190;
			Name = "Papun";
			CantWalk = true;

			AddItem(new HoodedShroudOfShadows(1427));
			AddItem(new Sandals(1427));

			Item hair = new Item(Utility.RandomList(0x203B, 0x203C, 0x203D, 0x2044, 0x2045, 0x2047, 0x2049, 0x204A));
			hair.Hue = Utility.RandomHairHue();
			hair.Layer = Layer.Hair;
			hair.Movable = false;
			AddItem(hair);
		}

		public override bool ClickTitle { get { return true; } }
		public override void InitBody() { }
		public override void InitOutfit() { }

		private List<SBInfo> m_SBInfos = new List<SBInfo>();
		protected override List<SBInfo> SBInfos{ get { return m_SBInfos; } }

		public override void InitSBInfo()
		{
			m_SBInfos.Add( new SBLLVendor() );
		}

		public override void GetContextMenuEntries( Mobile from, List<ContextMenuEntry> list )
		{
			list.Add( new LLTalkEntry( from ) );
			base.GetContextMenuEntries( from, list );
		}

		public LLVendor(Serial serial)
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

namespace Server.ContextMenus
{
	public class LLTalkEntry : ContextMenuEntry
	{
		private Mobile m_From;

		public LLTalkEntry( Mobile from ) : base( 6146, 3 )
		{
			m_From = from;
		}

		public override void OnClick()
		{
			m_From.CloseGump(typeof(CategoriesGump));
			m_From.SendGump( new CategoriesGump(m_From));
		}
	}
}