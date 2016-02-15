using System;
using System.Collections;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a plagued corpse" )]
	public class ThePlague : BaseCreature
	{
		public override bool HasBreath{ get{ return true; } }
		public override int BreathPoisonDamage{ get{ return 100; } }
		public override int BreathEffectItemID{ get{ return 0x36D4; } }
		public override int BreathEffectHue{ get{ return 0x3F; } }
		public override int BreathEffectSound{ get{ return 0x118; } }

		private static Type[] m_Rewards = new Type[]
			{
				typeof( BookOfLostKnowledge ),
				typeof( BowOfWeakening ),
				typeof( GlovesOfTheLepracon ),
				typeof( GrizzlysCourage )
			};

		public static Item CreateRandomArtifact()
		{
			return Loot.Construct( m_Rewards[Utility.Random(m_Rewards.Length)] );
		}

		[Constructable]
		public ThePlague() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.1, 0.2 )
		{
			Name = "The Plague";
			Body = 0x307;
			Hue = 0x232;

			SetStr( 500 );
			SetDex( 100 );
			SetInt( 1000 );

			SetHits( 30000 );
			SetMana( 5000 );

			SetDamage( 17, 21 );

			SetDamageType( ResistanceType.Physical, 20 );
			SetDamageType( ResistanceType.Fire, 20 );
			SetDamageType( ResistanceType.Cold, 20 );
			SetDamageType( ResistanceType.Poison, 20 );
			SetDamageType( ResistanceType.Energy, 20 );

			SetResistance( ResistanceType.Physical, 30 );
			SetResistance( ResistanceType.Fire, 30 );
			SetResistance( ResistanceType.Cold, 30 );
			SetResistance( ResistanceType.Poison, 30 );
			SetResistance( ResistanceType.Energy, 30 );

			SetSkill( SkillName.EvalInt, 100.0 );
			SetSkill( SkillName.SpiritSpeak, 100.0 );
			SetSkill( SkillName.Necromancy, 100.0 );
			SetSkill( SkillName.Meditation, 100.0 );
			SetSkill( SkillName.MagicResist, 100.0 );
			SetSkill( SkillName.Tactics, 100.0 );
			SetSkill( SkillName.Wrestling, 100.0 );
			SetSkill( SkillName.Macing, 100.0 );
			SetSkill( SkillName.Anatomy, 100.0 );

			Fame = 22500;
			Karma = -22500;

			VirtualArmor = 30;
		}

		public override void BreathDealDamage( Mobile target )
		{
			base.BreathDealDamage( target );

			Effects.PlaySound( target.Location, target.Map, 0x1CA );
			//new CustomPool("acid", 0x3F, 10, 15, 0, 0, 0, 100, 0).MoveToWorld( target.Location, target.Map );
		}

		public override int BreathComputeDamage()
		{
			return 50;
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.SuperBoss, 2 );
		}

		public override bool AlwaysMurderer{ get{ return true; } }
		public override bool BardImmune{ get{ return true; } }
		public override Poison PoisonImmune{ get{ return Poison.Lethal; } }

		public override int TreasureMapLevel{ get{ return 5; } }
		public override bool ShowFameTitle{ get{ return false; } }
		public override bool ClickTitle{ get{ return false; } }

		public override void OnGaveMeleeAttack( Mobile defender )
		{
			base.OnGaveMeleeAttack( defender );

			if ( 0.1 >= Utility.RandomDouble() )
				AddPlaguedSkull( defender, 0.25 );
		}

/* 		public override void OnGotMeleeAttack( Mobile attacker )
		{
			base.OnGotMeleeAttack( attacker );

			if ( 0.1 >= Utility.RandomDouble() )
				AddPlaguedSkull( attacker, 0.25 );
		} */

		public override void OnDamage( int amount, Mobile from, bool willKill )
		{
			base.OnDamage( amount, from, willKill );
			if ( 0.1 >= Utility.RandomDouble() )
				AddPlaguedSkull( from, 0.25 );
		}

		public override void AlterDamageScalarFrom( Mobile caster, ref double scalar )
		{
			base.AlterDamageScalarFrom( caster, ref scalar );

			if ( 0.1 >= Utility.RandomDouble() )
				AddPlaguedSkull( caster, 1.0 );
		}

		public override void OnDeath( Container c )
		{
			base.OnDeath( c );

			if ( (0.03 + ( LootPack.GetLuckChanceForKiller( this ) / 180000) ) > Utility.RandomDouble() )
				DemonKnight.DistributeArtifact( this, CreateRandomArtifact() );
		}

		public override void AlterMeleeDamageTo( Mobile to, ref int damage )
		{
			if( to is BaseCreature && ((BaseCreature)to).Controlled && !((BaseCreature)to).Summoned )
				damage *= 2;
		}

		public void AddPlaguedSkull( Mobile target, double chanceToThrow )
		{
			if ( chanceToThrow >= Utility.RandomDouble() )
			{
				Direction = GetDirectionTo( target );
				MovingEffect( target, 0xF7E, 10, 1, true, false, 0x496, 0 );
				new DelayTimer( this, target ).Start();
			}
			else
			{
				new PlaguedSkull().MoveToWorld( Location, Map );
			}
		}

		private class DelayTimer : Timer
		{
			private Mobile m_Mobile;
			private Mobile m_Target;

			public DelayTimer( Mobile m, Mobile target ) : base( TimeSpan.FromSeconds( 1.0 ) )
			{
				m_Mobile = m;
				m_Target = target;
			}

			protected override void OnTick()
			{
				if ( m_Mobile.CanBeHarmful( m_Target ) )
				{
					m_Mobile.DoHarmful( m_Target );
					AOS.Damage( m_Target, m_Mobile, Utility.RandomMinMax( 10, 20 ), 100, 0, 0, 0, 0 );
					new PlaguedSkull().MoveToWorld( m_Target.Location, m_Target.Map );
				}
			}
		}

		public ThePlague( Serial serial ) : base( serial )
		{
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