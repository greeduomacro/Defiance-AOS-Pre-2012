using Server.Mobiles;
using Server.Gumps;
using Server.Network;

namespace Server.Items
{
	public class SexChangeDeedAOS : DonationItem
	{
		[Constructable]
		public SexChangeDeedAOS()
			: this(false)
		{
		}

		[Constructable]
		public SexChangeDeedAOS(bool donated)
			: base(0x14F0, donated)
		{
			Name = "sex change deed";
			if (donated)
				Hue = 1195;
		}

		public SexChangeDeedAOS(Serial serial)
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

		public override void OnDoubleClick(Mobile from)
		{
			if (from == null || from.Deleted || from.Backpack == null)
				return;

			if (!IsChildOf(from.Backpack))
			{
				from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
				return;
			}

			from.SendGump(new SexChangeConfirmGump(from, this));
		}

		protected void Use(Mobile from)
		{
			from.SendMessage("You feel your body proportions change.");

			from.BodyValue = (from.BodyValue == 400 ? 401 : 400);
			from.Female = !from.Female;

			if (from.Female)
			{
				if (from.FacialHairItemID > 0)
				{
					from.SendMessage("You feel a sharp pain as your beard is torn off by invisible forces.");
					from.FacialHairItemID = 0;
				}
			}
			Delete();
		}

		private class SexChangeConfirmGump : Gump
		{
			Mobile m_Mobile;
			SexChangeDeedAOS m_Deed;
			public SexChangeConfirmGump(Mobile from, SexChangeDeedAOS deed)
				: base(200, 200)
			{
				m_Deed = deed;
				m_Mobile = from;

				Resizable = false;
				AddPage(0);
				AddBackground(0, 0, 300, 120, 9270);

				AddLabel(28, 15, 255, "Changing sex involves intense magic.");
				AddLabel(28, 31, 255, "Your facial hair, if any, will dissapear.");
				AddLabel(28, 47, 255, "Are you sure you want to proceed?");

				AddButton(28, 70, 4005, 4007, 1, GumpButtonType.Reply, 0);
				AddLabel(80, 70, 255, "Yes");

				AddButton(130, 70, 4005, 4007, 0, GumpButtonType.Reply, 0);
				AddLabel(182, 70, 255, "No");

				//this.AddItem(12, 38, 4171);
			}

			public override void OnResponse(NetState state, RelayInfo info)
			{
				switch (info.ButtonID)
				{
					case 1: // Yes
						{
							if (m_Mobile == null || m_Mobile.Deleted || m_Mobile.Backpack == null || m_Deed == null || m_Deed.Deleted)
								return;

							if (!m_Deed.IsChildOf(m_Mobile.Backpack))
							{
								m_Mobile.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
								return;
							}

							m_Deed.Use(m_Mobile);

							break;
						}
				}
			}
		}
	}
}