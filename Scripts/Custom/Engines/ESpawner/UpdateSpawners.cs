using System;
using System.Collections;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Regions;

namespace Server.Commands
{
	public class UpdateSpawners
	{
		public static void Initialize()
		{
			CommandSystem.Register("UpdateSpawners", AccessLevel.Administrator, new CommandEventHandler(UpdateSpawners_OnCommand));
		}

		[Usage( "UpdateSpawners" )]
		[Description( "UpdateSpawners command." )]
		private static void UpdateSpawners_OnCommand( CommandEventArgs e )
		{
			ArrayList SpawnerList = new ArrayList();

			foreach (object o in World.Items.Values)
			{
				if( o is ESpawner && !((ESpawner)o).Deleted )
					SpawnerList.Add(o);
			}

			e.Mobile.SendMessage("Updating Spawners...");
			foreach (ESpawner spawner in SpawnerList)
			{
				if( spawner.IgnoreWorldSpawn )
					continue;

				Region currentRegion = Region.Find( spawner.Location, spawner.Map );
				if( currentRegion is DungeonRegion )
				{
					if( spawner.MinDelay != TimeSpan.FromSeconds( 30.0 ) )
						spawner.MinDelay = TimeSpan.FromSeconds( 30.0 );

					if( spawner.MaxDelay != TimeSpan.FromSeconds( 60.0 ) )
						spawner.MaxDelay = TimeSpan.FromSeconds( 60.0 );
				}
				else
				{
					if( spawner.MinDelay != TimeSpan.FromMinutes( 1.0 ) )
						spawner.MinDelay = TimeSpan.FromMinutes( 1.0 );

					if( spawner.MaxDelay != TimeSpan.FromMinutes( 2.0 ) )
						spawner.MaxDelay = TimeSpan.FromMinutes( 2.0 );
				}
			}
			e.Mobile.SendMessage("Done...");
		}
	}
}