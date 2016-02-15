using System;
using Server;
using Server.Commands;
using Server.Items;
using Server.SkillHandlers;

namespace Server.Scripts.Commands
{
	public class Drink
	{
		public static void Initialize()
		{
			CommandSystem.Register("Drink", AccessLevel.Player, new CommandEventHandler(Drink_OnCommand));
		}

		private static void ErrorMessage( Mobile from )
		{
			from.SendMessage("Usage: Drink [heal/cure/refresh]");
		}

		[Usage( "Drink" )]
		public static void Drink_OnCommand( CommandEventArgs e )
		{
			Mobile from = e.Mobile;

			Container cont = from.Backpack;
			if( cont == null )
				return;

			Type type = null;
			string text;

			if (e.Arguments.Length > 0)
			{
				text = e.Arguments[0].ToLower();

				switch( text )
				{
					default: ErrorMessage( from ); return;
					case "heal": type = typeof(BaseHealPotion); break;
					case "cure": type = typeof(BaseCurePotion); break;
					case "refresh": type = typeof(BaseRefreshPotion); break;
				}
			}
			else
			{
				ErrorMessage( from );
				return;
			}

			BasePotion potion = (BasePotion)cont.FindItemByType( type );

			if( potion == null )
			{
				from.SendMessage("You don't have any {0} potions.", text);
				return;
			}

			Targeting.Target.Cancel( from );
			potion.OnDoubleClick( from );
		}
	}
}