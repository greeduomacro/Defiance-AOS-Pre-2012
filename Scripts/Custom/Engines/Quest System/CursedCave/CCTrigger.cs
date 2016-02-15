using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Mobiles;

namespace Server.Items
{
	public class CCTrigger : Item
	{
		private static TimeSpan m_TimeDelay = TimeSpan.FromHours(3.0);
		private List<CCTriggerEntry> m_alEntryList;

		#region CommandProperties
		private CursedCaveUtility.CCTriggerCheck m_tCheck;
		[CommandProperty(AccessLevel.GameMaster)]
		public CursedCaveUtility.CCTriggerCheck TriggerCheck
		{
			get { return m_tCheck; }
			set { m_tCheck = value; InvalidateProperties(); }
		}

		private string m_sMessage;
		[CommandProperty(AccessLevel.GameMaster)]
		public string Message
		{
			get { return m_sMessage; }
			set { m_sMessage = value; }
		}

		private int m_iEventRange;
		[CommandProperty(AccessLevel.GameMaster)]
		public int Range
		{
			get { return m_iEventRange; }
			set { m_iEventRange = value; InvalidateProperties(); }
		}
		#endregion

		[Constructable]
		public CCTrigger()
			: base(0x1F1C)
		{
			Name = "CC Trigger";
			Visible = false;
			Movable = false;

			m_iEventRange = 4;
		}

		public override void GetProperties(ObjectPropertyList list)
		{
			base.GetProperties(list);

			list.Add(1060658, "Range\t{0}", m_iEventRange); // ~1_val~: ~2_val~
			list.Add(1060659, "Check Type\t{0}", m_tCheck.ToString()); // ~1_val~: ~2_val~
		}

		public override bool HandlesOnMovement { get { return true; } }

		public override void OnMovement(Mobile m, Point3D oldLocation)
		{
			if (m.Player)
			{
				bool inOldRange = Utility.InRange(oldLocation, Location, m_iEventRange);
				bool inNewRange = Utility.InRange(m.Location, Location, m_iEventRange);

				if (inNewRange && !inOldRange)
				{
					if (m_alEntryList == null)
						m_alEntryList = new List<CCTriggerEntry>();

					bool isInList = false;
					for (int i = m_alEntryList.Count - 1; i >= 0; --i)
					{
						CCTriggerEntry entry = (CCTriggerEntry)m_alEntryList[i];
						if (m == entry.Mobile)
						{
							if (DateTime.Now > entry.End)
								m_alEntryList.RemoveAt(i);
							else
								isInList = true;
						}
					}

					if (!isInList)
					{
						string sWeaponName;
						if (Message != null && m_tCheck == CursedCaveUtility.CCTriggerCheck.None)
						{
							m_alEntryList.Add(new CCTriggerEntry(m, DateTime.Now + m_TimeDelay));
							CursedCaveUtility.PrivateMessage(m, Message);
						}
						else if (Message != null && CursedCaveUtility.HasTalkingWeapon(m, out sWeaponName, m_tCheck))
						{
							m_alEntryList.Add(new CCTriggerEntry(m, DateTime.Now + m_TimeDelay));
							CursedCaveUtility.PrivateMessage(m, string.Format("{0}: {1}", sWeaponName, Message));
						}
					}
				}
			}
		}

		private class CCTriggerEntry
		{
			private Mobile m_mobile;
			public Mobile Mobile
			{
				get { return m_mobile; }
				set { m_mobile = value; }
			}

			private DateTime m_dtEnd;
			public DateTime End
			{
				get { return m_dtEnd; }
				set { m_dtEnd = value; }
			}

			public CCTriggerEntry(Mobile mobile, DateTime end)
			{
				m_mobile = mobile;
				m_dtEnd = end;
			}
		}

		public CCTrigger(Serial serial)
			: base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write((int)1);

			writer.Write((TimeSpan)m_TimeDelay);
			writer.Write((int)m_iEventRange);
			writer.Write((string)m_sMessage);
			writer.Write((int)m_tCheck);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadInt();

			m_TimeDelay = reader.ReadTimeSpan();
			m_iEventRange = reader.ReadInt();

			if (version < 1)
				reader.ReadDateTime();

			m_sMessage = reader.ReadString();
			m_tCheck = (CursedCaveUtility.CCTriggerCheck)reader.ReadInt();
		}
	}
}