using System;
using Server.Misc;
using Server.Network;
using System.Collections;
using System.Collections.Generic;
using Server.Items;
using Server.Targeting;
using Server.Engines.CannedEvil;

namespace Server.Mobiles
{
	[CorpseName( "The Harvester's corpse" )]
	public class Harvester : BaseCreature
	{
		public override bool AutoDispel { get { return true; } }

		[Constructable]
		public Harvester():base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Body = 792;
			Name = "The Harvester";
			this.Hue = 30000;
			SetStr( 450, 500 );
			SetDex( 350, 390 );
			SetInt( 950, 1000 );
			SetHits( 24000 );
			SetSkill( SkillName.Wrestling, 100, 120 );
			SetSkill( SkillName.Tactics, 100 );
			SetSkill( SkillName.MagicResist, 150);
			SetSkill( SkillName.Meditation, 120 );
			SetSkill(SkillName.Necromancy, 100);
			SetSkill(SkillName.SpiritSpeak, 120, 140);

			SetDamageType(ResistanceType.Physical, 100);

			SetDamage(15, 20);

			SetResistance(ResistanceType.Physical, 40, 55);
			SetResistance(ResistanceType.Fire, 48, 65);
			SetResistance(ResistanceType.Cold, 55, 60);
			SetResistance(ResistanceType.Poison, 60, 62);
			SetResistance(ResistanceType.Energy, 30, 35);

			VirtualArmor = 45;
			Fame = 22500;
			Karma = -25000;
		}

		public override bool AlwaysMurderer{ get{ return true; } }
		public override bool Unprovokable{ get{ return true; } }
		public override Poison PoisonImmune { get { return Poison.Regular; } }

		public override WeaponAbility GetWeaponAbility()
		{
			switch ( Utility.Random( 5 ) )
			{
				default:
				case 0: return WeaponAbility.DoubleStrike;
				case 1: return WeaponAbility.WhirlwindAttack;
				case 2: return WeaponAbility.CrushingBlow;
				case 3: return WeaponAbility.MortalStrike;
				case 4: return WeaponAbility.BleedAttack;
			}
		}

		public override void GenerateLoot()
		{
			AddLoot(LootPack.SuperBoss, 2);
			AddLoot(LootPack.AosFilthyRich, 2);
			if (!this.m_Spawning)
			{
				ArrayList toGive = new ArrayList();
				List<DamageStore> rights = BaseCreature.GetLootingRights(this.DamageEntries, this.HitsMax);

				for (int i = rights.Count - 1; i >= 0; --i)
				{
					DamageStore ds = rights[i];

					if (ds.m_HasRight)
						toGive.Add(ds.m_Mobile);
				}

				if (toGive.Count == 0)
					return;

				// Randomize
				for (int i = 0; i < toGive.Count; ++i)
				{
					int rand = Utility.Random(toGive.Count);
					object hold = toGive[i];
					toGive[i] = toGive[rand];
					toGive[rand] = hold;
				}

				for (int i = 0; i < toGive.Count; ++i)
				{
					Mobile m = (Mobile)toGive[i % toGive.Count];

					if (!this.m_Spawning)
					{
						if (0.03 > Utility.RandomDouble())
						{
							m.SendMessage("For your valour in defeating The Harvester you have received a reward");
							m.AddToBackpack(new PlantRare());
						}
					}
				}
			}
		}

		public void DrainLife()
		{
			ArrayList list = new ArrayList();

			foreach (Mobile m in this.GetMobilesInRange(2))
			{
				if (m == this || !CanBeHarmful(m))
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

				int toDrain = Utility.RandomMinMax(10, 40);

				Hits += toDrain;
				m.Damage(toDrain, this);
			}
		}

		public override void OnGaveMeleeAttack(Mobile defender)
		{
			base.OnGaveMeleeAttack(defender);
			if (0.25 >= Utility.RandomDouble())
				DrainLife();
		}

		public override void AlterMeleeDamageTo(Mobile from, ref int damage)
		{
			if (from is BaseCreature && ((BaseCreature)from).Controlled && !((BaseCreature)from).Summoned)
			  damage = damage * 3;
		}

		public void SpawnLesser(Mobile m)
		{
			Map map = this.Map;

			if (map == null)
				return;

			Bogling bl = new Bogling();
			bl.Hue = 30000;
			bl.Name = "Lesser Harvester";
			bl.HitsMaxSeed = 200;
			bl.Hits = 200;
			bl.Team = this.Team;

			bool validLocation = false;
			Point3D loc = this.Location;

			for (int j = 0; !validLocation && j < 10; ++j)
			{
				int x = X + Utility.Random(3) - 1;
				int y = Y + Utility.Random(3) - 1;
				int z = map.GetAverageZ(x, y);

				if (validLocation = map.CanFit(x, y, this.Z, 16, false, false))
					loc = new Point3D(x, y, Z);
				else if (validLocation = map.CanFit(x, y, z, 16, false, false))
					loc = new Point3D(x, y, z);
			}

			bl.MoveToWorld(loc, map);
			bl.Combatant = m;
		}

		public override void OnGotMeleeAttack(Mobile attacker)
		{
			base.OnGotMeleeAttack(attacker);
			if (this.Hits < (this.HitsMax / 5))
			{
				if (0.05 >= Utility.RandomDouble())
					SpawnLesser(attacker);
			}
		}

		public Harvester( Serial serial ) : base( serial )
		{
		}

		public override int GetDeathSound()
		{
			return 0x108;
		}

		public override void OnDeath(Container c)
		{
			base.OnDeath(c);

			if (/*Utility.RandomDouble() < .5 &&*/ !Summoned && !NoKillAwards && DemonKnight.CheckArtifactChance(this))
				DemonKnight.DistributeArtifact(this);
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}

	}
}