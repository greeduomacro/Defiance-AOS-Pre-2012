using System;
using System.Collections;
using Server.Items;
using Server.ContextMenus;
using Server.Misc;
using Server.Network;

namespace Server.Mobiles
{
	public class NewbMage : BaseCreature
	{
		[Constructable]
		public NewbMage() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			SpeechHue = Utility.RandomDyedHue();
			Title = "the mage";
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

			SetStr( 81, 105 );
			SetDex( 41, 60 );
			SetInt( 66, 80 );

			SetDamage( 5, 10 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 15, 20 );
			SetResistance( ResistanceType.Fire, 5, 10 );
			SetResistance( ResistanceType.Poison, 5, 10 );
			SetResistance( ResistanceType.Energy, 5, 10 );

			SetSkill( SkillName.EvalInt, 65.1, 90.0 );
			SetSkill( SkillName.Magery, 55.1, 80.0 );
			SetSkill( SkillName.Meditation, 90.4, 100.0 );
			SetSkill( SkillName.MagicResist, 75.0, 97.5 );
			SetSkill( SkillName.Tactics, 65.0, 87.5 );
			SetSkill( SkillName.Wrestling, 20.2, 60.0 );

			Fame = 2500;
			Karma = -2500;

			PackReg( Utility.Random( 15, 10 ) );
			int itemHue = Utility.RandomNeutralHue();
			AddItem( new Robe( itemHue ) );
			AddItem( new WizardsHat( itemHue ) );
			AddItem( new Sandals() );

			if( Utility.RandomDouble() < 0.15 )
				PackItem( NewbLRCArmor() );
		}

		public override void GenerateLoot()
		{
			if ( Utility.RandomBool() )
				AddLoot( LootPack.HighScrolls );

			AddLoot( LootPack.Average );
		}

		public override bool AlwaysMurderer{ get{ return true; } }

		public static BaseArmor NewbLRCArmor()
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

			BaseRunicTool.ApplyAttributesTo(armor, Utility.Random( 3 ), 40, 80);

			armor.Attributes.LowerRegCost = 12 + Utility.Random( 5 ) + Utility.Random( 5 );
			armor.Hue = 2207;

			return armor;
		}

		public NewbMage( Serial serial ) : base( serial )
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