using System;
using Server.Items;

namespace Server.Mobiles
{
	public class HirePeasant : BaseHire
	{
		[Constructable]
		public HirePeasant() : base ( "the peasant" )
		{
			InitStats( 100, 50, 50 );

			Fame = 0;
			Karma = 0;

			PackGold( 10, 25 );
		}

		public override void InitSkills()
		{
			base.InitSkills();

			SetSkill( SkillName.Tactics,	50, 60 );
			SetSkill( SkillName.Wrestling,	50, 60 );
			SetSkill( SkillName.Swords,		50, 60 );
			SetSkill( SkillName.Anatomy,	50, 60 );
		}

		public override void InitOutfit()
		{
			base.InitOutfit();

			AddItem( new Sandals() );
		}

		public HirePeasant( Serial serial ) : base( serial )
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