using System;
using Server.Mobiles;

namespace Server.Events.CrateRace
{
	public class FlyingBomb : Item
	{
		public int X_Move = 0;
		public int Y_Move = 0;
		public Point3D StartLoc;

		public FlyingBomb(Mobile from) : base()
		{
			StartLoc = new Point3D(from.Location);
			int x = 0;
			int y = 0;

			switch ((int)from.Direction)
			{
				case 130:
				case (int)Direction.East: x = 100; X_Move = 1; break;
				case 131:
				case (int)Direction.Down: y = 100; x = 100; X_Move = 1; Y_Move = 1; break;
				case 133:
				case (int)Direction.Left: y = 100; x = -100; X_Move = 1; Y_Move = -1; break;
				case (int)Direction.ValueMask:
				case (int)Direction.Mask: y = -100; x = -100; X_Move = -1; Y_Move = -1; break;
				case (int)Direction.Running:
				case (int)Direction.North: y = -100; Y_Move = -1; break;
				case 129:
				case (int)Direction.Right: y = -100; x = 100; X_Move = -1; Y_Move = 1; break;
				case 132:
				case (int)Direction.South: y = 100; Y_Move = 1; break;
				case 134:
				case (int)Direction.West: x = -100; X_Move = -1; break;
			}

			MoveToWorld(new Point3D(from.X + x, from.Y + y, from.Z), CrateRace.Map);

			Effects.SendMovingEffect(from, this, 10249, 7, 10, true, false);
			new InternalTimer(this).Start();
		}

		public FlyingBomb( Serial serial ) : base( serial )
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

		private class InternalTimer : Timer
		{
			private FlyingBomb m_Bomb;
			private Point3D m_Point;

			public InternalTimer(FlyingBomb bomb)
				: base(TimeSpan.FromSeconds(0.10), TimeSpan.FromSeconds(0.10), 30)
			{
				m_Bomb = bomb;
				m_Point = m_Bomb.StartLoc;
			}

			protected override void OnTick()
			{
				if (m_Bomb == null || m_Bomb.Deleted)
					return;

				m_Point.X += m_Bomb.X_Move;
				m_Point.Y += m_Bomb.Y_Move;

				IPooledEnumerable ip = m_Bomb.Map.GetObjectsInRange(m_Point, 0);

				bool hitsomething = false;
				foreach (object obj in ip)
				{
					//if (obj is Item || obj is StaticTile)//not working fine
					//{
					//	m_Bomb.Location = m_Point;
					//	Effects.SendTargetParticles(m_Bomb, 0x36BD, 20, 10, 5044, EffectLayer.Head);
					//	hitsomething = true;
					//}

					if (obj is PlayerMobile)
					{
						PlayerMobile pm = (PlayerMobile)obj;
						RaceData rd = null;

						if (CrateRace.PartData.TryGetValue(pm, out rd))
						{
							pm.FixedParticles(0x36BD, 20, 10, 5044, EffectLayer.Head);
							pm.PlaySound(0x307);
							Timer.DelayCall(TimeSpan.Zero, new TimerStateCallback(CrateRace.HandleDamage), new object[] { rd, .20 });
							hitsomething = true;
						}
					}
				}
				if (hitsomething)
				{
					m_Bomb.Location = m_Point;
					m_Bomb.Delete();
					Stop();
				}
			}
		}
	}

	public class SlickPatch : Item
	{
		public SlickPatch()
			: base(0x914)
		{
			Hue = 1736;
			Timer.DelayCall(TimeSpan.FromSeconds(10.0),Delete);
		}

		public override bool OnMoveOver(Mobile m)
		{
			RaceData rd = null;

			if (CrateRace.PartData.TryGetValue(m, out rd))
			{
				rd.Frozen = true;
				CrateRace.SetPlayerSpeed(rd);
				rd.Part.SendMessage("Your movement has slowed down as you have stepped into a slick patch");
				Timer.DelayCall(TimeSpan.FromSeconds(6), new TimerStateCallback(StopFreeze),  rd );
				Delete();
			}

			return true;
		}

		private static void StopFreeze(object o)
		{
			RaceData rd = (RaceData)o;
			if (CrateRace.Running && rd != null)
			{
				rd.Frozen = false;
				CrateRace.SetPlayerSpeed(rd);
			}
		}

		public SlickPatch(Serial serial)
			: base(serial)
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