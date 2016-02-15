//////////////////////////////\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
//			   This is a Sunny Production ©2005					\\
//					 Based on RunUO©							\\
//					Version: Alpha 1.0							\\
//		Many thanks to my Csharp tutors Will and Eclipse		\\
//////////////////////////////\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
//

using System;
using Server.Items;
using Server.Gumps;

namespace Server.Events.CrateRace
{
	public class CrateApple : Item
	{
		private bool m_Health;

		[Constructable]
		public CrateApple() : base(2512)
		{
			Hue = 68;

			if (Utility.RandomDouble() < .20)
			{
				Hue = 53;
				m_Health = true;
			}

			Name = "Stamina apple";
			Movable = false;
		}

		public override bool OnMoveOver(Mobile m)
		{
			if (m_Health)
				m.Hits += (int)(m.HitsMax * 0.1);
			else
				m.Stam += (int)(m.StamMax * 0.1);
			Delete();
			return true;
		}

		public override void OnDelete()
		{
			if(CrateRace.Running)
				CrateRace.AppleList.Remove(this);
		}

		public CrateApple( Serial serial ) : base( serial )
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