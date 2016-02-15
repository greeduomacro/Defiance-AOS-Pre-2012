using System;
using System.Collections.Generic;
using System.Text;
using Server.Gumps;
using Server.Network;
using Server.Mobiles;
using Server.Items;

namespace Server.Events.CTF
{
	public class CTFBaseGump : AdvGump
	{
		protected int Width;
		protected int Height;
		protected string Title;//160,125
		protected int TotalWidth { get { return Width + 220; } }
		protected int TotalHeight { get { return Height + 220; } }
		protected virtual int Main_X { get { return 160; } }
		protected int Main_Y { get { return 125; } }
		protected int Loc_X { get { return TotalWidth + 50; } }
		protected int Loc_Y { get { return TotalHeight + 50; } }

		public CTFBaseGump() : this(true) { }

		public CTFBaseGump(bool presetdata) : base()
		{
			if (presetdata)
				SetData();
		}

		#region SetData
		public virtual void SetData()
		{
			AddBackground(50, 50, TotalWidth, TotalHeight, 2620);
			AddAlphaRegion(55, 55, TotalWidth - 10, TotalHeight - 10);

			AddImage(0, 33, 10400);
			AddImage(Loc_X - 32, 33, 10410);

			if (TotalHeight > 320)
			{
				AddImage(Loc_X - 32, 200, 10411);
				AddImage(Loc_X - 32, 355, 10412);
				AddImage(0, 200, 10401);
				AddImage(0, 355, 10402);
			}

			AddImage(70, Loc_Y - 80, 5504);

			AddItem(90, 70, 5671, 33);
			AddItem(Loc_X - 70, 70, 5671, 1266);
			AddItem(90, 170, 5671, 1150);
			AddItem(Loc_X - 70, 170, 5671, 1109);

			AddHtml(50, 70, TotalWidth, 19, Colorize(Center(Title), "FFFFFF"), false, false);
			AddButton(Loc_X - 50, Loc_Y - 40, 4017, 4018, 0, GumpButtonType.Reply, 0);
		}
		#endregion
	}

	public class CTFBaseDataGump : CTFBaseGump
	{
		protected int Location;
		protected List<string> Data = new List<string>();
		protected int[] Collumns;
		protected object ScoreData;

		public CTFBaseDataGump():base(true)
		{
		}

		public CTFBaseDataGump(object data):base(false)
		{
			ScoreData = data;
		}

		public CTFBaseDataGump(int start) : base(false)
		{
			Location = start;
		}
		#region SetData
		public override void SetData()
		{
			foreach (int i in Collumns)
				Width += i + 2;

			Height += Data.Count / Collumns.Length * 18;
			base.SetData();
		}
		#endregion
	}

	public class CTFAllGamesDataGump : CTFBaseDataGump
	{
		public CTFAllGamesDataGump() : base()
		{
		}
		#region SetData
		public override void SetData()
		{
			Title = "CTF Game Scoreboard";
			Collumns = new int[] { 90, 90, 120, 120, 120, 120 };

			Data.Add("Date:");
			Data.Add("Total Caps:");
			Data.Add("Black Caps/Losses:");
			Data.Add("Blue Caps/Losses:");
			Data.Add("Red Caps/Losses:");
			Data.Add("White Caps/Losses:");

			foreach (CTFScoreData sd in CTFData.GameDictionary.Values)
			{
				Title = "CTF Game Scoreboard";
				DateTime date = sd.Date;
				Data.Add(String.Format("{0}/{1}/{2}", date.Day, date.Month, date.Year));
				Data.Add(sd.TotalCaptures.ToString());
				foreach (CTFTeam team in CTFGame.TeamArray)
				{
					if (team != null)
					{
						bool found = false;
						foreach (CTFTeamScoreData tsd in sd.TeamList)
						{
							if (tsd.Team == team)
							{
								Data.Add(tsd.Captures.ToString() + " / " + tsd.Losses.ToString());
								found = true;
								break;
							}
						}

						if (!found)
							Data.Add("X");
					}
				}
			}
			base.SetData();

			AddBackground(Main_X - 30, Main_Y - 20, Width + 60, Height + 40, 3500);
			AddTable(Main_X, Main_Y, Collumns, Data, null, 1, 2);

			int nr = Data.Count / Collumns.Length - 1;
			for (int i = 0; i < nr; i++)
			{
				AddButton(Main_X - 15, Main_Y + 2 + 18 * (1 + i), 9762, 9763, i + 10, GumpButtonType.Reply, 0);
			}
		}

