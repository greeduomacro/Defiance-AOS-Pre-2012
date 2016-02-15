using System;
using Server;

namespace Server.Items
{
	public class CaveHole : BaseAddon
	{
		private static int[] compId = new int[]
		{
			0x56A,0x54D,0x548,
			0x54A,0x53C,0x546,
			0x547,0x543,0x551
		};

		[Constructable]
		public CaveHole()
		{
			int loc=0;
			AddonComponent[] comps = new AddonComponent[9];
			for(int a=0;a<3;a++)
				for(int b=0;b<3;b++)
				{
					if(loc == 4)
						AddComponent( new CaveHoleComponent(compId[loc]), b-1, a-1, 0 );
					else
					{
						comps[loc] = new AddonComponent(compId[loc]);
						comps[loc].Hue = 1;
						comps[loc].Name = "hole";
						AddComponent( comps[loc], b-1, a-1, 0 );
					}
					loc++;
				}
		}

		public CaveHole( Serial serial ) : base( serial )
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