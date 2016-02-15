using System;
using System.Collections;
using Server.Items;
using Server.ContextMenus;
using Server.Misc;
using Server.Network;
using Server.Engines.CannedEvil;

namespace Server.Mobiles
{
	public class DevaTheCursedOne : BaseCursedCreature
	{
		public override bool UseRearm { get { return true; } }
		public override Poison PoisonImmune { get { return Poison.Lethal; } }
		public override int TreasureMapLevel { get { return 5; } }
		public override bool AlwaysMurderer { get { return true; } }
		private DateTime m_TimeToCanSpawn;

		[Constructable]
		public DevaTheCursedOne()
			: base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
		{
			Name = "Deva";
			Title = "The Cursed One";
			Female = true;
			Body = 0x191;

			SetStr(200, 300);
			SetDex(81, 95);
			SetInt(61, 75);

			SetHits(1000);

			Fame = NotorietyHandlers.GetNotorietyByLevel( 4 );
			Karma = NotorietyHandlers.GetNotorietyByLevel( -4 );

			SetSkill(SkillName.Swords, 120.0);
			SetSkill(SkillName.Tactics, 120.0);
			SetSkill(SkillName.Wrestling, 120.0);
			SetSkill(SkillName.Anatomy, 120.0);
			SetSkill(SkillName.Parry, 100.0);
			SetSkill(SkillName.Healing, 90.0);

			SetResistance(ResistanceType.Physical, 40);
			SetResistance(ResistanceType.Cold, 40);
			SetResistance(ResistanceType.Poison, 30);
			SetResistance(ResistanceType.Energy, 40);
			SetResistance(ResistanceType.Fire, 40);

			AddItem(CursedCaveUtility.MutateItem(new BoneLegs(), 10));
			AddItem(CursedCaveUtility.MutateItem(new BoneArms(), 10));
			AddItem(CursedCaveUtility.MutateItem(new BoneGloves(), 10));
			AddItem(CursedCaveUtility.MutateItem(new BoneChest(), 10));
			AddItem(CursedCaveUtility.MutateItem(Loot.RandomJewelry(), 10));

			AddItem(new Sandals());
			AddItem(CursedCaveUtility.MutateItem(new DeerMask(), 10));

			Item wep = CursedCaveUtility.MutateItem(new Scythe(), 10);
			wep.Movable = false;
			AddItem(wep);

			new CursedSteed().Rider = this;
			SetDelay(TimeSpan.Zero);
		}

		public override void OnDamage(int amount, Mobile attacker, bool willKill)
		{
			base.OnDamage(amount, attacker, willKill);

			if (Hits < (HitsMax / 2) && 0.20 > Utility.RandomDouble() && !willKill)
			{
				if (DateTime.Now > m_TimeToCanSpawn)
				{
					this.PublicOverheadMessage(MessageType.Spell, 0, true, "* Screams for help! *");
					Timer.DelayCall(TimeSpan.FromSeconds(1.5), TimeSpan.FromSeconds(5.0), Utility.RandomMinMax(1, 4), new TimerCallback(SpawnRandom));
					SetDelay(TimeSpan.FromMinutes(2.0));
				}
			}
		}

		private void SetDelay(TimeSpan delay)
		{
			m_TimeToCanSpawn = DateTime.Now + delay;
		}

		public Point3D GetSpawnPosition(int iRange)
		{
			Map map = Map;

			if (map == null)
				return Location;

			for (int i = 0; i < 10; i++)
			{
				int x = Location.X + (Utility.Random((iRange * 2) + 1) - iRange);
				int y = Location.Y + (Utility.Random((iRange * 2) + 1) - iRange);
				int z = Map.GetAverageZ(x, y);

				if (Map.CanSpawnMobile(new Point2D(x, y), this.Z))
					return new Point3D(x, y, this.Z);
				else if (Map.CanSpawnMobile(new Point2D(x, y), z))
					return new Point3D(x, y, z);
			}
			return this.Location;
		}

		private void SpawnRandom()
		{
			Point3D loc = Location;
			BaseCreature creature = null;

			switch (Utility.Random(5))
			{
				case 0:
				case 1: creature = new TheCursed(); break;
				case 2: creature = new DreadSpider(); break;
				case 3: creature = new HellHound(); break;
				case 4: creature = new PredatorHellCat(); break;
			}

			Effects.SendLocationParticles(EffectItem.Create(loc, Map, EffectItem.DefaultDuration), 0x3728, 10, 10, 5023);
			Effects.PlaySound(loc, Map, 0x1FE);
			creature.MoveToWorld(loc, Map);
		}

		public override void GenerateLoot()
		{
			base.GenerateLoot();

			AddLoot(LootPack.AosSuperBoss);
			AddLoot(LootPack.HighScrolls);
			AddLoot(LootPack.Potions);
		}

		public DevaTheCursedOne(Serial serial)
			: base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write((int)0); // version
			writer.Write((DateTime)m_TimeToCanSpawn);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();
			m_TimeToCanSpawn = reader.ReadDateTime();
		}
	}
}