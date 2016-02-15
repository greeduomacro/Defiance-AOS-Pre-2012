using System;
using Server.Gumps;
using Server.Network;
using System.Collections.Generic;
using Server.Multis.CustomBuilding;

namespace Server.Events.CrateRace
{
	public class CrateStoneGump : AdvGump
	{
		CrateStone m_Stone;

		public CrateStoneGump(CrateStone stone)
			: base()
		{
			m_Stone = stone;

			Closable = true;
			Disposable = true;
			Dragable = true;
			Resizable = false;
			AddPage(0);
			AddBackground(106, 102, 363, 265, 9200);
			AddBackground(171, 110, 219, 27, 9200);
			AddLabel(202, 115, 0x480, "Crate Race Control Center");
			AddButton(385, 235, 241, 242, (int)Buttons.Canc, GumpButtonType.Reply, 0);
			AddButton(385, 265, 247, 248, (int)Buttons.Ok, GumpButtonType.Reply, 0);

			AddImageTiled(110, 150, 180, 70, 0xA40);
			AddAlphaRegion(110, 150, 180, 70);
			AddImageTiled(295, 150, 165, 70, 0xA40);
			AddAlphaRegion(295, 150, 165, 70);
			AddImageTiled(110, 225, 260, 85, 0xA40);
			AddAlphaRegion(110, 225, 260, 85);
			AddImageTiled(110, 315, 260, 40, 0xA40);
			AddAlphaRegion(110, 315, 260, 40);

			AddButton(300, 155, 4005, 4006, (int)Buttons.Build, GumpButtonType.Reply, 0); AddLabel(335, 155, 0x480, "Build RaceTrack");
			AddButton(300, 180, 4005, 4006, (int)Buttons.Demolish, GumpButtonType.Reply, 0); AddLabel(335, 180, 0x480, "Demolish RaceTrack");

			List<string> arr = new List<string>();
			arr.Add("Crates:");
			arr.Add(stone.Crates.ToString());
			arr.Add("Participants:");
			arr.Add(stone.Participants.ToString());
			arr.Add("First Place:");
			arr.Add(stone.FirstPlace);
			AddTable(120, 155, new int[] { 100, 80 }, arr, new string[2] { "FFFFFF", "FFFFFF" });

			/*AddCheck(120, 230, 210, 211, false, (int)Buttons.Checklaps); AddLabel(150, 230, 0x480, "Laps"); AddTextEntry(245, 230, 103, 20, 0x480, (int)Buttons.Entrylaps, stone.Laps.ToString());
			AddCheck(120, 255, 210, 211, false, (int)Buttons.Checkcrates); AddLabel(150, 255, 0x480, "Crates"); AddTextEntry(245, 255, 91, 20, 0x480, (int)Buttons.Entrycrates, stone.MaxCrates.ToString());
			*/
			AddButton(120, 230, 4005, 4006, (int)Buttons.Settings, GumpButtonType.Reply, 0); AddLabel(155, 230, 0x480, "Change Settings");
			if (m_Stone.Rectangles.Count > 0)
			{
				AddButton(120, 255, 4005, 4006, (int)Buttons.ShowRects, GumpButtonType.Reply, 0);
				AddLabel(155, 255, 0x480, "Show Regions");
			}
			AddButton(120, 280, 4005, 4006, (int)Buttons.NewRects, GumpButtonType.Reply, 0);
			AddLabel(155, 280, 0x480, "Set New Regions");

			AddRadio(120, 325, 208, 209, false, (int)Buttons.Radiostart); AddLabel(140, 325, 0x480, "Start");
			AddRadio(245, 325, 208, 209, false, (int)Buttons.Radiostop); AddLabel(265, 325, 0x480, "Stop");
		}

		public enum Buttons
		{
			Canc,
			Ok,
			Build,
			Demolish,
			Checklaps,
			Checkcrates,
			Entrylaps,
			Entrycrates,
			Radiostart,
			Radiostop,
			Settings,
			NewRects,
			ShowRects
		}

