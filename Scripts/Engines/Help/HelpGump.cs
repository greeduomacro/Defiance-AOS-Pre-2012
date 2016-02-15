using System;
using Server;
using Server.Gumps;
using Server.Network;
using Server.Menus;
using Server.Menus.Questions;
using Server.Accounting;
using Server.Multis;
using Server.Mobiles;

namespace Server.Engines.Help
{
	public class ContainedMenu : QuestionMenu
	{
		private Mobile m_From;

		public ContainedMenu( Mobile from ) : base( "You already have an open help request. We will have someone assist you as soon as possible.  What would you like to do?", new string[]{ "Leave my old help request like it is.", "Remove my help request from the queue." } )
		{
			m_From = from;
		}

		public override void OnCancel( NetState state )
		{
			m_From.SendLocalizedMessage( 1005306, "", 0x35 ); // Help request unchanged.
		}

		public override void OnResponse( NetState state, int index )
		{
			if ( index == 0 )
			{
				m_From.SendLocalizedMessage( 1005306, "", 0x35 ); // Help request unchanged.
			}
			else if ( index == 1 )
			{
				PageEntry entry = PageQueue.GetEntry( m_From );

				if ( entry != null && entry.Handler == null )
				{
					m_From.SendLocalizedMessage( 1005307, "", 0x35 ); // Removed help request.
					entry.AddResponse( entry.Sender, "[Canceled]" );
					PageQueue.Remove( entry );
				}
				else
				{
					m_From.SendLocalizedMessage( 1005306, "", 0x35 ); // Help request unchanged.
				}
			}
		}
	}

	public class HelpGump : Gump
	{
		public static void Initialize()
		{
			EventSink.HelpRequest += new HelpRequestEventHandler( EventSink_HelpRequest );
		}

		private static void EventSink_HelpRequest( HelpRequestEventArgs e )
		{
			foreach ( Gump g in e.Mobile.NetState.Gumps )
			{
				if ( g is HelpGump )
					return;
			}

			if ( !PageQueue.CheckAllowedToPage( e.Mobile ) )
				return;

			if ( PageQueue.Contains( e.Mobile ) )
				e.Mobile.SendMenu( new ContainedMenu( e.Mobile ) );
			else
				e.Mobile.SendGump( new HelpGump( e.Mobile ) );
		}

		private static bool IsYoung( Mobile m )
		{
			if ( m is PlayerMobile )
				return ((PlayerMobile)m).Young;

			return false;
		}

		public static bool CheckCombat( Mobile m )
		{
			for ( int i = 0; i < m.Aggressed.Count; ++i )
			{
				AggressorInfo info = m.Aggressed[i];

				if ( DateTime.Now - info.LastCombatTime < TimeSpan.FromSeconds( 30.0 ) )
					return true;
			}

			return false;
		}

