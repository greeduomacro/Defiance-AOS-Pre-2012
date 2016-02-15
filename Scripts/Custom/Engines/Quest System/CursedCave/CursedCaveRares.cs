using System;

namespace Server.Items
{
	[Flipable( 0x0C24, 0x0C25 )]
	public class SRCRBrokenChestOfDrawers : Item, IStealable
	{
		public override bool ForceShowProperties{get{return true;} }
		public double GetMin(){return 100.0;}
		public double GetMax(){return 150.0;}
		public void OnSteal(){}

		[Constructable]
		public SRCRBrokenChestOfDrawers() : base( 0x0C24 )
		{
			Weight = 10.0;
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );
			list.Add( 1061078, "9" );
		}

		public SRCRBrokenChestOfDrawers( Serial serial ) : base( serial )
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

	public class SRCREmptyToolKit : Item, IStealable
	{
		public override bool ForceShowProperties{get{return true;} }
		public double GetMin(){return 100.0;}
		public double GetMax(){return 150.0;}
		public void OnSteal(){}

		[Constructable]
		public SRCREmptyToolKit() : base( 0x1EB6 )
		{
			Weight = 5.0;
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );
			list.Add( 1061078, "5" );
		}

		public SRCREmptyToolKit( Serial serial ) : base( serial )
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

	public class SRCRBucket : BaseContainer
	{
		public override bool ForceShowProperties{get{return true;} }
		public double GetMin(){return 100.0;}
		public double GetMax(){return 150.0;}
		public void OnSteal(){}

		public override int DefaultGumpID{ get{ return 0x3E; } }
		public override int DefaultDropSound{ get{ return 0x42; } }

		public override Rectangle2D Bounds
		{
			get{ return new Rectangle2D( 33, 36, 109, 112 ); }
		}

		[Constructable]
		public SRCRBucket() : base( 0x14E0 )
		{
			Weight = 5.0;
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );
			list.Add( 1061078, "11" );
		}

