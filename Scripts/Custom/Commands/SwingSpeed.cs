using System;
using Server;
using Server.Commands;
using Server.Items;

namespace Server.Scripts.Commands
{
	public class SwingSpeed
	{
		public static void Initialize()
		{
			CommandSystem.Register("SwingSpeed", AccessLevel.Player, new CommandEventHandler(Bonuses_OnCommand));
		}

		[Usage( "SwingSpeed" )]
		[Description("Displays your current swing speed")]
		public static void Bonuses_OnCommand( CommandEventArgs e )
		{
			Mobile from = e.Mobile;

			TimeSpan delayInSeconds = (from.Weapon as BaseWeapon).GetDelay(from) < TimeSpan.FromSeconds(1.25) ? TimeSpan.FromSeconds(1.25) : (from.Weapon as BaseWeapon).GetDelay(from);

			from.SendMessage( String.Format("Your swing speed is {0} seconds", new DateTime(delayInSeconds.Ticks).ToString("s.ff") ) );
		}
	}
}