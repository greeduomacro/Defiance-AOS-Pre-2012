using System;
using Server.Targeting;
using Server.Mobiles;
using Server.Accounting;
using Server.Gumps;
using Server.Network;
using Server.Multis;

namespace Server.Items
{
	public class CharacterAccountChangeDeed : Item
	{
		[Constructable]
		public CharacterAccountChangeDeed()
			: base(0x14F0)
		{
			Name = "Character account change deed";
			Weight = 1.0;
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

			from.SendMessage("Target a player to whom's account you wish to transfer this character.");
			from.Target = new InternalTarget(this);
		}

		public static int GetEmptySlot(Mobile from, Mobile to, CharacterAccountChangeDeed deed)
		{
			int emptyslot = -1;

			if (from == null || from.Deleted || from.Backpack == null || deed == null || deed.Deleted || !deed.IsChildOf(from.Backpack))
				from.SendMessage("No deed could be found in your backpack.");

			else if (to == null || from == to)
				from.SendMessage("That is no valid player to transfer your character to.");

			else
			{
				Account acc = to.Account as Account;

				if (acc == null)
					from.SendMessage("That player does not have a valid account.");

				else
				{
					bool housepass = true;
					if (BaseHouse.GetHouses(from).Count > 0)
					{
						for (int i = 0; i < acc.Length; ++i)
						{
							Mobile mob = acc[i];

							if (mob != null && BaseHouse.GetHouses(mob).Count > 0)
							{
								housepass = false;
								from.SendMessage("This action would make one account have 2 houses, please demolish it first.");
								break;
							}

						}
					}

					if (housepass)
					{
						for (int i = 0; i < acc.Length; ++i)
						{
							if (acc[i] == null)
							{
								emptyslot = i;
								break;
							}
						}
						if (emptyslot == -1)
							from.SendMessage("That player does not have any empty character slots on his account.");
					}
				}
			}

			return emptyslot;
		}

		public CharacterAccountChangeDeed(Serial serial)
			: base(serial)
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}

		private class InternalTarget : Target
		{
			private CharacterAccountChangeDeed m_Deed;

			public InternalTarget(CharacterAccountChangeDeed deed)
				: base(-1, false, TargetFlags.None)
			{
				m_Deed = deed;
			}

			protected override void OnTarget(Mobile from, object targeted)
			{
				int emptyslot = CharacterAccountChangeDeed.GetEmptySlot(from, targeted as Mobile, m_Deed);

				if (emptyslot == -1)
					from.Target = this;

				else
					from.SendGump(new CharMoveGump(targeted as Mobile, m_Deed));
			}
		}

		public class CharMoveGump : AdvGump
		{
			private CharacterAccountChangeDeed m_Deed;
			private Mobile m_To;

			public CharMoveGump(Mobile to, CharacterAccountChangeDeed deed)
				: base(true)
			{
				m_Deed = deed;
				m_To = to;

				AddBackground(0, 0, 300, 200, 9260);
				AddHtml(0, 15, 300, 25, Colorize(Center("Confirm target"), "FFFFFF"), false, false);
				AddHtml(20, 40, 260, 90, String.Format("Are you sure you want to transfer this character to {0}'s account?", to.Name), true, false);
				AddButton(20, 150, 242, 241, 0, GumpButtonType.Reply, 0);
				AddButton(220, 150, 247, 248, 1, GumpButtonType.Reply, 0);
			}

			public override void OnResponse(NetState sender, RelayInfo info)
			{
				if(info.ButtonID == 1 && m_To != null)
					m_To.SendGump(new CharAcceptGump(sender.Mobile, m_To, m_Deed));
			}
		}

		public class CharAcceptGump : AdvGump
		{
			private CharacterAccountChangeDeed m_Deed;
			private Mobile m_From;
			private Mobile m_To;
			private DateTime m_OfferReceived = DateTime.Now;

			public CharAcceptGump(Mobile from, Mobile to, CharacterAccountChangeDeed deed)
				: base(true)
			{
				m_Deed = deed;
				m_To = to;
				m_From = from;

				int slot = CharacterAccountChangeDeed.GetEmptySlot(from, to, deed);

				if (slot == -1)
					return;

				string slotwarning = slot == 5 ? " The character would be moved to the 6th slot, this slot is only supported by the newest clients." : "";
				AddBackground(0, 0, 300, 200, 9260);
				AddHtml(0, 15, 300, 25, Colorize(Center("Confirm character relocation"), "FFFFFF"), false, false);
				AddHtml(20, 40, 260, 100, String.Format("{0} has offered to move his character to your account, are you willing to accept his character?{1}", from.Name , slotwarning), true, false);
				AddButton(20, 150, 242, 241, 0, GumpButtonType.Reply, 0);
				AddButton(220, 150, 247, 248, 1, GumpButtonType.Reply, 0);
			}

			public override void OnResponse(NetState sender, RelayInfo info)
			{
				if (info.ButtonID == 1)
				{
					if (m_OfferReceived - DateTime.Now < TimeSpan.FromMinutes(2.0))
					{
						int slot = CharacterAccountChangeDeed.GetEmptySlot(m_From, m_To, m_Deed);

						if (slot != -1)
						{
							Account facc = m_From.Account as Account;
							Account tacc = m_To.Account as Account;
							if (facc != null && tacc != null)
							{
								int fromloc = -1;
								for (int i = 0; i < facc.Length; ++i)
								{
									if (facc[i] == m_From)
									{
										fromloc = i;
										break;
									}
								}

								if (fromloc != -1)
								{
									if (m_From.NetState != null)
										m_From.NetState.Dispose();

									facc[fromloc] = null;
									tacc[slot] = m_From;
									m_From.Account = tacc;
									m_To.SendMessage("Transfer completed.");
									m_Deed.Delete();
								}
								else
									m_To.SendMessage("Could not initialize one of the two accounts.");
							}
							else
								m_To.SendMessage("Could not initialize one of the two accounts.");
						}
						else
							m_To.SendMessage("Could not execute the transfer, please ask the other player to correct it.");
					}
					else
						m_To.SendMessage("Reaction has not been received in time.");
				}
			}
		}
	}
}