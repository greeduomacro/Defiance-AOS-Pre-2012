using System;
using System.Collections.Generic;
using Server.Items;
using Server.Commands;
using Server.Mobiles;
using System.IO;

namespace Server.Engines.RewardSystem
{
	public static class LLVendorItems
	{
		public static List<ItemInfo> MageRegsList = new List<ItemInfo>();
		public static List<ItemInfo> NecroRegsList = new List<ItemInfo>();
		public static List<ItemInfo> PotionsList = new List<ItemInfo>();
		public static List<ItemInfo> BandsList = new List<ItemInfo>();
		public static List<ItemInfo> ArrowsList = new List<ItemInfo>();
		public static List<ItemInfo> WeapsList = new List<ItemInfo>();
		public static List<ItemInfo> ArmorsList = new List<ItemInfo>();
		public static List<ItemInfo> MiscList = new List<ItemInfo>();

		public static List<ItemInfo> GetList(int link)
		{
			switch (link)
			{
				default:
				case 1: return MageRegsList;
				case 2: return NecroRegsList;
				case 3: return PotionsList;
				case 4: return BandsList;
				case 5: return ArrowsList;
				case 6: return WeapsList;
				case 7: return ArmorsList;
				case 8: return MiscList;
			}
		}

		public static void CreateItem(ItemInfo info, Mobile m)
		{
			if (m == null)
				return;

			Item item = null;
			BaseCreature bc = null;
			switch (info.IncrID)
			{
				case 0: item = new BlackPearl(999); break;
				case 1: item = new Bloodmoss(999); break;
				case 2: item = new MandrakeRoot(999); break;
				case 3: item = new Garlic(999); break;
				case 4: item = new Ginseng(999); break;
				case 5: item = new Nightshade(999); break;
				case 6: item = new SpidersSilk(999); break;
				case 7: item = new SulfurousAsh(999); break;
				case 8: item = new Bag();
						((Bag)item).DropItem( new BlackPearl(100) );
						((Bag)item).DropItem( new Bloodmoss(100) );
						((Bag)item).DropItem( new MandrakeRoot(100) );
						((Bag)item).DropItem( new Garlic(100) );
						((Bag)item).DropItem( new Ginseng(100) );
						((Bag)item).DropItem( new Nightshade(100) );
						((Bag)item).DropItem( new SpidersSilk(100) );
						((Bag)item).DropItem( new SulfurousAsh(100) );
						break;
				case 9: item = new BatWing(999); break;
				case 10: item = new GraveDust(999); break;
				case 11: item = new DaemonBlood(999); break;
				case 12: item = new NoxCrystal(999); break;
				case 13: item = new PigIron(999); break;
				case 14: item = new Bag();
						((Bag)item).DropItem( new BatWing(100) );
						((Bag)item).DropItem( new GraveDust(100) );
						((Bag)item).DropItem( new DaemonBlood(100) );
						((Bag)item).DropItem( new NoxCrystal(100) );
						((Bag)item).DropItem( new PigIron(100) );
						break;
				case 15: for (int i=0; i < 2; i++) //The last potion is dropped later
						{
							item = new RefreshPotion();
							m.AddToBackpack(item);
						}
						item = new RefreshPotion();
						break;
				case 16: for (int i=0; i < 2; i++) //The last potion is dropped later
						{
							item = new RefreshPotion();
							m.AddToBackpack(item);
						}
						item = new LesserCurePotion();
						break;
				case 17: for (int i=0; i < 2; i++) //The last potion is dropped later
						{
							item = new RefreshPotion();
							m.AddToBackpack(item);
						}
						item = new LesserHealPotion();
						break;
				case 18: for (int i=0; i < 2; i++) //The last potion is dropped later
						{
							item = new RefreshPotion();
							m.AddToBackpack(item);
						}
						item = new NightSightPotion();
						break;
				case 19: item = new Bandage(50); break;
				case 20: item = new Bandage(200); break;
				case 21: item = new Bandage(1000); break;
				case 22: item = new Arrow(100); break;
				case 23: item = new Arrow(1000); break;
				case 24: item = new Bolt(100); break;
				case 25: item = new Bolt(1000); break;
				case 26: bc = new Horse(); break;
				case 27: bc = new PackLlama(); break;
				case 28: item = new Club();
						BaseRunicTool.ApplyAttributesTo((BaseWeapon)item, Utility.RandomMinMax(1, 4), 40, 80);
						break;
				case 29: item = new WarFork();
						BaseRunicTool.ApplyAttributesTo((BaseWeapon)item, Utility.RandomMinMax(1, 4), 40, 80);
						break;
				case 30: item = new Katana();
						BaseRunicTool.ApplyAttributesTo((BaseWeapon)item, Utility.RandomMinMax(1, 4), 40, 80);
						break;
				case 31: item = new Bow();
						BaseRunicTool.ApplyAttributesTo((BaseWeapon)item, Utility.RandomMinMax(1, 4), 40, 80);
						break;
				case 32: item = new MetalKiteShield();
						BaseRunicTool.ApplyAttributesTo((BaseShield)item, Utility.RandomMinMax(1, 4), 40, 80);
						break;
				case 33: item = new WoodenShield();
						BaseRunicTool.ApplyAttributesTo((BaseShield)item, Utility.RandomMinMax(1, 4), 40, 80);
						break;
				case 34: item = new LeatherChest();
						BaseRunicTool.ApplyAttributesTo((BaseArmor)item, Utility.RandomMinMax(1, 4), 40, 80);
						break;
				case 35: item = new LeatherGloves();
						BaseRunicTool.ApplyAttributesTo((BaseArmor)item, Utility.RandomMinMax(1, 4), 40, 80);
						break;
				case 36: item = new LeatherGorget();
						BaseRunicTool.ApplyAttributesTo((BaseArmor)item, Utility.RandomMinMax(1, 4), 40, 80);
						break;
				case 37: item = new LeatherLegs();
						BaseRunicTool.ApplyAttributesTo((BaseArmor)item, Utility.RandomMinMax(1, 4), 40, 80);
						break;
				case 38: item = new LeatherCap();
						BaseRunicTool.ApplyAttributesTo((BaseArmor)item, Utility.RandomMinMax(1, 4), 40, 80);
						break;
				case 39: item = new LeatherArms();
						BaseRunicTool.ApplyAttributesTo((BaseArmor)item, Utility.RandomMinMax(1, 4), 40, 80);
						break;
				case 40: item = new Gold(300); break;
				case 41: item = new Gold(3000); break;
				case 42: item = new Server.Engines.BulkOrders.BulkOrderBook(); break;
				case 43: item = new Drums();
						((BaseInstrument)item).Quality = InstrumentQuality.Exceptional;
						break;
				case 44: item = new TambourineTassel();
						((BaseInstrument)item).Quality = InstrumentQuality.Exceptional;
						break;
				case 45: item = new Server.Multis.SmallDragonBoatDeed(); break;
				case 46: item = new Server.Multis.LargeDragonBoatDeed(); break;
			}

			if (item != null)
			{
				m.AddToBackpack(item);
				m.SendMessage("Here are the goods you requested.");
			}
			else if (bc != null)
			{
				bc.Controlled = true;
				bc.ControlMaster = m;
				bc.MoveToWorld( m.Location, m.Map );
				m.SendMessage("Here is the pet you requested.");
			}
			else
			{
				m.SendMessage("That item is not available. Please report the bug at the site that will open in your browser.");
				m.LaunchBrowser( "http://bug.casiopia.net/" );
				m.AddToBackpack( new VeriteGem(info.Price) );
			}
		}
	}

