//////////////////////////////\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
//			   This is a Sunny Production ©2005					\\
//					 Based on RunUO©							\\
//					Version: Beta 1.0							\\
//		Many thanks to my Csharp tutors Will and Eclipse		\\
//////////////////////////////\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
//need to find white hue for gumps.

using Server.Items;

namespace Server.Events.Duel
{
	public class DuelStone : ConfirmationMoongate
	{
		[Constructable]
		public DuelStone() : base(Point3D.Zero, null)
		{
			MessageString = "This teleporter will bring you to the special Duel Arena. Select OKAY if you want to go there, or CANCEL if you'd rather stay here.<br><br>The area around the Duel Arena is safe and you can always come back.";
			GumpHeight = 210;
			GumpWidth = 300;
			TitleColor = 30720;
			MessageColor = 32500;
			TitleNumber = 1019005;
			Dispellable = false;
			Hue = 1122;
			Name = "Teleporter to the Duel Arena";
			TargetMap = Map.Malas;
			Target = (new Point3D(1392, 977, -88));
		}

		public override bool ValidateUse( Mobile m, bool message ) // Added by Silver
		{
			if (m.Criminal)
			{
				m.SendLocalizedMessage(1005561, "", 0x22); // Thou'rt a criminal and cannot escape so easily.
				return false;
			}
			else if (Server.Spells.SpellHelper.CheckCombat(m))
			{
				m.SendLocalizedMessage(1005564, "", 0x22); // Wouldst thou flee during the heat of battle??
				return false;
			}
			else if (m.Spell != null)
			{
				m.SendLocalizedMessage(1049616); // You are too busy to do that at the moment.
				return false;
			}

			return base.ValidateUse( m, message );
		}

		public override void BeginConfirmation(Mobile m)
		{
			if (m.Criminal)
				m.SendLocalizedMessage(1005561, "", 0x22); // Thou'rt a criminal and cannot escape so easily.

			else if (Server.Spells.SpellHelper.CheckCombat(m))
				m.SendLocalizedMessage(1005564, "", 0x22); // Wouldst thou flee during the heat of battle??

			else if (m.Spell != null)
				m.SendLocalizedMessage(1049616); // You are too busy to do that at the moment.

			else
				base.BeginConfirmation(m);
		}

		public DuelStone(Serial serial) : base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.WriteEncodedInt((int)0); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadEncodedInt();
		}
	}
}