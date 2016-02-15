using System;
using Server;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Network;

namespace Server.Engines.Lottery
{
	public class LotteryGump : Gump
	{
		private Mobile m_From;
		private string m_sStatus;

		public LotteryGump( Mobile from, string status ) : base( 40, 40 )
		{
			LotterySystem.CloseAllLotteryGumps(from);

			m_From = from;
			m_sStatus = status;

			this.Closable=true;
			this.Disposable=true;
			this.Dragable=true;
			this.Resizable=false;
			this.AddPage(0);

			LotteryEntry entry = LotterySystem.GetPlayerEntry(m_From);

			this.AddBackground(3, 16, 481, 316, 9380);
			this.AddLabel(219, 45, 0, @"Lottery Tickets");
			this.AddButton(442, 308, 4017, 4018, (int)Buttons.ButtonClose, GumpButtonType.Reply, 0);
			this.AddLabel(403, 308, 0, @"Close");

			this.AddLabel(34, 45, 0, @"Information");
			string info = "";
			info += string.Format("Ticket price: {0}<br><br>", LotterySystem.m_iTicketPrice);
			info += string.Format("<I>Winning List</I><BR>1 Correct: {0}gp<br>2 Correct: {1}gp<br>3 Correct: {2}gp<br>4 Correct: {3}gp<br>", LotterySystem.Prize1, LotterySystem.Prize2, LotterySystem.Prize3, LotterySystem.m_iJackpot);
			info += string.Format("<BR><I>Next Drawing</I><BR>{0} {1}", LotterySystem.m_dtStartTime.ToLongTimeString(), LotterySystem.m_dtStartTime.ToLongDateString());
			this.AddHtml( 32, 63, 178, 230, info, (bool)true, (bool)true);

			this.AddButton(432, 258, 4023, 4024, (int)Buttons.ButtonClear, GumpButtonType.Reply, 0);
			this.AddLabel(324, 258, 0, @"Clear ticket list");
			this.AddHtml( 28, 308, 349, 24, m_sStatus, (bool)false, (bool)false);

			string sTicketStatus = "";
			// Hide buttons when ticket is bought
			if( entry != null && !entry.m_bEnabled )
			{
				this.AddButton(432, 281, 4023, 4024, (int)Buttons.ButtonBuy, GumpButtonType.Reply, 0);
				this.AddLabel(265, 281, 0, @"Buy the tickets in the list");

				this.AddButton(432, 236, 4023, 4024, (int)Buttons.ButtonOK, GumpButtonType.Reply, 0);
				this.AddLabel(324, 236, 0, @"Add a new ticket");

				sTicketStatus = string.Format("Totat cost of tickets: {0}", entry == null ? 0 : LotterySystem.m_iTicketPrice * entry.m_NumberList.Count);
			}
			else
			{
				sTicketStatus = "Please wait for the number drawing to see if you have won.";
			}

			this.AddHtml( 217, 187, 246, 47, sTicketStatus, (bool)true, (bool)false);

			// Format the number list
			this.AddHtml(217, 63, 246, 121, FormatPlayerNumbers(entry, false), (bool)true, (bool)true);
		}

		public enum Buttons
		{
			ButtonClose,
			ButtonOK,
			ButtonBuy,
			ButtonClear,
		}

		/// <summary>
		/// Returns a string with the players numbers.
		/// </summary>
		/// <param name="entry"></param>
		/// <param name="showCorrectNumbers">Should correct numbers be shown</param>
		/// <returns></returns>
		public static string FormatPlayerNumbers(LotteryEntry entry, bool showCorrectNumbers)
		{
			string listText = "";
			if (entry != null && entry.m_NumberList.Count > 0)
			{
				for (int i = 0; i < entry.m_NumberList.Count; i++)
				{
					listText += string.Format("#{0} Numbers: ", i + 1);

					int[] number = (int[])entry.m_NumberList[i];
					for (int singleNumber = 0; singleNumber < number.Length; singleNumber++)
						listText += string.Format("{0} ", number[singleNumber]);

					// This will show the correct numbers
					if (showCorrectNumbers)
						listText += string.Format("  Correct Numbers: {0}<br>", entry.m_CorrectNumbers[i]);
					else
						listText += "<br>";
				}
			}
			return listText;
		}

		public override void OnResponse( NetState sender, RelayInfo info )
		{
			if( !(sender.Mobile is PlayerMobile) )
				return;

			LotteryEntry entry = LotterySystem.GetPlayerEntry(sender.Mobile);

			switch( info.ButtonID )
			{
				case (int)Buttons.ButtonClose:
					return;
				case (int)Buttons.ButtonBuy:
					if( entry != null && entry.m_NumberList.Count > 0 )
					{
						int goldNeeded = LotterySystem.m_iTicketPrice * entry.m_NumberList.Count;
						if (LotterySystem.TakeGold(sender.Mobile, goldNeeded))
						{
							m_sStatus = string.Format( "{0}gp has been removed from your bankbox.", goldNeeded );
							entry.m_bEnabled = true;
							entry.m_dtTicketBought = DateTime.Now;
						}
						else
							m_sStatus = "Not enough gold in your bankbox.";
					}
					else
						m_sStatus = "No tickets have been added to the list!";
					break;
				case (int)Buttons.ButtonClear:
					if( entry != null && entry.m_NumberList.Count > 0 )
					{
						string goldAddedMessage = "";
						if( entry.m_bEnabled )
						{
							int gold = LotterySystem.m_iTicketPrice * entry.m_NumberList.Count;
							LotterySystem.GiveGold(sender.Mobile.BankBox, gold);

							goldAddedMessage = string.Format( "{0}gp added to your bankbox.", gold );
							entry.m_bEnabled = false;
						}
						m_sStatus = string.Format( "Number list cleared. {0}", goldAddedMessage );
						entry.m_NumberList.Clear();
					}
					break;
				case (int)Buttons.ButtonOK:
					if (entry != null && entry.m_NumberList.Count < LotterySystem.MaxTicketsPerPlayer)
					{
						sender.Mobile.SendGump( new LotteryGumpNumbers( sender.Mobile, "" ) );
						return;
					}
					else
						m_sStatus = string.Format("You can not add more than {0} tickets!", LotterySystem.MaxTicketsPerPlayer);
					break;
			}

			sender.Mobile.SendGump( new LotteryGump( sender.Mobile, m_sStatus ) );
		}
	}
}