//******************************************************
// Name: BaseCreatureLoot
// Desc: Written by Eclipse
//******************************************************
using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Regions;

namespace Server.Mobiles
{
	public class BaseCreatureLoot
	{
		private static void IncreaseTypeAmount( Container c, Type type, double iMultiplyValue )
		{
			Item item = c.FindItemByType(type);
			if( item != null && item.Stackable )
				item.Amount = (int)(item.Amount * iMultiplyValue);
		}

		public static int GetPoisonFungusAmount( int fame )
		{
			int amount = fame / 1000;

			if( amount > 1 )
				return Utility.RandomMinMax( amount, amount+5 );
			else return 1;
		}

		public static void BCLoot( BaseCreature creature, Container c, bool spawning )
		{
			if( creature.Controlled || creature.Summoned || creature.NoKillAwards )
				return;

			//if( creature.IsChampionMonster && 0.0005 > Utility.RandomDouble() )
			//	BaseCreatureLoot.GivePowerScroll(creature);

			if( creature.IsPlagued && !creature.IgnorePlagueLoot )
			{
				if ( creature.TreasureMapLevel >= 0 && Plague.ChestChance > Utility.RandomDouble() )
					c.DropItem( new PlagueChest( creature.Name, creature.TreasureMapLevel ) );

				LootPackEntry.AddMoreLoot( creature, c, spawning, 1000, 1 );
				IncreaseTypeAmount( c, typeof(Gold), 1.50 );
			}
			else if ( creature.IsElder )
			{
				LootPackEntry.AddMoreLoot( creature, c, spawning, 1000, 1 );
				IncreaseTypeAmount( c, typeof(Gold), 1.60 );
			}
			else if ( creature.IsParagon )
			{
				LootPackEntry.AddMoreLoot( creature, c, spawning, 1000, 1 );
				IncreaseTypeAmount( c, typeof(Gold), 1.50 );
			}
		}

		public static void GivePowerScroll(BaseCreature creature)
		{
			if (creature.Map != Map.Felucca)
				return;

			ArrayList toGive = new ArrayList();
			List<DamageStore> rights = BaseCreature.GetLootingRights(creature.DamageEntries, creature.HitsMax);

			for (int i = rights.Count - 1; i >= 0; --i)
			{
				DamageStore ds = rights[i];

				if (ds.m_HasRight)
					toGive.Add(ds.m_Mobile);
			}

			if (toGive.Count == 0)
				return;

			// Randomize
			for (int i = 0; i < toGive.Count; ++i)
			{
				int rand = Utility.Random(toGive.Count);
				object hold = toGive[i];
				toGive[i] = toGive[rand];
				toGive[rand] = hold;
			}

			for (int i = 0; i < 1; ++i)
			{
				Mobile m = (Mobile)toGive[i % toGive.Count];
				PowerScroll ps = PowerScroll.CreateRandomNoCraft(5, 5);

				m.SendLocalizedMessage(1049524); // You have received a scroll of power!

				if (!Core.SE || m.Alive)
					m.AddToBackpack(ps);
				else
				{
					if (m.Corpse != null && !m.Corpse.Deleted)
						((Container)m.Corpse).DropItem(ps);
					else
						m.AddToBackpack(ps);
				}
			}
		}
	}
}