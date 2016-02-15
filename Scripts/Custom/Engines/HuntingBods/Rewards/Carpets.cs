using System;
using Server;

namespace Server.Items
{
	public class SmallBrownCarpetAddon : BaseAddon
	{
		public override BaseAddonDeed Deed{ get{ return new SmallBrownCarpetDeed(); } }

		[Constructable]
		public SmallBrownCarpetAddon()
		{
			AddComponent( new AddonComponent( 0xAE4 ), -1, -1, 0 );
			AddComponent( new AddonComponent( 0xAE7 ), -1,  0, 0 );
			AddComponent( new AddonComponent( 0xAE5 ), -1,  1, 0 );
			AddComponent( new AddonComponent( 0xAE8 ),  0, -1, 0 );
			AddComponent( new AddonComponent( 0xAEC ),  0,  0, 0 );
			AddComponent( new AddonComponent( 0xAEA ),  0,  1, 0 );
			AddComponent( new AddonComponent( 0xAE6 ),  1, -1, 0 );
			AddComponent( new AddonComponent( 0xAE9 ),  1,  0, 0 );
			AddComponent( new AddonComponent( 0xAE3 ),  1,  1, 0 );
		}

		public SmallBrownCarpetAddon( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.WriteEncodedInt( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadEncodedInt();
		}
	}

	public class SmallBrownCarpetDeed : BaseAddonDeed
	{
		public override BaseAddon Addon{ get{ return new SmallBrownCarpetAddon(); } }
		public override string DefaultName{ get{ return "a small brown carpet deed"; } }

		[Constructable]
		public SmallBrownCarpetDeed()
		{
			LootType = LootType.Blessed;
		}

		public SmallBrownCarpetDeed( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.WriteEncodedInt( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadEncodedInt();
		}
	}

	public class VerySmallBlueCarpetAddon : BaseAddon
	{
		public override BaseAddonDeed Deed{ get{ return new VerySmallBlueCarpetDeed(); } }

		[Constructable]
		public VerySmallBlueCarpetAddon()
		{
			AddComponent( new AddonComponent( 0xAC3 ), -1, -1, 0 );
			AddComponent( new AddonComponent( 0xAF6 ), -1,  0, 0 );
			AddComponent( new AddonComponent( 0xAC4 ), -1,  1, 0 );
			AddComponent( new AddonComponent( 0xAF7 ),  0, -1, 0 );
			AddComponent( new AddonComponent( 0xAFA ),  0,  0, 0 );
			AddComponent( new AddonComponent( 0xAF9 ),  0,  1, 0 );
			AddComponent( new AddonComponent( 0xAC5 ),  1, -1, 0 );
			AddComponent( new AddonComponent( 0xAF8 ),  1,  0, 0 );
			AddComponent( new AddonComponent( 0xAC2 ),  1,  1, 0 );
		}

		public VerySmallBlueCarpetAddon( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.WriteEncodedInt( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadEncodedInt();
		}
	}

	public class VerySmallBlueCarpetDeed : BaseAddonDeed
	{
		public override BaseAddon Addon{ get{ return new VerySmallBlueCarpetAddon(); } }
		public override string DefaultName{ get{ return "a very small blue carpet deed"; } }

		[Constructable]
		public VerySmallBlueCarpetDeed()
		{
			LootType = LootType.Blessed;
		}

		public VerySmallBlueCarpetDeed( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.WriteEncodedInt( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadEncodedInt();
		}
	}

	public class VerySmallRedCarpetAddon : BaseAddon
	{
		public override BaseAddonDeed Deed{ get{ return new VerySmallRedCarpetDeed(); } }

		[Constructable]
		public VerySmallRedCarpetAddon()
		{
			AddComponent( new AddonComponent( 0xACA ), -1, -1, 0 );
			AddComponent( new AddonComponent( 0xACD ), -1,  0, 0 );
			AddComponent( new AddonComponent( 0xACB ), -1,  1, 0 );
			AddComponent( new AddonComponent( 0xACE ),  0, -1, 0 );
			AddComponent( new AddonComponent( 0xAEC ),  0,  0, 0 );
			AddComponent( new AddonComponent( 0xAD0 ),  0,  1, 0 );
			AddComponent( new AddonComponent( 0xACC ),  1, -1, 0 );
			AddComponent( new AddonComponent( 0xACF ),  1,  0, 0 );
			AddComponent( new AddonComponent( 0xAC9 ),  1,  1, 0 );
		}

		public VerySmallRedCarpetAddon( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.WriteEncodedInt( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadEncodedInt();
		}
	}

	public class VerySmallRedCarpetDeed : BaseAddonDeed
	{
		public override BaseAddon Addon{ get{ return new VerySmallRedCarpetAddon(); } }
		public override string DefaultName{ get{ return "a very small red carpet deed"; } }

