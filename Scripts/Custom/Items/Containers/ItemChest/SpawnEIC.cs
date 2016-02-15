using System;
using Server.Spells;
using Server.Items;
using Server.Targeting;
using Server.Commands;
using System.Collections;
using Server.Regions;

namespace Server.Scripts.Commands
{
	public class SpawnEIC
	{
		public static void Initialize()
		{
			CommandSystem.Register("SpawnEIC", AccessLevel.Administrator, new CommandEventHandler(SpawnEIC_OnCommand));
		}

		private static int[,] ChestIds = new int[,] {{0x9A9,0xE7E}, {0xE3F,0xE3E}, {0xE3D,0xE3C}, {0xe43,0xe42}, {0x9ab,0xe7c}, {0xe41,0xe40}};

		private static BaseItemChest GetChest( int level )
		{
			switch( level )
			{
				default:
				case 0: return new EICSmallCrate();
				case 1: return new EICMediumCrate();
				case 2: return new EICLargeCrate();
				case 3: return new EICWooden();
				case 4: return new EICMetal();
				case 5: return new EICMetalGolden();
			}
		}

		[Usage( "SpawnEIC" )]
		[Description( "" )]
		private static void SpawnEIC_OnCommand( CommandEventArgs e )
		{
			e.Mobile.SendMessage("Spawning Item Chests...");

			ArrayList alChests = new ArrayList();
			int counter = 0;

			foreach ( Item i in World.Items.Values )
			{
				if( i is LockableContainer && !(i is BaseItemChest) && !(i is BaseTreasureChest) && !i.Movable )
				{
					Region currentRegion = Region.Find( i.Location, i.Map );
					if (currentRegion is DungeonRegion)
						alChests.Add( (LockableContainer)i );
				}
			}

			foreach ( LockableContainer cont in alChests )
			{
				for( int a=0;a<6;a++ )
					for( int b=0;b<2;b++ )
						if( cont.ItemID == ChestIds[a,b] )
						{
							BaseItemChest chest = GetChest( a );
							chest.ItemID = cont.ItemID;
							chest.Hue = cont.Hue;
							chest.MoveToWorld( cont.Location, cont.Map );
							cont.Delete();
							counter++;
						}
			}

			e.Mobile.SendMessage("Done... {0} Item Chests added.", counter);
		}
	}
}