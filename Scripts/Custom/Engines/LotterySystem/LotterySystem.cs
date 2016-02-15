using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Server.Commands;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.Lottery
{
	#region LotteryEntry
	public class LotteryEntry
	{
		public Mobile m_Player;
		public List<int[]> m_NumberList;

		public bool m_bEnabled;
		public bool m_bShowWinnings;

		public DateTime m_dtTicketBought;
		public DateTime m_dtLastAccessTime;

		public int m_iWinMoney;

		public int[] m_iWinningNumbers;
		public int[] m_CorrectNumbers;
		public int[] m_TotalCorrectNumbers;

		public LotteryEntry(Mobile player)
			: this(player, new List<int[]>(), false, new int[LotterySystem.LottoNumberAmount], new int[LotterySystem.LottoNumberAmount + 1], new int[LotterySystem.LottoNumberAmount], false, DateTime.MinValue, DateTime.Now, 0 )
		{
		}

		public LotteryEntry(Mobile player, List<int[]> numberList, bool enabled, int[] correctNumbers, int[] totalCorrectNumbers, int[] winningNumbers, bool showWinnings, DateTime ticketBought, DateTime lastAccessTime, int winMoney)
		{
			m_Player = player;
			m_NumberList = numberList;
			m_bEnabled = enabled;
			m_CorrectNumbers = correctNumbers;
			m_TotalCorrectNumbers = totalCorrectNumbers;
			m_iWinningNumbers = winningNumbers;
			m_bShowWinnings = showWinnings;
			m_dtTicketBought = ticketBought;
			m_dtLastAccessTime = lastAccessTime;
			m_iWinMoney = winMoney;
		}
	}
	#endregion

	public class LotterySystem
	{
		#region Lottery & NpcSpeak Timer
		public class LotteryTimer : Timer
		{
			private static TimeSpan m_Interval = TimeSpan.FromMinutes(1.0);

			public LotteryTimer()
				: base(m_Interval, m_Interval)
			{
				Priority = TimerPriority.OneSecond;
			}

			protected override void OnTick()
			{
				if (LotterySystem.UseNpcSpeak)
				{
					foreach (Mobile m in LotterySystem.m_NpcRegister)
					{
						switch (Utility.Random(4))
						{
							case 0:
								m.Say("Buy a lottery ticket and try your luck!");
								break;
							case 1:
								m.Say(string.Format("The next Lottery Drawing is at {0}!", LotterySystem.m_dtStartTime));
								break;
							case 2:
								m.Say(string.Format("If you get {0} correct numbers you will win {1}gp!", LottoNumberAmount, LotterySystem.m_iJackpot));
								break;
							case 3:
								m.Say("Buy a lottery ticket!");
								m.Say(string.Format("One ticket only costs {0}gp!", LotterySystem.m_iTicketPrice));
								break;
							case 4:
								m.Say("Anyone here want to buy a lottery ticket?");
								break;
							case 5:
								m.Say("Lottery tickets!");
								m.Say("Come and buy lottery tickets!");
								break;
						}
					}
				}

				if (LotterySystem.m_dtStartTime < DateTime.Now)
				{
					LotterySystem.UpdateTime();
					LotterySystem.CheckForWinnings();
				}
			}
		}
		#endregion

		#region Save & Load
		public static void Initialize()
		{
			EventSink.WorldSave += new WorldSaveEventHandler(EventSink_WorldSave);
			Load();

			UpdateTime();

			if (m_iJackpot == 0)
				UpdateJackPot();

			ClearOldEntries();

			new LotteryTimer().Start();
		}

		private static void EventSink_WorldSave(WorldSaveEventArgs e)
		{
			Save();
		}

		public static void Load()
		{
			string idxPath = Path.Combine("Saves/Lottery", "Lottery.idx");
			string binPath = Path.Combine("Saves/Lottery", "Lottery.bin");

			if (File.Exists(idxPath) && File.Exists(binPath))
			{
				// Declare and initialize reader objects.
				FileStream idx = new FileStream(idxPath, FileMode.Open, FileAccess.Read, FileShare.Read);
				FileStream bin = new FileStream(binPath, FileMode.Open, FileAccess.Read, FileShare.Read);
				BinaryReader idxReader = new BinaryReader(idx);
				BinaryFileReader binReader = new BinaryFileReader(new BinaryReader(bin));

				// Read start-position and length of current order from index file
				long startPos = idxReader.ReadInt64();
				int length = idxReader.ReadInt32();
				// Point the reading stream to the proper position
				binReader.Seek(startPos, SeekOrigin.Begin);

				try
				{
					Deserialize(binReader);

					if (binReader.Position != (startPos + length))
						throw new Exception("***** Bad serialize on Lottery *****");
				}
				catch { }

				idxReader.Close();
				binReader.Close();
			}
		}

		public static void Save()
		{
			if (!Directory.Exists("Saves/Lottery/"))
				Directory.CreateDirectory("Saves/Lottery/");

			string idxPath = Path.Combine("Saves/Lottery", "Lottery.idx");
			string binPath = Path.Combine("Saves/Lottery", "Lottery.bin");

			GenericWriter idx = new BinaryFileWriter(idxPath, false);
			GenericWriter bin = new BinaryFileWriter(binPath, true);

			long startPos = bin.Position;
			Serialize(bin);
			idx.Write((long)startPos);
			idx.Write((int)(bin.Position - startPos));

			idx.Close();
			bin.Close();
		}
		#endregion

		#region Settings
		public const int MaxNumbers = 49;
		public const int LottoNumberAmount = 4;
		public const int MaxTicketsPerPlayer = 5;
		public const int SystemHue = 0x38A;
		public const int MinimumSystemGold = 1000000;
		public const int m_iTicketPrice = 100;

		public const bool UseNpcSpeak = true;

		public const double JackpotPercent = 0.8; // Jackpot is 80% of money in system.

		public static TimeSpan StartTime = TimeSpan.FromHours(17.00);
		public static TimeSpan DecayTime { get { return TimeSpan.FromDays(30); } }
		private static TimeSpan Interval = TimeSpan.FromDays(1.0);

		public const int Prize1 = 0;
		public const int Prize2 = 5000;
		public const int Prize3 = 30000;
		#endregion

		#region Members
		public static int m_iJackpot = 0;
		public static int m_iTotalLotteryMoney = MinimumSystemGold;
		public static List<LotteryEntry> m_LottoRegister = new List<LotteryEntry>();
		public static List<Mobile> m_NpcRegister = new List<Mobile>();
		public static DateTime m_dtStartTime = DateTime.MinValue;

		public static int LottoEntries
		{
			get
			{
				if (m_LottoRegister == null)
					return 0;
				return m_LottoRegister.Count;
			}
		}
		#endregion

		/// <summary>
		/// Updates the start time.
		/// </summary>
		public static void UpdateTime()
		{
			m_dtStartTime = DateTime.Now.Date + StartTime;
			if (m_dtStartTime < DateTime.Now)
				m_dtStartTime += Interval;
		}

		/// <summary>
		/// Takes gold from the players bank and adds it to the lottery system bank.
		/// </summary>
		/// <param name="from"></param>
		/// <param name="amount"></param>
		/// <returns></returns>
		public static bool TakeGold(Mobile from, int amount)
		{
			bool returnValue = Banker.Withdraw(from, amount);
			if (returnValue)
				m_iTotalLotteryMoney += amount;

			return returnValue;
		}

		/// <summary>
		/// Gives gold to the container and withdraws it from lottery system bank.
		/// </summary>
		/// <param name="cont"></param>
		/// <param name="amount"></param>
		public static void GiveGold(Container cont, int amount)
		{
			if (cont != null)
			{
				Banker.Deposit(cont, amount);
				m_iTotalLotteryMoney -= amount;
			}
		}

		public static void CancelTickets(Mobile from, LotteryEntry entry)
		{
			if (entry.m_bEnabled)
			{
				int gold = LotterySystem.m_iTicketPrice * entry.m_NumberList.Count;
				LotterySystem.GiveGold(from.BankBox, gold);

				from.SendMessage("{0}gp added to your bankbox.", gold);
				entry.m_bEnabled = false;
			}
		}

		/// <summary>
		/// Gets the entry from the registry for the player. If the player is not found in registry
		/// he gets added.
		/// </summary>
		/// <param name="player"></param>
		/// <returns></returns>
		public static LotteryEntry GetPlayerEntry(Mobile from)
		{
			foreach (LotteryEntry entry in m_LottoRegister)
			{
				if (entry.m_Player == from)
				{
					entry.m_dtLastAccessTime = DateTime.Now;
					return entry;
				}
			}

			LotteryEntry newEntry = new LotteryEntry(from);
			newEntry.m_dtLastAccessTime = DateTime.Now;
			m_LottoRegister.Add(newEntry);
			return newEntry;
		}

		/// <summary>
		/// This will add a lotto number to the players entry in the registry.
		/// </summary>
		/// <param name="player"></param>
		/// <param name="number"></param>
		public static void AddNumber(Mobile from, int[] number)
		{
			LotteryEntry entry = GetPlayerEntry(from);
			entry.m_NumberList.Add(number);
		}

		/// <summary>
		/// Will clear entries older than the DecayTime variable.
		/// </summary>
		private static void ClearOldEntries()
		{
			for (int i = m_LottoRegister.Count - 1; i >= 0; --i)
			{
				LotteryEntry entry = (LotteryEntry)m_LottoRegister[i];
				if (entry.m_dtLastAccessTime + DecayTime < DateTime.Now)
				{
					m_LottoRegister.RemoveAt(i);
				}
			}
		}

		/// <summary>
		/// This will show winning info when needed.
		/// </summary>
		/// <param name="entry">This is the players entry in the registry</param>
		/// <returns>Returns true if gump has been shown.</returns>
		public static bool TryToShowWinInfo( Mobile from, LotteryEntry entry )
		{
			if( entry != null && entry.m_bShowWinnings )
			{
				from.SendGump( new LotteryGumpInfo( from, entry ) );
				return true;
			}
			return false;
		}

		/// <summary>
		/// Closes all gumps related to the lottery system.
		/// </summary>
		/// <param name="from"></param>
		public static void CloseAllLotteryGumps(Mobile from)
		{
			from.CloseGump(typeof(LotteryGump));
			from.CloseGump(typeof(LotteryGumpInfo));
			from.CloseGump(typeof(LotteryGumpNumbers));
		}

		private static BitArray m_WinningNumberList = new BitArray(MaxNumbers + 1);
		public static int[] m_iWinningNumbers = new int[LottoNumberAmount];

		/// <summary>
		/// Creates the winning lotto number and compares it to all player numbers.
		/// </summary>
		public static void CheckForWinnings()
		{
			// Get winning Lotto Number
			for (int i = 0; i < LottoNumberAmount; i++)
			{
				int randomNum = 0;
				do
				{
					randomNum = Utility.Random(MaxNumbers) + 1;
				} while (m_WinningNumberList[randomNum]);

				m_iWinningNumbers[i] = randomNum;
				m_WinningNumberList.Set(randomNum, true);
			}

			// Give out Winnings
			GiveOutWinnings(GetSortedTotalWinnings());

			string sWinningNumber = "";
			foreach (int iWinningNumber in m_iWinningNumbers)
				sWinningNumber += string.Format("{0} ", iWinningNumber);

			CommandHandlers.BroadcastMessage(AccessLevel.Player, SystemHue, string.Format("Lottery drawing... {0}", sWinningNumber));

			m_WinningNumberList.SetAll(false);
		}

		/// <summary>
		/// This function will give out winnings to the players bankbox.
		/// </summary>
		/// <param name="iSortedWinnings">This array should contain how many players have won.</param>
		private static void GiveOutWinnings(int[] iSortedWinnings)
		{
			int[] prices = new int[5];

			prices[0] = 0;
			prices[1] = Prize1;
			prices[2] = Prize2;
			prices[3] = Prize3;
			if (iSortedWinnings[4] == 0)
				prices[4] = m_iJackpot;
			else
				prices[4] = m_iJackpot / iSortedWinnings[4];

			foreach (LotteryEntry entry in m_LottoRegister)
			{
				// If ticket is not bought skip the check
				if (!entry.m_bEnabled)
					continue;

				entry.m_iWinMoney = 0;
				foreach (int correctNumber in entry.m_CorrectNumbers)
				{
					if (entry.m_Player != null && entry.m_Player.BankBox != null && prices[correctNumber] > 0)
					{
						entry.m_iWinMoney += prices[correctNumber];
						GiveGold(entry.m_Player.BankBox, prices[correctNumber]);
					}
				}

				// Disable players ticket
				entry.m_bEnabled = false;
			}

			UpdateJackPot();
		}

		/// <summary>
		/// Update the Jackpot and check if there is the minimum amount of gold.
		/// </summary>
		private static void UpdateJackPot()
		{
			if (m_iTotalLotteryMoney < MinimumSystemGold)
				m_iTotalLotteryMoney = MinimumSystemGold;

			m_iJackpot = (int)(m_iTotalLotteryMoney * JackpotPercent);
		}

		/// <summary>
		/// Checks for correct numbers for each player, also
		/// saves info about this turn to the players entry.
		/// </summary>
		/// <returns>Returns the total winnings</returns>
		private static int[] GetSortedTotalWinnings()
		{
			int[] sortedTotalWinnings = new int[LottoNumberAmount + 1];

			// Compare winning lotto number with player numbers
			foreach (LotteryEntry entry in m_LottoRegister)
			{
				// If ticket is not bought skip the check
				if (!entry.m_bEnabled)
					continue;

				int[] correctNumbers = new int[entry.m_NumberList.Count];

				for (int i = 0; i < entry.m_NumberList.Count; i++)
				{
					int[] number = (int[])entry.m_NumberList[i];
					int WinCounter = 0;
					foreach (int singleNumber in number)
						if (m_WinningNumberList[singleNumber])
							WinCounter++;

					correctNumbers[i] = WinCounter;
					sortedTotalWinnings[WinCounter]++;
				}

				// *** Save winning info
				entry.m_CorrectNumbers = correctNumbers;
				entry.m_TotalCorrectNumbers = sortedTotalWinnings;
				m_iWinningNumbers.CopyTo(entry.m_iWinningNumbers, 0);
				// *** Save winning info END

				entry.m_bShowWinnings = true;
			}

			return sortedTotalWinnings;
		}

		public static void Serialize(GenericWriter writer)
		{
			writer.Write((int)0); // version

			// Version 0
			writer.Write(m_dtStartTime);
			writer.Write(m_iJackpot);
			writer.Write(m_iTotalLotteryMoney);

			// LottoRegistry
			writer.Write(m_LottoRegister.Count);
			foreach (LotteryEntry entry in m_LottoRegister)
			{
				writer.Write(entry.m_Player);
				writer.Write(entry.m_bEnabled);
				writer.Write(entry.m_bShowWinnings);
				writer.Write(entry.m_iWinMoney);
				writer.Write(entry.m_dtLastAccessTime);
				writer.Write(entry.m_dtTicketBought);

				// m_NumberList
				writer.Write(entry.m_NumberList.Count);
				foreach (int[] number in entry.m_NumberList)
				{
					writer.Write(number.Length);
					foreach (int singleNumber in number)
						writer.Write(singleNumber);
				}

				// m_CorrectNumbers
				writer.Write(entry.m_CorrectNumbers.Length);
				foreach (int number in entry.m_CorrectNumbers)
				{
					writer.Write(number);
				}

				// m_TotalCorrectNumbers
				writer.Write(entry.m_TotalCorrectNumbers.Length);
				foreach (int number in entry.m_TotalCorrectNumbers)
				{
					writer.Write(number);
				}

				// m_iWinningNumbers
				writer.Write(entry.m_iWinningNumbers.Length);
				foreach (int winningNumber in entry.m_iWinningNumbers)
					writer.Write(winningNumber);
			}
		}

		public static void Deserialize(GenericReader reader)
		{
			int version = reader.ReadInt();
			switch (version)
			{
				case 0:
					{
						m_dtStartTime = reader.ReadDateTime();
						m_iJackpot = reader.ReadInt();
						m_iTotalLotteryMoney = reader.ReadInt();

						// LottoRegistry
						int sizeA = reader.ReadInt();
						for (int i = 0; i < sizeA; i++)
						{
							Mobile player = reader.ReadMobile();
							bool enabled = reader.ReadBool();
							bool showWinnings = reader.ReadBool();
							int winMoney = reader.ReadInt();
							DateTime lastAccessTime = reader.ReadDateTime();
							DateTime ticketBought = reader.ReadDateTime();

							// m_NumberList
							int sizeB = reader.ReadInt();
							List<int[]> numList = new List<int[]>(sizeB);
							for (int b = 0; b < sizeB; b++)
							{
								int numSize = reader.ReadInt();
								int[] number = new int[numSize];
								for (int a = 0; a < numSize; a++)
									number[a] = reader.ReadInt();

								numList.Add(number);
							}

							// m_CorrectNumbers
							sizeB = reader.ReadInt();
							int[] correctNum = new int[sizeB];
							for (int a = 0; a < sizeB; a++)
								correctNum[a] = reader.ReadInt();

							// m_TotalCorrectNumbers
							sizeB = reader.ReadInt();
							int[] totalCorrectNum = new int[sizeB];
							for (int a = 0; a < sizeB; a++)
								totalCorrectNum[a] = reader.ReadInt();

							// m_iWinningNumbers
							sizeB = reader.ReadInt();
							int[] winningNumbers = new int[sizeB];
							for (int d = 0; d < sizeB; d++)
								winningNumbers[d] = reader.ReadInt();

							// Create the final entry
							LotteryEntry entry = new LotteryEntry(player, numList, enabled, correctNum, totalCorrectNum, winningNumbers, showWinnings, ticketBought, lastAccessTime, winMoney);
							m_LottoRegister.Add(entry);
						}
						break;
					}
			}
		}
	}
}