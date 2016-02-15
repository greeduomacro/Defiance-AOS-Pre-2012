using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Server.Items
{
	class ESpecialAbilities
	{
		#region LifeDrain
		private static Hashtable m_Table = new Hashtable();

		private static bool IsBeingDrained(Mobile m)
		{
			return m_Table.Contains(m);
		}

		public static void BeginLifeDrain(Mobile defender, Mobile from)
		{
			if (!IsBeingDrained(defender))
			{
				defender.SendLocalizedMessage(1070848); // You feel your life force being stolen away.
				defender.FixedParticles(0x3779, 10, 15, 5009, EffectLayer.Waist);

				Timer t = (Timer)m_Table[defender];

				if (t != null)
					t.Stop();

				t = new ELifeDrainTimer(from, defender);
				m_Table[defender] = t;

				t.Start();
			}
		}

		private static void DrainLife(Mobile m, Mobile from)
		{
			if (m.Alive)
			{
				int damageGiven = AOS.Damage(m, from, 5, 100, 0, 0, 0, 0);
				from.Hits += damageGiven;
			}
			else
				EndLifeDrain(m);
		}

		private static void EndLifeDrain(Mobile m)
		{
			Timer t = (Timer)m_Table[m];

			if (t != null)
				t.Stop();

			m_Table.Remove(m);
			m.SendLocalizedMessage(1070849); // The drain on your life force is gone.
		}

		private class ELifeDrainTimer : Timer
		{
			private Mobile m_From;
			private Mobile m_Mobile;
			private int m_Count;

			public ELifeDrainTimer(Mobile from, Mobile m)
				: base(TimeSpan.FromSeconds(1.0), TimeSpan.FromSeconds(1.0))
			{
				m_From = from;
				m_Mobile = m;
				Priority = TimerPriority.TwoFiftyMS;
			}

			protected override void OnTick()
			{
				DrainLife(m_Mobile, m_From);

				if (++m_Count == 5)
					EndLifeDrain(m_Mobile);
			}
		}
		#endregion
	}
}