		[Constructable]
		public VerySmallRedCarpetDeed()
		{
			LootType = LootType.Blessed;
		}

		public VerySmallRedCarpetDeed( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.WriteEncodedInt( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadEncodedInt();
		}
	}

	public class SmallRedCarpetAddon : BaseAddon
	{
		public override BaseAddonDeed Deed{ get{ return new SmallRedCarpetDeed(); } }

		[Constructable]
		public SmallRedCarpetAddon()
		{
			AddComponent( new AddonComponent( 0xACD ), -2, 0, 0 );
			AddComponent( new AddonComponent( 0xAC6 ), -1, 0, 0 );
			AddComponent( new AddonComponent( 0xAC6 ), 0, 0, 0 );
			AddComponent( new AddonComponent( 0xACF ), 1, 0, 0 );
			AddComponent( new AddonComponent( 0xACB ), -2, 1, 0 );
			AddComponent( new AddonComponent( 0xAD0 ), -1, 1, 0 );
			AddComponent( new AddonComponent( 0xAD0 ), 0, 1, 0 );
			AddComponent( new AddonComponent( 0xAC9 ), 1, 1, 0 );
			AddComponent( new AddonComponent( 0xACA ), -2, -1, 0 );
			AddComponent( new AddonComponent( 0xACE ), -1, -1, 0 );
			AddComponent( new AddonComponent( 0xACE ), 0, -1, 0 );
			AddComponent( new AddonComponent( 0xACC ), 1, -1, 0 );
		}

		public SmallRedCarpetAddon( Serial serial ) : base( serial )
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

	public class SmallRedCarpetDeed : BaseAddonDeed
	{
		public override BaseAddon Addon{ get{ return new SmallRedCarpetAddon(); } }
		public override string DefaultName{ get{ return "a small red carpet deed"; } }

		[Constructable]
		public SmallRedCarpetDeed()
		{
			LootType = LootType.Blessed;
		}

		public SmallRedCarpetDeed( Serial serial ) : base( serial )
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

	public class RedCarpetAddon : BaseAddon
	{
		public override BaseAddonDeed Deed{ get{ return new RedCarpetDeed(); } }

		[Constructable]
		public RedCarpetAddon()
		{
			AddComponent( new AddonComponent( 0xACD ), -2, 0, 0 );
			AddComponent( new AddonComponent( 0xAC6 ), -1, 0, 0 );
			AddComponent( new AddonComponent( 0xAC6 ), 0, 0, 0 );
			AddComponent( new AddonComponent( 0xAC6 ), 1, 0, 0 );
			AddComponent( new AddonComponent( 0xACF ), 2, 0, 0 );
			AddComponent( new AddonComponent( 0xACB ), -2, 1, 0 );
			AddComponent( new AddonComponent( 0xAD0 ), -1, 1, 0 );
			AddComponent( new AddonComponent( 0xAD0 ), 0, 1, 0 );
			AddComponent( new AddonComponent( 0xAD0 ), 1, 1, 0 );
			AddComponent( new AddonComponent( 0xAC9 ), 2, 1, 0 );
			AddComponent( new AddonComponent( 0xACA ), -2, -1, 0 );
			AddComponent( new AddonComponent( 0xACE ), -1, -1, 0 );
			AddComponent( new AddonComponent( 0xACE ), 0, -1, 0 );
			AddComponent( new AddonComponent( 0xACE ), 1, -1, 0 );
			AddComponent( new AddonComponent( 0xACC ), 2, -1, 0 );
		}

		public RedCarpetAddon( Serial serial ) : base( serial )
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

	public class RedCarpetDeed : BaseAddonDeed
	{
		public override BaseAddon Addon{ get{ return new RedCarpetAddon(); } }
		public override string DefaultName{ get{ return "a red carpet deed"; } }

		[Constructable]
		public RedCarpetDeed()
		{
			LootType = LootType.Blessed;
		}

		public RedCarpetDeed( Serial serial ) : base( serial )
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

	public class BlueCarpetAddon : BaseAddon
	{
		public override BaseAddonDeed Deed{ get{ return new BlueCarpetDeed(); } }

