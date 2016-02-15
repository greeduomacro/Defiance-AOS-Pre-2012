using System;
using System.Collections;
using System.Collections.Generic;
using Server.Items;
using Server.Mobiles;
using System.Text;

namespace Server.Engines.Poker
{
	public class PokerSystem
	{
		public static readonly int MaxCards = 52;
		public static readonly Point2D CardSize = new Point2D(50, 74);
		public static readonly Point2D CardLocation = new Point2D(25, 100);
		public static readonly int CardSpace = 70;

		private int[] m_WinningsTable;
		public int[] WinningsTable
		{
			get { return m_WinningsTable; }
			set { m_WinningsTable = value; }
		}

		private Mobile m_Owner;
		public Mobile Owner
		{
			get { return m_Owner; }
			set { m_Owner = value; }
		}

		private List<PokerCard> m_Cards;
		public List<PokerCard> Cards
		{
			get { return m_Cards; }
			set { m_Cards = value; }
		}

		private int m_MinBet;
		public int MinBet
		{
			get { return m_MinBet; }
			set { m_MinBet = value; }
		}

		private int m_MaxBet;
		public int MaxBet
		{
			get { return m_MaxBet; }
			set { m_MaxBet = value; }
		}

		private int m_BetChange;
		public int BetChange
		{
			get { return m_BetChange; }
			set { m_BetChange = value; }
		}

		private int m_CurrentBet;
		public int CurrentBet
		{
			get { return m_CurrentBet; }
			set { m_CurrentBet = value; }
		}

		public PokerSystem(Mobile owner)
		{
			m_Cards = new List<PokerCard>();
			m_Owner = owner;
			m_CurrentBet = MinBet;
		}

		/// <summary>
		/// Return the winnings calculated with the bet.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public int GetCalculatedWinning(WinTypes type, PokerMachine pokerMachine)
		{
			if (type == 0)
				return (int)(pokerMachine.GoldInMachine / 3);
			return WinningsTable[(int)type] * (m_CurrentBet / BetChange);
		}

		/// <summary>
		/// Pay winning using the WinTypes.
		/// </summary>
		/// <param name="player"></param>
		/// <param name="pokerMachine"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		public bool PayWinning(Mobile player, PokerMachine pokerMachine, WinTypes type)
		{
			return PayWinning(player, pokerMachine, this.GetCalculatedWinning(type, pokerMachine));
		}

		/// <summary>
		/// Returns a cardID from the players card deck.
		/// </summary>
		/// <returns></returns>
		public int GetRandomCard()
		{
			int random = GetUniqueRandom(MaxCards);

			if (random == -1)
			{
				m_CardDeck.SetAll(false);
				random = GetUniqueRandom(MaxCards);
			}

			return random;
		}

		private BitArray m_CardDeck = new BitArray(MaxCards);
		private int[] m_Possible = new int[MaxCards];

		/// <summary>
		/// Returns a random unused number.
		/// </summary>
		/// <param name="count"></param>
		/// <returns></returns>
		public int GetUniqueRandom(int count)
		{
			int avail = 0;

			for (int i = 0; i < count; ++i)
			{
				if (!m_CardDeck[i])
					m_Possible[avail++] = i;
			}

			if (avail == 0)
				return -1;

			int v = m_Possible[Utility.Random(avail)];

			m_CardDeck.Set(v, true);

			return v;
		}

		#region Static Functions
		/// <summary>
		/// Returns true if all cards are same suit.
		/// </summary>
		/// <param name="cards"></param>
		/// <returns></returns>
		public static bool AreAllSameSuit(List<PokerCard> cards)
		{
			for (int i = 1; i < cards.Count; ++i)
				if (cards[i - 1].CardSuit != cards[i].CardSuit)
					return false;

			return true;
		}

		/// <summary>
		/// Returns the Ace value.
		/// </summary>
		/// <param name="cards">The cards used.</param>
		/// <param name="loc">The location in cards array.</param>
		/// <param name="useAceAsHigh">Returns 14 for the ace if true, if false ace is 1.</param>
		/// <returns></returns>
		private static int GetAceValue(List<PokerCard> cards, int loc, bool useAceAsHigh)
		{
			return useAceAsHigh ? cards[loc].CardValueAceHigh : cards[loc].CardValue;
		}

		/// <summary>
		/// Checks for a straight.
		/// </summary>
		/// <param name="cards"></param>
		/// <returns></returns>
		public static bool IsStraight(List<PokerCard> cards)
		{
			List<PokerCard> StraightCardLocationList = new List<PokerCard>(cards);
			bool isStraight = true;

			// First loop checks with Ace = 1 Second checks with Ace = 14
			for (int AceLoop = 0; AceLoop < 2; ++AceLoop)
			{
				if (AceLoop > 0)
					StraightCardLocationList.Sort(new PokerCardValueComparerAceHigh());

				isStraight = true;

				for (int i = 1; i < cards.Count; ++i)
				{
					if (GetAceValue(StraightCardLocationList, i - 1, AceLoop > 0) != GetAceValue(StraightCardLocationList, i, AceLoop > 0) - 1)
					{
						// This is not a straight... break
						isStraight = false;
						break;
					}
				}

				// Break if it is a straight
				if (isStraight)
					break;
			}

			return isStraight;
		}

