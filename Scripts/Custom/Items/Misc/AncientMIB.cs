using System;
using Server.Network;
using Server.Items;

namespace Server.Items
{
	public class AncientMIB : MessageInABottle
	{
		[Constructable]
		public AncientMIB() : this( Map.Felucca )
		{
		}

		[Constructable]
		public AncientMIB( Map map ) : base( map )
		{
		}

		public AncientMIB( Serial serial ) : base( serial )
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

		public override void OnDoubleClick(Mobile from)
		{
			if (IsChildOf(from.Backpack))
			{
				Consume();
				from.AddToBackpack(new SOS(Map.Felucca, 4));
				from.LocalOverheadMessage(Network.MessageType.Regular, 0x3B2, 501891); // You extract the message from the bottle.
			}
			else
			{
				from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
			}
		}
	}
}