/*
 * Copyright (c) 2005, Kai Sassmannshausen <kai@sassie.org>
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions
 * are met:
 *
 * - Redistributions of source code must retain the above copyright
 * notice, this list of conditions and the following disclaimer.
 *
 * - Redistributions in binary form must reproduce the above copyright
 * notice, this list of conditions and the following disclaimer in the
 * documentation and/or other materials provided with the
 * distribution.
 *
 * - Neither the name of Kai Sassmannshausen, nor the names of its
 * contributors may be used to endorse or promote products derived from
 * this software without specific prior written permission.
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND
 * CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING,
 * BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS
 * FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE
 * COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT,
 * INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
 * (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
 * SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION)
 * HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT,
 * STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
 * ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED
 * OF THE POSSIBILITY OF SUCH DAMAGE.
 *
 *  Stats Ball
 *  Version 0
 */

// Version of Silver - DefianceUO AOS

using System;
using System.Collections;
using System.Net;
using Server.Network;
using Server.Items;
using Server.Gumps;
using Server.Accounting;
using Server.Engines.RewardSystem;

namespace Server.Items
{
	public class NewpStatsball : Item, ITempItem
	{
		private DateTime m_RemovalTime;
		private string m_PropertyString;

		public DateTime RemovalTime { get { return m_RemovalTime; } }
		public string PropertyString { get { return m_PropertyString; } set { m_PropertyString = value; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public int DaysLeft
		{
			get { return (int)(m_RemovalTime - DateTime.Now).TotalDays; }
			set
			{
				m_RemovalTime = DateTime.Now + TimeSpan.FromDays(Math.Min(value, 365));
				TemporaryItemSystem.Verify(this);
			}
		}

		[Constructable]
		public NewpStatsball() : this( 7 )
		{
		}

		[Constructable]
		public NewpStatsball(int days) : base(3699)
		{
			m_RemovalTime = DateTime.Now + TimeSpan.FromDays( Math.Min(days, 365) );
			TemporaryItemSystem.Verify(this);

			Movable = false;
			Hue = 1266;
			Name = "A New Player Stats Ball";
			LootType = LootType.Blessed;
		}

		public NewpStatsball(Serial serial)
			: base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write((int)1); // version

			writer.Write(m_RemovalTime);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();

			switch(version)
			{
				case 1:
				m_RemovalTime = reader.ReadDateTime();
				TemporaryItemSystem.Verify(this);
				break;
			}
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );
			list.Add( "Use this before training your stats (" + m_PropertyString + ")" );
		}

		public override void OnDoubleClick(Mobile from)
		{
			if (from == null || from.Deleted || from.Backpack == null)
				return; /* pedantic check */

			if (!IsChildOf(from.Backpack) )
			{
				from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
				return;
			}
			if ( from.Str + from.Dex + from.Int > 120 )
			{
				from.SendMessage( "Your stats are too high to use this" );
				Delete();
				return;
			}

			from.CloseGump(typeof(NewpStatsballGump));
			from.SendGump(new NewpStatsballGump(this, from));

		}
	}
}