using System;
using System.Collections.Generic;
using Server.Gumps;
using Server.Network;
using Server.Targeting;

namespace Server.Events
{
	public class IndexGump : AdvGump
	{
		private int m_Start;
		private bool m_FullAcces;

		public IndexGump(int start, bool fullacces)
			: base(true)
		{
			m_Start = start;
			m_FullAcces = fullacces;
			int count = AutoStartEvent.Enlisted.Count;
			int max = Math.Min(count, start + 10);
			int totlength = 200 + max * 18;
			AddBackground(0, 0, 750, totlength, 9200);
			AddBackground(275, 15, 200, 50, 9200);
			AddHtml(0, 18, 750, 18, Center("Timed Events"), false, false);
			AddHtml(0, 38, 750, 18, Center("Index"), false, false);

			int[] collumns = new int[] { 150, 120, 40, 40, 12, 12, 12, 12, 12, 12, 12,150, 50 };
			string[] firstline = new string[] { "Name", "Event", "Hour", "Minute", "S","M", "T", "W", "T", "F", "S","NextRun", "LastDay" };
			List<string> data = new List<string>(firstline);

			int yinc = 0;
			if (AutoStartEvent.PersonalEvent != null)
			{
				yinc = 18;
				AddButton(25, 108, 4005, 4006, 10000, GumpButtonType.Reply, 0);
				PREStruct pres = AutoStartEvent.PersonalEvent;
				data.Add("Personal Ran Event");
				data.Add(pres.Starter == null ? "N/A" : pres.Starter.Name);

				TimeSpan timeleft = pres.EndTime - DateTime.Now;
				data.Add(timeleft.Hours.ToString());
				data.Add(timeleft.Minutes.ToString());
				for (int i = 0; i < 7; i++ )
					data.Add("/");
				data.Add("Now");
				data.Add("Now");
			}

			for (int i = start; i < max; i++)
			{
				AutoStartStruct ass = AutoStartEvent.Enlisted[i];
				if(fullacces)
					AddButton(25, 108 + yinc + i*18, 4005, 4006, 100 + i + start, GumpButtonType.Reply, 0);
				data.Add(ass.Name);
				data.Add((ass.Event == null || ass.Event.Deleted) ? "null" : ass.Event.GetType().Name);
				data.Add(ass.Hour.ToString());
				data.Add(ass.Minute.ToString());
				foreach (bool b in ass.Days)
					data.Add(b ? "Y":"N");
				data.Add(ass.NextExecution.ToString());
				data.Add(ass.LastExecutedDay.ToString());
			}

			AddTable(55, 90, collumns,data,null, 11, 1);

			if (max < count)
			{
				AddButton(473, totlength - 70, 4005, 4006, 5, GumpButtonType.Reply, 0);
				AddLabel(430, totlength - 70, 0, "Next");
			}

			if (start > 0)
			{
				AddButton(25, totlength - 70, 4014, 4015, 4, GumpButtonType.Reply, 0);
				AddLabel(72, totlength - 70, 0, "Previous");
			}

			if (m_FullAcces)
			{
				AddButton(10, totlength - 40, 4005, 4006, 1, GumpButtonType.Reply, 0);
				AddLabel(45, totlength - 40, 0, "Add new timed event");
			}
			AddButton(190, totlength - 40, 4005, 4006, 2, GumpButtonType.Reply, 0);
			AddLabel(225, totlength - 40, 0, "Run/Stop personal ran events");
			AddButton(680, totlength - 40, 247, 248, 0, GumpButtonType.Reply, 0);
		}