		/// <summary>
		/// Checks the cards for a winning combination.
		/// </summary>
		/// <param name="from"></param>
		/// <param name="pokerSystem"></param>
		/// <returns></returns>
		public static PokerWinInfo CheckForWinnings(Mobile from, PokerSystem pokerSystem)
		{
			if (pokerSystem.Cards.Count < 5)
				return new PokerWinInfo(pokerSystem, WinTypes.None);

			List<PokerCard> SortedCardValueList = new List<PokerCard>(pokerSystem.Cards);
			SortedCardValueList.Sort(new PokerCardValueComparer());

			// RoyalFlush
			if (SortedCardValueList[0].CardValue == 1 &&
				SortedCardValueList[1].CardValue == 10 &&
				SortedCardValueList[2].CardValue == 11 &&
				SortedCardValueList[3].CardValue == 12 &&
				SortedCardValueList[4].CardValue == 13 &&
				AreAllSameSuit(SortedCardValueList))
				return new PokerWinInfo(pokerSystem, WinTypes.RoyalFlush);

			// StraightFlush
			if (IsStraight(SortedCardValueList) && AreAllSameSuit(SortedCardValueList))
				return new PokerWinInfo(pokerSystem, WinTypes.StraightFlush);

			// FourOfAKind
			if (SortedCardValueList[0].CardValue == SortedCardValueList[1].CardValue && SortedCardValueList[1].CardValue == SortedCardValueList[2].CardValue && SortedCardValueList[2].CardValue == SortedCardValueList[3].CardValue ||
				SortedCardValueList[1].CardValue == SortedCardValueList[2].CardValue && SortedCardValueList[2].CardValue == SortedCardValueList[3].CardValue && SortedCardValueList[3].CardValue == SortedCardValueList[4].CardValue)
				return new PokerWinInfo(pokerSystem, WinTypes.FourOfAKind);

			// FullHouse
			if (SortedCardValueList[0].CardValue == SortedCardValueList[1].CardValue && SortedCardValueList[1].CardValue == SortedCardValueList[2].CardValue && SortedCardValueList[3].CardValue == SortedCardValueList[4].CardValue ||
				SortedCardValueList[2].CardValue == SortedCardValueList[3].CardValue && SortedCardValueList[3].CardValue == SortedCardValueList[4].CardValue && SortedCardValueList[0].CardValue == SortedCardValueList[1].CardValue)
				return new PokerWinInfo(pokerSystem, WinTypes.FullHouse);

			// ThreeOfAKind
			if (SortedCardValueList[0].CardValue == SortedCardValueList[1].CardValue && SortedCardValueList[1].CardValue == SortedCardValueList[2].CardValue ||
				SortedCardValueList[1].CardValue == SortedCardValueList[2].CardValue && SortedCardValueList[2].CardValue == SortedCardValueList[3].CardValue ||
				SortedCardValueList[2].CardValue == SortedCardValueList[3].CardValue && SortedCardValueList[3].CardValue == SortedCardValueList[4].CardValue)
				return new PokerWinInfo(pokerSystem, WinTypes.ThreeOfAKind);

			// Flush
			if (AreAllSameSuit(SortedCardValueList))
				return new PokerWinInfo(pokerSystem, WinTypes.Flush);

			// Straight
			if (IsStraight(SortedCardValueList))
				return new PokerWinInfo(pokerSystem, WinTypes.Straight);

			// ThreeOfAKind
			if (SortedCardValueList[0].CardValue == SortedCardValueList[1].CardValue && SortedCardValueList[1].CardValue == SortedCardValueList[2].CardValue ||
				SortedCardValueList[1].CardValue == SortedCardValueList[2].CardValue && SortedCardValueList[2].CardValue == SortedCardValueList[3].CardValue ||
				SortedCardValueList[2].CardValue == SortedCardValueList[3].CardValue && SortedCardValueList[3].CardValue == SortedCardValueList[4].CardValue)
				return new PokerWinInfo(pokerSystem, WinTypes.ThreeOfAKind);

			// TwoPairs
			if (SortedCardValueList[0].CardValue == SortedCardValueList[1].CardValue && SortedCardValueList[2].CardValue == SortedCardValueList[3].CardValue ||
				SortedCardValueList[1].CardValue == SortedCardValueList[2].CardValue && SortedCardValueList[3].CardValue == SortedCardValueList[4].CardValue ||
				SortedCardValueList[0].CardValue == SortedCardValueList[1].CardValue && SortedCardValueList[3].CardValue == SortedCardValueList[4].CardValue)
				return new PokerWinInfo(pokerSystem, WinTypes.TwoPairs);

			// Pair
			if (SortedCardValueList[0].CardValue == SortedCardValueList[1].CardValue ||
				SortedCardValueList[1].CardValue == SortedCardValueList[2].CardValue ||
				SortedCardValueList[2].CardValue == SortedCardValueList[3].CardValue ||
				SortedCardValueList[3].CardValue == SortedCardValueList[4].CardValue)
				return new PokerWinInfo(pokerSystem, WinTypes.Pair);

			return new PokerWinInfo(pokerSystem, WinTypes.None);
		}

