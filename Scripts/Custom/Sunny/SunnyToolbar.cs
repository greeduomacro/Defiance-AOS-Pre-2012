using System;
using System.Collections.Generic;
using Server.Network;
using Server.Gumps;
using Server.Mobiles;
using Server.Commands;
using Server.Targeting;

namespace Server.SunnyToolbar
{
	public enum Booleans
	{
		Tb,
		SpeedBoost,
		Pages,
		Who,
		NoPresetCommands,
		ShowLocations
	}

	public class SunnyToolbar
	{
		public static Dictionary<Mobile, ToolbarEntry> EntryCollection = new Dictionary<Mobile, ToolbarEntry>();
		public static string[] BaseCommands = new string[]
			{
				"GRP",//couns
				"Pages",
				"M Tele",
				"SpeedBoost",
				"Props", //4
				"Who", //GM
				"Wipe",
				"Move",
				"Go", //8
				"Admin",//admins
			};

		public static void Configure()
		{
			EventSink.Login += new LoginEventHandler(Login_Event);
			CustomSaving.AddSaveModule(new SaveData(new DC.SaveMethod(Serialize), new DC.LoadMethod(Deserialize)), "SunnyToolbar");
			CommandSystem.Register("TB", AccessLevel.Counselor, new CommandEventHandler(Toolbar_OnCommand));
			CommandSystem.Register("TBS", AccessLevel.Counselor, new CommandEventHandler(ToolbarSettings_OnCommand));
		}

		private static void Login_Event(LoginEventArgs e)
		{
			Mobile m = e.Mobile;
			Timer.DelayCall(TimeSpan.FromSeconds(2.0), new TimerStateCallback(Login_Callback), m);
		}

		private static void Login_Callback(object o)
		{
			Mobile m = (Mobile)o;
			ToolbarEntry entry = null;

			if (EntryCollection.TryGetValue(m, out entry))
			{
				for (int i = 0; i < 4; i++)
					if (entry.BooleanOptions[i])
						CommandSystem.Handle(m, ((Booleans)i).ToString(), MessageType.Command);
			}
		}

		private static void ToolbarSettings_OnCommand(CommandEventArgs e)
		{
			Mobile m = e.Mobile;
			ToolbarEntry entry = null;

			if (!EntryCollection.TryGetValue(m, out entry))
				entry = new ToolbarEntry(m);

			m.CloseGump(typeof(ToolBarSettingsGump));
			m.SendGump(new ToolBarSettingsGump(entry));
		}

		private static void Toolbar_OnCommand(CommandEventArgs e)
		{
			Mobile m = e.Mobile;
			ToolbarEntry entry = null;

			if (!EntryCollection.TryGetValue(m, out entry))
				entry = new ToolbarEntry(m);

			m.CloseGump(typeof(ToolbarGump));
			m.SendGump(new ToolbarGump(entry, m));
		}

		private static void Serialize(GenericWriter writer)
		{
			writer.Write(0);//version

			writer.Write(EntryCollection.Count);
			foreach (KeyValuePair<Mobile, ToolbarEntry> kvp in EntryCollection)
			{
				kvp.Value.Serialize(writer);
			}
		}

		private static void Deserialize(GenericReader reader)
		{
			int version = reader.ReadInt();

			int count =  reader.ReadInt();
			for (int i = 0; i < count; i++ )
			{
				new ToolbarEntry(reader);
			}
		}
	}

	public struct LocationStruct
	{
		public Map Map;
		public Point3D Location;
		public string Name;

		public LocationStruct(Map map, Point3D loc)
		{
			Map = map;
			Location = loc;
			Name = "";
		}

		public void Serialize(GenericWriter writer)
		{
			writer.Write(0);//version
			writer.Write(Map);
			writer.Write(Location);
			writer.Write(Name);
		}

		public LocationStruct(GenericReader reader)
		{
			int version = reader.ReadInt();
			Map = reader.ReadMap();
			Location = reader.ReadPoint3D();
			Name = reader.ReadString();
		}
	}

	public class ToolbarEntry
	{
		public int ButtonHeight, ButtonWidth, X_Loc, Y_Loc;
		public bool[] BooleanOptions = new bool[7];//LoadOnLogin, SpeedBoostOnLogin, PagesOnLogin, WhoOnLogin, presetcommands,ShowLocations, LockedLocation;
		public LocationStruct[] Locations = new LocationStruct[10];
		public Mobile Mob;
		public List<string> CustomCommands = new List<string>();

		public ToolbarEntry(Mobile m)
		{
			Mob = m;
			ButtonHeight = 2;
			ButtonWidth = 5;
			X_Loc = 20;
			Y_Loc = 50;
			SunnyToolbar.EntryCollection[m] = this;
		}

