using System;
using System.Collections.Generic;
using Server.Commands;

namespace Server.Events
{
	public static class EventSystem
	{
		public static bool Open;
		public static bool Running;
		public static int JoinMinutes;
		public static string EventName;
		public static EDC.StartMethod StartMethod;
		public static EDC.StopMethod StopMethod;
		public static EDC.JoinMethod JoinMethod;
		public static List<EventNpc> NpcList = new List<EventNpc>();
		private static JoinTimer m_Timer;
		private static FakeEventTimer m_PersonalEventTimer;

		public static void Configure()
		{
			CustomSaving.AddSaveModule(new SaveData(new DC.SaveMethod(Serialize), new DC.LoadMethod(Deserialize)), "InvulTimer");
		}

		public static void Start(int joinmins, string name, EDC.StartMethod start, EDC.StopMethod stop, EDC.JoinMethod join)
		{
			Open = true;
			Running = true;
			StartMethod = start;
			StopMethod = stop;
			JoinMethod = join;
			EventName = name;
			JoinMinutes = joinmins;
			m_Timer = new JoinTimer(joinmins, name);
			m_Timer.Start();
		}

		public static void StartFakeEvent(PREStruct pres)
		{
			if (pres != null)
			{
				StopFakeEvent(false);

				if (Open == true || Running == true)
					Stop();

				Running = true;
				m_PersonalEventTimer = new FakeEventTimer(TimeSpan.FromHours(pres.Hours) + TimeSpan.FromMinutes(pres.Minutes));
				m_PersonalEventTimer.Start();
			}
		}

		public static void StopFakeEvent(bool message)
		{
			if (m_PersonalEventTimer != null)
				m_PersonalEventTimer.Stop();

			AutoStartEvent.PersonalEvent = null;

			Running = false;

			if(message)
				CommandHandlers.BroadcastMessage(AccessLevel.Counselor, 0x482, "The custom started event has finished, if you are not done running your event please start another custom event.");
		}

		public static void StartGame()
		{
			Open = false;
			StartMethod();
			if (m_Timer != null)
			{
				m_Timer.Stop();
				m_Timer = null;
			}
		}

		public static void Stop()
		{
			if (Open || Running)
			{
				Open = false;
				Running = false;
				if (m_Timer != null)
				{
					m_Timer.Stop();
					m_Timer = null;
				}

				if (AutoStartEvent.PersonalEvent != null)
					StopFakeEvent(true);

				StopMethod();
			}
		}

		public static void AddNpc(EventNpc npc)
		{
			if (!NpcList.Contains(npc))
				NpcList.Add(npc);
		}

		private static Point3D[] m_ExitLocations = {new Point3D( 1522, 1757, 28 ),
											new Point3D( 1519, 1619, 10 ),
											new Point3D( 1457, 1538, 30 ),
											new Point3D( 1607, 1568, 20 ),
											new Point3D( 1643, 1680, 18 )};

		private static List<Mobile> m_InvulList = new List<Mobile>();
		public static void RemoveToRandomLoc(Mobile m)
		{
			m.Blessed = true;
			m_InvulList.Add(m);
			Timer.DelayCall(TimeSpan.FromSeconds(20.0), new TimerStateCallback(DelayCall_UnBless), m);
			m.MoveToWorld(m_ExitLocations[Utility.Random(m_ExitLocations.Length)], Map.Felucca);
		}

		private static void DelayCall_UnBless(object obj)
		{
			Mobile m = (Mobile)obj;
			m.Blessed = false;
			m_InvulList.Remove(m);
		}

		#region Ser / Deser
		public static void Serialize(GenericWriter writer)
		{
			writer.WriteMobileList(m_InvulList);
		}

		public static void Deserialize(GenericReader reader)
		{
			List<Mobile> invullist = reader.ReadStrongMobileList();

			foreach (Mobile m in invullist)
				m.Blessed = false;
		}
		#endregion
	}

	public class FakeEventTimer : Timer
	{
		public FakeEventTimer(TimeSpan ts)
			: base(ts)
		{
		}

		protected override void OnTick()
		{
			EventSystem.StopFakeEvent(true);
		}
	}

	public class JoinTimer : Timer
	{
		public const int SystemHue = 0x38A;
		public int BroadCasts;
		private string m_EventName;

		public JoinTimer(int broadmin, string name) : base(TimeSpan.Zero, TimeSpan.FromMinutes(1.0))
		{
			BroadCasts = Math.Max(2, broadmin);
			m_EventName = name;
		}

		protected override void OnTick()
		{
			// Start
			if (BroadCasts == 0)
			{
				CommandHandlers.BroadcastMessage(AccessLevel.Player, SystemHue, String.Format("---- {0} ----", m_EventName));
				CommandHandlers.BroadcastMessage(AccessLevel.Player, SystemHue, String.Format("The {0} is starting... Event Masters do not accept new players.", m_EventName));

				EventSystem.StartGame();
				Stop();
			}
			else // Broadcast time
			{
					foreach (EventNpc npc in EventSystem.NpcList)
					{
						npc.Say("Hear Ye! Hear Ye!");
						string npcText = "";
						switch (BroadCasts)
						{
							default: npcText = String.Format("Hurry up and join the {0}!", m_EventName); break;
							case 1: npcText = String.Format("Last call for the {0}!", m_EventName); break;
						}
						npc.Say(npcText);
					}


				string text = "";
				switch (BroadCasts)
				{
					default: text = "Event Masters at Britain Bank and at Buccaneers Den will now accept new players."; break;
					case 1: text = "Event Masters will not accept new players soon..."; break;
				}

				CommandHandlers.BroadcastMessage(AccessLevel.Player, SystemHue, String.Format("---- {0} ----", m_EventName));
				CommandHandlers.BroadcastMessage(AccessLevel.Player, SystemHue, String.Format("A {0} will start in {1} minutes!", m_EventName,BroadCasts));
				CommandHandlers.BroadcastMessage(AccessLevel.Player, SystemHue, text);
				BroadCasts--;
			}
		}
	}

	/// <summary>
	/// EventDelegateClass(EDC), delegates are created in this class.
	/// </summary>
	public static class EDC
	{
		public delegate void StartMethod();
		public delegate void StopMethod();
		public delegate void JoinMethod(Mobile m);
	}
}