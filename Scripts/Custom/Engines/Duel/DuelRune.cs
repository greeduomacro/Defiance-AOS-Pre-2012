using System;
using Server.Mobiles;


namespace Server.Events.Duel
{
	public class DuelRune
	{
		//m_NoSummons, m_NoArea ,m_NoMounts ,m_NoPots, m_NoArts, m_NoMagArmor, m_NoMagWeps, m_NoPoisWeps, m_SpellWatch;
		public bool[] Options = new bool[9];
		public Mobile[] Participants = new Mobile[2];
		public Mobile Starter{get{return Participants[0];}}
		public bool[] Accepted = new bool[3];
		public DuelType DType;
		public Dueller Npc;

		public DuelRune(Mobile starter, Dueller dueller)
		{
			Participants[0] = starter;
			Npc = dueller;
		}

		public bool PassedBalanceCheck()
		{
			bool passed = true;

			foreach (Mobile m in Participants)
				if (!(Banker.GetBalance(m) > DuelSystem.GoldCost))
					passed = false;

			return passed;
		}

		public bool PassedNullCheck()
		{
			bool passed = true;

			foreach (Mobile m in Participants)
				if (m == null)
					passed = false;

			return passed;
		}

		public bool PassedSkillCheck()
		{
			bool passed = true;

			foreach (Mobile m in Participants)
				if (!DType.PassedSkillCheck(m))
					passed = false;

			return passed;
		}

		public bool PassedAliveCheck()
		{
			foreach (Mobile m in Participants)
				if (!m.Alive)
					return false;

			return true;
		}

		public bool PassedCurrentDuellerCheck()
		{
			foreach (Mobile m in Participants)
				foreach(Dueller dueller in DuelSystem.Duellers)
					if(dueller.Participants != null)
						foreach(Mobile p in dueller.Participants)
							if(m == p)
								return false;

			return true;
		}

		public void ChangeType(DuelType type)
		{
			DType = type;
			type.SetOptions(this);
			if (type is DDDuelType)
				Array.Resize(ref Participants, 4);
		}
	}
}