using System;
using Server.Items;
using Server.Mobiles;

namespace Server.Commands
{
	public class ShowRegs
	{
		public static void Initialize()
		{
			CommandSystem.Register("ShowRegs", AccessLevel.Player, new CommandEventHandler(ShowRegs_OnCommand));
		}

		private static string[] m_RegsName = { "Mage: BP", "BM", "Ga", "Gi", "MR", "NS", "Sa", "SS" };
		private static string[] m_NecroRegsName = { "Necro: BW", "GD", "DB", "NC", "PI" };
		private const int ReagentWarningLimit = 10;

		private static void CheckArray(Mobile from, Container cont, Type[] regs, string[] regsName)
		{
			string sOverLimit = "";
			string sUnderLimit = "";

			for (int i = 0; i < regs.Length; i++)
			{
				int count = cont.GetAmount(regs[i], true);
				string text = string.Format("{0}:{1}", regsName[i], count.ToString());

				if (count > ReagentWarningLimit)
				{
					if ((sOverLimit.Length != 0) && (i < regs.Length)) sOverLimit += ", ";
					sOverLimit += text;
				}
				else
				{
					if ((sUnderLimit.Length != 0) && (i < regs.Length)) sUnderLimit += ", ";
					sUnderLimit += text;
				}
			}

			if (sOverLimit.Length == 0 && sUnderLimit.Length == 0)
			{
				from.SendAsciiMessage(40, "No regs was found !");
			}
			else
			{
				if (sOverLimit.Length != 0) from.SendAsciiMessage(76, sOverLimit);
				if (sUnderLimit.Length != 0) from.SendAsciiMessage(40, sUnderLimit);
			}
		}

		public static void ShowRegs_OnCommand(CommandEventArgs e)
		{
			Mobile from = e.Mobile;
			Container cont = from.Backpack;

			if (cont == null)
				return;

			CheckArray(from, cont, Loot.RegTypes, m_RegsName);
			CheckArray(from, cont, Loot.NecroRegTypes, m_NecroRegsName);
		}
	}
}