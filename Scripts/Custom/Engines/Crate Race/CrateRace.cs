using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Regions;
using Server.Network;
using Server.Gumps;
using Server.Multis;
using Server.Items;
using Server.Engines.RewardSystem;

namespace Server.Events.CrateRace
{
	public static class CrateRace
	{
		#region Members
		private static int m_DeathMoves = 20;

		public static Packet Walk = SpeedPacket.Walk;
		public static Packet SpeedUp = SpeedPacket.Run;
		public static Packet Reset = SpeedPacket.Disabled;
		public static string Name = "Crate Race Event";

		public static CrateRegion m_Region;
		public static List<RaceData> PartList = new List<RaceData>();
		public static Dictionary<Mobile, RaceData> PartData = new Dictionary<Mobile, RaceData>();
		public static List<RaceCrate> CrateList = new List<RaceCrate>();
		public static int Crates { get { return CrateList.Count; } }
		public static List<CrateApple> AppleList = new List<CrateApple>();
		public static List<FinishedData> FinData = new List<FinishedData>();
		public static List<CrateRectangle> Rectangles = new List<CrateRectangle>();
		public static List<RaceFinishLine> FinishLines = new List<RaceFinishLine>();
		public static int Participants { get { return CrateRace.PartData.Count; } }
		public static int OpenCrates, BankedMoney, MaxCrates, MinutesOpen,Price, Laps, X, Y, Z;
		public static Point3D StoneLocation;
		public static Item Track;
		public static Timer FreezeTimer;
		public static Map Map;
		public static bool Running, Animalised, QuickSpeed, Rewards;
		public static Mobile FirstPlace;

		private static double[] m_RectangleChance;
		private static DateTime m_TimeStarted;
		private static int  m_StartTime, m_TimerCounter;
		private static Timer StartTimer, RaceTimer, StopTimer;
		#endregion

		#region Supportive Methods

		public static bool Participant(Mobile m)
		{
			if (PartData.ContainsKey(m))
				return true;

			return false;
		}

		public static int GetRectangleID(int x, int y)
		{
			for (int i = 0; 0 < Rectangles.Count; i++)
			{
				if (RectangleContains(Rectangles[i].Rectangle,x,y))
					return i;
			}

			return -1;
		}

		public static bool IsFirstRectangle(CrateRectangle rect)
		{
			return rect == Rectangles[0];
		}

		private static bool RectangleContains(Rectangle2D rect, int x, int y)
		{
			return (rect.Start.X <= x && rect.Start.Y <= y && rect.End.X >= x && rect.End.Y >= y);
		}

		public static CrateRectangle GetRectangle(int x, int y)
		{
			foreach (CrateRectangle rect in Rectangles)
				if (RectangleContains(rect.Rectangle,x, y))
					return rect;

			return null;
		}

		private static void SMTP(string str) //Send Message To Participants
		{
			foreach (Mobile m in PartData.Keys)
				m.SendMessage(str);
		}

		public static int GetHighestZ(int x, int y)
		{
			int highestz = Map.Tiles.GetLandTile(x,y).Z;
			IPooledEnumerable pe = Map.GetItemsInBounds(new Rectangle2D(x,y, 1, 1));

			foreach (Item item in pe)
				highestz = Math.Max(highestz, item.GetSurfaceTop().Z);

			pe.Free();

			return highestz;
		}

		public static void SetPlayerSpeed(Mobile m)
		{
			RaceData rd = null;

			if (PartData.TryGetValue(m, out rd))
				SetPlayerSpeed(rd);
		}

		public static void SetPlayerSpeed(RaceData rd)
		{
			if (QuickSpeed)
			{
				if (rd.Frozen)
					rd.Part.Send(Reset);
				else
					rd.Part.Send(SpeedUp);
			}

			else
			{
				if (rd.Frozen)
					rd.Part.Send(Walk);

				else
					rd.Part.Send(Reset);
			}
		}

		public static void ResetPlayerSpeed(Mobile m)
		{
			if (QuickSpeed)
				m.Send(Reset);

			else
				m.Send(SpeedUp);
		}

		public static void RequestJoin(Mobile m)
		{
			if (m.Player)
			{
				m.CloseGump(typeof(CrateJoinGump));
				m.SendGump(new CrateJoinGump());
			}
		}
		#endregion

