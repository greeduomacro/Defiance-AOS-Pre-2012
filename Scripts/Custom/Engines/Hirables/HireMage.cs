using System;
using Server.Items;

namespace Server.Mobiles
{
	public class HireMage : BaseHire
	{
		[Constructable]
		public HireMage() : base( "the mage", AIType.AI_Mage )
		{
			InitStats( 70, 100, 30 );

			Fame = 100;
			Karma = 100;

			PackGold( 20, 100 );
		}

		public override void InitSkills()
		{
			base.InitSkills();

			SetSkill( SkillName.EvalInt,		70, 100 );
			SetSkill( SkillName.Magery,			70, 100 );
			SetSkill( SkillName.Meditation,		70, 100 );
			SetSkill( SkillName.MagicResist,	70, 100 );
			SetSkill( SkillName.Tactics,		70, 100 );
			SetSkill( SkillName.Macing,			70, 100 );
			SetSkill( SkillName.Wrestling,		70, 100 );
		}

		public override void InitOutfit()
		{
			base.InitOutfit();

			AddItem( new Sandals() );
			AddItem( new Robe( Utility.RandomNeutralHue() ) );
		}

		public HireMage( Serial serial ) : base( serial )
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