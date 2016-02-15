using System;
using System.Collections;
using System.Collections.Generic;
using Server.Commands;
using Server.Mobiles;
using Server.Network;

namespace Server.Gumps
{
	public class WhoOnline : Gump
	{
		public static void Initialize()
		{
			CommandSystem.Register("WhoOnline", AccessLevel.Player, new CommandEventHandler(WhoOnline_OnCommand));
		}

		[Usage("WhoOnline [filter]")]
		[Description("Lists all connected clients. Optionally filters results by name.")]
		private static void WhoOnline_OnCommand(CommandEventArgs e)
		{
			e.Mobile.SendGump(new WhoOnline(e.Mobile, e.ArgString));
		}

		public static bool OldStyle = PropsConfig.OldStyle;

		public static readonly int GumpOffsetX = PropsConfig.GumpOffsetX;
		public static readonly int GumpOffsetY = PropsConfig.GumpOffsetY;

		public static readonly int TextHue = PropsConfig.TextHue;
		public static readonly int TextOffsetX = PropsConfig.TextOffsetX;

		public static readonly int OffsetGumpID = PropsConfig.OffsetGumpID;
		public static readonly int HeaderGumpID = PropsConfig.HeaderGumpID;
		public static readonly int EntryGumpID = PropsConfig.EntryGumpID;
		public static readonly int BackGumpID = PropsConfig.BackGumpID;
		public static readonly int SetGumpID = PropsConfig.SetGumpID;

		public static readonly int SetWidth = PropsConfig.SetWidth;
		public static readonly int SetOffsetX = PropsConfig.SetOffsetX, SetOffsetY = PropsConfig.SetOffsetY;
		public static readonly int SetButtonID1 = PropsConfig.SetButtonID1;
		public static readonly int SetButtonID2 = PropsConfig.SetButtonID2;

		public static readonly int PrevWidth = PropsConfig.PrevWidth;
		public static readonly int PrevOffsetX = PropsConfig.PrevOffsetX, PrevOffsetY = PropsConfig.PrevOffsetY;
		public static readonly int PrevButtonID1 = PropsConfig.PrevButtonID1;
		public static readonly int PrevButtonID2 = PropsConfig.PrevButtonID2;

		public static readonly int NextWidth = PropsConfig.NextWidth;
		public static readonly int NextOffsetX = PropsConfig.NextOffsetX, NextOffsetY = PropsConfig.NextOffsetY;
		public static readonly int NextButtonID1 = PropsConfig.NextButtonID1;
		public static readonly int NextButtonID2 = PropsConfig.NextButtonID2;

		public static readonly int OffsetSize = PropsConfig.OffsetSize;

		public static readonly int EntryHeight = PropsConfig.EntryHeight;
		public static readonly int BorderSize = PropsConfig.BorderSize;

		private static bool PrevLabel = false, NextLabel = false;

		private static readonly int PrevLabelOffsetX = PrevWidth + 1;
		private static readonly int PrevLabelOffsetY = 0;

		private static readonly int NextLabelOffsetX = -29;
		private static readonly int NextLabelOffsetY = 0;

		private static readonly int EntryWidth = 180;
		private static readonly int EntryCount = 15;

		private static readonly int TotalWidth = OffsetSize + EntryWidth + OffsetSize + SetWidth + OffsetSize;
		private static readonly int TotalHeight = OffsetSize + ((EntryHeight + OffsetSize) * (EntryCount + 1));

		private static readonly int BackWidth = BorderSize + TotalWidth + BorderSize;
		private static readonly int BackHeight = BorderSize + TotalHeight + BorderSize;

		private Mobile m_Owner;
		private ArrayList m_Mobiles;
		private int m_Page;

		private class InternalComparer : IComparer
		{
			public static readonly IComparer Instance = new InternalComparer();

			public InternalComparer()
			{
			}

