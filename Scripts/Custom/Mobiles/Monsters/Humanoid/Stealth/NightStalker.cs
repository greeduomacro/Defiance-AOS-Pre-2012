using System;
using Server.Mobiles;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName("a night stalker corpse")]
	[TypeAlias( "Server.Mobiles.NightStalker" )]
	public class NightStalker : BaseCreature
	{

		public override WeaponAbility GetWeaponAbility()
		{
			return WeaponAbility.ParalyzingBlow;
		}

		[Constructable]
		public NightStalker() : base( AIType.AI_Stealth, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Body = 51;
			Name = "a night stalker";
			Hue = 32500;
			Hidden = true;
			BaseSoundID = 219;
			AllowedStealthSteps = 24;

			SetStr( 161, 320 );
			SetDex( 60, 100 );
			SetInt( 21, 40 );

			SetHits( 101, 180 );

			SetDamage( 8, 15 );

			SetDamageType( ResistanceType.Physical, 30 );
			SetDamageType( ResistanceType.Poison, 70 );

			SetResistance( ResistanceType.Physical, 50, 60 );
			SetResistance( ResistanceType.Fire, 35, 45 );
			SetResistance( ResistanceType.Cold, 50, 60 );
			SetResistance( ResistanceType.Poison, 100 );
			SetResistance( ResistanceType.Energy, 35, 45 );

			SetSkill( SkillName.Poisoning, 90.1, 100.0 );
			SetSkill( SkillName.MagicResist, 95.1, 100.0 );
			SetSkill( SkillName.AnimalLore, 40.1, 50.0 );
			SetSkill( SkillName.Wrestling, 110.1, 120.0 );
			SetSkill( SkillName.Hiding, 100.0, 100.0 );
			SetSkill( SkillName.Stealth, 120.0, 120.0 );
			SetSkill( SkillName.Veterinary, 100.1, 110.0 );
			SetSkill( SkillName.Tactics, 50.1, 60.0 );

			Fame = 12000;
			Karma = -12000;

			PackItem( new Bandage( Utility.RandomMinMax( 15, 25 ) ) );

			VirtualArmor = 40;

		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Rich );
			AddLoot( LootPack.Gems, 10 );
		}

		public override int Meat{ get{ return 1; } }
		public override Poison PoisonImmune{ get{ return Poison.Lethal; } }
		public override Poison HitPoison{ get{ return Poison.Lethal; } }

		public NightStalker(Serial serial) : base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write((int) 0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();

			if ( BaseSoundID == -1 )
				BaseSoundID = 219;
		}
	}
}