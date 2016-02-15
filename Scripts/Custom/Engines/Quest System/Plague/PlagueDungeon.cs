/*
using System;
using System.Collections;
using Server;
using Server.Mobiles;
using Server.Spells;
using Server.Spells.Seventh;
using Server.Spells.Fourth;
using Server.Spells.Sixth;
using Server.Spells.Chivalry;

namespace Server.Regions
{
	public class PlagueDungeon : Region
	{
		public static void Initialize()
		{
			Region.AddRegion( new PlagueDungeon( "Plague", new Rectangle2D( 6028, 397, 103, 101 ) ) );
		}

		public PlagueDungeon( string name, Rectangle2D locRect ) : base( "", name, Map.Trammel )
		{
			Priority = Region.HighestPriority;
			LoadFromXml = false;

			Coords = new ArrayList( 1 );
			Coords.Add( locRect );
		}

		public override bool AllowHousing( Mobile from, Point3D p )
		{
			return false;
		}

		public override bool OnBeginSpellCast( Mobile m, ISpell s )
		{
			if ( s is GateTravelSpell || s is MarkSpell || s is RecallSpell || s is SacredJourneySpell  )
			{
				m.SendMessage( "You cannot cast that spell here." );
				return false;
			}

			return base.OnBeginSpellCast( m, s );
		}

		public override void AlterLightLevel( Mobile m, ref int global, ref int personal )
		{
			global = LightCycle.NightLevel;
		}
	}
}
*/