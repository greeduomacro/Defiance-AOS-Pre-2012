namespace Server.Items
{
	public class NonCriminalTeleporter : Teleporter
	{
		public override bool OnMoveOver(Mobile m)
		{
			if (Active)
			{
				if ( !Creatures && !m.Player )
					return true;
				else if (m.Alive && (m.Criminal || Server.Spells.SpellHelper.CheckCombat(m) || m.Spell != null))
				{
					m.SendLocalizedMessage( 1005564, "", 0x22 ); // Wouldst thou flee during the heat of battle??
					return true;
				}
				else if ( Factions.Sigil.ExistsOn( m ) )
				{
					m.SendLocalizedMessage( 1061632 ); // You can't do that while carrying the sigil.
					return true;
				}

				StartTeleport(m);
				return false;
			}

			return true;
		}

		[Constructable]
		public NonCriminalTeleporter()
		{
		}

		public NonCriminalTeleporter(Serial serial)
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