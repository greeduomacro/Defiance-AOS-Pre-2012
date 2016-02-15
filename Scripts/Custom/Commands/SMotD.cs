using System;
using System.IO;
using System.Collections.Generic;
using Server.Gumps;
using Server.Network;
using Server.Mobiles;
using Server.Commands;
using Server.Accounting;

namespace Server.Misc
{
	public class SMotD
	{
		public static void Configure()
		{
			m_FirstLinks.Add("bug.casiopia.net");
			m_Messages.Add(new SMotDStruct(m_FirstSubject, m_FirstBody, m_FirstLinks));
			EventSink.Login += new LoginEventHandler(OnLogin);
			CommandSystem.Register("SMotD", AccessLevel.Counselor, new CommandEventHandler(OnCommand));
		}

		#region Members
		private static List<SMotDStruct> m_Messages = new List<SMotDStruct>();
		private static SeperateSaveData m_SaveData = new SeperateSaveData("Data", "SMotD", new DC.SaveMethod(Serialize), new DC.LoadMethod(Deserialize));
		public static List<SMotDStruct> Messages { get { return m_Messages; } }
		public static int LastMessage { get { return m_Messages.Count - 1; } }

		private const string m_GumpName = "Defiance Staff Message of the Day";
		private const string m_FirstSubject = "Welcome Defiance Staff";
		private const string m_FirstBody = "This is our new Staff Message of the Day."
			+ "<br><br>We will use it for staff announcments and other important staff info."
			+ "<br><br>You can read the messages again anytime you wish by using the [SMotD command.";
		private static List<string> m_FirstLinks = new List<string>();
		#endregion

		#region Event methods
		[Usage("SMotD")]
		[Description("Display the staff message of the day.")]
		private static void OnCommand(CommandEventArgs args)
		{
			SendMessage(args.Mobile);
		}

		private static void OnLogin(LoginEventArgs args)
		{
			Mobile m = args.Mobile;
			Account acct = (Account)m.Account;

			if (!Convert.ToBoolean(acct.GetTag("SMotD")) && m.AccessLevel > AccessLevel.Player)
			{
				SendMessage(m);
				m.SendMessage(0x35, "Staff Message has been updated.. Use [SMotD to view the staff message of the day!");
			}
		}
		#endregion

		#region Core methods
		private static void SendMessage(Mobile m)
		{
			Account acct = (Account)m.Account;
			m.CloseGump(typeof(SMotDGump));
			m.SendGump(new SMotDGump(m, LastMessage));
			acct.SetTag("SMotD", "true");
		}

		private static void OnListChanged()
		{
			m_SaveData.Save();
		}

		public static void OnNewMessage()
		{
			foreach (Mobile m in World.Mobiles.Values)
			{
				if (m == null || !(m is PlayerMobile) || (m.AccessLevel == AccessLevel.Player))
					continue;

				PlayerMobile pm = m as PlayerMobile;

				Account acct = (Account)m.Account;
				if (acct == null)
					continue;

				acct.SetTag("SMotD", "false");
				m.SendMessage(0x35, "Staff Message of the Day has been updated!");

			}

			OnListChanged();
		}

		public static void RemoveMessage(int entry)
		{
			if (m_Messages.Count > entry)
			{
				m_Messages.RemoveAt(entry);
				OnListChanged();
			}
		}

		public static void AddMessage(SMotDStruct mds)
		{
			m_Messages.Add(mds);
			OnNewMessage();
		}
		#endregion

		#region Serialising
		private static void Serialize(GenericWriter writer)
		{
			writer.Write(0);//version

			writer.Write(m_Messages.Count - 1);
			for (int i = 1; i < m_Messages.Count; i++)
			{
				SMotDStruct mds = m_Messages[i];

				writer.Write(mds.Subject);
				writer.Write(mds.Body);

				writer.Write(mds.Links.Count);
				for (int j = 0; j < mds.Links.Count; j++)
					writer.Write((string)mds.Links[j]);
			}
		}

		private static void Deserialize(GenericReader reader)
		{
			int version = reader.ReadInt();

			int count = reader.ReadInt();
			for (int i = 0; i < count; i++)
			{
				string subject = reader.ReadString();
				string body = reader.ReadString();

				int linkcount = reader.ReadInt();
				List<string> links = new List<string>();
				for (int j = 0; j < linkcount; j++)
					links.Add(reader.ReadString());

				m_Messages.Add(new SMotDStruct(subject, body, links));
			}
		}
		#endregion

		#region Gumps
		#region Regulars

