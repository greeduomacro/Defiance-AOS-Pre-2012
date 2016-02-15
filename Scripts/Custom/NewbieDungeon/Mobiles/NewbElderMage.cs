using System;
using System.Collections;
using Server.Items;
using Server.ContextMenus;
using Server.Misc;
using Server.Network;

namespace Server.Mobiles
{
	public class NewbElderMage : BaseCreature
	{
		[Constructable]
		public NewbElderMage() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			SpeechHue = Utility.RandomDyedHue();
			Title = "the elder mage";
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
			SetDex( 51, 75 );
			SetInt( 166, 180 );

			SetHits( 181, 200 );

			SetDamage( 7, 12 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 10, 20 );
			SetResistance( ResistanceType.Fire, 10, 20 );
			SetResistance( ResistanceType.Cold, 6, 16 );
			SetResistance( ResistanceType.Poison, 10, 20 );
			SetResistance( ResistanceType.Energy, 10, 20 );

			SetSkill( SkillName.EvalInt, 85.1, 100.0 );
			SetSkill( SkillName.Magery, 85.1, 100.0 );
			SetSkill( SkillName.Meditation, 90.4, 100.0 );
			SetSkill( SkillName.MagicResist, 75.1, 97.5 );
			SetSkill( SkillName.Tactics, 65.0, 87.5 );
			SetSkill( SkillName.Wrestling, 20.1, 40.0 );

			Fame = 7500;
			Karma = -7500;

			PackReg( Utility.Random( 25, 12 ) );

			int itemHue = Utility.Random( 5 ) + 1154;

			BaseClothing toAdd = new Robe( itemHue );
			toAdd.LootType = LootType.Blessed;
			AddItem( toAdd );

			toAdd = new WizardsHat( itemHue );
			toAdd.LootType = LootType.Blessed;
			AddItem( toAdd );

			toAdd = new Sandals( itemHue );
			toAdd.LootType = LootType.Blessed;
			AddItem( toAdd );

			if( Utility.RandomDouble() < 0.3 )
				PackItem( NewbMage.NewbLRCArmor() );
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.HighScrolls );
			AddLoot( LootPack.FilthyRich );
		}

		public override bool AlwaysMurderer{ get{ return true; } }

		public NewbElderMage( Serial serial ) : base( serial )
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