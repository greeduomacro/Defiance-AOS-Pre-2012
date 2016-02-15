using System;
using System.Collections;
using System.IO;
using Server;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Network;

namespace Server.Events.CTF
{
	[FlipableAttribute( 0x1e5e, 0x1e5f )]
	public class CTFTopScoreBoard : Item
	{
		[Constructable]
		public CTFTopScoreBoard() : base( 0x1e5e )
		{
			Movable = false;
			Name = "CTF Score Board";
		}

		public CTFTopScoreBoard( Serial serial ) : base(serial)
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
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( !from.InRange( GetWorldLocation(), 2 ) )
				from.SendLocalizedMessage( 500446 ); // That is too far away.
			else
			{
				from.CloseGump( typeof( CTFPlayerDataGump ) );
				from.SendGump(new CTFPlayerDataGump(0));
			}
		}
	}

	[FlipableAttribute(0x1e5e, 0x1e5f)]
	public class CTFGameScoreBoard : Item
	{
		[Constructable]
		public CTFGameScoreBoard() : base(0x1e5e)
		{
			Movable = false;
			Name = "CTF Score Board";
		}

		public CTFGameScoreBoard(Serial serial) : base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write((int)0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();
		}

		public override void OnDoubleClick(Mobile from)
		{
			if (!from.InRange(GetWorldLocation(), 2))
				from.SendLocalizedMessage(500446); // That is too far away.
			else
			{
				from.CloseGump(typeof(CTFAllGamesDataGump));
				from.SendGump(new CTFAllGamesDataGump());
			}
		}
	}
}