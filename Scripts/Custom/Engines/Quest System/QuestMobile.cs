using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Spells;
using Server.Spells.Third;
using Server.Spells.Fourth;

namespace Server.Mobiles
{
	public class QuestMobile : BaseCreature
	{
		#region CommandProperties

		private bool m_bUseGoldSpawn;
		[CommandProperty(AccessLevel.Seer)]
		public bool UseGoldSpawn
		{
			get { return m_bUseGoldSpawn; }
			set { m_bUseGoldSpawn = value; }
		}

		private bool m_GiveRewardBag;
		[CommandProperty(AccessLevel.GameMaster)]
		public bool GiveRewardBag
		{
			get { return m_GiveRewardBag; }
			set { m_GiveRewardBag = value; }
		}

		private int m_DrainChance;
		[CommandProperty(AccessLevel.GameMaster)]
		public int LifeDrainChance
		{
			get { return m_DrainChance; }
			set { m_DrainChance = value; }
		}

		private TimeSpan m_PullDelay;
		[CommandProperty(AccessLevel.GameMaster)]
		public TimeSpan DelayPull
		{
			get { return m_PullDelay; }
			set
			{
				m_PullDelay = value;
				if (m_PullDelay < TimeSpan.FromSeconds(10.0))
					m_PullDelay = TimeSpan.Zero;
			}
		}

		private TimeSpan m_DispelDelay;
		[CommandProperty(AccessLevel.GameMaster)]
		public TimeSpan DelayDispel
		{
			get { return m_DispelDelay; }
			set
			{
				m_DispelDelay = value;
				if (m_DispelDelay < TimeSpan.FromSeconds(5.0))
					m_DispelDelay = TimeSpan.FromSeconds(5.0);
			}
		}

		private TimeSpan m_FireballDelay;
		[CommandProperty(AccessLevel.GameMaster)]
		public TimeSpan DelayFireBall
		{
			get { return m_FireballDelay; }
			set
			{
				m_FireballDelay = value;
				if (m_FireballDelay < TimeSpan.FromSeconds(10.0))
					m_FireballDelay = TimeSpan.Zero;
			}
		}

		private TimeSpan m_LightningDelay;
		[CommandProperty(AccessLevel.GameMaster)]
		public TimeSpan DelayLightning
		{
			get { return m_LightningDelay; }
			set
			{
				m_LightningDelay = value;
				if (m_LightningDelay < TimeSpan.FromSeconds(10.0))
					m_LightningDelay = TimeSpan.Zero;
			}
		}

		private bool m_SpawnsFollowers;
		[CommandProperty(AccessLevel.GameMaster)]
		public bool SpawnsCreatures
		{
			get { return m_SpawnsFollowers; }
			set { m_SpawnsFollowers = value; }
		}

		private string m_FollowerOne;
		[CommandProperty(AccessLevel.GameMaster)]
		public string SpawnTypeOne
		{
			get { return m_FollowerOne; }
			set { m_FollowerOne = GetCreature(value); }
		}

		private string m_FollowerTwo;
		[CommandProperty(AccessLevel.GameMaster)]
		public string SpawnTypeTwo
		{
			get { return m_FollowerTwo; }
			set { m_FollowerTwo = GetCreature(value); }
		}

		private string m_FollowerThree;
		[CommandProperty(AccessLevel.GameMaster)]
		public string SpawnTypeThree
		{
			get { return m_FollowerThree; }
			set { m_FollowerThree = GetCreature(value); }
		}
		#endregion

		private string GetCreature(string str)
		{
			if (str != null)
			{
				Type type = ScriptCompiler.FindTypeByName(str);

				if (type != null && type.IsSubclassOf(typeof(BaseCreature)))
					return str;
			}

			return null;
		}

		private DateTime m_LastPull, m_LastFireBall, m_LastLightning, m_LastDispel;
		private List<Mobile> m_SpawnList;

		[Constructable]
		public QuestMobile()
			: base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.05, 0.1)
		{
			Name = "blank quest creature";
			Body = 0x190;

			SetStr(500);
			SetDex(500);
			SetInt(500);

			SetHits(40000);

			SetDamage(20, 30);
			SetDamageType(ResistanceType.Physical, 100);

			SetResistance(ResistanceType.Physical, 65);
			SetResistance(ResistanceType.Fire, 65);
			SetResistance(ResistanceType.Cold, 65);
			SetResistance(ResistanceType.Poison, 65);
			SetResistance(ResistanceType.Energy, 65);

			for (int i = 0; i < Skills.Length; ++i)
				Skills[i].Base = 140.0;

			Fame = 24000;
			Karma = -24000;

			m_SpawnList = new List<Mobile>();
		}

