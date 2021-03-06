using System;
using System.Collections.Generic;
using Server.Multis;
using Server.Mobiles;
using Server.Network;
using Server.ContextMenus;

namespace Server.Items
{
	public abstract class BaseContainer : Container
	{
		public override int DefaultMaxWeight
		{
			get
			{
				if ( IsSecure )
					return 0;

				return base.DefaultMaxWeight;
			}
		}

		public BaseContainer( int itemID ) : base( itemID )
		{
		}

		public override bool IsAccessibleTo( Mobile m )
		{
			if ( !BaseHouse.CheckAccessible( m, this ) )
				return false;

			return base.IsAccessibleTo( m );
		}

		public override bool CheckHold( Mobile m, Item item, bool message, bool checkItems, int plusItems, int plusWeight )
		{
			if ( this.IsSecure && !BaseHouse.CheckHold( m, this, item, message, checkItems, plusItems, plusWeight ) )
				return false;

			return base.CheckHold( m, item, message, checkItems, plusItems, plusWeight );
		}

		//Added by Blady:
		private class NameContainerEntry : ContextMenuEntry
		{
			private Mobile m_From;
			private BaseContainer m_Container;

			public NameContainerEntry( Mobile from, BaseContainer container) : base( 6175 ) //Inscribe
			{
				m_From = from;
				m_Container = container;
			}

			public override void OnClick()
			{
				if ( m_From.CheckAlive() && m_Container.IsChildOf( m_From.Backpack ) )
				{
					m_From.Prompt = new NameContainerPrompt( m_Container );
					m_From.SendMessage("Type the new name of the container:");
				}
			}
		}

		private class NameContainerPrompt : Server.Prompts.Prompt
		{
			private BaseContainer m_Container;

			public NameContainerPrompt( BaseContainer container )
			{
				m_Container = container;
			}

			public override void OnResponse( Mobile from, string text )
			{
				if ( text.Length > 40 )
					text = text.Substring( 0, 40 );

				if ( from.CheckAlive() && m_Container.IsChildOf( from.Backpack ) )
				{
					m_Container.Name = Utility.FixHtml( text.Trim() );

					from.SendMessage("The name has been changed.");
				}
			}

			public override void OnCancel( Mobile from )
			{
			}
		}
		//End of Blady Edit

		public override bool CheckItemUse( Mobile from, Item item )
		{
			if ( IsDecoContainer && item is BaseBook )
				return true;

			return base.CheckItemUse( from, item );
		}

		public override void GetContextMenuEntries( Mobile from, List<ContextMenuEntry> list )
		{
			base.GetContextMenuEntries( from, list );
			SetSecureLevelEntry.AddTo( from, this, list );

			//Name entry added by Blady:
			if ( from.CheckAlive() && IsChildOf( from.Backpack ) )
				list.Add( new NameContainerEntry( from, this ) );
		}

		public override bool TryDropItem( Mobile from, Item dropped, bool sendFullMessage )
		{
			if ( !CheckHold( from, dropped, sendFullMessage, true ) )
				return false;

			BaseHouse house = BaseHouse.FindHouseAt( this );

			if ( house != null && house.IsLockedDown( this ) )
			{
				if ( dropped is VendorRentalContract || ( dropped is Container && ((Container)dropped).FindItemByType( typeof( VendorRentalContract ) ) != null ) )
				{
					from.SendLocalizedMessage( 1062492 ); // You cannot place a rental contract in a locked down container.
					return false;
				}

				if ( !house.LockDown( from, dropped, false ) )
					return false;
			}

			List<Item> list = this.Items;

			for ( int i = 0; i < list.Count; ++i )
			{
				Item item = list[i];

				if ( !(item is Container) && item.StackWith( from, dropped, false ) )
					return true;
			}

			DropItem( dropped );

			return true;
		}

