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
	[CorpseName("The Remains of the Defended One")]
	public class RoyaltyDefender : BaseCreature
	{
		private Timer m_Timer;
		public int level = 1;
		public override bool ShowFameTitle{ get{ return true; } }
		bool dispel = false;
		public override bool AutoDispel { get { return dispel; } }

		[Constructable]
		public RoyaltyDefender():base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Body = 400;
			Name = "The Defender of Royalty";

			SetStr( 180, 220 );
			SetDex( 120, 140 );
			SetInt( 600, 700 );
			SetHits(1200);
			SetSkill(SkillName.Macing, 100);
			SetSkill(SkillName.Tactics, 100);
			SetSkill(SkillName.MagicResist, 120);
			SetSkill(SkillName.Magery, 100);
			SetSkill(SkillName.EvalInt, 100);
			SetSkill(SkillName.Meditation, 100);

			SetDamage(10, 12);

			SetResistance(ResistanceType.Physical, 24, 25);
			SetResistance(ResistanceType.Fire, 24, 25);
			SetResistance(ResistanceType.Cold, 24, 25);
			SetResistance(ResistanceType.Poison, 24, 25);
			SetResistance(ResistanceType.Energy, 24, 25);

			SetDamageType( ResistanceType.Physical, 100 );

			SetFameLevel( 0 );
			SetKarmaLevel( 0 );
			SetClothes(0);
		}

		public void DefenderPrince()
		{
			Body = 400;
			Name = "The Defender Prince";
			SetStr(240, 260);
			SetDex(270, 300);
			SetInt(450, 470);
			SetHits(2000);
			SetSkill(SkillName.Macing, 100);
			SetSkill(SkillName.Tactics, 100);
			SetSkill(SkillName.MagicResist, 120);
			SetSkill(SkillName.Magery, 100);
			SetSkill(SkillName.EvalInt, 100);
			SetSkill(SkillName.Meditation, 100);

			SetDamageType( ResistanceType.Fire, 100 );
			SetDamageType(ResistanceType.Physical, 0);

			SetDamage(10, 12);

			SetResistance(ResistanceType.Physical, 30, 31);
			SetResistance(ResistanceType.Fire, 30, 31);
			SetResistance(ResistanceType.Cold, 30, 31);
			SetResistance(ResistanceType.Poison, 30, 31);
			SetResistance(ResistanceType.Energy, 30, 31);

			SetFameLevel( 13000 );
			SetKarmaLevel( -14000 );
			SetClothes(1161);
		}

		public void DefenderQueen(){
			Body = 184;
			Name = "The Defender Queen";
			SetStr(260, 280);
			SetDex(320, 340);
			SetInt(500, 520);
			SetHits( 4000 );
			SetSkill( SkillName.Macing, 100);
			SetSkill( SkillName.Tactics, 100);
			SetSkill( SkillName.MagicResist, 120);
			SetSkill( SkillName.Magery, 100);
			SetSkill( SkillName.EvalInt, 100 );
			SetSkill( SkillName.Meditation, 100 );

			SetDamageType(ResistanceType.Cold, 100);
			SetDamageType(ResistanceType.Physical, 0);

			SetDamage(11, 13);

			SetResistance(ResistanceType.Physical, 35, 36);
			SetResistance(ResistanceType.Fire, 35, 36);
			SetResistance(ResistanceType.Cold, 35, 36);
			SetResistance(ResistanceType.Poison, 35, 36);
			SetResistance(ResistanceType.Energy, 35, 36);

			SetFameLevel( 15000 );
			SetKarmaLevel( 0 );
			SetClothes(1194);
		}

		public void DefenderKing(){
			Body = 183;
			Name = "The Defender King";
			SetStr(280, 300);
			SetDex(340, 360);
			SetInt(550, 570);
			SetHits(7000);
			SetSkill(SkillName.Macing, 100);
			SetSkill(SkillName.Tactics, 100);
			SetSkill(SkillName.MagicResist, 120);
			SetSkill(SkillName.Magery, 100);
			SetSkill(SkillName.EvalInt, 100);
			SetSkill(SkillName.Meditation, 100);

			SetDamageType(ResistanceType.Energy, 100);
			SetDamageType(ResistanceType.Physical, 0);

			SetDamage(11, 13);

			SetResistance(ResistanceType.Physical, 37, 43);
			SetResistance(ResistanceType.Fire, 37, 43);
			SetResistance(ResistanceType.Cold, 37, 43);
			SetResistance(ResistanceType.Poison, 37, 43);
			SetResistance(ResistanceType.Energy, 37, 43);

			SetFameLevel( 18000 );
			SetKarmaLevel( -18000 );
			SetClothes(0x4001);
		}

		public void DefendedOne(){
			dispel = true;

			Body = 174;
			Name = "The Defended One";
			this.Hue = 2002;
			SetStr( 280, 300 );
			SetDex( 380, 400 );
			SetInt( 600, 620 );
			SetHits( 20000 );
			SetSkill( SkillName.Wrestling, 100 );
			SetSkill( SkillName.Magery, 120 );
			SetSkill( SkillName.EvalInt, 150 );
			SetSkill(SkillName.Meditation, 120.0);
			SetSkill(SkillName.Tactics, 100.0);
			SetSkill(SkillName.Wrestling, 120.0);

			SetDamageType( ResistanceType.Poison, 100 );
			SetDamageType(ResistanceType.Physical, 0);

			SetDamage(12, 15);

			SetResistance(ResistanceType.Physical, 40, 55);
			SetResistance(ResistanceType.Fire, 25, 35);
			SetResistance(ResistanceType.Cold, 40, 45);
			SetResistance(ResistanceType.Poison, 70, 70);
			SetResistance(ResistanceType.Energy, 50, 50);

			VirtualArmor = 50;
			SetFameLevel( 4 );
			SetKarmaLevel( 4 );

			m_Timer = new TeleportTimer(this);
			m_Timer.Start();
		}

		public void SetClothes(int hue)
		{
			this.Hue = hue;
			LeatherGloves gloves = new LeatherGloves();
			gloves.Hue = hue;
			gloves.Movable = false;
			AddItem(gloves);

			FancyShirt fancyShirt = new FancyShirt();
			fancyShirt.Hue = hue;
			fancyShirt.Movable = false;
			AddItem(fancyShirt);

			BoneHelm helm = new BoneHelm();
			helm.Hue = hue;
			helm.Movable = false;
			AddItem(helm);

			BlackStaff blackStaff = new BlackStaff();
			blackStaff.Hue = hue;
			blackStaff.Movable = false;
			blackStaff.Attributes.SpellChanneling = 1;
			AddItem(blackStaff);

			Cloak cloak = new Cloak();
			cloak.Hue = hue;
			cloak.Movable = false;
			AddItem(cloak);

			Kilt kilt = new Kilt();
			kilt.Hue = hue;
			kilt.Movable = false;
			AddItem(kilt);

			Sandals sandals = new Sandals();
			sandals.Hue = hue;
			sandals.Movable = false;
			AddItem(sandals);
		}

		public override bool AlwaysMurderer{ get{ return true; } }
		public override bool Unprovokable{ get{ return true; } }

		public RoyaltyDefender( Serial serial ) : base( serial )
		{
		}

		public override int GetAngerSound()
		{
			return 0x175;
		}

		public override int GetHurtSound()
		{
			return 0x28B;
		}

		public override void AlterMeleeDamageTo(Mobile from, ref int damage)
		{
			if (from is BaseCreature && ((BaseCreature)from).Controlled && !((BaseCreature)from).Summoned)
			   damage = damage * 3;
		}

		public override bool OnBeforeDeath()
		{
			this.PlaySound(0x1FC);
			Effects.SendLocationEffect(Location, Map, 0x3709, 13, 0x3B2, 0);
			switch (level)
			{
				case 1:
					DefenderPrince();
					level = 2;
					return false;
				case 2:
					DefenderQueen();
					level = 3;
					return false;
				case 3:
					DefenderKing();
					level = 4;
					return false;
				case 4:
					DefendedOne();
					level = 5;
					return false;
				case 5:
					GenerateLoot();
					return true;
			   default:
					return true;
			}
		}

		public override void GenerateLoot()
		{
			AddLoot(LootPack.SuperBoss, 2);
			AddLoot(LootPack.AosFilthyRich, 2);

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
						m.SendMessage("For your valour in defeating the Defended One you have received a reward"); // You have received a scroll of power!
						m.AddToBackpack(new RoyalLoot());
					}
				}
			}
		}

		public override void OnDeath(Container c)
		{
			base.OnDeath(c);

			if (/*Utility.RandomDouble() < .5 &&*/ !Summoned && !NoKillAwards && DemonKnight.CheckArtifactChance(this))
				DemonKnight.DistributeArtifact(this);
		}

		private class TeleportTimer : Timer
		{
			private Mobile m_Owner;

			private static int[] m_Offsets = new int[]
			{
				-1, -1,
				-1,  0,
				-1,  1,
				0, -1,
				0,  1,
				1, -1,
				1,  0,
				1,  1
			};

			public TeleportTimer(Mobile owner)
				: base(TimeSpan.FromSeconds(5.0), TimeSpan.FromSeconds(5.0))
			{
				m_Owner = owner;
			}

			protected override void OnTick()
			{
				if (m_Owner.Deleted)
				{
					Stop();
					return;
				}

				Map map = m_Owner.Map;

				if (map == null)
					return;

				if (0.33 < Utility.RandomDouble())
					return;

				Mobile toTeleport = null;

				foreach (Mobile m in m_Owner.GetMobilesInRange(16))
				{
					if (m != m_Owner && m.Player && m_Owner.CanBeHarmful(m) && m_Owner.CanSee(m))
					{
						toTeleport = m;
						break;
					}
				}

				if (toTeleport != null)
				{
					int offset = Utility.Random(8) * 2;

					Point3D to = m_Owner.Location;

					for (int i = 0; i < m_Offsets.Length; i += 2)
					{
						int x = m_Owner.X + m_Offsets[(offset + i) % m_Offsets.Length];
						int y = m_Owner.Y + m_Offsets[(offset + i + 1) % m_Offsets.Length];

						if (map.CanSpawnMobile(x, y, m_Owner.Z))
						{
							to = new Point3D(x, y, m_Owner.Z);
							break;
						}
						else
						{
							int z = map.GetAverageZ(x, y);

							if (map.CanSpawnMobile(x, y, z))
							{
								to = new Point3D(x, y, z);
								break;
							}
						}
					}

					Mobile m = toTeleport;

					Point3D from = m.Location;

					m.Location = to;

					Server.Spells.SpellHelper.Turn(m_Owner, toTeleport);
					Server.Spells.SpellHelper.Turn(toTeleport, m_Owner);

					m.ProcessDelta();

					Effects.SendLocationParticles(EffectItem.Create(from, m.Map, EffectItem.DefaultDuration), 0x3728, 10, 10, 2023);
					Effects.SendLocationParticles(EffectItem.Create(to, m.Map, EffectItem.DefaultDuration), 0x3728, 10, 10, 5023);

					m.PlaySound(0x1FE);

					m_Owner.Combatant = toTeleport;
				}
			}
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