		public void Serialize(GenericWriter writer)
		{
			writer.Write(1);//version
			writer.Write(X_Loc);
			writer.Write(Y_Loc);
			writer.Write(Mob);
			writer.Write(ButtonHeight);
			writer.Write(ButtonWidth);
			writer.Write(CustomCommands.Count);
			foreach (string str in CustomCommands)
				writer.Write(str);

			writer.Write(BooleanOptions.Length);
			foreach(bool b in BooleanOptions)
				writer.Write(b);

			foreach (LocationStruct ls in Locations)
				ls.Serialize(writer);
		}

		public ToolbarEntry(GenericReader reader)
		{
			int version = reader.ReadInt();

			switch (version)
			{
				case 1:
					X_Loc = reader.ReadInt();
					Y_Loc = reader.ReadInt();

					goto case 0;
				case 0:
					Mob = reader.ReadMobile();
					ButtonHeight = reader.ReadInt();
					ButtonWidth = reader.ReadInt();
					int count = reader.ReadInt();
					for (int i = 0; i < count; i++)
						CustomCommands.Add(reader.ReadString());

					int length = reader.ReadInt();
					for (int i = 0; i < length; i++)
						BooleanOptions[i] = reader.ReadBool();

					for (int i = 0; i < 10; i++)
						Locations[i] = new LocationStruct(reader);

					break;
			}
			if(Mob != null)
				SunnyToolbar.EntryCollection[Mob] = this;
		}
	}

	public class ToolbarGump : AdvGump
	{
		private ToolbarEntry m_Entry;
		private List<string> m_CommandList = new List<string>();
		private int m_MaxLevel;

		public ToolbarGump(ToolbarEntry entry, Mobile m)
			: base( entry.X_Loc, entry.Y_Loc)
		{
			PlayerMobile pm = (PlayerMobile)m;
			if(pm == null)
				return;

			m_Entry = entry;

			Closable = false;
			Disposable = true;
			Dragable = !entry.BooleanOptions[6];
			Resizable = false;

			if (entry.BooleanOptions[(int)Booleans.NoPresetCommands])
				m_MaxLevel = 0;
			else
				switch (pm.AccessLevel)
				{
					case AccessLevel.Administrator:
					case AccessLevel.Developer:
					case AccessLevel.Owner:
						m_MaxLevel = SunnyToolbar.BaseCommands.Length; break;

					case AccessLevel.Seer:
					case AccessLevel.GameMaster:
						m_MaxLevel = 8; break;

					case AccessLevel.Counselor:
						m_MaxLevel = 4; break;
				}

			AddPage(0);
			int button = 0;
			int xpos = 10;
			int ypos = 15;
			int height = entry.ButtonHeight * 40 + 20;
			int width = entry.ButtonWidth * 110 + 25;
			bool locs = entry.BooleanOptions[(int)Booleans.ShowLocations];
			int locsextra = 460;
			int pages = (int)Math.Ceiling((double)(m_MaxLevel + entry.CustomCommands.Count) / (entry.ButtonHeight * entry.ButtonWidth));

			AddBackground(0, locsextra, width, height, 3600);
			AddBackground(width, 33 + locsextra, 45, 45, 3600);
			AddBackground(width, locsextra, 45, 45, 3600);
			AddBackground(width + 45, 33 + locsextra, 45, 45, 3600);
			AddBackground(width + 45, locsextra, 45, 45, 3600);
			AddButton(width + 57, 16 + locsextra,  2118, 2117, 1000, GumpButtonType.Reply, 0);
			AddButton(width + 56, 51 + locsextra, locs ? 2223 : 2224, locs ? 2223 : 2224, 2000, GumpButtonType.Reply, 0);

			if (locs)
			{
				AddBackground(width+ 90, 0, 165, 460, 3600);
				for (int i = 0; i < 10; i++)
					GenerateButton(width + 100, 15 + i *40, entry.Locations[i].Name, i + 100, false);

				GenerateButton(width + 100, 415, "Edit Locs", 3000, false);
			}

			for (int page = 1; page < pages +1; page++)
			{
				AddPage(page);
				if (page < pages )
					AddButton(width + 12, 16 + locsextra, 55, 55, 0, GumpButtonType.Page, page + 1);
				if (page != 1)
					AddButton(width + 12, 49 + locsextra, 56, 56, 0, GumpButtonType.Page, page - 1);

				for (int i = 0; i < entry.ButtonHeight; i++)
				{
					xpos = 10;
					for (int j = 0; j < entry.ButtonWidth; j++)
					{
						int custombutt = button - m_MaxLevel;

						if (button < m_MaxLevel)
							GenerateButton(xpos, ypos + locsextra, SunnyToolbar.BaseCommands[button], button + 1, true);

						else if (custombutt < entry.CustomCommands.Count)
							GenerateButton(xpos, ypos + locsextra, entry.CustomCommands[custombutt], button + 1, true);

						button++;
						xpos += 110;
					}
					ypos += 40;
				}
				ypos = 15;
			}
		}