		public override bool OnDragDropInto( Mobile from, Item item, Point3D p )
		{
			if ( !CheckHold( from, item, true, true ) )
				return false;

			BaseHouse house = BaseHouse.FindHouseAt( this );

			if ( house != null && house.IsLockedDown( this ) )
			{
				if ( item is VendorRentalContract || ( item is Container && ((Container)item).FindItemByType( typeof( VendorRentalContract ) ) != null ) )
				{
					from.SendLocalizedMessage( 1062492 ); // You cannot place a rental contract in a locked down container.
					return false;
				}

				if ( !house.LockDown( from, item, false ) )
					return false;
			}

			item.Location = new Point3D( p.X, p.Y, 0 );
			AddItem( item );

			from.SendSound( GetDroppedSound( item ), GetWorldLocation() );

			return true;
		}

		public override void UpdateTotal( Item sender, TotalType type, int delta )
		{
			base.UpdateTotal( sender, type, delta );

			if ( type == TotalType.Weight && RootParent is Mobile )
				((Mobile) RootParent).InvalidateProperties();
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( from.AccessLevel > AccessLevel.Player || from.InRange( this.GetWorldLocation(), 2 ) || this.RootParent is PlayerVendor )
				Open( from );
			else
				from.LocalOverheadMessage( MessageType.Regular, 0x3B2, 1019045 ); // I can't reach that.
		}

		public virtual void Open( Mobile from )
		{
			DisplayTo( from );
		}

		public BaseContainer( Serial serial ) : base( serial )
		{
		}

		#region Consume[...]
		public int ConsumeTotalGroupedCompared( Type[][] types, int[] amounts, bool recurse, OnItemConsumed callback, CheckItemGroup grouper, Item compareTo )
		{
			if ( types.Length != amounts.Length )
				throw new ArgumentException();
			else if ( grouper == null )
				throw new ArgumentNullException();

			Item[][][] items = new Item[types.Length][][];
			int[] totals = new int[types.Length];

			bool hasComparer = false;

			Item originalComparer = compareTo;

			int comparerType = -1;

			for ( int i = 0; i < types.Length; ++i )
			{
				Item[] typedItems = FindItemsByType( types[i], recurse );

				List<List<Item>> groups = new List<List<Item>>();

				int idx = 0;

				if ( originalComparer != null )
				{
					for ( int j = 0; j < types[i].Length; ++j )
					{
						if ( originalComparer.GetType() == types[i][j] )
						{
							hasComparer = true;
							compareTo = originalComparer;
							comparerType = i;
							break;
						}
					}
				}

				while( idx < typedItems.Length )
				{
					Item a = typedItems[idx++];
					List<Item> group = new List<Item>();

					if ( !hasComparer )
					{
						compareTo = a;
						group.Add( a );
					}
					else if ( grouper( compareTo, a ) == 0 )
						group.Add( a );

					while( idx < typedItems.Length )
					{
						Item b = typedItems[idx];
						int v = grouper( compareTo, b );

						if( v == 0 )
							group.Add( b );
						else
							break;

						++idx;
					}

					groups.Add( group );
				}

				compareTo = originalComparer;
				hasComparer = false;

				items[i] = new Item[groups.Count][];

				bool hasEnough = false;

				for ( int j = 0; j < groups.Count; ++j )
				{
					items[i][j] = groups[j].ToArray();

					for ( int k = 0; k < items[i][j].Length; ++k )
						totals[i] += items[i][j][k].Amount;
				}

				if ( totals[i] >= amounts[i] )
					hasEnough = true;

				if ( !hasEnough )
					return i;
			}


			for ( int i = 0; i < items.Length; ++i )
			{
				int need = amounts[i];

				if ( originalComparer != null && comparerType == i )
				{
					object parent = originalComparer.RootParent;

					if ( parent is Mobile )
					{
						Mobile m = (Mobile)parent;

						if ( this.RootParent is Mobile && m == (Mobile)this.RootParent )
							ConsumeNeeded( ref need, originalComparer, callback );
					}
				}

				for ( int j = 0; j < items[i].Length && need > 0; ++j )
				{
					if ( totals[i] >= amounts[i] )
					{
						for ( int k = 0; k < items[i][j].Length && need > 0; ++k )
						{
							Item item = items[i][j][k];

							if ( !item.Deleted )
								ConsumeNeeded( ref need, item, callback );
						}
					}
				}
			}

			return -1;
		}

