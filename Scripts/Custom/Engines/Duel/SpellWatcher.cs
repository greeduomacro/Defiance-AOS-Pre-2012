//////////////////////////////\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
//			   This is a Sunny Production ©2005					\\
//					 Based on RunUO©							\\
//					Version: Alpha 1.0							\\
//		Many thanks to my Csharp tutors Will and Eclipse		\\
//////////////////////////////\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
//
using Server.Mobiles;
using Server.Spells;
using Server.Spells.First;
using Server.Spells.Second;
using Server.Spells.Third;
using Server.Spells.Fourth;
using Server.Spells.Chivalry;
using System;

namespace Server.Events.Duel
{
	public class SpellWatcher
	{
		public Mobile m_Part;
		public int m_CastCount;
		public Type m_Spell;

		private static Type[] m_BeneficialTypes = new Type[]
			{
				typeof(HealSpell),
				typeof(CureSpell),
				typeof(CunningSpell),
				typeof(AgilitySpell),
				typeof(StrengthSpell),
				typeof(BlessSpell),
				typeof(GreaterHealSpell),
				typeof(CleanseByFireSpell),
				typeof(CloseWoundsSpell),
				typeof(RemoveCurseSpell)
			};

		public SpellWatcher(Mobile m)
		{
			m_Part = m;
		}

		public bool SpellAllowed(ISpell s)
		{
			Type stype = s.GetType();

			foreach (Type type in m_BeneficialTypes)
				if (type == stype)
				{
					m_Spell = s.GetType();
					return true;
				}

			if (s.GetType() == m_Spell)
				m_CastCount++;

			else
			{
				m_Spell = s.GetType();
				m_CastCount = 1;
			}

			if (m_CastCount > 3)
			{
				m_Part.SendMessage("You have already tried to cast that spell three times in a row!");
				return false;
			}

			return true;
		}
	}
}