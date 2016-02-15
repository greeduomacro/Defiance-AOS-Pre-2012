using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using Server.Items;
using Server.Mobiles;
using System.Threading;

namespace Server
{
	public static class SunnySystem
	{
		public static void Configure()
		{
			CustomSaving.AddSaveModule(new SaveData(new DC.SaveMethod(Serialize), new DC.LoadMethod(Deserialize)), "Sunny System");
		}

		#region Event Methods
		#region Members
		private static Point3D m_PetRemLoc = new Point3D(1430, 1686, 20);

		private static Layer[] m_ExcludedLayers = new Layer[]
			{
				Layer.Backpack,
				Layer.Bank,
				Layer.FacialHair,
//		Layer.Mount,
				Layer.Hair
			};
		#endregion

		#region Undressing / Redressing
		private static Dictionary<Mobile, UndressedPlayer> m_UndressedDictionary = new Dictionary<Mobile, UndressedPlayer>();

		public static bool Undress(Mobile m)
		{
			return Undress(m, null);
		}

		public static bool Undress(Mobile m, Predicate<Item> pred)
		{
			if (!m.Alive)
			{
				m.Resurrect();
				m.Hits = m.HitsMax;
			}

			if (m.Backpack != null)
			{
				Bag bag;
				UndressedPlayer ud;

				if (!m_UndressedDictionary.TryGetValue(m, out ud))
				{
					bag = new Bag();
					m.BankBox.DropItem(bag);
					ud = new UndressedPlayer(bag);
					m_UndressedDictionary[m] = ud;
				}

				else
					return false;

				if (m.Holding != null)
					m.Backpack.DropItem(m.Holding);

				StablePets(m, ud);

				for (int i = m.Items.Count - 1; i >= 0; --i)
				{
					Item item = (Item)m.Items[i];
					if (Array.BinarySearch(m_ExcludedLayers, item.Layer) < 0)
					{
						if (pred == null || pred(item))
						{
							ud.WornList.Add(item);
							bag.DropItem(item);
						}
					}
				}

				for (int i = m.Backpack.Items.Count; i > 0; i--)
				{
					Item item = m.Backpack.Items[i - 1];
					if (pred == null || pred(item))
					{
						ud.BackPackList.Add(item);
						bag.DropItem(item);
					}
				}

				return true;
			}

			else
			{
				m.SendMessage("Unfortunately we could not undress you, as you have no backpack, please contact the staff.");
				return false;
			}
		}

		public static bool ReDress(Mobile m)
		{
			UndressedPlayer ud;

			if ( !m_UndressedDictionary.TryGetValue(m, out ud) )
				m.SendMessage("Unfortunately we could not find your belongings, please contact the staff.");
			else
			{
				if ( !m.Alive )
					m.Resurrect();

				Container pack = m.Backpack;

				if ( pack != null )
				{
					foreach ( Item item in ud.WornList )
					{
						if (!item.CanEquip(m) || !m.EquipItem(item))
							m.AddToBackpack(item);
					}

					foreach (Item item in ud.BackPackList)
						m.AddToBackpack(item);

					if (ud.Bag.Items.Count == 0)
						ud.Bag.Delete();

					if (ud.PetList.Count > 0)
						ReturnPets(m, ud);

					m.SendMessage("Your belongings have been returned to you.");
					m_UndressedDictionary.Remove(m);

					return true;
				}
				else
					m.SendMessage("Unfortunately we could not return your belongings, as you have no backpack, please contact the staff.");
			}

			return false;
		}

		public static void StablePets(Mobile m) { StablePets(m, null); }

		private static void StablePets(Mobile m, UndressedPlayer ud)
		{
			if (m.Mount != null)
				m.Mount.Rider = null;

			if (m.Followers > 0)
			{
				int nonstabled = 0;
				List<BaseCreature> stable = new List<BaseCreature>();
				List<BaseCreature> delete = new List<BaseCreature>();

				foreach (Mobile i in m.GetMobilesInRange(20))
				{
					if (i is BaseCreature)
					{
						BaseCreature pet = (BaseCreature)i;

						if (pet.Summoned && pet.SummonMaster == m)
						   delete.Add(pet);

						else if (pet.Controlled && pet.ControlMaster == m)
							stable.Add(pet);
					}
				}

				foreach (Mobile pet in delete)
					pet.Delete();

				int max = AnimalTrainer.GetMaxStabled(m);

				foreach (BaseCreature pet in stable)
				{

					if (m.Stabled.Count >= max && ud == null)
					{
						pet.MoveToWorld(m_PetRemLoc, Map.Felucca);
						nonstabled++;
					}

					else
					{
						if (ud != null)
							ud.PetList.Add(pet);

						pet.ControlTarget = null;
						pet.ControlOrder = OrderType.Stay;
						pet.Internalize();
						pet.SetControlMaster(null);
						pet.SummonMaster = null;
						pet.IsStabled = true;
						m.Stabled.Add(pet);
					}
				}

				if (nonstabled > 0)
					m.SendMessage("While trying to stable your pets, you reached your maximum stable capacity. {0} Pets have been moved to Britain Bank.", nonstabled);

				else
					m.SendMessage("Your pets have been stabled.");
			}
		}

