using Server;

namespace Server.Regions
{
	public class NoHousingZoneBlock : Item
	{
		private Point3D m_AreaStart, m_AreaEnd;
		private NonHousingRegion m_Region;

		[CommandProperty(AccessLevel.GameMaster)]
		public Point3D AreaEnd
		{
			get { return m_AreaEnd; }
			set { m_AreaEnd = value; SetRegion(); }
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public Point3D AreaStart
		{
			get { return m_AreaStart; }
			set { m_AreaStart = value; SetRegion(); }
		}

		[Constructable]
		public NoHousingZoneBlock()
			: base(7885)
		{
			Name = "No Housing Zone Block";
			Movable = false;
			Visible = false;
		}

		public override void OnDelete()
		{
			if (m_Region != null)
				m_Region.Unregister();

			base.OnDelete();
		}

		private void SetRegion()
		{
			if (m_AreaEnd != Point3D.Zero && m_AreaStart != Point3D.Zero)
			{
				if(m_Region != null)
					m_Region.Unregister();

				Utility.FixPoints(ref m_AreaStart, ref m_AreaEnd);
				Rectangle2D[] array = new Rectangle2D[] {new Rectangle2D(new Point2D(m_AreaStart.X, m_AreaStart.Y), new Point2D(m_AreaEnd.X, m_AreaEnd.Y)) };
				m_Region = new NonHousingRegion(array, Serial.ToString(), Map);
				m_Region.Register();
			}
		}

		public NoHousingZoneBlock(Serial serial)
			: base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write((int)0); // version
			writer.Write(m_AreaStart);
			writer.Write(m_AreaEnd);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();
			m_AreaStart = reader.ReadPoint3D();
			m_AreaEnd = reader.ReadPoint3D();

			SetRegion();
		}
	}

	public class NonHousingRegion : Region
	{
		public NonHousingRegion(Rectangle2D[] array, string name, Map map)
			: base("NonHousingRegion-" + name, map, 1, array)
		{
		}

		public override bool AllowHousing(Mobile from, Point3D p)
		{
			return false;
		}
	}
}