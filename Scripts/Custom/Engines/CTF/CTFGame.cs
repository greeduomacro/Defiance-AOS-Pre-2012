using Server;
using System;
using Server.Items;
using Server.Commands;
using System.Collections;
using System.Collections.Generic;
using Server.Gumps;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;
using Server.Regions;
using Server.Engines.RewardSystem;

namespace Server.Events.CTF
{
	public static class CTFGame
	{
		public static void Configure()
		{
			CustomSaving.AddSaveModule(new SaveData(new DC.SaveMethod(CTFData.Serialize), new DC.LoadMethod(CTFData.Deserialize)), "CTFScore");
		}
		#region Members
		public static int HueGlobal = 0x489;
		public static int HuePerson = 0x4F2;
		public static int CaptureRobeHue = 0x35;
		private static int m_MinSupplySkill = 40;

		public static TimeSpan IdleTime = TimeSpan.FromMinutes( 4.0 );

		public static CTFTeam[] TeamArray = new CTFTeam[] { new CTFBlackTeam(), new CTFBlueTeam(), new CTFRedTeam(), new CTFWhiteTeam() };
		public static CTFGameStone Stone;
		public static List<Mobile> PlayerJoinList = new List<Mobile>();
		public static CTFFlag[] FlagArray = new CTFFlag[4];
		public static double RewardItemChance;
		public static bool GiveRobe, Open, Running, MsgStaff, GiveRewards;
		public static int Teams, Price, PrizeMoney;
		public static TimeSpan CurLength, DrawLength, GameLength;
		public static DateTime StartTime, NextIdleCheck;
		public static Timer GameTimer, StartTimer;
		public static CTFSpawn Spawn;

		public static Rectangle2D GameArea;
		public static CTFGameRegion Region;
		public static CTFGameData GameData;
		public static Item[] walls = new Item[4];
		public static TimeSpan TimeLeft
		{
			get
			{
				TimeSpan tsTime = CurLength - (DateTime.Now - StartTime);
				if (tsTime < TimeSpan.Zero)
					tsTime = TimeSpan.Zero;
				return tsTime;
			}
		}
		#endregion

		public static void StartGame(CTFGameStone stone)
		{
			Running = true;
			Open = true;
			Stone = stone;
			Price = stone.Price;
			RewardItemChance = stone.RewardItemChance;
			GiveRobe = stone.GiveRobe;
			MsgStaff = stone.MessageStaff;
			GiveRewards = stone.GiveRewards;
			Teams = stone.Teams;
			DrawLength = stone.DrawLength;
			GameLength = stone.GameLength;
			GameArea = stone.GameArea;
			Spawn = stone.Spawn;
			FlagArray = stone.FlagArray;
			GameData = new CTFGameData();

			if (Region != null)
				Region.Unregister();

			Region = new CTFGameRegion();
			Region.Register();

			EventSystem.Start(stone.MinutesOpen,"Capture The Flag Game", new EDC.StartMethod(StartGame), new EDC.StopMethod(EndGame), new EDC.JoinMethod(JoinMethod));
		}

		public static void JoinMethod(Mobile m)
		{
			if(m.Player)
				m.SendGump(new CTFJoinGump((PlayerMobile)m));
		}

		public static void StartGame()
		{
			Open = false;
			StartGame(GameLength, false);
		}

		public static void EndGame()
		{
			EndGame(false);
		}

