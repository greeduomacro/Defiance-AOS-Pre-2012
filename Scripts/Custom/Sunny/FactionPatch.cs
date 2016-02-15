using System;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Guilds;

namespace Server.Factions
{
	public static class FactionPatch
	{
		private static DateTime m_LastCheck;

		public static void Configure()
		{
			CustomSaving.AddSaveModule(new SaveData(new DC.SaveMethod(OnSave), new DC.LoadMethod(OnLoad)), "FactionPatch");
		}

		private static void OnSave(GenericWriter writer)
		{
			writer.Write(0);//version
			writer.Write(m_LastCheck);
		}

		private static void OnLoad(GenericReader reader)
		{
			int version = reader.ReadInt();
			m_LastCheck = reader.ReadDateTime();
			Timer.DelayCall(TimeSpan.FromMinutes(30.0), TimeSpan.FromMinutes(30.0), new TimerCallback(Check_OnCallback));
		}

		private static void Check_OnCallback()
		{
			DateTime now = DateTime.Now;
			if (now - m_LastCheck > TimeSpan.FromHours(47.45)) //every two days
			{
				List<PlayerState> toremove = new List<PlayerState>();

				foreach (Mobile m in World.Mobiles.Values)
				{
					if (m.Player)
					{
						PlayerMobile pm = (PlayerMobile)m;
						PlayerState ps = pm.FactionPlayerState;
						if (ps != null)
						{
							if ((now - pm.LastOnline) > TimeSpan.FromDays(14.0))
								toremove.Add(ps);

							else if (ps.KillPoints > 10)
								ps.KillPoints -= (int)(ps.KillPoints * 0.01) + 1;

							else if (ps.KillPoints < 0)
								ps.KillPoints++;
						}
					}
				}

				foreach (PlayerState ps in toremove)
				{
					Mobile m = ps.Mobile;
					ps.Faction.RemoveMember(m);
					if (m.Guild != null)
						((Guild)m.Guild).RemoveMember(m);
				}

				m_LastCheck = now;
			}
		}
	}
}