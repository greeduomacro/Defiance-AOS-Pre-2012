using System;
using Server.Network;

namespace Server.Events.CrateRace
{
	public abstract class CrateType
	{
		public abstract int Hue { get; }
		public virtual int GumpID { get { return 0; } }

		public virtual void OnMoveOver(Mobile m) { }
		public virtual void OnUsage(Mobile m) { }
	}

	public abstract class SingleUseCrateType : CrateType
	{
	}

	public class ExplosionCrateType : CrateType
	{
		public override int Hue { get { return 1161; } }
		public override int GumpID { get { return 2282; } }

		public override void OnUsage(Mobile m)
		{
			RaceCrate crate = new RaceCrate();
			crate.m_Type = new ExploCrateType(m, crate);
			crate.MoveToWorld(m.Location, m.Map);
			CrateRace.OpenCrates--;
		}
	}

	public class SlickCrateType : CrateType
	{
		public override int Hue { get { return 1736; } }
		public override int GumpID { get { return 2240; } }

		public override void OnUsage(Mobile m)
		{
			SlickPatch patch = new SlickPatch();
			patch.MoveToWorld(m.Location, m.Map);
			CrateRace.OpenCrates--;
		}
	}

	public class ExploCrateType : SingleUseCrateType
	{
		public override int Hue { get { return 33; } }
		public Mobile Mob;
		private RaceCrate m_Crate;

		public ExploCrateType(Mobile m, RaceCrate crate)
		{
			m_Crate = crate;
			Mob = m;
			Timer.DelayCall(TimeSpan.FromSeconds(3), new TimerCallback(OnSelfDestruct));
		}

		public override void OnMoveOver(Mobile m)
		{
			if (m != Mob)
				OnSelfDestruct();
		}

		private void OnSelfDestruct()
		{
			if (m_Crate != null && !m_Crate.Deleted)
			{
				foreach (Mobile m in m_Crate.Map.GetMobilesInRange(m_Crate.Location, 3))
				{
					RaceData rd = null;

					if (CrateRace.PartData.TryGetValue(m, out rd))
					{
						m.SendMessage("You have been hit by a dropbomb!");
						m.FixedParticles(0x36BD, 20, 10, 5044, EffectLayer.Head);
						m.PlaySound(0x307);
						Timer.DelayCall(TimeSpan.Zero, new TimerStateCallback(CrateRace.HandleDamage), new object[] { rd, .30 });
					}
				}

				m_Crate.Delete();
			}
		}
	}

	public class HealingCrateType : SingleUseCrateType
	{
		public override int Hue { get { return 53; } }

		public override void OnMoveOver(Mobile m)
		{
			m.Hits = m.HitsMax;
			m.FixedParticles(0x376A, 9, 32, 5030, EffectLayer.Waist);
			m.PlaySound(0x202);
			m.SendMessage("The crate has healed all of your wounds!");
		}
	}

	public class TempMirrorCrateType : SingleUseCrateType
	{
		public override int Hue { get { return 1154; } }

		public override void OnMoveOver(Mobile m)
		{
			RaceData rd = null;

			if (CrateRace.PartData.TryGetValue(m, out rd))
			{
				rd.TempMirror = true;
				m.PlaySound(0x1E9);
				m.FixedParticles(0x375A, 9, 20, 5016, EffectLayer.Waist);
				Timer.DelayCall(TimeSpan.FromSeconds(20), new TimerStateCallback(TempMirrEnd), new object[] { rd,  });
			}
		}

		private static void TempMirrEnd(object data)
		{
			object[] Data = (object[])data;
		   // RaceCrate  = (RaceCrate)Data[1];
			RaceData rd = (RaceData)Data[0];

			if (CrateRace.Running)
				rd.TempMirror = false;
		}
	}

	public class MirrorCrateType : SingleUseCrateType
	{
		public override int Hue { get { return 1150; } }

		public override void OnMoveOver(Mobile m)
		{
			RaceData rd = null;

			if (CrateRace.PartData.TryGetValue(m, out rd))
			{
				rd.Mirror = true;
				m.PlaySound(0x1E9);
				m.FixedParticles(0x375A, 9, 20, 5016, EffectLayer.Waist);
			}
		}
	}

