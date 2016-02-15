//////////////////////////////\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
//			   This is a Sunny Production ©2006					\\
//					 Based on RunUO©							\\
//					Version: Beta 1.0							\\
//		Many thanks to my Csharp tutors Will and Eclipse		\\
//////////////////////////////\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
//
using System;
using Server.Gumps;
using Server.Targeting;
using Server.Network;
using System.Collections;
using System.Collections.Generic;

namespace Server.Multis.CustomBuilding
{
	public class BaseBuildingGump : AdvGump
	{
		protected const string m_Signature = "<center>Custom Building<br>Version: 1.0 Beta<br>Author: Sunny Productions</center>";

		public BaseBuildingGump() : base()
		{
			Closable = true;
			Disposable = true;
			Dragable = true;
			Resizable = false;
			AddPage(0);
		}
	}

	public class ConstructGump : BaseBuildingGump
	{
		public ConstructGump() : base()
		{
			AddBackground(0, 0, 450, 438, 3500);
			AddBackground(100, 25, 250, 75, 3000);
			AddHtml(105, 35, 240, 54, m_Signature, false, false);
			AddButton(275, 375, 247, 248, 1, GumpButtonType.Reply, 0);
			AddButton(75, 375, 242, 241, 0, GumpButtonType.Reply, 0);
			AddLabel(50, 125, 0, "Building name:");
			AddBackground(150, 125, 200, 25, 9350);
			AddTextEntry(155, 130, 190, 20, 0, 0, "");
			AddLabel(50, 166, 0, "Options:");
			AddBackground(50, 195, 350, 149, 9350);
			AddLabel(100, 210, 0, "Filebased");
			AddLabel(265, 210, 0, "Scriptbased");
			AddLabel(100, 265, 0, "Only statics");
			AddRadio(60, 210, 9792, 9793, false, 1);
			AddRadio(225, 210, 9792, 9793, false, 2);
			AddCheck(60, 265, 9792, 9793, false, 10);
		}

		public override void OnResponse(NetState sender, RelayInfo info)
		{
			switch (info.ButtonID)
			{
				case 0: break;

				case 1:
					string name = info.GetTextEntry(0).Text;
					bool scriptbased = false;
					bool onlystatics = false;

					for (int i = 0; i < info.Switches.Length; i++)
					{
						int m = info.Switches[i];
						switch (m)
						{
							case 2: scriptbased = true; break;
							case 20: onlystatics = true; break;
						}
					}

					if (name == "" || name.Contains("_")  ||(!scriptbased && FileBasedBuilding.BuildingTable.ContainsKey(name)))
						sender.Mobile.SendMessage("That name is either already used or not valid. Do not use underscores.");

					else
						BoundingBoxPicker.Begin(sender.Mobile, new BoundingBoxCallback(BuildingCreator.CreateBuilding), new object[] { name, scriptbased, onlystatics });
					break;
			}
		}
	}

	public class ManageFileBasedBuildingsGump : BaseBuildingGump
	{
		public ManageFileBasedBuildingsGump() : base()
		{
			AddBackground(0, 0, 450, 438, 3500);
			AddBackground(100, 25, 250, 75, 3000);
			AddHtml(105, 35, 240, 54, m_Signature, false, false);
			AddButton(275, 375, 247, 248, 0, GumpButtonType.Reply, 0);
			AddButton(75, 375, 242, 241, 0, GumpButtonType.Reply, 0);
			AddLabel(50, 125, 0, "Options:");
			AddBackground(50, 150, 350, 191, 9350);
			AddButton(60, 160, 9792, 9793, 1, GumpButtonType.Reply, 0); AddLabel(100, 160, 0, "Show All Buildings (incl. components)");
			AddButton(60, 185, 9792, 9793, 2, GumpButtonType.Reply, 0); AddLabel(100, 185, 0, "Show Components TreeWise (incl. components)");
			AddButton(60, 210, 9792, 9793, 3, GumpButtonType.Reply, 0); AddLabel(100, 210, 0, "Show Full Buildings (excl. components)");
			AddButton(60, 235, 9792, 9793, 4, GumpButtonType.Reply, 0); AddLabel(100, 235, 0, "Show Buildings Without Graphics (incl. components)");
			AddButton(60, 260, 9792, 9793, 5, GumpButtonType.Reply, 0); AddLabel(100, 260, 0, "Show All BuildingTypes (excl. components)");
			AddButton(60, 285, 9792, 9793, 6, GumpButtonType.Reply, 0); AddLabel(100, 285, 0, "Show Unused BuildingTypes (excl. components)");
		}

