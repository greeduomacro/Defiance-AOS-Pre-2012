using System;
using System.Collections.Generic;
using Server;
using Server.Engines.Poker;
using Server.Items;
using Server.Mobiles;
using Server.Network;

namespace Server.Gumps
{
	public class PokerGump : Gump
	{
		private PokerMachine m_PokerMachine;
		private PokerSystem m_PokerSystem;
		private PokerTurn m_PokerTurn;
		private bool[] m_HoldButtons;
		private bool m_bUseBank;

		public enum PokerTurn
		{
			First,
			Second
		}

		public PokerGump(Mobile from, PokerMachine pokerMachine, string message)
			: this(from, pokerMachine, null, PokerTurn.First, new bool[5] { false, false, false, false, false }, message, true, false, false, true)
		{
		}

		private PokerGump(Mobile from, PokerMachine pokerMachine, PokerSystem pokerSystem, PokerTurn pokerTurn, bool[] holdButtons, string status, bool updateCards, bool useBank, bool checkForWinnings, bool showBack)
			: base(50, 35)
		{
			from.CloseGump(typeof(PokerGump));

			m_PokerMachine = pokerMachine;
			m_PokerTurn = pokerTurn;
			m_HoldButtons = holdButtons;
			m_bUseBank = useBank;

			if (pokerSystem == null)
			{
				m_PokerSystem = new PokerSystem(from);
				m_PokerSystem.MinBet = pokerMachine.MinBet;
				m_PokerSystem.MaxBet = pokerMachine.MaxBet;
				m_PokerSystem.BetChange = pokerMachine.BetChange;
				m_PokerSystem.WinningsTable = pokerMachine.m_WinningsTable;
				m_PokerSystem.CurrentBet = pokerMachine.MinBet;
			}
			else
				m_PokerSystem = pokerSystem;

			if (updateCards)
				UpdateCards(m_PokerSystem, m_HoldButtons, showBack);

			if (checkForWinnings)
				CheckForWinnings(from, m_PokerMachine, m_PokerSystem, m_PokerTurn, out status);

			this.Closable = true;
			this.Disposable = true;
			this.Dragable = true;
			this.Resizable = false;
			this.AddPage(0);
			this.AddBackground(0, 0, 524, 341, 2620);
			this.AddAlphaRegion(5, 7, 513, 326);

			foreach (PokerCard card in m_PokerSystem.Cards)
				card.AddToGump(this);

			this.AddButton(476, 275, 4017, 4018, (int)Buttons.Cancel, GumpButtonType.Reply, 0);
			this.AddLabel(434, 277, 940, "Close");
			this.AddButton(175, 218, 4023, 4024, (int)Buttons.OK, GumpButtonType.Reply, 0);

			if (m_PokerTurn == PokerTurn.First)
			{
				this.AddButton(476, 246, 4014, 4015, (int)Buttons.ChangeBet, GumpButtonType.Reply, 0);
				this.AddLabel(398, 248, 940, "Change Bet");
			}
			else if (m_PokerTurn == PokerTurn.Second)
			{
				int checkY = PokerSystem.CardLocation.Y + PokerSystem.CardSize.Y + 10;
				this.AddCheck(PokerSystem.CardLocation.X + PokerSystem.CardSize.X / 2 - 10, checkY, 210, 211, m_HoldButtons[0], (int)Buttons.Check1);
				this.AddCheck(PokerSystem.CardLocation.X + PokerSystem.CardSize.X / 2 + PokerSystem.CardSpace - 10, checkY, 210, 211, m_HoldButtons[1], (int)Buttons.Check2);
				this.AddCheck(PokerSystem.CardLocation.X + PokerSystem.CardSize.X / 2 + PokerSystem.CardSpace * 2 - 10, checkY, 210, 211, m_HoldButtons[2], (int)Buttons.Check3);
				this.AddCheck(PokerSystem.CardLocation.X + PokerSystem.CardSize.X / 2 + PokerSystem.CardSpace * 3 - 10, checkY, 210, 211, m_HoldButtons[3], (int)Buttons.Check4);
				this.AddCheck(PokerSystem.CardLocation.X + PokerSystem.CardSize.X / 2 + PokerSystem.CardSpace * 4 - 10, checkY, 210, 211, m_HoldButtons[4], (int)Buttons.Check5);
			}

			this.AddCheck(191, 271, 210, 211, m_bUseBank, (int)Buttons.UseBankCheck);
			this.AddLabel(18, 272, 940, @"Withdraw money from bank.");

			this.AddLabel(18, 252, 940, string.Format("Bank balance: {0}gp.", Banker.GetBalance( from ).ToString()));

			this.AddBackground(366, 16, 144, 226, 3000);
			for (int i = 0; i < m_PokerSystem.WinningsTable.Length - 1; ++i)
			{
				this.AddLabel(370, 20 + i * 20, 0, ((WinTypes)i).ToString());
				this.AddLabel(450, 20 + i * 20, 0, string.Format("{0}gp", m_PokerSystem.GetCalculatedWinning((WinTypes)i, pokerMachine)));
			}

			this.AddLabel(370, (m_PokerSystem.WinningsTable.Length + 1) * 20, 0, string.Format("Current Bet: {0}gp", m_PokerSystem.CurrentBet));
			this.AddHtml(13, 302, 497, 27, status, (bool)true, (bool)false);
		}

