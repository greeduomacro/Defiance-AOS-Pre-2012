using System;
using System.Collections;
using Server;
using Server.Mobiles;
using Server.ContextMenus;
using Server.Network;
using Server.Gumps;
using Server.Engines.Quests.CursedCave;

namespace Server.Items
{
	public class CursedCaveUtility
	{
		public static Item MutateItem( Item item, int precentValue )
		{
			return LootPackEntry.Mutate(item, Utility.RandomMinMax(1, 5), 20, 100, precentValue * 100 );
		}

		public class WeaponTalkEntry : ContextMenuEntry
		{
			private BaseWeapon m_Weapon;
			private Mobile m_from;

			public WeaponTalkEntry(Mobile from, BaseWeapon weapon)
				: base(6146)
			{
				m_Weapon = weapon;
				m_from = from;
			}

			public override void OnClick()
			{
				string sSpeakString = "";
				switch (Utility.Random(6))
				{
					case 0: sSpeakString = string.Format("I will serve thee well master {0}.", m_from.Name); break;
					case 1: sSpeakString = string.Format("Lets go kill something master {0}!", m_from.Name); break;
					case 2: sSpeakString = string.Format("Lets slay some cursed creatures master {0}!", m_from.Name); break;
					case 3: sSpeakString = string.Format("Nice day today master {0}.", m_from.Name); break;
					case 4: sSpeakString = string.Format("Im bored."); break;
					case 5: sSpeakString = string.Format("Lets go to the cursed cave."); break;
				}
				MessageHelper.SendLocalizedMessageTo(m_Weapon, m_from, 1070722, sSpeakString, 0x3B2);
			}
		}

		public static void WeaponSpeak(Mobile attacker, Mobile defender, string sWeaponName)
		{
			string sSpeakString = "";

			if (defender is BaseCreature)
			{
				switch (Utility.Random(8))
				{
					case 0: sSpeakString = "I never liked these creatures!"; break;
					case 1: sSpeakString = "Slay all of them!"; break;
					case 2: sSpeakString = "Take that foul creature!"; break;
					case 3: sSpeakString = "I will slay you all!"; break;
					case 4: sSpeakString = "Die foul creature!"; break;
					case 5: sSpeakString = string.Format("Lets kill them all master {0}!", attacker.Name); break;
					case 6: sSpeakString = "Die die die!"; break;
					case 7: sSpeakString = string.Format("Nice hit master {0}!", attacker.Name); break;
				}
			}
			else if (defender is PlayerMobile)
			{
				switch (Utility.Random(4))
				{
					case 0: sSpeakString = string.Format("My master {0} will kill you foul human!", attacker.Name); break;
					case 1: sSpeakString = "Die human scum!"; break;
					case 2: sSpeakString = "Die die die!"; break;
					case 3: sSpeakString = string.Format("Nice hit master {0}!", attacker.Name); break;
				}
			}
			else
				sSpeakString = "Take that!";

			PrivateMessage(attacker, string.Format("{0}: {1}", sWeaponName, sSpeakString));
		}

		public enum CCTriggerCheck { All, None, LongswordOfJustice, HarvesterOfTheGhost, BowOfHephaestus }
		public static bool HasTalkingWeapon(Mobile from, out string weaponName, CCTriggerCheck type)
		{
			weaponName = "";

			if (from.Weapon is HarvesterOfTheGhost && (type == CCTriggerCheck.HarvesterOfTheGhost || type == CCTriggerCheck.All))
			{
				weaponName = "Harvester Of The Ghost";
				return true;
			}
			else if (from.Weapon is LongswordOfJustice && (type == CCTriggerCheck.LongswordOfJustice || type == CCTriggerCheck.All))
			{
				weaponName = "Longsword Of Justice";
				return true;
			}
			else if (from.Weapon is BowOfHephaestus && (type == CCTriggerCheck.BowOfHephaestus || type == CCTriggerCheck.All))
			{
				weaponName = "Bow Of Hephaestus";
				return true;
			}
			return false;
		}

		public static void PrivateMessage(Mobile from, string message)
		{
			from.PrivateOverheadMessage(MessageType.Regular, 0x3B2, false, message, from.NetState);
		}

		public static CCRareEntry[] CCRares = new CCRareEntry[]
		{
			new CCRareEntry(typeof( SRCRBrokenChestOfDrawers ), "Broken Chest Of Drawers"),
			new CCRareEntry(typeof( SRCREmptyToolKit ), "Empty ToolKit"),
			new CCRareEntry(typeof( SRCRBucket ), "Bucket"),
			new CCRareEntry(typeof( SRCRSmallWeb ), "Small Web"),
			new CCRareEntry(typeof( SRCRPluckedChicken ), "Plucked Chicken"),
			new CCRareEntry(typeof( SRCRBooks ), "Books"),
			new CCRareEntry(typeof( SRCRSturdySmithHammer ), "Sturdy Smith Hammer"),
			new CCRareEntry(typeof( SRCRSturdySaw ), "Sturdy Saw"),
			new CCRareEntry(typeof( SRCRSturdyProspectorsTool ), "Sturdy Prospectors Tool"),
			new CCRareEntry(typeof( SRCREmptyTub ), "Empty Tub"),
			new CCRareEntry(typeof( SRCRSturdyMortarPestle ), "Sturdy Mortar Pestle"),
			new CCRareEntry(typeof( SRCRWallTorchEast ), "Wall Torch"),
			new CCRareEntry(typeof( SRCRWallTorchSouth ), "Wall Torch")
		};

