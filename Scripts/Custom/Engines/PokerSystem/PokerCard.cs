using System;
using System.Collections.Generic;
using System.Text;
using Server.Gumps;

namespace Server.Engines.Poker
{
	public class PokerCard
	{
		private int m_Location;
		public int Location
		{
			get { return m_Location; }
			set { m_Location = value; }
		}

		private int m_X;
		public int X
		{
			get { return m_X; }
			set { m_X = value; }
		}

		private int m_Y;
		public int Y
		{
			get { return m_Y; }
			set { m_Y = value; }
		}

		private int m_CardID;
		public int CardID
		{
			get { return m_CardID; }
			set { m_CardID = value; }
		}

		private bool m_ShowBack;
		public bool ShowBack
		{
			get { return m_ShowBack; }
			set { m_ShowBack = value; }
		}

		private PokerSystem m_PokerSystem;
		public PokerSystem PokerSystem
		{
			get { return m_PokerSystem; }
			set { m_PokerSystem = value; }
		}

		public int CardValueAceHigh
		{
			get { return CardValue == 1 ? 14 : CardValue; }
		}

		public int CardValue
		{
			get { return m_CardID % 13 + 1; }
		}

		public CardSuits CardSuit
		{
			get
			{
				CardSuits returnValue;

				if (m_CardID >= 0 && m_CardID <= 12)
					returnValue = CardSuits.Hearts;
				else if (m_CardID >= 13 && m_CardID <= 25)
					returnValue = CardSuits.Diamonds;
				else if (m_CardID >= 26 && m_CardID <= 38)
					returnValue = CardSuits.Spades;
				else
					returnValue = CardSuits.Clubs;

				return returnValue;
			}
		}

		public enum CardSuits
		{
			Hearts,
			Diamonds,
			Clubs,
			Spades
		}

		public PokerCard(PokerSystem pokerSystem, int x, int y, int location, bool showBack)
			: this(pokerSystem, x, y, location, pokerSystem.GetRandomCard(), showBack)
		{
		}

		public PokerCard(PokerSystem pokerSystem, int x, int y, int location, int cardID, bool showBack)
		{
			m_X = x;
			m_Y = y;
			m_Location = location;
			m_PokerSystem = pokerSystem;
			m_CardID = cardID;
			m_PokerSystem.Cards.Add(this);
			m_ShowBack = showBack;
		}

		public void Delete()
		{
			m_PokerSystem.Cards.Remove(this);
		}

		public void AddToGump(PokerGump gump)
		{
			gump.AddImageTiled(m_X, m_Y, PokerSystem.CardSize.X, PokerSystem.CardSize.Y, 9354);

			if (m_ShowBack)
			{
				gump.AddImage((m_X + PokerSystem.CardSize.X / 2) - 12, (m_Y + PokerSystem.CardSize.Y / 2) - 12, 11320);
			}
			else
			{
				gump.AddLabel(m_X + 4, m_Y + 1, 0, GetCardValueString(CardValue));
				gump.AddLabel(m_X + PokerSystem.CardSize.X - (8 + CardValue.ToString().Length * 4), m_Y + PokerSystem.CardSize.Y - 20, 0, GetCardValueString(CardValue));
				gump.AddLabel((m_X + PokerSystem.CardSize.X / 2) - 3, (m_Y + PokerSystem.CardSize.Y / 2) - 10, GetSuitHue(CardID), GetCardSuitString());
			}
		}

		private string GetCardSuitString()
		{
			switch (CardSuit)
			{
				default:
					return "C";
				case CardSuits.Diamonds:
					return "D";
				case CardSuits.Hearts:
					return "H";
				case CardSuits.Spades:
					return "S";
			}
		}

		private int GetSuitHue(int cardID)
		{
			if (CardSuit == CardSuits.Hearts || CardSuit == CardSuits.Diamonds)
				return 1171;
			else
				return 0;
		}

		private string GetCardValueString(int value)
		{
			string returnValue;

			if (value >= 2 && value <= 10)
				returnValue = value.ToString();
			else if (value == 11)
				returnValue = "J";
			else if (value == 12)
				returnValue = "Q";
			else if (value == 13)
				returnValue = "K";
			else
				returnValue = "A";

			return returnValue;
		}
	}
}