		/// <summary>
		/// This is the function that starts the game.
		/// </summary>
		/// <param name="lenght">Lenght of the game.</param>
		/// <param name="draw">If this is a draw game.</param>
		public static void StartGame(TimeSpan lenght, bool draw)
		{
			if( draw )
			{
				CTFMessage( string.Format( "A Draw! Game will continue for {0}", MiscUtility.FormatTimeSpan( lenght ) ) );
			}
			else
			{
				if (Spawn != null)
					Spawn.StartTimer();

				Teams = Stone.Teams;
				if (Teams == 0) {
					int number = PlayerJoinList.Count;
					if ( number < 20 ) // Silver edit, old: (number < 16) || (number == 18)
						Teams = 2;
					else
						Teams = 4;
				}

				Item wall;
				if (Teams == 3) {
					wall = new Item(578);
					wall.Hue = 1150;
					wall.Movable = false;
					wall.MoveToWorld(new Point3D(5943, 451, 22), Map.Felucca);
					walls[0] = wall;
					wall = new Item(578);
					wall.Hue = 1150;
					wall.Movable = false;
					wall.MoveToWorld(new Point3D(5943, 452, 22), Map.Felucca);
					walls[1] = wall;
				} else if (Teams == 2) {
					wall = new Item(578);
					wall.Hue = 1150;
					wall.Movable = false;
					wall.MoveToWorld(new Point3D(5943, 451, 22), Map.Felucca);
					walls[0] = wall;
					wall = new Item(578);
					wall.Hue = 1150;
					wall.Movable = false;
					wall.MoveToWorld(new Point3D(5943, 452, 22), Map.Felucca);
					walls[1] = wall;
					wall = new Item(577);
					wall.Hue = 33;
					wall.Movable = false;
					wall.MoveToWorld(new Point3D(5955, 455, 22), Map.Felucca);
					walls[2] = wall;
					wall = new Item(577);
					wall.Hue = 33;
					wall.Movable = false;
					wall.MoveToWorld(new Point3D(5956, 455, 22), Map.Felucca);
					walls[3] = wall;
				}

				for (int i = 0; i < Teams; i++)
					GameData.TeamList.Add(new CTFTeamGameData(TeamArray[i]));

				List<CTFPlayerData> playerlist = new List<CTFPlayerData>();

				foreach (Mobile m in PlayerJoinList)
				{
					CTFPlayerData pd = CTFData.GetPlayerData(m);

					if (pd != null)
						playerlist.Add(pd);

					else
					{
						pd = new CTFPlayerData(m);
						CTFData.PlayerDictionary[m] = pd;
						CTFData.PlayerList.Add(pd);
						playerlist.Add(pd);
					}
				}

				playerlist.Sort();

				// Selection System by Silver

				int idx = 0;
				int max = playerlist.Count;

				if( Stone.CTFLeagueGame )
				{
					int CTFLeagueTeam = -1;

					while( idx < max )
					{
						CTFLeagueTeam = -1;

						CTFPlayerData pd = playerlist[idx];

						if( pd.Mob.SolidHueOverride == 1109 )
							CTFLeagueTeam = 0;
						else if( pd.Mob.SolidHueOverride == 1266 )
							CTFLeagueTeam = 1;
						else
							LeaveGame( pd.Mob );


						if( CTFLeagueTeam != -1 )
						{
							CTFPlayerGameData pgd = new CTFPlayerGameData(pd.Mob, TeamArray[CTFLeagueTeam]);
							GameData.PlayerDictionary[pd.Mob] = pgd;
							GameData.PlayerList.Add(pgd);
							GameData.TeamList[CTFLeagueTeam].PlayerList.Add(pgd);
							ArmPlayer(pgd);
							pd.Mob.SendGump(new CTFRewardGump(pd.Mob, 0));
						}

						pd.Mob.SolidHueOverride = -1;
						idx++;
					}
				}
				else
				{
					int startTeam = Utility.Random(Teams);
					bool revert = false;

					while( idx < max )
					{
						for( int team = 0; team < Teams && idx < max; team++ )
						{
							CTFPlayerData pd = playerlist[idx];
							CTFPlayerGameData pgd = new CTFPlayerGameData(pd.Mob, TeamArray[((revert ? (Teams - team - 1) : team) + startTeam)%Teams]);
							GameData.PlayerDictionary[pd.Mob] = pgd;
							GameData.PlayerList.Add(pgd);
							GameData.TeamList[((revert ? (Teams - team - 1) : team) + startTeam)%Teams].PlayerList.Add(pgd);
							ArmPlayer(pgd);
							pd.Mob.SendGump(new CTFRewardGump(pd.Mob, 0));

							idx++;
						}
						revert = !revert;
					}
				}


				PlayerJoinList.Clear();
				ResetTeams();
				CTFMessage( "The game has started." );
			}

			// Set idle check 4 min after game starts
			NextIdleCheck = DateTime.Now + CTFGame.IdleTime;

			StartTime = DateTime.Now;
			CurLength = lenght;

			GameTimer = new CtfGameTimer(lenght, TimeSpan.FromMinutes(1.0));
			GameTimer.Start();
		}

