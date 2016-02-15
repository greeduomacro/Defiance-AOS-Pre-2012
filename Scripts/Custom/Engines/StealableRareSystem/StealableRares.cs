using System;
using Server;

namespace Server.Items
{
	public class SRDriedOnions : Item, IStealable
	{
		public double GetMin(){return 80.0;}
		public double GetMax(){return 100.0;}
		public void OnSteal(){}

		public SRDriedOnions() : base( 0xC40 )
		{
			Name = "Dried Onions";
			Stackable = true;
			Movable = false;
		}

		public SRDriedOnions( Serial serial ) : base( serial )
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

			//ItemConversion.AddToRareConversion( this );
		}
	}

	public class SRDriedHerbs : Item, IStealable
	{
		public double GetMin(){return 80.0;}
		public double GetMax(){return 100.0;}
		public void OnSteal(){}

		public SRDriedHerbs() : base( 0xC41 )
		{
			Name = "Dried Herbs";
			Stackable = true;
			Movable = false;
		}

		public SRDriedHerbs( Serial serial ) : base( serial )
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

			//ItemConversion.AddToRareConversion( this );
		}
	}

	public class SRDriedFlowers : Item, IStealable
	{
		public double GetMin(){return 80.0;}
		public double GetMax(){return 100.0;}
		public void OnSteal(){}

		public SRDriedFlowers() : base( 0xC3E )
		{
			Name = "Dried Flowers";
			Stackable = true;
			Movable = false;
		}

		public SRDriedFlowers( Serial serial ) : base( serial )
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

			//ItemConversion.AddToRareConversion( this );
		}
	}

	public class SRHorseShoes : Item, IStealable
	{
		public double GetMin(){return 80.0;}
		public double GetMax(){return 100.0;}
		public void OnSteal(){}

		public SRHorseShoes() : base( 0xFB6 )
		{
			Weight = 2.0;
			Movable = false;
		}

		public SRHorseShoes( Serial serial ) : base( serial )
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

			//ItemConversion.AddToRareConversion( this );
		}
	}

	public class SRArrowShafts : Item, IStealable
	{
		public double GetMin(){return 80.0;}
		public double GetMax(){return 100.0;}
		public void OnSteal(){}

		public SRArrowShafts() : base( 0x1024 )
		{
			Weight = 2.0;
			Movable = false;
		}

		public SRArrowShafts( Serial serial ) : base( serial )
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

			//ItemConversion.AddToRareConversion( this );
		}
	}
	public class SREIronWire : Item, IStealable
	{
		public double GetMin(){return 80.0;}
		public double GetMax(){return 100.0;}
		public void OnSteal(){}

		public SREIronWire() : base( 0x1876 )
		{
			Stackable = true;
			Weight = 2.0;
			Movable = false;
		}

		public SREIronWire( Serial serial ) : base( serial )
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

			//ItemConversion.AddToRareConversion( this );
		}
	}

	public class SRESilverWire : Item, IStealable
	{
		public double GetMin(){return 80.0;}
		public double GetMax(){return 100.0;}
		public void OnSteal(){}

		public SRESilverWire() : base( 0x1877 )
		{
			Stackable = true;
			Weight = 2.0;
			Movable = false;
		}

		public SRESilverWire( Serial serial ) : base( serial )
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

			//ItemConversion.AddToRareConversion( this );
		}
	}

	public class SREGoldWire : Item, IStealable
	{
		public double GetMin(){return 80.0;}
		public double GetMax(){return 100.0;}
		public void OnSteal(){}

		public SREGoldWire() : base( 0x1878 )
		{
			Stackable = true;
			Weight = 2.0;
			Movable = false;
		}

		public SREGoldWire( Serial serial ) : base( serial )
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

			//ItemConversion.AddToRareConversion( this );
		}
	}

	public class SRECopperWire : Item, IStealable
	{
		public double GetMin(){return 80.0;}
		public double GetMax(){return 100.0;}
		public void OnSteal(){}

		public SRECopperWire() : base( 0x1879 )
		{
			Stackable = true;
			Weight = 2.0;
			Movable = false;
		}

		public SRECopperWire( Serial serial ) : base( serial )
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

			//ItemConversion.AddToRareConversion( this );
		}
	}
}