		public override void OnResponse(NetState sender, RelayInfo info)
		{
			Mobile from = sender.Mobile;

			if (info.ButtonID != 0)
			{
				int loc = info.ButtonID - 10;

				CTFScoreData sd = null;
				if (CTFData.GameDictionary.TryGetValue(loc, out sd))
					from.SendGump(new CTFGameDataGump(sd));

				else
					Console.WriteLine("does not exist {0}", loc);
			}
		}

		#endregion
	 }

	public class CTFGameDataGump : CTFBaseDataGump
	{
		public CTFGameDataGump(CTFScoreData sd) : base(sd)
		{
			SetData();
		}

		#region SetData
		public override void SetData()
		{
			Title = "CTF Last 10 Games Scoreboard";
			CTFScoreData sd = (CTFScoreData)ScoreData;
			Collumns = new int[] { 130, 90, 50, 50, 60, 50, 50 };

			Data.Add("Name:");
			Data.Add("Team:");
			Data.Add("Kills:");
			Data.Add("Deaths:");
			Data.Add("Captures:");
			Data.Add("Returns:");
			Data.Add("Score:");
			sd.PlayerList.Sort();
			foreach (CTFPlayerScoreData psd in sd.PlayerList)
			{
				string name = psd.Mob == null? "Anonymous" :psd.Mob.Name;
				Data.Add(name.Replace(" ", "_"));
				Data.Add(psd.Team.Name);
				Data.Add(psd.Kills.ToString());
				Data.Add(psd.Deaths.ToString());
				Data.Add(psd.Captures.ToString());
				Data.Add(psd.Returns.ToString());
				Data.Add(psd.Score.ToString());
			}
			base.SetData();

			AddBackground(Main_X - 30, Main_Y - 20, Width + 60, Height + 40, 3500);
			AddTable(Main_X, Main_Y, Collumns, Data, null, 1, 2);
		}
		#endregion

		public override void OnResponse(NetState sender, RelayInfo info)
		{
			Mobile from = sender.Mobile;
			from.SendGump(new CTFAllGamesDataGump());
		}
	}

	public class CTFPlayerDataGump : CTFBaseDataGump
	{
		public CTFPlayerDataGump(int start) : base(start)
		{
			SetData();
		}