		public override void OnResponse(NetState sender, RelayInfo info)
		{
			Mobile m = sender.Mobile;

			switch (info.ButtonID)
			{
				case 0: break;
				case 1: m.SendGump(new BuildingListGump(false, 0)); break;
				case 2: m.SendGump(new BuildingListGump(false, 1)); break;
				case 3: m.SendGump(new BuildingListGump(false, 2)); break;
				case 4: m.SendGump(new BuildingListGump(false, 3)); break;
				case 5: m.SendGump(new NameListGump(4)); break;
				case 6: m.SendGump(new NameListGump(5)); break;
			}
		}
	}

	public class ManageScriptBasedBuildingsGump : BaseBuildingGump
	{
		public ManageScriptBasedBuildingsGump() : base()
		{
			AddBackground(0, 0, 450, 379, 3500);
			AddBackground(100, 25, 250, 75, 3000);
			AddHtml(105, 35, 240, 54, m_Signature, false, false);
			AddButton(279, 326, 247, 248, 0, GumpButtonType.Reply, 0);
			AddButton(75, 327, 242, 241, 0, GumpButtonType.Reply, 0);
			AddLabel(50, 125, 0, "Options:");
			AddBackground(50, 150, 350, 139, 9350);
			AddButton(60, 160, 9792, 9793, 1, GumpButtonType.Reply, 0); AddLabel(100, 160, 0, "Show All buildings (incl. components)");
			AddButton(60, 185, 9792, 9793, 2, GumpButtonType.Reply, 0); AddLabel(100, 185, 0, "Show Components TreeWise (incl. components)");
			AddButton(60, 210, 9792, 9793, 3, GumpButtonType.Reply, 0); AddLabel(100, 210, 0, "Show Full Buildings (excl. components)");
		}

		public override void OnResponse(NetState sender, RelayInfo info)
		{
			Mobile m = sender.Mobile;

			switch (info.ButtonID)
			{
				case 0: break;
				default:
				case 1: m.SendGump(new BuildingListGump(true, 0)); break;
				case 2: m.SendGump(new BuildingListGump(true, 1)); break;
				case 3: m.SendGump(new BuildingListGump(true, 2)); break;
			}
		}
	}

	public class BaseListGump : BaseBuildingGump
	{
		public BaseListGump() : base()
		{
			AddBackground(0, 0, 450, 591, 3500);
			AddBackground(100, 25, 250, 75, 3000);
			AddHtml(105, 35, 240, 54, m_Signature, false, false);
		}
	}

	public class NameListGump : BaseListGump
	{
		private int m_MasterCount;
		private int m_SmallCount;
		private int m_Option;
		private List< string> m_NameList;
		private List<int> m_CountList;

		public NameListGump(int option) : base()
		{
			m_Option = option;

			m_NameList = new List< string> ();
			m_CountList = new List<int>();
			Dictionary<string, int> namecounttable = new Dictionary<string, int>();

			foreach (Item item in World.Items.Values)
			{
				if (item is FileBasedBuilding && FileBasedBuilding.BuildingTable.ContainsKey(item.Name))
				{
					if (namecounttable.ContainsKey(item.Name))
						namecounttable[item.Name]++;

					else
						namecounttable[item.Name] = 1;
				}
			}

			foreach (KeyValuePair<string, BuildingEntry> de in FileBasedBuilding.BuildingTable)
			{
				if (!(de.Value is ComponentEntry))
				{
					if (namecounttable.ContainsKey((string)de.Key))
					{
						if (m_Option == 4)
						{
							m_NameList.Add((string)de.Key);
							m_CountList.Add(namecounttable[(string)de.Key]);
						}
					}
					else
					{
						m_NameList.Add((string)de.Key);
						m_CountList.Add(0);
					}
				}
			}
			AddBasics();
			AddData();
		}

