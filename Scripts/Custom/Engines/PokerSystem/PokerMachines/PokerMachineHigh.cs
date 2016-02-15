using System;
using System.Collections.Generic;
using Server;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;

namespace Server.Items
{
	[FlipableAttribute(0x1173, 0x1174)]
	public class PokerMachineHigh : PokerMachine
	{
		public override int MaxBet { get { return 5000; } }
		public override int MinBet { get { return 1000; } }
		public override int BetChange { get { return 1000; } }

		private static int[] winTableHigh =
		{
			100000,	// RoyalFlush - Not read, jackpot is given
			50000,	// StraightFlush
			30000,	// FourOfAKind
			10000,	// FullHouse
			5000,	// Flush
			4500,	// Straight
			2500,	// ThreeOfAKind
			1500,	// TwoPairs
			500,	// Pair
			0,		// None
		};

		public override int[] m_WinningsTable { get { return winTableHigh; } }

		[Constructable]
		public PokerMachineHigh()
		{
		}

		public PokerMachineHigh(Serial serial)
			: base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write((int)0); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();
		}
	}
}