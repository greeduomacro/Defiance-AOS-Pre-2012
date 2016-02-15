using System;
using Server.Targeting;
using Server.Mobiles;

namespace Server.Items
{
	class PetBondingDeedAOS : DonationItem
	{
		[Constructable]
		public PetBondingDeedAOS()
			: this(false)
		{
		}

		[Constructable]
		public PetBondingDeedAOS(bool donated)
			: base(0x14F0, donated)
		{
			Weight = 1.0;
			Name = "Pet Bonding Deed";
			if (donated)
				Hue = 1193;
		}

		public PetBondingDeedAOS(Serial serial)
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

			from.SendMessage("Target a tamed creature that you want bonded.");
			from.Target = new PetBondTarget(this);
		}

		private class PetBondTarget : Target
		{
			private PetBondingDeedAOS m_Deed;

			public PetBondTarget(PetBondingDeedAOS deed)
				: base(-1, false, TargetFlags.None)
			{
				m_Deed = deed;
			}

			protected override void OnTarget(Mobile from, object targeted)
			{
				if (from == null || from.Deleted || from.Backpack == null || m_Deed == null || m_Deed.Deleted)
					return;

				if (targeted is BaseCreature)
				{
					BaseCreature creature = (BaseCreature)targeted;
					if (creature.ControlMaster == from && creature.Controlled && creature.IsBondable)
					{
						if (!creature.IsBonded)
						{
							if (!m_Deed.IsChildOf(from.Backpack))
							{
								from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
								return;
							}

							creature.IsBonded = true;
							creature.BondingBegin = DateTime.MinValue;
							from.SendLocalizedMessage(1049666); // Your pet has bonded with you!
							m_Deed.Delete();
						}
						else
							from.SendMessage("Creature is already bonded!");
					}
					else
						from.SendMessage("You cannot bond that creature!");
				}
				else
					from.SendMessage("You cannot bond that!");
			}
		}
	}
}