		public NameListGump(int count, int option, List<int> countlist, List<string> namelist) : base()
		{
			m_NameList = namelist;
			m_CountList = countlist;
			m_MasterCount = count;
			m_Option = option;

			AddBasics();
			AddData();
		}

		private void AddBasics()
		{
			if (m_MasterCount + 20 < m_CountList.Count)
				AddButton(289, 545, 9771, 9773, 2, GumpButtonType.Reply, 0);
			if (m_MasterCount >= 20)
				AddButton(75, 542, 9770, 9772, 1, GumpButtonType.Reply, 0);
		}

		private void AddData()
		{
			List<string> data = new List<string>();
			string[] color = new string[]{"33333", "33333", "33333"};
			int[] collumns = new int[]{100, 160, 80};

			data.Add("Name:");
			data.Add("Existing instances:");
			data.Add("Tiles:");

			for (int i = m_MasterCount; i < m_CountList.Count; i++)
			{
				int count = m_CountList[i];
				string name = m_NameList[i];

				AddButton(50, 145 + 18 * m_SmallCount, 2224, 2224, 100 + m_SmallCount, GumpButtonType.Reply, 0);
				data.Add(name);
				data.Add(count.ToString());

				BuildingEntry entry;
				if(FileBasedBuilding.BuildingTable.TryGetValue(name, out entry))
					data.Add(entry.ComponentList.List.Length.ToString());

				if (++m_SmallCount > 20)
					break;
			}

			AddTable(75, 125, collumns, data, color);
		}

		public override void OnResponse(NetState sender, RelayInfo info)
		{
			Mobile m = sender.Mobile;

			if (info.ButtonID == 0)
				m.SendGump(new ManageFileBasedBuildingsGump());

			if (info.ButtonID == 1)
			{
				int count = m_MasterCount - 20;
				if (count < 0)
					count = 0;
				m.SendGump(new NameListGump(count, m_Option, m_CountList, m_NameList));
			}

			else if (info.ButtonID == 2)
				m.SendGump(new NameListGump(m_MasterCount + 20, m_Option, m_CountList, m_NameList));

			else if (info.ButtonID > 99)
			{
				m.SendGump(new BuildingGump(m_NameList[info.ButtonID - 100 + m_MasterCount]));
			}
		}
	}

	public class BuildingListGump : BaseListGump
	{
		List<BaseBuilding> m_List = new List<BaseBuilding>();
		private int m_MasterCount;
		private int m_SmallCount;
		private int m_Option;
		private bool m_ScriptBased;

		public BuildingListGump(List<BaseBuilding> list, int count, int option, bool scriptbased) : base()
		{
			m_List = list;
			m_MasterCount = count;
			m_Option = option;
			m_ScriptBased = scriptbased;

			AddBasics();
			AddData();
		}

		public BuildingListGump(bool scriptbased, int option) : base()
		{
			m_Option = option;
			m_ScriptBased = scriptbased;

			foreach (Item item in World.Items.Values)
			{
				if (scriptbased)
				{
					if (item is ScriptBasedBuilding)
					{
						if (option == 0 || (option == 2 && !(item is ScriptBasedBuildingAddon)))
							m_List.Add((BaseBuilding)item);
						else if (option == 1 && !(item is ScriptBasedBuildingAddon))
						{
							if (!(item is ScriptBasedAddonBuilding))
								m_List.Add((BaseBuilding)item);
							else
							{
								m_List.Add((BaseBuilding)item);

								List<ScriptBasedBuildingAddon> addonlist = ((ScriptBasedAddonBuilding)item).AddonComponents;
								for (int i = 0; i < addonlist.Count; i++)
									m_List.Add((BaseBuilding)addonlist[i]);
							}
						}
					}
				}

				else if (item is FileBasedBuilding)
				{
					if (option == 3)
					{
						if (!FileBasedBuilding.BuildingTable.ContainsKey(item.Name))
							m_List.Add((BaseBuilding)item);
					}

					else if (option == 0 || (option == 2 && !(item is FileBasedBuildingAddon)))
						m_List.Add((BaseBuilding)item);

					else if (option == 1 && !(item is FileBasedBuildingAddon))
					{
						if (!(item is FileBasedAddonBuilding))
							m_List.Add((BaseBuilding)item);
						else
						{
							m_List.Add((BaseBuilding)item);

							List<FileBasedBuildingAddon> addonlist = ((FileBasedAddonBuilding)item).AddonComponents;
							for (int i = 0; i < addonlist.Count; i++)
								m_List.Add((BaseBuilding)addonlist[i]);
						}
					}
				}
			}
			AddBasics();
			AddData();
		}

