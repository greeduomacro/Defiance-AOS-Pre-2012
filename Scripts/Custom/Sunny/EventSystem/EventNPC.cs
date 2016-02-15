using System;
using System.Collections;
using System.Collections.Generic;
using Server.Items;
using Server.Gumps;
using Server.Network;
using Server.Mobiles;
using Server.ContextMenus;
using Server.Engines.VeteranRewards;

namespace Server.Events
{
	public class EventNpc : BaseVendor
	{
		private List<SBInfo> m_SBInfos = new List<SBInfo>();
		protected override List<SBInfo> SBInfos{ get { return m_SBInfos; } }

		[Constructable]
		public EventNpc()
			: base("the event master")
		{
			EventSystem.AddNpc(this);
		}

		public override void InitSBInfo()
		{
		}

		public override VendorShoeType ShoeType  { get { return VendorShoeType.Sandals; } }

		public override void InitOutfit()
		{
			base.InitOutfit();

			AddItem(new Robe(Utility.RandomNeutralHue()));
			AddItem(new WizardsHat(Utility.RandomNeutralHue()));
			AddItem(new GnarledStaff());
		}

		public EventNpc(Serial serial)
			: base(serial)
		{
		}

		public override void AddCustomContextEntries(Mobile from, List<ContextMenuEntry> list)
		{
			base.AddCustomContextEntries(from, list);

			if (from.Alive && from is PlayerMobile)
				list.Add(new TalkEntry(this));
		}

		public class TalkEntry : ContextMenuEntry
		{
			private EventNpc m_Npc;

			public TalkEntry(EventNpc Npc)
				: base(6146) // Talk
			{
				m_Npc = Npc;
			}

			public override void OnClick()
			{
				PlayerMobile from = Owner.From as PlayerMobile;
				if (from == null)
					return;

				if (EventSystem.Open)
				{
					EventSystem.JoinMethod(from);
					from.CloseGump( typeof( RewardChoiceGump ) );
					from.CloseGump( typeof( RewardConfirmGump ) );
					from.CloseGump( typeof( RewardNoticeGump ) );
				}

				else
					m_Npc.SayTo(from, "Currently there is no game open.");
			}
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
			EventSystem.AddNpc(this);
		}
	}
}