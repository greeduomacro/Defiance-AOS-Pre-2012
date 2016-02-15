/*
 * Copyright (c) 2006, Kai Sassmannshausen <kai@sassie.org>
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
 */

// Version of Silver - DefianceUO AOS

using System;
using System.Collections;
using System.Net;
using Server.Network;
using Server.Items;
using Server.Accounting;

namespace Server.Gumps
{
	public class NewpSkillballGump : Gump
	{
		private const int FieldsPerPage = 18;
		private Mobile m_From;
		private NewpSkillball m_SkillBall;
		private bool[] ChosenSkills = new bool[60];
		private int numChosen = 0;
		private int numSkills = 52;
		private string m_GumpHeadline;

		public NewpSkillballGump(NewpSkillball skillball, Mobile from, string gumpheadline) : base(20, 30)
		{
			m_From = from;
			m_SkillBall = skillball;
			m_GumpHeadline = gumpheadline;

			constructGump();
		}

		public NewpSkillballGump(NewpSkillball skillball, Mobile from, string gumpheadline, bool[] selected)
			: base(20, 30)
		{
			m_From = from;
			m_SkillBall = skillball;
			m_GumpHeadline = gumpheadline;
			ChosenSkills = selected;

			foreach (bool f in ChosenSkills)
				if (f)
					numChosen++;

			constructGump();
		}

		public void constructGump()
		{

			AddBackground(0, 0, 620, 620, 2600);

			AddLabel(150, 20, 2213, m_GumpHeadline);

			Skills skills = m_From.Skills;

			int index = 0;
			int rowspan = 0;
			int spacer = 0;

			// add all skills with the buttons in the gump
			for (int i = 0; i < numSkills; ++i)
			{
				bool selected = ChosenSkills[i];

				if (i >= 19)
					spacer = (i >= 38) ? 30 : 5;

				Skill skill = skills[i];

				if ( !selected && numChosen < 7 && !(i == 47 && !ChosenSkills[21]) )
					AddButton(190 + rowspan - spacer, 49 + (index * 28), 0x15E3, 0x15E7, i + 1, GumpButtonType.Reply, 0);

				AddLabel(25 + rowspan, 45 + (index * 28), selected ? 39 : 902, skill.Name);
				AddLabel(160 + rowspan - spacer, 45 + (index * 28), selected ? 39 : 902, "" + (int)SkillValue(i) );

				++index;

				if ((i == 18) || (i == 37))
				{
					rowspan += 200; //175
					index = 0;
				}
			}

			// Number of Skills Chosen
			AddLabel(25 + rowspan, 101 + ( index * 28 ), 2213, "Skills chosen: " + numChosen );

			// Cancel Button
			AddButton(95 + rowspan, 170 + (index * 28), 0x0819, 0x0818, -5, GumpButtonType.Reply, 0);

			// Clear Button
			if (numChosen > 0)
			{
				AddButton(53 + rowspan, 125 + (index * 28), 0x04B9, 0x04BA, -1, GumpButtonType.Reply, 0);
				AddLabel(70 + rowspan, 123 + (index * 28), 2213, "Clear");
			}

			// Okay button
			if (numChosen == 7)
			{
				AddButton(35 + rowspan, 170 + (index * 28), 0x081A, 0x081B, -2, GumpButtonType.Reply, 0);
			}

		}

		public double SkillValue(int idx)
		{
			if(idx == 50 || idx == 46)
				return 100.0;
			if(idx == 3 || idx == 4 || idx == 6 || idx == 10 || idx == 19 || idx == 36 || idx == 48 )
				return 90.0;
			if(idx == 12 || idx == 13 || idx == 18 || idx == 22 || idx == 44 || idx == 45 || idx == 47 )
				return 70.0;
			if(idx == 0 || idx == 7 || idx == 8 || idx == 11 || idx == 23 || idx == 30 || idx == 34 || idx == 35 || idx == 37 )
				return 65.0;

			return 80.0;
		}

		public override void OnResponse(NetState state, RelayInfo info)
		{
			NetState sender = state;
			m_From = state.Mobile;

			// some anti crash/abuse checks
			if (sender == null ||
				sender.Mobile == null || sender.Mobile.Deleted ||
				sender.Mobile.Backpack == null ||
				info == null)
				return;

			if (!m_SkillBall.IsChildOf(sender.Mobile.Backpack))
			{
				sender.Mobile.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
				return;
			}

			// a skill was chosen
			if (info.ButtonID >= 1 && info.ButtonID <= numSkills)
			{
				if (numChosen < 7)
					ChosenSkills[info.ButtonID - 1] = true;

				sender.Mobile.SendGump(new NewpSkillballGump(m_SkillBall, sender.Mobile, m_GumpHeadline, ChosenSkills));
			}

			// Clear
			if (info.ButtonID == -1)
				sender.Mobile.SendGump(new NewpSkillballGump(m_SkillBall, sender.Mobile, m_GumpHeadline, new bool[numSkills]));

			// Set the Skills
			if (info.ButtonID == -2)
			{
				if (	m_SkillBall.Deleted || m_SkillBall == null ||
					m_From.Deleted || m_From == null ||
					m_From.Backpack == null ||
					!m_SkillBall.IsChildOf(sender.Mobile.Backpack))
					return;

				m_SkillBall.Delete();

				// set the new skills
				for (int i = 0; i < numSkills; i++)
				{
					if (ChosenSkills[i])
						m_From.Skills[i].Base = SkillValue(i);
					else
						m_From.Skills[i].Base = 0;
				}
			}

			// Reset - nothing to do
		}

	}
}