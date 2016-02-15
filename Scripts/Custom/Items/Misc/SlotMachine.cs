using System;
using Server;
using Server.Gumps;
using Server.Network;
using Server.Mobiles;
using Server.Items;

namespace Server.Items
{
	[FlipableAttribute( 0x1173, 0x1174 )]
	public class SlotMachine : Item
	{
		private int m_iGoldInMachine;
		[CommandProperty( AccessLevel.Administrator )]
		public int GoldInMachine
		{
			get{ return m_iGoldInMachine; }
			set{ m_iGoldInMachine = value; InvalidateProperties(); }
		}

		public int Jackpot
		{
			get{ return (int)(m_iGoldInMachine * 0.30); }
		}

		public static int m_iMinBet = 100, m_iMaxBet = 500, m_iBetChange = 100;
		public static string m_sDefaultMessage = "Welcome to Defiance AOS Slot Machine";

		public readonly static Version CurrentVersion = new Version(1, 0, 1);

		[Constructable]
		public SlotMachine() : base( 0x1173 )
		{
			Movable = false;
			Name = "a Slot Machine";
			Hue = 0x22B;
			m_iGoldInMachine = 30000 + ((Utility.Random(700)+1)*100);
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );

			list.Add( 1070722, string.Format( "Jackpot: {0}", Jackpot ) );
		}

		public SlotMachine( Serial serial ) : base( serial )
		{
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( !from.InRange( GetWorldLocation(), 2 ) )
				from.SendLocalizedMessage( 500446 ); // That is too far away.
			else
			{
				Effects.PlaySound( this.Location, this.Map, 533 );
				from.SendGump( new SlotMachineGump( from, new int[3]{0,0,0}, false, this, SlotMachine.m_sDefaultMessage, new bool[3]{false,false,false}, m_iMinBet, false ) );
			}
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
			writer.Write( (int) m_iGoldInMachine );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
			m_iGoldInMachine = reader.ReadInt();
		}
	}
}

namespace Server.Gumps
{
	public class SlotMachineGump : Gump
	{
		public class SlotMachineEntry
		{
			public string m_sName;
			public int m_itemID;
			public Type m_item;
			public int m_goldWin;
			public int m_AmountNeeded;
			public double m_ItemChance;
			public bool m_Jackpot;

			public SlotMachineEntry(string name, int itemID, bool jackpot, Type item, double itemChance, int amountNeeded, int goldWin)
			{
				m_sName = name;
				m_itemID = itemID;
				m_item = item;
				m_goldWin = goldWin;
				m_AmountNeeded = amountNeeded;
				m_ItemChance = itemChance;
				m_Jackpot = jackpot;
			}
		}

		public class ChanceEntry
		{
			public int m_itemID;
			public double m_Chance;

			public ChanceEntry(int itemID, double chance)
			{
				m_itemID = itemID;
				m_Chance = chance;
			}
		}

		/// <summary>
		/// The total sum of all "chances" need to be 1.0
		/// </summary>
		private static ChanceEntry[] m_ChanceTable = new ChanceEntry[]
		{
			new ChanceEntry( 7186, 0.03 ),	//bell
			new ChanceEntry( 7150, 0.06 ),	//gold brick
			new ChanceEntry( 6366, 0.08 ),	//mandrake root
			new ChanceEntry( 6225, 0.09 ),	//scales
			new ChanceEntry( 6237, 0.11 ),	//full vials
			new ChanceEntry( 2599, 0.12 ),	//candle
			new ChanceEntry( 2474, 0.14 ),	//wooden box
			new ChanceEntry( 2482, 0.16 ),	//backpack
			new ChanceEntry( 3702, 0.21 )	//bag
		};

		private static SlotMachineEntry[] m_MainTable = new SlotMachineEntry[]
		{
			new SlotMachineEntry( "Bells",			7186, true,  null, 0.00, 3, 0 ),
			new SlotMachineEntry( "Gold Bricks",	7150, false, null, 0.00, 3, 10000 ),
			new SlotMachineEntry( "Mandrake Roots", 6366, false, null, 0.00, 3, 4000 ),
			new SlotMachineEntry( "Scales",			6225, false, null, 0.00, 3, 2500 ),
			new SlotMachineEntry( "Full Vials",		6237, false, null, 0.00, 3, 1800 ),
			new SlotMachineEntry( "Candles",		2599, false, null, 0.00, 3, 1200 ),
			new SlotMachineEntry( "Wooden Boxes",	2474, false, null, 0.00, 3, 600 ),
			new SlotMachineEntry( "Backpacks",		2482, false, null, 0.00, 3, 400 ),
			new SlotMachineEntry( "Bags",			3702, false, null, 0.00, 3, 200 ),
			new SlotMachineEntry( "Bags",			3702, false, null, 0.00, 2, 50 )
		};

