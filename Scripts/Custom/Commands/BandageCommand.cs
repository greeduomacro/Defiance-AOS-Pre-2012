using System;
using Server.Commands;
using System.Collections;
using Server;
using Server.Items;
using Server.Mobiles;

namespace Server.Scripts.Commands
{
	public class Band
	{
		public static void Initialize()
		{
			CommandSystem.Register("Band", AccessLevel.Player, new CommandEventHandler(Band_OnCommand));
		}
		public static void Band_OnCommand( CommandEventArgs e )
		{
			Bandage m_Bandage = (Bandage)e.Mobile.Backpack.FindItemByType( typeof( Bandage ) );

			int m_Exists = e.Mobile.Backpack.GetAmount( typeof( Bandage ) );
			if ( m_Exists == 0 )
				e.Mobile.SendMessage( "Cannot find bandage" );
			else
			{
				e.Mobile.SendMessage( "Bandage found" );
				m_Bandage.OnDoubleClick(e.Mobile);
			}
		}
	}

	public class BandSelf
	{
		public static void Initialize()
		{
			CommandSystem.Register("BandSelf", AccessLevel.Player, new CommandEventHandler(BandSelf_OnCommand));
		}

		[Usage( "BandSelf" )]
		public static void BandSelf_OnCommand( CommandEventArgs e )
		{
			Mobile from = e.Mobile;
			if (from==null) return;
			Container bp = from.Backpack;
			if (bp==null) return;
			Bandage b = (Bandage)bp.FindItemByType( typeof(Bandage));
			if (b==null)
			{
				from.SendMessage("You must have bandages in your backpack to heal!");
				return;
			}
			from.RevealingAction();
			BandageContext.BeginHeal( from, from);
			b.Consume();
		}
	}
}