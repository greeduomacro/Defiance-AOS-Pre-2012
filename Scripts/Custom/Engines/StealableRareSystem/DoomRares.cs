using System;

namespace Server.Items
{
	public class SRDRBloodyWater : Item, IStealable
	{
		public override bool ForceShowProperties{get{return true;} }

		public double GetMin(){return 100.0;}
		public double GetMax(){return 150.0;}
		public void OnSteal(){}

		public SRDRBloodyWater() : base( 0x0E23 )
		{
			Movable = false;
			Weight = 1.0;
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );
			list.Add( 1061078, "5" );
		}

		public SRDRBloodyWater( Serial serial ) : base( serial )
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

			//ItemConversion.AddToRareConversion( this );
		}
	}

	public class SRDRCocoon : Item, IStealable
	{
		public override bool ForceShowProperties{get{return true;} }

		public double GetMin(){return 100.0;}
		public double GetMax(){return 150.0;}
		public void OnSteal(){}

		public SRDRCocoon() : base( 0x10DB )
		{
			Movable = false;
			Weight = 1.0;
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );
			list.Add( 1061078, "7" );
		}

		public SRDRCocoon( Serial serial ) : base( serial )
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

			//ItemConversion.AddToRareConversion( this );
		}
	}

	public class SRDRDamagedBooks : Item, IStealable
	{
		public override bool ForceShowProperties{get{return true;} }

		public double GetMin(){return 100.0;}
		public double GetMax(){return 150.0;}
		public void OnSteal(){}

		public SRDRDamagedBooks() : base( 0xC16 )
		{
			Movable = false;
			Weight = 1.0;
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );
			list.Add( 1061078, "1" );
		}

		public SRDRDamagedBooks( Serial serial ) : base( serial )
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

			//ItemConversion.AddToRareConversion( this );
		}
	}

	public class SRDRRuinedBooks : Item, IStealable
	{
		public override bool ForceShowProperties{get{return true;} }

		public double GetMin(){return 100.0;}
		public double GetMax(){return 150.0;}
		public void OnSteal(){}

		public SRDRRuinedBooks() : base( 0x1E21 )
		{
			Movable = false;
			Weight = 1.0;
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );
			list.Add( 1061078, "3" );
		}

		public SRDRRuinedBooks( Serial serial ) : base( serial )
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

			//ItemConversion.AddToRareConversion( this );
		}
	}

	public class SRDRBottle : Item, IStealable
	{
		public override bool ForceShowProperties{get{return true;} }

		public double GetMin(){return 100.0;}
		public double GetMax(){return 150.0;}
		public void OnSteal(){}

		public SRDRBottle() : base( 0x0E28 )
		{
			Movable = false;
			Weight = 1.0;
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );
			list.Add( 1061078, "1" );
		}

		public SRDRBottle( Serial serial ) : base( serial )
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

			//ItemConversion.AddToRareConversion( this );
		}
	}

	public class SRDRBrazier : Item, IStealable
	{
		public override bool ForceShowProperties{get{return true;} }

		public double GetMin(){return 100.0;}
		public double GetMax(){return 150.0;}
		public void OnSteal(){}

		public SRDRBrazier() : base( 0xE31 )
		{
			Movable = false;
			Weight = 1.0;
			Light = LightType.Circle225;
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );
			list.Add( 1061078, "2" );
		}

		public SRDRBrazier( Serial serial ) : base( serial )
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

			//ItemConversion.AddToRareConversion( this );
		}
	}

	public class SRDRLampPost : Item, IStealable
	{
		public override bool ForceShowProperties{get{return true;} }

		public double GetMin(){return 100.0;}
		public double GetMax(){return 150.0;}
		public void OnSteal(){}

		public SRDRLampPost() : base( 0xB24 )
		{
			Movable = false;
			Weight = 1.0;
			Light = LightType.Circle300;

			if ( 0.08 > Utility.RandomDouble() )
				Hue = Utility.RandomList( m_Hues );
		}

		private static int[] m_Hues = new int[]
			{
				0x47E,
				0x482,
				0x8AB,
				0x963,
				0x594,
				0x558
			};

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );
			list.Add( 1061078, "3" );
		}

		public SRDRLampPost( Serial serial ) : base( serial )
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

			//ItemConversion.AddToRareConversion( this );
		}
	}

	public class SRDRRuinedPainting : Item, IStealable
	{
		public override bool ForceShowProperties{get{return true;} }

		public double GetMin(){return 100.0;}
		public double GetMax(){return 150.0;}
		public void OnSteal(){}

		public SRDRRuinedPainting() : base( 0xC2C )
		{
			Movable = false;
			Weight = 1.0;
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );
			list.Add( 1061078, "12" );
		}

		public SRDRRuinedPainting( Serial serial ) : base( serial )
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

			//ItemConversion.AddToRareConversion( this );
		}
	}

	public class SRDRSaddle : Item, IStealable
	{
		public override bool ForceShowProperties{get{return true;} }

		public double GetMin(){return 100.0;}
		public double GetMax(){return 150.0;}
		public void OnSteal(){}

		public SRDRSaddle() : base( 0xF38 )
		{
			Movable = false;
			Weight = 1.0;
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );
			list.Add( 1061078, "9" );
		}

		public SRDRSaddle( Serial serial ) : base( serial )
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

			//ItemConversion.AddToRareConversion( this );
		}
	}

	public class SRDRTarot : Item, IStealable
	{
		public override bool ForceShowProperties{get{return true;} }

		public double GetMin(){return 100.0;}
		public double GetMax(){return 150.0;}
		public void OnSteal(){}

		public SRDRTarot() : base( 0x12A6 )
		{
			Movable = false;
			Weight = 1.0;
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );
			list.Add( 1061078, "5" );
		}

		public SRDRTarot( Serial serial ) : base( serial )
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

			//ItemConversion.AddToRareConversion( this );
		}
	}

	public class SRDREggCase : Item, IStealable
	{
		public override bool ForceShowProperties{get{return true;} }

		public double GetMin(){return 100.0;}
		public double GetMax(){return 150.0;}
		public void OnSteal(){}

		public SRDREggCase() : base( 0x10D9 )
		{
			Movable = false;
			Weight = 1.0;
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );
			list.Add( 1061078, "5" );
		}

		public SRDREggCase( Serial serial ) : base( serial )
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

			//ItemConversion.AddToRareConversion( this );
		}
	}

	public class SRDRGruesomeStandard : Item, IStealable
	{
		public override bool ForceShowProperties{get{return true;} }

		public double GetMin(){return 100.0;}
		public double GetMax(){return 150.0;}
		public void OnSteal(){}

		public SRDRGruesomeStandard() : base( 0x428 )
		{
			Movable = false;
			Weight = 1.0;
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );
			list.Add( 1061078, "5" );
		}

		public SRDRGruesomeStandard( Serial serial ) : base( serial )
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

			//ItemConversion.AddToRareConversion( this );
		}
	}

	public class SRDRHangingLeatherTunic : Item, IStealable
	{
		public override bool ForceShowProperties{get{return true;} }

		public double GetMin(){return 100.0;}
		public double GetMax(){return 150.0;}
		public void OnSteal(){}

		public SRDRHangingLeatherTunic() : base( 0x13CA )
		{
			Movable = false;
			Weight = 1.0;
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );
			list.Add( 1061078, "9" );
		}

		public SRDRHangingLeatherTunic( Serial serial ) : base( serial )
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

			//ItemConversion.AddToRareConversion( this );
		}
	}

	public class SRDRHangingStuddedLeggings : Item, IStealable
	{
		public override bool ForceShowProperties{get{return true;} }

		public double GetMin(){return 100.0;}
		public double GetMax(){return 150.0;}
		public void OnSteal(){}

		public SRDRHangingStuddedLeggings() : base( 0x13D8 )
		{
			Movable = false;
			Weight = 1.0;
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );
			list.Add( 1061078, "5" );
		}

		public SRDRHangingStuddedLeggings( Serial serial ) : base( serial )
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

			//ItemConversion.AddToRareConversion( this );
		}
	}

	public class SRDRHangingStuddedTunic : Item, IStealable
	{
		public override bool ForceShowProperties{get{return true;} }

		public double GetMin(){return 100.0;}
		public double GetMax(){return 150.0;}
		public void OnSteal(){}

		public SRDRHangingStuddedTunic() : base( 0x13D9 )
		{
			Movable = false;
			Weight = 1.0;
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );
			list.Add( 1061078, "7" );
		}

		public SRDRHangingStuddedTunic( Serial serial ) : base( serial )
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

			//ItemConversion.AddToRareConversion( this );
		}
	}

	public class SRDRRock : Item, IStealable
	{
		public override bool ForceShowProperties{get{return true;} }

		public double GetMin(){return 100.0;}
		public double GetMax(){return 150.0;}
		public void OnSteal(){}

		public SRDRRock() : base( 0x1363 )
		{
			Movable = false;
			Weight = 1.0;
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );
			list.Add( 1061078, "1" );
		}

		public SRDRRock( Serial serial ) : base( serial )
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

			//ItemConversion.AddToRareConversion( this );
		}
	}

	public class SRDRSkinnedDeer : Item, IStealable
	{
		public override bool ForceShowProperties{get{return true;} }

		public double GetMin(){return 100.0;}
		public double GetMax(){return 150.0;}
		public void OnSteal(){}

		public SRDRSkinnedDeer() : base( 0x1E91 )
		{
			Movable = false;
			Weight = 1.0;
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );
			list.Add( 1061078, "7" );
		}

		public SRDRSkinnedDeer( Serial serial ) : base( serial )
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

			//ItemConversion.AddToRareConversion( this );
		}
	}

	public class SRDRSkinnedGoat : Item, IStealable
	{
		public override bool ForceShowProperties{get{return true;} }

		public double GetMin(){return 100.0;}
		public double GetMax(){return 150.0;}
		public void OnSteal(){}

		public SRDRSkinnedGoat() : base( 0x1E88 )
		{
			Movable = false;
			Weight = 1.0;
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );
			list.Add( 1061078, "5" );
		}

		public SRDRSkinnedGoat( Serial serial ) : base( serial )
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

			//ItemConversion.AddToRareConversion( this );
		}
	}

	public class SRDRSkullCandle : Item, IStealable
	{
		public override bool ForceShowProperties{get{return true;} }

		public double GetMin(){return 100.0;}
		public double GetMax(){return 150.0;}
		public void OnSteal(){}

		public SRDRSkullCandle() : base( 0x1854 )
		{
			Movable = false;
			Weight = 1.0;
			Light = LightType.Circle150;
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );
			list.Add( 1061078, "1" );
		}

		public SRDRSkullCandle( Serial serial ) : base( serial )
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

			//ItemConversion.AddToRareConversion( this );
		}
	}

	public class SRDRStackedBooks2 : Item, IStealable
	{
		public override bool ForceShowProperties{get{return true;} }

		public double GetMin(){return 100.0;}
		public double GetMax(){return 150.0;}
		public void OnSteal(){}

		public SRDRStackedBooks2() : base( 0x1E24 )
		{
			Movable = false;
			Weight = 1.0;
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );
			list.Add( 1061078, "3" );
		}

		public SRDRStackedBooks2( Serial serial ) : base( serial )
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

			//ItemConversion.AddToRareConversion( this );
		}
	}

	public class SRDRStackedBooks : Item, IStealable
	{
		public override bool ForceShowProperties{get{return true;} }

		public double GetMin(){return 100.0;}
		public double GetMax(){return 150.0;}
		public void OnSteal(){}

		public SRDRStackedBooks() : base( 0x1E25 )
		{
			Movable = false;
			Weight = 1.0;
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );
			list.Add( 1061078, "3" );
		}

		public SRDRStackedBooks( Serial serial ) : base( serial )
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

			//ItemConversion.AddToRareConversion( this );
		}
	}

	public class SRDRStretchedHide : Item, IStealable
	{
		public override bool ForceShowProperties{get{return true;} }

		public double GetMin(){return 100.0;}
		public double GetMax(){return 150.0;}
		public void OnSteal(){}

		public SRDRStretchedHide() : base( 0x106B )
		{
			Movable = false;
			Weight = 1.0;
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );
			list.Add( 1061078, "2" );
		}

		public SRDRStretchedHide( Serial serial ) : base( serial )
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

			//ItemConversion.AddToRareConversion( this );
		}
	}

	public class SRDRReversedBackPack : BaseContainer, IStealable
	{
		public override bool ForceShowProperties{get{return true;} }

		public double GetMin(){return 100.0;}
		public double GetMax(){return 150.0;}
		public void OnSteal(){m_bStolen = true;}

		public bool m_bStolen;

		public override int DefaultGumpID{ get{ return 0x3C; } }
		public override int DefaultDropSound{ get{ return 0x48; } }

		public override Rectangle2D Bounds
		{
			get{ return new Rectangle2D( 44, 65, 142, 94 ); }
		}

		public SRDRReversedBackPack() : base( 0x09B2 )
		{
			Movable = false;
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );
			list.Add( 1061078, "5" );
		}

		public SRDRReversedBackPack( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
			writer.Write( (bool) m_bStolen );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			//ItemConversion.AddToRareConversion( this );
			m_bStolen = reader.ReadBool();
		}
	}
}