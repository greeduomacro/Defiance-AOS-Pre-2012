using System;
using System.Collections.Generic;
using Server.Gumps;
using Server.ContextMenus;
using Server.Items;

namespace Server.Misc
{
	public class AdvertMobile : Mobile
	{
		public override bool HandlesOnSpeech(Mobile from) { return true; }

		[Constructable]
		public AdvertMobile()
			: base()
		{
			InitStats(100, 100, 25);

			Title = "the advertiser";
			Hue = Utility.RandomSkinHue();
			Body = 0x190;
			Name = NameList.RandomName("male");
			AddItem(new FancyShirt(Utility.RandomBlueHue()));
			Item skirt = new Kilt();
			skirt.Hue = Utility.RandomGreenHue();
			AddItem(skirt);
			AddItem(new FeatheredHat(Utility.RandomGreenHue()));
			Item boots = new ThighBoots();
			AddItem(boots);
			Utility.AssignRandomHair(this);
			AdvertSystem.Advertisers.Add(this);
		}

		public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
		{
			base.GetContextMenuEntries(from, list);

			if (from.Alive)
			{
				list.Add(new InformationEntry(from));
				list.Add(new TalkEntry(from));
			}
		}

		public override void OnDelete()
		{
			AdvertSystem.Advertisers.Remove(this);

			base.OnDelete();
		}

		public override void OnSpeech(SpeechEventArgs e)
		{
			base.OnSpeech(e);

			if (!e.Handled && InRange(e.Mobile, 6))
			{
				string text = e.Speech.ToLower();
				Mobile m = e.Mobile;

				if (text == "go" && AdvertSystem.CurrentPublicAdd != null)
					AdvertSystem.UseAdvert(m, AdvertSystem.CurrentPublicAdd);

				else if (text == "show adds")
				{
					if (!m.HasGump(typeof(AdvertCategoriesGump)) && !m.HasGump(typeof(AdvertEntriesGump)))
						m.SendGump(new AdvertCategoriesGump());
				}

				else if (text == "i wish to place a new advertisement")
				{
					if (!m.HasGump(typeof(AddVendorGump)))
					m.SendGump(new AddVendorGump(m));
				}

				else if (text == "i wish to review my advertisements")
				{
					if (!m.HasGump(typeof(AdvertCategoriesGump)) && !m.HasGump(typeof(AdvertEntriesGump)))
					m.SendGump(new AdvertEntriesGump(null, 0, m));
				}
			}
		}

		public AdvertMobile(Serial serial)
			: base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(0); // version;
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();

			if (!AdvertSystem.Advertisers.Contains(this))
				AdvertSystem.Advertisers.Add(this);
		}

		public class InformationEntry : ContextMenuEntry
		{
			private Mobile m_From;

			public InformationEntry(Mobile from)
				: base(0098, 12)
			{
				m_From = from;
			}

			public override void OnClick()
			{
				m_From.SendGump(new InfoGump());
			}
		}

		public class TalkEntry : ContextMenuEntry
		{
			private Mobile m_From;

			public TalkEntry(Mobile from)
				: base(6146, 12)
			{
				m_From = from;
			}

			public override void OnClick()
			{
				m_From.SendGump(new AdvertCategoriesGump());
			}
		}
	}
}