		private void ConsumeNeeded( ref int need, Item item, OnItemConsumed callback )
		{
			int theirAmount = item.Amount;

			if ( theirAmount < need )
			{
				if ( callback != null )
					callback( item, theirAmount );

				item.Delete();
				need -= theirAmount;
			}
			else
			{
				if ( callback != null )
					callback( item, need );

				item.Consume( need );
				need = 0;
			}
		}
		#endregion

		#region Get[Compared]Amount
		public int GetComparedGroupAmount( Type[] types, bool recurse, CheckItemGroup grouper, Item compareTo )
		{
			if( grouper == null )
				throw new ArgumentNullException();

			Item[] typedItems = FindItemsByType( types, recurse );

			bool hasComparer = false;

			if ( compareTo != null )
			{
				for ( int i = 0; i < types.Length; ++i )
				{
					if ( compareTo.GetType() == types[i] )
					{
						hasComparer = true;
						break;
					}
				}
			}

			List<List<Item>> groups = new List<List<Item>>();

			int idx = 0;

			while( idx < typedItems.Length )
			{
				Item a = typedItems[idx++];
				List<Item> group = new List<Item>();

				if ( !hasComparer )
				{
					compareTo = a;
					group.Add( a );
				}
				else if ( grouper( compareTo, a ) == 0 )
					group.Add( a );

				while( idx < typedItems.Length )
				{
					Item b = typedItems[idx];
					int v = grouper( compareTo, b );

					if( v == 0 )
						group.Add( b );
					else
						break;

					++idx;
				}

				groups.Add( group );
			}

			int total = 0;

			for( int j = 0; j < groups.Count; ++j )
			{
				Item[] items = groups[j].ToArray();

				for( int k = 0; k < items.Length; ++k )
					total += items[k].Amount;
			}

			return total;
		}

		public int GetAmountCompared( Type[] types, CheckItemGroup grouper, Item compareTo )
		{
			return GetAmountCompared( types, grouper, compareTo, true );
		}

		public int GetAmountCompared( Type[] types, CheckItemGroup grouper, Item compareTo, bool recurse )
		{
			Item[] items = FindItemsByType( types, recurse );

			int amount = 0;

			for ( int i = 0; i < items.Length; ++i )
			{
				if ( compareTo == null )
				{
					compareTo = items[i];
					amount += items[i].Amount;
				}
				else if ( grouper( compareTo, items[i] ) == 0 )
					amount += items[i].Amount;
			}

			return amount;
		}
		#endregion

