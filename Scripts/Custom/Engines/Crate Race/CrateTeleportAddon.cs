using System;
using Server;
using Server.Items;

namespace Server.Events.CrateRace
{
	public class CrateTeleport : BaseAddon
	{
		private Item m_Tele1, m_Tele2;
		private int m_Direction;
		private Item m_Compo1, m_Compo2;

		[Constructable]
		public CrateTeleport(int direction)
		{
			m_Direction = direction;
			CrateTeleporter tele1 = new CrateTeleporter();
			CrateTeleporter tele2 = new CrateTeleporter();
			m_Tele1 = (Item)tele1;
			m_Tele2 = (Item)tele2;

			switch (m_Direction)
			{
				case 1:
					AddComponent(1170, 0, 0, 0, 0);
					AddComponent(1169, 0, 1, 0, 0);
					AddComponent(1263, 1, 0, 0, 1);
					AddComponent(1264, 1, 1, 0, 2);
					AddComponent(1171, 2, -1, 0, 0);
					AddComponent(1170, 2, 0, 0, 0);
					AddComponent(1169, 2, 1, 0, 0);
					AddComponent(1172, 2, 2, 0, 0);
					AddComponent(1171, 3, 0, 0, 0);
					AddComponent(1172, 3, 1, 0, 0);
					break;
				case 2:
					AddComponent(1171, 0, 0, 0, 0);
					AddComponent(1169, 1, 0, 0, 0);
					AddComponent(1265, 0, 1, 0, 1);
					AddComponent(1264, 1, 1, 0, 2);
					AddComponent(1170, -1, 2, 0, 0);
					AddComponent(1171, 0, 2, 0, 0);
					AddComponent(1169, 1, 2, 0, 0);
					AddComponent(1172, 2, 2, 0, 0);
					AddComponent(1170, 0, 3, 0, 0);
					AddComponent(1172, 1, 3, 0, 0);
					break;

				case 3:
					AddComponent(1172, 0, 0, 0, 0);
					AddComponent(1171, 0, 1, 0, 0);
					AddComponent(1262, -1, 0, 0, 1);
					AddComponent(1265, -1, 1, 0, 2);
					AddComponent(1169, -2, -1, 0, 0);
					AddComponent(1172, -2, 0, 0, 0);
					AddComponent(1171, -2, 1, 0, 0);
					AddComponent(1170, -2, 2, 0, 0);
					AddComponent(1169, -3, 0, 0, 0);
					AddComponent(1170, -3, 1, 0, 0);
					break;

				case 4:
					AddComponent(1172, 0, 0, 0, 0);
					AddComponent(1170, 1, 0, 0, 0);
					AddComponent(1262, 0, -1, 0, 1);
					AddComponent(1263, 1, -1, 0, 2);
					AddComponent(1169, -1, -2, 0, 0);
					AddComponent(1172, 0, -2, 0, 0);
					AddComponent(1170, 1, -2, 0, 0);
					AddComponent(1171, 2, -2, 0, 0);
					AddComponent(1169, 0, -3, 0, 0);
					AddComponent(1171, 1, -3, 0, 0);
					break;
				default: Delete(); break;
			}
		}

		public void AddComponent(int id, int x, int y, int z, int teleporter)
		{
			AddonComponent ac = new AddonComponent(id);

			ac.Hue = 1372;

			AddComponent(ac, x, y, z);

			if (teleporter == 1)
				m_Compo1 = (Item)ac;

			if (teleporter == 2)
				m_Compo2 = (Item)ac;
		}

		public override void OnLocationChange(Point3D oldLocation)
		{
			base.OnLocationChange(oldLocation);
			if (m_Tele1 != null && m_Compo1 != null)
				m_Tele1.Location = m_Compo1.Location;
			if (m_Tele2 != null && m_Compo2 != null)
				m_Tele2.Location = m_Compo2.Location;
		}

		public override void OnMapChange()
		{
			if (m_Tele1 != null)
				m_Tele1.Map = Map;
			if (m_Tele2 != null)
				m_Tele2.Map = Map;
			base.OnMapChange();
		}

