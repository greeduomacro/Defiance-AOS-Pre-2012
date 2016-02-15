using System;
using Server;
using Server.Items;
using Server.Factions;

namespace Server.Mobiles
{
	[CorpseName( "a daemon corpse" )]
	public class FactionDaemon : Daemon
	{

		public override Faction FactionAllegiance { get { return Shadowlords.Instance; } }
		[Constructable]
		public FactionDaemon () : base()
		{}

		public FactionDaemon( Serial serial ) : base( serial )
		{
		}

		public override bool IsEnemy( Mobile m )
		{
			if ( m.Player && (Faction.Find(m) == Shadowlords.Instance))
				return false;

			return base.IsEnemy( m );
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
	}
}