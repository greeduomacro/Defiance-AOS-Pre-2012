using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a pile of rust" )]
	public class CursedAxe : BaseCreature
	{

		public override double WeaponAbilityChance { get { return 0.8; } }
		public override WeaponAbility GetWeaponAbility()
		{
			return WeaponAbility.ArmorIgnore;
		}

		[Constructable]
		public CursedAxe () : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a cursed weapon";
			Body = 611 + Utility.Random( 5 );
			Hue = 561;

			SetStr( 326, 355 );
			SetDex( 66, 85 );
			SetInt( 271, 295 );

			SetHits( 80, 90 );

			SetDamage( 13, 18 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 90, 95 );
			SetResistance( ResistanceType.Fire, 60, 75 );
			SetResistance( ResistanceType.Cold, 60, 75 );
			SetResistance( ResistanceType.Poison, 60, 75 );
			SetResistance( ResistanceType.Energy, 60, 75 );

			SetSkill( SkillName.Anatomy, 60.3, 70.0 );
			SetSkill( SkillName.MagicResist, 60.1, 75.0 );
			SetSkill( SkillName.Tactics, 80.1, 90.0 );
			SetSkill( SkillName.Wrestling, 90.1, 100.0 );

			Fame = 10000;
			Karma = -10000;

			VirtualArmor = 40;
		}

		public override bool BleedImmune{ get{ return true; } }
		public override Poison PoisonImmune{ get{ return Poison.Lethal; } }

		public override bool OnBeforeDeath()
		{
			Effects.SendLocationEffect( Location, Map, 0x376A, 10, 1 );
			Gold g = new Gold( 900, 1000 );
			g.MoveToWorld( new Point3D( X, Y, Z ), Map );

			return true;
		}

		public CursedAxe( Serial serial ) : base( serial )
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