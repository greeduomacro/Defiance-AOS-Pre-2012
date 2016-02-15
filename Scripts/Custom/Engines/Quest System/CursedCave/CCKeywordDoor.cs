using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Mobiles;
using Server.Network;

namespace Server.Items
{
	public class CCKeywordDoor : KeywordTeleporter
	{
		public override int LabelNumber { get { return 1020000 + (ItemID & 0x3FFF); } }

		private string m_sQuestion;
		[CommandProperty(AccessLevel.GameMaster)]
		public string Question
		{
			get { return m_sQuestion; }
			set { m_sQuestion = value; }
		}

		private Point3D m_PointDest1;
		[CommandProperty(AccessLevel.GameMaster)]
		public Point3D PointDest1
		{
			get { return m_PointDest1; }
			set { m_PointDest1 = value; }
		}

		private Point3D m_PointDest2;
		[CommandProperty(AccessLevel.GameMaster)]
		public Point3D PointDest2
		{
			get { return m_PointDest2; }
			set { m_PointDest2 = value; }
		}

		private Rectangle2D m_OneSideArea;
		[CommandProperty(AccessLevel.GameMaster)]
		public Rectangle2D OneSideArea
		{
			get { return m_OneSideArea; }
			set { m_OneSideArea = value; }
		}

		private TimeSpan m_UsageDelay;
		[CommandProperty(AccessLevel.GameMaster)]
		public TimeSpan UsageDelay
		{
			get { return m_UsageDelay; }
			set { m_UsageDelay = value; }
		}

		[Constructable]
		public CCKeywordDoor()
		{
			ItemID = 0x677;
			Visible = true;

			m_UsageDelay = TimeSpan.FromSeconds(10.0);
		}

		public CCKeywordDoor(Serial serial)
			: base(serial)
		{
		}

		public override void OnDoubleClick(Mobile from)
		{
			base.OnDoubleClick(from);

			if (from.InRange(this.Location, Range))
			{
				from.LocalOverheadMessage(MessageType.Regular, 0x5A, true, m_sQuestion);
			}
		}

		private List<Mobile> m_Users;
		private bool m_bHasPlayedAnimation;

		private void Delay_Callback(object state)
		{
			m_Users.Remove((Mobile)state);
		}

		private void Animation_Callback(object state)
		{
			m_bHasPlayedAnimation = false;
		}

		public override void StartTeleport(Mobile m)
		{
			if (m_Users == null)
				m_Users = new List<Mobile>();

			if (m_Users.Contains(m))
			{
				if (!m_bHasPlayedAnimation)
				{
					Effects.PlaySound(this.Location, this.Map, 0x348);
					Effects.SendLocationParticles(EffectItem.Create(new Point3D(Location.X, Location.Y + 1, Location.Z), this.Map, EffectItem.DefaultDuration), 0x398C, 8, 72, 5042);
					m_bHasPlayedAnimation = true;
					Timer.DelayCall(TimeSpan.FromSeconds(3.5), new TimerStateCallback(Animation_Callback), m);
				}
				return;
			}
			else
			{
				m_Users.Add(m);
				Timer.DelayCall(m_UsageDelay, new TimerStateCallback(Delay_Callback), m);
			}

			if (m_OneSideArea.Contains( m.Location ))
				PointDest = m_PointDest1;
			else
				PointDest = m_PointDest2;

			base.StartTeleport(m);
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write((int)1); // version

			// Version 1
			writer.Write(m_UsageDelay);

			// Version 0
			writer.Write((string)m_sQuestion);
			writer.Write(m_PointDest1);
			writer.Write(m_PointDest2);
			writer.Write(m_OneSideArea);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadInt();

			switch (version)
			{
				case 1:
					m_UsageDelay = reader.ReadTimeSpan();
					goto case 0;

				case 0:
					m_sQuestion = reader.ReadString();
					m_PointDest1 = reader.ReadPoint3D();
					m_PointDest2 = reader.ReadPoint3D();
					m_OneSideArea = reader.ReadRect2D();
					break;
			}
		}
	}
}