		public override void OnResponse(NetState sender, RelayInfo info)
		{
			Mobile from = sender.Mobile;

			switch (info.ButtonID)
			{
				case (int)Buttons.Canc: break;
				case (int)Buttons.Settings:
					from.SendGump(new CRSGump(m_Stone));
					break;

				case (int)Buttons.Build:
					if (m_Stone.Track == null)
					{
						Point3D loc = m_Stone.Location;
						if (FileBasedBuilding.BuildingTable.ContainsKey("RaceTrack"))
						{
							FileBasedAddonBuilding track = new FileBasedAddonBuilding("RaceTrack");
							track.Location = new Point3D(loc.X - 13, loc.Y - 23, loc.Z);
							track.Map = m_Stone.Map;
							m_Stone.Track = (Item)track;
						}
						else
							from.SendMessage("The buildingTable does not contain this type.");
					}

					break;

				case (int)Buttons.Demolish:
					if (m_Stone.Track != null)
					{
						m_Stone.Track.Delete();
						m_Stone.Track = null;
					}

					break;

				case (int)Buttons.Ok:
					if (info.IsSwitched((int)Buttons.Radiostart))
						m_Stone.Running = true;

					if (info.IsSwitched((int)Buttons.Radiostop))
						m_Stone.Running = false;

					break;
				case (int)Buttons.NewRects:
					new RectangleSetup(from,m_Stone,false);
					break;
				case (int)Buttons.ShowRects:
					if(m_Stone.Rectangles.Count > 0)
						new RectangleSetup(from, m_Stone, true);
					break;
			}
		}
	}

	public class CRSGump : AdvGump
	{
		CrateStone m_Stone;

		public CRSGump(CrateStone stone)
			: base()
		{
			m_Stone = stone;

			Closable = true;
			Disposable = true;
			Dragable = true;
			Resizable = false;

			AddBackground(100, 100, 250, 300, 9200);
			AddBackground(125, 115, 200, 25, 9200);
			AddLabel(160, 115, 0x480, "Crate Race Settings");

			/*AddAlphaRegion(110, 158, 104, 18);
			AddAlphaRegion(216, 158, 112, 17);*/

			List<string> arr = new List<string>();
			arr.Add("Laps:");
			arr.Add("");
			arr.Add("Max. Crates:");
			arr.Add("");
			arr.Add("Gates Open:");
			arr.Add("");
			arr.Add("Gateprice:");
			arr.Add("");
			arr.Add("Props for more");
			arr.Add("options!");
			AddTable(110, 155, new int[2] { 100, 120 }, arr, new string[2] { "333333", "FFFFFF" }, 100, 2);

			AddTextEntry(217, 156, 103, 20, 0x480, (int)Buttons.Entrylaps, stone.Laps.ToString());
			AddTextEntry(217, 174, 103, 20, 0x480, (int)Buttons.EntryCrates, stone.MaxCrates.ToString());
			//AddTextEntry(217, 192, 103, 20, 0x480, (int)Buttons.EntryGates, stone.GatesInterval.ToString());
			AddTextEntry(217, 210, 103, 20, 0x480, (int)Buttons.EntryGateprice, stone.Price.ToString());

			AddButton(257, 350, 247, 248, (int)Buttons.Ok, GumpButtonType.Reply, 0);
			AddButton(145, 350, 241, 242, (int)Buttons.Canc, GumpButtonType.Reply, 0);
		}

		public enum Buttons
		{
			Canc,
			Ok,
			Entrylaps,
			EntryCrates,
			EntryGates,
			EntryGateprice
		}

