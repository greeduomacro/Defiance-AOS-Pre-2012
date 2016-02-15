//******************************************************
// Name: HolidayBell
// Desc: Written by Eclipse
//******************************************************
using System;
using Server;

namespace Server.Items
{
	public class HolidayBell : Item
	{
		private int m_Sound;

		[Constructable]
		public HolidayBell() : base( 0x1C12 )
		{
			Name = string.Format( "a holiday bell from {0}", m_Names[Utility.Random(m_Names.Length)] );
			Hue = m_Hues[Utility.Random(m_Hues.Length)];
			m_Sound = Utility.RandomMinMax(245,259);
			LootType = LootType.Blessed;
			Weight = 1.0;
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( IsChildOf( from.Backpack ) )
				from.PlaySound( m_Sound );
			else
				from.SendLocalizedMessage( 1062334 ); // This item must be in your backpack to be used.
		}

		private static int[] m_Hues = new int[]
			{
				0x23,
				0x1A,
				0x24,
				0x2E,
				0x38,
				0x3C,
				0x10A,
				0x51,
				0x430,
				0x13A,
				0x137,
				0x132,
				0x128,
				0x123,
				0x119
			};

		private static string[] m_Names = new string[]
			{
				"Admin Blady",
				"Dev Hekate",
				"Seer Nystal",
				"GM Wings",
				"GM Astrid",
				"GM Days",
				"GM Kale",
				"GM Selaon",
				"Chief X Lord X",
				"Chief Light"
			};

		public HolidayBell( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );

			writer.Write( (int) m_Sound );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();

			m_Sound = reader.ReadInt();
		}
	}
}