		private void AddBasics()
		{
			if (m_MasterCount + 20 < m_List.Count)
				AddButton(289, 545, 9771, 9773, 2, GumpButtonType.Reply, 0);
			if (m_MasterCount >= 20)
				AddButton(75, 542, 9770, 9772, 1, GumpButtonType.Reply, 0);
		}

		private void AddData()
		{
			List<string> data = new List<string>();
			string[] color = new string[] { "33333", "33333", "33333", "33333" };
			int[] collumns = new int[] { 80, 135, 80, 80 };

			data.Add("Name:");
			data.Add("Location:");
			data.Add("Map:");
			data.Add("Tiles:");

			for (int i = m_MasterCount; i < m_List.Count; i++)
			{
				BaseBuilding building = m_List[i];

				AddButton(50 + Indent_int(building), 145 + 18 * m_SmallCount, 2224, 2224, 100 + m_SmallCount, GumpButtonType.Reply, 0);

				data.Add(Indent_str(building) + building.Name);
				data.Add(Indent_str(building) + building.Location.ToString());
				data.Add(Indent_str(building) + building.Map.ToString());
				data.Add(Indent_str(building) + building.Components.List.Length.ToString());

				if (++m_SmallCount > 20)
					break;
			}

			AddTable(75, 125, collumns, data, color);
		}

		private string Indent_str(BaseBuilding building)
		{
			if (m_Option == 1 && ((building is FileBasedBuildingAddon) || (building is ScriptBasedBuildingAddon)))
				return "  ";
			return "";
		}

		private int Indent_int(BaseBuilding building)
		{
			if (m_Option == 1 && ((building is FileBasedBuildingAddon) || (building is ScriptBasedBuildingAddon)))
				return 15;
			return 0;
		}

		public override void OnResponse(NetState sender, RelayInfo info)
		{
			Mobile m = sender.Mobile;

			if (info.ButtonID == 0)
			{
				if (!m_ScriptBased)
					m.SendGump(new ManageFileBasedBuildingsGump());

				else
					m.SendGump(new ManageScriptBasedBuildingsGump());
			}

			else if (info.ButtonID == 1)
			{
				int count = m_MasterCount - 20;
				if (count < 0)
					count = 0;
				m.SendGump(new BuildingListGump(m_List, count, m_Option, m_ScriptBased));
			}

			else if (info.ButtonID == 2)
				m.SendGump(new BuildingListGump(m_List, m_MasterCount + 20, m_Option, m_ScriptBased));

			else if (info.ButtonID > 99)
			{
				//m.SendGump(new BuildingListGump(m_List, m_MasterCount, m_Option, m_ScriptBased));
				//m.SendGump(this);

				BaseBuilding building = m_List[info.ButtonID - 100 + m_MasterCount];
				if (building == null || building.Deleted)
					return;

				m.SendGump(new BuildingGump(building));
			}
		}
	}

	public class BuildingGump : BaseBuildingGump
	{
		BaseBuilding m_Building;
		string m_Name;

		public BuildingGump()
			: base()
		{
			AddBackground(0, 0, 300, 348, 3500);
			AddBackground(25, 25, 250, 75, 3000);
			AddHtml(30, 35, 240, 54, m_Signature, false, false);
			AddButton(111, 290, 247, 248, 0, GumpButtonType.Reply, 0);
			AddBackground(40, 110, 220, 75, 3000);
			AddBackground(25, 200, 250, 85, 9350);
		}