		public override void OnResponse(NetState sender, RelayInfo info)
		{
			Mobile from = sender.Mobile;

			switch (info.ButtonID)
			{
				case (int)Buttons.Canc:
					break;

				case (int)Buttons.Ok:

					for (int i = 0; i < 4; i++)
					{
						TextRelay text = info.GetTextEntry((int)Buttons.Entrylaps);

						switch (i)
						{
							case 0: break;
							case 1: text = info.GetTextEntry((int)Buttons.EntryCrates); break;
							case 2: text = info.GetTextEntry((int)Buttons.EntryGates); break;
							case 3: text = info.GetTextEntry((int)Buttons.EntryGateprice); break;
						}

						try
						{
							if (text != null)
								switch (i)
								{
									case 0: m_Stone.Laps = Convert.ToInt32(text.Text); break;
									case 1: m_Stone.MaxCrates = Convert.ToInt32(text.Text); break;
									//case 2: m_Stone.GatesInterval = Convert.ToInt32(text.Text); break;
									case 3: m_Stone.Price = Convert.ToInt32(text.Text); break;
								}
						}
						catch
						{
							from.SendMessage("Bad format. An integer was expected.");
						}
					}
					break;
			}
		}
	}

	public class RectangleSetup
	{
		private CrateStone m_Stone;
		private Mobile m_Mobile;
		private List<CrateRectangle> m_List = new List<CrateRectangle>();
		private int m_Number;
		private List<Item> TileList = new List<Item>();

		public CrateRectangle Current = null;
		public CrateRectangle Last;
		public CrateRectangle First;
		public List<CrateRectangle> List { get { return m_List; } }
		public int Number
		{
			get { return m_Number; }
			set
			{
				m_Number = value;
				ResetEntries();
				SetEntries();
			}
		}
		public int Count { get { return m_List.Count; } }

		public RectangleSetup(Mobile m, CrateStone stone, bool haslist)
		{
			m_Mobile = m;
			m_Stone = stone;
			if (haslist)
			{
				m_List = stone.Rectangles;
				ChangeNumber(m_List.Count - 1);
			}
			else
				BoundingBoxPicker.Begin(m, new BoundingBoxCallback(NewRegion_Callback), 0);
		}

		public void HighLightRectangle(Rectangle2D rect, int color)
		{
			Map map = m_Stone.Map;

			for (int x = rect.Start.X; x <= rect.End.X; x++)
			{
				for (int y = rect.Start.Y; y <= rect.End.Y; y++)
				{
					Tile landtile = map.Tiles.GetLandTile(x, y);
					Point3D xyz = new Point3D(x, y, landtile.Z);
					Item item = new HighLighter(color);
					item.MoveToWorld(xyz, map);

					foreach (Item it in TileList)
					{
						if (it.Location == item.Location)
						{
							it.Hue = 33;
							item.Hue = 33;
						}
					}

					TileList.Add(item);
				}
			}
		}

		public void SetList()
		{
			m_Stone.Rectangles = m_List;
			ResetEntries();
		}

		public void ResetEntries()
		{
			foreach (Item item in TileList)
				if(item != null && !item.Deleted)
				item.Delete();

			TileList.Clear();
		}

		public void SetEntries()
		{
			if (Count > 0)
			{
				First = m_List[0];
				HighLightRectangle(First.Rectangle, 1150);

				if (m_Number < Count)
				{
					Current = m_List[m_Number];
					HighLightRectangle(Current.Rectangle, 1151);
				}

				else
					Current = null;

				if (m_Number - 1 > 0)
				{
					Last = m_List[m_Number - 1];
					HighLightRectangle(Last.Rectangle, 1152);
				}
				else
					Last = null;
			}

			else
			{
				Current = null;
				First = null;
				Last = null;
			}
		}

		public void ChangeNumber(int newnumber)
		{
			if (newnumber >= 0 && newnumber < m_List.Count)
				Number = newnumber;

			else
				Number = m_List.Count;

			m_Mobile.SendGump(new RegionSettingsGump(this));
		}

		public void NewRegion_Callback(Mobile from, Map map, Point3D start, Point3D end, object state)
		{
			int loc = (int)state;
			Rectangle2D rect = new Rectangle2D(start, end);
			if (loc < m_List.Count)
			{
				m_List[loc] = new CrateRectangle(rect);
				Number = loc;
			}

			else
			{
				m_List.Add(new CrateRectangle(rect));
				Number = m_List.Count - 1;
			}

			from.SendGump(new RegionSettingsGump(this));
		}

