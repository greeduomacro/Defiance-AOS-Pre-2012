using System;
using System.Collections.Generic;
using Server.Items;

namespace Server.Exchange
{
	public static class Config
	{
		public const double SellGoodsSeizure = 0.025; //goods not sold within given timeframe, seller pays fine
		public const int BuyCosts = 150; //buyer pays transaction costs (gp) when adding a bid
		public static TimeSpan BidTime = TimeSpan.FromDays(5.0); //time after which the bid is removed if no transaction took place
		public static TimeSpan SellTime = TimeSpan.FromDays(5.0); //time after which the offer is removed if no transaction took place
		public static int HistoryCleanAmount = 500; //at which amount should the transactionhistory be cleaned
		public static TimeSpan HistoryCleanTime = TimeSpan.FromDays(5.0);//at which time should the transactionhistory be removed

		//position is referred to by the Categories table, any type can be added or removed.
		// watch out for a possible position change when adding/removing a type array
		//to solve this update the Categories table with the new posion as typesid
		public static ExchangeTypeInfo[][] NumeratedTypes = new ExchangeTypeInfo[][]
		  {
				new ExchangeTypeInfo[] {new ExchangeTypeInfo(typeof(IronIngot), "Iron Ingot"), new ExchangeTypeInfo(typeof(DullCopperIngot), "Dull Copper Ingot"), new ExchangeTypeInfo(typeof(ShadowIronIngot), "Shadow Iron Ingot"), new ExchangeTypeInfo(typeof(CopperIngot), "Copper Ingot"), new ExchangeTypeInfo(typeof(BronzeIngot), "Bronze Ingot"), new ExchangeTypeInfo(typeof(GoldIngot), "Gold Ingot"), new ExchangeTypeInfo(typeof(AgapiteIngot), "Agapite Ingot"), new ExchangeTypeInfo(typeof(VeriteIngot), "Verite Ingot"), new ExchangeTypeInfo(typeof(ValoriteIngot), "Valorite Ingot"),new ExchangeTypeInfo(typeof(RedScales), "Red Scales"),new ExchangeTypeInfo(typeof(YellowScales), "Yellow Scales"),new ExchangeTypeInfo(typeof(BlackScales), "Black Scales"),new ExchangeTypeInfo(typeof(GreenScales), "Green Scales"),new ExchangeTypeInfo(typeof(WhiteScales), "White Scales"),new ExchangeTypeInfo(typeof(BlueScales), "Blue Scales")},
				new ExchangeTypeInfo[] {new ExchangeTypeInfo(typeof(SulfurousAsh), "Sulfurous Ash"), new ExchangeTypeInfo(typeof(SpidersSilk), "Spider Silk"),new ExchangeTypeInfo(typeof(PigIron), "Pig Iron"),new ExchangeTypeInfo(typeof(NoxCrystal), "Nox Crystal"), new ExchangeTypeInfo(typeof(Nightshade), "Night Shade"), new ExchangeTypeInfo(typeof(MandrakeRoot), "Mandrake Root"), new ExchangeTypeInfo(typeof(GraveDust), "Grave Dust"),new ExchangeTypeInfo(typeof(Ginseng), "Ginseng"),new ExchangeTypeInfo(typeof(Garlic), "Garlic"), new ExchangeTypeInfo(typeof(DaemonBlood), "Demon Blood"), new ExchangeTypeInfo(typeof(Bloodmoss), "Bloodmoss"), new ExchangeTypeInfo(typeof(BlackPearl), "Black Pearl"), new ExchangeTypeInfo(typeof(BatWing), "Bat Wing")},
				new ExchangeTypeInfo[] {new ExchangeTypeInfo(typeof(UncutCloth), "Uncut Cloth"),new ExchangeTypeInfo(typeof(Leather), "Leather"), new ExchangeTypeInfo(typeof(SpinedLeather), "Spined Leather"),new ExchangeTypeInfo(typeof(HornedLeather), "Horned Leather"),new ExchangeTypeInfo(typeof(BarbedLeather), "Barbed Leather"), new ExchangeTypeInfo(typeof(Cloth), "Cloth"), new ExchangeTypeInfo(typeof(Bone), "Bones"), new ExchangeTypeInfo(typeof(BoltOfCloth), "Bolt of Cloth")},
				new ExchangeTypeInfo[] {new ExchangeTypeInfo(typeof(Shaft), "Shaft"), new ExchangeTypeInfo(typeof(Feather), "Feather"), new ExchangeTypeInfo(typeof(Board), "Board"),new ExchangeTypeInfo(typeof(Sand), "Sand")},
				new ExchangeTypeInfo[] {new ExchangeTypeInfo(typeof(Arrow), "Arrow"), new ExchangeTypeInfo(typeof(Bolt), "Bolt")}
		  };

		//id has to be one of a kind, when replacing a category do not reuse the id
		//name is just a name you may call it anything you wish to
		//typesid refers to the NumeratedTypes table above, it has to be within the bounds of the NumeratedTypes table
		public static ExchangeCategory[] Categories = new ExchangeCategory[]
		   {
				new ExchangeCategory(0,"BlackSmith Resources",0),
				new ExchangeCategory(1,"Magic Resources",1),
				new ExchangeCategory(2,"Tailor Resources",2),
				new ExchangeCategory(3,"Other Craft Resources",3),
				new ExchangeCategory(4,"Archery Related Resources",4),
		   };

		//Create an enum for each category, so you can change them on the npc's
		public enum NPCCategories
		{
			BlackSmith = 0,
			Magic = 1,
			Tailor = 2,
			OtherCrafts = 3,
			Archery = 4
		}
	}
}