		#region Setup Race
		public static void Start(CrateStone stone)
		{
			m_StartTime = 5;
			Map = stone.Map;
			X = stone.X;
			Y = stone.Y;
			Z = stone.Z;
			Rewards = stone.Rewards;
			StoneLocation = stone.Location;
			Animalised = stone.Animalised;
			QuickSpeed = stone.QuickSpeed;
			MaxCrates = stone.MaxCrates;
			Price = stone.Price;
			Laps = stone.Laps;
			MinutesOpen = stone.MinutesOpen;
			Rectangles = stone.Rectangles;
			m_Region = new CrateRegion(stone.RegionRect, Map);
			m_Region.Register();
			Running = true;

			EventSystem.Start(MinutesOpen, Name, new EDC.StartMethod(StartNewRace), new EDC.StopMethod(StopRace), new EDC.JoinMethod(RequestJoin));
		}

		private static void StartNewRace()
		{
			if (Rectangles.Count < 4 || PartData.Count == 0)
				EventSystem.Stop();

			else
			{
				SetRectangleChances();

				foreach (RaceData rd in PartList)
				{
					rd.Part.CloseGump(typeof(CrateRaceCharGump));
					rd.Part.CloseGump(typeof(CrateRacePlacer));
					rd.Part.CloseGump(typeof(CrateRacePlacerAccept));

					if (rd.Xaxe == 0)
					{
						rd.Xaxe = 100;
						rd.Yaxe = 450;
					}
				}

				if (Animalised)
					foreach (Mobile m in PartData.Keys)
					{
						if (m.BodyMod == 401 || m.BodyMod == 400)
							m.BodyMod = 0xD0;
					}

				m_TimeStarted = DateTime.Now;

				int y = 0;
				int x = 1;

				foreach (Mobile m in PartData.Keys)
				{
					m.Frozen = true;
					m.Location = new Point3D(X - x, Y - 22 - y, Z);
					y++;
					if (y % 8 == 0)
					{
						y = 0;
						x++;
					}
				}
				m_StartTime = 3;
				StartTimer = Timer.DelayCall(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1), new TimerCallback(StartRace));
			}
		}

		private static void SetRectangleChances()
		{
			int count = Rectangles.Count;
			int totalsquaretiles = 0;
			int[] rectsquaretiles = new int[count];

			for (int i = 0; i < count; i++)
			{
				CrateRectangle rect = Rectangles[i];
				int square = rect.Rectangle.Height * rect.Rectangle.Width;
				totalsquaretiles += square;
				rectsquaretiles[i] = square;
			}
			double cumulative = 0.0;
			m_RectangleChance = new double[count];
			for (int i = 0; i < count; i++)
			{
				cumulative += (double)rectsquaretiles[i] / totalsquaretiles;
				m_RectangleChance[i] = cumulative;
			}
		}
		#endregion

		#region RaceEvents
		private static void StartRace()
		{
			m_StartTime--;
			foreach (Mobile m in PartData.Keys)
			{
				if (m_StartTime == 0)
				{
					m.Frozen = false;
					m.SendMessage("The race has started!");
				}

				else
					m.SendMessage(String.Format("{0} seconds untill the race begins!", m_StartTime));
			}

			if (m_StartTime == 0)
			{
				StartTimer.Stop();
				RaceTimer = Timer.DelayCall(TimeSpan.FromSeconds(0), TimeSpan.FromMilliseconds(500), new TimerCallback(OnRaceTimer));
			}
			if (FinData != null)
				FinData.Clear();
		}

		private static void OnRaceTimer()
		{
			m_TimerCounter++;

			CrateDrop();
			AppleDrop();

			if (m_TimerCounter % 2 == 0)
			{
				foreach (RaceData rd in PartList)
				{
					Mobile m = rd.Part;

					if (rd.SwitchGump)
					{
						m.SendGump(new CrateRaceGump2(rd));
						m.CloseGump(typeof(CrateRaceGump1));
						rd.SwitchGump = false;
					}

					else
					{
						m.SendGump(new CrateRaceGump1(rd));
						m.CloseGump(typeof(CrateRaceGump2));
						rd.SwitchGump = true;
					}
				}
			}

			if (m_TimerCounter == 4)
			{
				m_TimerCounter = 0;
				CheckPlace();
			}
		}

		private static Rectangle2D GetRandomRectangle()
		{
			double random = Utility.RandomDouble();
			for (int i = 0; i < m_RectangleChance.Length; i++)
				if (random <= m_RectangleChance[i])
					return Rectangles[i].Rectangle;

			return new Rectangle2D(); // Never happens
		}