	public class DamagedCrateType : SingleUseCrateType
	{
		public override int Hue { get { return 0; } }

		public override void OnMoveOver(Mobile m)
		{
			RaceData rd = null;

			if (CrateRace.PartData.TryGetValue(m, out rd))
			{
				m.FixedParticles(0x36BD, 20, 10, 5044, EffectLayer.Head);
				m.PlaySound(0x307);
				CrateRace.HandleDamage(rd, .15);
				m.SendMessage("The crate blew up when you came too close!");
			}
		}
	}

	public class FreezeCrateType : CrateType
	{
		public override int Hue { get { return 1312; } }
		public override int GumpID { get { return 2277; } }

		public override void OnUsage(Mobile m)
		{
			foreach (RaceData rd in CrateRace.PartList)
			{
				if (rd.Part != m)
				{
					if (rd.UsedMirror())
						rd.Part.SendMessage("Your shield has blocked a paralyze crate.");

					else
					{
						rd.Frozen = true;
						CrateRace.SetPlayerSpeed(rd);
						rd.Part.SendMessage("You have been frozen by {0}", m.Name);
					}
				}

				else
					m.SendMessage("You have crippled your opponents!");
			}
			CrateRace.OpenCrates--;
			if (CrateRace.FreezeTimer != null)
				CrateRace.FreezeTimer.Stop();
			CrateRace.FreezeTimer = Timer.DelayCall(TimeSpan.FromSeconds(3), new TimerCallback(CrateRace.StopFreeze));
		}
	}

	public class EarthQuakeCrateType : CrateType
	{
		public override int Hue { get { return 2430; } }
		public override int GumpID { get { return 2296; } }

		public override void OnUsage(Mobile m)
		{
			foreach (Mobile mob in m.GetMobilesInRange(5))
			{
				RaceData rd = null;

				if (CrateRace.PartData.TryGetValue(mob, out rd))
				{
					if (mob != m)
					{
						mob.SendMessage("You feel the earth below you tremble!");
						Timer.DelayCall(TimeSpan.Zero, new TimerStateCallback(CrateRace.HandleDamage), new object[] { rd, .50 });
					}

					else
					{
						m.SendMessage("You make the earth swing from one side to another!");
						m.PlaySound(0x2F3);
					}
				}
			}
			CrateRace.OpenCrates--;
		}
	}

	public class EnergyBoltCrateType : CrateType
	{
		public override int Hue { get { return 1266; } }
		public override int GumpID { get { return 2281; } }

		public override void OnUsage(Mobile m)
		{
			if (CrateRace.FirstPlace != null)
			{
				if (CrateRace.PartList.Count > 0)
				{
					RaceData rd = CrateRace.PartList[0];

					m.MovingParticles(rd.Part, 0x379F, 7, 0, false, true, 3043, 4043, 0x211);
					m.PlaySound(0x20A);
					rd.Part.SendMessage("You have been hit by an enormous energy blast!");
					CrateRace.HandleDamage(rd, .50);
				}
				m.SendMessage("The first in place has been hit by an enormous energy blast!");
			}

			CrateRace.OpenCrates--;
		}
	}

	public class ChainLightningCrateType : CrateType
	{
		public override int Hue { get { return 2124; } }
		public override int GumpID { get { return 2288; } }

		public override void OnUsage(Mobile m)
		{
			foreach (RaceData rd in CrateRace.PartData.Values)
			{
				if (rd.Part != m)
				{
					rd.Part.SendMessage("You have been lightened up by {0}", m.Name);
					rd.Part.BoltEffect(0);
					CrateRace.HandleDamage(rd, .25);
				}

				else
					m.SendMessage("You have crippled your opponents!");
			}
			CrateRace.OpenCrates--;
		}
	}

	public class MovingBombCrateType : CrateType
	{
		public override int Hue { get { return 2129; } }
		public override int GumpID { get { return 2266; } }

		public override void OnUsage(Mobile m)
		{
			new FlyingBomb( m);
			CrateRace.OpenCrates--;
		}
	}
}