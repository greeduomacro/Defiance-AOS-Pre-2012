using System;
using System.Collections;
using Server;
using Server.Misc;
using Server.Engines.RewardSystem;
using Server.Items;
using Server.Spells;

namespace Server.Mobiles
{

	public class Stalker : BaseCreature
	{

		private class SpawnEntry
		{
			public Point3D m_Location;

			public SpawnEntry( Point3D loc )
			{
				m_Location = loc;
			}
		}

		private static SpawnEntry[] m_Entries = new SpawnEntry[]
		{
				new SpawnEntry( new Point3D( 2051, 837, -28 ) ),
				new SpawnEntry( new Point3D( 2082, 867, -28 ) ),
				new SpawnEntry( new Point3D( 2114, 840, -28 ) ),
				new SpawnEntry( new Point3D( 2114, 888, -28 ) ),
				new SpawnEntry( new Point3D( 2058, 950, -28 ) ),
				new SpawnEntry( new Point3D( 2114, 954, -28 ) ),
				new SpawnEntry( new Point3D( 2174, 936, -28 ) ),
				new SpawnEntry( new Point3D( 2169, 887, -28 ) ),
				new SpawnEntry( new Point3D( 2140, 912, -28 ) ),
				new SpawnEntry( new Point3D( 2083, 915, -28 ) ),
				new SpawnEntry( new Point3D( 2144, 867, -28 ) ),
				new SpawnEntry( new Point3D( 2114, 995, -28 ) )
		};

		public override double WeaponAbilityChance { get { return 0.5; } }
		public override WeaponAbility GetWeaponAbility()
		{
			return WeaponAbility.ParalyzingBlow;
		}

		[Constructable]
		public Stalker() : base( AIType.AI_Stealth, FightMode.Closest, 10, 1, 0.1, 0.2 )
		{
			Name = "Selene the Stalker";
			Body = 401;
			Female = true;
			Hue = 900;

			SetStr( 81, 105 );
			SetDex( 140, 150 );
			SetInt( 900, 1000 );

			SetHits( 450, 500 );

			SetDamage( 14, 17 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 65, 70 );
			SetResistance( ResistanceType.Fire, 80, 85 );
			SetResistance( ResistanceType.Cold, 60, 65 );
			SetResistance( ResistanceType.Poison, 60, 65 );
			SetResistance( ResistanceType.Energy, 60, 65 );

			SetSkill( SkillName.Healing, 110.1, 120.0 );
			SetSkill( SkillName.EvalInt, 100.1, 110.0 );
			SetSkill( SkillName.Meditation, 90.1, 100.0 );
			SetSkill( SkillName.MagicResist, 95.0, 107.5 );
			SetSkill( SkillName.Anatomy, 110.2, 120.0 );
			SetSkill( SkillName.Tactics, 100.0, 110.5 );
			SetSkill( SkillName.Parry, 100.0, 110.5 );
			SetSkill( SkillName.Wrestling, 100.0, 110.5 );
			SetSkill( SkillName.Fencing, 180.2, 200.0 );
			SetSkill( SkillName.Hiding, 100.0, 100.0 );
			SetSkill( SkillName.Stealth, 120.0, 120.0 );

			Fame = 15000;
			Karma = -15000;

			VirtualArmor = -30;

			AddItem( new Bandana( 50 ) );
			AddItem( new Sandals() );

			LeatherBustierArms chest = new LeatherBustierArms();
			chest.Hue = 800;
			AddItem( chest );
			LeatherGloves gloves = new LeatherGloves();
			gloves.Hue = 800;
			AddItem( gloves );
			LeatherLegs legs = new LeatherLegs();
			legs.Hue = 800;
			AddItem( legs );

			Spear weapon = new Spear();

			weapon.Movable = false;
			weapon.Attributes.WeaponDamage = 150;
			weapon.Attributes.WeaponSpeed = 15;
			weapon.Attributes.AttackChance = 30;
			weapon.WeaponAttributes.HitLightning = 100;
			weapon.Hue = 16500;

			AddItem( weapon );

			Item hair = new Item( Utility.RandomList( 0x203B, 0x2049, 0x2048, 0x204A ) );
			hair.Hue = 700;
			hair.Layer = Layer.Hair;
			hair.Movable = false;
			AddItem( hair );
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.FilthyRich );
			AddLoot( LootPack.UltraRich );
		}

		public override bool IsEnemy( Mobile m )
		{

			if ( m.Hits > 200 )
				return false;

			return base.IsEnemy( m );
		}

		public override bool AlwaysMurderer{ get{ return true; } }
		public override bool ShowFameTitle{ get{ return false; } }

		public Stalker( Serial serial ) : base( serial )
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
			int damage = Utility.RandomMinMax( this.Kills+1, this.Kills+10 );

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
					int toDrain = damage;
					Hits += toDrain;
					m.Damage( toDrain, this );
				}
		}

		public override bool OnBeforeDeath()
		{
			EventRewardSystem.CreateCopperBar(this.Name, this.Backpack, 3, "Selene the Stalker quest");

			return base.OnBeforeDeath();
		}

		public override void OnGaveMeleeAttack( Mobile defender )
		{
			base.OnGaveMeleeAttack( defender );

			DrainLife();

			if ( !(defender.Alive) )
			{

			this.BoltEffect( 0 );
			this.BoltEffect( 0 );
			this.BoltEffect( 0 );
			this.Kills = this.Kills + 1;

			SpawnEntry entry = m_Entries[Utility.Random( m_Entries.Length )];
			this.Hidden = true;
			this.MoveToWorld( entry.m_Location, Map.Ilshenar );
			this.Home = entry.m_Location;
			}

		}

	}
}