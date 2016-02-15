namespace Server.Items
{
	public class FireAddon : BaseAddon
	{
		public override BaseAddonDeed Deed{ get{ return new FireDeed(); } }

		[Constructable]
		public FireAddon()
		{
			AddComponent( new AddonComponent( 0x19BB ), 0, 0, 0 );
			AddComponent( new AddonComponent( 0x19AB ), 0, 0, 2 );
		}

		public FireAddon( Serial serial ) : base( serial )
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

	public class FireDeed : BaseAddonDeed
	{
		public override BaseAddon Addon{ get{ return new FireAddon(); } }
		public override string DefaultName{ get{ return "a fired brazier deed"; } }

		[Constructable]
		public FireDeed()
		{
			LootType = LootType.Blessed;
		}

		public FireDeed( Serial serial ) : base( serial )
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

	public class CampfireAddon : BaseAddon
	{
		public override BaseAddonDeed Deed{ get{ return new CampfireDeed(); } }

		[Constructable]
		public CampfireAddon()
		{
			AddComponent( new AddonComponent( 0xDE1 ), 0, 0, 0 );
			AddComponent( new AddonComponent( 0xDE3 ), 0, 0, 1 );
		}

		public CampfireAddon( Serial serial ) : base( serial )
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

	public class CampfireDeed : BaseAddonDeed
	{
		public override BaseAddon Addon{ get{ return new CampfireAddon(); } }
		public override string DefaultName{ get{ return "a campfire deed"; } }

		[Constructable]
		public CampfireDeed()
		{
			LootType = LootType.Blessed;
		}

		public CampfireDeed( Serial serial ) : base( serial )
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
}