using System;
using System.Collections.Generic;
using Server;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;

namespace Server.Items
{
	[FlipableAttribute(0x1173, 0x1174)]
	public class PokerMachineLow : PokerMachine
	{
		public override int MaxBet { get { return 50; } }
		public override int MinBet { get { return 10; } }
		public override int BetChange { get { return 10; } }

		private static int[] winTableLow =
		{
			1000,	// RoyalFlush - Not read, jackpot is given
			600,	// StraightFlush
			300,	// FourOfAKind
			150,	// FullHouse
			55,		// Flush
			50,		// Straight
			35,		// ThreeOfAKind
			15,		// TwoPairs
			10,	 	// Pair
			0,	 	// None
		};

		public override int[] m_WinningsTable { get { return winTableLow; } }

		[Constructable]
		public PokerMachineLow()
		{
		}

		public PokerMachineLow(Serial serial)
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