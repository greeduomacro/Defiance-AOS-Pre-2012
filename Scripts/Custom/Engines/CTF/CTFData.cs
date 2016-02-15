using Server.Mobiles;
using System;
using System.Collections.Generic;


namespace Server.Events.CTF
{
	public static class CTFData
	{
		#region Members
		public static Dictionary<int, CTFScoreData> GameDictionary = new Dictionary<int, CTFScoreData>();
		public static Dictionary<Mobile, CTFPlayerData> PlayerDictionary = new Dictionary<Mobile, CTFPlayerData>();
		public static List<CTFPlayerData> PlayerList = new List<CTFPlayerData>();
		#endregion

		public static CTFPlayerData GetPlayerData(Mobile m)
		{
			CTFPlayerData pd = null;
			PlayerDictionary.TryGetValue(m, out pd);

			return pd;
		}

		#region Update
		public static void Update(CTFGameData gd)
		{
			if (GameDictionary.Count == 10)
				GameDictionary.Remove(9);

			for (int i = GameDictionary.Count; i > 0; i--)
			{
				CTFScoreData gsd = null;
				if (GameDictionary.TryGetValue(i - 1, out gsd))
					GameDictionary[i] = gsd;
			}
			CTFScoreData sd = new CTFScoreData(gd);
			GameDictionary[0] = sd;

			int maxDiff = 0;
			int maxPoints = 0;
			bool draw;
			CTFTeamGameData winner = CTFGame.GetWinningTeam(out draw);
			for (int i =0; i < gd.TeamList.Count; i++)
			{
				CTFTeamGameData tgd = (CTFTeamGameData)gd.TeamList[i];
				sd.TeamList.Add(new CTFTeamScoreData(tgd));
				int diff = tgd.Captures - tgd.FlagLosses;
				if (tgd == winner)
				{
					maxDiff = diff;
				}
				int teamReturns = 0;
				foreach (CTFPlayerGameData pgd in tgd.PlayerList)
					teamReturns += pgd.Returns;
				int teamPoints = ((diff * 10) - teamReturns + 250)/2;
				if (maxPoints < teamPoints)
					maxPoints = teamPoints;
			}

			if (maxPoints > 150)
				maxPoints = 150;

			double maxContribution = 0;
			foreach (CTFPlayerGameData pgd in winner.PlayerList)
			{
				double contribution = (pgd.Captures * 10 + pgd.Kills * 2 - pgd.Deaths * 0.5 + pgd.Returns * 4); //Edit by Silver, Old: *8 | *3 | *1 | *3
				if (maxContribution < contribution)
					maxContribution = contribution;
			}
			Dictionary<Mobile, int> oldranks = new Dictionary<Mobile, int>();
			for (int i =0; i < gd.TeamList.Count; i++)
			{
				CTFTeamGameData tgd = (CTFTeamGameData)gd.TeamList[i];
				foreach (CTFPlayerGameData pgd in tgd.PlayerList)
				{
					CTFPlayerData pd = GetPlayerData(pgd.Mob);
					if (pd != null)
					{
						oldranks[pd.Mob] = pd.Rank;
						if (!pgd.InGame)
						{
							int penalty = Math.Min(Banker.GetBalance(pd.Mob), 10000);
							if (penalty > 0 && Banker.Withdraw(pd.Mob, penalty))
								pd.Mob.SendMessage(CTFGame.HuePerson, String.Format("You have been charged {0} gp for abandoning the game.", penalty.ToString()));
							pgd.Score -= 200 - penalty / 100;
						}
						else
						{
							double contribution = (pgd.Captures * 10 + pgd.Kills * 2 - pgd.Deaths * 0.5 + pgd.Returns * 4); //Edit by Silver, Old: *8 | *3 | *1 | *3
							if (tgd == winner)
								pgd.Score = Math.Min(150, (int)(50 + (contribution * maxPoints) / maxContribution));
							else
								pgd.Score = (int)((Math.Min(contribution, maxContribution) * maxPoints) / (maxContribution * 1.5));
							if (pgd.Score < 0)
								pgd.Score = 0;
						}
						pd.Update(pgd);
						sd.PlayerList.Add(new CTFPlayerScoreData(pgd));
					}
				}
			}


			UpdateRanks();
			foreach (KeyValuePair<Mobile, int> pair in oldranks)
			{
				int newrank = GetRank(pair.Key);
				if(pair.Value != newrank)
					pair.Key.SendGump(new CTFRegradeGump(pair.Key, pair.Value, newrank));
			}
		}
		#endregion

