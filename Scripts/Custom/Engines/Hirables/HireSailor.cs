using System;
using Server.Items;

namespace Server.Mobiles
{
	public class HireSailor : BaseHire
	{
		[Constructable]
		public HireSailor() : base( "the sailor" )
		{
			InitStats( 100, 50, 50 );

			Fame = 100;
			Karma = 0;

			PackGold( 10, 25 );
		}

		public override void InitSkills()
		{
			base.InitSkills();

			SetSkill( SkillName.Swords,		50, 100 );
			SetSkill( SkillName.Fencing,	50, 100 );
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
			AddItem( AddProps(new Cutlass()) );
		}

		public HireSailor( Serial serial ) : base( serial )
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