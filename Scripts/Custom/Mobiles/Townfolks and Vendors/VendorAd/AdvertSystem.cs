 using System;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Accounting;

namespace Server.Misc
{
	public static class AdvertSystem
	{
		public const int Minimumprice = 10;

		public static AdvertEntry CurrentPublicAdd;
		public static List<AdvertMobile> Advertisers = new List<AdvertMobile>();
		public static Category[] Categories = new Category[]
			{
				new Category(0, "Weapons"),
				new Category(1, "Armor"),
				new Category(2, "Misc"),
				new Category(3, "Resources"),
			};

		public static void Configure()
		{
			CustomSaving.AddSaveModule(new SaveData(new DC.SaveMethod(Serialize), new DC.LoadMethod(Deserialize)), "Vendor Advertisement");
			Timer.DelayCall(TimeSpan.FromHours(12.0), TimeSpan.FromHours(12.0), new TimerCallback(VerifyAllAdds));
			Timer.DelayCall(TimeSpan.FromMinutes(3.0), TimeSpan.FromMinutes(3.0), new TimerCallback(AdvertiseRandomAdd));
		}

		public static void AdvertiseRandomAdd()
		{
			Category cat = Categories[Utility.Random(Categories.Length)];
			if (cat.Entries.Count > 0)
			{
				AdvertEntry entry = cat.Entries[Utility.Random(cat.Entries.Count)];
				if (entry != null)
				{
					CurrentPublicAdd = entry;
					foreach (AdvertMobile mob in Advertisers)
					{
						if (MobileExists(mob))
						{
							mob.Say("[public advertisement]	   " + entry.Vendor.Name + ": " + entry.Description);
							mob.Say("Say \"Go\" to meet this vendor");
						}
					}
				}
			}
		}

		public static void VerifyAllAdds()
		{
			List<AdvertEntry> list = new List<AdvertEntry>();

			foreach (Category cat in Categories)
			{
				foreach (AdvertEntry entry in cat.Entries)
				{
					if (!MobileExists(entry.Vendor) || !MobileExists(entry.Owner) || !Banker.Withdraw(entry.Owner, 50))
						list.Add(entry);

					else
						entry.FreeAccounts.Clear();
				}
			}

			foreach (AdvertEntry entry in list)
				entry.Category.Entries.Remove(entry);
		}

		public static void AddEntry(AdvertEntry entry)
		{
			foreach(Category cat in Categories)
				if(cat.Entries.Remove(entry))
					break;

			entry.Category.Entries.Add(entry);
			entry.Category.Entries.Sort();
		}

		public static void UseAdvert(Mobile m, AdvertEntry entry)
		{
			if (MobileExists(m) && m.Player && m.Alive && !m.Criminal && !Server.Spells.SpellHelper.CheckCombat(m) && m.Spell == null)
			{
				bool remove = true;

				if (entry != null && MobileExists(entry.Vendor) && MobileExists(entry.Owner))
				{
					if (entry.FreeAccounts.Contains((Account)m.Account))
						remove = false;
					else if (Banker.Withdraw(entry.Owner, entry.Owner.NetState != null ? (int)(entry.Price * 0.5):entry.Price))
					{
						entry.FreeAccounts.Add((Account)m.Account);
						remove = false;
					}
				}

				if (!remove)
				{
					m.MoveToWorld(entry.Vendor.Location, entry.Vendor.Map);
					m.SendMessage("You have been moved to the chosen vendor.");
				}

				else
				{
					m.SendMessage("We are sorry, this is an old advertisement, please select a different one.");
					entry.Category.Entries.Remove(entry);
					m.SendGump(new AdvertCategoriesGump());
				}
			}
		}

		public static bool MobileExists(Mobile m)
		{
			return m != null && !m.Deleted;
		}

		private static void Serialize(GenericWriter writer)
		{
			writer.Write(0);//version

			writer.Write(Categories.Length);
			foreach (Category cat in Categories)
			{
				writer.Write(cat.Index);
				writer.Write(cat.Name);

				writer.Write(cat.Entries.Count);
				foreach (AdvertEntry entry in cat.Entries)
					entry.Serialize(writer);
			}
		}

		private static void Deserialize(GenericReader reader)
		{
			int version = reader.ReadInt();

			int count = reader.ReadInt();
			for (int i = 0; i < count; i++)
			{
				int index = reader.ReadInt();
				string name = reader.ReadString();
				Category cat = null;
				if (Categories.Length > index && Categories[index].Name == name)
					cat = Categories[index];

				else
				{
					foreach (Category cato in Categories)
					{
						if (cato.Name == name)
						{
							cat = cato;
							break;
						}
					}
				}

				int counter = reader.ReadInt();
				for (int j = 0; j < counter; j++)
				{
					AdvertEntry entry = AdvertEntry.Deserialize(reader, cat);
					if (entry != null && cat != null)
						cat.Entries.Add(entry);
				}
			}

		}
	}

	public class Category
	{
		public string Name;
		public int Index;
		public List<AdvertEntry> Entries;

		public Category(int index, string name)
		{
			Index = index;
			Name = name;
			Entries = new List<AdvertEntry>();
		}
	}

	public class AdvertEntry : IComparable
	{
		public PlayerVendor Vendor;
		public Category Category;
		public string Description;
		public Mobile Owner;
		public int Price;
		public List<Account> FreeAccounts = new List<Account>();

		public AdvertEntry(Mobile owner)
		{
			Owner = owner;
		}

		public static AdvertEntry Deserialize(GenericReader reader, Category cat)
		{
			AdvertEntry entry = null;

			int version = reader.ReadInt();
			PlayerVendor vendor = reader.ReadMobile() as PlayerVendor;
			string descr = reader.ReadString();
			int price = reader.ReadInt();

			if (vendor != null)
			{
				entry = new AdvertEntry(vendor.Owner);

				entry.Vendor = vendor;
				entry.Description = descr;
				entry.Price = price;
				entry.Category = cat;
			}

			return entry;
		}

		public void Serialize(GenericWriter writer)
		{
			writer.Write(0);//version

			writer.Write(Vendor);
			writer.Write(Description);
			writer.Write(Price);
		}

		public int CompareTo(object o)
		{
			if (o is AdvertEntry)
			{
				AdvertEntry ae = (AdvertEntry)o;
				return ae.Price - Price;
			}
			else
				throw new ArgumentException("object is not AdvertEntry");
		}
	}
}