		/// <summary>
		/// This is the function gives weaponry to players.
		/// </summary>
		private static void ArmPlayer(CTFPlayerGameData pgd)
		{
			Mobile m = pgd.Mob;

			if (!m.Alive)
				m.Resurrect();

			for (int i = m.Items.Count - 1; i >= 0; --i)
			{
				Item item = (Item)m.Items[i];
				if (item.Layer == Layer.OuterTorso)
				{
					item.Delete();
					break;
				}
			}

			List<Item> armthis = new List<Item>();

			if(GiveRobe)
				armthis.Add(new CTFRobe(pgd.Team));

			Item rankedCloth = null;
			CTFTeam team = pgd.Team;

			// 21 Ranks
			switch (CTFData.GetRank(m))
			{
				default:
				case 0: rankedCloth = new JesterHat(team.Hue); break;
				case 1: rankedCloth = new TallStrawHat(team.Hue); break;
				case 2: rankedCloth = new FloppyHat(team.Hue); break;
				case 3: rankedCloth = new WideBrimHat(team.Hue); break;
				case 4: rankedCloth = new Cap(team.Hue); break;
				case 5: rankedCloth = new SkullCap(team.Hue); break;
				case 6: rankedCloth = new FlowerGarland(team.Hue); break;
				case 7: rankedCloth = new StrawHat(team.Hue); break;
				case 8: rankedCloth = new FeatheredHat(team.Hue); break;
				case 9: rankedCloth = new TricorneHat(team.Hue); break;
				case 10: rankedCloth = new TribalMask(team.Hue); break;
				case 11: rankedCloth = new HornedTribalMask(team.Hue); break;
				case 12: rankedCloth = new BearMask(team.Hue); break;
				case 13: rankedCloth = new DeerMask(team.Hue); break;
				case 14: rankedCloth = new OrcishKinMask(team.Hue); break;
				case 15: rankedCloth = new SavageMask(team.Hue); break;
				case 16: rankedCloth = new WizardsHat(team.Hue); break;
				case 17: rankedCloth = new Bandana(team.Hue); break;
				case 18: rankedCloth = new ClothNinjaHood(team.Hue); break;
				case 19: rankedCloth = new Kasa(team.Hue); break;
				case 20:
					rankedCloth = new BoneHelm();
					rankedCloth.Hue = team.Hue;
					((BaseArmor)rankedCloth).ArmorAttributes.MageArmor = 1;
					break;
			}
			if (rankedCloth != null)
			{
				rankedCloth.Movable = false;
				armthis.Add(rankedCloth);
			}

			//Alchemy removed by Blady
/* 			if (m.Skills[SkillName.Alchemy].Value >= (m_MinSupplySkill + 15)) //80 Alchemy req - by Blady
			{
				for (int i = 0; i < 6; i++) //Amount reduced to 6 by Blady - used to be 10
				{
					armthis.Add(new ExplosionPotion());
					armthis.Add(new GreaterHealPotion());
					armthis.Add(new GreaterCurePotion());
					armthis.Add(new GreaterAgilityPotion());
					armthis.Add(new RefreshPotion());
					armthis.Add(new GreaterStrengthPotion());
				}
			} */

			if (m.Skills[SkillName.Chivalry].Value >= m_MinSupplySkill)
			{
				BookOfChivalry book = new BookOfChivalry();
				book.Content = 1023;//all spells
				armthis.Add(book);
			}

			if (m.Skills[SkillName.Necromancy].Value >= m_MinSupplySkill)
			{
				NecromancerSpellbook book = new NecromancerSpellbook();
				book.Content = 0x1FFFF;
				armthis.Add(book);
			}

			if (m.Skills[SkillName.Magery].Value >= m_MinSupplySkill)
			{
				GnarledStaff gs = new GnarledStaff();
				gs.Attributes.SpellChanneling = 1;
				gs.WeaponAttributes.MageWeapon = 20;
				armthis.Add(gs);

				Spellbook book = new Spellbook();
				book.Content = ulong.MaxValue;
				armthis.Add(book);
			}

			if (m.Skills[SkillName.Healing].Value >= m_MinSupplySkill)
				armthis.Add(new Bandage(1000));

			if (m.Skills[SkillName.Fencing].Value >= m_MinSupplySkill)
			{
				Spear sp = new Spear();
				sp.Attributes.SpellChanneling = 1;
				armthis.Add(sp);

				ShortSpear ssp = new ShortSpear();
				ssp.Attributes.SpellChanneling = 1;
				armthis.Add(ssp);

				WarFork wf = new WarFork();
				wf.Attributes.SpellChanneling = 1;
				armthis.Add(wf);

				Kryss k = new Kryss();
				k.Attributes.SpellChanneling = 1;
				armthis.Add(k);
			}

			if (m.Skills[SkillName.Swords].Value >= m_MinSupplySkill)
			{
				if (m.Skills[SkillName.Lumberjacking].Value >= m_MinSupplySkill)
				{
					ExecutionersAxe ea = new ExecutionersAxe();
					ea.Attributes.SpellChanneling = 1;
					armthis.Add(ea);
				}

				Katana k = new Katana();
				k.Attributes.SpellChanneling = 1;
				armthis.Add(k);

				Longsword ls = new Longsword();
				ls.Attributes.SpellChanneling = 1;
				armthis.Add(ls);

				Cleaver c = new Cleaver();
				c.Attributes.SpellChanneling = 1;
				armthis.Add(c);

				BoneHarvester bh = new BoneHarvester();
				bh.Attributes.SpellChanneling = 1;
				armthis.Add(bh);
			}

			if (m.Skills[SkillName.Macing].Value >= m_MinSupplySkill)
			{
				WarAxe wa = new WarAxe();
				wa.Attributes.SpellChanneling = 1;
				armthis.Add(wa);

				HammerPick hp = new HammerPick();
				hp.Attributes.SpellChanneling = 1;
				armthis.Add(hp);

				QuarterStaff qs = new QuarterStaff();
				qs.Attributes.SpellChanneling = 1;
				armthis.Add(qs);
			}

			if (m.Skills[SkillName.Archery].Value >= m_MinSupplySkill)
			{
				Bow b = new Bow();
				b.Attributes.SpellChanneling = 1;
				armthis.Add(b);

				Crossbow xb = new Crossbow();
				xb.Attributes.SpellChanneling = 1;
				armthis.Add(xb);

				CompositeBow cb = new CompositeBow();
				cb.Attributes.SpellChanneling = 1;
				armthis.Add(cb);

				armthis.Add(new Arrow(150));
				armthis.Add(new Bolt(150));
			}

			if (m.Skills[SkillName.Poisoning].Value >= m_MinSupplySkill)
			{
				for (int i = 0; i < 2; i++)
					armthis.Add(new GreaterPoisonPotion());
			}

			if (m.Skills[SkillName.Parry].Value >= m_MinSupplySkill)
			{
				MetalKiteShield ks = new MetalKiteShield();
				ks.Attributes.SpellChanneling = 1;
				armthis.Add(ks);
			}

			SunnySystem.ArmPlayer(m, armthis);
		}

