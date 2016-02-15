using System;
using System.Collections.Generic;
using Server.Commands;
//
//create singlesavetime
namespace Server.Events
{
	public static class AutoStartEvent
	{
		private static List<AutoStartStruct> m_Enlisted = new List<AutoStartStruct>();
		public static List<AutoStartStruct> Enlisted { get { return m_Enlisted; } set { m_Enlisted = value; } }
		public static PREStruct PersonalEvent;
		public static DateTime Now = DateTime.Now;
		public static int NowDayOfWeek = (int)Now.DayOfWeek;

		public static void Configure()
		{
			new EventStartTimer().Start();
			CustomSaving.AddSaveModule(new SaveData(new DC.SaveMethod(Serialize), new DC.LoadMethod(Deserialize)), "AutoStartEvent");
			CommandSystem.Register("EventSystem", AccessLevel.GameMaster, new CommandEventHandler(AutoStartSystem_OnCommand));
		}

		public static void Initialize()
		{
			m_Enlisted.Sort();
		}

		public static void AutoStartSystem_OnCommand(CommandEventArgs args)
		{
			bool fullacces = args.Mobile.AccessLevel >= AccessLevel.Administrator;
			args.Mobile.SendGump(new IndexGump(0, fullacces));
		}

		public static void AddNewTimedEvent(AutoStartStruct ass)
		{
			m_Enlisted.Add(ass);
			m_Enlisted.Sort();
		}

		public static void OnTick()
		{
			Now = DateTime.Now;
			NowDayOfWeek = (int)Now.DayOfWeek;

			if (m_Enlisted.Count > 0)
			{
				AutoStartStruct ass = m_Enlisted[0];
				if (ass.RunNow())
				{
					if (!EventSystem.Running && ass.Event != null && !ass.Event.Deleted)
						((ITimableEvent)ass.Event).Running = true;
					ass.LastExecutedDay = Now.Day;
					ass.SetNextExecution();
					m_Enlisted.Sort();
				}
			}
		}

		private static void Serialize(GenericWriter writer)
		{
			writer.Write(0);//version

			writer.Write(m_Enlisted.Count);
			foreach (AutoStartStruct ass in m_Enlisted)
				ass.Serialize(writer);
		}

		private static void Deserialize(GenericReader reader)
		{
			int version = reader.ReadInt();

			int count = reader.ReadInt();
			for (int i = 0; i < count; i++)
			{
				AutoStartStruct ass = new AutoStartStruct(reader);
				if(ass.Event != null && !ass.Event.Deleted)
					m_Enlisted.Add(ass);
			}
		}
	}

	public class PREStruct
	{
		public DateTime EndTime;
		public Mobile Starter;
		public int Hours;
		public int Minutes;

		public PREStruct(Mobile m, int hours, int minutes)
		{
			Starter = m;
			Hours = hours;
			Minutes = minutes;
			EndTime = DateTime.Now + TimeSpan.FromMinutes(Minutes) + TimeSpan.FromHours(Hours);
		}
	}

	public class AutoStartStruct : IComparable
	{
		public int LastExecutedDay;
		public DateTime NextExecution;
		public string Name;
		public bool[] Days;
		public int Hour;
		public int Minute;
		public Item Event;

		public AutoStartStruct(string name, bool[] days, int hour, int minute, Item item)
		{
			LastExecutedDay = 0;
			Days = days;
			Hour = hour;
			Minute = minute;
			Event = item;
			Name = name;
		}

		public AutoStartStruct(GenericReader reader)
		{
			int version = reader.ReadInt();

			LastExecutedDay = reader.ReadInt();
			Days = CustomSaving.DeserializeBoolArray(reader);
			Hour = reader.ReadInt();
			Minute = reader.ReadInt();
			Event = reader.ReadItem();
			Name = reader.ReadString();

			SetNextExecution();
		}

		public bool RunNow()
		{
			return AutoStartEvent.Now.Day != LastExecutedDay && Days[AutoStartEvent.NowDayOfWeek] && (AutoStartEvent.Now.Hour > Hour || (AutoStartEvent.Now.Hour == Hour && AutoStartEvent.Now.Minute >= Minute));
		}

		public void SetNextExecution()
		{
			DateTime next = AutoStartEvent.Now;
			int count = next.Day == LastExecutedDay ? 1 : 0;
			bool found = false;

			for (int i = AutoStartEvent.NowDayOfWeek + count; i < 7; i++)
			{
				if (Days[i])
				{
					found = true;
					break;
				}

				count++;
			}

			if (!found)
				for (int i = 0; i < AutoStartEvent.NowDayOfWeek; i++)
				{
					if (Days[i])
						break;

					count++;
				}

			NextExecution = new DateTime(next.Year, next.Month, next.Day, Hour, Minute, 0);
			NextExecution.AddDays(count);
		}

		public int CompareTo(object o)
		{
			AutoStartStruct a = (AutoStartStruct)o;

			if (a.NextExecution < NextExecution)
				return 1;

			if (a.NextExecution > NextExecution)
				return -1;

			return 0;
		}

		public void Serialize(GenericWriter writer)
		{
			writer.Write(0);//version
			writer.Write(LastExecutedDay);
			CustomSaving.SerializeBoolArray(Days, writer);
			writer.Write(Hour);
			writer.Write(Minute);
			writer.Write(Event);
			writer.Write(Name);
		}
	}

	public class EventStartTimer : Timer
	{
		public EventStartTimer()
			: base(TimeSpan.FromSeconds(30.0), TimeSpan.FromSeconds(30.0))
		{
		}

		protected override void OnTick()
		{
			AutoStartEvent.OnTick();
		}
	}

	public interface ITimableEvent
	{
		bool Running { get; set; }
	}
}