			public int Compare(object x, object y)
			{
				if (x == null && y == null)
					return 0;
				else if (x == null)
					return -1;
				else if (y == null)
					return 1;

		string aname;
		string bname;
		AccessLevel aAccessLevel;
		AccessLevel bAccessLevel;
		if ( x is Mobile )
		{
			Mobile a = x as Mobile;
			if (a == null)
				throw new ArgumentException();
			aname = a.Name;
			aAccessLevel = a.AccessLevel;
		}
		else
		{
			aname = (string)(x);
			aAccessLevel = AccessLevel.Player;
		}
		if ( y is Mobile )
		{
			Mobile b = y as Mobile;
			if (b == null)
				throw new ArgumentException();
			bname = b.Name;
			bAccessLevel = b.AccessLevel;
		}
		else
		{
			bname = (string)(y);
			bAccessLevel = AccessLevel.Player;
		}

			if (aAccessLevel > bAccessLevel)
				return -1;
			else if (aAccessLevel < bAccessLevel)
				return 1;
			else
				return Insensitive.Compare(aname, bname);
			}
		}

		public WhoOnline(Mobile owner, string filter)
			: this(owner, BuildList(owner, filter), 0)
		{
		}

		public WhoOnline(Mobile owner, ArrayList list, int page)
			: base(GumpOffsetX, GumpOffsetY)
		{
			owner.CloseGump(typeof(WhoOnline));

			m_Owner = owner;
			m_Mobiles = list;

			Initialize(page);
		}

		public static ArrayList BuildList(Mobile owner, string filter)
		{
			if (filter != null && (filter = filter.Trim()).Length == 0)
				filter = null;
			else
				filter = filter.ToLower();

			ArrayList list = new ArrayList();
			List<NetState> states = NetState.Instances;

			for (int i = 0; i < states.Count; ++i)
			{
				Mobile m = states[i].Mobile;

				if (m != null && (m == owner || m.AccessLevel == AccessLevel.Player))
				{
					if (filter != null && (m.Name == null || m.Name.ToLower().IndexOf(filter) < 0))
						continue;

					list.Add(m);
				}
			}

			/* Disabled by Silver
			//Add begin
			int fake_cnt;
			if (list.Count < 10 || list.Count > 60)
				fake_cnt = 1 + (int)(list.Count * 0.3 );
			else
				fake_cnt = 1 + (int)(list.Count * 0.4 );
			DateTime now = DateTime.Now;
			string[] namelist = NameList.GetNameList("female").List;
			int idx;
			for (int i = 0; i < fake_cnt; ++i)
			{
				idx = 250 + 30*i + now.Hour + (now.Day * 30);
				while (idx > namelist.Length) //It doesn't exceed the length, but let's check to be sure :)
					idx /= 10;
				string fname = (string)(namelist[idx]);
				list.Add( fname );
			}
			//Add end
			*/

			list.Sort(InternalComparer.Instance);

			return list;
		}

