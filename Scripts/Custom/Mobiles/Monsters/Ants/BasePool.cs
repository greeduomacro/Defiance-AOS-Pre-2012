using System;
using System.Collections;
using System.Collections.Generic;
using Server.Targeting;
using Server.Network;
using Server;
using Server.Mobiles;

namespace Server.Items
{
	public class BasePool : Item
	{
		private Timer m_PoolTimer;
		private int m_dmgMin, m_dmgMax, m_phys, m_fire, m_cold, m_pois, m_nrgy;
		private DateTime m_Created;

		public virtual TimeSpan Duration { get { return TimeSpan.FromSeconds(10.0); } }

		public BasePool(string name, int hue, int dmgMin, int dmgMax, int phys, int fire, int cold, int pois, int nrgy)
			: base(0x122A)
		{
			Name = name;
			Movable = false;
			Hue = hue;
			m_dmgMin = dmgMin;
			m_dmgMax = dmgMax;
			m_phys = phys;
			m_fire = fire;
			m_cold = cold;
			m_pois = pois;
			m_nrgy = nrgy;

			m_Created = DateTime.Now;
			m_PoolTimer = Timer.DelayCall(TimeSpan.Zero, TimeSpan.FromSeconds(1), new TimerCallback(OnTick));
		}

		public BasePool(Serial serial)
			: base(serial)
		{
		}

		public virtual void OnDoDamage(Mobile target)
		{
			AOS.Damage(target, Utility.RandomMinMax(m_dmgMin, m_dmgMax), m_phys, m_fire, m_cold, m_pois, m_nrgy);
			target.PlaySound(0x1dE);
		}

		public virtual bool IsValidPoolTarget(Mobile m)
		{
			if (m is PlayerMobile && m.Alive)
				return true;

			if (m is BaseCreature && m.Alive)
			{
				BaseCreature creature = (BaseCreature)m;
				if (!creature.IsDeadBondedPet && (creature.Controlled || creature.Summoned))
					return true;
			}
			return false;
		}

		public override void OnAfterDelete()
		{
			if (m_PoolTimer != null)
				m_PoolTimer.Stop();

			base.OnAfterDelete();
		}

		private void OnTick()
		{
			DateTime now = DateTime.Now;
			TimeSpan age = now - m_Created;

			if (age > Duration)
				Delete();
			else
			{
				List<Mobile> toDamage = new List<Mobile>();

				foreach (Mobile m in GetMobilesInRange(0))
				{
					if (IsValidPoolTarget(m))
					{
						toDamage.Add(m);
					}
				}

				for (int i = 0; i < toDamage.Count; i++)
					OnDoDamage(toDamage[i]);
			}
		}

		public override void Serialize(GenericWriter writer)
		{
		}

		public override void Deserialize(GenericReader reader)
		{
		}
	}
}