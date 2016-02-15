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
	public class MotD
	{
		public static void Configure()
		{
			m_FirstLinks.Add("www.defianceuo.com/rules.php");
			m_FirstLinks.Add("www.defianceuo.com/forums");
			m_Messages.Add(new MotDStruct(m_FirstSubject, m_FirstBody, m_FirstLinks));
			EventSink.Login += new LoginEventHandler(OnLogin);
			CommandSystem.Register("MotD", AccessLevel.Player, new CommandEventHandler(OnCommand));
		}

		#region Members
		private static List<MotDStruct> m_Messages = new List<MotDStruct>();
		private static SeperateSaveData m_SaveData = new SeperateSaveData("Data", "MotD", new DC.SaveMethod(Serialize), new DC.LoadMethod(Deserialize));
		public static List<MotDStruct> Messages { get { return m_Messages; } }
		public static int LastMessage { get { return m_Messages.Count - 1; } }

		private const string m_GumpName = "Defiance Message of the Day";
		private const string m_FirstSubject = "Welcome to the Defiance AOS Shard";
		private const string m_FirstBody = "The staff wishes you a pleasant stay and good luck on your journey at Defiance AOS!"
			+ "<br><br>Be aware of our TOS which can be found in the first link below. And review our forums for alot of useful information."
			+ "<br><br>You can read the message of the day again anytime you wish by using the [MotD command.";
		private static List<string> m_FirstLinks = new List<string>();
		#endregion

		#region Event methods
		[Usage("MotD")]
		[Description("Display the message of the day.")]
		private static void OnCommand(CommandEventArgs args)
		{
			SendMessage(args.Mobile);
		}

		private static void OnLogin(LoginEventArgs args)
		{
			Mobile m = args.Mobile;
			Account acct = (Account)m.Account;

			if (!Convert.ToBoolean(acct.GetTag("MotD")))
			{
				SendMessage(m);
				m.SendMessage(0x35, "Message of the Day has been updated.. Use [MotD to view the message of the day!");
			}
		}
		#endregion

		#region Core methods
		private static void SendMessage(Mobile m)
		{
			Account acct = (Account)m.Account;
			m.CloseGump(typeof(MotDGump));
			m.SendGump(new MotDGump(m, LastMessage));
			acct.SetTag("MotD", "true");
		}

		private static void OnListChanged()
		{
			m_SaveData.Save();
		}

		public static void OnNewMessage()
		{
			foreach (Mobile m in World.Mobiles.Values)
			{
				if (m == null || !(m is PlayerMobile))
					continue;

				PlayerMobile pm = m as PlayerMobile;

				Account acct = (Account)m.Account;
				if (acct == null)
					continue;

				acct.SetTag("MotD", "false");
			}

			World.Broadcast(0x35, true, "Message of the Day has been updated!");
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

		public static void AddMessage(MotDStruct mds)
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
				MotDStruct mds = m_Messages[i];

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

				m_Messages.Add(new MotDStruct(subject, body, links));
			}
		}
		#endregion

		#region Gumps
		#region Regulars

		public static void GenerateRegulars(AdvGump gump, MotDStruct mds, int id)
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
					if (gump is MotDGump)
						gump.AddButton(80 + i * 35, 320, 4011, 4011, 900 + i, GumpButtonType.Reply, 0);
					else
						gump.AddImage(80 + i * 35, 320, 4011);
				}
			}

			else
				gump.AddHtml(0, 320, 400, 18, gump.Center("Using the [MotD command you can review all messages"), false, false);

			if (id != -1)
			{
				if (id < (MotD.LastMessage))
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


		public class MotDGump : AdvGump
		{
			private Mobile m_Mobile;
			private int m_ID;

			public MotDGump(Mobile from, int mID) : base(50, 50)
			{
				m_Mobile = from;
				m_ID = mID;

				if (m_ID < 0 || m_ID >= MotD.Messages.Count)
					return;

				MotDStruct mds = Messages[m_ID];
				MotD.GenerateRegulars(this, mds, mID);

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
				if (m_ID < 0 || m_ID >= MotD.Messages.Count)
					return;

				PlayerMobile from = (PlayerMobile)m_Mobile;
				MotDStruct mds = Messages[m_ID];

				if (info.ButtonID >= 900)
				{
					int li = info.ButtonID - 900;
					if (li >= 0 && li < mds.Links.Count)
						from.LaunchBrowser(mds.Links[li]);
					from.SendGump(new MotDGump(from, m_ID));
				}

				else
					switch (info.ButtonID)
				{
					case 0: break;
					case 1: from.SendGump(new MotDGump(from, m_ID + 1)); break;
					case 2: from.SendGump(new MotDGump(from, m_ID - 1)); break;
					case 3:
						if (from.AccessLevel >= AccessLevel.Administrator)
								from.SendGump(new ModMoTDGump(new MotDStruct(from.Name.ToString() + " : " + DateTime.Today.ToShortDateString(), "", new List<string>()), false));
						break;
					case 4:
						if (from.AccessLevel >= AccessLevel.Administrator)
						{
							if (m_ID == 0)
							{
								from.SendMessage("You cannot edit this message.");
								from.SendGump(new MotDGump(from, MotD.LastMessage));
							}

							else
								from.SendGump(new ModMoTDGump(Messages[m_ID], true));
						}
						break;
					case 5:
						if (from.AccessLevel >= AccessLevel.Administrator)
						{
							if (m_ID == 0)
							{
								from.SendMessage("You cannot remove this message.");
								from.SendGump(new MotDGump(from, MotD.LastMessage));
							}

							else
							{
								MotD.RemoveMessage(m_ID);
								from.SendGump(new MotDGump(from, MotD.LastMessage));
							}
						 }
						break;
				}
			}
		}

		#region ModMoTDGump
		public class ModMoTDGump : AdvGump
		{
			private MotDStruct m_Mds;
			private bool m_Existing;

			public ModMoTDGump(MotDStruct mds, bool existing)
				: base()
			{
				m_Mds = mds;
				m_Existing = existing;

				MotD.GenerateRegulars(this, mds, -1);

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
							MotD.AddMessage(m_Mds);

						from.SendGump( new MotDGump(from, MotD.LastMessage));
						break;
					case 2:
						m_Mds.Subject = info.GetTextEntry(2).Text;
						from.SendGump(new ModMoTDGump(m_Mds, m_Existing));
						break;
					case 3:
						m_Mds.Body = m_Mds.Body + info.GetTextEntry(3).Text;
						from.SendGump(new ModMoTDGump(m_Mds, m_Existing));
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

						from.SendGump(new ModMoTDGump(m_Mds, m_Existing));
						break;
					case 5:
						if (m_Mds.Links.Count < 5)
							m_Mds.Links.Add(info.GetTextEntry(5).Text);

						else
							from.SendMessage("The maximum amount of links has been reached already.");

						from.SendGump(new ModMoTDGump(m_Mds, m_Existing));
						break;
					case 6:
						if (m_Mds.Links.Count > 0)
							m_Mds.Links.RemoveAt(m_Mds.Links.Count - 1);

						else
							from.SendMessage("This message does not contain any links.");

						from.SendGump(new ModMoTDGump(m_Mds, m_Existing));
						break;
				}
			}
		}
		#endregion
		#endregion

		#region Struct
		public struct MotDStruct
		{
			public string Subject;
			public string Body;
			public List<string> Links;

			public MotDStruct(string subj, string body, List<string> links)
			{
				Subject = subj;
				Body = body;
				Links = links;
			}
		}
		#endregion
	}
}