		public BuildingGump(string name) : this()
		{
			m_Name = name;

			AddLabel(45, 115, 0, "Name:");
			AddLabel(125, 115, 0, name);
			AddButton(130, 240, 2224, 2224, 4, GumpButtonType.Reply, 0);
			AddLabel(165, 235, 0, "Remove Type");
			AddButton(30, 240, 2224, 2224, 5, GumpButtonType.Reply, 0);
			AddLabel(60, 235, 0, "Construct");
			AddButton(30, 260, 2224, 2224, 6, GumpButtonType.Reply, 0);
			AddLabel(60, 260, 0, "Export Type");
		}

		public BuildingGump(BaseBuilding building)
			: this()
		{
			m_Building = building;

			AddLabel(45, 115, 0, "Name:");
			AddLabel(45, 135, 0, "Location:");
			AddLabel(45, 155, 0, "Map:");
			AddLabel(125, 115, 0, building.Name);
			AddLabel(125, 135, 0, building.Location.ToString());
			AddLabel(125, 155, 0, building.Map.ToString());
			AddButton(30, 215, 2224, 2224, 1, GumpButtonType.Reply, 0);
			AddLabel(60, 210, 0, "Go To");
			AddButton(30, 240, 2224, 2224, 2, GumpButtonType.Reply, 0);
			AddLabel(60, 235, 0, "Remove");
			AddButton(130, 215, 2224, 2224, 3, GumpButtonType.Reply, 0);
			AddLabel(165, 210, 0, "Props");

			if (building is FileBasedBuilding)
			{
				AddButton(130, 240, 2224, 2224, 4, GumpButtonType.Reply, 0);
				AddLabel(165, 235, 0, "Remove Type");
				AddButton(30, 260, 2224, 2224, 6, GumpButtonType.Reply, 0);
				AddLabel(60, 260, 0, "Export Type");
			}
		}

		public override void OnResponse(NetState sender, RelayInfo info)
		{
			Mobile m = sender.Mobile;

			if (m_Building == null || m_Building is FileBasedBuilding)
				m.SendGump(new ManageFileBasedBuildingsGump());

			else
				m.SendGump(new ManageScriptBasedBuildingsGump());

			if (info.ButtonID == 6)
			{
				if (m_Building != null)
					FileBasedBuilding.ExportBuilding(m_Building.Name, m);

				else
					FileBasedBuilding.ExportBuilding(m_Name, m);
			}

			else if (info.ButtonID == 5)
			{
				m.BeginTarget(-1, true, TargetFlags.None, new TargetStateCallback(Commands.OnTarget_Construct), m_Name);
				m.SendMessage("Please target the location where you wish to place the house.");
			}

			else if (info.ButtonID == 4)
			{
				string name = m_Name;

				if (m_Building != null)
					name = m_Building.Name;

				List<string> nameslist = new List<string>();
				nameslist.Add(name);

				BuildingEntry entry;

				if (FileBasedBuilding.BuildingTable.TryGetValue(name, out entry))
				{
					if (entry is AddonEntry)
						foreach (string str in ((AddonEntry)entry).Components)
						{
							nameslist.Add(name);
							FileBasedBuilding.BuildingTable.Remove(str);
						}

					else if (entry is ComponentEntry)
						foreach (KeyValuePair<string, BuildingEntry> de in FileBasedBuilding.BuildingTable)
							if (de.Value is AddonEntry)
								((AddonEntry)de.Value).Components.Remove(name);

					ArrayList list = new ArrayList();
					foreach (Item item in World.Items.Values)
					{
						if (item is FileBasedBuilding && nameslist.Contains(item.Name))
							list.Add(item);
					}

					foreach (Item item in list)
						item.Delete();

					FileBasedBuilding.BuildingTable.Remove(name);
					FileBasedBuilding.SeperateData.Save();
				}
			}

			else if (m_Building != null && !m_Building.Deleted)
			{
				switch (info.ButtonID)
				{
					case 1:
						Point3D loc = m_Building.Location;
						m.MoveToWorld(new Point3D(loc.X,loc.Y,loc.Z + 60), m_Building.Map);

						break;

					case 2:
						m_Building.Delete();
						break;

					case 3:
						m.SendGump(new PropertiesGump(m, m_Building));
						break;
				}
			}
		}
	}
}