		public override bool AlwaysMurderer { get { return true; } }
		public override bool BardImmune { get { return true; } }
		public override Poison PoisonImmune { get { return Poison.Deadly; } }
		public override bool ShowFameTitle { get { return false; } }
		public override bool ClickTitle { get { return false; } }

		public override void AlterMeleeDamageTo(Mobile to, ref int damage)
		{
			if (to is BaseCreature && (((BaseCreature)to).Controlled || ((BaseCreature)to).Summoned))
				damage *= 3;
		}

		public Container GetNewContainer()
		{
			Bag bag = new Bag();
			bag.Hue = 3;
			bag.Name = "Reward bag for slaying " + Name;
			return bag;
		}

		public override void OnDamage(int amount, Mobile from, bool willKill)
		{
			if (m_DrainChance != 0 && (m_DrainChance * 0.01) > Utility.RandomDouble())
				DrainLife();

//edit - Disabled
/*			if (m_SpawnsFollowers && Utility.RandomDouble() < .02)
			{
				string[] strarray = new string[] { m_FollowerOne, m_FollowerTwo, m_FollowerThree };

				foreach (string str in strarray)
				{
					if (str != null)
					{
						Type type = ScriptCompiler.FindTypeByName(str);
						BaseCreature bc = (BaseCreature)Activator.CreateInstance(type);

						if (bc != null && this != null && Map != null && from != null)
						{
							bc.MoveToWorld(GetSpawnLocation(from), Map);
							bc.RangeHome = 5;
							bc.Home = Location;

							bc.Combatant = from;

							if (m_SpawnList == null)
								m_SpawnList = new List<Mobile>();

							m_SpawnList.Add(bc);
						}
					}
				}
			}
*/

//Add by Draconis
 if ( m_SpawnsFollowers && Map != null && from != null )
	 {
	  if (m_FollowerOne != null && Utility.RandomDouble() < .10)
			 {
				 Type type = ScriptCompiler.FindTypeByName (m_FollowerOne);
				 BaseCreature bc = (BaseCreature)Activator.CreateInstance(type);

				 if (bc != null && this != null)
				  {
					 bc.MoveToWorld(GetSpawnLocation(from), Map);
					 bc.RangeHome = 5;
					 bc.Home = Location;
					  bc.Combatant = from;
				 }
			 }

	  if (m_FollowerTwo != null && Utility.RandomDouble() < .30)
			 {
				Type type = ScriptCompiler.FindTypeByName(m_FollowerTwo);
				 BaseCreature bc = (BaseCreature)Activator.CreateInstance(type);

				 if (bc != null && this != null)
				 {
					 bc.MoveToWorld(GetSpawnLocation(from), Map);
					 bc.RangeHome = 5;
					 bc.Home = Location;
					 bc.Combatant = from;
				 }
			 }

			 if (m_FollowerThree != null && Utility.RandomDouble() < .50)
			 {
				 Type type = ScriptCompiler.FindTypeByName(m_FollowerThree);
				 BaseCreature bc = (BaseCreature)Activator.CreateInstance(type);

				 if (bc != null && this != null)
				 {
					 bc.MoveToWorld(GetSpawnLocation(from), Map);
					 bc.RangeHome = 5;
					 bc.Home = Location;
					 bc.Combatant = from;
				 }
			 }
	 }
//End add

			base.OnDamage(amount, from, willKill);
		}

		public Point3D GetSpawnLocation(Mobile m)
		{
			if (m != null && Utility.RandomDouble() < 0.3 && m.Location != Point3D.Zero && m.Map != Map.Internal)
				return m.Location;

			for (int i = 0; i < 10; i++)
			{
				int x = X + Utility.RandomMinMax(-5, 5);
				int y = Y + Utility.RandomMinMax(-5, 5);
				int z = Map.GetAverageZ(x, y);

				if (Map.CanSpawnMobile(new Point2D(x, y), Z))
					return new Point3D(x, y, Z);
				else if (Map.CanSpawnMobile(new Point2D(x, y), z))
					return new Point3D(x, y, z);
			}

			return Location;
		}

