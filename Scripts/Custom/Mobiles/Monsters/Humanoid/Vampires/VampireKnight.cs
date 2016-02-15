using System;
using System.Collections;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a Vampire corpse" )]
	public class VampireKnight : BaseVampire
	{

		[Constructable]
		public VampireKnight () : base( AIType.AI_Stealth, FightMode.Closest, 10, 1, 0.2, 0.4 )
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

			SetHits( 220, 230 );

			SetDamage( 15, 22 );

			SetDamageType( ResistanceType.Physical, 50 );
			SetDamageType( ResistanceType.Cold, 50 );

			SetResistance( ResistanceType.Physical, 44 );
			SetResistance( ResistanceType.Fire, -10 );
			SetResistance( ResistanceType.Cold, 35 );
			SetResistance( ResistanceType.Poison, 35 );
			SetResistance( ResistanceType.Energy, 35 );

			SetSkill( SkillName.SpiritSpeak, 100.0 );
			SetSkill( SkillName.Necromancy, 50.0 );
			SetSkill( SkillName.Focus, 90.1, 100.0 );
			SetSkill( SkillName.MagicResist, 100.0 );
			SetSkill( SkillName.Tactics, 105.0 );
			SetSkill( SkillName.Anatomy, 105.0 );
			SetSkill( SkillName.Macing, 100.1, 120.0 );
			SetSkill( SkillName.Fencing, 100.1, 120.0 );
			SetSkill( SkillName.Parry, 100.0 );

			Fame = 9000;
			Karma = -14000;

			VirtualArmor = 5;

			AddItem( new LeatherChest() );
			AddItem( new LeatherLegs() );
			AddItem( new LeatherArms() );
			AddItem( new LeatherCap() );
			AddItem( new LeatherGloves() );
			AddItem( new LeatherGorget() );
			switch ( Utility.Random( 6 ))
			{
				case 0:
				{
					Maul bozdugan = new Maul();
					bozdugan.Movable = false;
					bozdugan.WeaponAttributes.HitLeechHits = 100;
					AddItem( bozdugan );
					AddItem( new WoodenKiteShield() );
					break;
				}
				case 1:
				{
					Mace chuk = new Mace();
					chuk.Movable = false;
					chuk.WeaponAttributes.HitLeechHits = 100;
					AddItem( chuk );
					AddItem( new WoodenKiteShield() );
					break;
				}
				case 2:
				{
					BladedStaff bstaff = new BladedStaff();
					bstaff.Movable = false;
					bstaff.WeaponAttributes.HitLeechHits = 100;
					AddItem( bstaff );
					break;
				}
				case 3:
				{
					Spear kopie = new Spear();
					kopie.Movable = false;
					kopie.WeaponAttributes.HitLeechHits = 100;
					AddItem( kopie );
					break;
				}
				case 4:
				{
					Pike pika = new Pike();
					pika.Movable = false;
					pika.WeaponAttributes.HitLeechHits = 100;
					AddItem( pika );
					break;
				}
				case 5:
				{
					Longsword mech = new Longsword();
					mech.Movable = false;
					mech.WeaponAttributes.HitLeechHits = 100;
					AddItem( mech );
					AddItem( new WoodenKiteShield() );
					break;
				}
			}

			Item hair = new Item( Utility.RandomList( 0x203B, 0x2049, 0x2048, 0x204A ) );
			hair.Hue = Utility.RandomNondyedHue();
			hair.Layer = Layer.Hair;
			AddItem( hair );

			BaseMount horse = new SkeletalMount();
			horse.Rider = this;
			horse.Hue = 1;
			horse.Team = this.Team;

		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Rich, 1 );
		}

		public override bool AlwaysMurderer{ get{ return true; } }
		public override int TreasureMapLevel{ get{ return 3; } }

		public override bool ShowFameTitle{ get{ return false; } }
		public override bool ClickTitle{ get{ return false; } }

		public VampireKnight( Serial serial ) : base( serial )
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