		#region RankMethods
		public static void UpdateRanks()
		{
			PlayerList.Sort();
			for (int p = 0; p < PlayerList.Count; p++)
			{
				CTFPlayerData pd = PlayerList[p];
				int rank = 0;

				if (pd.Score < 800)
				{
					for (int i = CTFRankRewardSystem.CTFRankLevel.Length -1; i >= 0; i--)
					{
						if (CTFRankRewardSystem.CTFRankLevel[i] <= pd.Score)
						{
							rank = i;
							break;
						}
					}
				}

				else
				{
					for (int i = 0; i < CTFRankRewardSystem.CTFOfficerRankLevel.Length; i++)
					{
						if (CTFRankRewardSystem.CTFOfficerRankLevel[i] >= p)
						{
							rank = 20 - i;
							break;
						}
					}
				}

				pd.Rank = rank;
			}
		}

		public static int GetRank(Mobile m)
		{
			CTFPlayerData pd = GetPlayerData(m);

			if (pd == null)
				return 0;

			return pd.Rank;
		}
		#endregion

		#region Ser / Deser
		public static void Serialize(GenericWriter writer)
		{
			writer.Write(GameDictionary.Count);
			foreach (KeyValuePair<int, CTFScoreData> kvp in GameDictionary)
			{
				writer.Write(kvp.Key);
				kvp.Value.Serialize(writer);
			}

			writer.Write(PlayerList.Count);
			foreach (CTFPlayerData pd in PlayerList)
			{
				pd.Serialize(writer);
			}
		}

		public static void Deserialize(GenericReader reader)
		{
			int gamescount = reader.ReadInt();
			for (int i = 0; i < gamescount; i++)
			{
				int loc = reader.ReadInt();
				CTFScoreData sd = new CTFScoreData();
				sd.Deserialize(reader);

				GameDictionary[loc] = sd;
			}

			int playerscount = reader.ReadInt();
			for (int i = 0; i < playerscount; i++)
			{
				CTFPlayerData pd = new CTFPlayerData();
				pd.Deserialize(reader);

				if (pd.Mob != null)
				{
					PlayerDictionary[pd.Mob] = pd;
					PlayerList.Add(pd);
				}
			}

			PlayerList.Sort();
		}
		#endregion
	}

	#region scoredata classes
	public class CTFPlayerData : IComparable
	{
		#region Members
		public Mobile Mob;
		public int Kills;
		public int Deaths;
		public int Captures;
		public int Returns;
		public int Score;
		public int Rank;
		public DateTime LastEntry;
		public List<CTFPlayerScoreData> GameDataList = new List<CTFPlayerScoreData>();
		#endregion

		public CTFPlayerData() { }

		public CTFPlayerData(Mobile m)
		{
			Mob = m;
		}

		#region Update
		public void Update(CTFPlayerGameData pgd)
		{
			if (GameDataList.Count == 10)
			{
				CTFPlayerScoreData old = GameDataList[0];
				Kills -= old.Kills;
				Deaths -= old.Deaths;
				Captures -= old.Captures;
				Returns -= old.Returns;
				Score -= old.Score;
				GameDataList.Remove(old);
			}

			CTFPlayerScoreData psd = new CTFPlayerScoreData(pgd);
			GameDataList.Add(psd);

			Kills += psd.Kills;
			Deaths += psd.Deaths;
			Captures += psd.Captures;
			Returns += psd.Returns;
			Score += psd.Score;

			LastEntry = DateTime.Now;
		}
		#endregion

