//////////////////////////////\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
//			   This is a Sunny Production ©2005					\\
//					 Based on RunUO©							\\
//					Version: Alpha 1.0							\\
//		Many thanks to my Csharp tutors Will and Eclipse		\\
//////////////////////////////\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
//conver to int not working

using System;
using System.Collections;
using System.Collections.Generic;
using Server.Network;
using Server.Mobiles;
using Server.Targeting;
using Server.Gumps;
using Server.Items;

namespace Server.Events.CrateRace
{
	public class CrateRaceCharGump : Gump
	{
		private class CharEntry
		{
			public int m_GumpImage;
			public int m_BodyVal;

			public CharEntry(int GumpImage, int bodyval)
			{
				m_GumpImage = GumpImage;
				m_BodyVal = bodyval;
			}
		}

		public CrateRaceCharGump() : base(0, 0)
		{
			AddBackground(137, 130, 324, 299, 9200);
			AddBackground(152, 143, 182, 28, 9200);
			AddLabel(166, 146, 0, "Choose your character");
			AddButton(190, 385, 241, 242, 0, GumpButtonType.Reply, 0);
			AddButton(320, 385, 247, 248, 1, GumpButtonType.Reply, 0);

			for (int i = 0; i < 8; i++)
			{
				if (i < 4)
				{
					AddRadio(160, 200 + (i * 50), 210, 211, false, i);
					AddItem(190, 200 + (i * 50), m_CharEnt[i].m_GumpImage);
				}

				else
				{
					AddRadio(300, 200 + ((i - 4) * 50), 210, 211, false, i);
					AddItem(330, 200 + ((i - 4) * 50), m_CharEnt[i].m_GumpImage);
				}
			}
		}

		private CharEntry[] m_CharEnt = new CharEntry[]
		{
				new CharEntry( 8401, 0xD0 ),//Chicken
				new CharEntry( 8423, 50 ),//Skeleton
				new CharEntry( 8426, 0xE1 ),//Wolf
				new CharEntry( 8473, 0xD6 ),//Panther
				new CharEntry( 8437, 0x1D ),//Gorilla
				new CharEntry( 8416, 0x11 ),//Orc
				new CharEntry( 8424, 0x33 ),//Slime
				new CharEntry( 8417, 0xD5 ),//Polar Bear
				new CharEntry( 8458, 31 )//headlessone
		};

		public override void OnResponse(NetState sender, RelayInfo info)
		{
			Mobile from = sender.Mobile;

			switch (info.ButtonID)
			{
				case 0: break;
				case 1:
					if (info.Switches.Length > 0)
						from.BodyMod = m_CharEnt[info.Switches[0]].m_BodyVal;

					break;
			}
		}
	}

	public class CrateRacePlacer : Gump
	{
		private RaceData m_Rd;


		public CrateRacePlacer(RaceData data): base(0, 0)
		{
			m_Rd = data;

			Closable = true;
			Disposable = true;
			Dragable = false;
			Resizable = false;

			for (int i = 0; i < 17; i++)
				for (int j = 0; j < 11; j++)
				{
					AddBackground(i * 60, j * 60, 60, 60, 2620);
					AddButton(i * 60 + 15, j * 60 + 15, 2641, 2641, (int)(i * 100 + j), GumpButtonType.Reply, 0);
				}
		}

		public override void OnResponse(NetState sender, RelayInfo info)
		{
			int place = info.ButtonID;
			int xmult = (int)(place / 100);
			int ymult = (int)(place - (xmult * 100));

			int xaxe = xmult * 60;
			int yaxe = ymult * 60;

			sender.Mobile.SendGump(new CrateRacePlacerAccept(xaxe, yaxe, m_Rd));
		}
	}

	public class CrateRacePlacerAccept : AdvGump
	{
		private RaceData m_Rd;

		private int Xaxis;
		private int Yaxis;

		public CrateRacePlacerAccept(int x, int y, RaceData data) : base(x, y)
		{
			m_Rd = data;
			Xaxis = x;
			Yaxis = y;

			Closable = true;
			Disposable = true;
			Dragable = false;
			Resizable = false;

			AddBackground(0, 0, 480, 290, 2620);
			AddHtml(90, 30, 300, 100, Colorize("If this is the correct place where you want your racegump to pop up, please click okay. If you wish to choose a different location please click cancel.", "ffffff"), false, false);
			AddButton(120, 200, 242, 241, 0, GumpButtonType.Reply, 0);
			AddButton(260, 200, 248, 247, 1, GumpButtonType.Reply, 0);
		}

		public override void OnResponse(NetState sender, RelayInfo info)
		{
			switch (info.ButtonID)
			{
				case 0:
					sender.Mobile.SendGump(new CrateRacePlacer(m_Rd));
					break;

				case 1:
					m_Rd.Xaxe = Xaxis;
					m_Rd.Yaxe = Yaxis;
					if (CrateRace.Animalised)
						sender.Mobile.SendGump(new CrateRaceCharGump());
					break;
			}
		}
	}


