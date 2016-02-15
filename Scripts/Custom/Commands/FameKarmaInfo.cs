using System;
using Server;
using Server.Commands;

namespace Server.Scripts.Commands
{
	public class FameKarmaInfo
	{
		public static void Initialize()
		{
			CommandSystem.Register("Fame", AccessLevel.Player, new CommandEventHandler(Fame_OnCommand));
			CommandSystem.Register("Karma", AccessLevel.Player, new CommandEventHandler(Karma_OnCommand));
		}

		[Usage( "Fame" )]
		[Description("Displays your current Fame points.")]
		public static void Fame_OnCommand( CommandEventArgs e )
		{
			Mobile from = e.Mobile;
			from.SendMessage( String.Format("Your current Fame points are: {0}.", from.Fame.ToString() ));
		}

		[Usage( "Karma" )]
		[Description("Displays your current Karma points.")]
		public static void Karma_OnCommand( CommandEventArgs e )
		{
			Mobile from = e.Mobile;
			from.SendMessage( String.Format("Your current Karma points are: {0}.", from.Karma.ToString() ));
		}
	}
}