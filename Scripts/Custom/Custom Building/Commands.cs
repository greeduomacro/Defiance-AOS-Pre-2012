//////////////////////////////\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
//			   This is a Sunny Production ©2006					\\
//					 Based on RunUO©							\\
//					Version: Beta 1.0							\\
//		Many thanks to my Csharp tutors Will and Eclipse		\\
//////////////////////////////\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
//
using Server.Commands;
using Server.Targeting;

namespace Server.Multis.CustomBuilding
{
	public static class Commands
	{
		public static void Initialize()
		{
			CommandSystem.Register("ManageFileBasedBuildings", AccessLevel.GameMaster, new CommandEventHandler(OnCommand_ManageFileBasedBuildings));
			CommandSystem.Register("ManageScriptBasedBuildings", AccessLevel.GameMaster, new CommandEventHandler(OnCommand_ManageScriptBasedBuildings));
			CommandSystem.Register("MFBB", AccessLevel.GameMaster, new CommandEventHandler(OnCommand_ManageFileBasedBuildings));
			CommandSystem.Register("MSBB", AccessLevel.GameMaster, new CommandEventHandler(OnCommand_ManageScriptBasedBuildings));
			CommandSystem.Register("ImportFileBasedBuilding", AccessLevel.GameMaster, new CommandEventHandler(OnCommand_ImportFileBasedBuilding));
			CommandSystem.Register("IFBB", AccessLevel.GameMaster, new CommandEventHandler(OnCommand_ImportFileBasedBuilding));
			CommandSystem.Register("CreateBuilding", AccessLevel.GameMaster, new CommandEventHandler(OnCommand_Create));
			CommandSystem.Register("AddBuilding", AccessLevel.GameMaster, new CommandEventHandler(OnCommand_Construct));
		}

		[Usage("ManageFileBasedBuildings")]
		private static void OnCommand_ManageFileBasedBuildings(CommandEventArgs e)
		{
			Mobile m = e.Mobile;
			m.SendGump(new ManageFileBasedBuildingsGump());
		}

		[Usage("ManageScriptBasedBuildings")]
		private static void OnCommand_ManageScriptBasedBuildings(CommandEventArgs e)
		{
			Mobile m = e.Mobile;
			m.SendGump(new ManageScriptBasedBuildingsGump());
		}

		[Usage("CreateBuilding")]
		private static void OnCommand_Create(CommandEventArgs e)
		{
			Mobile m = e.Mobile;
			m.SendGump(new ConstructGump());
		}

		[Usage("ImportFileBasedBuilding")]
		private static void OnCommand_ImportFileBasedBuilding(CommandEventArgs e)
		{
			Mobile m = e.Mobile;

			if (e.Length == 1)
			{
				string name = e.GetString(0);
				if (FileBasedBuilding.ImportBuilding(name))
				{
					FileBasedBuilding.SeperateData.Save();
					m.SendMessage("The building has been succesfully imported and is ready to be used.");
				}
				else
					m.SendMessage("The building could not be imported, either the file does not exist or an error occured dureing the load.");
			}

			else
				m.SendMessage("Wrong format: [Command] [buildingname] >> [buildingname] = filename without .bin (castle.bin = castle)");
		}


		[Usage("AddBuilding")]
		private static void OnCommand_Construct(CommandEventArgs e)
		{
			Mobile m = e.Mobile;

			if (e.Length == 1)
			{
				string name = e.GetString(0);
				m.BeginTarget(-1, true, TargetFlags.None, new TargetStateCallback(OnTarget_Construct), name);
				m.SendMessage("Please target the loaction where you wish to place the house.");
			}
		}

		public static void OnTarget_Construct(Mobile from, object targeted, object state)
		{
			string name = (string)state;
			IPoint3D p = (IPoint3D)targeted;

			if (p != null)
			{
				Point3D point = new Point3D(p);

				BuildingEntry entry;

				if (FileBasedBuilding.BuildingTable.TryGetValue(name, out entry))
				{
					FileBasedBuilding building = null;

					switch (entry.BuildType)
					{
						case 0: building = new FileBasedBuilding(name); break;
						case 1: building = new FileBasedBuildingAddon(name); break;
						case 2: building = new FileBasedAddonBuilding(name); break;
					}

					if (building != null)
						building.MoveToWorld(point, from.Map);
				}

				else
					from.SendMessage("No building found with that name.");
			}
		}
	}
}