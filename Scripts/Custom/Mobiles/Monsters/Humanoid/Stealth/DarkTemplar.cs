using System;
using System.Collections;
using Server.Misc;
using Server.Items;
using Server.Targeting;

namespace Server.Mobiles
{
	[CorpseName( "a dark templar corpse" )]
	public class DarkTemplar : BaseCreature
	{

		public override double WeaponAbilityChance { get { return 0.8; } }
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
		public DarkTemplar() : base( AIType.AI_Stealth, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "Krahnam The Leader of The Dark Templars";
			Body = 318;
			BaseSoundID = 0x2A7;
			Hue = 32500;
			Hidden = true;
			AllowedStealthSteps = 24;
			RangeFight = 3;

			SetStr( 650, 900 );
			SetDex( 150, 200 );
			SetInt( 860, 1000 );

			SetHits( 2400, 3000 );

			SetDamage( 22, 25 );

			SetDamageType( ResistanceType.Physical, 30 );
			SetDamageType( ResistanceType.Cold, 40 );
			SetDamageType( ResistanceType.Fire, 30 );

			SetResistance( ResistanceType.Physical, 60, 70 );
			SetResistance( ResistanceType.Fire, 60, 70 );
			SetResistance( ResistanceType.Cold, 60, 70 );
			SetResistance( ResistanceType.Poison, 60, 65 );
			SetResistance( ResistanceType.Energy, 60, 70 );

			SetSkill( SkillName.Anatomy, 110.2, 120.0 );
			SetSkill( SkillName.Archery, 110.1, 120.0 );
			SetSkill( SkillName.MagicResist, 90.1, 100.0 );
			SetSkill( SkillName.Tactics, 110.1, 120.0 );
			SetSkill( SkillName.Hiding, 100.0, 100.0 );
			SetSkill( SkillName.Stealth, 120.0, 120.0 );
			SetSkill( SkillName.SpiritSpeak, 100.1, 110.0 );
			SetSkill( SkillName.Meditation, 100.1, 110.0 );
			SetSkill( SkillName.EvalInt, 70.1, 80.0 );

			Fame = 25000;
			Karma = -25000;

			VirtualArmor = 40;

			HeavyCrossbow weapon = new HeavyCrossbow();

			weapon.Movable = false;
			weapon.Attributes.WeaponDamage = 35;
			weapon.Attributes.AttackChance = 40;
			weapon.Attributes.RegenHits = 40;
			weapon.WeaponAttributes.HitFireball = 100;
			weapon.WeaponAttributes.HitLightning = 100;
			weapon.WeaponAttributes.HitDispel = 100;
			weapon.Hue = 163;

			AddItem( weapon );
			PackItem( new Bolt( Utility.RandomMinMax( 500, 700 ) ) );
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.UltraRich, 3 );
		}

		public DarkTemplar( Serial serial ) : base( serial )
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