		private static void ReArmPlayer(Mobile m)
		{
			List<Item> torearm = new List<Item>();

			//Alchemy rmeoved by Blady
/* 			if (m.Skills[SkillName.Alchemy].Value >= m_MinSupplySkill)
			{
				AddItemsByType(m, typeof(ExplosionPotion), torearm);
				AddItemsByType(m, typeof(GreaterHealPotion), torearm);
				AddItemsByType(m, typeof(GreaterCurePotion), torearm);
				AddItemsByType(m, typeof(GreaterAgilityPotion), torearm);
				AddItemsByType(m, typeof(RefreshPotion), torearm);
				AddItemsByType(m, typeof(GreaterStrengthPotion), torearm);
			} */

			if (m.Skills[SkillName.Poisoning].Value >= m_MinSupplySkill)
				AddItemsByType(m, typeof(GreaterPoisonPotion), torearm);

			if (m.Skills[SkillName.Archery].Value >= m_MinSupplySkill)
			{
				AddProjectiles(m, typeof(Arrow), torearm);
				AddProjectiles(m, typeof(Bolt), torearm);
			}

			SunnySystem.ReArmPlayer(m, torearm);
		}

		private static void AddProjectiles(Mobile m, Type type, List<Item> list)
		{
			Item[] itemarray = m.Backpack.FindItemsByType(type);

			int count = 0;

			foreach (Item item in itemarray)
				count += item.Amount;

			int toadd = 150 - count;

			Item addeditem = (Item)Activator.CreateInstance(type);
			addeditem.Amount = toadd;

			list.Add(addeditem);
		}

