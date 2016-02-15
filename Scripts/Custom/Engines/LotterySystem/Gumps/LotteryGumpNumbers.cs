using System;
using Server;
using Server.Gumps;
using Server.Mobiles;
using Server.Items;
using Server.Network;

namespace Server.Engines.Lottery
{
	public class LotteryGumpNumbers : Gump
	{
		private Mobile m_From;
		private string m_sStatus;

		public LotteryGumpNumbers(Mobile from, string status)
			: base(20, 20)
		{
			LotterySystem.CloseAllLotteryGumps(from);

			m_From = from;
			m_sStatus = status;

			this.Closable = true;
			this.Disposable = true;
			this.Dragable = true;
			this.Resizable = false;
			this.AddPage(0);

			this.AddBackground(17, 19, 553, 332, 9380);
			this.AddLabel(242, 21, 0, @"Lottery Ticket");
			this.AddButton(414, 297, 247, 248, (int)Buttons.ButtonOkay, GumpButtonType.Reply, 0);
			this.AddButton(482, 297, 241, 242, (int)Buttons.ButtonCancel, GumpButtonType.Reply, 0);
			this.AddHtml(375, 57, 172, 227, string.Format("Choose {0} numbers to add to the ticket.<br><br>You can buy up to {1} tickets.", LotterySystem.LottoNumberAmount, LotterySystem.MaxTicketsPerPlayer), (bool)true, (bool)true);
			this.AddHtml(35, 326, 522, 23, m_sStatus, (bool)false, (bool)false);

			int iTotalNumbers = LotterySystem.MaxNumbers;
			int iLineLenght = 10;
			int iRows = iTotalNumbers / iLineLenght;

			for (int j = 0; j <= iRows; j++)
			{
				for (int i = 1; i <= iLineLenght; i++)
				{
					this.AddLabel(20 + i * 30, 55 + j * 50, 0, ((int)(i + j * iLineLenght)).ToString());
					this.AddCheck(15 + i * 30, 77 + j * 50, 210, 211, false, i + j * iLineLenght);

					if (i + j * iLineLenght >= iTotalNumbers)
						break;
				}
			}
		}

		public enum Buttons
		{
			Reserved,
			ButtonCancel,
			ButtonOkay,
		}

		public override void OnResponse(NetState sender, RelayInfo info)
		{
			if (!(sender.Mobile is PlayerMobile))
				return;

			string sReturnMessage = "";

			switch (info.ButtonID)
			{
				case (int)Buttons.ButtonOkay:
					int[] numbers = new int[LotterySystem.LottoNumberAmount];
					int counter = 0;

					// Get the numbers player has chosen
					for (int i = 1; i <= LotterySystem.MaxNumbers; i++)
					{
						if (info.IsSwitched(i))
						{
							if (counter >= numbers.Length)
							{
								counter = -1; // signal to many numbers
								break;
							}

							numbers[counter] = i;
							counter++;
						}
					}

					// Invalid number amount
					if (counter <= LotterySystem.LottoNumberAmount - 1)
					{
						sender.Mobile.SendGump(new LotteryGumpNumbers(sender.Mobile, string.Format("Invalid amount of numbers! Choose {0} numbers and click the Okay button.", LotterySystem.LottoNumberAmount)));
						return;
					}

					LotterySystem.AddNumber(sender.Mobile, numbers);
					sReturnMessage = "Ticket added to the list.";

					break;
				case (int)Buttons.ButtonCancel:
					break;
				case (int)Buttons.Reserved:
					return;
			}

			sender.Mobile.SendGump(new LotteryGump(sender.Mobile, sReturnMessage));
		}
	}
}