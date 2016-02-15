using System;
using Server;
using Server.Items;
using Server.Targeting;
using Server.Commands;

namespace Server.Mobiles
{
	public class EFindSpawner
	{
		public static void Initialize()
		{
			CommandSystem.Register("EFindSpawner", AccessLevel.GameMaster, new CommandEventHandler(EFindSpawner_OnCommand));
		}

		[Usage("EFindSpawner")]
		[Description("")]
		private static void EFindSpawner_OnCommand(CommandEventArgs e)
		{
			e.Mobile.SendMessage(55, "Target a BaseCreature.");
			e.Mobile.Target = new EFindSpawnerTarget();
		}

		private class EFindSpawnerTarget : Target
		{
			public EFindSpawnerTarget()
				: base(-1, false, TargetFlags.None)
			{
			}

			protected override void OnTarget(Mobile from, object targeted)
			{
				if (targeted is BaseCreature)
				{
					foreach (object item in World.Items.Values)
					{
						if (item is ESpawner)
						{
							ESpawner spawner = (ESpawner)item;
							foreach (EclSpawnEntry entry in spawner.SpawnEntries)
							{
								foreach (object o in entry.SpawnObjects)
								{
									if (o is BaseCreature)
									{
										if (((BaseCreature)o).Serial == ((BaseCreature)targeted).Serial)
										{
											from.Location = spawner.Location;
											from.Map = spawner.Map;
											from.SendMessage(55, "Spawner found for creature.");
											return;
										}
									}
								}
							}
						}
					}
				}
				from.SendMessage(55, "No spawner found for creature.");
			}
		}
	}
}