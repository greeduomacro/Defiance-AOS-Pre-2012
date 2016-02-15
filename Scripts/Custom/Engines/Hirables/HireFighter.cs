using System;
using Server.Items;

namespace Server.Mobiles
{
	public class HireFighter : BaseHire
	{
		[Constructable]
		public HireFighter() : base( "the fighter" )
		{
			InitStats( 100, 50, 50 );

			Fame = 100;
			Karma = 100;

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
			AddItem( new Shoes() );

			switch ( Utility.Random( 3 ))
			{
				case 0: AddItem( AddProps(new Longsword()) ); break;
				case 1: AddItem( AddProps(new Broadsword()) ); break;
				case 2: AddItem( AddProps(new VikingSword()) ); break;
			}

			switch ( Utility.Random( 7 ))
			{
				case 0: AddItem( AddProps(new BronzeShield()) ); break;
				case 1: AddItem( AddProps(new HeaterShield()) ); break;
				case 2: AddItem( AddProps(new MetalKiteShield()) ); break;
				case 3: AddItem( AddProps(new MetalShield()) ); break;
				case 4: AddItem( AddProps(new WoodenKiteShield()) ); break;
				case 5: AddItem( AddProps(new WoodenShield()) ); break;
				case 6: AddItem( AddProps(new OrderShield()) ); break;
			}

			switch( Utility.Random( 5 ) )
			{
				case 0: break;
				case 1: AddItem( AddProps(new Bascinet()) ); break;
				case 2: AddItem( AddProps(new CloseHelm()) ); break;
				case 3: AddItem( AddProps(new NorseHelm()) ); break;
				case 4: AddItem( AddProps(new Helmet()) ); break;
			}

			switch( Utility.Random( 3 ) )
			{
				case 0:
					AddItem( AddProps(new LeatherChest()) );
					AddItem( AddProps(new LeatherArms()) );
					AddItem( AddProps(new LeatherGloves()) );
					AddItem( AddProps(new LeatherGorget()) );
					AddItem( AddProps(new LeatherLegs()) );
					break;

				case 1:
					AddItem( AddProps(new StuddedChest()) );
					AddItem( AddProps(new StuddedArms()) );
					AddItem( AddProps(new StuddedGloves()) );
					AddItem( AddProps(new StuddedGorget()) );
					AddItem( AddProps(new StuddedLegs()) );
					break;

				case 2:
					AddItem( AddProps(new RingmailChest()) );
					AddItem( AddProps(new RingmailArms()) );
					AddItem( AddProps(new RingmailGloves()) );
					AddItem( AddProps(new RingmailLegs()) );
					break;
			}
		}

		public HireFighter( Serial serial ) : base( serial )
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