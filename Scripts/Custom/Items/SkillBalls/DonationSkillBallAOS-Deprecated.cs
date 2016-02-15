using System;
using System.Collections.Generic;
using Server;

namespace Server.Items
{
	public class DonationSkillBallAOS : Item
	{
		public int SkillBonus;

		//[Constructable]
		public DonationSkillBallAOS() : this( 25 )
		{
		}

		//[Constructable]
		public DonationSkillBallAOS( int bonus ) : this( bonus, true )
		{
		}

		//[Constructable]
		public DonationSkillBallAOS( int bonus, bool limited ) : this( bonus, 100, limited )
		{
		}

		//[Constructable]
		public DonationSkillBallAOS( int bonus, int maxcap ) : this( bonus, maxcap, true )
		{
		}

		//[Constructable]
		public DonationSkillBallAOS( int bonus, int maxcap, bool limited ) : this( bonus, maxcap, limited, 0 )
		{
		}

		//[Constructable]
		public DonationSkillBallAOS( int bonus, int maxcap, bool limited, int days ) : this( bonus, maxcap, limited, days, 100 )
		{
		}

		//[Constructable]
		public DonationSkillBallAOS( int bonus, int maxcap, bool limited, int days, int mincap ) : base( 7885 )
		{
		}

		public DonationSkillBallAOS( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
			writer.Write( (int) SkillBonus );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			SkillBonus = reader.ReadInt();

			ItemConversion.AddToDonationAOSConversion( this );
		}
	}
}