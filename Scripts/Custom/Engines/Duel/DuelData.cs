//////////////////////////////\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
//			   This is a Sunny Production ©2005			\\
//					 Based on RunUO©				\\
//					Version: Alpha 1.0				\\
//		Many thanks to my Csharp tutors Will and Eclipse		\\
//////////////////////////////\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\

using Server.Mobiles;
using System;

namespace Server.Events.Duel
{
	public class DuelData : IComparable
	{
		public int Points;
		public int Rank;
		public int Wins;
		public int Losses;
		public Mobile Part;

		public DuelData()
		{
		}

		public DuelData(Mobile part, int points)
		{
			Part = part;
			Points = points;
			Wins = 0;
			Losses = 0;
		}

		public int CompareTo(object obj)
		{
			if (obj is DuelData)
			{
				DuelData dd = (DuelData)obj;
				if (dd.Points == Points)
				{
					int score = (dd.Wins - dd.Losses) - (Wins - Losses);
					if (score == 0)
						return dd.Wins - Wins;
					else return score;
				}
				return dd.Points - Points;
			}
			else
				throw new ArgumentException("object is not DuelData");
		}

		public void Serialize( GenericWriter writer )
		{
			writer.Write(Points);
			writer.Write(Wins);
			writer.Write(Losses);
			writer.Write(Part);
		}

		public void Deserialize( GenericReader reader )
		{
			Points = reader.ReadInt();
			Wins = reader.ReadInt();
			Losses = reader.ReadInt();
			Part = reader.ReadMobile();
		}
	}

}