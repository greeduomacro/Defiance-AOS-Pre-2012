using System;
using Server;
using Server.Regions;

namespace Server.Misc
{
	public class DoomLogoutHandler
	{
		private static TimeSpan DoomMoveDelay = TimeSpan.FromMinutes( 30.0 );

		public static void Initialize()
		{
			EventSink.Login += new LoginEventHandler( EventSink_Login );
		}

		public static void EventSink_Login( LoginEventArgs e )
		{
			Mobile m = e.Mobile;

			if (m.AccessLevel == AccessLevel.Player && m.LastMoveTime.Ticks > 0 && m.LastMoveTime + DoomMoveDelay < DateTime.Now)
			{
				Region reg = m.Region.GetRegion( "Doom Gauntlet" );
				if ( m.Region != null && m.Region.IsPartOf( "Doom Gauntlet" ) )
				{
					m.Location = new Point3D(2367, 1268, -85);
					m.SendMessage("You have been moved outside doom dungeon.");
				}
			}
		}
	}
}