//////////////////////////////\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
//			   This is a Sunny Production ©2005					\\
//					 Based on RunUO©							\\
//					Version: Alpha 1.0							\\
//		Many thanks to my Csharp tutors Will and Eclipse		\\
//////////////////////////////\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
//
using Server.Mobiles;
using System;

namespace Server.Events.CrateRace
{
	public class RaceData : IComparable
	{
		public Mobile Part;
		public bool Mirror, TempMirror, SwitchGump, Frozen, SpeedUp;
		public CrateType Crate1, Crate2, Crate3;
		public int Place, Lap, Xaxe, Yaxe;

		public RaceData(Mobile m)
		{
			Part = m;
		}

		public bool UsedMirror()
		{
			if (TempMirror)
			{
				TempMirror = false;
				return true;
			}

			if (Mirror)
			{
				Mirror = false;
				return true;
			}

			return false;
		}

		public int CompareTo(object obj)
		{
			if (obj is RaceData)
			{
				RaceData rd = (RaceData)obj;

				if (Lap != rd.Lap)
					return (rd.Lap) - (Lap);
				else
					return CrateRace.GetRectangleID(rd.Part.X, rd.Part.Y) - CrateRace.GetRectangleID(Part.X, Part.Y);
			}
			else
				throw new ArgumentException("object is not RaceData");
		}
	}

	public class FinishedData : IComparable
	{
		public Mobile m_FinPart;
		public int m_Place;
		public TimeSpan m_Time;

		public FinishedData(Mobile m, int place, TimeSpan time)
		{
			m_FinPart = m;
			m_Place = place;
			m_Time = time;
		}

		public int CompareTo(object obj)
		{
			if (obj is FinishedData)
			{
				FinishedData fd = (FinishedData)obj;

				return m_Place - fd.m_Place;
			}

			else
				throw new ArgumentException("object is not FinishedData");
		}
	}
}