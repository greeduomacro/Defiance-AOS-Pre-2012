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

using System;
using System.Collections;
using System.Net;
using Server.Network;
using Server.Items;
using Server.Accounting;

namespace Server.Gumps
{
	public class SevenGMSkillBallGump : Gump
	{
		private const int FieldsPerPage = 18;
		private Mobile m_From;
		private SevenGMSkillBall m_SkillBall;
		private bool[] GMSkills = new bool[60];
		private uint GMsSelected = 0;
		private int numSkills = 49;
		private string m_GumpHeadline;

		public SevenGMSkillBallGump(SevenGMSkillBall skillball, Mobile from, string gumpheadline)
			: base(20, 30)
		{
			m_From = from;
			m_SkillBall = skillball;
			m_GumpHeadline = gumpheadline;

			constructGump();
		}

		public SevenGMSkillBallGump(SevenGMSkillBall skillball, Mobile from, string gumpheadline, bool[] selected)
			: base(20, 30)
		{
			m_From = from;
			m_SkillBall = skillball;
			m_GumpHeadline = gumpheadline;
			GMSkills = selected;

			foreach (bool f in GMSkills)
				if (f)
					GMsSelected++;

			constructGump();
		}

		public void constructGump()
		{

			AddBackground(0, 0, 520, 620, 2600);

			AddLabel(130, 20, 2213, m_GumpHeadline);

			AddImage(400, 520, 0x0033);
			AddLabel(419, 535, 2213, "" + GMsSelected * m_SkillBall.SkillValue);

			Skills skills = m_From.Skills;

			if (Core.AOS)
				numSkills = skills.Length;
			else
				numSkills = skills.Length - 5;

			int index = 0;
			int rowspan = 0;
			int spacer = 0;

			// add all skills with the buttons in the gump
			for (int i = 0; i < numSkills; ++i)
			{
				bool selected = GMSkills[i];

				if (i >= 19)
					spacer = (i >= 38) ? 30 : 5;

				Skill skill = skills[i];

				if (IsRestrictedSkill((SkillName)i) && GMsSelected < 7)
					AddLabel(152 + rowspan - spacer, 45 + (index * 28), 902, "n/a");

				else if (!selected && GMsSelected < 7)
					AddButton(160 + rowspan - spacer, 49 + (index * 28), 0x15E3, 0x15E7, i + 1, GumpButtonType.Reply, 0);

				AddLabel(25 + rowspan, 45 + (index * 28),
					 selected ? 39 : 902, skill.Name);

				++index;

				if ((i == 18) || (i == 37))
				{
					rowspan += 175;
					index = 0;
				}
			}

			// Cancel Button
			AddButton(50 + rowspan, 170 + (index * 28), 0x0819, 0x0818, -5, GumpButtonType.Reply, 0);

			// Clear Button
			if (GMsSelected > 0)
			{
				AddButton(53 + rowspan, 125 + (index * 28), 0x04B9, 0x04BA, -1, GumpButtonType.Reply, 0);
				AddLabel(70 + rowspan, 123 + (index * 28), 2213, "Clear");
			}

			// Set this skills button
			if (GMsSelected == 7)
			{
				AddButton(54 + rowspan, 80 + (index * 28), 0x081A, 0x081B, -2, GumpButtonType.Reply, 0);
			}

		}

		public bool IsRestrictedSkill(SkillName i)
		{
			// Skills that are n/a at the gump
			return i == SkillName.Blacksmith || i == SkillName.Peacemaking || i == SkillName.Provocation ||
				i == SkillName.Musicianship || i == SkillName.Tailoring || i == SkillName.AnimalTaming ||
				i == SkillName.Lockpicking || i == SkillName.RemoveTrap || i == SkillName.Mining ||
				i == SkillName.Necromancy || i == SkillName.Focus || i == SkillName.Chivalry ||
				i == SkillName.AnimalLore || i == SkillName.Cartography || i == SkillName.Carpentry ||
				i == SkillName.Fishing || i == SkillName.Poisoning || i == SkillName.Alchemy ||
				i == SkillName.ItemID || i == SkillName.Begging || i == SkillName.Fletching ||
				i == SkillName.Veterinary || i == SkillName.Tinkering;
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
				if (GMsSelected < 7)
					GMSkills[info.ButtonID - 1] = true;

				sender.Mobile.SendGump(new SevenGMSkillBallGump(m_SkillBall, sender.Mobile, m_GumpHeadline, GMSkills));
			}