	public enum ItemType
	{
		MageRegs = 1,
		NecroRegs = 2,
		Potions = 3,
		Bands = 4,
		Arrows = 5,
		Weaps = 6,
		Armors = 7,
		Misc = 8
	}

	public struct ItemInfo
	{
		public ItemType IType;
		public string Name;
		public int ItemID;
		public int Price;
		public int IncrID;

		public ItemInfo(ItemType itype, string name, int itemid, int price, int incrid)
		{
			IType = itype;
			Name = name;
			ItemID = itemid;
			Price = price;
			IncrID = incrid;

			LLVendorItems.GetList((int)itype).Add(this);
		}
	}

	public static class AddLLItemInfo
	{
		public static void Initialize()
		{
			new ItemInfo (	ItemType.MageRegs,
				"999 Black Pearl", 3962,	//Name, ItemID
				5000,						//Price in GOLD. Do not change prices here, update the formula in LLItemGump.CalculatePrice(int goldPrice) if needed.
				0							//Index
			);
			new ItemInfo (	ItemType.MageRegs,
				"999 Blood Moss", 3963,
				5000, 1
			);
			new ItemInfo (	ItemType.MageRegs,
				"999 Mandrake Root", 3974,
				3000, 2
			);
			new ItemInfo (	ItemType.MageRegs,
				"999 Garlic", 3972,
				3000, 3
			);
			new ItemInfo (	ItemType.MageRegs,
				"999 Ginseng", 3973,
				3000, 4
			);
			new ItemInfo (	ItemType.MageRegs,
				"999 Nightshade", 3976,
				3000, 5
			);
			new ItemInfo (	ItemType.MageRegs,
				"999 Spider's Silk", 3981,
				3000, 6
			);
			new ItemInfo (	ItemType.MageRegs,
				"999 Sulfurous Ash", 3980,
				3000, 7
			);
			new ItemInfo (	ItemType.MageRegs,
				"Bag of reagents (100)", 3702,
				2800, 8
			);

			new ItemInfo (	ItemType.NecroRegs,
				"999 Batwing", 3960,
				3000, 9
			);
			new ItemInfo (	ItemType.NecroRegs,
				"999 Grave Dust", 3983,
				3000, 10
			);
			new ItemInfo (	ItemType.NecroRegs,
				"999 Daemon Blood", 3965,
				6000, 11
			);
			new ItemInfo (	ItemType.NecroRegs,
				"999 Nox Crystal", 3982,
				6000, 12
			);
			new ItemInfo (	ItemType.NecroRegs,
				"999 Pig Iron", 3978,
				5000, 13
			);
			new ItemInfo (	ItemType.NecroRegs,
				"Bag of reagents (100)", 3702,
				2250, 14
			);
			new ItemInfo (	ItemType.Potions,
				"3 Refresh Potions", 3851,
				45, 15
			);
			new ItemInfo (	ItemType.Potions,
				"3 Lesser Cure Potions", 3847,
				45, 16
			);
			new ItemInfo (	ItemType.Potions,
				"3 Lesser Heal Potions", 3852,
				45, 17
			);
			new ItemInfo (	ItemType.Potions,
				"3 NightSight Potions", 3846,
				45, 18
			);
			new ItemInfo (	ItemType.Bands,
				"50 Clean Bandages", 3617,
				250, 19
			);
			new ItemInfo (	ItemType.Bands,
				"200 Clean Bandages", 3617,
				1000, 20
			);
			new ItemInfo (	ItemType.Bands,
				"1000 Clean Bandages", 3617,
				5000, 21
			);
			new ItemInfo (	ItemType.Arrows,
				"100 Arrows", 3903,
				300, 22
			);
			new ItemInfo (	ItemType.Arrows,
				"1000 Arrows", 3903,
				3000, 23
			);
			new ItemInfo (	ItemType.Arrows,
				"100 Crossbow Bolts", 7163,
				600, 24
			);
			new ItemInfo (	ItemType.Arrows,
				"1000 Crossbow Bolts", 7163,
				6000, 25
			);
			new ItemInfo (	ItemType.Misc,
				"A Horse", 8484,
				1100, 26
			);
			new ItemInfo (	ItemType.Misc,
				"A Pack Llama", 8487,
				1130, 27
			);
			new ItemInfo (	ItemType.Weaps,
				"Club", 5044,
				250, 28
			);
			new ItemInfo (	ItemType.Weaps,
				"War Fork", 5125,
				250, 29
			);
			new ItemInfo (	ItemType.Weaps,
				"Katana", 5119,
				250, 30
			);
			new ItemInfo (	ItemType.Weaps,
				"Bow", 5042,
				250, 31
			);
			new ItemInfo (	ItemType.Armors,
				"Metal Kite Shield", 7028,
				700, 32
			);
			new ItemInfo (	ItemType.Armors,
				"Wooden Shield", 7034,
				250, 33
			);
			new ItemInfo (	ItemType.Armors,
				"Leather Tunic", 5068,
				605, 34
			);
			new ItemInfo (	ItemType.Armors,
				"Leather Gloves", 5062,
				400, 35
			);
			new ItemInfo (	ItemType.Armors,
				"Leather Gorget", 5063,
				470, 36
			);
			new ItemInfo (	ItemType.Armors,
				"Leather Leggings", 5067,
				500, 37
			);
			new ItemInfo (	ItemType.Armors,
				"Leather Cap", 7609,
				150, 38
			);
			new ItemInfo (	ItemType.Armors,
				"Leather Sleeves", 5069,
				500, 39
			);
			new ItemInfo (	ItemType.Misc,
				"300 Gold coins", 3823,
				100, 40
			);
			new ItemInfo (	ItemType.Misc,
				"3000 Gold coins", 3823,
				1000, 41
			);
			new ItemInfo (	ItemType.Misc,
				"Bulk Order Book", 8793,
				6000, 42
			);
			new ItemInfo (	ItemType.Misc,
				"Drums", 3740,
				21, 43
			);
			new ItemInfo (	ItemType.Misc,
				"Exceptional Quality Tambourine", 3742,
				5000, 44
			);
			new ItemInfo (	ItemType.Misc,
				"Small Dragon Ship", 5364,
				10177, 45
			);
			new ItemInfo (	ItemType.Misc,
				"Large Dragon Ship", 5364,
				12927, 46
			);
		}
	}
}