		private static Point3D RandomDropLocation()
		{
			Rectangle2D rect = GetRandomRectangle();

			int max = rect.End.X;
			int mix = rect.Start.X;
			int may = rect.End.Y;
			int miy = rect.Start.Y;

			int x = Utility.RandomMinMax(mix, max);
			int y = Utility.RandomMinMax(miy, may);

			return new Point3D(x, y, GetHighestZ(x,y));
		}

		private static void CrateDrop()
		{
			if (Crates < MaxCrates)
			{
				RaceCrate crate = new RaceCrate();
				Point3D loc = RandomDropLocation();
				crate.MoveToWorld(loc, Map);

				Effects.PlaySound(loc, Map, 0x202);
				Effects.SendLocationEffect(loc, Map, 0x376A, 30, crate.Hue, 3);
				Effects.SendLocationEffect(loc, Map, 0x3779, 30, crate.Hue, 3);

				CrateList.Add(crate);
			}
		}

		private static void AppleDrop()
		{
			if (AppleList.Count < 15)
			{
				CrateApple apple = new CrateApple();
				apple.MoveToWorld(RandomDropLocation(), Map);
				AppleList.Add(apple);
			}
		}

		public static void CheckPlace()
		{
			PartList.Sort();

			int rdi = 1;
			int fdi = (FinData == null) ? 0 : FinData.Count;

			foreach (RaceData rd in PartList)
			{
				if (rdi == 1)
					FirstPlace = rd.Part;

				rd.Place = rdi + fdi;
				rdi++;
			}
		}

		public static void OnDeath(Mobile m)
		{
			bool finished = false;
			bool finishcheck = false;

			int x = m.Location.X;
			int y = m.Location.Y;
			int movements = m_DeathMoves;

			CrateRectangle currentrect = GetRectangle(x, y);
			bool firstdir = true;

			while (!finished && currentrect != null)
			{
				if (NewLocation(currentrect, firstdir, ref x, ref y, ref movements))
					finished = true;

				else
				{
					if (currentrect == Rectangles[0])
						finishcheck = true;

					firstdir = !firstdir;

					if (firstdir)
						currentrect = GetRectangle(x, y);
				}
			}

			if (currentrect != null)
			{
				if (IsFirstRectangle(currentrect))
				{
					foreach (Item item in FinishLines)
					{
						if (item.X == x && item.Y == y)
						{
							switch ((int)currentrect.FirstDirection)
							{
								case (int)Direction.North: y++; break;
								case (int)Direction.East: x--; break;
								case (int)Direction.South: y--; break;
								case (int)Direction.West: x++; break;
							}
							break;
						}
					}
				}

				else if (finishcheck)
				{
					RaceData rd = null;

					if (PartData.TryGetValue(m, out rd))
						rd.Lap--;
				}

				m.MoveToWorld(new Point3D(x, y, GetHighestZ(x,y)), Map);
			}
		}

		private static bool NewLocation(CrateRectangle rect, bool first, ref int x, ref int y, ref int movementpoints)
		{
			int changecoord = 0;
			bool vertical = false;
			bool positive = false;

			switch (first ? (int)rect.FirstDirection : (int)rect.SecondDirection)
			{
				case (int)Direction.North:
					changecoord = rect.Rectangle.Start.Y;
					vertical = true;
					break;

				case (int)Direction.East:
					changecoord = rect.Rectangle.End.X;
					positive = true;
					break;

				case (int)Direction.South:
					changecoord = rect.Rectangle.End.Y;
					positive = true;
					vertical = true;
					break;

				case (int)Direction.West:
					changecoord = rect.Rectangle.Start.X;
					break;
			}

			int tochange = vertical ? y : x;
			int difference = 0;
			bool completed = false;

			if (positive && (tochange + movementpoints) <= changecoord)
			{
					tochange = tochange + movementpoints;
					completed = true;
			}

			else if ((tochange - movementpoints) >= changecoord)
			{
					tochange = tochange - movementpoints;
					completed = true;
			}

			if(!completed)
				difference = changecoord - tochange + (first ? 0 : (positive ?1 : -1));

			tochange = tochange + difference;

			if (vertical)
				y = tochange;
			else
				x = tochange;

			movementpoints = movementpoints - Math.Abs(difference);

			return completed;
		}

		#endregion

		#region CrateEvent
		public static void HandleDamage(object o)
		{
			object[] array = (object[])o;
			HandleDamage((RaceData)array[0], (double)array[1]);
		}