			// Clear
			if (info.ButtonID == -1)
				sender.Mobile.SendGump(new SevenGMSkillBallGump(m_SkillBall, sender.Mobile, m_GumpHeadline, new bool[numSkills]));

			// Set the Skills
			if (info.ButtonID == -2)
			{
				if (!m_SkillBall.IsValidSkillBallUse(sender.Mobile) ||
					m_SkillBall.Deleted || m_SkillBall == null ||
					m_From.Deleted || m_From == null ||
					m_From.Backpack == null ||
					!m_SkillBall.IsChildOf(sender.Mobile.Backpack))
					return;

				m_SkillBall.Delete();

				// set the new skills
				for (int cnt = 0; cnt < numSkills; cnt++)
				{
					if (GMSkills[cnt])
						m_From.Skills[cnt].Base = m_SkillBall.SkillValue;
					else
						m_From.Skills[cnt].Base = 0;
				}
			}

			// Reset - nothing to do
		}

	}

	public class StatsBallGump : Gump
	{
		private StatsBall m_StatsBall;
		private Mobile m_From;
		private int m_str = 0;
		private int m_dex = 0;
		private int m_int = 0;
		private string m_status = "";
		private int m_statusOffset = 0;

		public StatsBallGump(StatsBall skillball, Mobile from)
			: base(20, 30)
		{
			m_From = from;
			m_StatsBall = skillball;

			constructGump();
		}


		public StatsBallGump(StatsBall skillball, Mobile from, int str, int dex, int intel, string status, int statusOffset)
			: base(20, 30)
		{
			m_From = from;
			m_StatsBall = skillball;

			m_str = str;
			m_dex = dex;
			m_int = intel;

			m_status = status;
			m_statusOffset = statusOffset;

			constructGump();
		}


		private void constructGump()
		{
			AddBackground(0, 0, 320, 200, 2600);
			AddLabel(70, 20, 2213, "Stats Ball - Chose your stats");
			AddLabel(85, 52, 902, "Strengh :");
			AddLabel(85, 72, 902, "Dexterity :");
			AddLabel(85, 92, 902, "Intelligence :");

			AddLabel(175, 52, 902, "" + m_str);
			AddLabel(175, 72, 902, "" + m_dex);
			AddLabel(175, 92, 902, "" + m_int);

			AddButton(225, 54, 0x15E3, 0x15E7, 1, GumpButtonType.Reply, 0);
			AddButton(225, 74, 0x15E3, 0x15E7, 2, GumpButtonType.Reply, 0);
			AddButton(225, 94, 0x15E3, 0x15E7, 3, GumpButtonType.Reply, 0);

			if (m_status != "")
				AddLabel(m_statusOffset, 120, 38, "" + m_status);

			AddButton(90, 150, 0x081A, 0x081B, -1, GumpButtonType.Reply, 0);
			AddButton(180, 150, 0x0819, 0x0818, -2, GumpButtonType.Reply, 0);
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

			sender.Mobile.CloseGump(typeof(SetStatGump));

			if (!m_StatsBall.IsChildOf(sender.Mobile.Backpack))
			{
				sender.Mobile.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
				return;
			}

			if (info.ButtonID > 0)
			{
				sender.Mobile.SendGump(new SetStatGump(sender.Mobile, m_StatsBall, (int)info.ButtonID - 1, m_str, m_dex, m_int));
				return;
			}

			// OK Button
			if (info.ButtonID == -1)
			{
				if (m_StatsBall.Deleted || m_StatsBall == null ||
					m_From.Deleted || m_From == null ||
					m_From.Backpack == null ||
					!m_StatsBall.IsChildOf(sender.Mobile.Backpack) ||
					!m_StatsBall.IsValidUse(m_From))
						return; // another pedantic check

				if (m_str + m_int + m_dex > 225)
				{
					m_status = "You can only spread 225 stat points!";
					m_statusOffset = 45;
					sender.Mobile.SendGump(new StatsBallGump(m_StatsBall, sender.Mobile, m_str, m_dex, m_int, m_status, m_statusOffset));
					return;
				}

				if (m_str < 10 || m_dex < 10 || m_int < 10)
				{
					m_status = "You need a minimum of 10 in each stat!";
					m_statusOffset = 35;
					sender.Mobile.SendGump(new StatsBallGump(m_StatsBall, sender.Mobile, m_str, m_dex, m_int, m_status, m_statusOffset));
					return;
				}

				if (m_str > 100 || m_dex > 100 || m_int > 100)
				{
					m_status = "You cant set a stat to more than 100!";
					m_statusOffset = 37;
					sender.Mobile.SendGump(new StatsBallGump(m_StatsBall, sender.Mobile, m_str, m_dex, m_int, m_status, m_statusOffset));
					return;
				}

				m_StatsBall.Delete();

				m_From.RawStr = m_str;
				m_From.RawDex = m_dex;
				m_From.RawInt = m_int;
			}

			// Cancel Button
			// Nothing to do

		}

	}


	public class SetStatGump : Gump
	{
		private Mobile m_From;
		private int m_str;
		private int m_dex;
		private int m_int;
		private StatsBall m_StatsBall;
		private int m_stat;

		public SetStatGump(Mobile from, StatsBall ball, int statToChange, int str, int dex, int intel)
			: base(20, 30)
		{
			m_From = from;
			m_StatsBall = ball;

			m_stat = statToChange;
			m_str = str;
			m_dex = dex;
			m_int = intel;

			AddBackground(0, 0, 90, 60, 5054);

			AddImageTiled(10, 10, 72, 22, 0x52);
			AddImageTiled(11, 11, 70, 20, 0xBBC);
			AddTextEntry(11, 11, 70, 20, 0, 0, "0");
			AddButton(15, 35, 0xFB7, 0xFB8, 1, GumpButtonType.Reply, 0);
			AddButton(50, 35, 0xFB1, 0xFB2, 0, GumpButtonType.Reply, 0);
		}

		public override void OnResponse(NetState state, RelayInfo info)
		{

			if ( !m_StatsBall.IsValidUse(m_From) ||
				m_StatsBall.Deleted || m_StatsBall == null ||
				m_From.Deleted || m_From == null ||
				m_From.Backpack == null ||
				!m_StatsBall.IsChildOf(m_From.Backpack))
					return;

			if (info.ButtonID > 0)
			{
				TextRelay text = info.GetTextEntry(0);

				if (text != null)
				{
					try
					{
						int value = Convert.ToInt32(text.Text);
						if (value < 10 || value > 100)
						{
							m_From.SendMessage("Value must be between 10 and 100.");
						}

						else if (((m_stat == 0) && (m_dex + m_int + value > 225)) ||
						 ((m_stat == 1) && (m_str + m_int + value > 225)) ||
						 ((m_stat == 2) && (m_str + m_dex + value > 225)))
						{
							m_From.SendMessage("You can not exceed the stat cap of 225.  Try setting another stat lower first.");
						}
						else
						{
							if (m_stat == 0)
								m_str = value;
							else
							{
								if (m_stat == 1)
									m_dex = value;
								else
								{
									if (m_stat == 2)
										m_int = value;
								}
							}

						}
					}
					catch
					{
						m_From.SendMessage("Bad format. Number expected.");
					}
				}
			}
			m_From.SendGump(new StatsBallGump(m_StatsBall, m_From, m_str, m_dex, m_int, "", 0));
			return;
		}
	}
}