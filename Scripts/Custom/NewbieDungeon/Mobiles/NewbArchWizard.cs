using System;
using System.Collections;
using Server.Items;
using Server.ContextMenus;
using Server.Misc;
using Server.Network;

namespace Server.Mobiles
{
	public class NewbArchWizard : BaseCreature
	{
		[Constructable]
		public NewbArchWizard() : base( AIType.AI_NecroMage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			SpeechHue = Utility.RandomDyedHue();
			Title = "the arch wizard";
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
			SetDex( 91, 115 );
			SetInt( 166, 180 );

			SetHits( 381, 400 );
			SetMana( 381, 400 );

			SetDamage( 8, 13 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 15, 25 );
			SetResistance( ResistanceType.Fire, 15, 25 );
			SetResistance( ResistanceType.Cold, 15, 25 );
			SetResistance( ResistanceType.Poison, 15, 25 );
			SetResistance( ResistanceType.Energy, 15, 25 );

			SetSkill( SkillName.SpiritSpeak, 80.1, 95.0 );
			SetSkill( SkillName.Necromancy, 85.1, 100.0 );
			SetSkill( SkillName.EvalInt, 85.1, 100.0 );
			SetSkill( SkillName.Magery, 85.1, 100.0 );
			SetSkill( SkillName.Meditation, 90.4, 100.0 );
			SetSkill( SkillName.MagicResist, 75.1, 97.5 );
			SetSkill( SkillName.Tactics, 65.0, 87.5 );
			SetSkill( SkillName.Wrestling, 40.1, 60.0 );

			Fame = 10000;
			Karma = -10000;

			PackReg( Utility.Random( 35, 15 ) );
			PackNecroReg( Utility.Random( 20, 10 ) );

			if( Utility.RandomDouble() >= 0.4)
				PackItem( NewbJewelry() );

			Item toAdd = new Sandals( 1151 );
			toAdd.LootType = LootType.Blessed;
			AddItem( toAdd );

			toAdd = new HoodedShroudOfShadows( 1151 );
			toAdd.Movable = false;
			AddItem( toAdd );
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.HighScrolls, 2 );
			AddLoot( LootPack.FilthyRich, 1 );
			AddLoot( LootPack.Average, 1 );
		}

		public override bool ShowFameTitle{ get{ return false; } }
		public override bool AlwaysMurderer{ get{ return true; } }

		public static BaseJewel NewbJewelry()
		{
			BaseJewel jewel = null;

			switch( Utility.Random( 4 ) )
			{
				case 0: jewel = new GoldRing(); break;
				case 1: jewel = new GoldBracelet(); break;
				case 2: jewel = new SilverRing(); break;
				case 3: jewel = new SilverBracelet(); break;
			}

			if( Utility.RandomBool() )
			{
				int luck = 60 + Utility.Random( 26 ) + Utility.Random( 26 );
				if( luck > 100 )
					luck = 100;

				jewel.Attributes.Luck = luck;
			}
			else
			{
				jewel.Attributes.CastSpeed = 1;
				jewel.Attributes.CastRecovery = Utility.RandomMinMax( 1, 3 );
			}

			return jewel;
		}

		public NewbArchWizard( Serial serial ) : base( serial )
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