		public HelpGump( Mobile from ) : base( 0, 0 )
		{
			from.CloseGump( typeof( HelpGump ) );

			bool isYoung = IsYoung( from );

			AddBackground( 50, 25, 540, 495, 2600 );

			AddPage( 0 );

			AddHtmlLocalized( 150, 50, 360, 40, 1001002, false, false ); // <CENTER><U>Ultima Online Help Menu</U></CENTER>
			AddButton( 425, 415, 2073, 2072, 0, GumpButtonType.Reply, 0 ); // Close

			AddButton( 80, 405, 5540, 5541, 10, GumpButtonType.Reply, 2 );
			AddHtml(110, 405, 450, 58, @"<u>Defiance Network Home Page</u>", false, false);

			AddButton( 80, 430, 5540, 5541, 11, GumpButtonType.Reply, 2 );
			AddHtml(110, 430, 450, 58, @"<u>Defiance Forums</u>", false, false);

			AddButton( 80, 455, 5540, 5541, 12, GumpButtonType.Reply, 2 );
			AddHtml(110, 455, 450, 58, @"<u>Defiance Wiki</u>", false, false);

			AddButton( 80, 480, 5540, 5541, 13, GumpButtonType.Reply, 2 );
			AddHtml(110, 480, 450, 58, @"Vote for DefianceUO AOS at <u>Ultima Online Top 200</u>", false, false);

			AddPage( 1 );

			if ( isYoung )
			{
				AddButton( 80, 75, 5540, 5541, 9, GumpButtonType.Reply, 2 );
				// *** Changed Code ***
				AddHtml(110, 75, 450, 58, @"<BODY><BASEFONT COLOR=BLACK><u>Young Player Britain Transport.</u> Select this option if you want to be transported to Britain.</BODY>", true, true);
				// *** ***

				AddButton( 80, 140, 5540, 5541, 1, GumpButtonType.Reply, 2 );
				AddHtml( 110, 140, 450, 58, @"<u>General question about Ultima Online.</u> Select this option if you have a general gameplay question, need help learning to use a skill, or if you would like to search the UO Knowledge Base.", true, true );

				AddButton( 80, 205, 5540, 5541, 2, GumpButtonType.Reply, 0 );
				AddHtml( 110, 205, 450, 58, @"<u>My character is physically stuck in the game.</u> You will be teleported to a town of your choice if you are not inside a dungeon. This option will only work two times in 24 hours.
If you have used it already or if you are inside a dungeon region, you can send a message to online staff. Do NOT use this option if you are just lost, not stuck.", true, true );

				AddButton( 80, 270, 5540, 5541, 0, GumpButtonType.Page, 3 );
				AddHtml( 110, 270, 450, 58, @"<u>Another player is harassing me.</u> Another player is verbally harassing your character. When you select this option you will be sending a text log to currently online staff. Make sure you did not use bad language as well to provoke the other player.", true, true );

				AddButton( 80, 335, 5540, 5541, 0, GumpButtonType.Page, 2 );
				AddHtml( 110, 335, 450, 58, @"<u>Other.</u> If you are experiencing a problem in the game that does not fall into one of the other categories or you cannot find information in our forums http://www.defianceuo.com/forum, please use this option.", true, true );
			}
			else
			{
				AddButton( 80, 90, 5540, 5541, 1, GumpButtonType.Reply, 2 );
				AddHtml( 110, 90, 450, 74, @"<u>General question about Ultima Online.</u> Select this option if you have a general gameplay question, need help learning to use a skill, or if you would like to search the UO Knowledge Base.", true, true );

				AddButton( 80, 170, 5540, 5541, 2, GumpButtonType.Reply, 0 );
				AddHtml( 110, 170, 450, 74, @"<u>My character is physically stuck in the game.</u> You will be teleported to a town of your choice if you are not inside a dungeon. This option will only work two times in 24 hours.
If you have used it already or if you are inside a dungeon region, you can send a message to online staff. Do NOT use this option if you are just lost, not stuck.", true, true );

				AddButton( 80, 250, 5540, 5541, 0, GumpButtonType.Page, 3 );
				AddHtml( 110, 250, 450, 74, @"<u>Another player is harassing me.</u> Another player is verbally harassing your character. When you select this option you will be sending a text log to currently online staff. Make sure you did not use bad language as well to provoke the other player.", true, true );

				AddButton( 80, 330, 5540, 5541, 0, GumpButtonType.Page, 2 );
				AddHtml( 110, 330, 450, 74, @"<u>Other.</u> If you are experiencing a problem in the game that does not fall into one of the other categories or you cannot find information in our forums http://www.defianceuo.com/forum, please use this option.", true, true );
			}

			AddPage( 2 );

			AddButton( 80, 90, 5540, 5541, 3, GumpButtonType.Reply, 0 );
			AddHtml( 110, 90, 450, 74, @"<u>Report a bug</u> Use this option to launch your web browser at http://bug.casiopia.net. Your report will be read by our Developers. You may also write a message to currently online staff (if any). ", true, true );

			AddButton( 80, 170, 5540, 5541, 4, GumpButtonType.Reply, 0 );
			AddHtml( 110, 170, 450, 74, @"<u>Suggestion for the Game.</u> If you'd like to make a suggestion for the game, it should be directed to the Development Team Members who participate in the discussion forums on the DefianceUO.com web site. Choosing this option will take you to the Suggestions Forum. You may also write a message to currently online staff (if any). ", true, true );

			AddButton( 80, 250, 5540, 5541, 5, GumpButtonType.Reply, 0 );
			AddHtml( 110, 250, 450, 74, @"<u>Account Management</u> Choosing this option will open your browser at the Account Management section of our site. You may also write a message to currently online staff (if any).", true, true );

			AddButton( 80, 330, 5540, 5541, 6, GumpButtonType.Reply, 0 );
			AddHtml( 110, 330, 450, 74, @"<u>Other.</u> If you are experiencing a problem in the game that does not fall into one of the other categories and you cannot find help in our forums (located at http://www.defianceuo.com/forum), and requires in-game assistance, use this option. ", true, true );

			AddPage( 3 );

			AddButton( 80, 90, 5540, 5541, 7, GumpButtonType.Reply, 0 );
			AddHtmlLocalized( 110, 90, 450, 145, 1062572, true, true ); /* <U><CENTER>Another player is harassing me (or Exploiting).</CENTER></U><BR>
																		 * VERBAL HARASSMENT<BR>
																		 * Use this option when another player is verbally harassing your character.
																		 * Verbal harassment behaviors include but are not limited to, using bad language, threats etc..
																		 * Before you submit a complaint be sure you understand what constitutes harassment
																		 * <A HREF="http://uo.custhelp.com/cgi-bin/uo.cfg/php/enduser/std_adp.php?p_faqid=40">� what is verbal harassment? -</A>
																		 * and that you have followed these steps:<BR>
																		 * 1. You have asked the player to stop and they have continued.<BR>
																		 * 2. You have tried to remove yourself from the situation.<BR>
																		 * 3. You have done nothing to instigate or further encourage the harassment.<BR>
																		 * 4. You have added the player to your ignore list.
																		 * <A HREF="http://uo.custhelp.com/cgi-bin/uo.cfg/php/enduser/std_adp.php?p_faqid=138">- How do I ignore a player?</A><BR>
																		 * 5. You have read and understand Origin�s definition of harassment.<BR>
																		 * 6. Your account information is up to date. (Including a current email address)<BR>
																		 * *If these steps have not been taken, GMs may be unable to take action against the offending player.<BR>
																		 * **A chat log will be review by a GM to assess the validity of this complaint.
																		 * Abuse of this system is a violation of the Rules of Conduct.<BR>
																		 * EXPLOITING<BR>
																		 * Use this option to report someone who may be exploiting or cheating.
																		 * <A HREF="http://uo.custhelp.com/cgi-bin/uo.cfg/php/enduser/std_adp.php?p_faqid=41">� What constitutes an exploit?</a>
																		 */

			AddButton( 80, 240, 5540, 5541, 8, GumpButtonType.Reply, 0 );
			AddHtmlLocalized( 110, 240, 450, 145, 1062573, true, true ); /* <U><CENTER>Another player is harassing me using game mechanics.</CENTER></U><BR>
																		  * <BR>
																		  * PHYSICAL HARASSMENT<BR>
																		  * Use this option when another player is harassing your character using game mechanics.
																		  * Physical harassment includes but is not limited to luring, Kill Stealing, and any act that causes a players death in Trammel.
																		  * Before you submit a complaint be sure you understand what constitutes harassment
																		  * <A HREF="http://uo.custhelp.com/cgi-bin/uo.cfg/php/enduser/std_adp.php?p_faqid=59"> � what is physical harassment?</A>
																		  * and that you have followed these steps:<BR>
																		  * 1. You have asked the player to stop and they have continued.<BR>
																		  * 2. You have tried to remove yourself from the situation.<BR>
																		  * 3. You have done nothing to instigate or further encourage the harassment.<BR>
																		  * 4. You have added the player to your ignore list.
																		  * <A HREF="http://uo.custhelp.com/cgi-bin/uo.cfg/php/enduser/std_adp.php?p_faqid=138"> - how do I ignore a player?</A><BR>
																		  * 5. You have read and understand Origin�s definition of harassment.<BR>
																		  * 6. Your account information is up to date. (Including a current email address)<BR>
																		  * *If these steps have not been taken, GMs may be unable to take action against the offending player.<BR>
																		  * **This issue will be reviewed by a GM to assess the validity of this complaint.
																		  * Abuse of this system is a violation of the Rules of Conduct.
																		  */

			AddButton( 150, 390, 5540, 5541, 0, GumpButtonType.Page, 1 );
			AddHtmlLocalized( 180, 390, 335, 40, 1001015, false, false ); // NO  - I meant to ask for help with another matter.
		}

		public override void OnResponse( NetState state, RelayInfo info )
		{
			Mobile from = state.Mobile;

			PageType type = (PageType)(-1);

			switch ( info.ButtonID )
			{
				case 0: // Close/Cancel
				{
					from.SendLocalizedMessage( 501235, "", 0x35 ); // Help request aborted.

					break;
				}
				case 1: // General question
				{
					type = PageType.Question;
					break;
				}
				case 2: // Stuck
				{
					BaseHouse house = BaseHouse.FindHouseAt( from );

					if ( house != null && house.IsAosRules )
					{
						from.Location = house.BanLocation;
					}
					else if ( from.Region.IsPartOf( typeof( Server.Regions.Jail ) ) )
					{
						from.SendLocalizedMessage( 1041530, "", 0x35 ); // You'll need a better jailbreak plan then that!
					}
					else if ( Factions.Sigil.ExistsOn( from ) )
					{
						from.SendLocalizedMessage( 1061632 ); // You can't do that while carrying the sigil.
					}
					else if ( from.CanUseStuckMenu() && from.Region.CanUseStuckMenu( from ) && !CheckCombat( from ) && !from.Frozen && !from.Criminal && (Core.AOS || from.Kills < 5) )
					{
						StuckMenu menu = new StuckMenu( from, from, true );

						menu.BeginClose();

						from.SendGump( menu );
					}
					else
					{
						type = PageType.Stuck;
					}

					break;
				}
				case 3: // Report bug
				{
					from.LaunchBrowser( "http://bug.casiopia.net/" );
					type = PageType.Bug;
					break;
				}
				case 4: // Game suggestion
				{
					from.LaunchBrowser( "http://www.defianceuo.com/forum/forumdisplay.php?f=232" );
					type = PageType.Suggestion;
					break;
				}
				case 5: // Account management
				{
					from.LaunchBrowser( "http://accounts.defianceuo.com/pwchange.php" );
					type = PageType.Account;
					break;
				}
				case 6: // Other
				{
					type = PageType.Other;
					break;
				}
				case 7: // Harassment: verbal/exploit
				{
					type = PageType.VerbalHarassment;
					break;
				}
				case 8: // Harassment: physical
				{
					type = PageType.PhysicalHarassment;
					break;
				}
				case 9: // Young player transport
				{
					if ( IsYoung( from ) )
					{
						if ( from.Region.IsPartOf( typeof( Regions.Jail ) ) )
						{
							from.SendLocalizedMessage( 1041530, "", 0x35 ); // You'll need a better jailbreak plan then that!
						}
						// *** Changed Code ***
						else if ( from.Region.IsPartOf( "Britain" ) )
						{
							from.SendMessage("You're already in Britain");
						}
						else
						{
							from.MoveToWorld(new Point3D(1519, 1619, 10), Map.Felucca);
						}
						// *** ***
					}
					break;
				}
				case 10: // Dfi homepage
				{
					from.LaunchBrowser( "http://www.defianceuo.com/main.php" );
					break;
				}
				case 11: // Dfi forums
				{
					from.LaunchBrowser( "http://www.defianceuo.com/forum/" );
					break;
				}
				case 12: // Dfi wiki
				{
					from.LaunchBrowser( "http://www.mydotdot.com/dfiwiki/index.php?title=Main_Page" );
					break;
				}
				case 13: // Ultima Online Top 200
				{
					from.LaunchBrowser( "http://www.gamesites200.com/ultimaonline/in.php?id=1298" );
					break;
				}
			}

			if ( type != (PageType)(-1) && PageQueue.CheckAllowedToPage( from ) )
				from.SendGump( new PagePromptGump( from, type ) );
		}
	}
}