		/* Note: base class insertion; we cannot serialize anything here */
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
		}
	}

	public class CreatureBackpack : Backpack	//Used on BaseCreature
	{
		[Constructable]
		public CreatureBackpack( string name )
		{
			Name = name;
			Layer = Layer.Backpack;
			Hue = 5;
			Weight = 3.0;
		}

		public override void AddNameProperty( ObjectPropertyList list )
		{
			if ( Name != null )
				list.Add( 1075257, Name ); // Contents of ~1_PETNAME~'s pack.
			else
				base.AddNameProperty( list );
		}

		public override void OnItemRemoved( Item item )
		{
			if ( Items.Count == 0 )
				this.Delete();

			base.OnItemRemoved( item );
		}

		public override bool OnDragLift( Mobile from )
		{
			if ( from.AccessLevel > AccessLevel.Player )
				return true;

			from.SendLocalizedMessage( 500169 ); // You cannot pick that up.
			return false;
		}

		public override bool OnDragDropInto( Mobile from, Item item, Point3D p )
		{
			return false;
		}

		public override bool TryDropItem( Mobile from, Item dropped, bool sendFullMessage )
		{
			return false;
		}

		public CreatureBackpack( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			if ( version == 0 )
				Weight = 13.0;
		}
	}

	public class StrongBackpack : Backpack	//Used on Pack animals
	{
		[Constructable]
		public StrongBackpack()
		{
			Layer = Layer.Backpack;
			Weight = 13.0;
		}

		public override bool CheckHold( Mobile m, Item item, bool message, bool checkItems, int plusItems, int plusWeight )
		{
			return base.CheckHold( m, item, false, checkItems, plusItems, plusWeight );
		}

		public override int DefaultMaxWeight{ get{ return 1600; } }

		public override bool CheckContentDisplay( Mobile from )
		{
			object root = this.RootParent;

			if ( root is BaseCreature && ((BaseCreature)root).Controlled && ((BaseCreature)root).ControlMaster == from )
				return true;

			return base.CheckContentDisplay( from );
		}

		public StrongBackpack( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			if ( version == 0 )
				Weight = 13.0;
		}
	}

	public class Backpack : BaseContainer
	{
		[Constructable]
		public Backpack() : base( 0xE75 )
		{
			Layer = Layer.Backpack;
			Weight = 3.0;
			Dyable = true;
		}

		public override bool DisplayDyable{ get{ return false; } }
		public override bool DisplayLootType{ get{ return !(Parent is Mobile); } }

		public override int DefaultMaxWeight {
			get {
				if ( Core.ML ) {
					Mobile m = ParentEntity as Mobile;
					if ( m != null && m.Player && m.Backpack == this ) {
						return 550;
					} else {
						return base.DefaultMaxWeight;
					}
				} else {
					return base.DefaultMaxWeight;
				}
			}
		}

		public Backpack( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 2 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			if ( version == 0 && ItemID == 0x9B2 )
				ItemID = 0xE75;

			if ( version < 2 )
				Dyable = true;
		}
	}

	public class Pouch : TrapableContainer
	{
		[Constructable]
		public Pouch() : base( 0xE79 )
		{
			Weight = 1.0;
		}

		public Pouch( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}

	public abstract class BaseBagBall : BaseContainer
	{
		public BaseBagBall( int itemID ) : base( itemID )
		{
			Weight = 1.0;
			Dyable = true;
		}

		public override bool DisplayDyable{ get{ return false; } }

		public BaseBagBall( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 1 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			if ( version == 0 )
				Dyable = true;
		}
	}

	public class SmallBagBall : BaseBagBall
	{
		[Constructable]
		public SmallBagBall() : base( 0x2256 )
		{
		}

		public SmallBagBall( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}

	public class LargeBagBall : BaseBagBall
	{
		[Constructable]
		public LargeBagBall() : base( 0x2257 )
		{
		}

		public LargeBagBall( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}

	public class Bag : BaseContainer
	{
		[Constructable]
		public Bag() : base( 0xE76 )
		{
			Weight = 2.0;
			Dyable = true;
		}

		public override bool DisplayDyable{ get{ return false; } }

		public Bag( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 1 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			if ( version == 0 )
				Dyable = true;
		}
	}

	public class Barrel : BaseFurnitureContainer
	{
		[Constructable]
		public Barrel() : base( 0xE77 )
		{
			Weight = 25.0;
		}

		public Barrel( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = ( InheritsBaseCont ? OldVersion : reader.ReadInt() ); //Required for BaseFurnitureContainer insertion

			if ( Weight == 0.0 )
				Weight = 25.0;
		}
	}

	public class Keg : BaseFurnitureContainer
	{
		[Constructable]
		public Keg() : base( 0xE7F )
		{
			Weight = 15.0;
		}

		public Keg( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = ( InheritsBaseCont ? OldVersion : reader.ReadInt() ); //Required for BaseFurnitureContainer insertion
		}
	}

	public class PicnicBasket : BaseContainer
	{
		[Constructable]
		public PicnicBasket() : base( 0xE7A )
		{
			Weight = 2.0; // Stratics doesn't know weight
		}

		public PicnicBasket( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}

	public class Basket : BaseContainer
	{
		[Constructable]
		public Basket() : base( 0x990 )
		{
			Weight = 1.0; // Stratics doesn't know weight
		}

		public Basket( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}

	[Furniture]
	[Flipable( 0x9AA, 0xE7D )]
	public class WoodenBox : LockableContainer
	{
		[Constructable]
		public WoodenBox() : base( 0xE7D )
		{
			Weight = 4.0;
			Dyable = true;
		}

		public override Type DyeType{ get{ return typeof(FurnitureDyeTub); } }
		public override bool DisplayDyable{ get{ return false; } }

		public WoodenBox( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 1 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			if ( version == 0 )
				Dyable = true;
		}
	}

	[Furniture]
	[Flipable( 0x2DF1, 0x2DF1 )]
	public class ElevenBox : LockableContainer
	{
		[Constructable]
		public ElevenBox() : base( 0x2DF1 )
		{
			Weight = 4.0;
			Dyable = true;
		}

		public override Type DyeType{ get{ return typeof(FurnitureDyeTub); } }
		public override bool DisplayDyable{ get{ return false; } }

		public ElevenBox( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}

	[Furniture]
	[Flipable( 0x9A9, 0xE7E )]
	public class SmallCrate : LockableContainer
	{
		[Constructable]
		public SmallCrate() : base( 0xE7E )
		{
			Weight = 1.0;
			Dyable = true;
		}

		public override Type DyeType{ get{ return typeof(FurnitureDyeTub); } }
		public override bool DisplayDyable{ get{ return false; } }

		public SmallCrate( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 1 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			if ( version == 0 )
				Dyable = true;

			if ( Weight == 4.0 )
				Weight = 2.0;
		}
	}

	[Furniture]
	[Flipable( 0xE3F, 0xE3E )]
	public class MediumCrate : LockableContainer
	{
		[Constructable]
		public MediumCrate() : base( 0xE3E )
		{
			Weight = 1.0;
			Dyable = true;
		}

		public override Type DyeType{ get{ return typeof(FurnitureDyeTub); } }
		public override bool DisplayDyable{ get{ return false; } }

		public MediumCrate( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 1 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			if ( version == 0 )
				Dyable = true;

			if ( Weight == 6.0 )
				Weight = 2.0;
		}
	}

	[Furniture]
	[Flipable( 0xE3D, 0xE3C )]
	public class LargeCrate : LockableContainer
	{
		[Constructable]
		public LargeCrate() : base( 0xE3C )
		{
			Weight = 1.0;
			Dyable = true;
		}

		public override Type DyeType{ get{ return typeof(FurnitureDyeTub); } }
		public override bool DisplayDyable{ get{ return false; } }

		public LargeCrate( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 1 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			if ( version == 0 )
				Dyable = true;

			if ( Weight == 8.0 )
				Weight = 1.0;
		}
	}

	[DynamicFliping]
	[Flipable( 0x9A8, 0xE80 )]
	public class MetalBox : LockableContainer
	{
		public override CraftResource DefaultResource{ get{ return CraftResource.Iron; } }

		[Constructable]
		public MetalBox() : base( 0x9A8 )
		{
		}

		public MetalBox( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			if ( version == 0 && Weight == 3 )
				Weight = -1;
		}
	}

	[DynamicFliping]
	[Flipable( 0x9AB, 0xE7C )]
	public class MetalChest : LockableContainer
	{
		public override CraftResource DefaultResource{ get{ return CraftResource.Iron; } }

		[Constructable]
		public MetalChest() : base( 0x9AB )
		{
		}

		public MetalChest( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			if ( version == 0 && Weight == 25 )
				Weight = -1;
		}
	}

	[DynamicFliping]
	[Flipable( 0xE41, 0xE40 )]
	public class MetalGoldenChest : LockableContainer
	{
		public override CraftResource DefaultResource{ get{ return CraftResource.Iron; } }

		[Constructable]
		public MetalGoldenChest() : base( 0xE41 )
		{
		}

		public MetalGoldenChest( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			if ( version == 0 && Weight == 25 )
				Weight = -1;
		}
	}

	[Furniture]
	[Flipable( 0xE43, 0xE42 )]
	public class WoodenChest : LockableContainer
	{
		[Constructable]
		public WoodenChest() : base( 0xE42 )
		{
			Weight = 1.0;
		}

		public WoodenChest( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			if ( Weight == 15.0 || Weight == 2.0 )
				Weight = 1.0;
		}
	}

	[Furniture]
	[Flipable( 0x280B, 0x280C )]
	public class PlainWoodenChest : LockableContainer
	{
		[Constructable]
		public PlainWoodenChest() : base( 0x280C )
		{
			Dyable = true;
		}

		public override Type DyeType{ get{ return typeof(FurnitureDyeTub); } }
		public override bool DisplayDyable{ get{ return false; } }

		public PlainWoodenChest( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			if ( version == 0 )
				Dyable = true;

			if ( version == 0 && Weight == 15 )
				Weight = -1;
		}
	}

	[Furniture]
	[Flipable( 0x280D, 0x280E )]
	public class OrnateWoodenChest : LockableContainer
	{
		[Constructable]
		public OrnateWoodenChest() : base( 0x280E )
		{
			Dyable = true;
		}

		public override Type DyeType{ get{ return typeof(FurnitureDyeTub); } }
		public override bool DisplayDyable{ get{ return false; } }

		public OrnateWoodenChest( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 2 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			if ( version < 2 )
				Dyable = true;

			if ( version == 0 && Weight == 15 )
				Weight = -1;
		}
	}

	[Furniture]
	[Flipable( 0x280F, 0x2810 )]
	public class GildedWoodenChest : LockableContainer
	{
		[Constructable]
		public GildedWoodenChest() : base( 0x2810 )
		{
			Dyable = true;
		}

		public override Type DyeType{ get{ return typeof(FurnitureDyeTub); } }
		public override bool DisplayDyable{ get{ return false; } }

		public GildedWoodenChest( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 2 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			if ( version < 2 )
				Dyable = true;

			if ( version == 0 && Weight == 15 )
				Weight = -1;
		}
	}

	[Furniture]
	[Flipable( 0x2811, 0x2812 )]
	public class WoodenFootLocker : LockableContainer
	{
		[Constructable]
		public WoodenFootLocker() : base( 0x2812 )
		{
			Dyable = true;
			GumpID = 0x10B;
		}

		public override Type DyeType{ get{ return typeof(FurnitureDyeTub); } }
		public override bool DisplayDyable{ get{ return false; } }

		public WoodenFootLocker( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 3 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			if ( version == 0 && Weight == 15 )
				Weight = -1;

			if ( version < 2 )
				GumpID = 0x10B;

			if ( version < 3 )
				Dyable = true;
		}
	}

	[Furniture]
	[Flipable( 0x2813, 0x2814 )]
	public class FinishedWoodenChest : LockableContainer
	{
		[Constructable]
		public FinishedWoodenChest() : base( 0x2814 )
		{
			Dyable = true;
		}

		public override Type DyeType{ get{ return typeof(FurnitureDyeTub); } }
		public override bool DisplayDyable{ get{ return false; } }

		public FinishedWoodenChest( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 2 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			if ( version == 0 && Weight == 15 )
				Weight = -1;

			if ( version < 2 )
				Dyable = true;
		}
	}
}