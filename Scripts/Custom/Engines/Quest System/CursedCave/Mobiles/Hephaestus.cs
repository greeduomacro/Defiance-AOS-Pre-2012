using System;
using System.Collections.Generic;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	public class Hephaestus : BaseCreature
	{
		public override bool AlwaysMurderer { get { return true; } }
		public override Poison PoisonImmune { get { return Poison.Lethal; } }

		public override bool HasBreath { get { return true; } }
		public override int BreathFireDamage { get { return 100; } }

		public override int BreathComputeDamage()
		{
			return Utility.RandomMinMax(60, 80);
		}

		private static Type[] m_Rewards = new Type[]
			{
				typeof( HearthOfHomeFire ),
				typeof( HolySword ),
				typeof( LeggingsOfEmbers ),
				typeof( RoseOfTrinsic ),
				typeof( SamuraiHelm ),
				typeof( ShaminoCrossbow ),
				typeof( TapestryOfSosaria )
			};

		public static Item CreateRandomArtifact()
		{
			return Loot.Construct(m_Rewards[Utility.Random(m_Rewards.Length)]);
		}

		[Constructable]
		public Hephaestus()
			: base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
		{
			Name = "Hephaestus";
			Title = "the god of fire";

			Body = 15;
			BaseSoundID = 838;

			SetStr(505, 1000);
			SetDex(102, 300);
			SetInt(402, 600);

			SetHits(10000);
			SetStam(500, 600);

			SetDamage(25, 30);

			SetDamageType(ResistanceType.Fire, 80);
			SetDamageType(ResistanceType.Physical, 20);

			SetResistance(ResistanceType.Physical, 75, 80);
			SetResistance(ResistanceType.Fire, 100);
			SetResistance(ResistanceType.Cold, 60, 70);
			SetResistance(ResistanceType.Poison, 60, 70);
			SetResistance(ResistanceType.Energy, 60, 70);

			SetSkill(SkillName.MagicResist, 70.7, 140.0);
			SetSkill(SkillName.Tactics, 97.6, 100.0);
			SetSkill(SkillName.Wrestling, 97.6, 100.0);

			Fame = 22500;
			Karma = -22500;

			VirtualArmor = 80;

			AddItem(new LightSource());
		}

		public override int GetAngerSound()
		{
			return 0x567;
		}

		public override void OnGaveMeleeAttack(Mobile defender)
		{
			base.OnGaveMeleeAttack(defender);

			if (0.1 > Utility.RandomDouble())
			{
				List<Mobile> targets = new List<Mobile>();
				foreach (Mobile m in defender.GetMobilesInRange(8))
				{
					if (m == this || m.AccessLevel > AccessLevel.Player || !CanBeHarmful(m) || !this.InLOS(m))
						continue;

					if (m is BaseCreature && (((BaseCreature)m).Controlled || ((BaseCreature)m).Summoned || ((BaseCreature)m).Team != this.Team))
						targets.Add(m);
					else if (m.Player && m.Alive)
						targets.Add(m);
				}

				for (int i = 0; i < targets.Count; ++i)
					ESpecialAbilities.BeginLifeDrain(targets[i], this);
			}
		}

		public override void BreathDealDamage(Mobile target)
		{
			base.BreathDealDamage(target);

			Effects.PlaySound(target.Location, target.Map, 0x1CA);
			new BasePool("lava", 0x1B5, 30, 35, 0, 100, 0, 0, 0).MoveToWorld(target.Location, target.Map);
		}

		public override void GenerateLoot()
		{
			AddLoot(LootPack.UltraRich, 4);

			if (!m_Spawning)
			{
				for (int i = 0; i < 5; ++i)
					AddItem(new SulfurousAsh(30));
			}
		}

		public override void OnDeath(Container c)
		{
			base.OnDeath(c);

			// 5,8% at 1200luck
			if ((0.03 + (LootPack.GetLuckChanceForKiller(this) / 180000)) > Utility.RandomDouble())
				DemonKnight.DistributeArtifact(this, CreateRandomArtifact());
		}

		public override int TreasureMapLevel { get { return 5; } }

		public Hephaestus(Serial serial)
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
	}
}