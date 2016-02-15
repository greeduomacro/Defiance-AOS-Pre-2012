using System;
using Server;
using Server.Items;

namespace Server.Items
{
	public class BagOfArrows : Bag
	{
		public override string DefaultName
		{
			get { return "bag of arrows"; }
		}

		[Constructable]
		public BagOfArrows() : this( 1 )
		{
			Movable = true;
		}

		[Constructable]
		public BagOfArrows( int amount )
		{
			DropItem( new Arrow( amount ) );
			DropItem( new Bolt( amount ) );
		}

		public BagOfArrows( Serial serial ) : base( serial )
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