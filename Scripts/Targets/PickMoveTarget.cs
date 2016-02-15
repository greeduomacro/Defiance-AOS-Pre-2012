using System;
using Server;
using Server.Targeting;
using Server.Commands;
using Server.Commands.Generic;
using Server.Mobiles;

namespace Server.Targets
{
	public class PickMoveTarget : Target
	{
		public PickMoveTarget() : base( -1, false, TargetFlags.None )
		{
		}

		protected override void OnTarget( Mobile from, object o )
		{
			if ( !BaseCommand.IsAccessible( from, o ) )
			{
				from.SendMessage( "That is not accessible." );
				return;
			}

			if ( ( o is Item || o is Mobile ) && from.AccessLevel > AccessLevel.Counselor ) // Edited by Silver
				from.Target = new MoveTarget( o );
			else if ( o is PlayerMobile )
			{
				if( ((Mobile)o).AccessLevel == AccessLevel.Player )
					from.Target = new MoveTarget( o );
				else
					from.SendMessage( "Counselors may only use this command on players" );
			}
		}
	}
}