		#region SetData
		public override void SetData()
		{
			Title = "CTF 10 Games Average Scoreboard";
			Collumns = new int[] { 40, 130, 170, 50, 50, 60,60,50 };
			int max = CTFData.PlayerList.Count;

			if (Location < max)
			{
				int end = Location + 20;

				if (end > max)
					end = max;

				Data.Add("Place:");
				Data.Add("Name:");
				Data.Add("Rank:");
				Data.Add("Kills:");
				Data.Add("Deaths:");
				Data.Add("Captures:");
				Data.Add("Returns:");
				Data.Add("Score:");

				for (int i = Location; i < end; i++)
				{
					CTFPlayerData pd = CTFData.PlayerList[i];

					Data.Add((i+1).ToString());
					string name = pd.Mob == null ? "Anonymous" : pd.Mob.Name;
					Data.Add(name.Replace(" ", "_"));

					Data.Add(CTFRankRewardSystem.CTFRank[pd.Rank]);
					Data.Add(pd.Kills.ToString());
					Data.Add(pd.Deaths.ToString());
					Data.Add(pd.Captures.ToString());
					Data.Add(pd.Returns.ToString());
					Data.Add(pd.Score.ToString());
				}
			}
			base.SetData();

			AddBackground(Main_X - 30, Main_Y - 20, Width + 60, Height + 40, 3500);
			AddTable(Main_X, Main_Y, Collumns, Data, null, 1, 2);

			int nr = Data.Count / Collumns.Length - 1;
			for (int i = 0; i < nr; i++)
			{
				AddButton(Main_X - 15, Main_Y + 2 + 18 * (1 + i), 9762, 9763, i + 100000 + Location, GumpButtonType.Reply, 0);
			}

			if (Location - 20 >= 0)
				AddButton(160, Loc_Y - 80, 4508, 4508, 1, GumpButtonType.Reply, 0);

			if(Location + 20 < max)
				AddButton(TotalWidth - 160, Loc_Y - 80, 4502, 4502, 2, GumpButtonType.Reply, 0);
		}

		public override void OnResponse(NetState sender, RelayInfo info)
		{
			Mobile from = sender.Mobile;

			if (info.ButtonID == 1 && (Location - 20) >= 0)
				from.SendGump(new CTFPlayerDataGump(Location - 20));

			else if (info.ButtonID == 2 && (Location + 20 ) < CTFData.PlayerList.Count)
				from.SendGump(new CTFPlayerDataGump(Location + 20));

			else if (info.ButtonID > 9999)
			{
				int loc = info.ButtonID - 100000;
				if (loc >= 0 && loc < CTFData.PlayerList.Count)
					from.SendGump(new CTFPlayerGameDataGump(CTFData.PlayerList[loc]));
			}
		}
		#endregion
	}

	public class CTFPlayerGameDataGump : CTFBaseDataGump
	{
		public CTFPlayerGameDataGump(CTFPlayerData pd) : base(pd)
		{
			SetData();
		}

		#region SetData
		public override void SetData()
		{
			Title = "CTF Player Gamedata";
			CTFPlayerData pd = (CTFPlayerData)ScoreData;

			Collumns = new int[] { 100, 50, 50, 50, 50, 50 };

			Data.Add("Team:");
			Data.Add("Kills:");
			Data.Add("Deaths:");
			Data.Add("Captures:");
			Data.Add("Returns:");
			Data.Add("Score:");

			foreach (CTFPlayerScoreData psd in pd.GameDataList)
			{
				Data.Add(psd.Team.Name);
				Data.Add(psd.Kills.ToString());
				Data.Add(psd.Deaths.ToString());
				Data.Add(psd.Captures.ToString());
				Data.Add(psd.Returns.ToString());
				Data.Add(psd.Score.ToString());
			}
			base.SetData();

			AddBackground(Main_X - 30, Main_Y - 20, Width + 60, Height + 40, 3500);
			AddTable(Main_X, Main_Y, Collumns, Data, null, 1, 2);
		}

		public override void OnResponse(NetState sender, RelayInfo info)
		{
			Mobile from = sender.Mobile;
			from.SendGump(new CTFPlayerDataGump(0));
		}
		#endregion
	}

	#region ShowPlayerStatsDataGump
	public class ShowPlayerStatsDataGump : CTFBaseDataGump
	{
		public ShowPlayerStatsDataGump(Mobile from)
			: base()
		{
			from.CloseGump(typeof(ShowPlayerStatsDataGump));
		}

