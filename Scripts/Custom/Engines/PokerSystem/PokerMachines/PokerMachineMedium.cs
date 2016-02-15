using System;
using System.Collections.Generic;
using Server;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;

namespace Server.Items
{
	[FlipableAttribute(0x1173, 0x1174)]
	public class PokerMachineMedium : PokerMachine
	{
		public override int MaxBet { get { return 500; } }
		public override int MinBet { get { return 100; } }
		public override int BetChange { get { return 100; } }

		private static int[] winTableMedium =
		{
			10000,  // RoyalFlush - Not read, jackpot is given
			6000,   // StraightFlush
			3000,   // FourOfAKind
			1500,   // FullHouse
			550,	// Flush
			500,	// Straight
			350,	// ThreeOfAKind
			150,	// TwoPairs
			80,	 // Pair
			0,	 // None
		};

		public override int[] m_WinningsTable { get { return winTableMedium; } }

		[Constructable]
		public PokerMachineMedium()
		{
		}

		public PokerMachineMedium(Serial serial)
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