		public void GiveReward()
		{
			ArrayList toGive = new ArrayList();
			List<DamageStore> rights = BaseCreature.GetLootingRights(DamageEntries, HitsMax);

			for (int i = 0; i < rights.Count; ++i)
			{
				DamageStore ds = (DamageStore)rights[i];

				if (ds.m_HasRight)
					toGive.Add(ds.m_Mobile);
			}

			if (toGive.Count == 0)
				return;

			for (int i = 0; i < toGive.Count; ++i)
			{
				Mobile m = (Mobile)toGive[i];

				m.SendMessage("You have been rewarded for your help in the battle against {0}.", Name.ToString());

				Container cont = GetNewContainer();
				//items
				LootPackEntry.AddRandomLoot(cont, 5, 360,  4, 5, 50, 100);
				LootPackEntry.AddRandomLoot(cont, 1, 360,  5, 5, 70, 100);

				if (0.50 > Utility.RandomDouble())//sos
					cont.DropItem(new MessageInABottle(Map.Felucca));
				if (0.20 > Utility.RandomDouble())
					cont.DropItem(new MessageInABottle(Map.Felucca, 4));

				if (0.50 > Utility.RandomDouble())//t-maps
					cont.DropItem(new TreasureMap(5, Map.Felucca));
				if (0.25 > Utility.RandomDouble())
					cont.DropItem(new TreasureMap(6, Map.Felucca));

				cont.DropItem(new BankCheck(5000));
				m.PlaceInBackpack(cont);
			}
		}

		public override bool OnBeforeDeath()
		{
			if (m_GiveRewardBag)
				GiveReward();

			if (UseGoldSpawn)
			{
				if (Map != null)
				{
					for (int x = -12; x <= 12; ++x)
					{
						for (int y = -12; y <= 12; ++y)
						{
							double dist = Math.Sqrt(x * x + y * y);

							if (dist <= 12)
								new GoldSpawnTimer(Map, X + x, Y + y).Start();
						}
					}
				}
			}
			return base.OnBeforeDeath();
		}

		public override void GenerateLoot()
		{
			AddLoot(LootPack.SuperBoss, 2);
			AddLoot(LootPack.HighScrolls, Utility.RandomMinMax(5, 10));
		}

		public override void OnThink()
		{
			base.OnThink();

			if (m_DispelDelay != TimeSpan.Zero && DateTime.Now > m_LastDispel + m_DispelDelay)
			{
				Dispel();
				m_LastDispel = DateTime.Now;
			}

			if (m_FireballDelay != TimeSpan.Zero && Utility.RandomBool() && DateTime.Now > m_LastFireBall + m_FireballDelay)
			{
				FireBall();
				m_LastFireBall = DateTime.Now;
			}

			if (m_LightningDelay != TimeSpan.Zero && Utility.RandomBool() && DateTime.Now > m_LastLightning + m_LightningDelay)
			{
				Lightning();
				m_LastLightning = DateTime.Now;
			}

			if (m_PullDelay != TimeSpan.Zero && Utility.RandomBool() && DateTime.Now > m_LastPull + m_PullDelay)
			{
				Pull();
				m_LastPull = DateTime.Now;
			}
		}

		public void DrainLife()
		{
			ArrayList list = new ArrayList();

			foreach (Mobile m in this.GetMobilesInRange(2))
			{
				if (m == this || !CanBeHarmful(m) || !m.Alive || m.Hidden)
					continue;

				if (m is BaseCreature && (((BaseCreature)m).Controlled || ((BaseCreature)m).Summoned || ((BaseCreature)m).Team != this.Team))
					list.Add(m);
				else if (m.Player)
					list.Add(m);
			}

			foreach (Mobile m in list)
			{
				DoHarmful(m);

				m.FixedParticles(0x374A, 10, 15, 5013, 0x496, 0, EffectLayer.Waist);
				m.PlaySound(0x231);

				m.SendMessage("You feel the life drain out of you!");
				int mh = m.HitsMax;
				int toDrain = Utility.RandomMinMax((int)(mh * 0.1), (int)(mh * 0.5));

				Hits += toDrain;
				m.Damage(toDrain, this);
			}
		}

		private void FireBall()
		{
			ArrayList targets = new ArrayList();

			foreach (Mobile m in GetMobilesInRange(16))
				if (m != this && CanBeHarmful(m) && CanSee(m) && (m.Player || m is BaseCreature && ((BaseCreature)m).Controlled))
					targets.Add(m);

			foreach (Mobile m in targets)
			{
				AOS.Damage(m, this, 80, 0, 100, 0, 0, 0);
				MovingParticles(m, 0x36D4, 7, 0, false, true, 9502, 4019, 0x160);
				PlaySound(0x15E);
				m.SendMessage("Your body is scorched by a massive fireball.");
			}
		}

