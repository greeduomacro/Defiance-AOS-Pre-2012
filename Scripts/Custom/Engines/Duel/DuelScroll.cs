//////////////////////////////\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
//			   This is a Sunny Production ©2005					\\
//					 Based on RunUO©							\\
//					Version: Beta 1.0							\\
//		Many thanks to my Csharp tutors Will and Eclipse		\\
//////////////////////////////\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
//

using Server.Gumps;

namespace Server.Events.Duel
{
	public class DuelScroll : Item
	{
		[Constructable]
		public DuelScroll() : base( 0x227A )
		{
			Hue = 48;
			Name = "Duel Scroll";
			Movable = false;
		}

		public override void OnDoubleClick( Mobile from )
		{
			from.CloseGump( typeof( DuelWelcomeGump ) );
			from.SendGump( new DuelWelcomeGump() );
		}

		public DuelScroll( Serial serial ) : base( serial )
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