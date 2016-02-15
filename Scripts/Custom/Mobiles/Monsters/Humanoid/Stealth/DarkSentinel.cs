using System;
using System.Collections;
using Server.Items;
using Server.Targeting;

namespace Server.Mobiles
{
	[CorpseName( "a dark sentinel corpse" )]
	public class DarkSentinel : BaseCreature
	{

		public override WeaponAbility GetWeaponAbility()
		{
			return WeaponAbility.ArmorIgnore;
		}

		[Constructable]
		public DarkSentinel() : base( AIType.AI_Stealth, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Body = 0x191;
			Name = NameList.RandomName( "female" );
			RangePerception = 10;

			SetStr( 125, 130 );
			SetDex( 100, 125 );
			SetInt( 80, 90 );

			SetHits( 120, 145 );
			SetMana( 150, 160 );

			SetDamage( 10, 23 );

			SetDamageType( ResistanceType.Physical, 60 );
			SetDamageType( ResistanceType.Poison, 40 );

			SetResistance( ResistanceType.Physical, 35, 40 );
			SetResistance( ResistanceType.Fire, 35, 40 );
			SetResistance( ResistanceType.Cold, 35, 40 );
			SetResistance( ResistanceType.Poison, 35, 40 );
			SetResistance( ResistanceType.Energy, 35, 40 );

			SetSkill( SkillName.MagicResist, 80.1, 95.0 );
			SetSkill( SkillName.Tactics, 100.1, 105.0 );
			SetSkill( SkillName.Anatomy, 100.1, 105.0 );
			SetSkill( SkillName.Archery, 100.1, 110.0 );
			SetSkill( SkillName.Meditation, 95.1, 100.0 );
			SetSkill( SkillName.Focus, 90.1, 100.0 );
			SetSkill( SkillName.SpiritSpeak, 90.1, 100.0 );
			SetSkill( SkillName.Hiding, 95.1, 100.0 );

			Fame = 9000;
			Karma = -9000;

			VirtualArmor = 18;

			AddItem( new Sandals() );
			AddItem( new FemaleLeatherChest() );
			AddItem( new LeatherShorts() );
			AddItem( new LeatherGloves() );
			AddItem( new LeatherGorget() );
			AddItem( new LeatherCap() );
			AddItem( new LeatherArms() );

			CompositeBow weapon = new CompositeBow();

			weapon.Movable = false;
			weapon.Attributes.WeaponDamage = 40;
			weapon.Attributes.WeaponSpeed = 15;
			weapon.Attributes.AttackChance = 10;
			weapon.WeaponAttributes.HitLightning = 50;
			weapon.WeaponAttributes.HitFireball = 50;
			weapon.WeaponAttributes.HitMagicArrow = 50;
			weapon.WeaponAttributes.HitDispel = 100;

			weapon.Hue = 15;

			AddItem( weapon );

		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Rich );
		}

		public override Poison PoisonImmune{ get{ return Poison.Lesser; } }
		public override bool DisallowAllMoves{ get{ return true; } }
		public override bool AlwaysMurderer{ get{ return true; } }
		public override bool ShowFameTitle{ get{ return false; } }

		public DarkSentinel( Serial serial ) : base( serial )
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