using System;
using Server;

namespace Server.Items
{
	public class RandomWand
	{
		public static BaseWand CreateWand()
		{
			BaseWand wand = CreateRandomWand();
			if( 0.30 > Utility.RandomDouble() )
				BaseRunicTool.ApplyAttributesTo( wand, false, 0, Utility.RandomMinMax( 1,2 ), 0, 100 );
			return wand;
		}

		public static BaseWand CreateRandomWand( )
		{
			switch ( Utility.Random( 10 ) )
			{
				default:
				case  0: return new ClumsyWand();
				case  1: return new FeebleWand();
				case  2: return new FireballWand();
				case  3: return new GreaterHealWand();
				case  4: return new HarmWand();
				case  5: return new HealWand();
				//case  6: return new IDWand();
				case  6: return new LightningWand();
				case  7: return new MagicArrowWand();
				case  8: return new ManaDrainWand();
				case  9: return new WeaknessWand();
			}
		}
	}
}