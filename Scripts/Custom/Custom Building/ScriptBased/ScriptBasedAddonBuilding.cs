//////////////////////////////\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
//			   This is a Sunny Production ©2006					\\
//					 Based on RunUO©							\\
//					Version: Beta 1.0							\\
//		Many thanks to my Csharp tutors Will and Eclipse		\\
//////////////////////////////\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
//
using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Regions;

namespace Server.Multis.CustomBuilding
{
	public abstract class ScriptBasedAddonBuilding : ScriptBasedBuilding
	{
		private List<ScriptBasedBuildingAddon> m_AddonComponents;

		public void AddComponent(ScriptBasedBuildingAddon c, int x, int y)
		{
			if (Deleted)
				return;

			m_AddonComponents.Add(c);

			c.Addon = this;
			c.Offset = new Point2D(x, y);
			c.MoveToWorld(new Point3D(X + x, Y + y, Z), Map);
		}

		public ScriptBasedAddonBuilding() : base()
		{
			m_AddonComponents = new List<ScriptBasedBuildingAddon>();
		}

		public List<ScriptBasedBuildingAddon> AddonComponents
		{
			get
			{
				return m_AddonComponents;
			}
		}

		public ScriptBasedAddonBuilding(Serial serial) : base(serial)
		{
		}

		public override void OnLocationChange(Point3D oldLoc)
		{
			if (Deleted)
				return;

			base.OnLocationChange(oldLoc);

			foreach (ScriptBasedBuildingAddon c in m_AddonComponents)
				c.Location = new Point3D(X + c.Offset.X, Y + c.Offset.Y, Z);
		}

		public override void OnMapChange()
		{
			if (Deleted)
				return;

			foreach (ScriptBasedBuildingAddon c in m_AddonComponents)
				c.Map = Map;
		}

		public override void OnAfterDelete()
		{
			base.OnAfterDelete();

			foreach (ScriptBasedBuildingAddon c in m_AddonComponents)
				c.Delete();
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(0); // version


			writer.WriteItemList<ScriptBasedBuildingAddon>(m_AddonComponents);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();

			switch (version)
			{
				case 0:
					{
						m_AddonComponents = reader.ReadStrongItemList<ScriptBasedBuildingAddon>();
						break;
					}
			}
		}
	}
}