		private void GenerateButton(int x, int y, string str, int id, bool command)
		{
			if(command)
				m_CommandList.Add(str);
			AddButton(x + 13, y + 5, 5, 5, id, GumpButtonType.Reply, 0);
			AddBackground(x, y, command?110:140, 40, 9270);
			AddHtml(x,y + 10, command?110:140,20,Colorize(Center(str), "ffffff"),false, false);
		}

		public override void OnResponse(NetState sender, RelayInfo info)
		{
			Mobile m = sender.Mobile;
			int id =info.ButtonID;

			if (id == 0)
				return;

			else if (id == 1000)
			{
				m.SendGump(new ToolBarSettingsGump(m_Entry));
				return;
			}

			else if (id == 2000)
			{
				m_Entry.BooleanOptions[(int)Booleans.ShowLocations] = !m_Entry.BooleanOptions[(int)Booleans.ShowLocations];
				m.SendGump(new ToolbarGump(m_Entry, m));
				return;
			}

			else if (id == 3000)
			{
				m.SendGump(new ToolBarLocsGump(m_Entry));
				return;
			}

			else if (id >= 100 && id < 110)
			{
				LocationStruct ls = m_Entry.Locations[id - 100];
				if (ls.Location != Point3D.Zero)
					m.MoveToWorld(ls.Location, ls.Map);
			}

			m.SendGump(this);
			int loc = info.ButtonID - 1;
			int custloc = loc - m_MaxLevel;

			if (loc < m_CommandList.Count)
				CommandSystem.Handle(m, m_CommandList[loc], MessageType.Command);
		}
	}

	public class ToolBarSettingsGump : AdvGump
	{
		private ToolbarEntry m_Entry;

		public ToolBarSettingsGump(ToolbarEntry entry)
			: base()
		{
			m_Entry = entry;

			Closable = false;
			Disposable = true;
			Dragable = true;
			Resizable = false;

			AddPage(0);

			AddBackground(0, 0, 475, 525, 3600);
			AddBackground(162, 30, 150, 40, 9270);
			AddHtml(0,40,475,20,Colorize(Center("Toolbar Settings"), "ffffff"),false, false);

			AddBackground(235, 95, 225, 200, 9270);
			AddLabel(250, 75, 1152, "Login Options:");
			AddCheck(255, 115, 210, 211, entry.BooleanOptions[0], 0); AddLabel(285, 115, 1152, "Load Toolbar");
			AddCheck(255, 145, 210, 211, entry.BooleanOptions[2], 2); AddLabel(285, 145, 1152, "Page Command");
			AddCheck(255, 175, 210, 211, entry.BooleanOptions[3], 3); AddLabel(285, 175, 1152, "Who Command");
			AddCheck(255, 205, 210, 211, entry.BooleanOptions[1], 1); AddLabel(285, 205, 1152, "SpeedBoost Command");
			AddCheck(255, 235, 210, 211, entry.BooleanOptions[4], 4); AddLabel(285, 235, 1152, "No preset commands");

			AddBackground(235, 320, 225, 200, 9270);
			AddLabel(245, 300, 1152, "Options:");
			AddLabel(250, 335, 1152, "Command Collumns:");AddLabel(413, 335, 1152, entry.ButtonWidth.ToString());
			AddButton(390, 340, 9706, 9707, 300, GumpButtonType.Reply, 0);
			AddButton(425, 340, 9702, 9703, 301, GumpButtonType.Reply, 0);
			AddLabel(250, 360, 1152, "Command Rows:");AddLabel(411, 360, 1152, entry.ButtonHeight.ToString());
			AddButton(390, 365, 9706, 9707, 400, GumpButtonType.Reply, 0);
			AddButton(425, 365, 9702, 9703, 401, GumpButtonType.Reply, 0);
			AddCheck(250, 385, 210, 211, entry.BooleanOptions[6], 6); AddLabel(280, 385, 1152, "Locked Location");
			AddLabel(250, 410, 1152, "X-Loc:"); AddTextEntry(300, 410, 170, 18, 1152, 1000, entry.X_Loc.ToString());
			AddLabel(250, 435, 1152, "Y-Loc:"); AddTextEntry(300, 435, 170, 18, 1152, 1001, entry.Y_Loc.ToString());


			AddBackground(10, 95, 225, 200, 9270);
			AddLabel(25, 75, 1152, "Custom Commands:");

			AddButton(171, 484, 247, 248, 1, GumpButtonType.Reply, 0);
			AddButton(17, 485, 242, 241, 0, GumpButtonType.Reply, 0);

			for (int page = 1; page < 6; page++)
			{
				AddPage(page);

				for (int i = 0; i < 6; i++)
				{
					int loc = i + (page - 1) * 6;
					AddLabel(25, 115 + 25 * i, 1152, (loc + 1).ToString());

					AddTextEntry(45, 115 + 25 * i, 170, 18, 1152, loc, loc < entry.CustomCommands.Count ? entry.CustomCommands[loc] : "");
					AddImageTiled(45, 133 + 25 * i, 170, 2, 2700);
				}

				if (page < 5)
				{
					AddLabel(170, 295, 1152, "Next");
					AddButton(205, 300, 9762, 9763, 0, GumpButtonType.Page, page + 1);
				}
				if (page > 1)
				{
					AddLabel(45, 295, 1152, "Previous");
					AddButton(20, 300, 9766, 9767, 0, GumpButtonType.Page, page - 1);
				}
			}
		}

