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
	public class FileBasedAddonBuilding : FileBasedBuilding
	{
		private List<FileBasedBuildingAddon> m_AddonComponents;

		public void AddComponent(string name)
		{
			if (Deleted)
				return;

			BuildingEntry entry;
			if (FileBasedBuilding.BuildingTable.TryGetValue(name, out entry))
			{
				if (entry is ComponentEntry)
				{
					ComponentEntry ent = (ComponentEntry)entry;
					FileBasedBuildingAddon component = new FileBasedBuildingAddon(name);

					m_AddonComponents.Add(component);

					component.Addon = this;
					component.Offset = new Point2D(ent.X_Offset, ent.Y_Offset);
					component.MoveToWorld(new Point3D(X + ent.X_Offset, Y + ent.Y_Offset, Z), Map);
				}
			}
		}

		public FileBasedAddonBuilding(string name) : base(name)
		{
			m_AddonComponents = new List<FileBasedBuildingAddon>();
			List<string> list = ((AddonEntry)FileBasedBuilding.BuildingTable[name]).Components;
			for (int i = 0; i < list.Count; i++)
				AddComponent(list[i]);
		}

		public List<FileBasedBuildingAddon> AddonComponents
		{
			get
			{
				return m_AddonComponents;
			}
		}

		public FileBasedAddonBuilding(Serial serial) : base(serial)
		{
		}

		public override void OnLocationChange(Point3D oldLoc)
		{
			if (Deleted)
				return;

			base.OnLocationChange(oldLoc);

			foreach (FileBasedBuildingAddon c in m_AddonComponents)
				c.Location = new Point3D(X + c.Offset.X, Y + c.Offset.Y, Z);
		}

		public override void OnMapChange()
		{
			if (Deleted)
				return;

			foreach (FileBasedBuildingAddon c in m_AddonComponents)
				c.Map = Map;
		}

		public override void OnAfterDelete()
		{
			base.OnAfterDelete();

			foreach (FileBasedBuildingAddon c in m_AddonComponents)
				c.Delete();
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(0); // version

			writer.WriteItemList<FileBasedBuildingAddon>(m_AddonComponents);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();

			switch (version)
			{
				case 0:
					{
						m_AddonComponents = reader.ReadStrongItemList<FileBasedBuildingAddon>();
						break;
					}
			}
		}
	}
}