using System;
using System.Collections.Generic;
using Server.Items;
using Server.Commands;
using System.IO;

namespace Server.Engines.RewardSystem
{
	public static class TemporaryItemSystem
	{
		private static List<ITempItem> m_Enlisted = new List<ITempItem>();
		private static DateTime m_Now = DateTime.Now;

		public static void Initialize()
		{
			new InternalTimer().Start();
		}

		public static void Verify(ITempItem item)
		{
			if(!m_Enlisted.Contains(item))
				m_Enlisted.Add(item);

			int daysleft = (item.RemovalTime - m_Now).Days;

			if (daysleft < 0)
				Timer.DelayCall(TimeSpan.Zero, new TimerStateCallback(Remove_OnCallback), item);

			else
			{
				item.PropertyString = "Deletion " + (daysleft > 1 ? String.Format("in {0} days", daysleft) : daysleft == 0 ? "today": "tomorrow");
				if (item is Item)
					((Item)item).InvalidateProperties();
			}
		}

		private static void Remove_OnCallback(object o)
		{
			Item item = o as Item;
			ITempItem temp = o as ITempItem;

			if (item != null && !item.Deleted)
			{
				item.Delete();
				if(temp != null)
					m_Enlisted.Remove(temp);
			}
		}

		public static void VerifyAll()
		{
			m_Now = DateTime.Now;
			foreach (ITempItem iti in m_Enlisted)
				Verify(iti);
		}

		private class InternalTimer : Timer
		{
			public InternalTimer()
				: base(TimeSpan.FromHours(0.1), TimeSpan.FromHours(12.0))
			{
			}

			protected override void OnTick()
			{
				TemporaryItemSystem.VerifyAll();
			}
		}
	}

	public static class EventRewardSystem
	{
		public const int MaxCopper = 25;
		public static Dictionary<Mobile, int> CopperHandoutDictionary = new Dictionary<Mobile, int>();
		public static List<EventRewardInfo> PlayItemList = new List<EventRewardInfo>();
		public static List<EventRewardInfo> DecoList = new List<EventRewardInfo>();
		public static List<EventRewardInfo> TrammeliteList = new List<EventRewardInfo>();

		public static List<EventRewardInfo> GetList(int link)
		{
			switch (link)
			{
				default:
				case 1: return PlayItemList;
				case 2: return DecoList;
				case 3: return TrammeliteList;
			}
		}

		public static void Initialize()
		{
			CommandSystem.Register("CreateCopperBars", AccessLevel.GameMaster, new CommandEventHandler(CreateCopperBars_OnCommand));
		}

		private static void CreateCopperBars_OnCommand(CommandEventArgs e)
		{
			e.Mobile.SendGump(new RequestCopperBarsGump(e.Mobile));
		}

		public static void CreateCopperBar(string receiver, Container cont, int amount, string reason)
		{
			if (cont != null)
			{
				cont.AddItem(new CopperBar(amount));

				StreamWriter writer = new StreamWriter("Logs\\CopperBarLogging.log", true);
				string output = String.Format( @"
{4} - System Created Copper Bars
Name: {0}
Amount: {1}
Serial: {2}
Further Explanation: {3}
",
				receiver, amount.ToString(), cont.Serial.ToString(), reason, DateTime.Now.ToString( "MM/dd/yyyy" ) );
				writer.Write(output);
				writer.Close();
			}
		}

		public static void CreateReward(EventRewardInfo info, Mobile m)
		{
			if (m == null)
				return;

			Item item = null;
			switch (info.IncrID)
			{
				case 0: item = new MagicSewingKit(1); break;
				case 1:
					string str = "PottedCactus";
					int randint = Utility.Random(6);
					if (randint > 0)
						str = str + randint.ToString();

					Type type = ScriptCompiler.FindTypeByName(str);
					item = (Item)Activator.CreateInstance(type);
					item.Weight = 100;
					break;
				case 2:
					if (Utility.RandomBool())
						item = new PottedTree();
					else
						item = new PottedTree1();
					break;
				case 3:
					switch (Utility.Random(3))
					{
						case 0: item = new PottedPlant(); break;
						case 1: item = new PottedPlant1(); break;
						case 2: item = new PottedPlant2(); break;
					}
					break;
				case 4: item = new SpecialHairDye(); break;
				case 5: item = new SpecialBeardDye(); break;
				case 6: item = new TempHorseEthereal(50);
						item.Hue = Utility.Random(795, 7);
						break;
				case 7: item = new FireworksWand(99); break;
				case 8: item = new LayeredSashDeed(); break;
				case 9: item = new SkillBall(1); break;
				case 10: item = new SkillBall(5); break;
				case 11: item = new SkillBall(10); break;
				case 12: item = new SkillBall(25); break;
				case 13: item = new SkillBall(50); break;
				case 14: item = new PersonalisationDeed(); break;
				case 15: item = new CrystalPedestalAddonDeed(); break;
				case 16: item = new FountainStoneAddonDeed(); break;
				case 17: item = new FountainSandstoneAddonDeed(); break;
				case 18: item = new SquirrelStatueEastDeed(); break;
				case 19: item = new SquirrelStatueSouthDeed(); break;
				case 20: item = new ArcanistStatueEastDeed(); break;
				case 21: item = new ArcanistStatueSouthDeed(); break;
				case 22: item = new WarriorStatueEastDeed(); break;
				case 23: item = new WarriorStatueSouthDeed(); break;
				case 24: item = new TempRobe(31);
						((BaseClothing)item).Attributes.RegenHits = 3;
						break;
				case 25: item = new TempCloak(31);
						((BaseClothing)item).Attributes.RegenHits = 3;
						break;
				case 26: item = new CampfireDeed(); break;
				case 27: item = new FireDeed(); break;
				case 28: item = new SoulstoneFragment();
						((SoulstoneFragment)item).Account = m.Account.Username;
						break;
				case 29: item = new NameChangeDeed(); break;
				case 30: item = new SexChangeDeed(); break;
				case 31: item = new KillResetDeedAOS(); break;
				case 32: item = new PetBondingDeed(); break;
				case 33: item = new WarHorseBondingDeed(); break;
				case 34: item = new AntiBlessDeed(); break;
				case 35: item = new WhisperingRose(m.Name); break;
				case 36: item = new WeddingDeed(); break;
				case 37: item = new KillBook();
						((KillBook)item).BookOwner = m;
						break;
			}

			if (item != null)
			{
				m.AddToBackpack(item);
				m.SendMessage("The reward item has been placed into your backpack, have fun!");
			}
			else
			{
				m.SendMessage("That item is not available. Please report the bug at the site that will open in your browser.");
				m.LaunchBrowser( "http://bug.casiopia.net/" );
				m.AddToBackpack( new CopperBar(info.Price) );
			}
		}
	}

	public enum RewardType
	{
		Deco = 2,
		PlayItem = 1,
		Trammelite = 3

	}

	public struct EventRewardInfo
	{
		public RewardType RType;
		public string Name;
		public string Info;
		public int Price;
		public int ItemID;
		public int IncrID;
		public int X, Y;

		public EventRewardInfo(RewardType rtype, string name, string info, int price, int itemid, int incrid, int x, int y)
		{
			RType = rtype;
			Name = name;
			Info = info;
			Price = price;
			ItemID = itemid;
			IncrID = incrid;
			X = x;
			Y = y;

			EventRewardSystem.GetList((int)rtype).Add(this);
		}
	}
}