		private int[] m_slotIds;
		private bool m_bSecondTurn;
		private SlotMachine m_Machine;
		private bool[] m_HoldButtons;
		private int m_curBet;
		private bool m_bUseBank;

		public SlotMachineGump(Mobile from, int[] slotIds, bool secondTurn, SlotMachine machine, string statusText, bool[] holdButtons, int curBet, bool bUseBank)
			: base(0, 0)
		{
			from.CloseGump(typeof(SlotMachineGump));
			m_slotIds = slotIds;
			m_bSecondTurn = secondTurn;
			m_Machine = machine;
			m_HoldButtons = holdButtons;
			m_curBet = curBet;
			m_bUseBank = bUseBank;

			this.Closable = true;
			this.Disposable = true;
			this.Dragable = true;
			this.Resizable = false;

			this.AddPage(0);
			this.AddBackground(50, 68, 566, 483, 9200);
			this.AddBackground(78, 53, 502, 35, 9200);

			this.AddBackground(94, 98, 81, 82, 3000);
			this.AddBackground(184, 98, 81, 82, 3000);
			this.AddBackground(274, 98, 81, 82, 3000);
			this.AddBackground(364, 95, 241, 426, 3000);

			this.AddLabel(534, 525, 0, @"Close");
			this.AddLabel(294, 60, 0, @"Slot Machine");
			this.AddLabel(67, 525, 0, string.Format("Version {0}", SlotMachine.CurrentVersion.ToString()));

			this.AddImage(0, 26, 10440);
			this.AddImage(584, 26, 10441);

			this.AddButton(572, 524, 4017, 4018, (int)Buttons.CLOSE, GumpButtonType.Reply, 0);

			// Options
			this.AddLabel(160, 390, 0, @"Machine Options:");
			this.AddCheck(160, 415, 210, 211, m_bUseBank, (int)Buttons.USEBANK);
			this.AddLabel(190, 415, 0, @"Withdraw money from bank.");
			this.AddLabel(160, 440, 0, string.Format("Bank balance: {0}gp.", Banker.GetBalance( from ).ToString()));

			if (secondTurn)
			{
				this.AddCheck(140, 185, 210, 211, m_HoldButtons[0], (int)Buttons.HOLD1);
				this.AddCheck(230, 185, 210, 211, m_HoldButtons[1], (int)Buttons.HOLD2);
				this.AddCheck(320, 185, 210, 211, m_HoldButtons[2], (int)Buttons.HOLD3);
				this.AddLabel(110, 186, 0, @"Hold");
				this.AddLabel(200, 186, 0, @"Hold");
				this.AddLabel(290, 186, 0, @"Hold");
			}

			this.AddButton(292, 270, 4023, 4024, (int)Buttons.SPIN, GumpButtonType.Reply, 0);
			this.AddLabel(253, 270, 0, @"Spin!");

			int slotLocationX = 109;
			for (int slotLocation = 0; slotLocation < m_slotIds.Length; slotLocation++)
			{
				if (m_slotIds[slotLocation] != 0)
					this.AddItem(slotLocationX, 123, m_slotIds[slotLocation]);
				slotLocationX += 90;
			}

			int y = 102;
			for (int a = 0; a < m_MainTable.Length; a++)
			{
				SlotMachineEntry entry = (SlotMachineEntry)m_MainTable[a];

				for (int b = 0; b < entry.m_AmountNeeded; b++)
					this.AddItem(371 + (40 * b), y, entry.m_itemID);

				this.AddLabel(525, y + 3, 0, entry.m_Jackpot ? "Jackpot" : string.Format("{0}gp", entry.m_goldWin * (m_curBet / SlotMachine.m_iBetChange)));
				y += 40;
			}

			if (!secondTurn)
			{
				this.AddButton(177, 283, 4023, 4024, (int)Buttons.CHANGEBET, GumpButtonType.Reply, 0);
				this.AddLabel(101, 283, 0, @"Change Bet");
			}
			this.AddLabel(94, 260, 0, string.Format("Current Bet: {0}", m_curBet));

			this.AddImage(67, 389, 9000);
			this.AddHtml(62, 223, 301, 24, string.Format("<center>{0}</center>", statusText), (bool)true, (bool)false);
		}

