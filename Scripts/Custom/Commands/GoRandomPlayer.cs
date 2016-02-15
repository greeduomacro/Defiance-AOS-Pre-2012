using System;
using System.Collections.Generic;
using Server.Network;

namespace Server.Commands
{
	public static class GoRandomPlayer
	{
		public static void Initialize()
		{
			CommandSystem.Register("GRP", AccessLevel.Counselor, new CommandEventHandler(GRP_OnCommand));
		}

		[Usage("GRP")]
		[Description("Staffmember moves to a random online player")]
		public static void GRP_OnCommand(CommandEventArgs e)
		{
			Mobile m = e.Mobile;

			if ( m != null )
			{
				List<NetState> states = NetState.Instances;
				List<NetState> list = new List<NetState>();

				for ( int i = 0; i < states.Count; ++i )
				{
					NetState state = states[i];
					if ( state.Mobile != null && state.Mobile.AccessLevel == AccessLevel.Player )
						list.Add( state );
				}

				if ( list.Count > 0 )
				{
					NetState state = list[Utility.Random( list.Count )];
					if ( state != null )
					{
						Mobile to = state.Mobile;
						if ( to != null )
						{
							m.MoveToWorld( to.Location, to.Map );
							m.SendMessage( String.Format( "You have moved to {0}, at {1} in {2}", to.Name, to.Location, to.Map ) );
							CommandLogging.WriteLine( m, "{0} {1} going to {2} Location {3}, Map {4}", m.AccessLevel, CommandLogging.Format( m ), CommandLogging.Format( to ), to.Location, to.Map );
						}
					}
					else
						m.SendMessage("Random player logged off, please try again.");
				}
			}
		}
	}
}