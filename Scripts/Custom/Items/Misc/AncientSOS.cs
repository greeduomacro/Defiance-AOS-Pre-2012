using System;
using Server.Network;
using Server.Gumps;

namespace Server.Items
{
	public class AncientSOS : SOS
	{
		[Constructable]
		public AncientSOS() : this( Map.Felucca )
		{
		}

		[Constructable]
		public AncientSOS( Map map ) : base( map, 4 )
		{
		}

		public AncientSOS( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 1 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();

			if (version < 1)
			{
				Level = 4;
				this.UpdateHue();
			}
		}
	}
}