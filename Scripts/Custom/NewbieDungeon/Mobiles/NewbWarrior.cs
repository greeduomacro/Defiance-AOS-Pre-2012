using System;
using System.Collections;
using Server.Items;
using Server.ContextMenus;
using Server.Misc;
using Server.Network;

namespace Server.Mobiles
{
	public class NewbWarrior : BaseCreature
	{
		public override bool ClickTitle{ get{ return false; } }

		[Constructable]
		public NewbWarrior() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			SpeechHue = Utility.RandomDyedHue();
			Title = "the warrior";
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

			SetStr( 86, 100 );
			SetDex( 81, 95 );
			SetInt( 61, 75 );

			SetDamage( 10, 17 );

			SetSkill( SkillName.MagicResist, 35.0, 47.5 );
			SetSkill( SkillName.Swords, 75.0, 87.5 );
			SetSkill( SkillName.Tactics, 75.0, 87.5 );

			Fame = 1000;
			Karma = -1000;

			// Equip
			Item toArm = new RingmailChest();
			toArm.LootType = LootType.Blessed;
			AddItem( toArm );

			toArm = new RingmailLegs();
			toArm.LootType = LootType.Blessed;
			AddItem( toArm );

			toArm = new RingmailArms();
			toArm.LootType = LootType.Blessed;
			AddItem( toArm );

			toArm = new RingmailGloves();
			toArm.LootType = LootType.Blessed;
			AddItem( toArm );

			toArm = new Cutlass();
			toArm.Movable = false;
			toArm.LootType = LootType.Blessed;
			AddItem( toArm );

			Utility.AssignRandomHair( this );

			PackItem( new Gold( Utility.Random( 25, 25 ) ) );
			PackItem( new Bandage( Utility.Random( 8, 4 ) ) );

			if( Utility.RandomDouble() < 0.15 )
				PackItem( NewbLuckArmor() );
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Average );
		}

		public override bool AlwaysMurderer{ get{ return true; } }

		public static BaseArmor NewbLuckArmor()
		{
			BaseArmor armor = null;

			switch( Utility.Random( 6 ) )
			{
				case 0: armor = new LeatherChest(); break;
				case 1: armor = new LeatherLegs(); break;
				case 2: armor = new LeatherArms(); break;
				case 3: armor = new LeatherGloves(); break;
				case 4: armor = new LeatherGorget(); break;
				case 5: armor = new LeatherCap(); break;
			}

			BaseRunicTool.ApplyAttributesTo(armor, Utility.Random( 4 ), 40, 80);

			int luck = 65 + Utility.Random( 21 ) + Utility.Random( 21 );
			if( luck > 100 )
				luck = 100;

			armor.Attributes.Luck = luck;
			armor.Hue = 2123;

			return armor;
		}

		public NewbWarrior( Serial serial ) : base( serial )
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