		private static void AddItemsByType(Mobile m, Type type, List<Item> list)
		{
			Item[] itemarray = m.Backpack.FindItemsByType(type);

			int toadd = 2 - itemarray.Length;

			if(toadd > 0)
			for(int i = toadd; i > 0; i--)
				list.Add((Item)Activator.CreateInstance(type));
		}

		/// <summary>
		/// Each team member in the teams will be moved to their homebases
		/// and be resurrected if needed. Their stats will also be set to max.
		/// </summary>
		public static void ResetTeams()
		{
			MoveAllFlagsHome();

			foreach (CTFPlayerGameData pgd in GameData.PlayerList)
					ResurrectPlayer(pgd);
		}

		/// <summary>
		/// Will remove the player from the game.
		/// </summary>
		/// <param name="m"></param>
		public static void LeaveGame(Mobile m)
		{
			if (!CTFGame.Running)
				return;

			CTFPlayerGameData pgd = GameData.GetPlayerData(m);

			if (pgd != null)
			{
				pgd.InGame = false;
				RemoveItems(m);
				SunnySystem.DisArmPlayer(m);

				m.Criminal = false;
				m.InvalidateProperties();
				m.Warmode = false;
				m.Aggressed.Clear();
				m.Aggressors.Clear();

				EventSystem.RemoveToRandomLoc(m);
				SunnySystem.ReDress(m);
				m.Delta(MobileDelta.Noto);
				m.InvalidateProperties();
			}
			else if (PlayerJoinList.Contains(m)) {
				try {
					PlayerJoinList.Remove(m);
					EventSystem.RemoveToRandomLoc(m);
					SunnySystem.ReDress(m);
				} catch {}
			} else
				EventSystem.RemoveToRandomLoc(m);
		}

		/// <summary>
		/// Sends a message to all players in the game.
		/// </summary>
		/// <param name="message"></param>
		public static void CTFMessage(string message)
		{
			string msg = string.Format("CTF Game: {0}", message);

			foreach (CTFPlayerGameData pgd in GameData.PlayerList)
				if(pgd.InGame)
					pgd.Mob.SendMessage(HueGlobal, msg);

			if (MsgStaff)
				CommandHandlers.BroadcastMessage(AccessLevel.GameMaster, HueGlobal, msg);
		}

		/// <summary>
		/// All flags will be moved to their homebase.
		/// </summary>
		public static void MoveAllFlagsHome()
		{
			foreach (CTFFlag flag in FlagArray)
			{
				if (flag != null)
					flag.ReturnToHome();
			}
		}

		/// <summary>
		/// This will find the winning team.
		/// </summary>
		/// <param name="draw">If its a draw then this variable will be true.</param>
		public static CTFTeamGameData GetWinningTeam(out bool draw)
		{
			CTFTeamGameData Winner = null;
			draw = false;

			foreach (CTFTeamGameData tgd in GameData.TeamList)
			{
				if (Winner == null)
				{
					Winner = tgd;
					continue;
				}

				else
				{
					int winnerscore = (Winner.Captures - Winner.FlagLosses);
					int tgdscore = (tgd.Captures - tgd.FlagLosses);

					if (winnerscore == tgdscore)
					{
						if (tgd.Captures == Winner.Captures)
						{
							draw = true;

							int winKills = 0;
							int tgdKills = 0;
							foreach (CTFPlayerGameData playerData in Winner.PlayerList)
								winKills += playerData.Kills - playerData.Deaths;
							foreach (CTFPlayerGameData playerData in tgd.PlayerList)
								tgdKills += playerData.Kills - playerData.Deaths;
							if (tgdKills > winKills)
								Winner = tgd;
						}
						else if(tgd.Captures > Winner.Captures)
						{
							Winner = tgd;
							draw = false;
						}
					}
					else if (tgdscore > winnerscore)
					{
						Winner = tgd;
						draw = false;
					}
				}
			}

			return Winner;
		}

		/// <summary>
		/// The game will end here unless it is a draw.
		/// </summary>
		/// <param name="useDrawCheck"></param>