		#region SetData
		public override void SetData()
		{
			Title = "CTF In-Game Statistics";

			Collumns = new int[] { 130, 100, 50, 50, 60, 50, 50 };

			Data.Add("Name:");
			Data.Add("Team:");
			Data.Add("Kills:");
			Data.Add("Deaths:");
			Data.Add("Captures:");
			Data.Add("Returns:");
			Data.Add("In-Game:");

			foreach (CTFPlayerGameData pgd in CTFGame.GameData.PlayerList)
			{
				string name = pgd.Mob == null ? "Anonymous" : pgd.Mob.Name;
				Data.Add(name.Replace(" ", "_"));
				Data.Add(pgd.Team.Name);
				Data.Add(pgd.Kills.ToString());
				Data.Add(pgd.Deaths.ToString());
				Data.Add(pgd.Captures.ToString());
				Data.Add(pgd.Returns.ToString());
				Data.Add(pgd.InGame.ToString());
			}
			base.SetData();

			AddBackground(Main_X - 30, Main_Y - 20, Width + 60, Height + 40, 3500);
			AddTable(Main_X, Main_Y, Collumns, Data, null, 1, 2);
		}
		#endregion
	}
	#endregion

	#region ShowTeamStatsDataGump
	public class CTFShowTeamStatsDataGump : CTFBaseDataGump
	{
		public CTFShowTeamStatsDataGump(Mobile from)
			: base()
		{
			from.CloseGump(typeof(CTFShowTeamStatsDataGump));
		}

		#region SetData
		public override void SetData()
		{
			Title = "CTF In-Game Statistics";

			Collumns = new int[] { 120, 60, 70};

			Data.Add("Team:");
			Data.Add("Captures:");
			Data.Add("FlagLosses:");
			Data.Add("All Teams");
			Data.Add(CTFGame.GameData.TotalCaptures.ToString());
			Data.Add("N/A");

			foreach (CTFTeamGameData tgd in CTFGame.GameData.TeamList)
			{
				Data.Add(tgd.Team.Name);
				Data.Add((tgd.Captures).ToString());
				Data.Add((tgd.FlagLosses).ToString());
			}
			base.SetData();

			AddBackground(Main_X - 30, Main_Y - 20, Width + 60, Height + 40, 3500);
			AddTable(Main_X, Main_Y, Collumns, Data, null, 1, 2);
		}
		#endregion
	}
	#endregion

	#region WelcomeGump
	public class CTFWelcomeGump : CTFBaseGump
	{
		public CTFWelcomeGump(Mobile from) : base()
		{
			from.CloseGump(typeof(CTFWelcomeGump));

			string text =
			"<I><U>How to capture a flag</U></I><BR>" +
			"First thing you will need to do is to find another teams flag, " +
			"when you have found their flag you have to walk over it to take it. " +
			"When you have the flag double click it in your backpack and target your own flag." +

			"<BR><BR><I><U>Commands</U></I>" +
			"<br>* [CTFHelp will show you this screen." +
			"<br>* [CTFShowPlayerStats will show the statistics of all in-game players." +
			"<br>* [CTFShowTeamStats will show the statistics of all in-game teams." +
			"<br>* [T or [Team will allow you to type messages to team mates." +

			"<BR><BR><I><U>Hints</U></I>" +
			"<br>* You cannot capture a flag without your own flag in your base." +
			"<br>* When a player takes a flag his robe will turn yellow." +
			"<br>* You can also target a team mate to transfer the flag to him." +
			"<br>* Special weapons can spawn on the pentagram."+

			"<BR><BR><I><U>Score System</U></I>" +
			"<br>Supporting your team is an important role in this game, as that is the only way to succes." +
			 " The score is calculated by the captures minus the losses of your team." +
			" There is a minimum of 1 capture or 5 kills to receive any score at all." +
			" The other information such as deaths and returns is just for your info." +
			" A capture means that you have stolen a flag and brought it to your home." +
			" A loss occurs when a flag is stolen and brought to someone elses home." +
			" If a flag is intercepted meanwhile it does not count as a capture/loss.";

			AddHtml(Main_X, Main_Y, 345, 249, text, true, true);

			if (CTFGame.GameData != null)
			{
				CTFTeam team = CTFGame.GameData.GetPlayerTeam(from);
				if (team != null)
					AddHtml(148, 457, 354, 17, string.Format("<BASEFONT COLOR=#EFEF5A>You are in the {0} team.</BASEFONT>", team.Name), false, false);

				//AddHtml(148, 360, 212, 17, string.Format("<BASEFONT COLOR=#EFEF5A>Max Players Per Team: {0}</BASEFONT>", game.TeamSize), false, false);
				//AddHtml(148, 379, 135, 17, string.Format("<BASEFONT COLOR=#EFEF5A>Players: {0}</BASEFONT>", game.TotalNumOfPlayers), false, false);
				//AddHtml(148, 398, 135, 17, string.Format("<BASEFONT COLOR=#EFEF5A>Teams: {0}</BASEFONT>", CTFGame.Teams), false, false);
				//AddHtml(148, 417, 135, 17, string.Format("<BASEFONT COLOR=#EFEF5A>Game Time: {0}</BASEFONT>", CTFGame.GameLength), false, false);
				//AddHtml(148, 436, 135, 17, string.Format("<BASEFONT COLOR=#EFEF5A>Draw Time: {0}</BASEFONT>", CTFGame.DrawLength), false, false);
			}
		}

