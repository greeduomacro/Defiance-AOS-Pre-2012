using System;
using System.Collections;
using Server.Items;
using Server.ContextMenus;
using Server.Misc;
using Server.Network;

namespace Server.Mobiles
{
	public class WolfMaster : BaseCreature
	{
		public override bool ClickTitle{ get{ return false; } }

		[Constructable]
		public WolfMaster() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			SpeechHue = Utility.RandomDyedHue();
			Title = "the wolf master";
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
				AddItem( new Skirt( Utility.RandomNeutralHue() ) );
			}

			SetStr( 86, 100 );
			SetDex( 31, 45 );
			SetInt( 61, 75 );

			SetDamage( 6, 11 );

			SetHits( 171, 190 );

			SetSkill( SkillName.MagicResist, 35.0, 47.5 );
			SetSkill( SkillName.Macing, 75.0, 87.5 );
			SetSkill( SkillName.Tactics, 45.0, 57.5 );

			Fame = 3000;
			Karma = -3000;

			// Equip
			Item toArm = new LeatherChest();
			toArm.LootType = LootType.Blessed;
			AddItem( toArm );

			toArm = new LeatherLegs();
			toArm.LootType = LootType.Blessed;
			AddItem( toArm );

			toArm = new StrawHat();
			toArm.LootType = LootType.Blessed;
			AddItem( toArm );

			toArm = new GnarledStaff();
			toArm.Movable = false;
			toArm.LootType = LootType.Blessed;
			AddItem( toArm );

			Utility.AssignRandomHair( this );

			if( Utility.RandomDouble() < 0.06 )
				PackItem( new BallOfSummoning() );
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Rich );
		}

		public override bool AlwaysMurderer{ get{ return true; } }

		public WolfMaster( Serial serial ) : base( serial )
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