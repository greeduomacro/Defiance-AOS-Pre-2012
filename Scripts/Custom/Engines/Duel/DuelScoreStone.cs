//////////////////////////////\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
//			   This is a Sunny Production ©2005					\\
//					 Based on RunUO©							\\
//					Version: Beta 1.0							\\
//		Many thanks to my Csharp tutors Will and Eclipse		\\
//////////////////////////////\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
//
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Items;
using System.Net;
using Server.Gumps;

namespace Server.Events.Duel
{
	public class DuelScoreStone : Item
	{
		[Constructable]
		public DuelScoreStone() : base(3796)
		{
			Hue = 1122;
			Name = "Duel Score Stone";
			Movable = false;
		}

		[CommandProperty(AccessLevel.Counselor)]
		public DateTime LastPayout
		{
			get { return DuelScoreSystem.LastPayout; }
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public bool RunPayoutNow
		{
			get { return false; }
			set
			{
				if(value)
					DuelScoreSystem.Payout();
			}
		}

		public override void OnDoubleClick(Mobile from)
		{
			from.CloseGump(typeof(DuelScoreGump));
			from.SendGump(new DuelScoreGump(from));
		}

		public DuelScoreStone(Serial serial) : base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(0); //version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadInt();
		}
	}
}