		[Constructable]
		public BlueCarpetAddon()
		{
			AddComponent( new AddonComponent( 0xAC3 ), -2, -1, 0 );
			AddComponent( new AddonComponent( 0xAF7 ), -1, -1, 0 );
			AddComponent( new AddonComponent( 0xAF7 ), 0, -1, 0 );
			AddComponent( new AddonComponent( 0xAF7 ), 1, -1, 0 );
			AddComponent( new AddonComponent( 0xAC5 ), 2, -1, 0 );

			AddComponent( new AddonComponent( 0xAF6 ), -2, 0, 0 );
			AddComponent( new AddonComponent( 0xABF ), -1, 0, 0 );
			AddComponent( new AddonComponent( 0xABF ), 0, 0, 0 );
			AddComponent( new AddonComponent( 0xABF ), 1, 0, 0 );
			AddComponent( new AddonComponent( 0xAF8 ), 2, 0, 0 );

			AddComponent( new AddonComponent( 0xAC4 ), -2, 1, 0 );
			AddComponent( new AddonComponent( 0xAF9 ), -1, 1, 0 );
			AddComponent( new AddonComponent( 0xAF9 ), 0, 1, 0 );
			AddComponent( new AddonComponent( 0xAF9 ), 1, 1, 0 );
			AddComponent( new AddonComponent( 0xAC2 ), 2, 1, 0 );
		}

		public BlueCarpetAddon( Serial serial ) : base( serial )
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

	public class BlueCarpetDeed : BaseAddonDeed
	{
		public override BaseAddon Addon{ get{ return new BlueCarpetAddon(); } }
		public override string DefaultName{ get{ return "a blue carpet deed"; } }

		[Constructable]
		public BlueCarpetDeed()
		{
			LootType = LootType.Blessed;
		}

		public BlueCarpetDeed( Serial serial ) : base( serial )
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

	public class FancyBlueCarpetAddon : BaseAddon
	{
		public override BaseAddonDeed Deed{ get{ return new FancyBlueCarpetDeed(); } }

		[Constructable]
		public FancyBlueCarpetAddon()
		{
			AddComponent( new AddonComponent( 0xAD3 ), -2, -1, 0 );
			AddComponent( new AddonComponent( 0xAD7 ), -1, -1, 0 );
			AddComponent( new AddonComponent( 0xAD7 ), 0, -1, 0 );
			AddComponent( new AddonComponent( 0xAD7 ), 1, -1, 0 );
			AddComponent( new AddonComponent( 0xAD5 ), 2, -1, 0 );

			AddComponent( new AddonComponent( 0xAD6 ), -2, 0, 0 );
			AddComponent( new AddonComponent( 0xAD1 ), -1, 0, 0 );
			AddComponent( new AddonComponent( 0xAD1 ), 0, 0, 0 );
			AddComponent( new AddonComponent( 0xAD1 ), 1, 0, 0 );
			AddComponent( new AddonComponent( 0xAD8 ), 2, 0, 0 );

			AddComponent( new AddonComponent( 0xAD4 ), -2, 1, 0 );
			AddComponent( new AddonComponent( 0xAD9 ), -1, 1, 0 );
			AddComponent( new AddonComponent( 0xAD9 ), 0, 1, 0 );
			AddComponent( new AddonComponent( 0xAD9 ), 1, 1, 0 );
			AddComponent( new AddonComponent( 0xAD2 ), 2, 1, 0 );
		}

		public FancyBlueCarpetAddon( Serial serial ) : base( serial )
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

	public class FancyBlueCarpetDeed : BaseAddonDeed
	{
		public override BaseAddon Addon{ get{ return new FancyBlueCarpetAddon(); } }
		public override string DefaultName{ get{ return "a fancy blue carpet deed"; } }

		[Constructable]
		public FancyBlueCarpetDeed()
		{
			LootType = LootType.Blessed;
		}

		public FancyBlueCarpetDeed( Serial serial ) : base( serial )
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

	public class FancyRedCarpetAddon : BaseAddon
	{
		public override BaseAddonDeed Deed{ get{ return new FancyRedCarpetDeed(); } }

		[Constructable]
		public FancyRedCarpetAddon()
		{
			AddComponent( new AddonComponent( 0xADC ), -2, -1, 0 );
			AddComponent( new AddonComponent( 0xAE0 ), -1, -1, 0 );
			AddComponent( new AddonComponent( 0xAE0 ), 0, -1, 0 );
			AddComponent( new AddonComponent( 0xAE0 ), 1, -1, 0 );
			AddComponent( new AddonComponent( 0xADE ), 2, -1, 0 );

			AddComponent( new AddonComponent( 0xADF ), -2, 0, 0 );
			AddComponent( new AddonComponent( 0xADA ), -1, 0, 0 );
			AddComponent( new AddonComponent( 0xADA ), 0, 0, 0 );
			AddComponent( new AddonComponent( 0xADA ), 1, 0, 0 );
			AddComponent( new AddonComponent( 0xAE1 ), 2, 0, 0 );

			AddComponent( new AddonComponent( 0xADD ), -2, 1, 0 );
			AddComponent( new AddonComponent( 0xAE2 ), -1, 1, 0 );
			AddComponent( new AddonComponent( 0xAE2 ), 0, 1, 0 );
			AddComponent( new AddonComponent( 0xAE2 ), 1, 1, 0 );
			AddComponent( new AddonComponent( 0xADB ), 2, 1, 0 );
		}

