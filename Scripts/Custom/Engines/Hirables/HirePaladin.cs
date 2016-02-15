using System;
using Server.Items;

namespace Server.Mobiles
{
	public class HirePaladin : BaseHire
	{
		[Constructable]
		public HirePaladin() : base( "the paladin" )
		{
			InitStats( 100, 50, 50 );

			Fame = 100;
			Karma = 250;

			PackGold( 20, 100 );
		}

		public override void InitSkills()
		{
			base.InitSkills();

			SetSkill( SkillName.Swords,		50, 100 );
			SetSkill( SkillName.Macing,		50, 100 );
			SetSkill( SkillName.Tactics,	50, 80 );
			SetSkill( SkillName.Parry,		50, 80 );
			SetSkill( SkillName.Focus,		50, 70 );
			SetSkill( SkillName.Wrestling,	50, 70 );
			SetSkill( SkillName.Anatomy,	50, 60 );
		}

		public override void InitOutfit()
		{
			AddItem( AddProps(new VikingSword()) );
			AddItem( AddProps(new MetalKiteShield()) );

			switch( Utility.Random( 5 ) )
			{
				case 0: break;
				case 1: AddItem( AddProps(new Bascinet()) ); break;
				case 2: AddItem( AddProps(new CloseHelm()) ); break;
				case 3: AddItem( AddProps(new NorseHelm()) ); break;
				case 4: AddItem( AddProps(new Helmet()) ); break;
			}

			AddItem( AddProps(new PlateChest()) );
			AddItem( AddProps(new PlateLegs()) );
			AddItem( AddProps(new PlateArms()) );
			AddItem( AddProps(new LeatherGorget()) );
		}

		public HirePaladin( Serial serial ) : base( serial )
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