		public SRCRBucket( Serial serial ) : base( serial )
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
			if (version == 0)
				reader.ReadBool();
		}
	}

	public class SRCRSmallWeb : Item
	{
		public override bool ForceShowProperties { get { return true; } }
		public double GetMin(){return 100.0;}
		public double GetMax(){return 150.0;}
		public void OnSteal(){}

		[Constructable]
		public SRCRSmallWeb()
			: base(0x10D2)
		{
			Weight = 1.0;
		}

		public override void GetProperties(ObjectPropertyList list)
		{
			base.GetProperties(list);
			list.Add(1061078, "4");
		}

		public SRCRSmallWeb(Serial serial)
			: base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write((int)0); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();
		}
	}

	public class SRCRPluckedChicken : Item, IStealable
	{
		public override bool ForceShowProperties{get{return true;} }
		public double GetMin(){return 100.0;}
		public double GetMax(){return 150.0;}
		public void OnSteal(){}

		[Constructable]
		public SRCRPluckedChicken() : base( 0x1E8B )
		{
			Weight = 3.0;
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );
			list.Add( 1061078, "7" );
		}

		public SRCRPluckedChicken( Serial serial ) : base( serial )
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

	public class SRCRBooks : Item, IStealable
	{
		public override bool ForceShowProperties{get{return true;} }
		public double GetMin(){return 100.0;}
		public double GetMax(){return 150.0;}
		public void OnSteal(){}

		public int[] Hues = new int[] { 36, 41, 46, 51, 141, 436, 441, 446, 451, 836, 841, 851, 946, 1072, 1111	 };

		[Constructable]
		public SRCRBooks() : base( 0x1E21 )
		{
			Weight = 3.0;
			Hue = Hues[Utility.Random(Hues.Length)];
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );
			list.Add( 1061078, "6" );
		}

		public SRCRBooks( Serial serial ) : base( serial )
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

	public class SRCRSturdySmithHammer : SmithHammer, IStealable
	{
		public override bool ForceShowProperties{get{return true;} }
		public double GetMin(){return 100.0;}
		public double GetMax(){return 150.0;}
		public void OnSteal(){}
		public override string DefaultName{ get{ return "sturdy smith's hammer"; } }

		[Constructable]
		public SRCRSturdySmithHammer() : base( Utility.RandomMinMax( 10, 25 ) * 10 )
		{
			Hue = 0x973;
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );
			list.Add( 1061078, "1" );
		}

		public SRCRSturdySmithHammer( Serial serial ) : base( serial )
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
			if (version == 0)
				reader.ReadBool();
		}
	}

	public class SRCRSturdySaw : Saw, IStealable
	{
		public override bool ForceShowProperties{get{return true;} }
		public double GetMin(){return 100.0;}
		public double GetMax(){return 150.0;}
		public void OnSteal(){}
		public override string DefaultName{ get{ return "sturdy saw"; } }

		[Constructable]
		public SRCRSturdySaw() : base( Utility.RandomMinMax( 10, 25 ) * 10 )
		{
			Hue = 0x973;
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );
			list.Add( 1061078, "1" );
		}

		public SRCRSturdySaw( Serial serial ) : base( serial )
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
			if (version == 0)
				reader.ReadBool();
		}
	}

	public class SRCRSturdyProspectorsTool : ProspectorsTool, IStealable
	{
		public override bool ForceShowProperties{get{return true;} }
		public double GetMin(){return 100.0;}
		public double GetMax(){return 150.0;}
		public void OnSteal(){}
		public override string DefaultName{ get{ return "sturdy prospector's tool"; } }

		[Constructable]
		public SRCRSturdyProspectorsTool() : base()
		{
			Hue = 0x973;
			UsesRemaining = Utility.RandomMinMax( 10, 20 ) * 10;
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );
			list.Add( 1061078, "3" );
		}

		public SRCRSturdyProspectorsTool( Serial serial ) : base( serial )
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
			if (version == 0)
				reader.ReadBool();
		}
	}

	public class SRCREmptyTub : Barrel, IStealable
	{
		public override bool ForceShowProperties{get{return true;} }
		public double GetMin(){return 100.0;}
		public double GetMax(){return 150.0;}
		public void OnSteal(){}

		public override int DefaultGumpID { get { return 0x3E; } }
		public override int DefaultDropSound { get { return 0x42; } }

		public override Rectangle2D Bounds
		{
			get { return new Rectangle2D(33, 36, 109, 112); }
		}

		[Constructable]
		public SRCREmptyTub() : base()
		{
			ItemID = 0xE83;
			Weight = 10.0;
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );
			list.Add( 1061078, "9" );
		}

		public SRCREmptyTub( Serial serial ) : base( serial )
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
			if (version == 0)
				reader.ReadBool();
		}
	}

	public class SRCRSturdyMortarPestle : MortarPestle, IStealable
	{
		public override bool ForceShowProperties{get{return true;} }
		public double GetMin(){return 100.0;}
		public double GetMax(){return 150.0;}
		public void OnSteal(){}
		public override string DefaultName{ get{ return "sturdy mortar and pestle"; } }

		[Constructable]
		public SRCRSturdyMortarPestle() : base()
		{
			Hue = 0x973;
			UsesRemaining = Utility.RandomMinMax( 10, 20 ) * 10;
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );
			list.Add( 1061078, "3" );
		}

		public SRCRSturdyMortarPestle( Serial serial ) : base( serial )
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
			if (version == 0)
				reader.ReadBool();
		}
	}

	public class SRCRWallTorchEast : BaseLight, IStealable
	{
		public override bool ForceShowProperties{get{return true;} }
		public double GetMin(){return 100.0;}
		public double GetMax(){return 150.0;}
		public void OnSteal(){}

		public override int LitItemID{ get { return 0xA07; } }
		public override int UnlitItemID{ get { return 0xA05; } }

		[Constructable]
		public SRCRWallTorchEast() : base( 0xA05 )
		{
			Duration = TimeSpan.Zero; // Never burnt out
			Burning = false;
			Light = LightType.Circle225;
			Weight = 3.0;

			Ignite();
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );
			list.Add( 1061078, "5" );
		}

		public SRCRWallTorchEast( Serial serial ) : base( serial )
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
			if (version == 0)
				reader.ReadBool();
		}
	}

	public class SRCRWallTorchSouth : BaseLight, IStealable
	{
		public override bool ForceShowProperties{get{return true;} }
		public double GetMin(){return 100.0;}
		public double GetMax(){return 150.0;}
		public void OnSteal(){}

		public override int LitItemID{ get { return 0xA0C; } }
		public override int UnlitItemID{ get { return 0xA0A; } }

		[Constructable]
		public SRCRWallTorchSouth() : base( 0xA0A )
		{
			Duration = TimeSpan.Zero; // Never burnt out
			Burning = false;
			Light = LightType.Circle225;
			Weight = 3.0;

			Ignite();
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );
			list.Add( 1061078, "5" );
		}

		public SRCRWallTorchSouth( Serial serial ) : base( serial )
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
			if (version == 0)
				reader.ReadBool();
		}
	}
}