	public class CrateRaceGump2 : CrateRaceGump1
	{
		public CrateRaceGump2(RaceData data) : base( data)
		{
		}

		public override void OnResponse(NetState sender, RelayInfo info)
		{
			base.OnResponse(sender, info);
		}
	}

	public class CrateRaceGump1 : AdvGump
	{
		public RaceData m_Rd;

		public CrateRaceGump1(RaceData data) : base( data.Xaxe, data.Yaxe )
		{
			m_Rd = data;

			Closable=false;
			Disposable=false;
			Dragable=false;
			Resizable=false;

			AddPage(0);
			AddImage(60, 18, 5058, 2105);
			AddImage(188, 18, 5058, 2105);
			AddImage(316, 18, 5058, 2105);
			AddImage(59, 146, 5058, 2105);
			AddImage(187, 145, 5058, 2105);
			AddImage(315, 145, 5058, 2105);
			AddImage(412, 142, 5058, 2105);
			AddImage(415, 18, 5058, 2105);// lback
			AddImage(143, 62, 2440, 2105);// titelback
			AddLabel(173, 63, 1152, "Crate Race Gump"); //title
			AddAlphaRegion(80, 50, 54, 54);//crate reg
			AddAlphaRegion(80, 120, 54, 54);//crate reg
			AddAlphaRegion(80, 190, 54, 54);//crate reg
			if (m_Rd.Crate1 != null)
				AddButton(85, 55, m_Rd.Crate1.GumpID, m_Rd.Crate1.GumpID, (int)Buttons.Crate1, GumpButtonType.Reply, 0);//crate1
			if (m_Rd.Crate2 != null)
				AddButton(85, 125, m_Rd.Crate2.GumpID, m_Rd.Crate2.GumpID, (int)Buttons.Crate2, GumpButtonType.Reply, 0);//crate2
			if (m_Rd.Crate3 != null)
				AddButton(85, 195, m_Rd.Crate3.GumpID, m_Rd.Crate3.GumpID, (int)Buttons.Crate3, GumpButtonType.Reply, 0);//crate3
			AddAlphaRegion(350, 50, 54, 54);//crate shield reg
			AddAlphaRegion(420, 50, 54, 54);//crate shield reg
			if (m_Rd.TempMirror)
				AddImage(355, 55, 2275);//crate tempshield
			if (m_Rd.Mirror)
				AddImage(425, 55, 2265);//crate shield

			//deco
			AddImage(530, 15, 10431, 2105);//rightupbody
			AddImage(511, 0, 10430, 2105);//righthead
			AddImage(12, 15, 10421, 2105);//leftupbody
			AddImage(35, 00, 10420, 2105);//lefthead
			AddImage(26, 135, 10422, 2105);///leftdownbody
			AddImage(524, 135, 10432, 2105);//rightdownbody
			//firebreath

			for (int i = 95; i <= 500; i += 16 )
				AddImage(i, 13, 10250, 2105);

			//bottomtiles
			AddImage(116, 269, 9267, 2105);
			AddImage(50, 269, 9267, 2105);
			AddImage(534, 269, 9268, 2105);
			AddImage(47, 269, 9266, 2105);
			//deco

			int hits = (int)(100 * m_Rd.Part.Hits / m_Rd.Part.HitsMax);
			int hitshue = 73;
			if (hits < 30)
				hitshue = 38;

			else if (hits < 50)
				hitshue = 55;

			AddAlphaRegion(140, 119, 383, 117);//large txt reg
			AddLabel(155, 130, hitshue, "Health:"); AddLabel(235, 130, hitshue, String.Format("{0}%", hits.ToString()));
			AddLabel(155, 150, 1152, "Lap:"); AddLabel(235, 150, 1152, m_Rd.Lap.ToString());
			AddLabel(155, 170, 1152, "Laps to go:"); AddLabel(235, 170, 1152, (CrateRace.Laps - m_Rd.Lap).ToString());
			AddLabel(155, 190, 1152, "Rank:"); AddLabel(235, 190, 1152, m_Rd.Place.ToString());
			AddLabel(295, 150, 1152, "Crates:"); AddLabel(375, 150, 1152, CrateRace.Crates.ToString());
			AddLabel(295, 170, 1152, "Opponents:"); AddLabel(375, 170, 1152, (CrateRace.Participants - 1).ToString());
			AddLabel(295, 190, 1152, "First Place:"); AddLabel(375, 190, 1152, CrateRace.FirstPlace == null ? "Noone" : CrateRace.FirstPlace.Name);

		}

		public enum Buttons
		{
			Cancel,
			Crate1,
			Crate2,
			Crate3
		}

		public override void OnResponse(NetState sender, RelayInfo info)
		{
			Mobile from = sender.Mobile;

			if (CrateRace.Running)
				return;


			RaceData rd = null;

			if (CrateRace.PartData.TryGetValue(from, out rd))
			{
				switch (info.ButtonID)
				{
					case 1: rd.Crate1.OnUsage(from); rd.Crate1 = null; break;
					case 2: rd.Crate2.OnUsage(from); rd.Crate2 = null; break;
					case 3: rd.Crate3.OnUsage(from); rd.Crate3 = null; break;
				}
			}

		}
	}

