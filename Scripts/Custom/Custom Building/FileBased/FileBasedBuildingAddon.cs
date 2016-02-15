//////////////////////////////\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
//			   This is a Sunny Production ©2006					\\
//					 Based on RunUO©							\\
//					Version: Beta 1.0							\\
//		Many thanks to my Csharp tutors Will and Eclipse		\\
//////////////////////////////\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
//
using System;
using Server;
using Server.Multis;

namespace Server.Multis.CustomBuilding
{
	public class FileBasedBuildingAddon : FileBasedBuilding
	{
		private Point2D m_Offset;
		private BaseBuilding m_Addon;

		[CommandProperty(AccessLevel.GameMaster)]
		public BaseBuilding Addon
		{
			get
			{
				return m_Addon;
			}
			set
			{
				m_Addon = value;
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public Point2D Offset
		{
			get
			{
				return m_Offset;
			}
			set
			{
				m_Offset = value;
			}
		}

		[Constructable]
		public FileBasedBuildingAddon(string name) : base(name)
		{
		}

		public FileBasedBuildingAddon(Serial serial) : base(serial)
		{
		}

		public override void OnLocationChange(Point3D old)
		{
			if (m_Addon != null)
				m_Addon.Location = new Point3D(X - m_Offset.X, Y - m_Offset.Y, Z);
		}

		public override void OnMapChange()
		{
			if (m_Addon != null)
				m_Addon.Map = Map;
		}

		public override void OnAfterDelete()
		{
			base.OnAfterDelete();

			if (m_Addon != null)
				m_Addon.Delete();
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write((int)0); // version

			writer.Write(m_Addon);
			writer.Write(m_Offset);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();

			switch (version)
			{
				case 0:
					{
						m_Addon = (BaseBuilding)reader.ReadItem();
						m_Offset = reader.ReadPoint2D();
						break;
					}
			}
		}
	}
}