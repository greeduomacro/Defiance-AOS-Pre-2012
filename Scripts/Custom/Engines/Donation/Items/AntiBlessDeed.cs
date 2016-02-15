using System;
using Server.Network;
using Server.Prompts;
using Server.Items;
using Server.Targeting;

namespace Server.Items
{
	public class AntiBlessTarget : Target // Create our targeting class (which we derive from the base target class)
	{
		private AntiBlessDeed m_Deed;

		public AntiBlessTarget( AntiBlessDeed deed ) : base( 1, false, TargetFlags.None )
		{
						 m_Deed = deed;
		}

		protected override void OnTarget( Mobile from, object target ) // Override the protected OnTarget() for our feature
		{
			if ( m_Deed == null || !m_Deed.IsChildOf( from.Backpack ) )
				from.SendLocalizedMessage( 1042001 );
			else if ( target is BaseClothing )
			{
				Item item = (Item)target;

				if ( item.LootType == LootType.Regular || item.BlessedFor != null || (Mobile.InsuranceEnabled && item.Insured) ) // Check if its already newbied (blessed)
					from.SendMessage( "This item cannot have its bless status removed" ); // That item is already blessed
				else if ( item.LootType != LootType.Blessed || !item.Dyable || item is HoodedShroudOfShadows )
					from.SendLocalizedMessage( 1045114 ); // You can not bless that item
				else
				{
					if( item.RootParent != from ) // Make sure its in their pack or they are wearing it
						from.SendLocalizedMessage( 1045158 ); // You must have the item in your backpack to target it.
					else
					{
						item.LootType = LootType.Regular;
						from.SendLocalizedMessage( 1075289 ); // Your item has been unblessed

						m_Deed.Delete(); // Delete the bless deed
					}
				}
			}
			else
				from.SendLocalizedMessage( 500509 ); // You cannot bless that object
		}
	}

	public class AntiBlessDeed : Item // Create the item class which is derived from the base item class
	{
		[Constructable]
		public AntiBlessDeed() : base( 0x14F0 )
		{
			Weight = 1.0;
			Name = "an Anti Bless Deed";
			LootType = LootType.Blessed;
				}

		public AntiBlessDeed( Serial serial ) : base( serial )
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
			LootType = LootType.Blessed;

			int version = reader.ReadInt();
		}

		public override bool DisplayLootType{ get{ return false; } }

		public override void OnDoubleClick( Mobile from ) // Override double click of the deed to call our target
		{
			if ( !IsChildOf( from.Backpack ) ) // Make sure its in their pack
			{
				 from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
			}
			else
			{
				from.SendLocalizedMessage( 1005018 ); // What item would you like to bless? (Clothes Only)
				from.Target = new AntiBlessTarget( this ); // Call our target
			 }
		}
	}
}