using System;
using Server;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Network;

namespace Server.Engines.Lottery
{
	public class LotteryGumpInfo : Gump
	{
		public LotteryGumpInfo( Mobile from, LotteryEntry entry ) : base( 0, 0 )
		{
			LotterySystem.CloseAllLotteryGumps(from);

			this.Closable=true;
			this.Disposable=true;
			this.Dragable=true;
			this.Resizable=false;
			this.AddPage(0);
			this.AddBackground(6, 10, 468, 276, 9380);
			this.AddLabel(191, 14, 0, @"Lottery Report");
			this.AddButton(422, 263, 4017, 4018, (int)Buttons.ButtonClose, GumpButtonType.Reply, 0);
			this.AddLabel(382, 263, 0, @"Close");

			string info = "";

			info += string.Format("Lottery report from {0}.<br><br>", entry.m_dtTicketBought);
			info += string.Format("You Won {0}gp.<br><br>", entry.m_iWinMoney);

			// Show winning Number
			info += "Winning Numbers: ";
			foreach( int winningNumber in entry.m_iWinningNumbers )
				info += string.Format("{0} ", winningNumber);
			info += "<br>";

			// Show Player Numbers
			info += "<br>Your Numbers:<br>";
			info += LotteryGump.FormatPlayerNumbers(entry, true);

			// Show Global correct numbers
			info += "<br>Global Correct Numbers:<br>";
			for( int sortedNum=0;sortedNum<entry.m_TotalCorrectNumbers.Length;sortedNum++ )
				info += string.Format( "{0} correct numbers: {1}<br>", sortedNum, entry.m_TotalCorrectNumbers[sortedNum] );

			this.AddHtml( 33, 47, 417, 208, info, (bool)true, (bool)true);

			// Do not show info again
			if( entry != null )
				entry.m_bShowWinnings = false;
		}

		public enum Buttons
		{
			Reserved,
			ButtonClose,
		}

		public override void OnResponse( NetState sender, RelayInfo info )
		{
			if( !(sender.Mobile is PlayerMobile) )
				return;

			switch (info.ButtonID)
			{
				case (int)Buttons.Reserved:
					return;
			}

			sender.Mobile.SendGump( new LotteryGump( (PlayerMobile)sender.Mobile, "" ) );
		}
	}
}