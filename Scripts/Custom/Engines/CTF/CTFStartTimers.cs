using System;
using Server.Commands;
using Server.Mobiles;

namespace Server.Events.CTF
{
	public class CTFAutoStart : Timer
	{
		public const int SystemHue = 0x38A;
		public static TimeSpan CTFStartTime = TimeSpan.FromHours(18.0);
		private static TimeSpan m_DayInterval = TimeSpan.FromDays(1.0);
		public static bool Enabled = false;

		private DateTime m_StartTime;

		public static void Initialize()
		{
			if (Enabled)
				new CTFAutoStart().Start();
		}

		public CTFAutoStart() : base(TimeSpan.Zero, TimeSpan.FromMinutes(1.0))
		{
			Priority = TimerPriority.OneMinute;

			m_StartTime = DateTime.Now.Date + CTFStartTime;

			if (m_StartTime < DateTime.Now)
				m_StartTime += m_DayInterval;
		}

		protected override void OnTick()
		{
			if (!Enabled)
				Stop();

			else if( DateTime.Now < m_StartTime)
				return;

			//fetchctf
			foreach (Item item in World.Items.Values)
			{
				if (item is CTFGameStone)
				{
					CTFGameStone stone = (CTFGameStone)item;
					if (stone.CanRun())
						CTFGame.StartGame(stone);
					break;
				}
			}

			m_StartTime += m_DayInterval;
		}
	}
}