		public class CCRareEntry
		{
			private Type m_Type;
			public Type Type
			{
				get { return m_Type; }
				set { m_Type = value; }
			}

			private string m_Name;
			public string Name
			{
				get { return m_Name; }
				set { m_Name = value; }
			}

			public CCRareEntry(Type type, string name)
			{
				m_Type = type;
				m_Name = name;
			}
		}

		public static ArrayList GetCCRares(PlayerMobile player, int maxNum)
		{
			ArrayList foundItems = new ArrayList();
			foreach (Item item in player.Backpack.Items)
			{
				if (CursedCaveUtility.isCCRare(item))
					foundItems.Add(item);

				if (foundItems.Count >= maxNum)
					break;
			}
			return foundItems;
		}

		public static bool isCCRare(Item item)
		{
			for (int i = 0; i < CursedCaveUtility.CCRares.Length; i++)
				if (((CCRareEntry)CursedCaveUtility.CCRares[i]).Type == item.GetType())
					return true;

			return false;
		}

		public static string GetCCRareName(Item item)
		{
			return GetCCRareName(item.GetType());
		}

		public static string GetCCRareName(Type type)
		{
			for (int i = 0; i < CursedCaveUtility.CCRares.Length; i++)
				if (((CCRareEntry)CursedCaveUtility.CCRares[i]).Type == type)
					return ((CCRareEntry)CursedCaveUtility.CCRares[i]).Name;

			return string.Empty;
		}

		public class CCRaresGump : Gump
		{
			private ArrayList m_foundItems;
			private WarrenTheGuard m_npc;
			Mobile m_owner;

			public CCRaresGump(ArrayList foundItems, WarrenTheGuard npc, Mobile owner)
				: base(50, 50)
			{
				owner.CloseGump(typeof(CCRaresGump));
				m_foundItems = foundItems;
				m_npc = npc;
				m_owner = owner;

				string sRareList = string.Empty;
				foreach (Item item in foundItems)
					sRareList += CursedCaveUtility.GetCCRareName(item) + "<br>";

				this.Closable = true;
				this.Disposable = true;
				this.Dragable = true;
				this.Resizable = false;
				this.AddPage(0);
				this.AddBackground(0, 0, 467, 234, 9200);
				this.AddLabel(148, 6, 0, @"Lost Treasures Of Cursed Cave");
				this.AddHtml(15, 35, 434, 137, string.Format("I see that you have enough cursed cave rares in your backpack to earn a reward. Do you wish to turn in the following {0} rares to recive a reward and maybe even a special artifact?<br><br>Rares Found<br>{1}", m_npc.ArtifactAmount, sRareList), (bool)true, (bool)true);
				this.AddButton(161, 190, 247, 248, (int)Buttons.B_OK, GumpButtonType.Reply, 0);
				this.AddButton(239, 190, 242, 241, (int)Buttons.B_CANCEL, GumpButtonType.Reply, 0);
			}

			public enum Buttons
			{
				B_CANCEL,
				B_OK,
			}

			public override void OnResponse(Server.Network.NetState sender, RelayInfo info)
			{
				Mobile from = sender.Mobile;

				switch (info.ButtonID)
				{
					case (int)Buttons.B_OK:
						int counter = 0;
						foreach (Item item in m_foundItems)
							if (item.IsChildOf(from.Backpack))
								counter++;

						if (counter >= m_npc.ArtifactAmount)
						{
							Bag rewardBag = new Bag();
							rewardBag.Hue = Utility.RandomDyedHue();
							LootPackEntry.AddRandomLoot(rewardBag, 5, 600,  5, 5, 50, 100);
							rewardBag.DropItem(new Bandage(Utility.RandomMinMax(200, 300)));
							rewardBag.DropItem(new Gold(2000, 4000));
							rewardBag.DropItem(new TreasureMap(Utility.RandomMinMax(2, 6), Map.Felucca));

							// Add Artifact ***
							if (0.40 > Utility.RandomDouble())
								rewardBag.DropItem(new HarvesterOfTheGhost());

							if (from.PlaceInBackpack(rewardBag))
							{
								foreach (Item item in m_foundItems)
									item.Delete();
								m_npc.Say(1070984, "Reward Bag"); // You have earned the gratitude of the Empire. I have placed the ~1_OBJTYPE~ in your backpack.
							}
							else
							{
								rewardBag.Delete();
								from.SendLocalizedMessage(1046260); // You need to clear some space in your inventory to continue with the quest.  Come back here when you have more space in your inventory.
							}
						}
						break;
				}
			}
		}
	}
}