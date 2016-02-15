using System;
using Server.Items;

namespace Server.Mobiles
{
	public class HireRanger : BaseHire
	{
		[Constructable]
		public HireRanger() : base( "the ranger" )
		{
			InitStats( 100, 50, 50 );

			Fame = 100;
			Karma = 125;

			PackItem ( new Arrow( 20 ) );
			PackGold( 10, 75 );
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
			AddItem( new Shoes() );

			switch ( Utility.Random( 3 ))
			{
				case 0: AddItem( AddProps(new Longsword()) ); break;
				case 1: AddItem( AddProps(new VikingSword()) ); break;
				case 2: AddItem( AddProps(new Broadsword()) ); break;
			}

			AddItem( AddProps(new RangerChest()) );
			AddItem( AddProps(new RangerArms()) );
			AddItem( AddProps(new RangerGloves()) );
			AddItem( AddProps(new RangerGorget()) );
			AddItem( AddProps(new RangerLegs()) );
		}

		public HireRanger( Serial serial ) : base( serial )
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