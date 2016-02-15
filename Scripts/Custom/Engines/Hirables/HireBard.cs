using System;
using Server.Items;

namespace Server.Mobiles
{
	public class HireBard : BaseHire
	{
		[Constructable]
		public HireBard() : base( "the bard" )
		{
			InitStats( 100, 50, 50 );

			Fame = 100;
			Karma = 100;

			PackGold( 10, 50 );
		}

		public override void InitSkills()
		{
			base.InitSkills();

			SetSkill( SkillName.Swords,		50, 100 );
			SetSkill( SkillName.Archery,	50, 100 );
			SetSkill( SkillName.Tactics,	50, 80 );
			SetSkill( SkillName.Parry,		50, 80 );
			SetSkill( SkillName.Focus,		50, 70 );
			SetSkill( SkillName.Wrestling,	50, 70 );
			SetSkill( SkillName.Anatomy,	50, 60 );
		}

		public override void InitOutfit()
		{
			base.InitOutfit();

			AddItem( new Shoes() );

			switch ( Utility.Random( 4 ) )
			{
				case 0: PackItem( new Harp() ); break;
				case 1: PackItem( new Lute() ); break;
				case 2: PackItem( new Drums() ); break;
				case 3: PackItem( new Tambourine() ); break;
			}

			AddItem( AddProps(new Longsword()) );
		}

		public HireBard( Serial serial ) : base( serial )
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