using System;
using Server;
using Server.Network;

namespace Server.Items
{
	[FlipableAttribute( 0x236E, 0x2371 )]
	public class LightOfTheWinterSolstice  : Item
	{
		private static string[] m_StaffNames = new string[]
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
				"Chief Light",
				"Lead Dev Kamron"
			};

		private string m_Dipper;
		private string m_Label;

		[CommandProperty( AccessLevel.GameMaster )]
		public string Dipper{ get{ return m_Dipper; } set{ m_Dipper = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public string Label{ get{ return m_Label; } set{ m_Label = value; } }

		[Constructable]
		public LightOfTheWinterSolstice() : this( m_StaffNames[Utility.Random( m_StaffNames.Length )] )
		{
		}

		[Constructable]
		public LightOfTheWinterSolstice( string dipper ) : base( 0x236E )
		{
			m_Dipper = dipper;
			m_Label = "December 2007";

			Weight = 1.0;
			LootType = LootType.Blessed;
			Light = LightType.Circle300;
			Hue = Utility.RandomDyedHue();
		}

		public LightOfTheWinterSolstice( Serial serial ) : base( serial )
		{
		}

		public override void OnSingleClick( Mobile from )
		{
			base.OnSingleClick( from );

			LabelTo( from, 1070881, m_Dipper ); // Hand Dipped by ~1_name~
			LabelTo( from, m_Label );
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );

			list.Add( 1070881, m_Dipper ); // Hand Dipped by ~1_name~
			list.Add( m_Label  );
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 ); // version

			writer.Write( (string) m_Dipper );
			writer.Write( (string) m_Label );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 1:
				{
					m_Dipper = reader.ReadString();
					m_Label = reader.ReadString();
					break;
				}
				case 0:
				{
					m_Label = "Winter 2007";
					m_Dipper = m_StaffNames[Utility.Random( m_StaffNames.Length )];
					break;
				}
			}

			Utility.Intern( ref m_Dipper );
		}
	}
}