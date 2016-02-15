using System;
using System.Collections;
using Server.Items;
using Server.Targeting;
using Server.Factions;

namespace Server.Mobiles
{
	[CorpseName( "an ogre lords corpse" )]
	public class FactionOgreLord : OgreLord
	{
		public override Faction FactionAllegiance { get { return Minax.Instance; } }

		[Constructable]
		public FactionOgreLord () : base(){}

		public FactionOgreLord( Serial serial ) : base( serial )
		{
		}

		public override bool IsEnemy( Mobile m )
		{
			if ( m.Player && (Faction.Find(m) == Minax.Instance))
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