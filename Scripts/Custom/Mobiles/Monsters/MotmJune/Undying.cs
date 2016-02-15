using System;
using System.Collections;
using Server;
using Server.Network;
using Server.Misc;
using Server.Items;
using Server.Engines.RewardSystem;

namespace Server.Mobiles
{
	public class undying : BaseCreature
	{
		public override double WeaponAbilityChance { get { return 0.4; } }
		public override WeaponAbility GetWeaponAbility()
		{
			if (Weapon is BaseWeapon)
			{
				BaseWeapon wep = (BaseWeapon)Weapon;
				return wep.SecondaryAbility;
			}
			return base.GetWeaponAbility();
		}

		[Constructable]
		public undying() : base( AIType.AI_Necro, FightMode.Closest, 10, 1, 0.1, 0.2 )
		{
			Name = "a dark knight";
			Body = 401;
			Female = true;
			Hue = 16800;

			SetStr( 201, 205 );
			SetDex( 91, 100 );
			SetInt( 1500, 1800 );

			SetHits( 9000, 10000 );

			SetDamage( 24, 25 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 45, 50 );
			SetResistance( ResistanceType.Fire, 45, 50 );
			SetResistance( ResistanceType.Cold, 45, 50 );
			SetResistance( ResistanceType.Poison, 45, 50 );
			SetResistance( ResistanceType.Energy, 45, 50 );

			SetSkill( SkillName.SpiritSpeak, 110.1, 125.0 );
			SetSkill( SkillName.Necromancy, 115.1, 120.0 );
			SetSkill( SkillName.EvalInt, 110.1, 120.0 );
			SetSkill( SkillName.Meditation, 90.1, 100.0 );
			SetSkill( SkillName.MagicResist, 95.0, 107.5 );
			SetSkill( SkillName.Anatomy, 110.2, 120.0 );
			SetSkill( SkillName.Tactics, 100.0, 110.5 );
			SetSkill( SkillName.Parry, 100.0, 110.5 );
			SetSkill( SkillName.Wrestling, 100.0, 110.5 );
			SetSkill( SkillName.Fencing, 110.2, 120.0 );

			Fame = 22000;
			Karma = -15000;

			VirtualArmor = 16;

			PackNecroReg( 8, 15 );
			AddItem( new HoodedShroudOfShadows( Utility.RandomNeutralHue() ) );
			AddItem( new Sandals() );
			AddItem( new Shirt( Utility.RandomNeutralHue() ) );
			AddItem( new Skirt( Utility.RandomNeutralHue() ) );

			WarFork weapon = new WarFork();

			weapon.Movable = false;
			weapon.Attributes.WeaponDamage = 45;
			weapon.Attributes.WeaponSpeed = 15;
			weapon.Attributes.AttackChance = 10;
			weapon.WeaponAttributes.HitHarm = 50;
			weapon.WeaponAttributes.HitFireball = 50;
			weapon.Hue = 163;

			AddItem( weapon );

			ChaosShield shield = new ChaosShield();

			shield.Movable = false;
			shield.Attributes.SpellChanneling = 1;
			shield.Attributes.DefendChance = 10;
			shield.Hue = 163;

			AddItem( shield );

			BaseMount horse = new Horse();
			horse.Rider = this;
			horse.Hue = 16800;
			horse.Tamable = false;
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.SuperBoss, 2 );
		}

		public override bool CanRummageCorpses{ get{ return true; } }
		public override Poison PoisonImmune{ get{ return Poison.Lethal; } }
		public override bool AlwaysMurderer{ get{ return true; } }
		public override bool ShowFameTitle{ get{ return false; } }

		public override void OnBeforeSpawn( Point3D location, Map m )
		{
			base.OnBeforeSpawn( location, m );

			IsParagon = false;
			IsElder = false;
			IsPlagued = false;
			Hue = 16800;
		}

		public undying( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}

