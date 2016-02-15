using System;
using Server.Mobiles;
using Server.Factions;

namespace Server.Mobiles
{
	[CorpseName("a silver serpent corpse")]
	[TypeAlias( "Server.Mobiles.Silverserpant" )]
	public class FactionSilverSerpent : SilverSerpent
	{
		public override Faction FactionAllegiance { get { return TrueBritannians.Instance; } }

		[Constructable]
		public FactionSilverSerpent() : base()
		{}

		public FactionSilverSerpent( Serial serial ) : base( serial )
		{
		}

		public override bool IsEnemy( Mobile m )
		{
			if ( m.Player && (Faction.Find(m) == TrueBritannians.Instance))
				return false;

			return base.IsEnemy( m );
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write((int) 0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();

			if ( BaseSoundID == -1 )
				BaseSoundID = 219;
		}
	}
}