		#region Compare
		public int CompareTo(object obj)
		{
			if (obj is CTFPlayerData)
			{
				CTFPlayerData pd = (CTFPlayerData)obj;

				if (pd.Score - Score == 0)
					return pd.Captures - Captures;

				return pd.Score - Score;
			}
			else
				throw new ArgumentException("object is not DuelData");
		}
		#endregion

		#region Ser / Deser
		public void Serialize(GenericWriter writer)
		{
			writer.Write(0);//version
			writer.Write(LastEntry);
			writer.Write(Mob);
			writer.Write(Kills);
			writer.Write(Deaths);
			writer.Write(Captures);
			writer.Write(Returns);
			writer.Write(Score);
			writer.Write(Rank);

			writer.Write(GameDataList.Count);
			foreach (CTFPlayerScoreData psd in GameDataList)
				psd.Serialize(writer);
		}

		public void Deserialize(GenericReader reader)
		{
			int version = reader.ReadInt();
			LastEntry = reader.ReadDateTime();
			Mob = reader.ReadMobile();
			Kills = reader.ReadInt();
			Deaths = reader.ReadInt();
			Captures = reader.ReadInt();
			Returns = reader.ReadInt();
			Score = reader.ReadInt();
			Rank = reader.ReadInt();


			int datacount = reader.ReadInt();
			for (int i = 0; i < datacount; i++)
			{
				CTFPlayerScoreData psd = new CTFPlayerScoreData();
				psd.Deserialize(reader);

				GameDataList.Add(psd);
			}

			if (DateTime.Now - LastEntry > TimeSpan.FromDays(14))
			{
				LastEntry = DateTime.Now;
				int count = GameDataList.Count;

				if (count > 0)
				{
					CTFPlayerScoreData old = GameDataList[0];
					Kills -= old.Kills;
					Deaths -= old.Deaths;
					Captures -= old.Captures;
					Returns -= old.Returns;
					Score -= old.Score;
					GameDataList.Remove(old);
				}

				if (count == 1)
					Mob = null;
			}
		}
		#endregion
	}

	public class CTFScoreData
	{
		#region Members
		public int TotalCaptures;
		public DateTime Date;
		public List<CTFTeamScoreData> TeamList = new List<CTFTeamScoreData>();
		public List<CTFPlayerScoreData> PlayerList = new List<CTFPlayerScoreData>();
		#endregion

		public CTFScoreData() { }

		public CTFScoreData(CTFGameData gd)
		{
			TotalCaptures = gd.TotalCaptures;
			Date = DateTime.Now;
		}

		#region Ser / Deser
		public void Serialize(GenericWriter writer)
		{
			writer.Write(TotalCaptures);
			writer.Write(Date);

			writer.Write(TeamList.Count);
			foreach (CTFTeamScoreData team in TeamList)
				team.Serialize(writer);

			writer.Write(PlayerList.Count);
			foreach (CTFPlayerScoreData psd in PlayerList)
				psd.Serialize(writer);
		}

		public void Deserialize(GenericReader reader)
		{
			TotalCaptures = reader.ReadInt();
			Date = reader.ReadDateTime();

			int teamcount = reader.ReadInt();
			for (int i = 0; i < teamcount; i++)
			{
				CTFTeamScoreData tsd = new CTFTeamScoreData();
				tsd.Deserialize(reader);
				TeamList.Add(tsd);
			}

			int playerscount = reader.ReadInt();
			for (int i = 0; i < playerscount; i++)
			{
				CTFPlayerScoreData psd = new CTFPlayerScoreData();
				psd.Deserialize(reader);
				PlayerList.Add(psd);
			}
		}
		#endregion
	}

	public class CTFTeamScoreData
	{
		#region Members
		public CTFTeam Team;
		public int Captures;
		public int Losses;
		#endregion

		public CTFTeamScoreData() { }

		public CTFTeamScoreData(CTFTeamGameData tgd)
		{
			Team = tgd.Team;
			Captures = tgd.Captures;
			Losses = tgd.FlagLosses;
		}

		#region Ser / Deser
		public void Serialize(GenericWriter writer)
		{
			writer.Write(Team.Number);
			writer.Write(Captures);
			writer.Write(Losses);
		}

