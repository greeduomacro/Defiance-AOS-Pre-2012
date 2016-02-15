using System;
using System.Collections;
using Server.Items;
using Server.ContextMenus;
using Server.Misc;
using Server.Network;

namespace Server.Mobiles
{
	public class NewbKnight : BaseCreature
	{
		public override bool ClickTitle{ get{ return false; } }

		[Constructable]
		public NewbKnight() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			SpeechHue = Utility.RandomDyedHue();
			Title = "the knight";
			Hue = Utility.RandomSkinHue();

			if ( this.Female = Utility.RandomBool() )
			{
				Body = 0x191;
				Name = NameList.RandomName( "female" );
			}
			else
			{
				Body = 0x190;
				Name = NameList.RandomName( "male" );
			}

			SetStr( 191, 210 );
			SetDex( 141, 160 );
			SetInt( 61, 75 );

			SetDamage( 10, 19 );

			SetSkill( SkillName.MagicResist, 55.0, 67.5 );
			SetSkill( SkillName.Fencing, 85.0, 97.5 );
			SetSkill( SkillName.Tactics, 75.0, 87.5 );

			Fame = 5000;
			Karma = -5000;

			// Equip
			CraftResource material = Utility.RandomBool() ? CraftResource.Verite : CraftResource.Valorite;

			BaseArmor toArm = new PlateChest();
			toArm.Resource = material;
			toArm.LootType = LootType.Blessed;
			AddItem( toArm );

			toArm = new PlateLegs();
			toArm.Resource = material;
			toArm.LootType = LootType.Blessed;
			AddItem( toArm );

			toArm = new PlateArms();
			toArm.Resource = material;
			toArm.LootType = LootType.Blessed;
			AddItem( toArm );

			BaseWeapon lance = new Lance();
			lance.Movable = false;
			lance.LootType = LootType.Blessed;
			AddItem( lance );

			Utility.AssignRandomHair( this );

			PackItem( new Bandage( Utility.Random( 10, 10 ) ) );

			if( Utility.RandomDouble() < 0.3 )
				PackItem( NewbWarrior.NewbLuckArmor() );
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.FilthyRich );
		}

		public override bool AlwaysMurderer{ get{ return true; } }

		public NewbKnight( Serial serial ) : base( serial )
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