		private static void ReturnPets(Mobile m, UndressedPlayer ud)
		{
			foreach (BaseCreature pet in ud.PetList)
			{
				pet.SetControlMaster(m);

				if (pet.Summoned)
					pet.SummonMaster = m;

				pet.ControlTarget = m;
				pet.ControlOrder = OrderType.Follow;

				pet.MoveToWorld(m.Location, m.Map);

				pet.IsStabled = false;
				m.Stabled.Remove(pet);
			}

			m.SendMessage("Your pets have been returned to you.");
		}
		#endregion

		#region ArmPlayer / DisarmPlayer
		private static Dictionary<Mobile, List<Item>> m_ArmedDictionary = new Dictionary<Mobile, List<Item>>();
		public static Dictionary<Mobile, List<Item>> ArmedDictionary { get { return m_ArmedDictionary; } set { m_ArmedDictionary = value; } }

		public static bool ArmPlayer(Mobile m, List<Item> toarm)
		{
			if (m_ArmedDictionary.ContainsKey(m))
			{
				m.SendMessage("You have already been armed, please contact the staff.");
				return false;
			}

			if (!ProcedeArming(m, toarm))
				return false;

			m.SendMessage("You have been armed for the battle ahead.");
			m_ArmedDictionary[m] = toarm;

			return true;
		}

		public static bool ReArmPlayer(Mobile m, List<Item> toarm)
		{
			List<Item> armedlist;

			if (!m_ArmedDictionary.TryGetValue(m, out armedlist))
			{
				m.SendMessage("You have not been armed yet, please contact the staff.");
				return false;
			}

			if (!ProcedeArming(m, toarm))
				return false;

			foreach (Item item in toarm)
				armedlist.Add(item);

			return true;
		}

		private static bool ProcedeArming(Mobile m, List<Item> toarm)
		{
			Container pack = m.Backpack;

			if (pack != null)
			{
				int xarmor = 44;
				int yarmor = 107;

				int xweap = 44;
				int yweap = 127;

				foreach (Item item in toarm)
				{
					item.LootType = LootType.Blessed;
					item.Name = "[Event Item]";

					if (item is Spellbook || !item.CanEquip(m) || !m.EquipItem(item))
					{
						int x = 0; int y = 0;
						if (item.GetType() == typeof(Bandage)) { x = 142; y = 65; }
						else if (item.GetType().IsSubclassOf(typeof(BasePoisonPotion))) { x = 65; y = 65; }

						else if (item.GetType() == typeof(Arrow)) { x = 110; y = 65; }
						else if (item.GetType() == typeof(Bolt)) { x = 125; y = 65; }

						else if (item.GetType() == typeof(NecromancerSpellbook)) { x = 44; y = 85; }
						else if (item.GetType() == typeof(BookOfChivalry)) { x = 64; y = 85; }
						else if (item.GetType() == typeof(Spellbook)) { x = 84; y = 85; }

						else if (item.GetType().IsSubclassOf(typeof(BaseArmor))) { x = xarmor += 15; y = yarmor; if (xarmor > 142) xarmor = 44; }
						else if (item.GetType().IsSubclassOf(typeof(BaseWeapon))) { x = xweap += 15; y = yweap; if (xweap > 142) xweap = 44; }

						else { x = 142; y = 90; }

						pack.DropItem(item);

						item.Location = new Point3D(x, y, 0);
					}
				}
			}

			else
			{
				m.SendMessage("As you have no backpack, we cannot provide you with any armament, please contact the staff.");
				return false;
			}

			return true;
		}

		public static bool DisArmPlayer(Mobile m)
		{
			List<Item> toremove;

			if (!m_ArmedDictionary.TryGetValue(m, out toremove))
			{
				m.SendMessage("No record has been found of you being armed, please contact the staff.");
				return false;
			}

			foreach (Item item in toremove)
				if (item != null && !item.Deleted)
					item.Delete();

			for (int i = m.Items.Count - 1; i >= 0; i--)
			{
				Item item = m.Items[i];
				if (item.Name == "[Event Item]")
					item.Delete();
			}

			if (m.Backpack != null)
			{
				Container pack = m.Backpack;

				if (m.Holding != null)
					pack.DropItem(m.Holding);

				for (int i = pack.Items.Count - 1; i >= 0; i--)
				{
					Item item = pack.Items[i];
					if (item.Name == "[Event Item]")
						item.Delete();
				}
			}

			m_ArmedDictionary.Remove(m);

			m.SendMessage("You have been disarmed.");
			return true;
		}
		#endregion
		#endregion

		#region Serializing
		public static void Serialize(GenericWriter writer)
		{
			writer.Write(0);//version

			writer.Write(m_ArmedDictionary.Count);
			foreach (KeyValuePair<Mobile, List<Item>> kv in m_ArmedDictionary)
			{
				writer.WriteItemList(kv.Value);
			}
		}

		public static void Deserialize(GenericReader reader)
		{
			int version = reader.ReadInt();
			switch (version)
			{
				case 0:
					int count = reader.ReadInt();
					for (int i = 0; i < count; i++)
					{
						List<Item> list = reader.ReadStrongItemList();
						foreach (Item item in list)
							if (item != null && !item.Deleted)
								item.Delete();
					}
					break;
			}
		}
		#endregion
	}

	#region Support Classes
	public class UndressedPlayer
	{
		public List<BaseCreature> PetList = new List<BaseCreature>();
		public List<Item> BackPackList = new List<Item>();
		public List<Item> WornList = new List<Item>();
		public Bag Bag;

		public UndressedPlayer(Bag bag)
		{
			Bag = bag;
		}
	}
	#endregion
}