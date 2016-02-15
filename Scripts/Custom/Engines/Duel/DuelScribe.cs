using System.Collections;
using System.Collections.Generic;
using Server.Mobiles;
using Server.ContextMenus;

namespace Server.Events.Duel
{
	public class TalkEntry : ContextMenuEntry
	{
		private DuelScribe m_Scribe;

		public TalkEntry(DuelScribe scribe) : base(scribe.TalkNumber)
		{
			m_Scribe = scribe;
		}

		public override void OnClick()
		{
			Mobile from = Owner.From;

			if (from.CheckAlive() && from is PlayerMobile && m_Scribe.CanTalkTo((PlayerMobile)from))
				m_Scribe.OnTalk((PlayerMobile)from, true);
		}
	}

	public class DuelScribe : Scribe
	{
		public virtual int TalkNumber { get { return 6146; } }

		public override bool AlwaysAttackable { get { return false; } }

		public bool CanTalkTo(PlayerMobile to)
		{
			return true;
		}

		public override void AddCustomContextEntries(Mobile from, List<ContextMenuEntry> list)
		{
			base.AddCustomContextEntries(from, list);

			if (from.Alive && from is PlayerMobile && TalkNumber > 0 && CanTalkTo((PlayerMobile)from))
				list.Add(new TalkEntry(this));
		}

		[Constructable]
		public DuelScribe() : base()
		{
		}

		public void OnTalk(PlayerMobile talker, bool contextMenu)
		{
			if (talker.Player)
			{
				talker.CloseGump(typeof(DuelScoreGump));
				talker.SendGump(new DuelScoreGump(talker));
				Say("*The scribe looks trough his scrolls, finds what he is looking for. And hands you a piece of paper with your name on it.*");
			}
		}

		public DuelScribe(Serial serial) : base(serial)
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