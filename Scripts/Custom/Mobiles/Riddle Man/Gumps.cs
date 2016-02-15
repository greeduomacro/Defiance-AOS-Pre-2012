using System;
using Server.Gumps;
using Server.Network;

namespace Server.Events.Riddle
{
	public class RiddlesGump : AdvGump
	{
		public RiddlesGump()
			: base()
		{
			Closable = true;
			Disposable = true;
			Dragable = true;
			Resizable = false;
			AddPage(0);
			AddBackground(0, 0, 429, 297, 9200);
			AddLabel(18, 16, 0, "Riddles:");

			int count = 0;
			foreach (Riddle riddle in RiddleNPC.Riddles)
			{
				count++;

				AddLabel(17, 28 + (18 * count), 0, count.ToString()+".");
				AddButton(39, 28 + (18 * count), 1210, 1209, count, GumpButtonType.Reply, 0);
				AddLabel(62, 28 + (18 * count), 0, riddle.Question.Length > 35 ? riddle.Question.Substring(0, 30) : riddle.Question);
			}

			AddButton(20, 255, 247, 248, 0, GumpButtonType.Reply, 0);

			AddButton(260, 255, 4029, 248, 20, GumpButtonType.Reply, 0);
			AddLabel(300, 255, 0, "Add new riddle");
		}

		public override void OnResponse(NetState sender, RelayInfo info)
		{
			Mobile m = sender.Mobile;

			if (info.ButtonID == 20)
			{
				if (RiddleNPC.Riddles.Count < 11)
					m.SendGump(new RiddleGump(null, false));

				else
					m.SendMessage("There are already 10 riddles, that is enough for now.");
			}

			else if (info.ButtonID > 0 && info.ButtonID <= RiddleNPC.Riddles.Count)
				m.SendGump(new RiddleGump(RiddleNPC.Riddles[info.ButtonID - 1], true));
		}
	}

	public class RiddleGump : AdvGump
	{
		private Riddle m_Riddle;
		private bool m_Existing;

		public RiddleGump(Riddle riddle, bool existing)
			: base()
		{
			m_Riddle = riddle;
			m_Existing = existing;

			Closable = true;
			Disposable = true;
			Dragable = true;
			Resizable = false;
			AddPage(0);
			AddBackground(0, 0, 400, 400, 9200);
			AddHtml(0, 15, 400, 22, Center("Riddle"), false, false);
			AddLabel(15, 55, 0, "Question:");
			AddBackground(15, 80, 370, 120, 9350);
			AddTextEntry(20, 84, 360, 110, 0, 0, existing? riddle.Question:"");
			AddLabel(15, 220, 0, "Answer:");
			AddBackground(15, 240, 370, 83, 9350);
			AddTextEntry(20, 245, 360, 73, 0, 1, existing ? riddle.Answer : "");
			AddLabel(15, 340, 0, "Reward Amount:");
			AddBackground(118, 336, 249, 30, 9350);
			AddTextEntry(123, 341, 239, 20, 0, 2, existing ? riddle.RewardAmount.ToString() : "");

			if(existing)
				AddButton(158, 374, 2464, 2463, 2, GumpButtonType.Reply, 0);
			AddButton(290, 370, 247, 248, 1, GumpButtonType.Reply, 0);
			AddButton(45, 370, 241, 242, 0, GumpButtonType.Reply, 0);
		}

		public override void OnResponse(NetState sender, RelayInfo info)
		{
			Mobile m = sender.Mobile;

			string q = info.TextEntries[0].Text;
			string a = info.TextEntries[1].Text;
			int amount = 0;

			try { amount = Convert.ToInt32(info.TextEntries[2].Text); }
			catch { }

			amount = Math.Min(Math.Max(amount, 1), 20);

			switch (info.ButtonID)
			{
				case 1:
					if (m_Existing && m_Riddle != null)
					{
						m_Riddle.Question = q;
						m_Riddle.Answer = a;
						m_Riddle.RewardAmount = amount;
					}

					else if (RiddleNPC.Riddles.Count < 11)
						RiddleNPC.Riddles.Add(new Riddle(q, a, amount));

					else if (!m_Existing)
						m.SendMessage("There are already 10 riddles, that is enough for now.");

					break;
				case 2:
					if (m_Existing && m_Riddle != null)
						RiddleNPC.Riddles.Remove(m_Riddle);
					break;
			}
			m.SendGump(new RiddlesGump());
		}
	}
}