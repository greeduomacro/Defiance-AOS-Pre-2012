using System;
using Server;
using Server.Items;

namespace Server.Misc
{
	public class WinterGiftGiver2007 : GiftGiver
	{
		public static void Initialize()
		{
			GiftGiving.Register( new WinterGiftGiver2007() );
		}

		public override DateTime Start{ get{ return new DateTime( 2007, 12, 24 ); } }
		public override DateTime Finish{ get{ return new DateTime( 2008, 1, 1 ); } }

		public override void GiveGift( Mobile mob )
		{
			Item item = null;
			GiftBox box = new GiftBox();
			box.Name = "Merry Christmas and a Happy New Year! December, 2007";

			box.DropItem( new WreathDeed());
			box.DropItem( new SnowPile());
			box.DropItem( new SnowGlobe());

			int random = Utility.Random( 100 );
			if ( random < 20 ) {
				item = new MistletoeDeed();
				((MistletoeDeed)item).Label = "December 2007";
				box.DropItem( item );
			}
			else if ( random < 25 ) {
				item = new FurBoots();
				item.Hue = 1150;
				item.LootType = LootType.Blessed;
				item.Name = "Warm Fur Boots";
				((BaseClothing)item).CustomPropName = "Winter 2007";
				box.DropItem( item );
			}
			else if ( random < 40 ) {
				item = new HolidayTreeDeed();
				box.DropItem( item );
			}
			else if ( random < 60 ) {
				item = new LightOfTheWinterSolstice();
				((LightOfTheWinterSolstice)item).Label = "December 2007";
				box.DropItem( item );
			}
			else if ( random < 80 ) {
				item = new HolidayBell();
				box.DropItem( item );
			}
			else {
				int rnd = Utility.Random(2);
				switch(rnd) {
					case 0: item = new BlueSnowflake(); break;
					case 1: item = new WhiteSnowflake(); break;
				}
				rnd = Utility.Random(7);
				switch(rnd) {
					case 0: item.Hue = 1150; break;
					case 1: item.Hue = 1151; break;
					case 2: item.Hue = 1153; break;
					case 3: item.Hue = 1154; break;
					case 4: item.Hue = 1165; break;
					case 5: item.Hue = 1167; break;
					default: break;
				}

				item.Name = "Snowflake, December 2007";
				box.DropItem( item );
			}


			switch ( GiveGift( mob, box ) )
			{
				case GiftResult.Backpack:
					mob.SendMessage( 0x482, "Happy Holidays from the team!  Gift items have been placed in your backpack." );
					break;
				case GiftResult.BankBox:
					mob.SendMessage( 0x482, "Happy Holidays from the team!  Gift items have been placed in your bank box." );
					break;
			}
		}
	}
}