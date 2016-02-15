using System;
using Server.Targeting;
using Server.Mobiles;

namespace Server.Items
{
	public class PetBondingStone : Item
	{
		[Constructable]
		public PetBondingStone()
			: base(0xEDE)
		{
			Movable = false;
			Hue = 0x482;
			Name = "a pet bonding stone";
		}

		public override void OnDoubleClick(Mobile from)
		{
			if (!from.InRange(GetWorldLocation(), 2))
			{
				from.SendLocalizedMessage(500446); // That is too far away.
				return;
			}

			from.SendMessage("Target a tamed creature that you want bonded.");
			from.Target = new PetBondTarget();
		}

		private class PetBondTarget : Target
		{
			public PetBondTarget()
				: base(-1, false, TargetFlags.None)
			{
			}

			protected override void OnTarget(Mobile from, object targeted)
			{
				if (targeted is BaseCreature)
				{
					BaseCreature creature = (BaseCreature)targeted;
					if (creature.ControlMaster == from && creature.Controlled && creature.IsBondable)
					{
						if (!creature.IsBonded)
						{
							creature.IsBonded = true;
							creature.BondingBegin = DateTime.MinValue;
							from.SendLocalizedMessage(1049666); // Your pet has bonded with you!
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

		public PetBondingStone(Serial serial)
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
	}
}