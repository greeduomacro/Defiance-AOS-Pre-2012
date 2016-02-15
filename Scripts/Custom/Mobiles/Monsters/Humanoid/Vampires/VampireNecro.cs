using System;
using System.Collections;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a Vampire corpse" )]
	public class VampireNecro : BaseVampire
	{

		[Constructable]
		public VampireNecro () : base( AIType.AI_Necro, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			if ( Female = Utility.RandomBool() )
			{
				Body = 0x191;
				Name = NameList.RandomName( "female" );
			}
			else
			{
				Body = 0x190;
				Name = NameList.RandomName( "male" );
			}
			Hue = 1150;
			BaseSoundID = 0x4B0;

			SetStr( 100, 105 );
			SetDex( 128, 131 );
			SetInt( 60, 62 );

			SetHits( 218, 225 );

			SetDamage( 15, 22 );

			SetDamageType( ResistanceType.Physical, 50 );
			SetDamageType( ResistanceType.Cold, 50 );

			SetResistance( ResistanceType.Physical, 40 );
			SetResistance( ResistanceType.Fire, -10 );
			SetResistance( ResistanceType.Cold, 35 );
			SetResistance( ResistanceType.Poison, 35 );
			SetResistance( ResistanceType.Energy, 35 );

			SetSkill( SkillName.SpiritSpeak, 100.0, 110.0 );
			SetSkill( SkillName.Necromancy, 110.0, 120.0 );
			SetSkill( SkillName.Focus, 90.1, 100.0 );
			SetSkill( SkillName.Meditation, 100.0 );
			SetSkill( SkillName.MagicResist, 100.0 );
			SetSkill( SkillName.Tactics, 100.0 );
			SetSkill( SkillName.Swords, 100.0 );
			SetSkill( SkillName.Poisoning, 100.0 );
			SetSkill( SkillName.Fencing, 100.0 );
			SetSkill( SkillName.Parry, 100.0 );

			Fame = 9000;
			Karma = -14000;

			VirtualArmor = -5;

			AddItem( new LeatherChest() );
			AddItem( new LeatherLegs() );
			AddItem( new LeatherArms() );
			AddItem( new LeatherCap() );
			AddItem( new LeatherGloves() );
			AddItem( new LeatherGorget() );
			switch ( Utility.Random( 3 ))
			{
				case 0:
				{
					Dagger kama = new Dagger();
					kama.Movable = false;
					kama.WeaponAttributes.HitLeechHits = 100;
					kama.Poison = Poison.Deadly;
					kama.PoisonCharges = 200;
					AddItem( kama );
					break;
				}
				case 1:
				{
					Cleaver satur = new Cleaver();
					satur.Movable = false;
					satur.WeaponAttributes.HitLeechHits = 100;
					satur.Poison = Poison.Deadly;
					satur.PoisonCharges = 200;
					AddItem( satur );
					break;
				}
				case 2:
				{
					Kryss kinjal = new Kryss();
					kinjal.Movable = false;
					kinjal.WeaponAttributes.HitLeechHits = 100;
					kinjal.Poison = Poison.Deadly;
					kinjal.PoisonCharges = 200;
					AddItem( kinjal );
					break;
				}
			}

			AddItem( new WoodenKiteShield() );

			Item hair = new Item( Utility.RandomList( 0x203B, 0x2049, 0x2048, 0x204A ) );
			hair.Hue = Utility.RandomNondyedHue();
			hair.Layer = Layer.Hair;
			AddItem( hair );

		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Rich, 1 );
		}

		public override bool AlwaysMurderer{ get{ return true; } }
		public override int TreasureMapLevel{ get{ return 3; } }

		public override bool ShowFameTitle{ get{ return false; } }
		public override bool ClickTitle{ get{ return false; } }

		public override bool IsEnemy( Mobile m )
		{

			if ( this.Hits < 100 && this.Hidden )
				return false;

			return base.IsEnemy( m );
		}

		public override void OnGaveMeleeAttack( Mobile defender )
		{
			base.OnGaveMeleeAttack( defender );
			if ( this.Body == 317 )
			{
				if ( this.Female )
					this.Body = 401;
				else
					this.Body = 400;
				this.ActiveSpeed = 0.2;
			}
		}

		public VampireNecro( Serial serial ) : base( serial )
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