		/// <summary>
		/// Remove an amount of gold from the player.
		/// </summary>
		/// <param name="player"></param>
		/// <param name="pokerMachine"></param>
		/// <param name="amount"></param>
		/// <param name="useBank"></param>
		/// <returns></returns>
		public static bool ChargePlayer(Mobile player, PokerMachine pokerMachine, int amount, bool useBank)
		{
			Container backPack = player.Backpack;

			if (backPack != null && !useBank && backPack.ConsumeTotal(typeof(Gold), amount))
			{
				player.SendMessage("{0}gp has been removed from your backpack.", amount);
				Effects.PlaySound(pokerMachine.Location, pokerMachine.Map, 0x36); // Coin Sound
				pokerMachine.GoldInMachine += amount;
				return true;
			}
			else if (useBank && Banker.Withdraw(player, amount))
			{
				player.SendMessage("{0}gp has been removed from your banbox.", amount);
				Effects.PlaySound(pokerMachine.Location, pokerMachine.Map, 0x36); // Coin Sound
				pokerMachine.GoldInMachine += amount;
				return true;
			}

			return false;
		}

		/// <summary>
		/// Adds an amount of gold to the players bankbox.
		/// </summary>
		/// <param name="player"></param>
		/// <param name="pokerMachine"></param>
		/// <param name="amount"></param>
		/// <returns></returns>
		public static bool PayWinning(Mobile player, PokerMachine pokerMachine, int amount)
		{
			if (pokerMachine.GoldInMachine <= 0)
				return false;

			int amountToGive = amount;
			if (amount > pokerMachine.GoldInMachine)
			{
				amountToGive = pokerMachine.GoldInMachine;
				pokerMachine.GoldInMachine = 0;
				pokerMachine.Visible = false;
			}
			else
				pokerMachine.GoldInMachine -= amountToGive;

			Effects.PlaySound(pokerMachine.Location, pokerMachine.Map, 0x36); // Coin Sound
			if (Banker.Deposit(player, amountToGive))
				return true;
			else
			{
				while ( amountToGive > 0 )
				{
					Item item;
					if ( amountToGive <= 1000000 )
					{
						item = new BankCheck( amountToGive );
						amountToGive = 0;
					}
					else
					{
						item = new BankCheck( 1000000 );
						amountToGive -= 1000000;
					}
					player.AddToBackpack(item);
				}
				return false;
			}
		}
		#endregion
	}

	#region PokerCardValueComparer
	public class PokerCardValueComparer : IComparer<PokerCard>
	{
		public int Compare(PokerCard a, PokerCard b)
		{
			int aValue = a.CardValue;
			int bValue = b.CardValue;

			if (aValue < bValue)
				return -1;
			else if (aValue > bValue)
				return 1;
			else
				return 0;
		}
	}
	#endregion

	#region PokerCardValueComparerAceHigh
	public class PokerCardValueComparerAceHigh : IComparer<PokerCard>
	{
		public int Compare(PokerCard a, PokerCard b)
		{
			int aValue = a.CardValueAceHigh;
			int bValue = b.CardValueAceHigh;

			if (aValue < bValue)
				return -1;
			else if (aValue > bValue)
				return 1;
			else
				return 0;
		}
	}
	#endregion

	#region PokerCardLocationComparer
	public class PokerCardLocationComparer : IComparer<PokerCard>
	{
		public int Compare(PokerCard a, PokerCard b)
		{
			int aValue = a.Location;
			int bValue = b.Location;

			if (aValue < bValue)
				return -1;
			else if (aValue > bValue)
				return 1;
			else
				return 0;
		}
	}
	#endregion

	#region PokerWinInfo
	public enum WinTypes
	{
		RoyalFlush,
		StraightFlush,
		FourOfAKind,
		FullHouse,
		Flush,
		Straight,
		ThreeOfAKind,
		TwoPairs,
		Pair,
		None,
	}

	public class PokerWinInfo
	{
		private WinTypes m_WinType;
		public WinTypes WinType
		{
			get { return m_WinType; }
			set { m_WinType = value; }
		}

		private string m_Message;
		public string Message
		{
			get { return m_Message; }
			set { m_Message = value; }
		}

		private int m_WinAmount;
		public int WinAmount
		{
			get { return m_WinAmount; }
			set { m_WinAmount = value; }
		}

		public PokerWinInfo(PokerSystem pokerSystem, WinTypes winType)
		{
			m_WinType = winType;
			m_Message = CreateMessage(m_WinType);
			m_WinAmount = pokerSystem.WinningsTable[(int)m_WinType];
		}

		private string CreateMessage(WinTypes winType)
		{
			if (winType == WinTypes.None)
				return "You did not win anything.";
			else
				return string.Format("You won! ( {0} )", winType.ToString());
		}
	}
	#endregion
}