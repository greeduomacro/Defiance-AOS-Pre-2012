//////////////////////////////\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
//			   This is a Sunny Production ©2005					\\
//					 Based on RunUO©							\\
//					Version: Alpha 1.0							\\
//		Many thanks to my Csharp tutors Will and Eclipse		\\
//////////////////////////////\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
//

using System;
using Server.Items;

namespace Server.Events.CrateRace
{
	public class RaceFinishLine : Item
	{
		private static bool m_White;
		private Static m_Static;

		[CommandProperty(AccessLevel.GameMaster)]
		public Static Static
		{
			get { return m_Static; }
			set { m_Static = value; }
		}

		[Constructable]
		public RaceFinishLine() : base (0x709)
		{
			m_White = !m_White;
			Hue = m_White ? 1153 : 2333;
			Movable = false;

			Timer.DelayCall(TimeSpan.Zero, new TimerCallback(AddStatic_CallBack));
		}

		private void AddStatic_CallBack()
		{
			m_Static = new Static(0x70D);
			m_Static.MoveToWorld(new Point3D(X - 1, Y, Z), Map);
			m_Static.Hue = Hue == 1153 ? 2333 : 1153;
		}

		public override void OnDelete()
		{
			if (m_Static != null && !m_Static.Deleted)
				m_Static.Delete();

			base.OnDelete();
		}

		public override bool HandlesOnMovement { get { return true; } }

		//Needs to be changed accordingly to rectangle direction
		public override void OnMovement(Mobile m, Point3D oldLocation)
		{
			if (CrateRace.Running && oldLocation.X == X && oldLocation.Y == Y)
			{
				if (m.X == X + 1)
					CrateRace.MobFinish(m);
				//check if it is a finish or minusfinish, not done well yet

				/*if (
				m_Stone.MobFinish(m, m_Stone.GetRectangleID(m.Location.X, m.Location.Y) != 0))*/
					//return true;//m.MoveToWorld(new Point3D(X + 1, Y, Z), Map);//needs to be directed by rectangle
			}
			//return false;
		}

		public RaceFinishLine( Serial serial ) : base( serial )
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write(0);//version

			writer.Write((Item)m_Static);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadInt();

			switch (version)
			{
				case 0:
					m_Static = (Static)reader.ReadItem();
					break;
			}
		}
	}
}