		private class HighLighter : Item
		{
			public HighLighter(int hue)
				: base(1312)
			{
				Hue = hue;
				Movable = false;
			}

			public HighLighter(Serial serial)
				: base(serial)
			{
			}

			public override void Serialize(GenericWriter writer)
			{
				base.Serialize(writer);
			}
			public override void Deserialize(GenericReader reader)
			{
				base.Deserialize(reader);
				Delete();
			}
		}
	}

	public class RegionSettingsGump : AdvGump
	{
		private RectangleSetup m_Setup;

		public RegionSettingsGump(RectangleSetup setup)
			: base(0, 0)
		{
			m_Setup = setup;
			int count = setup.Count;
			if (setup.Number >= setup.Count)
				return;

			Closable = true;
			Disposable = true;
			Dragable = true;
			Resizable = false;
			AddPage(0);

			AddBackground(0, 260, 245, 232, 3600);
			AddBackground(246, 260, 245, 232, 3600);
			AddBackground(0, 146, 450, 129, 3600);
			AddBackground(0, 0, 450, 167, 3600);
			AddHtml(8, 19, 352, 22, "Mod gump", false, false);

			AddButton(15, 69, 4005, 4006, 1, GumpButtonType.Reply, 0);
			AddLabel(61, 71, 1152, "Add Rectangle");
			AddButton(186, 67, 4005, 4006, 6, GumpButtonType.Reply, 0);
			AddLabel(232, 69, 1152, "Finish");

			if (m_Setup.First != null)
			{
				AddButton(15, 96, 4005, 4006, 2, GumpButtonType.Reply, 0);
				AddLabel(58, 100, 1152, "First Rectangle:");
				AddLabel(173, 100, 1152, m_Setup.First.Rectangle.ToString());
				AddLabel(310, 101, 1152, m_Setup.First.FirstDirection.ToString());
				AddLabel(350, 101, 1152, m_Setup.First.SecondDirection.ToString());
			}

			if (m_Setup.Last != null)
			{
				int last = m_Setup.Number - 1;
				AddButton(15, 124, 4005, 4006, 3, GumpButtonType.Reply, 0);
				AddLabel(56, 127, 1152, "Last Rectangle:");
				AddLabel(174, 126, 1152, m_Setup.Last.Rectangle.ToString());
				AddLabel(310, 126, 1152, m_Setup.Last.FirstDirection.ToString());
				AddLabel(350, 126, 1152, m_Setup.Last.SecondDirection.ToString());
			}

			AddLabel(15, 176, 1152, "Current Rectangle:");
			AddLabel(143, 176, 1152, m_Setup.Current.Rectangle.ToString());
			AddLabel(310, 176, 1152, m_Setup.Current.FirstDirection.ToString());
			AddLabel(350, 176, 1152, m_Setup.Current.SecondDirection.ToString());
			AddButton(20, 203, 4005, 4006, 4, GumpButtonType.Reply, 0);
			AddLabel(64, 205, 1152, "Remove this rectangle");

			AddLabel(70, 229, 1152, "Retry");
			AddButton(18, 229, 4005, 4006, 5, GumpButtonType.Reply, 0);

			AddLabel(78, 365, 1152, "First Direction:");
			//AddButton(94, 290, 4500, 4500, 11, GumpButtonType.Reply, 0);
			AddButton(161, 292, 4501, 4501, 11, GumpButtonType.Reply, 0);
			//AddButton(162, 354, 4502, 4502, 13, GumpButtonType.Reply, 0);
			AddButton(21, 290, 4507, 4507, 14, GumpButtonType.Reply, 0);
			AddButton(152, 422, 4503, 4503, 12, GumpButtonType.Reply, 0);
			//AddButton(13, 352, 4506, 4506, 13, GumpButtonType.Reply, 0);
			//AddButton(83, 435, 4504, 4504, 17, GumpButtonType.Reply, 0);
			AddButton(18, 417, 4505, 4505, 13, GumpButtonType.Reply, 0);

			AddLabel(324, 365, 1152, "Second Direction:");
			//AddButton(340, 290, 4500, 4500, 21, GumpButtonType.Reply, 0);
			AddButton(405, 292, 4501, 4501, 21, GumpButtonType.Reply, 0);
			//AddButton(405, 354, 4502, 4502, 23, GumpButtonType.Reply, 0);
			AddButton(265, 290, 4507, 4507, 24, GumpButtonType.Reply, 0);
			AddButton(395, 422, 4503, 4503, 22, GumpButtonType.Reply, 0);
			//AddButton(265, 352, 4506, 4506, 23, GumpButtonType.Reply, 0);
			//AddButton(320, 435, 4504, 4504, 27, GumpButtonType.Reply, 0);
			AddButton(265, 417, 4505, 4505, 23, GumpButtonType.Reply, 0);
		}