		public override void OnResponse(NetState sender, RelayInfo info)
		{
			Mobile m = sender.Mobile;

			if (info.ButtonID == 10000)
				m.SendGump(new PREEntryGump());

			if (m_FullAcces && info.ButtonID >= 100 && info.ButtonID - 100 < AutoStartEvent.Enlisted.Count)
			{
				AutoStartStruct ass = AutoStartEvent.Enlisted[info.ButtonID - 100];
				AutoStartEvent.Enlisted.Remove(ass);
				m.SendGump(new EntryGump(ass));
				return;
			}

			switch (info.ButtonID)
			{
				case 1:
					if (m_FullAcces)
					{
						m.Target = new InternalTarget();
						m.SendMessage("Please target the event.");
					}
					break;

				case 2:
					m.SendGump(new PREEntryGump());
					break;

				case 4:
					m.SendGump(new IndexGump(Math.Max(m_Start - 10, 0), m_FullAcces));
					break;

				case 5:
					m.SendGump(new IndexGump(Math.Min(m_Start + 10, Math.Max(0, AutoStartEvent.Enlisted.Count - 11)), m_FullAcces));
					break;
			}
		}

		private class InternalTarget : Target
		{
			public InternalTarget()
				: base(20, false, TargetFlags.None)
			{
			}

			protected override void OnTarget(Mobile m, object targeted)
			{
				if (targeted is ITimableEvent && targeted is Item)
				{
					Item item = (Item)targeted;
					m.SendGump(new EntryGump(new AutoStartStruct("not assigned",new bool[7],0,0,item)));
				}

				else
				{
					m.SendMessage("That is not a timable event.");
					m.SendGump(new IndexGump(0, m.AccessLevel >= AccessLevel.Administrator));
				}
			}
		}
	}


	public class EntryGump : AdvGump
	{
		private AutoStartStruct m_Struct;

		public EntryGump(AutoStartStruct ass)
			: base(true)
		{
			m_Struct = ass;

			Closable = false;
			Disposable = false;

			AddBackground(0, 0, 500, 443, 9200);
			AddBackground(150, 15, 200, 50, 9200);
			AddHtml(0, 18, 500, 18, Center("Timed Events"), false, false);
			AddHtml(0, 38, 500, 18, Center("Single Entry"), false, false);

			AddLabel(65, 90, 0, "Event:");
			AddLabel(125, 90, 0, (ass.Event == null || ass.Event.Deleted) ? "null" : ass.Event.GetType().Name);
			AddLabel(65, 115, 0, "Name:");
			AddTextEntry(125, 115, 150, 20, 0, 0, ass.Name);
			AddLabel(65, 140, 0, "Hour:");
			AddTextEntry(125, 140, 79, 20, 0, 1, ass.Hour.ToString());
			AddLabel(65, 165, 0, "Minute:");
			AddTextEntry(125, 165, 79, 20, 0, 2, ass.Minute.ToString());

			int count = 0;
			foreach (bool b in ass.Days)
			{
				AddCheck(65, 190 + count * 25, 210, 211, b, count);
				AddLabel(100, 190 + count * 25, 0, ((DayOfWeek)count).ToString());
				count++;
			}

			AddButton(16, 406, 4020, 4006, 0, GumpButtonType.Reply, 0);
			AddLabel(56, 407, 0, "Remove");
			AddButton(423, 406, 247, 248, 1, GumpButtonType.Reply, 0);
		}

