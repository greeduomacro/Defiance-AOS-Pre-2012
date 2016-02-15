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
	public class RaceCrate : Item
	{
		/// <summary>
		/// Should count up to 100%;
		/// </summary>
		private static double[] m_CrateChances = new double[]
			{
				0.125,//  10%
				0.075,//  5%
				0.20,//  15%
				0.05,//  2.5%
				0.10,//  7.5%
				0.075,//  2.5%
				0.05,//  2.5%
				0.075,//  5%
				0.15,//  5%
				0.1//
			};

		private static CrateType m_DamagedType = new DamagedCrateType();

		private static CrateType[] m_CrateTypes = new CrateType[]
			{
			   new TempMirrorCrateType(),//tempmirror//lichtblauw
			   new MirrorCrateType(),//mirror // wit
			   new ExplosionCrateType(),//explo // blaze
			   new FreezeCrateType(),//freeze //donkerblauw
			   new EarthQuakeCrateType(),//quake // bruin
			   new EnergyBoltCrateType(),//ebolt // neon blauw
			   new ChainLightningCrateType(),//lightning //birdblauw
			   new HealingCrateType(),//healing //geel
			   new MovingBombCrateType(),//movingbomb //??
			   new SlickCrateType() //slickbomb //bruingeel
			};

		private static double[] m_ComputedCrateChances;
		public CrateStone m_Stone;
		public CrateType m_Type;

		[Constructable]
		public RaceCrate()
			: base(3645)
		{
			Name = "Special Race Crate";
			Movable = false;

			double random = Utility.RandomDouble();
			for (int i = 0; i < m_ComputedCrateChances.Length; i++)
			{
				if (random <= m_ComputedCrateChances[i])
				{
					m_Type = m_CrateTypes[i];
					break;
				}
			}

			Hue = m_Type.Hue;

			if (m_Type == null || (Utility.RandomDouble() < 0.05 && !(m_Type is HealingCrateType))) //damage
				m_Type = m_DamagedType;
		}

		public static void Initialize()
		{
			double cumulative = 0.0;

			m_ComputedCrateChances = new double[m_CrateChances.Length];

			for (int i = 0; i < m_CrateChances.Length; i++)
			{
				cumulative += m_CrateChances[i];
				m_ComputedCrateChances[i] = cumulative;
			}
		}

		public override bool HandlesOnMovement { get { return true; } }

		public override void OnMovement(Mobile m, Point3D oldLocation)
		{
			if (Utility.InRange(m.Location, Location, 1))
			{
				RaceData rd = null;

				if (CrateRace.PartData.TryGetValue(m, out rd))
				{
					m_Type.OnMoveOver(m);

					if (!(m_Type is SingleUseCrateType))
					{
						if (rd.Crate1 == null)
							rd.Crate1 = m_Type;

						else if (rd.Crate2 == null)
							rd.Crate2 = m_Type;

						else if (rd.Crate3 == null)
							rd.Crate3 = m_Type;

						else
						{
							rd.Crate2 = rd.Crate1;
							rd.Crate3 = rd.Crate2;
							rd.Crate1 = m_Type;
							CrateRace.OpenCrates--;
						}

						CrateRace.OpenCrates++;
					}
				}
				if (!(m_Type is ExploCrateType) || m != ((ExploCrateType)m_Type).Mob)
					Delete();
			}

			base.OnMovement(m, oldLocation);
		}

		public override void OnDelete()
		{
			if (CrateRace.Running)
				CrateRace.CrateList.Remove(this);
		}

		public RaceCrate(Serial serial)
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