		public static void EndGame(bool useDrawCheck)
		{
			if (!Running)
				return;

			bool draw = false;
			CTFTeamGameData winner = GetWinningTeam(out draw);

			if (draw && useDrawCheck)
			{
				StartGame(DrawLength, true);
				return;
			}

			if (winner != null)
			{
				DateTime now = DateTime.Now;
				int prize = Math.Max(0, (int)(PrizeMoney / (winner.TeamCount + 1)));
				List<Mobile> moblist = new List<Mobile>();

				foreach (CTFPlayerGameData pgd in winner.PlayerList)
					if (pgd.InGame && pgd.Mob.BankBox != null && (pgd.Captures != 0 || pgd.Kills > 0 || pgd.Returns > 0))
						moblist.Add(pgd.Mob);

				GiveRewards = Stone.GiveRewards;
				if (GiveRewards)
				{
				// Edited by Silver
				// Reward Item Chance is now decided per individual, rather than all or nothing
				// People will still get copper bars when they get clothing
					RewardItemChance = Stone.RewardItemChance;
					foreach (Mobile m in moblist)
					{
						if (Utility.Random( 100 ) <= RewardItemChance )
						{
							Item[] item = {new Robe(),new Doublet(), new BodySash(), new Skirt(), new Sandals(), new Cloak(), new HalfApron()};
							Item Clothing = (item[Utility.Random(6)]);
							Clothing.Hue = winner.Team.Hue;
							Clothing.Name = String.Format("CTF Winner, {0}-{1}-{2}", now.Day, now.Month, now.Year);
							m.BankBox.DropItem(Clothing);
							m.SendMessage(HuePerson, "A clothing item has been placed into your bankbox.");
						}
					}

					foreach (Mobile m in moblist)
					{
						if( Teams == 4 )
						{
							EventRewardSystem.CreateCopperBar(m.Name, m.BankBox, 2, "CTF winner");
					   		m.SendMessage(HuePerson, "Two copper bars has been placed into your bankbox.");
						}
						else
						{
							EventRewardSystem.CreateCopperBar(m.Name, m.BankBox, 1, "CTF winner");
					   		m.SendMessage(HuePerson, "A copper bar has been placed into your bankbox.");
						}
					}
				}

				if (prize > 0)
					foreach (Mobile m in moblist)
					{
						// m.BankBox.DropItem(new BankCheck(prize));
						// m.SendLocalizedMessage(1042764, prize.ToString()); // A check worth ~1_AMOUNT~ in gold has been placed in your bank box.
						Banker.Deposit(m, prize);
						m.SendLocalizedMessage(1060397, prize.ToString()); // ~1_AMOUNT~ gold has been deposited into your bank box.
					}

				foreach (NetState ns in NetState.Instances)
				{
					Mobile m = ns.Mobile;

					if (m != null)
						m.SendMessage(0x482, String.Format("The {0} have won this Capture The Flag Game, with {1} captures and {2} flag losses.", winner.Team.Name, winner.Captures, winner.FlagLosses));
				}
			}
			if (Teams == 3) {
				walls[0].Delete();
				walls[1].Delete();
			} else if (Teams == 2) {
				walls[0].Delete();
				walls[1].Delete();
				walls[2].Delete();
				walls[3].Delete();
			}
			if(!Stone.NoScore)
				CTFData.Update(GameData);

			RemoveAllPlayers();
			StopTimers();
			MoveAllFlagsHome();
			if (PlayerJoinList.Count != 0)
				foreach (Mobile m in PlayerJoinList)
					SunnySystem.ReDress(m);

			Running = false;
			PrizeMoney = 0;
		}

		/// <summary>
		/// Removes all players from the game.
		/// </summary>
		private static void RemoveAllPlayers()
		{
			foreach (CTFPlayerGameData pgd in GameData.PlayerList)
			{
				if (pgd.InGame)
				{
					LeaveGame(pgd.Mob);

					if( !Stone.CTFLeagueGame )
						pgd.Mob.SendGump(new CTFGameDataGump(CTFData.GameDictionary[0]));
				}
			}
		}

		/// <summary>
		/// Will return a flag if it finds one on the player mobile.
		/// </summary>
		/// <param name="m"></param>
		/// <returns></returns>
		public static CTFFlag GetFlag(Mobile m)
		{
			foreach (CTFFlag flag in FlagArray)
				if (flag != null && flag.RootParent == m)
					return flag;

			return null;
		}

		public static CTFFlag GetTeamFlag(CTFTeam team)
		{
			return FlagArray[team.Number];
		}

