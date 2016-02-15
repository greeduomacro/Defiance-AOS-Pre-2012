//******************************************************
// Name: SnowGlobe
// Desc: Written by Eclipse
//******************************************************
using System;
using Server;

namespace Server.Items
{
	public class SnowGlobe : Item
	{
		[Constructable]
		public SnowGlobe() : base( 0xE2D )
		{
			Name = string.Format( "a snowy scene of {0}.", m_Names[Utility.Random(m_Names.Length)] );
			LootType = LootType.Blessed;
			Weight = 1.0;
		}

		private static string[] m_Names = new string[]
			{
				"Britain",
				"Buccaneer's Den",
				"Cove",
				"Empath Abbey",
				"Jhelom",
				"The Lycaeum",
				"Magincia",
				"Minoc",
				"Moonglow",
				"Nujel'm",
				"Occlo",
				"Serpent's Hold",
				"Skara Brae",
				"Trinsic",
				"Vesper",
				"Wind",
				"Yew",
				"Delucia",
				"Papua"
			};

		public SnowGlobe( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}