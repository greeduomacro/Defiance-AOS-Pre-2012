using System;
using Server.Targeting;
using Server.Mobiles;
using Server.Factions;


namespace Server.Items
{
	class WarHorseBondingDeed : Item
	{
		public override string DefaultName
		{
			get { return "a faction warhorse bonding deed"; }
		}

		[Constructable]
		public WarHorseBondingDeed() : base( 0x14F0 )
		{
			Weight = 1.0;
		}

		public WarHorseBondingDeed( Serial serial ) : base( serial )
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

		public override void OnDoubleClick(Mobile from)
		{
			if (from == null || from.Deleted || from.Backpack == null)
				return;

			if (!IsChildOf(from.Backpack))
			{
				from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
				return;
			}

			from.SendMessage("Target a faction warhorse that you want bonded.");
			from.Target = new WarHorseBondTarget(this);
		}

		private class WarHorseBondTarget : Target
		{
			private WarHorseBondingDeed m_Deed;

			public WarHorseBondTarget(WarHorseBondingDeed deed)
				: base(-1, false, TargetFlags.None)
			{
				m_Deed = deed;
			}

			protected override void OnTarget(Mobile from, object targeted)
			{
				if (from == null || from.Deleted || from.Backpack == null || m_Deed == null || m_Deed.Deleted)
					return;

				if (targeted is FactionWarHorse)
				{
					FactionWarHorse creature = (FactionWarHorse)targeted;
					if (creature.ControlMaster == from && creature.Controlled)
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
							from.SendMessage("That creature is already bonded!");
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