		public override void OnResponse(NetState sender, RelayInfo info)
		{
			Mobile m = sender.Mobile;

			switch (info.ButtonID)
			{
				case 1:
					List<string> commlist = m_Entry.CustomCommands;
					commlist.Clear();
					for (int i = 0; i < 30; i++)
					{
						string str = info.GetTextEntry(i).Text;

						if (str != "")
							commlist.Add(str);
					}

					if (!Int32.TryParse(info.GetTextEntry(1000).Text, out m_Entry.X_Loc))
						m.SendMessage("The x-location was not formatted correctly.");

					if (!Int32.TryParse(info.GetTextEntry(1001).Text, out m_Entry.Y_Loc))
						m.SendMessage("The y-location was not formatted correctly.");

					for (int i = 0; i < 5; i++)
						m_Entry.BooleanOptions[i] = info.IsSwitched(i);

					m_Entry.BooleanOptions[6] = info.IsSwitched(6);

					m.SendGump(new ToolbarGump(m_Entry, m));
					break;
				case 300:
					if (m_Entry.ButtonWidth > 1)
						m_Entry.ButtonWidth--;
					m.SendGump(new ToolBarSettingsGump(m_Entry));
					break;
				case 301:
					if (m_Entry.ButtonWidth < 20)
						m_Entry.ButtonWidth++;
					m.SendGump(new ToolBarSettingsGump(m_Entry));
					break;
				case 400:
					if (m_Entry.ButtonHeight > 1)
						m_Entry.ButtonHeight--;
					m.SendGump(new ToolBarSettingsGump(m_Entry));
					break;
				case 401:
					if (m_Entry.ButtonHeight < 20)
						m_Entry.ButtonHeight++;
					m.SendGump(new ToolBarSettingsGump(m_Entry));
					break;
			}
		}
	}

	public class ToolBarLocsGump : AdvGump
	{
		private ToolbarEntry m_Entry;

		public ToolBarLocsGump(ToolbarEntry entry)
			: base()
		{
			m_Entry = entry;

			Closable = false;
			Disposable = true;
			Dragable = true;
			Resizable = false;

			AddPage(0);
			AddBackground(0, 0, 241, 460, 3600);
			for (int i = 0; i < 10; i++)
			{
				AddBackground(10, 15 + i * 40, 140, 40, 9270);
				AddButton(150, 23 + i * 40, 4014, 4015, i, GumpButtonType.Reply, 0);
				AddButton(185, 23 + i * 40, 4029, 4030, i + 10, GumpButtonType.Reply, 0);
				AddTextEntry(20, 24 + i * 40, 118, 20, 1152, i, m_Entry.Locations[i].Name);
			}
			AddButton(84, 419, 247, 248, 100, GumpButtonType.Reply, 0);
		}

		public override void OnResponse(NetState sender, RelayInfo info)
		{
			Mobile m = sender.Mobile;
			int id = info.ButtonID;

			if (id > -1 && id < 10)
			{
				m_Entry.Locations[id].Location = m.Location;
				m_Entry.Locations[id].Map = m.Map;
			}

			else if (id > 9 && id < 20)
				m_Entry.Locations[id - 10].Name = info.TextEntries[id - 10].Text;

			else if (id == 100)
			{
				m.SendGump(new ToolbarGump(m_Entry, m));
				return;
			}

			m.SendGump(new ToolBarLocsGump(m_Entry));
		}
	}
}