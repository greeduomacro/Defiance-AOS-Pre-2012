using System;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Multis;
using Server.Mobiles;

namespace Server.Misc
{
	public class Cleanup
	{
		public static void Initialize()
		{
			Timer.DelayCall( TimeSpan.FromSeconds( 2.5 ), new TimerCallback( Run ) );
		}

		public static void Run()
		{
			List<Item> items = new List<Item>();
			List<Item> validItems = new List<Item>();
			List<Mobile> hairCleanup = new List<Mobile>();

			List<Mobile> orphans = new List<Mobile>();
			int stabled = 0;

			foreach ( Mobile m in World.Mobiles.Values )
			{
				if ( m is PlayerMobile && ((PlayerMobile)m).Account == null )
					orphans.Add( m );
				else
				{
					BaseCreature pet = m as BaseCreature;
					if ( pet != null && pet.Controlled && !pet.Summoned && pet.ControlMaster != null && !pet.Blessed && pet.ControlMaster.Player && !pet.IsStabled && pet.Map != Map.Internal )
					{
						if ( pet.IsDeadPet )
							pet.Resurrect();

						Mobile master = pet.ControlMaster;

						pet.ControlTarget = null;
						pet.ControlOrder = OrderType.Stay;
						pet.Internalize();

						pet.SetControlMaster( null );
						pet.SummonMaster = null;

						pet.IsStabled = true;

						//if ( Core.SE )
							pet.Loyalty = BaseCreature.MaxLoyalty; // Wonderfully happy

						master.Stabled.Add( pet );
						stabled++;
					}
				}
			}

			int boxes = 0;
			int spawners = 0;
			int parents = 0;
			int emptyspawners = 0;

			foreach ( Item item in World.Items.Values )
			{
				if ( item.Map == null )
				{
					items.Add( item );
					continue;
				}
				else if ( item is Spawner )
				{
					if ( item.Map == Map.Internal && item.Parent == null )
					{
						spawners++;
						continue;
					}
					else if ( item.Parent == null && (((Spawner)item).Entries == null || ((Spawner)item).Entries.Count == 0) )
					{
						items.Add( item );
						//Console.WriteLine( "Cleanup: Detected invalid spawner {0} at ({1},{2},{3}) [{4}]", item.Serial, item.X, item.Y, item.Z, item.Map );
						emptyspawners++;
					}
				}
				else if ( item is CommodityDeed )
				{
					CommodityDeed deed = (CommodityDeed)item;

					if ( deed.CommodityItem != null )
						validItems.Add( deed.CommodityItem );

					continue;
				}
				else if ( item is BaseHouse )
				{
					BaseHouse house = (BaseHouse)item;

					foreach ( RelocatedEntity relEntity in house.RelocatedEntities )
						if ( relEntity.Entity is Item )
							validItems.Add( (Item)relEntity.Entity );

					foreach ( VendorInventory inventory in house.VendorInventories )
						foreach ( Item subItem in inventory.Items )
							validItems.Add( subItem );
				}
				else if ( item is BankBox )
				{
					BankBox box = (BankBox)item;
					Mobile owner = box.Owner;

					if ( owner == null )
					{
						items.Add( box );
						++boxes;
					}
					else if ( box.Items.Count == 0 )
					{
						items.Add( box );
						++boxes;
					}

					continue;
				}
				else if ( (item.Layer == Layer.Hair || item.Layer == Layer.FacialHair) )
				{
					object rootParent = item.RootParent;

					if ( rootParent is Mobile )
					{
						Mobile rootMobile = (Mobile)rootParent;
						if ( item.Parent != rootMobile /*&& rootMobile.AccessLevel == AccessLevel.Player*/ )
						{
							items.Add( item );
							continue;
						}
						else if( item.Parent == rootMobile )
						{
							hairCleanup.Add( rootMobile );
							continue;
						}
					}
				}
				else if ( item is Container )
				{
					for ( int i = 0; i < item.Items.Count; i++ )
					{
						Item child = item.Items[i];

						if ( child.Parent != item )
						{
							//if ( child is Spawner && child.Parent == null )
							//	item.Items.RemoveAt( i-- );

							Console.WriteLine( "Cleanup: Detected orphan item {0} ({1}) of {2} ({3})", child.GetType().Name, child.Serial, item.GetType().Name, item.Serial );
							parents++;
						}
					}
				}
				else if ( item.RootParent is BaseTreasureChest ) //Clear out all items in a treasure chest
				{
					items.Add( item );
					continue;
				}

				if ( item.Parent != null || ( item.Map != null && item.Map != Map.Internal ) || item.HeldBy != null )
					continue;

				if ( item.Location != Point3D.Zero )
					continue;

				if ( !IsBuggable( item ) )
					continue;

				if ( !item.Movable )
					continue;

				items.Add( item );
			}

			for ( int i = 0; i < validItems.Count; ++i )
				items.Remove( validItems[i] );

			if ( items.Count > 0 )
			{
				String message = String.Format( "Cleanup: Detected {0} inaccessible items, ", items.Count );

				if ( boxes > 0 )
					message += String.Format( "including {0} bank boxes, ", boxes );

				if ( emptyspawners > 0 )
					message += String.Format( "including {0} empty/invalid spawners, ", emptyspawners );

				message += "removing..";

				Console.WriteLine( message );

				for ( int i = 0; i < items.Count; ++i )
					items[i].Delete();
			}

			if ( spawners > 0 )
				Console.WriteLine( "Cleanup: Detected {0} inaccessible spawners..", spawners );

			if ( parents > 0 )
				Console.WriteLine( "Cleanup: Detected {0} invalid parent-child items..", parents );

			if ( hairCleanup.Count > 0 )
			{
				Console.WriteLine( "Cleanup: Detected {0} hair and facial hair items being worn, converting to virtual hair..", hairCleanup.Count );

				for ( int i = 0; i < hairCleanup.Count; i++ )
					hairCleanup[i].ConvertHair();
			}

			if ( orphans.Count > 0 )
			{
				Console.WriteLine( "Cleanup: Detected {0} orphaned players, removing..", orphans.Count );

				for ( int i = 0; i < orphans.Count; ++i )
					orphans[i].Delete();
			}

			if ( stabled > 0 )
				Console.WriteLine( "Cleanup: Detected {0} pets requiring stables..", stabled );
		}

		public static bool IsBuggable( Item item )
		{
			if ( item is Fists )
				return false;

			if ( item is ICommodity || item is Multis.BaseBoat
				|| item is Fish || item is BigFish
				|| item is BasePotion || item is Food || item is CookableFood
				|| item is SpecialFishingNet || item is BaseMagicFish
				|| item is Shoes || item is Sandals
				|| item is Boots || item is ThighBoots
				|| item is TreasureMap || item is MessageInABottle
				|| item is BaseArmor || item is BaseWeapon
				|| item is BaseClothing || item is Bone
				|| (item is BaseJewel && Core.AOS)
				|| (item is BasePotion && Core.ML))
				return true;

			return false;
		}
	}
}