		private void Dispel()
		{
			ArrayList targets = new ArrayList();

			foreach (Mobile m in GetMobilesInRange(8))
			{
				if (this != m && m is BaseCreature && ((BaseCreature)m).Summoned)
					targets.Add(m);
			}

			if (targets.Count > 0)
			{
				PlaySound(0x299);
				FixedParticles(0x37C4, 1, 25, 9922, 14, 3, EffectLayer.Head);

				for (int i = 0; i < targets.Count; ++i)
				{
					Mobile m = (Mobile)targets[i];
					BaseCreature bc = m as BaseCreature;

					if (bc != null)
					{
						if (bc.Summoned)
						{
							Effects.SendLocationParticles(EffectItem.Create(m.Location, m.Map, EffectItem.DefaultDuration), 0x3728, 8, 20, 5042);
							Effects.PlaySound(m, m.Map, 0x201);

							m.Delete();
						}
					}
				}
			}
		}

		private void Lightning()
		{
			ArrayList targets = new ArrayList();

			foreach (Mobile m in GetMobilesInRange(16))
				if (m != this && CanBeHarmful(m) && CanSee(m) && (m.Player || m is BaseCreature && ((BaseCreature)m).Controlled))
					targets.Add(m);

			foreach (Mobile m in targets)
			{

				m.BoltEffect(0);
				AOS.Damage(m, this, 100, 0, 0, 0, 100, 0);
				m.SendMessage("A huge lightning bolt strikes your body.");
			}
		}

		private void Pull()
		{
			Mobile topull = null;

			foreach (Mobile m in GetMobilesInRange(16))
			{
				if (m != this && m.Player && CanBeHarmful(m) && m.Alive && !m.Hidden)
				{
					topull = m;
					break;
				}
			}

			if (topull != null)
			{
				Effects.SendLocationParticles(EffectItem.Create(this.Location, Map, EffectItem.DefaultDuration), 0x3728, 10, 10, 2023);
				Effects.SendLocationParticles(EffectItem.Create(topull.Location, Map, EffectItem.DefaultDuration), 0x3728, 10, 10, 5023);
				topull.PlaySound(0x1FE);
				topull.MoveToWorld(Location, Map);
				Combatant = topull;
			}
		}

		public override void OnDelete()
		{
			base.OnDelete();

			if (m_SpawnList != null)
				foreach (Mobile m in m_SpawnList)
					if (m != null)
						m.Delete();
		}

		public QuestMobile(Serial serial)
			: base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write((int)0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadInt();
		}

		private class GoldSpawnTimer : Timer
		{
			private Map m_Map;
			private int m_X, m_Y;

			public GoldSpawnTimer(Map map, int x, int y)
				: base(TimeSpan.FromSeconds(Utility.RandomDouble() * 10.0))
			{
				m_Map = map;
				m_X = x;
				m_Y = y;
			}

			protected override void OnTick()
			{
				int z = m_Map.GetAverageZ(m_X, m_Y);
				bool canFit = m_Map.CanFit(m_X, m_Y, z, 6, false, false);

				for (int i = -3; !canFit && i <= 3; ++i)
				{
					canFit = m_Map.CanFit(m_X, m_Y, z + i, 6, false, false);

					if (canFit)
						z += i;
				}

				if (!canFit)
					return;

				Gold g = new Gold(300, 500);

				g.MoveToWorld(new Point3D(m_X, m_Y, z), m_Map);

				if (0.5 >= Utility.RandomDouble())
				{
					switch (Utility.Random(3))
					{
						case 0: // Fire column
							{
								Effects.SendLocationParticles(EffectItem.Create(g.Location, g.Map, EffectItem.DefaultDuration), 0x3709, 10, 30, 5052);
								Effects.PlaySound(g, g.Map, 0x208);
								break;
							}
						case 1: // Explosion
							{
								Effects.SendLocationParticles(EffectItem.Create(g.Location, g.Map, EffectItem.DefaultDuration), 0x36BD, 20, 10, 5044);
								Effects.PlaySound(g, g.Map, 0x307);
								break;
							}
						case 2: // Ball of fire
							{
								Effects.SendLocationParticles(EffectItem.Create(g.Location, g.Map, EffectItem.DefaultDuration), 0x36FE, 10, 10, 5052);
								break;
							}
					}
				}
			}
		}
	}
}