		public void DrainLife()
		{
			ArrayList list = new ArrayList();

			foreach ( Mobile m in this.GetMobilesInRange( 2 ) )
			{
				if ( m == this || !CanBeHarmful( m ) )
					continue;

				if ( m is BaseCreature && (((BaseCreature)m).Controlled || ((BaseCreature)m).Summoned || ((BaseCreature)m).Team != this.Team) )
					list.Add( m );
				else if ( m.Player )
					list.Add( m );
			}

				foreach ( Mobile m in list )
				{
					DoHarmful( m );
					m.FixedParticles( 0x374A, 10, 15, 5013, 0x496, 0, EffectLayer.Waist );
					m.PlaySound( 0x231 );
					m.SendMessage( "You feel the life drain out of you!" );
					int toDrain = Utility.RandomMinMax( 30, 40 );
					Hits += toDrain;
					m.Damage( toDrain, this );
				}
		}

		public override void OnDamagedBySpell( Mobile caster )
		{
			if ( 0.15 > Utility.RandomDouble( ) )
				DrainLife();

			if ( this.Hue != 900 && 0.02 > Utility.RandomDouble( ) )
				this.Say( "Hahaha! You can't kill me with ordinary weapons! Didn't I tell you that I am immortal?" );

			base.OnDamagedBySpell( caster );
		}

		public override void OnGotMeleeAttack( Mobile attacker )
		{
			base.OnGotMeleeAttack( attacker );

			BaseWeapon weapon_attacker = attacker.Weapon as BaseWeapon;

			if ( 0.15 > Utility.RandomDouble( ) )
				DrainLife();

			if ( this.Hue != 900 && 0.02 > Utility.RandomDouble( ) )
				this.Say( "Hahaha! You can't kill me with ordinary weapons! Didn't I tell you that I am immortal?" );

			if ( weapon_attacker is UndeadsBane )
			{
				this.Hue = 900;
				this.FixedParticles( 0x3709, 10, 30, 5052, EffectLayer.LeftFoot );
				this.PublicOverheadMessage(MessageType.Regular, 0x0, true, "Nooooooooooo! The curse is lifted! You will pay for this, you moron!!!");
				weapon_attacker.Delete();
				attacker.SendMessage( "Your weapon broke after you hit the powerful creature!" );
			}
		}

		public void CreateLoot() {
			int chance = Utility.Random( 100 );
			Item m_Reward = null;
			if ( chance < 3 )
				m_Reward = new NameChangeDeed();
			else if ( chance < 33 )
				m_Reward = new PowerScroll( PowerScroll.Skills[Utility.Random(PowerScroll.Skills.Count)], (100+(Utility.Random(3)+2)*5) );
			else if ( chance < 50 )
				m_Reward = new StatCapScroll( (225+(Utility.Random(2)+1)*5) );
			else if ( chance < 55 )
				m_Reward = new MagicSewingKit();
			else if ( chance < 60 )
				m_Reward = new ClothingBlessDeed();
			else if ( chance < 70 )
				m_Reward = new RunicSewingKit(CraftResource.BarbedLeather, ( Utility.Random( 2 ) + 3 ) );
			else {
				int amount = Utility.RandomMinMax( 5, 10 )*10000;
				m_Reward = new BankCheck(amount);
			}

			PackItem( new CopperBar(Utility.Random( 2 ) + 2) );
			if (m_Reward != null)
			{
				PackItem( m_Reward );
				Timer m_TimerCursed = new CursedArtifactSystem.CursedTimer( m_Reward, 6 );
				m_TimerCursed.Start();
			}
		}

		public override bool OnBeforeDeath()
		{
			if ( this.Hue == 900 )
			{
				CreateLoot();
				return base.OnBeforeDeath();
			}
			else
			{
				this.Hits = 50;
				DrainLife();
				this.Say("Die mortals!! Ahahahahahaaaa!!!!!! Only a special weapon can kill me!");
				return false;
			}
		}

	}
}