		public enum Buttons
		{
			CLOSE,
			HOLD1,
			HOLD2,
			HOLD3,
			SPIN,
			CHANGEBET,
			USEBANK
		}

		private bool ChargePlayer(Mobile player, int amount)
		{
			Container backPack = player.Backpack;

			if (backPack != null && !m_bUseBank && backPack.ConsumeTotal(typeof(Gold), amount))
			{
				player.SendMessage("{0}gp has been removed from your backpack.", amount);
				Effects.PlaySound(m_Machine.Location, m_Machine.Map, 0x36); // Coin Sound
				return true;
			}
			else if (m_bUseBank && Banker.Withdraw(player, amount))
			{
				player.SendMessage("{0}gp has been removed from your banbox.", amount);
				Effects.PlaySound(m_Machine.Location, m_Machine.Map, 0x36); // Coin Sound
				return true;
			}

			return false;
		}

		private void PayWinning(Mobile player, int amount)
		{
			int amountToGive = amount;
			if (amount > m_Machine.GoldInMachine)
			{
				amountToGive = m_Machine.GoldInMachine;
				m_Machine.GoldInMachine = 0;
			}
			else
				m_Machine.GoldInMachine -= amountToGive;

			if (!Banker.Deposit(player, amountToGive))
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
				player.SendMessage("Your bank box is full, so your reward has been added your backpack. Please clear some room in your bank");
			}
			Effects.PlaySound(m_Machine.Location, m_Machine.Map, 0x36); // Coin Sound
		}

		private SlotMachineEntry GetWinEntry(int slotId, int amountNeeded)
		{
			for (int i = 0; i < m_MainTable.Length; i++)
				if (slotId == ((SlotMachineEntry)m_MainTable[i]).m_itemID && amountNeeded == ((SlotMachineEntry)m_MainTable[i]).m_AmountNeeded)
					return (SlotMachineEntry)m_MainTable[i];
			return null;
		}

		private string CheckForPayWinnings(Mobile player, SlotMachineEntry entry)
		{
			int goldAmount = entry.m_goldWin * (m_curBet / SlotMachine.m_iBetChange);

			if (entry.m_Jackpot)
			{
				int oldJackpot = m_Machine.Jackpot;
				PayWinning(player, m_Machine.Jackpot);
				m_Machine.PublicOverheadMessage(MessageType.Regular, 0x0, true, "Jackpot Winner!");
				return string.Format("JackPot! You Win {0}gp! {1} {2}", oldJackpot, entry.m_AmountNeeded, entry.m_sName);
			}
			else
			{
				PayWinning(player, entry.m_goldWin * (m_curBet / SlotMachine.m_iBetChange));
				m_Machine.PublicOverheadMessage(MessageType.Regular, 0x0, true, "Winner!");
				m_Machine.PublicOverheadMessage(MessageType.Regular, 0x0, true, string.Format("{0} {1}", entry.m_AmountNeeded, entry.m_sName));
				return string.Format("You Win {0}gp! {1} {2}", goldAmount, entry.m_AmountNeeded, entry.m_sName);
			}
		}

		private int GetNumOfCorrectSlots(int[] slotIds)
		{
			if (slotIds[0] == slotIds[1] && slotIds[1] == slotIds[2])
				return 3;
			else
			{
				int iTwoSlotWin = Get2SlotWin(slotIds);
				if (iTwoSlotWin != -1)
					return 2;
			}
			return 0;
		}

		private string CheckForWinnings(Mobile player, int[] slotIds)
		{
			int iNumOfCorrectSlots = GetNumOfCorrectSlots(slotIds);

			if (iNumOfCorrectSlots == 3)
			{
				SlotMachineEntry entry = GetWinEntry(slotIds[0], iNumOfCorrectSlots);
				if (entry != null)
				{
					return CheckForPayWinnings(player, entry);
				}
			}
			else if (iNumOfCorrectSlots == 2)
			{
				int iTwoSlotWin = Get2SlotWin(slotIds);
				if (iTwoSlotWin != -1)
				{
					SlotMachineEntry entry = GetWinEntry(slotIds[iTwoSlotWin], iNumOfCorrectSlots);
					if (entry != null)
					{
						return CheckForPayWinnings(player, entry);
					}
				}
			}
			return SlotMachine.m_sDefaultMessage;
		}