		public void Deserialize(GenericReader reader)
		{
			int index  = reader.ReadInt();
			Team = CTFGame.TeamArray[index];
			Captures = reader.ReadInt();
			Losses = reader.ReadInt();
		}
		#endregion
	}

	public class CTFPlayerScoreData : IComparable
	{
		#region Members
		public Mobile Mob;
		public CTFTeam Team;
		public int Kills;
		public int Deaths;
		public int Captures;
		public int Returns;
		public int Score;
		#endregion

		public CTFPlayerScoreData() { }

		public CTFPlayerScoreData(CTFPlayerGameData pgd)
		{
			Mob = pgd.Mob;
			Team = pgd.Team;
			Kills = pgd.Kills;
			Deaths = pgd.Deaths;
			Captures = pgd.Captures;
			Returns = pgd.Returns;
			Score = pgd.Score;
		}

		#region Ser / Deser
		public void Serialize(GenericWriter writer)
		{
			writer.Write(Mob);
			writer.Write(Team.Number);
			writer.Write(Kills);
			writer.Write(Deaths);
			writer.Write(Captures);
			writer.Write(Returns);
			writer.Write(Score);
		}

		public void Deserialize(GenericReader reader)
		{
			Mob = reader.ReadMobile();
			int index = reader.ReadInt();
			Team = CTFGame.TeamArray[index];
			Kills = reader.ReadInt();
			Deaths = reader.ReadInt();
			Captures = reader.ReadInt();
			Returns = reader.ReadInt();
			Score = reader.ReadInt();
		}
		#endregion

		#region Compare
		public int CompareTo(object obj)
		{
			if (obj is CTFPlayerScoreData)
			{
				CTFPlayerScoreData psd = (CTFPlayerScoreData)obj;

				if (psd.Score - Score == 0)
					return psd.Captures - Captures;

				return psd.Score - Score;
			}
			else
				throw new ArgumentException("object is not DuelData");
		}
		#endregion
	}
	#endregion

	#region gamedata classes
	public class CTFGameData
	{
		public int TotalCaptures;
		public List<CTFTeamGameData> TeamList = new List<CTFTeamGameData>();
		public List<CTFPlayerGameData> PlayerList = new List<CTFPlayerGameData>();
		public Dictionary<Mobile, CTFPlayerGameData> PlayerDictionary = new Dictionary<Mobile, CTFPlayerGameData>();

		public CTFGameData()
		{
		}

		public CTFTeamGameData GetTeamData(CTFPlayerGameData pgd)
		{
			foreach (CTFTeamGameData tgd in TeamList)
				if (tgd.Team == pgd.Team)
					return tgd;

			return null;
		}

		public CTFPlayerGameData GetPlayerData(Mobile m)
		{
			CTFPlayerGameData pd = null;
			if(PlayerDictionary.TryGetValue(m, out pd)&& !pd.InGame)
				pd = null;

			return pd;
		}

		public bool IsInGame(Mobile m)
		{
			return GetPlayerData(m) != null;
		}

		public CTFTeam GetPlayerTeam(Mobile m)
		{
			CTFPlayerGameData pd = null;
			if (PlayerDictionary.TryGetValue(m, out pd) && !pd.InGame)
					pd = null;

			if (pd == null)
				return null;

			return pd.Team;
		}
	}

	public class CTFTeamGameData
	{
		public CTFTeam Team;
		public int Captures;
		public int FlagLosses;
		public List<CTFPlayerGameData> PlayerList = new List<CTFPlayerGameData>();
		public int TeamCount { get { return PlayerList.Count; } }

		public CTFTeamGameData(CTFTeam team)
		{
			Team = team;
		}
	}

	public class CTFPlayerGameData
	{
		public Mobile Mob;
		public int GameID;
		public CTFTeam Team;
		public int Kills;
		public int Deaths;
		public int Captures;
		public int Returns;
		public int Score;
		public int RewardsChosen;
		public bool InGame;

		public CTFPlayerGameData(Mobile m, CTFTeam team)
		{
			Mob = m;
			Team = team;
			InGame = true;
		}
	}
	#endregion
}