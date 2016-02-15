using System;
using System.Collections.Generic;
using Server;
using Server.Misc;
using Server.Items;

namespace Server.Mobiles
{
	public class TheCursedCorpse : BaseCursedCreature
	{
		public static readonly bool Enabled = true;
		public static readonly int MaxCorpsesPerPlayer = 5;

		private static double ScaleHits = 0.5; // 50%
		private static double ScaleSkills = 0.8; // 80%

		public override bool UseRearm { get { return true; } }
		public override InhumanSpeech SpeechType { get { return CursedCaveSpeech.Cursed; } }
		public override bool AlwaysAttackable { get { return true; } }
		public override bool AlwaysMurderer { get { return true; } }

		private PlayerMobile m_owner;
		public PlayerMobile Owner
		{
			get { return m_owner; }
			set { m_owner = value; }
		}

		public static void Initialize()
		{
			if (Enabled)
				EventSink.PlayerDeath += new PlayerDeathEventHandler(EventSink_PlayerDeath);
		}

		public TheCursedCorpse(PlayerMobile owner, Corpse corpse)
			: base(AIType.AI_Melee, FightMode. Closest, 10, 1, 0.2, 0.4)
		{
			m_owner = owner;

			Body = m_owner.Female ? 0x191 : 0x190;
			Name = m_owner.Name;
			Title = m_owner.Title;
			Hue = m_owner.Hue;

			HairItemID = m_owner.HairItemID;
			HairHue = m_owner.HairHue;

			SetStr(m_owner.RawStr);
			SetDex(m_owner.RawDex);
			SetInt(m_owner.RawInt);

			for (int i = 0; ; ++i)
			{
				if (m_owner.Skills[i] == null || Skills[i] == null)
					break;

				Skills[i].Base = ScaleValue(m_owner.Skills[i].Base, ScaleSkills);
			}

			SetHits(ScaleValue(m_owner.HitsMax, ScaleHits));
			SetStam(m_owner.StamMax);

			corpse.TurnToBones();
			GetItems(corpse, this.Backpack);

			Timer.DelayCall(TimeSpan.Zero, CheckAI);
		}

		public TheCursedCorpse(Serial serial)
			: base(serial)
		{
		}

		private int ScaleValue(double value, double scale)
		{
			if (value <= 0)
				return 0;
			else return (int)(value * scale);
		}

		private void CheckAI()
		{
			if (Weapon is BaseWeapon && ((BaseWeapon)Weapon).Skill == SkillName.Archery)
				AI = AIType.AI_Archer;
		}

		private static Dictionary<PlayerMobile, int> CorpseRegistry = new Dictionary<PlayerMobile, int>();

		public static int GetCorpsesInRegistry(PlayerMobile player)
		{
			if (CorpseRegistry.ContainsKey(player))
				return CorpseRegistry[player];
			else return 0;
		}

		public static void RegisterCorpse(PlayerMobile player)
		{
			if (CorpseRegistry.ContainsKey(player))
				CorpseRegistry[player]++;
			else
				CorpseRegistry.Add(player, 1);
		}

		public static void UnRegisterCorpse(PlayerMobile player)
		{
			if (CorpseRegistry.ContainsKey(player))
			{
				if (CorpseRegistry[player] > 1)
					CorpseRegistry[player]--;
				else
					CorpseRegistry.Remove(player);
			}
		}

		private static void EventSink_PlayerDeath(PlayerDeathEventArgs e)
		{
			if (e.Mobile is PlayerMobile && e.Mobile.Region is Regions.CursedCaveRegion && GetCorpsesInRegistry((PlayerMobile)e.Mobile) < MaxCorpsesPerPlayer)
			{
				RegisterCorpse((PlayerMobile)e.Mobile);
				Timer.DelayCall(TimeSpan.FromSeconds(10.0), new TimerStateCallback(StartCorpseCreation), new CCDeathEntry((PlayerMobile)e.Mobile, (Corpse)e.Mobile.Corpse));
			}
		}

		private static void StartCorpseCreation(object state)
		{
			if (state is CCDeathEntry)
			{
				CCDeathEntry entry = (CCDeathEntry)state;

				if (CheckCCDeathEntry(entry))
				{
					Effects.PlaySound(entry.Corpse.Location, entry.Corpse.Map, 0x1FB);
					Effects.SendLocationParticles(EffectItem.Create(entry.Corpse.Location, entry.Corpse.Map, EffectItem.DefaultDuration), 0x3789, 1, 40, 0x3F, 3, 9907, 0);

					Timer.DelayCall(TimeSpan.FromSeconds(4.0), new TimerStateCallback(CreateTheCursedCorpse), entry);
				}
			}
		}

		private static void CreateTheCursedCorpse(object state)
		{
			if (state is CCDeathEntry)
			{
				CCDeathEntry entry = (CCDeathEntry)state;

				if (CheckCCDeathEntry(entry))
				{
					TheCursedCorpse cursedCorpse = new TheCursedCorpse(entry.Player, entry.Corpse);
					cursedCorpse.Home = entry.Corpse.Location;
					cursedCorpse.RangeHome = 10;
					cursedCorpse.MoveToWorld(entry.Corpse.Location, entry.Corpse.Map);
				}
			}
		}

		private static bool CheckCCDeathEntry(CCDeathEntry entry)
		{
			if (entry.Player != null && !entry.Player.Deleted && entry.Corpse != null && !entry.Corpse.Deleted)
				return true;
			return false;
		}

		#region CCDeathEntry
		public class CCDeathEntry
		{
			private PlayerMobile m_Player;
			public PlayerMobile Player
			{
				get { return m_Player; }
				set { m_Player = value; }
			}

			private Corpse m_Corpse;
			public Corpse Corpse
			{
				get { return m_Corpse; }
				set { m_Corpse = value; }
			}

			public CCDeathEntry(PlayerMobile player, Corpse corpse)
			{
				m_Player = player;
				m_Corpse = corpse;
			}
		}
		#endregion

		public override void OnDelete()
		{
			if (m_owner != null && !m_owner.Deleted)
				UnRegisterCorpse(m_owner);

			base.OnDelete();
		}

		public override void AddNameProperties(ObjectPropertyList list)
		{
			base.AddNameProperties(list);

			list.Add(1070722, "(Cursed Corpse)");
		}

		public void GetItems(Corpse corpse, Container cont)
		{
			List<Item> items = new List<Item>(corpse.Items);

			for (int i = 0; i < items.Count; ++i)
			{
				Item item = items[i];
				Point3D loc = item.Location;

				if ((item.Layer == Layer.Hair || item.Layer == Layer.FacialHair) || !item.Movable || !corpse.GetRestoreInfo(item, ref loc))
					continue;

				if (!(corpse.EquipItems.Contains(item) && this.EquipItem(item)))
				{
					item.Location = loc;
					cont.AddItem(item);
				}
			}
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write((int)1);

			// Version 1
			writer.Write(m_owner);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadInt();

			switch (version)
			{
				default: break;
				case 1:
					m_owner = (PlayerMobile)reader.ReadMobile();
					break;
			}

			if (m_owner != null && !m_owner.Deleted)
				RegisterCorpse(m_owner);
		}
	}
}