	public class RaceRankGump : AdvGump
	{
		private List<FinishedData> m_List;
		private int m_Page;

		public RaceRankGump(List<FinishedData> list, int page) : base()
		{
			m_List = list;
			m_Page = page;

			Closable = true;
			Disposable = true;
			Dragable = true;
			Resizable = false;

			AddBackground(156, 102, 294, 344, 9300);
			AddBackground(191, 113, 225, 23, 9300);
			AddHtml(191, 118, 225, 23, Center("Crate Race Rankings"), false, false);
			AddButton(152, 418, 52, 52, 0, GumpButtonType.Reply, 0);
			AddButton(169, 378, 9772, 9770, 1, GumpButtonType.Reply, 0);
			AddButton(408, 378, 9773, 9771, 2, GumpButtonType.Reply, 0);

			int maxpages = (int)(list.Count / 10);
			if (page < 0)
				m_Page = 0;
			if (page > maxpages)
				m_Page = maxpages;
			int listcount = list.Count;
			int start = m_Page * 10;
			int max = start + 10;

			if (listcount < start + 10)
			{
				if (listcount < 10)
					max = listcount;

				else
					max = listcount - 10;
			}

			List<string> array = new List<string>();

			array.Add("Place:");
			array.Add("Name:");
			array.Add("Time:");

			for (int j = start; j < max; j++)
			{
				FinishedData fd = list[j];
				array.Add(fd.m_Place.ToString());
				if (fd.m_FinPart != null)
					array.Add(fd.m_FinPart.Name);
				else
					array.Add("Anonymous");
				array.Add(String.Format("{0}:{1}", fd.m_Time.Minutes.ToString(), fd.m_Time.Seconds.ToString()));
			}

			AddHtml(156, 380, 294, 20, Center(String.Format("{0}/{1}", m_Page + 1, maxpages + 1)), false, false);
			AddTable(170, 150, new int[3] { 40, 150, 60 }, array, new string[3] { "333333", "333333", "333333" }, 1, 1);
		}

		public override void OnResponse(NetState sender, RelayInfo info)
		{
			Mobile from = sender.Mobile;

			switch (info.ButtonID)
			{
				case 1: from.SendGump(new RaceRankGump(m_List, m_Page - 1)); break;
				case 2: from.SendGump(new RaceRankGump(m_List, m_Page + 1)); break;
			}
		}
	}

	public class CrateJoinGump : Gump
	{
		public CrateJoinGump() : base(30, 30)
		{
			Closable = true;
			Disposable = true;
			Dragable = true;
			Resizable = false;
			AddPage(0);

			AddBackground(0, 0, 500, 270, 9300);
			AddBackground(150, 9, 200, 30, 9300);
			AddLabel(196, 15, 0, "Crate Race Gump");
			AddBackground(25, 45, 450, 180, 9300);
			AddHtml(30, 50, 440, 170, "Welcome,<br><br>You have entered the moongate that leads to a crate race event. It will cost you 5000gp to join. If you are a donator it will be free for you to join.<br><br>If you want to gather some information about this event first, please look here:<br><ahref=\"n-mediaservers1.nl/~stefan/DFI/Craterace/craterace.php\">Website link</a>.", false, false);
			AddButton(290, 230, 247, 248, 1, GumpButtonType.Reply, 0);
			AddButton(150, 230, 242, 241, 0, GumpButtonType.Reply, 0);
			AddImage(0, 241, 52);
			AddImage(110, 15, 59);
			AddImage(360, 14, 57);
		}

		public override void OnResponse(NetState sender, RelayInfo info)
		{
			Mobile from = sender.Mobile;

			switch (info.ButtonID)
			{
				case 0: break;
				case 1:
					if (from is PlayerMobile && !CrateRace.Participant(from))
					{
						PlayerMobile pm = (PlayerMobile)from;

						if (pm.HasDonated || CrateRace.Price < 1 || (CrateRace.Price > 0 && Banker.Withdraw(pm, CrateRace.Price)))
						{
							SunnySystem.Undress(pm);

							if (!pm.HasDonated && CrateRace.Price > 0)
								CrateRace.BankedMoney += CrateRace.Price;

							RaceData data = new RaceData(pm);
							CrateRace.PartData[pm] = data;
							CrateRace.PartList.Add(data);
							pm.SendGump(new CrateRacePlacer(data));
							CrateRace.SetPlayerSpeed(pm);

							pm.MoveToWorld(CrateRace.StoneLocation, CrateRace.Map);
							pm.SendMessage("You have entered the race!");
						}

						else
							from.SendMessage("You do not have enough money to participate.");
					}

					else
						from.SendMessage("The race has already started.");

					break;
			}
		}
	}
}