		public override void OnResponse(NetState sender, RelayInfo info)
		{
			Mobile m = sender.Mobile;

			if (info.ButtonID == 1)
			{
				if (m_Struct.Event == null || m_Struct.Event.Deleted)
				{
					m.SendGump(new IndexGump(0, m.AccessLevel >= AccessLevel.Administrator));
					m.SendMessage("The event has been deleted, thus the timer could not be added.");
					return;
				}

				bool add = true;

				string name = info.GetTextEntry(0).Text;
				int hour;
				int minute;

				if(!Int32.TryParse(info.GetTextEntry(1).Text, out hour))
					add = false;

				if (!Int32.TryParse(info.GetTextEntry(2).Text, out minute))
					add = false;

				bool[] barray = new bool[7];
				for (int i = 0; i < 7; i++)
					barray[i] = info.IsSwitched(i);

				if (!add)
				{
					m.SendGump(this);
					m.SendMessage("The hour or minute are not valid Int32 values.");
				}

				else if (!AutoStartEvent.Enlisted.Contains(m_Struct))
				{
					m_Struct.Name = name;
					m_Struct.Hour = hour;
					m_Struct.Minute = minute;
					m_Struct.Days = barray;

					if (m_Struct.Days[AutoStartEvent.NowDayOfWeek] && (AutoStartEvent.Now.Hour > m_Struct.Hour || (AutoStartEvent.Now.Hour == m_Struct.Hour && AutoStartEvent.Now.Minute > m_Struct.Minute)))
						m_Struct.LastExecutedDay = AutoStartEvent.Now.Day;

					m_Struct.SetNextExecution();
					AutoStartEvent.AddNewTimedEvent(m_Struct);
					m.SendGump(new IndexGump(0, m.AccessLevel >= AccessLevel.Administrator));
					m.SendMessage("The timer has been succesfully added.");
				}

				else
					m.SendMessage("Someone else has edited this entry in the meantime.");
			}
		}
	}
		public class PREEntryGump : AdvGump
		{
			public PREEntryGump()
				: base(true)
			{
				AddBackground(0, 0, 500, 420, 9200);
				AddBackground(150, 13, 200, 50, 9200);

				AddHtml(0, 18, 500, 18, Center("Timed Events"), false, false);
				AddHtml(0, 38, 500, 18, Center("PRE Entry"), false, false);
				int mins = 0;
				int hours = 0;

				if (AutoStartEvent.PersonalEvent != null)
				{
					PREStruct pres = AutoStartEvent.PersonalEvent;
					TimeSpan timeleft = pres.EndTime - DateTime.Now;
					mins = timeleft.Minutes;
					hours = timeleft.Hours;
				}
				AddLabel(15, 85, 0, "Hours:");
				AddTextEntry(80, 85, 80, 20, 0, 0, hours.ToString());
				AddLabel(15, 110, 0, "Minutes:");
				AddTextEntry(80, 110, 77, 20, 0, 1, mins.ToString());
				AddHtml(15, 145, 410, 220, "You are required to enter the time it takes to hold your event here, if you do not, the automated event system will think there are no events running and might start one on it's own while you run your event.<br>You can fill out one or both fields and press okay. You are also able to enter a new time if you havn't finished when the timer stopped or remove the entry when you finished earlier. Adding this does not start any events, but it prevents the automated system from running events while you hold yours.<br><br>Good luck on your event!", true, false);

				AddButton(423, 385, 247, 248, 1, GumpButtonType.Reply, 0);
				AddButton(16, 385, 4020, 4006, 0, GumpButtonType.Reply, 0);
				AddLabel(56, 385, 0, "Cancel/Remove");
			}

			public override void OnResponse(NetState sender, RelayInfo info)
		   {
				Mobile m = sender.Mobile;

				switch (info.ButtonID)
				{
					case 0:
						if (AutoStartEvent.PersonalEvent != null)
						{
							m.SendMessage("Warning, you have removed the current personal ran event timer, please make sure this was your intention.");
							AutoStartEvent.PersonalEvent = null;
							m.SendGump(new IndexGump(0, m.AccessLevel >= AccessLevel.Administrator));
						}
						break;

					case 1:
						bool add = true;

						int hours;
						int minutes;

						if (!Int32.TryParse(info.GetTextEntry(0).Text, out hours))
							add = false;

						if (!Int32.TryParse(info.GetTextEntry(1).Text, out minutes))
							add = false;

						if (!add)
						{
							m.SendGump(this);
							m.SendMessage("The hour or minute are not valid Int32 values.");
						}

						else
						{
							PREStruct pres= new PREStruct(m, hours, minutes);

							EventSystem.StartFakeEvent(pres);
							AutoStartEvent.PersonalEvent = pres;

							m.SendGump(new IndexGump(0, m.AccessLevel >= AccessLevel.Administrator));
							m.SendMessage("The timer has been succesfully added.");
						}
						break;
				}
			}
		}

}