		public override void OnDelete()
		{
			if (m_Tele1 != null)
			{
				//((CrateTeleporter)m_Tele1).m_CanDelete = true; // Edit by Silver
				m_Tele1.Delete();
			}

			if (m_Tele2 != null)
			{
				//((CrateTeleporter)m_Tele2).m_CanDelete = true; // Edit by Silver
				m_Tele2.Delete();
			}

			base.OnDelete();
		}

		public CrateTeleport(Serial serial) : base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write(m_Compo1);
			writer.Write(m_Compo2);
			writer.Write(m_Tele1);
			writer.Write(m_Tele2);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			m_Compo1 = reader.ReadItem();
			m_Compo2 = reader.ReadItem();
			m_Tele1 = reader.ReadItem();
			m_Tele2 = reader.ReadItem();
		}
	}

	public class CrateTeleporter : Item
	{
		private int m_XInc, m_YInc;
		// public bool m_CanDelete; // Edit by Silver

		[CommandProperty(AccessLevel.GameMaster)]
		public int XIncrease
		{
			get { return m_XInc; }
			set { m_XInc = value; m_YInc = 0; }
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public int YIncrease
		{
			get { return m_YInc; }
			set { m_YInc = value; m_XInc = 0; }
		}

		[Constructable]
		public CrateTeleporter(): base( 0x1BC3 )
		{
			Movable = false;
			Visible = false;
		}

		public override bool OnMoveOver(Mobile m)
		{
			Point3D NewLoc = new Point3D(m.X + m_XInc, m.Y + m_YInc, m.Z);

			if (m_XInc != 0 || m_YInc != 0)
			{
				int inc = 0;
				int dir = 0;

				if (m_XInc > 0)
				{
					inc = m_XInc;
					dir = 1;
				}

				else if (m_XInc < 0)
				{
					inc = -m_XInc;
					dir = 2;
				}

				if (m_YInc > 0)
				{
					inc = m_YInc;
					dir = 3;
				}

				else if (m_YInc < 0)
				{
					inc = -m_YInc;
					dir = 4;
				}

				m.Location = NewLoc;
				m.ProcessDelta();
				new EffectTimer(dir, inc, Location, m).Start();
			}

			return false;
		}

		// Edit by Silver
		//public override void Delete()
		//{
		//	if (m_CanDelete)
		//		base.Delete();
		//}

		public CrateTeleporter(Serial serial) : base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write(m_XInc);
			writer.Write(m_YInc);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			m_XInc = reader.ReadInt();
			m_YInc = reader.ReadInt();
		}
	}

	public class EffectTimer : Timer
	{
		private Mobile m_Mob;
		private int m_Count, m_MaxCount, m_Direction;
		private Point3D m_Loc;


		public EffectTimer(int direction, int maxcount, Point3D loc, Mobile mob) : base(TimeSpan.FromSeconds(0.0), TimeSpan.FromSeconds(0.05))
		{
			Priority = 0;
			m_Mob = mob;
			m_MaxCount = maxcount;
			m_Direction = direction;
			m_Loc = loc;
		}

		protected override void OnTick()
		{
			m_Count++;
			if (m_Count >= m_MaxCount)
				Stop();

			int divider = m_MaxCount - m_Count;
			if (divider < 1)
				divider = 1;

			int z = (int)(20 / divider);

			switch (m_Direction)
			{
				case 1: Effects.SendLocationEffect(new Point3D(m_Loc.X + m_Count, m_Loc.Y, m_Loc.Z - z), m_Mob.Map, 0x3709, 17); break;
				case 2: Effects.SendLocationEffect(new Point3D(m_Loc.X - m_Count, m_Loc.Y, m_Loc.Z - z), m_Mob.Map, 0x3709, 17); break;
				case 3: Effects.SendLocationEffect(new Point3D(m_Loc.X, m_Loc.Y + m_Count, m_Loc.Z - z), m_Mob.Map, 0x3709, 17); break;
				case 4: Effects.SendLocationEffect(new Point3D(m_Loc.X, m_Loc.Y - m_Count, m_Loc.Z - z), m_Mob.Map, 0x3709, 17); break;
			}
		}
	}
}