		public static void HandleDamage(RaceData rd, double amount)
		{
			if (rd.TempMirror)
			{
				rd.Part.SendMessage("But your temporary shield has saved you from any harm done.");
				rd.TempMirror = false;
			}
			else if (rd.Mirror)
			{
				rd.Part.SendMessage("But your shield has saved you from any harm done.");
				rd.Mirror = false;
			}

			else
			{
				int dam = (int)(rd.Part.HitsMax * amount);
				int damage = (int)(dam - (dam * ((OpenCrates + (Crates / 2)) / 100)));
				rd.Part.Damage(damage);
			}
		}

		public static void StopFreeze()
		{
			for (int i = 0; i < PartList.Count; i++)
			{
				RaceData rd = PartList[i];

				if (rd.Frozen)
				{
					rd.Frozen = false;
					SetPlayerSpeed(rd);
					rd.Part.SendMessage("The effect of a freezecrate has worn off!");
				}
			}
		}
		#endregion

		#region mobfinish
		public static bool MobFinish(Mobile m)
		{
			RaceData rd = null;

			if (PartData.TryGetValue(m, out rd))
			{
				rd.Lap++;

				if (rd.Lap != (Laps + 1))
					return true;

				else
				{
					int place = FinData.Count + 1;
					FinData.Add(new FinishedData(m, place, DateTime.Now - m_TimeStarted));

					switch (place)
					{
						case 1: StopTimer = Timer.DelayCall(TimeSpan.FromMinutes(3), new TimerCallback(EventSystem.Stop)); m.Location = new Point3D(X, Y - 15, Z + 23); break;
						case 2: m.Location = new Point3D(X, Y - 17, Z + 20); break;
						case 3: m.Location = new Point3D(X, Y - 13, Z + 20); break;
						default: m.Location = new Point3D(X + 3, Y - 17, Z); break;
					}

					PartData.Remove(m);
					PartList.Remove(rd);

					if (PartData.Count == 0)
						EventSystem.Stop();
					return false;
				}
			}
			return false;
		}
		#endregion

		#region End
		private static void StopRace()
		{
			SMTP("The race has stopped!");
			if (StopTimer != null)
				StopTimer.Stop();
			if (RaceTimer != null)
				RaceTimer.Stop();

			if (m_Region != null)
			{
				m_Region.Unregister();
				m_Region = null;
			}

			while (CrateList.Count > 0)
				CrateList[0].Delete();

			while (AppleList.Count > 0)
				AppleList[0].Delete();

			if (Participants > 0)
			{
				while (PartList.Count > 0)
				{
					RaceData rd = PartList[0];
					Mobile mob = rd.Part;

					PartList.Remove(PartList[0]);
					PartData.Remove(mob);
					RemovePlayer(mob);
				}
			}

			if (FinData != null)
			{
				foreach (FinishedData fd in FinData)
				{
					Mobile mob = fd.m_FinPart;

					string str;
					int money = 0;
					switch (fd.m_Place)
					{
						default: str = "Thank you for participating in this Crate Race, unfortunately you have not won anything. Good luck next time!"; break;
						case 1: money = (int)BankedMoney / 3; str = "You have won this Crate Race, Congratulations!"; break;
						case 2: money = (int)BankedMoney / 6; str = "You have managed to become second in this Crate Race, Congratulations!"; break;
						case 3: money = (int)BankedMoney / 9; str = "You have managed to become third in this Crate Race, Congratulations!"; break;
					}

					if (Rewards)
					{
						int amount = 0;
						switch (fd.m_Place)
						{
							case 1: amount = 3; break;
							case 2: amount = 2; break;
							case 3: amount = 1;  break;
						}

						if(amount > 0)
							EventRewardSystem.CreateCopperBar(mob.Name, mob.BankBox, amount, "Crate Race Event");
					}

					mob.SendMessage(str);
					if (money > 0)
						mob.AddToBackpack(new BankCheck(money));

					RemovePlayer(mob);
					mob.SendGump(new RaceRankGump(FinData, 0));
				}
			}

			OpenCrates = 0;
			BankedMoney = 0;
			PartData.Clear();
			Running = false;
		}

		private static void RemovePlayer(Mobile m)
		{
			m.CloseGump(typeof(CrateRaceGump1));
			m.CloseGump(typeof(CrateRaceGump2));
			m.Send( SpeedControl.Disable );
			m.BodyMod = 0;
			SunnySystem.ReDress(m);
			EventSystem.RemoveToRandomLoc(m);
		}
		#endregion
	}
}