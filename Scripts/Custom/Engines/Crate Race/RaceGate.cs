//////////////////////////////\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
//			   This is a Sunny Production ©2005					\\
//					 Based on RunUO©							\\
//					Version: Alpha 1.0							\\
//		Many thanks to my Csharp tutors Will and Eclipse		\\
//////////////////////////////\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
//

namespace Server.Events.CrateRace
{
	public class RaceGate : Item
	{
		private CrateStone m_Stone;

		[Constructable]
		public RaceGate(CrateStone stone) : base(0xF6C)
		{
			m_Stone = stone;
			Movable = false;
			Hue = 1150;
		}

		public override bool OnMoveOver(Mobile m)
		{
			if (m.Player)
			{
				m.CloseGump(typeof(CrateJoinGump));
				m.SendGump(new CrateJoinGump());
			}
			return true;
		}

		public RaceGate(Serial serial) : base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			Delete();
		}
	}
}