		public static void GenerateRegulars(AdvGump gump, SMotDStruct mds, int id)
		{
			gump.Closable = true;
			gump.Disposable = true;
			gump.Dragable = true;
			gump.Resizable = false;

			gump.AddPage(0);
			gump.AddBackground(0, 0, 400, 476, 9380);
			gump.AddImageTiled(25, 60, 350, 10, 50);
			gump.AddImageTiled(75, 115, 250, 10, 50);
			gump.AddImageTiled(25, 300, 350, 10, 50);
			gump.AddImageTiled(25, 345, 350, 10, 50);
			gump.AddImageTiled(25, 383, 350, 10, 50);
			gump.AddBackground(30, 130, 340, 165, 9300);
			gump.AddHtml(0, 40, 400, 18, gump.Center(m_GumpName), false, false);
			gump.AddImage(343, 411, 9004);

			gump.AddPage(1);
			gump.AddHtml(0, 95, 400, 18, gump.Center(mds.Subject), false, false);
			gump.AddHtml(35, 135, 330, 155, gump.Colorize(mds.Body, "333333"), false, true);

			if (mds.Links.Count > 0)
			{
				gump.AddLabel(35, 320, 0, "Links:");
				for (int i = 0; i < mds.Links.Count; i++)
				{
					if (gump is SMotDGump)
						gump.AddButton(80 + i * 35, 320, 4011, 4011, 900 + i, GumpButtonType.Reply, 0);
					else
						gump.AddImage(80 + i * 35, 320, 4011);
				}
			}

			else
				gump.AddHtml(0, 320, 400, 18, gump.Center("Using the [SMotD command you can review all messages"), false, false);

			if (id != -1)
			{
				if (id < (SMotD.LastMessage))
				{
					gump.AddButton(32, 360, 9770, 9772, 1, GumpButtonType.Reply, 0);
					gump.AddLabel(60, 360, 0, "Next Message");
				}

				if (id > 0)
				{
					gump.AddButton(349, 360, 9771, 9773, 2, GumpButtonType.Reply, 0);
					gump.AddLabel(230, 360, 0, "Previous Message");
				}
			}
		}
		#endregion


		public class SMotDGump : AdvGump
		{
			private Mobile m_Mobile;
			private int m_ID;

			public SMotDGump(Mobile from, int mID) : base(50, 50)
			{
				m_Mobile = from;
				m_ID = mID;

				if (m_ID < 0 || m_ID >= SMotD.Messages.Count)
					return;

				SMotDStruct mds = Messages[m_ID];
				SMotD.GenerateRegulars(this, mds, mID);

				if (from.AccessLevel < AccessLevel.Administrator)
				{
					AddLabel(30, 395, 0, "Greetings,");
					AddLabel(90, 420, 0, "The Defiance AOS staff");
				}

				else
				{
					AddButton(30, 400, 4011, 4012, 3, GumpButtonType.Reply, 0);
					AddLabel(65, 400, 0, "New Message");
					AddButton(165, 400, 4026, 4027, 4, GumpButtonType.Reply, 0);
					AddLabel(200, 400, 0, "Edit");
					AddButton(260, 400, 4017, 4018, 5, GumpButtonType.Reply, 0);
					AddLabel(295, 400, 0, "Remove");
				}
			}

			public override void OnResponse(NetState Sender, RelayInfo info)
			{
				if (m_ID < 0 || m_ID >= SMotD.Messages.Count)
					return;

				PlayerMobile from = (PlayerMobile)m_Mobile;
				SMotDStruct mds = Messages[m_ID];

				if (info.ButtonID >= 900)
				{
					int li = info.ButtonID - 900;
					if (li >= 0 && li < mds.Links.Count)
						from.LaunchBrowser(mds.Links[li]);
					from.SendGump(new SMotDGump(from, m_ID));
				}

				else
					switch (info.ButtonID)
				{
					case 0: break;
					case 1: from.SendGump(new SMotDGump(from, m_ID + 1)); break;
					case 2: from.SendGump(new SMotDGump(from, m_ID - 1)); break;
					case 3:
						if (from.AccessLevel >= AccessLevel.Administrator)
								from.SendGump(new ModSMoTDGump(new SMotDStruct(from.Name.ToString() + " : " + DateTime.Today.ToShortDateString(), "", new List<string>()), false));
						break;
					case 4:
						if (from.AccessLevel >= AccessLevel.Administrator)
						{
							if (m_ID == 0)
							{
								from.SendMessage("You cannot edit this message.");
								from.SendGump(new SMotDGump(from, SMotD.LastMessage));
							}

							else
								from.SendGump(new ModSMoTDGump(Messages[m_ID], true));
						}
						break;
					case 5:
						if (from.AccessLevel >= AccessLevel.Administrator)
						{
							if (m_ID == 0)
							{
								from.SendMessage("You cannot remove this message.");
								from.SendGump(new SMotDGump(from, SMotD.LastMessage));
							}

							else
							{
								SMotD.RemoveMessage(m_ID);
								from.SendGump(new SMotDGump(from, SMotD.LastMessage));
							}
						 }
						break;
				}
			}
		}

