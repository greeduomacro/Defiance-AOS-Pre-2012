using System;
using System.Reflection;
using System.Collections;
using Server.Items;
using Server.Targeting;
using Server.Mobiles;

namespace Server.Commands
{
	public class Vote
	{
		public static void Initialize()
		{
			CommandSystem.Register("Vote", AccessLevel.Player, new CommandEventHandler(Donate_OnCommand));
		}

		[Usage( "Vote" )]
		[Description( "Links user to the voting gateway." )]
		public static void Donate_OnCommand( CommandEventArgs e )
		{
			string url = "http://www.defianceuo.com/aosvote.htm";

			Mobile m = e.Mobile;
			m.LaunchBrowser( url );
		}
	}
}