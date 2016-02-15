using System;
using System.Collections;
using Server.Items;
using Server.ContextMenus;
using Server.Misc;
using Server.Network;

namespace Server.Mobiles
{
	public class NewbArcher : BaseCreature
	{
		public override bool ClickTitle{ get{ return false; } }

		[Constructable]
		public NewbArcher() : base( AIType.AI_Archer, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			SpeechHue = Utility.RandomDyedHue();
			Title = "the archer";
			Hue = Utility.RandomSkinHue();

			if ( this.Female = Utility.RandomBool() )
			{
				Body = 0x191;
				Name = NameList.RandomName( "female" );
				AddItem( new Skirt( Utility.RandomNeutralHue() ) );
			}
			else
			{
				Body = 0x190;
				Name = NameList.RandomName( "male" );
			}

			SetStr( 146, 160 );
			SetDex( 121, 135 );
			SetInt( 61, 75 );

			SetDamage( 10, 17 );

			SetSkill( SkillName.MagicResist, 35.0, 47.5 );
			SetSkill( SkillName.Archery, 85.0, 97.5 );
			SetSkill( SkillName.Tactics, 75.0, 87.5 );

			Fame = 3000;
			Karma = -3000;

			// Equip
			Item toArm = new StuddedChest();
			toArm.LootType = LootType.Blessed;
			AddItem( toArm );

			toArm = new StuddedLegs();
			toArm.LootType = LootType.Blessed;
			AddItem( toArm );

			toArm = new StuddedArms();
			toArm.LootType = LootType.Blessed;
			AddItem( toArm );

			toArm = new StuddedGloves();
			toArm.LootType = LootType.Blessed;
			AddItem( toArm );

			toArm = new Bow();
			toArm.Movable = false;
			toArm.LootType = LootType.Blessed;
			AddItem( toArm );

			Utility.AssignRandomHair( this );

			PackItem( new Bandage( Utility.Random( 8, 4 ) ) );
			PackItem( new Arrow( Utility.Random( 30, 20 ) ) );

			if( Utility.RandomDouble() < 0.2 )
				PackItem( NewbWarrior.NewbLuckArmor() );
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Rich );
		}

		public override bool AlwaysMurderer{ get{ return true; } }

		public NewbArcher( Serial serial ) : base( serial )
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