		/// <summary>
		/// Ressurects player and moves him to his base.
		/// </summary>
		public static void ResurrectPlayer(CTFPlayerGameData pgd)
		{
			if (pgd != null && pgd.InGame)
			{
				Mobile m = pgd.Mob;

				CTFFlag flag = CTFGame.GetTeamFlag(pgd.Team);
				if (flag != null)
				{
					m.Resurrect();
					m.Hits = m.HitsMax;
					m.Mana = m.ManaMax;
					m.Stam = m.StamMax;
					ReArmPlayer(m);

					m.LogoutLocation = flag.FlagHome;
					m.MoveToWorld(flag.PlayerSpawn, flag.FlagHomeMap);
				}
			}
		}

		/// <summary>
		/// Removes items on the player.
		/// </summary>
		/// <param name="from"></param>
		public static void RemoveItems( Mobile from )
		{
			if ( from.Holding != null )
				from.Holding.Delete();

			if ( from.Backpack != null )
			{
				for ( int i = from.Backpack.Items.Count - 1; i >= 0; --i )
				{
					Item item = (Item)from.Backpack.Items[i];

					if( item is CTFFlag )
					{
						((CTFFlag)item).ReturnToHome();
						continue;
					}

					item.Delete();
				}
			}

			for ( int i = from.Items.Count - 1; i >= 0; --i )
			{
				Item item = (Item)from.Items[i];
				if( item.Layer == Layer.Hair ||
					item.Layer == Layer.FacialHair ||
					item.Layer == Layer.Backpack ||
					item.Layer == Layer.Bank )
					continue;

				item.Delete();
			}
		}

		public enum CTFScoreType
		{
			Kills,
			Deaths,
			Captures,
			Returns
		}

		/// <summary>
		/// Add scores for the given player to all needed places, including Top Score.
		/// </summary>
		/// <param name="player"></param>
		/// <param name="type"></param>
		public static void AddScore(Mobile from, CTFScoreType type)
		{
			if (!CTFGame.Running)
				return;

			CTFPlayerGameData data = GameData.GetPlayerData(from);
			CTFTeamGameData tgd = GameData.GetTeamData(data);

			if (data != null && tgd != null)
			{
				switch (type)
				{
					case CTFScoreType.Captures:
						GameData.TotalCaptures++;
						tgd.Captures++;
						data.Captures++;
						break;
					case CTFScoreType.Deaths:
						data.Deaths++;
						break;
					case CTFScoreType.Kills:
						data.Kills++;
						break;
					case CTFScoreType.Returns:
						data.Returns++;
						break;
				}
			}
		}

		/// <summary>
		/// Stops the timers.
		/// </summary>
		private static void StopTimers()
		{
			if( GameTimer != null )
			{
				GameTimer.Stop();
				GameTimer = null;
			}

			if (StartTimer != null)
			{
				StartTimer.Stop();
				StartTimer = null;
			}

			CurLength = GameLength;
		}

		#region CtfGameTimer
		private class CtfGameTimer : Timer
		{
			private DateTime m_dtGameEnd;
			private DateTime m_dtMessageTime;
			private TimeSpan m_MessageInterval;

			public CtfGameTimer( TimeSpan gameLenght, TimeSpan messageInterval ) : base( TimeSpan.FromSeconds( 1.0 ), TimeSpan.FromSeconds( 1.0 ) )
			{
				Priority = TimerPriority.OneSecond;
				m_dtGameEnd = DateTime.Now + gameLenght;
				m_dtMessageTime = DateTime.Now + messageInterval;
				m_MessageInterval = messageInterval;
			}

			protected override void OnTick()
			{
				#region Idle Check
				if (NextIdleCheck < DateTime.Now)
				{
					NextIdleCheck = DateTime.Now + CTFGame.IdleTime;

					foreach (CTFPlayerGameData pgd in GameData.PlayerList)
					{
						if (pgd.InGame)
						{
							Mobile m = pgd.Mob;
							if (m.LastMoveTime + CTFGame.IdleTime < DateTime.Now)
							{
								m.SendMessage(HuePerson, "You have been idle for too long... you have been removed from the ctf game.");
								LeaveGame(m);
							}
						}
					}
				}
				#endregion

				if (m_dtGameEnd <= DateTime.Now)
				{
					EventSystem.Stop();
					this.Stop();
				}
				else if (m_dtMessageTime <= DateTime.Now)
				{
					CTFMessage(string.Format("Time Left {0}", MiscUtility.FormatTimeSpan(TimeLeft)));
					m_dtMessageTime = DateTime.Now + m_MessageInterval;
				}
			}
		}
		#endregion
	}
}