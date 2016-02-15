using System;
using Server;

namespace Server.Items
{
	public class BloodPart : Item
	{
		[Constructable]
		public BloodPart() : base( Utility.Random ( 7409, 34 ))
		{
		}

		[Constructable]
		public BloodPart( int itemID ) : base( itemID + 7408 )
		{
		}

		public BloodPart( Serial serial ) : base( serial )
		{
		}

		public override void OnDoubleClick( Mobile from )
		{
			string partLabel = string.Format( "part {0}", ItemID - 7408 );
			MessageHelper.SendLocalizedMessageTo(this, from, 1070722, partLabel, 32);
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