		private int Get2SlotWin(int[] slotIds)
		{
			if (slotIds[0] == slotIds[1] || slotIds[1] == slotIds[2])
				return 1;
			else if (slotIds[0] == slotIds[2])
				return 0;
			else return -1;
		}

		private int[] GetSlots(int[] slotIds, bool[] slot)
		{
			int lastSlotInTable = m_ChanceTable[m_ChanceTable.Length - 1].m_itemID;
			int[] returnSlots = { lastSlotInTable, lastSlotInTable, lastSlotInTable };

			for (int i = 0; i < 3; i++)
				if (slot[i])
				{
					double cumulativeChance = 0.0;
					double[] cumulativeTable = new double[m_ChanceTable.Length + 1];
					int a;

					cumulativeTable[0] = 0;
					for (a = 0; a < m_ChanceTable.Length; a++)
					{
						cumulativeChance += ((ChanceEntry)m_ChanceTable[a]).m_Chance;
						cumulativeTable[a + 1] = cumulativeChance;
					}

					double randomNumber = Utility.RandomDouble();

					for (a = 0; a < m_ChanceTable.Length; a++)
					{
						if (randomNumber >= cumulativeTable[a] && randomNumber <= cumulativeTable[a + 1])
						{
							returnSlots[i] = ((ChanceEntry)m_ChanceTable[a]).m_itemID;
							break;
						}
					}
				}
				else returnSlots[i] = slotIds[i];

			return returnSlots;
		}

		private void ReturnDefault(Mobile from)
		{
			from.SendGump(new SlotMachineGump(from, m_slotIds, m_bSecondTurn, m_Machine, SlotMachine.m_sDefaultMessage, m_HoldButtons, m_curBet, m_bUseBank));
		}

		public override void OnResponse(NetState state, RelayInfo info)
		{
			base.OnResponse(state, info);

			if (!state.Mobile.InRange(m_Machine.GetWorldLocation(), 2))
			{
				state.Mobile.SendLocalizedMessage(1060178); // You are too far away to perform that action!
				return;
			}

			m_HoldButtons[0] = info.IsSwitched((int)Buttons.HOLD1);
			m_HoldButtons[1] = info.IsSwitched((int)Buttons.HOLD2);
			m_HoldButtons[2] = info.IsSwitched((int)Buttons.HOLD3);

			m_bUseBank = info.IsSwitched((int)Buttons.USEBANK);

			switch (info.ButtonID)
			{
				case (int)Buttons.CHANGEBET:
					if (m_curBet >= SlotMachine.m_iMaxBet)
						m_curBet = SlotMachine.m_iMinBet;
					else
						m_curBet += SlotMachine.m_iBetChange;

					ReturnDefault(state.Mobile);
					return;
				case (int)Buttons.SPIN:
					if (!m_bSecondTurn)
					{
						if (m_Machine.GoldInMachine <= 0)
						{
							state.Mobile.SendGump(new SlotMachineGump(state.Mobile, m_slotIds, m_bSecondTurn, m_Machine, "This machine is out of gold!", new bool[3] { false, false, false }, m_curBet, m_bUseBank));
							return;
						}

						if (ChargePlayer(state.Mobile, m_curBet))
						{
							m_Machine.GoldInMachine += m_curBet;
						}
						else
						{
							state.Mobile.SendGump(new SlotMachineGump(state.Mobile, m_slotIds, m_bSecondTurn, m_Machine, "You do not have enough gold!", new bool[3] { false, false, false }, m_curBet, m_bUseBank));
							return;
						}
					}

					int[] iSlots = GetSlots(m_slotIds, new bool[] { !m_HoldButtons[0], !m_HoldButtons[1], !m_HoldButtons[2] });

					state.Mobile.SendGump(new SlotMachineGump(state.Mobile, iSlots, !m_bSecondTurn, m_Machine, m_bSecondTurn ? CheckForWinnings(state.Mobile, iSlots) : SlotMachine.m_sDefaultMessage, new bool[3] { false, false, false }, m_curBet, m_bUseBank));
					break;
			}
		}
	}
}