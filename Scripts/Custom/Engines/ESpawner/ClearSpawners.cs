using System;
using System.Collections;
using Server;
using Server.Items;
using Server.Mobiles;

namespace Server.Commands
{
	public class ClearSpawners
	{
		public static void Initialize()
		{
			CommandSystem.Register("ClearSpawners", AccessLevel.Administrator, new CommandEventHandler(ClearSpawners_OnCommand));
		}

		[Usage( "ClearSpawners" )]
		[Description( "ClearSpawners command." )]
		private static void ClearSpawners_OnCommand( CommandEventArgs e )
		{
			ArrayList SpawnerList = new ArrayList();

			foreach (object o in World.Items.Values)
			{
				if( o is Spawner && !((Spawner)o).Deleted && ((Spawner)o).Map == e.Mobile.Map )
					SpawnerList.Add(o);
			}
			int amount = 0;

			e.Mobile.SendMessage("Removing Spawners on this facet...");
			foreach (Spawner spawner in SpawnerList)
			{
				amount++;
				spawner.Delete();
			}
			e.Mobile.SendMessage("Done... Removed {0} Spawners.", amount);
		}
	}
}