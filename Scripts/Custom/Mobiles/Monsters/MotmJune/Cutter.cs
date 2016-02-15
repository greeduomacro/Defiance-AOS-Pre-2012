using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	public class Cutter : BaseCreature
	{
		[Constructable]
		public Cutter () : base( AIType.AI_Necro, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = NameList.RandomName( "male" );
			Body = 400;
			Hue = 456;

			SetStr( 510, 525 );
			SetDex( 66, 85 );
			SetInt( 226, 350 );

			SetHits( 416, 469 );

			SetDamage( 14, 19 );

			SetDamageType( ResistanceType.Physical, 30 );
			SetDamageType( ResistanceType.Poison, 50 );
			SetDamageType( ResistanceType.Energy, 20 );

			SetResistance( ResistanceType.Physical, 50, 60 );
			SetResistance( ResistanceType.Fire, 50, 60 );
			SetResistance( ResistanceType.Cold, 40, 45 );
			SetResistance( ResistanceType.Poison, 60, 60 );
			SetResistance( ResistanceType.Energy, 60, 60 );

			SetSkill( SkillName.SpiritSpeak, 95.1, 100.0 );
			SetSkill( SkillName.Necromancy, 95.1, 100.0 );
			SetSkill( SkillName.Meditation, 60.4, 90.0 );
			SetSkill( SkillName.MagicResist, 90.1, 95.0 );
			SetSkill( SkillName.Tactics, 90.1, 100.0 );
			SetSkill( SkillName.Swords, 100.1, 110.0 );
			SetSkill( SkillName.Anatomy, 100.1, 120.0 );

			Fame = 12500;
			Karma = -12500;

			VirtualArmor = 60;

			AddItem( new PlateHelm() );
			AddItem( new HalfApron() );

			Axe weapon = new Axe();

			weapon.Movable = false;
			weapon.Hue = 38;
			weapon.Attributes.WeaponDamage = 20;
			weapon.Attributes.AttackChance = 15;
			weapon.WeaponAttributes.HitDispel = 100;

			AddItem( weapon );

		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.FilthyRich, 2 );
			AddLoot( LootPack.Rich );
		}

		public override int TreasureMapLevel{ get{ return 5; } }
		public override bool AlwaysMurderer{ get{ return true; } }
		public override bool ShowFameTitle{ get{ return false; } }
		public override bool AutoDispel{ get{ return true; } }

		public override void OnBeforeSpawn( Point3D location, Map m )
		{
			base.OnBeforeSpawn( location, m );

			IsParagon = false;
			IsElder = false;
			IsPlagued = false;
			Hue = 456;
		}

		public override void OnGaveMeleeAttack( Mobile defender )
		{
			base.OnGaveMeleeAttack( defender );
			if ( !(defender.Alive) && ( defender is PlayerMobile ) )
			{
				this.Say( "Your head is mine!" );
			}
		}

		public Cutter( Serial serial ) : base( serial )
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
	}
}