		public FancyRedCarpetAddon( Serial serial ) : base( serial )
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

	public class FancyRedCarpetDeed : BaseAddonDeed
	{
		public override BaseAddon Addon{ get{ return new FancyRedCarpetAddon(); } }
		public override string DefaultName{ get{ return "a fancy red carpet deed"; } }

		[Constructable]
		public FancyRedCarpetDeed()
		{
			LootType = LootType.Blessed;
		}

		public FancyRedCarpetDeed( Serial serial ) : base( serial )
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

	public class RoyalRedCarpetAddon : BaseAddon
	{
		public override BaseAddonDeed Deed{ get{ return new RoyalRedCarpetDeed(); } }

		[Constructable]
		public RoyalRedCarpetAddon()
		{
			AddComponent( new AddonComponent( 0xAE4 ), -2, -1, 0 );
			AddComponent( new AddonComponent( 0xAE8 ), -1, -1, 0 );
			AddComponent( new AddonComponent( 0xAE8 ), 0, -1, 0 );
			AddComponent( new AddonComponent( 0xAE8 ), 1, -1, 0 );
			AddComponent( new AddonComponent( 0xAE6 ), 2, -1, 0 );

			AddComponent( new AddonComponent( 0xAE7 ), -2, 0, 0 );
			AddComponent( new AddonComponent( 0xAEB ), -1, 0, 0 );
			AddComponent( new AddonComponent( 0xAEB ), 0, 0, 0 );
			AddComponent( new AddonComponent( 0xAEB ), 1, 0, 0 );
			AddComponent( new AddonComponent( 0xAE9 ), 2, 0, 0 );

			AddComponent( new AddonComponent( 0xAE5 ), -2, 1, 0 );
			AddComponent( new AddonComponent( 0xAEA ), -1, 1, 0 );
			AddComponent( new AddonComponent( 0xAEA ), 0, 1, 0 );
			AddComponent( new AddonComponent( 0xAEA ), 1, 1, 0 );
			AddComponent( new AddonComponent( 0xAE3 ), 2, 1, 0 );
		}

		public RoyalRedCarpetAddon( Serial serial ) : base( serial )
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

	public class RoyalRedCarpetDeed : BaseAddonDeed
	{
		public override BaseAddon Addon{ get{ return new RoyalRedCarpetAddon(); } }
		public override string DefaultName{ get{ return "a royal red carpet deed"; } }

		[Constructable]
		public RoyalRedCarpetDeed()
		{
			LootType = LootType.Blessed;
		}

		public RoyalRedCarpetDeed( Serial serial ) : base( serial )
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

	public class RoyalBlueCarpetAddon : BaseAddon
	{
		public override BaseAddonDeed Deed{ get{ return new RoyalBlueCarpetDeed(); } }

		[Constructable]
		public RoyalBlueCarpetAddon()
		{
			AddComponent( new AddonComponent( 0xAEF ), -2, -1, 0 );
			AddComponent( new AddonComponent( 0xAF3 ), -1, -1, 0 );
			AddComponent( new AddonComponent( 0xAF3 ), 0, -1, 0 );
			AddComponent( new AddonComponent( 0xAF3 ), 1, -1, 0 );
			AddComponent( new AddonComponent( 0xAF1 ), 2, -1, 0 );

			AddComponent( new AddonComponent( 0xAF2 ), -2, 0, 0 );
			AddComponent( new AddonComponent( 0xAED ), -1, 0, 0 );
			AddComponent( new AddonComponent( 0xAED ), 0, 0, 0 );
			AddComponent( new AddonComponent( 0xAED ), 1, 0, 0 );
			AddComponent( new AddonComponent( 0xAF4 ), 2, 0, 0 );

			AddComponent( new AddonComponent( 0xAF0 ), -2, 1, 0 );
			AddComponent( new AddonComponent( 0xAF5 ), -1, 1, 0 );
			AddComponent( new AddonComponent( 0xAF5 ), 0, 1, 0 );
			AddComponent( new AddonComponent( 0xAF5 ), 1, 1, 0 );
			AddComponent( new AddonComponent( 0xAEE ), 2, 1, 0 );
		}

		public RoyalBlueCarpetAddon( Serial serial ) : base( serial )
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

	public class RoyalBlueCarpetDeed : BaseAddonDeed
	{
		public override BaseAddon Addon{ get{ return new RoyalBlueCarpetAddon(); } }
		public override string DefaultName{ get{ return "a royal blue carpet deed"; } }

		[Constructable]
		public RoyalBlueCarpetDeed()
		{
			LootType = LootType.Blessed;
		}

		public RoyalBlueCarpetDeed( Serial serial ) : base( serial )
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