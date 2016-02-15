using System;
using Server;
using Server.Commands;

namespace Server.Scripts.Commands
{
	public class ServerTime
	{
		public static void Initialize()
		{
			CommandSystem.Register("ServerTime", AccessLevel.Player, new CommandEventHandler(ServerTime_OnCommand));
		}

		[Usage( "ServerTime" )]
		public static void ServerTime_OnCommand( CommandEventArgs e )
		{
			Mobile from = e.Mobile;

			from.SendMessage( DateTime.Now.ToString() );
		}
	}
}