using System;
using Server.Items;
using Server.Targeting;
using Server.Mobiles;
using Server.Gumps;
using Server.Commands;
using System.Collections.Generic;

namespace Server.Events.CTF
{
	public class CTFCommands
	{
		public static void Initialize()
		{
			CommandSystem.Register("CTFKick", AccessLevel.GameMaster, new CommandEventHandler(CTFKick_OnCommand));
			CommandSystem.Register("CTFHelp", AccessLevel.Player, new CommandEventHandler(CTFHelp_OnCommand));
			CommandSystem.Register("CTFShowPlayerStats", AccessLevel.Player, new CommandEventHandler(CTFShowPlayerStats_OnCommand));
			CommandSystem.Register("CTFShowTeamStats", AccessLevel.Player, new CommandEventHandler(CTFShowTeamStats_OnCommand));
			CommandSystem.Register("Team", AccessLevel.Player, new CommandEventHandler(TeamMessage_Command));
			CommandSystem.Register("T", AccessLevel.Player, new CommandEventHandler(TeamMessage_Command));
			CommandSystem.Register("CTFResetScore", AccessLevel.Administrator, new CommandEventHandler(CTFResetScore_Command));
		}

		[Usage("CTFResetScore")]
		[Description("Resets the top score board of CTF")]
		private static void CTFResetScore_Command(CommandEventArgs e)
		{
			CTFData.GameDictionary = new Dictionary<int, CTFScoreData>();
			CTFData.PlayerDictionary = new Dictionary<Mobile, CTFPlayerData>();
			CTFData.PlayerList = new List<CTFPlayerData>();
		}

		[Usage("Team / T")]
		[Description("Sends a message to all members of a certain team")]
		private static void TeamMessage_Command(CommandEventArgs e)
		{
			if (!CTFGame.Running || e.ArgString == null || e.ArgString.Length <= 0 || e.Mobile == null)
				return;

			CTFPlayerGameData pgd = CTFGame.GameData.GetPlayerData(e.Mobile);

			if (pgd != null)
			{
				string message = string.Format("Team [{0}]: {1}", e.Mobile.Name, e.ArgString);

				foreach (CTFPlayerGameData gd in CTFGame.GameData.PlayerList)
					if (gd.InGame && gd.Team == pgd.Team)
						gd.Mob.SendMessage(message);
			}
		}

		[Usage("CTFShowPlayerStats")]
		[Description("Shows the stats of in-game players")]
		private static void CTFShowPlayerStats_OnCommand(CommandEventArgs e)
		{
			Mobile m = e.Mobile;

			if (CTFGame.Running)
			{
				if (m.AccessLevel > AccessLevel.Player)
					m.SendGump(new ShowPlayerStatsDataGump(m));

				else if (CTFGame.GameData.IsInGame(m))
					m.SendGump(new ShowPlayerStatsDataGump(m));
			}
		}

		[Usage("CTFShowTeamStats")]
		[Description("Shows the stats of in-game players")]
		private static void CTFShowTeamStats_OnCommand(CommandEventArgs e)
		{
			Mobile m = e.Mobile;

			if (CTFGame.Running)
			{
				if (m.AccessLevel > AccessLevel.Player)
					m.SendGump(new CTFShowTeamStatsDataGump(m));

				else if (CTFGame.GameData.IsInGame(m))
					m.SendGump(new CTFShowTeamStatsDataGump(m));
			}
		}

		[Usage("CTFHelp")]
		[Description("Shows info about the running ctf")]
		private static void CTFHelp_OnCommand(CommandEventArgs e)
		{
			if (CTFGame.Running)
				e.Mobile.SendGump(new CTFWelcomeGump(e.Mobile));
		}

		[Usage("CTFKick")]
		[Description("Kicks a player from CTF.")]
		private static void CTFKick_OnCommand(CommandEventArgs e)
		{
			e.Mobile.Target = new CTFKickTarget();
			e.Mobile.SendMessage("Select a player to kick from CTF.");
			e.Mobile.SendMessage("BE EXTREMELY CAREFUL WITH THIS! ALL OF THE TARGET'S BELONGINGS WILL BE WIPED!");
		}

		private class CTFKickTarget : Target
		{
			public CTFKickTarget() : base(15, false, TargetFlags.None)
			{
			}

			protected override void OnTarget(Mobile from, object targ)
			{
				if (!(targ is PlayerMobile))
				{
					from.SendMessage("You can only target players.");
					return;
				}

				CTFPlayerGameData pgd = CTFGame.Running ? CTFGame.GameData.GetPlayerData((Mobile)targ) : null;

				if (pgd == null || !pgd.InGame)
				{
					from.SendMessage("This player is not in a CTF game.");
				}
				else
				{
					CTFGame.LeaveGame((PlayerMobile)targ);
					from.SendMessage("Player kicked from CTF.");
					((PlayerMobile)targ).SendMessage("You have been kicked from CTF!");
				}
			}
		}
	}
}