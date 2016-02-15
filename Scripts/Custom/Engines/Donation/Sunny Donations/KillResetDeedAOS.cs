using System;
using Server;
using Server.Mobiles;
using Server.Items;

namespace Server.Items
{
	public class KillResetDeedAOS : DonationItem
	{
		[Constructable]
		public KillResetDeedAOS() :this(false)
		{
		}

		[Constructable]
		public KillResetDeedAOS(bool donated):base(5360, donated)
		{
			Weight = 1.0;
			Name = "Kill Reset Deed";
			if (donated)
				Hue = 1194;
		}

		public KillResetDeedAOS(Serial serial)
			: base(serial)
		{
		}

		public override void OnDoubleClick(Mobile from)
		{
			{
				if (IsChildOf(from.Backpack))
				{
					if (from.ShortTermMurders == 0 && from.Kills == 0)
					{
						from.SendMessage("You have no murders to disolve.");
					}
					else
					{
						from.ShortTermMurders = 0;
						from.Kills = 0;
						from.SendMessage("Your long and short term murders have been disolved.");
						Delete();
					}
				}
				else
					from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
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