		public void Initialize(int page)
		{
			m_Page = page;

			int count = m_Mobiles.Count - (page * EntryCount);

			if (count < 0)
				count = 0;
			else if (count > EntryCount)
				count = EntryCount;

			int totalHeight = OffsetSize + ((EntryHeight + OffsetSize) * (count + 1));

			AddPage(0);

			AddBackground(0, 0, BackWidth, BorderSize + totalHeight + BorderSize, BackGumpID);
			AddImageTiled(BorderSize, BorderSize, TotalWidth - (OldStyle ? SetWidth + OffsetSize : 0), totalHeight, OffsetGumpID);

			int x = BorderSize + OffsetSize;
			int y = BorderSize + OffsetSize;

			int emptyWidth = TotalWidth - PrevWidth - NextWidth - (OffsetSize * 4) - (OldStyle ? SetWidth + OffsetSize : 0);

			if (!OldStyle)
				AddImageTiled(x - (OldStyle ? OffsetSize : 0), y, emptyWidth + (OldStyle ? OffsetSize * 2 : 0), EntryHeight, EntryGumpID);

			AddLabel(x + TextOffsetX, y, TextHue, String.Format("Page {0} of {1} ({2})", page + 1, (m_Mobiles.Count + EntryCount - 1) / EntryCount, m_Mobiles.Count));

			x += emptyWidth + OffsetSize;

			if (OldStyle)
				AddImageTiled(x, y, TotalWidth - (OffsetSize * 3) - SetWidth, EntryHeight, HeaderGumpID);
			else
				AddImageTiled(x, y, PrevWidth, EntryHeight, HeaderGumpID);

			if (page > 0)
			{
				AddButton(x + PrevOffsetX, y + PrevOffsetY, PrevButtonID1, PrevButtonID2, 1, GumpButtonType.Reply, 0);

				if (PrevLabel)
					AddLabel(x + PrevLabelOffsetX, y + PrevLabelOffsetY, TextHue, "Previous");
			}

			x += PrevWidth + OffsetSize;

			if (!OldStyle)
				AddImageTiled(x, y, NextWidth, EntryHeight, HeaderGumpID);

			if ((page + 1) * EntryCount < m_Mobiles.Count)
			{
				AddButton(x + NextOffsetX, y + NextOffsetY, NextButtonID1, NextButtonID2, 2, GumpButtonType.Reply, 1);

				if (NextLabel)
					AddLabel(x + NextLabelOffsetX, y + NextLabelOffsetY, TextHue, "Next");
			}

			for (int i = 0, index = page * EntryCount; i < EntryCount && index < m_Mobiles.Count; ++i, ++index)
			{
				x = BorderSize + OffsetSize;
				y += EntryHeight + OffsetSize;

				if (m_Mobiles[index] is Mobile)
				{
					Mobile m = (Mobile)m_Mobiles[index];

					AddImageTiled(x, y, EntryWidth, EntryHeight, EntryGumpID);
					AddLabelCropped(x + TextOffsetX, y, EntryWidth - TextOffsetX, EntryHeight, GetHueFor(m, m_Owner), m.Deleted ? "(deleted)" : m.Name + ((m.Guild == null) ? "" : " ["+ m.Guild.Abbreviation + "]"));
				}
				else
				{
					AddImageTiled(x, y, EntryWidth, EntryHeight, EntryGumpID);
					AddLabelCropped(x + TextOffsetX, y, EntryWidth - TextOffsetX, EntryHeight, 0x58, (string)(m_Mobiles[index]));
				}
				x += EntryWidth + OffsetSize;

				if (SetGumpID != 0)
					AddImageTiled(x, y, SetWidth, EntryHeight, SetGumpID);
			}
		}

		private static int GetHueFor(Mobile m, Mobile from)
		{
			switch (m.AccessLevel)
			{
				case AccessLevel.Owner:
				case AccessLevel.Developer:
				case AccessLevel.Administrator: return 0x516;
				case AccessLevel.Seer: return 0x144;
				case AccessLevel.GameMaster: return 0x21;
				case AccessLevel.Counselor: return 0x2;
				case AccessLevel.Player:
				default:
					{
						if (m.Kills >= 5)
							return 0x21;
						else if (Server.Misc.NotorietyHandlers.MobileNotoriety(from, m) == Notoriety.Enemy)
							return 0x90;
						else if (m.Criminal)
							return 0x3B1;

						return 0x58;
					}
			}
		}

		public override void OnResponse(NetState state, RelayInfo info)
		{
			Mobile from = state.Mobile;

			switch (info.ButtonID)
			{
				case 0: // Closed
					{
						return;
					}
				case 1: // Previous
					{
						if (m_Page > 0)
							from.SendGump(new WhoOnline(from, m_Mobiles, m_Page - 1));

						break;
					}
				case 2: // Next
					{
						if ((m_Page + 1) * EntryCount < m_Mobiles.Count)
							from.SendGump(new WhoOnline(from, m_Mobiles, m_Page + 1));

						break;
					}
			}
		}
	}
}