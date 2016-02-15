using System;
using System.Collections.Generic;
using Server.Network;
using Server.Mobiles;

namespace Server.Commands
{
	public static class GlobalSkillFix
	{
		public static void Initialize()
		{
			CommandSystem.Register("GlobalSkillFix", AccessLevel.Counselor, new CommandEventHandler(GlobalSkillFix_OnCommand));
		}

		[Usage("GlobalSkillFix")]
		[Description("Globally fixes players overcapped skills.")]
		public static void GlobalSkillFix_OnCommand(CommandEventArgs e)
		{
			Mobile m = e.Mobile;

			foreach ( Mobile mob in World.Mobiles.Values )
			{
				if ( mob is PlayerMobile )
				{
					PlayerMobile player = mob as PlayerMobile;

					if ( player.AccessLevel == AccessLevel.Player && player.SkillsTotal > player.SkillsCap )
						ApplyFix( player );
				}
			}

			m.SendMessage( String.Format( "You have fixed the overcapped skills of all players" ) );
			CommandLogging.WriteLine( m, "{0} {1} globally fixed overcapped skills", m.AccessLevel, CommandLogging.Format( m ) );
		}

		private static void ApplyFix( PlayerMobile player )
		{
			player.Skills[52].BaseFixedPoint = 0; // Bushido
			player.Skills[53].BaseFixedPoint = 0; // Ninjitsu
			player.Skills[54].BaseFixedPoint = 0; // SpellWeaving

			for ( int i = 0; i < 52; i++ )
			{
				int diff = player.SkillsTotal - player.SkillsCap;

				if( diff < 0 )
					diff = 0;

				if( player.Skills[i].BaseFixedPoint >= diff )
				{
					player.Skills[i].BaseFixedPoint -= diff;
					break;
				}
				else
					player.Skills[i].BaseFixedPoint = 0;
			}
		}
	}
}