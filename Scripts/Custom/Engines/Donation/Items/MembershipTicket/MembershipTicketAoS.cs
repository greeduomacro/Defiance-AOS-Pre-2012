using System;
using Server.Network;
using Server.Items;
using Server.Mobiles;
using Server.Accounting;

namespace Server.Items
{
	public class MembershipTicketAoS : Item
	{
		private TimeSpan m_MemberShipTime;

		[CommandProperty(AccessLevel.Administrator)]
		public TimeSpan MemberShipTime
		{
			get { return m_MemberShipTime; }
			set
			{
				m_MemberShipTime = value;

				if (m_MemberShipTime == TimeSpan.MaxValue)
				{
					Name = "a ticket for permanent membership";
				}
				else
				{
					Name = "a memebership ticket for " + m_MemberShipTime.Days + " days";
				}
			}
		}

		[Constructable]
		public MembershipTicketAoS()
			: base(0x14F0)
		{
			Weight = 1.0;
			Name = "a membership ticket";
			LootType = LootType.Blessed;
		}

		public MembershipTicketAoS(Serial serial)
			: base(serial)
		{
		}

		public override void OnDoubleClick(Mobile from)
		{
			PlayerMobile m = (PlayerMobile)from;

			if (m != null && m.NetState != null)
			{
				if (m.Backpack != null && IsChildOf(m.Backpack))
				{
					DateTime DonationStart = DateTime.MinValue;
					TimeSpan DonationDuration = TimeSpan.Zero;

					try
					{
						DonationStart = DateTime.Parse(((Account)from.Account).GetTag("DonationStart"));
						DonationDuration = TimeSpan.Parse(((Account)from.Account).GetTag("DonationDuration"));
					}
					catch
					{
					}


					if (DonationStart == DateTime.MinValue && DonationDuration == TimeSpan.Zero)
					{
						try
						{
							((Account)from.Account).SetTag("DonationStart", DateTime.Now.ToString());
							((Account)from.Account).SetTag("DonationDuration", m_MemberShipTime.ToString());
							from.SendMessage("Your donation status has been updated.");
							this.Delete();
						}
						catch
						{
							from.SendMessage("An error ocurred trying to update your donation status. Contact an Administrator.");
						}
					}
					else if (DonationDuration == TimeSpan.MaxValue)
					{
						//already at max
						from.SendMessage("You are already at permanent membership status.");
					}
					else
					{
						//existing donation


						try
						{
							//Avoid overflow
							if (m_MemberShipTime == TimeSpan.MaxValue)
								DonationDuration = m_MemberShipTime;
							else
								DonationDuration += m_MemberShipTime;

							((Account)from.Account).SetTag("DonationDuration", DonationDuration.ToString());
							from.SendMessage("Your donation status has been updated.");
							this.Delete();
						}
						catch
						{
							from.SendMessage("An error ocurred trying to update your donation status. Contact an Administrator.");
						}
					}
				}
				else
				{
					from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
				}
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
		}
	}
}