		public enum Buttons
		{
			Cancel,
			OK,
			Check1,
			Check2,
			Check3,
			Check4,
			Check5,
			ChangeBet,
			UseBankCheck
		}

		private void ReturnMessage(Mobile from, string message)
		{
			from.SendGump(new PokerGump(from, m_PokerMachine, m_PokerSystem, m_PokerTurn, m_HoldButtons, message, false, m_bUseBank, false, false));
		}

		private void CheckForWinnings(Mobile from, PokerMachine pokerMachine, PokerSystem pokerSystem, PokerTurn pokerTurn, out string message)
		{
			message = string.Empty;

			if (pokerTurn == PokerTurn.First)
			{
				PokerWinInfo pokerWinInfo = PokerSystem.CheckForWinnings(from, pokerSystem);
				message = pokerWinInfo.Message;

				if (pokerWinInfo.WinType != WinTypes.None)
				{
					if (pokerSystem.PayWinning(from, pokerMachine, pokerWinInfo.WinType))
						message += string.Format(" {0} gp have been placed in your bank box.", pokerSystem.GetCalculatedWinning(pokerWinInfo.WinType, pokerMachine));
					else
						message += string.Format(" {0} gp have been placed in your backpack, because your bank box is full.", pokerSystem.GetCalculatedWinning(pokerWinInfo.WinType, pokerMachine));
					pokerMachine.PublicOverheadMessage(MessageType.Regular, 0x0, true, "Winner!");
					pokerMachine.PublicOverheadMessage(MessageType.Regular, 0x0, true, pokerWinInfo.WinType.ToString());
				}
			}
		}

		private void UpdateCards(PokerSystem pokerSystem, bool[] holdButtons, bool showBack)
		{
			List<PokerCard> SortedCardLocationList = new List<PokerCard>(pokerSystem.Cards);
			SortedCardLocationList.Sort(new PokerCardLocationComparer());

			for (int i = 0; i < 5; ++i)
			{
				if (!holdButtons[i])
				{
					if (i < SortedCardLocationList.Count)
						SortedCardLocationList[i].Delete();

					int iCardX = PokerSystem.CardLocation.X + PokerSystem.CardSpace * i;

					if (showBack)
						new PokerCard(pokerSystem, iCardX, PokerSystem.CardLocation.Y, i, -1, showBack);
					else
						new PokerCard(pokerSystem, iCardX, PokerSystem.CardLocation.Y, i, showBack);
				}
			}
		}

		public override void OnResponse(NetState sender, RelayInfo info)
		{
			if (!(sender.Mobile is PlayerMobile))
				return;

			if (!sender.Mobile.InRange(m_PokerMachine.GetWorldLocation(), 2))
			{
				sender.Mobile.SendLocalizedMessage(1060178); // You are too far away to perform that action!
				return;
			}

			if (m_PokerMachine.GoldInMachine <= 0)
			{
				sender.Mobile.SendMessage("This machine is out of gold.");
				return;
			}

			string statusText = "";
			m_HoldButtons[0] = info.IsSwitched((int)Buttons.Check1);
			m_HoldButtons[1] = info.IsSwitched((int)Buttons.Check2);
			m_HoldButtons[2] = info.IsSwitched((int)Buttons.Check3);
			m_HoldButtons[3] = info.IsSwitched((int)Buttons.Check4);
			m_HoldButtons[4] = info.IsSwitched((int)Buttons.Check5);
			m_bUseBank = info.IsSwitched((int)Buttons.UseBankCheck);

			switch (info.ButtonID)
			{
				case (int)Buttons.Cancel:
					return;
				case (int)Buttons.OK:
					if (m_PokerTurn == PokerTurn.First)
					{
						if (!PokerSystem.ChargePlayer(sender.Mobile, m_PokerMachine, m_PokerSystem.CurrentBet, m_bUseBank))
						{
							if (m_bUseBank)
								ReturnMessage(sender.Mobile, "You do not have enough gold in your bankbox!");
							else
								ReturnMessage(sender.Mobile, "You do not have enough gold in your backpack!");
							return;
						}
					}

					if (m_PokerTurn == PokerTurn.First)
						m_PokerTurn++;
					else
						m_PokerTurn = PokerTurn.First;
					break;
				case (int)Buttons.ChangeBet:
					if (m_PokerSystem.CurrentBet >= m_PokerSystem.MaxBet)
						m_PokerSystem.CurrentBet = m_PokerSystem.MinBet;
					else
						m_PokerSystem.CurrentBet += m_PokerSystem.BetChange;
					break;
			}

			sender.Mobile.SendGump(new PokerGump(sender.Mobile, m_PokerMachine, m_PokerSystem, m_PokerTurn, m_HoldButtons, statusText, info.ButtonID == (int)Buttons.OK, m_bUseBank, info.ButtonID == (int)Buttons.OK, false));
		}
	}
}