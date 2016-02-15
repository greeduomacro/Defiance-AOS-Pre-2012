using System;
using Server;
using Server.Misc;
using Server.Items;
using Server.Factions;

namespace Server.Mobiles
{
	[CorpseName( "a wisp corpse" )]
	public class FactionWisp : Wisp
	{
		public override Faction FactionAllegiance{ get{ return CouncilOfMages.Instance; } }

		[Constructable]
		public FactionWisp() : base()
		{
			FightMode = FightMode.Closest;
		}

		public FactionWisp( Serial serial ) : base( serial )
		{
		}

		public override bool IsEnemy( Mobile m )
		{
			if ( m.Player && (Faction.Find(m) == CouncilOfMages.Instance))
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