		#region ModSMoTDGump
		public class ModSMoTDGump : AdvGump
		{
			private SMotDStruct m_Mds;
			private bool m_Existing;

			public ModSMoTDGump(SMotDStruct mds, bool existing)
				: base()
			{
				m_Mds = mds;
				m_Existing = existing;

				SMotD.GenerateRegulars(this, mds, -1);

				AddButton(30, 400, 4011, 4012, 1, GumpButtonType.Reply, 0);
				AddLabel(65, 400, 0, existing? "Submit Message":"Add Message");
				AddButton(260, 400, 4017, 4018, 0, GumpButtonType.Reply, 0);
				AddLabel(295, 400, 0, "Cancel");

				AddBackground(400, 30, 375, 400, 9380);

				AddLabel(445, 60, 0, "Change title:");
				AddBackground(445, 85, 250, 20, 9300);
				AddTextEntry(450, 85, 230, 20, 0, 2, "");
				AddButton(700, 85, 4014, 4015, 2, GumpButtonType.Reply, 0);

				AddImageTiled(450, 111, 277, 11, 50);

				AddLabel(445, 125, 0, "Add text to body:");
				AddBackground(445, 150, 250, 140, 9300);
				AddTextEntry(450, 155, 230, 130, 0, 3, "");
				AddButton(700, 270, 4014, 4015, 3, GumpButtonType.Reply, 0);

				AddLabel(445, 300, 0, "Remove characters:");
				AddBackground(600, 300, 100, 22, 9300);
				AddTextEntry(605, 300, 90, 15, 0, 4, "");
				AddButton(700, 300, 4014, 4015, 4, GumpButtonType.Reply, 0);

				AddImageTiled(448, 330, 277, 11, 50);


				AddLabel(445, 350, 0, "Add Link:");
				AddBackground(520, 345, 180, 25, 9300);
				AddTextEntry(525, 350, 170, 15, 0, 5, "");
				AddButton(700, 350, 4014, 4015, 5, GumpButtonType.Reply, 0);
				AddLabel(590, 370, 0, "Remove last link");
				AddButton(700, 370, 4014, 4015, 6, GumpButtonType.Reply, 0);
			}

			public override void OnResponse(NetState sender, RelayInfo info)
			{
				Mobile from = sender.Mobile;

				switch (info.ButtonID)
				{
					case 0: break;
					case 1:
						if(!m_Existing)
							SMotD.AddMessage(m_Mds);

						from.SendGump( new SMotDGump(from, SMotD.LastMessage));
						break;
					case 2:
						m_Mds.Subject = info.GetTextEntry(2).Text;
						from.SendGump(new ModSMoTDGump(m_Mds, m_Existing));
						break;
					case 3:
						m_Mds.Body = m_Mds.Body + info.GetTextEntry(3).Text;
						from.SendGump(new ModSMoTDGump(m_Mds, m_Existing));
						break;
					case 4:
						try
						{
							int toremove = Convert.ToInt32(info.GetTextEntry(4).Text);
							int length = m_Mds.Body.Length;

							if(toremove > length)
								from.SendMessage("You cannot remove that many characters.");

							else if(toremove < 1)
								from.SendMessage("You can only remove a positive amount of characters.");

							else
								m_Mds.Body = m_Mds.Body.Remove(length - toremove);
						}
						catch
						{
							from.SendMessage("Bad format. An integer was expected.");
						}

						from.SendGump(new ModSMoTDGump(m_Mds, m_Existing));
						break;
					case 5:
						if (m_Mds.Links.Count < 5)
							m_Mds.Links.Add(info.GetTextEntry(5).Text);

						else
							from.SendMessage("The maximum amount of links has been reached already.");

						from.SendGump(new ModSMoTDGump(m_Mds, m_Existing));
						break;
					case 6:
						if (m_Mds.Links.Count > 0)
							m_Mds.Links.RemoveAt(m_Mds.Links.Count - 1);

						else
							from.SendMessage("This message does not contain any links.");

						from.SendGump(new ModSMoTDGump(m_Mds, m_Existing));
						break;
				}
			}
		}
		#endregion
		#endregion

		#region Struct
		public struct SMotDStruct
		{
			public string Subject;
			public string Body;
			public List<string> Links;

			public SMotDStruct(string subj, string body, List<string> links)
			{
				Subject = subj;
				Body = body;
				Links = links;
			}
		}
		#endregion
	}
}