		public override void SetData()
		{
			Width = 345;
			Height = 249;

			Title = "CTF Help";

			base.SetData();
		}
	}
	#endregion

	#region RegradeGump
	public class CTFRegradeGump : CTFBaseGump
	{
		public CTFRegradeGump(Mobile m, int oldrank, int newrank) : base()
		{
			string str  = "promoted";
			if (newrank < oldrank)
				str = "demoted";
			AddHtml(68, 39, 181, 15, HtmlUtility.Color(String.Format("You have been {0} to:", str), HtmlUtility.HtmlYellow), false, false);
			AddImage(132, 81, 5587);
			AddHtml(72, 58, 175, 15, HtmlUtility.Color(string.Format("<center>{0}</center>", CTFRankRewardSystem.CTFRank[newrank]), HtmlUtility.HtmlYellow), false, false);

			m.PlaySound(1207);
		}
	}
	#endregion

	#region JoinGump
	public class CTFJoinGump : AdvGump
	{
		private int personalPrice = CTFGame.Price;

		public CTFJoinGump(PlayerMobile mob)
			: base(50, 50)
		{
			Closable = true;
			Disposable = true;
			Dragable = true;
			Resizable = false;
			AddPage(0);
			AddBackground(0, 0, 400, 275, 2620);
			AddAlphaRegion(5, 5, 490, 265);

			AddHtml(0, 15, 400, 20, Colorize(Center("CTF Join Gump"), "FFFFFF"), false, false);

			int[] collumns = new int[] { 150, 150 };
			string[] colors = new string[] { "FFFFFF", "FFFFFF" };
			List<string> list = new List<string>();

			int max = CTFData.PlayerList.Count;
			for (int i = 0; i < max; i++)
			{
				CTFPlayerData pd = CTFData.PlayerList[i];
				if (pd.Mob == mob)
				{
					if (pd.Score < 0)
					{
						int penalty = 1 + pd.Score / (-3);
						personalPrice *= penalty;
					}
				}
			}

			list.Add("Price:"); list.Add(mob.HasDonated ? "Free" : personalPrice + " (gp)");
			list.Add("Game Time:"); list.Add(CTFGame.GameLength.TotalMinutes.ToString() + " (min.)");
			list.Add("Undressing:"); list.Add("Auto");
			list.Add("Current Players:"); list.Add(CTFGame.PlayerJoinList.Count.ToString());
			list.Add("Current Prize Money:"); list.Add(CTFGame.PrizeMoney.ToString() + " (gp)");

			AddTable(55, 90, collumns, list, colors);
			AddHtml(0, 185, 400, 20, Colorize(Center("Do you wish to join this game?"), "FFFFFF"), false, false);
			AddButton(250, 220, 247, 248, 1, GumpButtonType.Reply, 0);
			AddButton(90, 220, 242, 243, 0, GumpButtonType.Reply, 0);
		}