		public override void OnResponse(NetState sender, RelayInfo info)
		{
			Mobile m = sender.Mobile;

			switch (info.ButtonID)
			{
				case 1:
					BoundingBoxPicker.Begin(m, new BoundingBoxCallback(m_Setup.NewRegion_Callback), m_Setup.Number + 1);
					break;
				case 2:
					m_Setup.ChangeNumber(0);
					break;
				case 3:
					m_Setup.ChangeNumber(m_Setup.Number - 1);
					break;
				case 4:
					m_Setup.List.RemoveAt(m_Setup.Number);
					m_Setup.ChangeNumber(m_Setup.Number - 1);
					break;
				case 5:
					BoundingBoxPicker.Begin(m, new BoundingBoxCallback(m_Setup.NewRegion_Callback), m_Setup.Number);
					break;
				case 6:
					m_Setup.SetList();
					break;
			}

			if (info.ButtonID > 10 && info.ButtonID < 19)
			{
				if (m_Setup.Current != null)
				{
					CrateRectangle newest = m_Setup.Current;

					switch (info.ButtonID - 10)
					{
						//case 1: newest.FirstDirection = Direction.Mask; break;
						case 1: newest.FirstDirection = Direction.North; break;
						//case 3: newest.FirstDirection = Direction.Right; break;
						case 2: newest.FirstDirection = Direction.East; break;
						//case 5: newest.FirstDirection = Direction.Down; break;
						case 3: newest.FirstDirection = Direction.South; break;
					   // case 7: newest.FirstDirection = Direction.Left; break;
						case 4: newest.FirstDirection = Direction.West; break;
					}
				}
				m.SendGump(new RegionSettingsGump(m_Setup));
			}

			else if (info.ButtonID > 20 && info.ButtonID < 29)
			{
				if (m_Setup.Current != null)
				{
					CrateRectangle newest = m_Setup.Current;

					switch (info.ButtonID - 20)
					{
						//case 1: newest.FirstDirection = Direction.Mask; break;
						case 1: newest.SecondDirection = Direction.North; break;
						//case 3: newest.FirstDirection = Direction.Right; break;
						case 2: newest.SecondDirection = Direction.East; break;
						//case 5: newest.FirstDirection = Direction.Down; break;
						case 3: newest.SecondDirection = Direction.South; break;
						// case 7: newest.FirstDirection = Direction.Left; break;
						case 4: newest.SecondDirection = Direction.West; break;
					}
				}
				m.SendGump(new RegionSettingsGump(m_Setup));
			}
		}
	}

	public class CrateRectangle
	{
		public Rectangle2D Rectangle;
		public Direction FirstDirection, SecondDirection;

		public CrateRectangle(Rectangle2D rect)
		{
			Rectangle = rect;
			FirstDirection = Direction.North;
			SecondDirection = Direction.North;
		}

		public CrateRectangle(GenericReader reader)
		{
			int version = reader.ReadInt();

			Rectangle = reader.ReadRect2D();
			FirstDirection = (Direction)reader.ReadInt();
			SecondDirection = (Direction)reader.ReadInt();
		}

		public void Serialize(GenericWriter writer)
		{
			writer.Write(0); // version

			writer.Write(Rectangle);
			writer.Write((int)FirstDirection);
			writer.Write((int)SecondDirection);
		}
	}
}