		public override void OnResponse(NetState sender, RelayInfo info)
		{
			if (info.ButtonID == 1)
			{
				PlayerMobile from = (PlayerMobile)sender.Mobile;

				if (from != null && CTFGame.Open)
				{
					CTFPlayerGameData pgd = CTFGame.GameData.GetPlayerData(from);

					if (pgd != null)
						from.SendMessage("You are already playing!");

					else if (from.SkillsTotal < 6000)
						from.SendMessage("You do not have enough skills to join the ctf-game.");

					else if (Factions.Sigil.ExistsOn(from))
						from.SendLocalizedMessage(1061632); // You can't do that while carrying the sigil.

					else if (from.Spell != null)
						from.SendLocalizedMessage(1049616); // You are too busy to do that at the moment.

					else if (from.BodyMod != 0)
						from.SendLocalizedMessage(1061628); // You can't do that while polymorphed.

					else if (!from.HasDonated && !Banker.Withdraw(from, personalPrice))
						from.SendMessage("Your bank-account does not contain enough money.");

					else if (!SunnySystem.Undress(from))
						from.SendMessage("Something went wrong while undressing, please notify staff.");

					else if (!from.IsNaked())
						from.SendMessage("Your backpack must be empty and you must also be naked to join.");

					else if (from.Mounted)
						from.SendLocalizedMessage(1042561); // Please dismount first.

					else
					{
						Effects.SendLocationEffect( from.Location, from.Map, 0x3728, 10, 10 );
						Effects.PlaySound( from.Location, from.Map, 0x288 );

						if (!from.Alive)
							from.Resurrect();

						from.CloseAllGumps();

						if (from.Target != null)
							Targeting.Target.Cancel(from);

						CTFGame.PrizeMoney += (personalPrice > 10)? personalPrice : personalPrice * 300;
						from.InvalidateProperties();
						from.SendGump(new CTFWelcomeGump(from));
						CTFGame.PlayerJoinList.Add(from);
						from.MoveToWorld(CTFGame.Stone.Location, CTFGame.Stone.Map);
					}
				}
				else
					from.SendMessage("The CTF game is closed.");
			}
		}
	}

	public class CTFExitGump : AdvGump
	{
		public CTFExitGump(PlayerMobile mob)
			: base(50, 50)
		{
			Closable = true;
			Disposable = true;
			Dragable = true;
			Resizable = false;
			AddPage(0);
			AddBackground(0, 0, 400, 275, 2620);
			AddAlphaRegion(5, 5, 490, 265);

			AddHtml(0, 15, 400, 20, Colorize(Center("CTF Exit Confirmation"), "FFFFFF"), false, false);

			int[] collumns = new int[] { 150, 150 };
			string[] colors = new string[] { "FFFFFF", "FFFFFF" };
			AddHtml(0, 55, 400, 20, Colorize(Center("If you select OKAY,"), "FFFFFF"), false, false);
			AddHtml(0, 75, 400, 20, Colorize(Center("you will be removed from the game"), "FFFFFF"), false, false);
			AddHtml(0, 95, 400, 20, Colorize(Center("and you will not be able to return!"), "FFFFFF"), false, false);
			AddHtml(0, 185, 400, 20, Colorize(Center("Are you sure you wish to exit this game?"), "FFFFFF"), false, false);
			AddButton(250, 220, 247, 248, 1, GumpButtonType.Reply, 0);
			AddButton(90, 220, 242, 243, 0, GumpButtonType.Reply, 0);
		}

		public override void OnResponse(NetState sender, RelayInfo info)
		{
			if (info.ButtonID == 1)
			{
				PlayerMobile from = (PlayerMobile)sender.Mobile;
				CTFGame.LeaveGame(from);
			}
		}
	}

	#endregion
}