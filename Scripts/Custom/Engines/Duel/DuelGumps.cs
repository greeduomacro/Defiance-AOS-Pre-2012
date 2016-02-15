//////////////////////////////\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
//			   This is a Sunny Production ©2005			\\
//					 Based on RunUO©				\\
//					Version: Beta 1.1				\\
//		Many thanks to my Csharp tutors Will and Eclipse		\\
//////////////////////////////\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\

using System;
using System.Collections;
using System.Collections.Generic;
using Server.Network;
using Server.Mobiles;
using Server.Gumps;
using Server.Factions;
using Server.Targeting;

namespace Server.Events.Duel
{
	public class DuelWelcomeGump : Gump
	{
		public DuelWelcomeGump()
			: base(30, 30)
		{
			Closable = true;
			Disposable = true;
			Dragable = true;
			Resizable = false;

			AddPage(0);
			AddImage(80, 36, 2080);
			AddImage(98, 143, 2082);
			AddImage(98, 73, 2081);
			AddImage(99, 353, 2081);
			AddImage(99, 283, 2082);
			AddImage(98, 213, 2081);
			AddImage(98, 422, 2082);
			AddImage(101, 492, 2083);
			AddImage(284, 445, 9004);
			AddHtml(112, 73, 236, 416,
				"This place has been specially constructed for the real duelists in this world. " +
				"Please talk to me to start a duel.<br><br>But be carefull, and prepared!<br>" +
				"Once you targeted an opponent, they may decide against duelling you, however " +
				"when your opponent decides to take on the challenge there is no way back for you " +
				String.Format("anymore.<br><br>To take part in a duel, you should have at least {0} gp in your bank account.", DuelSystem.GoldCost.ToString()) +
				String.Format("<br><br>The winner shall receive {0} gp. In case of a draw your money shall be ", (DuelSystem.GoldCost * 1.4).ToString())  +
				"returned minus 30% commission I take for my work.<br><br>Signed,<br>The DuelMaster", false, false);
			AddLabel(154, 39, 0, "Welcome to the Duel Arena!");
		}
	}

	public class BaseDuelGump : AdvGump
	{
		public BaseDuelGump() : base(30, 30)
		{
			Closable = true;
			Disposable = true;
			Dragable = true;
			Resizable = false;

			AddPage(0);
		}

		protected void AddBackType()
		{
			AddBackground(0, 0, 403, 200, 9270);
			AddLabel(20, 20, 1161, "Duel Type:");
		}

		protected void AddBackOptions()
		{
			AddBackground(0, 193, 403, 204, 9270);
			AddLabel(20, 213, 1161, "Options:");
		}
		protected void AddBackOpponent()
		{
			AddBackground(0, 390, 403, 115, 9270);
			AddLabel(20, 410, 1161, "Opponent:");
		}

		protected void AddBack2v2()
		{
			AddBackground(0, 390, 403, 150, 9270);
			AddLabel(20, 410, 1161, "Participants:");
		}

		protected void AddAccept()
		{
			AddBackground(396, 136, 190, 64, 9270);
			AddButton(415, 155, 247, 248, 1, GumpButtonType.Reply, 0);
			AddButton(500, 155, 241, 242, 0, GumpButtonType.Reply, 0);
		}
	}

	public class BaseDuelDataGump : BaseDuelGump
	{
		public DuelRune m_Rune;

		public BaseDuelDataGump(DuelRune rune) : base()
		{
			 m_Rune = rune;
		}

		protected string GetType(int type)
		{
			switch (type)
			{
				case 0: return "Normal Duel";
				case 1: return "Normal Mage Fight";
				case 2: return "True Mage Fight";
				case 3: return "Ultimate Mage Fight";
				case 4: return "Normal Dex Fight";
				case 5: return "True Dex Fight";
				case 6: return "Ultimate Dex Fight";
				case 7: return "2 versus 2";
			}
			return "";
		}

		protected string GetOption(int option)
		{
			switch (option)
			{
				case 0: return "No Summons";
				case 1: return "No Area Spells";
				case 2: return "No Mounts";
				case 3: return "No Potions";
				case 4: return "No Artifacts";
				case 5: return "No Magic Armor";
				case 6: return "No Magic Weapons";
				case 7: return "No Poisoned Weapons";
				case 8: return "3x SpellWatch";
			}
			return "";
		}

		protected void AddType()
		{
			AddBackType();
			for (int i = 0; i < 4; i++)
			{
				int im = 2151;
				if (m_Rune.DType == DuelType.TypeArray[i])
					im = 2154;
				AddImage(20, 30 * i + 50, im); AddLabel(55, 30 * i + 55, 1152, GetType(i));

				im = 2151;
				if (m_Rune.DType == DuelType.TypeArray[i + 4])
					im = 2154;
				AddImage(200, 30 * i + 50, im); AddLabel(235, 30 * i + 55, 1152, GetType(i + 4));
			}
		}

		protected void AddOptions()
		{
			AddBackOptions();
			for (int i = 0; i < 5; i++)
			{
				AddImage(20, 25 * i + 220, Nr(m_Rune.Options[i])); AddLabel(55, 25 * i + 220, 1152, GetOption(i));
			}

			for (int i = 1; i < 5; i++)
			{
				AddImage(200, 25 * i + 220, Nr(m_Rune.Options[i + 4])); AddLabel(235, 25 * i + 220, 1152, GetOption(i + 4));
			}
		}

		public int Nr(bool getcheck){ return getcheck ? 211 : 210; }

		protected void SMS(string str)
		{
			foreach (Mobile m in m_Rune.Participants)
				m.SendMessage(str);
		}

		//2v2
		public string GetName(Mobile m)
		{
			if (m != null)
				return m.Name;

			return "";
		}

		public void AcceptButt(int x, int y, bool accepted)
		{
			if (accepted == true)
				AddImage(x, y, 211);
		}
		//end
	}

	public class DuelStartGump : BaseDuelDataGump
	{
		public DuelStartGump(Mobile starter, Dueller npc) : base(null)
		{
			m_Rune = new DuelRune(starter, npc);

			AddAccept();
			AddBackType();

			for (int i = 0; i < 4; i++)
			{
				AddRadio(20, 30 * i + 50, 2151, 2154, false, i); AddLabel(55, 30 * i + 55, 1152, GetType(i));
				AddRadio(200, 30 * i + 50, 2151, 2154, false, i + 4); AddLabel(235, 30 * i + 55, 1152, GetType(i + 4));
			}

		}

		public DuelStartGump(DuelRune rune) : base(rune)
		{
			m_Rune = rune;

			AddAccept();
			AddType();
			AddBackOptions();

			for (int k = 0; k < 5; k++)
			{
				CustomAdd(20, 25 * k + 245, k); AddLabel(55, 25 * k + 245, 1152, GetOption(k));
			}

			for (int k = 0; k < 4; k++)
			{
				CustomAdd(200, 25 * k + 245, k + 5); AddLabel(235, 25 * k + 245, 1152, GetOption(k + 5));
			}
		}

		private void CustomAdd(int x, int y, int option)
		{
			if (m_Rune.Options[option])
				AddImage(x, y, 211);

			else
				AddCheck(x, y, 210, 211, false, option);
		}

		public override void OnResponse(NetState sender, RelayInfo info)
		{
			Mobile m = sender.Mobile;

			switch (info.ButtonID)
			{
				case 0: break;

				case 1:
					{
						if (m_Rune.DType == null)
						{
							int dueltype = 10;

							for (int i = 0; i < info.Switches.Length; i++)
								dueltype = info.Switches[i];

							if (dueltype == 10)
								break;

							m_Rune.ChangeType(DuelType.TypeArray[dueltype]);

							Dueller.CloseGumps(m);
							m_Rune.Starter.SendGump(new DuelStartGump(m_Rune));
						}

						else
						{
							for (int i = 0; i < info.Switches.Length; i++)
								m_Rune.Options[info.Switches[i]] = true;

							if (m_Rune.DType is DDDuelType)
							{
								Dueller.CloseGumps(m);
								m_Rune.Starter.SendGump(new DuelStarterGump(m_Rune));
							}

							else
								m_Rune.Starter.Target = new DuelTarget(m_Rune);
						}
						break;
					}
			}
		}
	}


	public class DuelAcceptTimer : Timer
	{
		private Mobile m_From, m_Target;
		private int m_Type;

		public DuelAcceptTimer( Mobile from, Mobile target, int type ) : base( TimeSpan.FromSeconds( 30.0 ) )
		{
			m_From = from;
			m_Target = target;
			m_Type = type;
		}

		protected override void OnTick()
		{
			CancelDuel();
		}

		public void CancelDuel()
		{
			m_From.SendMessage(String.Format("{0} does not wish to engage into a duel with you.", m_Target.Name));
			m_Target.SendMessage(String.Format("You did not accept the duel challenge from {0}.", m_From.Name));
			m_Target.CloseGump(typeof(DuelAcceptGump));
			//Edit by Silver: Disabled score loss on decline
			//DuelScoreSystem.UpdateScore(m_Target, m_Type);
		}
	}

	public class DuelAcceptGump : BaseDuelDataGump
	{
		private DateTime Active;
		private DuelAcceptTimer m_Timer;

		public DuelAcceptGump(DuelRune rune, DuelAcceptTimer timer) : base(rune)
		{
			m_Rune = rune;
			Active = (DateTime.Now + TimeSpan.FromMinutes(1.0));
			AddType();
			AddOptions();
			AddBackOpponent();
			AddAccept();
			m_Timer = timer;

			int score = DuelScoreSystem.GetScore(m_Rune.Starter, m_Rune.DType.TypeNumber);
			int place = DuelScoreSystem.GetRank(m_Rune.Starter, m_Rune.DType.TypeNumber);

			string rank = "N/A";
			if (place != 0)
				rank = place.ToString();

			string points = "N/A";
			if (score > -99)
				points = score.ToString();

			List<string> arr = new List<string>();

			arr.Add("Name:"); arr.Add(m_Rune.Starter.Name); arr.Add("Raw Strength:"); arr.Add(m_Rune.Starter.RawStr.ToString());
			arr.Add("Points:"); arr.Add(points); arr.Add("Raw Dexterty:"); arr.Add(m_Rune.Starter.RawDex.ToString());
			arr.Add("Rank:"); arr.Add(rank); arr.Add("Raw Intelligence:"); arr.Add(m_Rune.Starter.RawInt.ToString());

			AddTable(20, 430, new int[] { 48, 135, 135, 56 }, arr, new string[] { "ffffff", "ffffff", "ffffff", "ffffff", });
		}

		public override void OnResponse(NetState sender, RelayInfo info)
		{
			Mobile from = sender.Mobile;

			if (m_Rune.Starter.NetState == null)
			{
				from.SendMessage("Your challenger has logged off.");
				m_Timer.Stop();
			}

			else
			{
				switch (info.ButtonID)
				{
					case (int)Buttons.Canc: m_Timer.CancelDuel();
											m_Timer.Stop();
											break;

					case (int)Buttons.Ok:
						{
							if (Active > DateTime.Now)
							{
								if (m_Rune.PassedAliveCheck())
								{
									if (m_Rune.PassedSkillCheck())
									{
										if (m_Rune.PassedBalanceCheck())
										{
											if (m_Rune.PassedCurrentDuellerCheck())
											{
												if (!m_Rune.Npc.Deleted)
												{
													if (!m_Rune.Npc.Running)
													{
														from.SendMessage("You have accepted to duel with your challenger.");
														m_Rune.Starter.SendMessage("Your opponent has accepted your challenge.");
														m_Rune.Npc.Imput(m_Rune);
													}
													else
														SMS("The arena is already being used by other duellers, please wait for it to finish.");
												}
												else
													SMS("The arena is currently not active.");
											}
											else
												SMS("One of the participants is already engaged in a duel.");
										}
										else
											SMS("One of the participants lacks the needed amount of gold.");
									}
									else
										SMS("One of the participants does not have the correct skills for this type of duel.");
								}
								else
									SMS("Both participants must be alive to duel.");
							}
							else
								SMS("You have not accepted the duel in time.");
							m_Timer.Stop();
						}
						break;
				}
			}
		}

		public enum Buttons
		{
			Canc,
			Ok
		}
	}

	public class DuelStarterGump : BaseDuelDataGump
	{
		public DuelStarterGump(DuelRune rune) : base(rune)
		{
			AddBack2v2();
			AddType();
			AddOptions();

			for (int i = 0; i < 3; i++)
			{
				string str = "";

				switch (i)
				{
					case 0: str = "Opponent 1:"; break;
					case 1: str = "Opponent 2:"; break;
					case 2: str = "Ally:"; break;
				}
				AddLabel(20, 440 + i * 20, 1152, str);
				AddLabel(120, 440 + i * 20, 1152, GetName(m_Rune.Participants[i + 1]));
				AcceptButt(325, 440 + i * 20, m_Rune.Accepted[i]);
			}

			//AddLabel(20, 440, 1152, "Opponent 1:");
			//AddLabel(120, 440, 1152, GetName(m_Opp1));
			//AcceptButt(325, 440, m_Acc1);
			//AddLabel(20, 460, 1152, "Opponent 2:");
			//AddLabel(120, 460, 1152, GetName(m_Opp2));
			//AcceptButt(325, 460, m_Acc2);
			//AddLabel(20, 480, 1152, "Ally:");
			//AddLabel(120, 480, 1152, GetName(m_Ally));
			//AcceptButt(325, 480, m_AccA);
			AddLabel(20, 500, 1152, "You:");
			AddLabel(120, 500, 1152,m_Rune.Starter.Name);
			AddImage(325, 500, 211);

			AddButton(350, 440, 4014, 4015, 11, GumpButtonType.Reply, 0);
			AddButton(350, 460, 4014, 4015, 12, GumpButtonType.Reply, 0);
			AddButton(350, 480, 4014, 4015, 13, GumpButtonType.Reply, 0);
			AddImage(350, 500, 4014);

			if (Array.TrueForAll(m_Rune.Accepted, IsTrue))
				AddAccept();

			SendGumps();
		}

		private bool IsTrue(bool bol){ return bol; }

		public void SendGumps()
		{
			for (int i = 1; i < 4; i++)
			{
				Mobile m = m_Rune.Participants[i];
				if (m != null)
				{
					Dueller.CloseGumps(m);
					m.SendGump(new Duel2v2Gump(m_Rune, i));
				}
			}
		}

		public void Cancel()
		{
			foreach (Mobile m in m_Rune.Participants)
			{
				if (m != null)
					Dueller.CloseGumps(m);
			}
		}

		public override void OnResponse(NetState sender, RelayInfo info)
		{
			Mobile from = sender.Mobile;

			switch (info.ButtonID)
			{
				case 0: Cancel(); break;
				case 11: from.Target = new DuelTarget(m_Rune, 1); break;
				case 12: from.Target = new DuelTarget(m_Rune, 2); break;
				case 13: from.Target = new DuelTarget(m_Rune, 3); break;

				case 1:
					Cancel();//whats this??!!
					if (m_Rune.PassedNullCheck())
					{
						if (m_Rune.PassedBalanceCheck())
						{
							if (!m_Rune.Npc.Deleted)
							{
								if (!m_Rune.Npc.Running)
								{
									m_Rune.Npc.Imput(m_Rune);//needs to be changed
									return;
								}

								else
									SMS("The arena is already being used by other duellers, please wait for it to finish.");
							}
							else
								SMS("The arena is currently not active.");
						}
						else
							SMS("One of the participants lacks the needed amount of gold.");
					}
					break;
			}
			Cancel();//why double?
		}

		public enum Buttons
		{
			Canc,
			Ok,
			Add1,
			Add2,
			Add3
		}
	}

	public class Duel2v2Gump : BaseDuelDataGump
	{
		private int m_PartNr;

		public Duel2v2Gump(DuelRune rune, int partnr) : base(rune)
		{
			m_PartNr = partnr;

			AddBack2v2();
			AddType();
			AddOptions();

			string you = "";
			string opp1 = "";
			string opp2 = "";
			string ally = "";

			switch (m_PartNr)
			{
				case 1:
					you = GetName(m_Rune.Participants[1]);
					opp1 = m_Rune.Starter.Name;
					opp2 = GetName(m_Rune.Participants[3]);
					ally = GetName(m_Rune.Participants[2]);

					AcceptButt(325, 440, m_Rune.Accepted[0]);//you
					AcceptButt(325, 460, m_Rune.Accepted[1]);//all
					AddImage(325, 480, 211);//opp1
					AcceptButt(325, 500, m_Rune.Accepted[2]);//opp2

					if (!m_Rune.Accepted[0])
						AddAccept();
					break;

				case 2:
					you = GetName(m_Rune.Participants[2]);
					opp1 = m_Rune.Starter.Name;
					opp2 = GetName(m_Rune.Participants[3]);
					ally = GetName(m_Rune.Participants[1]);

					AcceptButt(325, 440, m_Rune.Accepted[1]);//you
					AcceptButt(325, 460, m_Rune.Accepted[0]);//all
					AddImage(325, 480, 211);//opp1
					AcceptButt(325, 500, m_Rune.Accepted[2]);//opp2

					if (!m_Rune.Accepted[1])
						AddAccept();
					break;

				case 3:
					you = GetName(m_Rune.Participants[3]);
					ally = m_Rune.Starter.Name;
					opp1 = GetName(m_Rune.Participants[1]);
					opp2 = GetName(m_Rune.Participants[2]);

					AcceptButt(325, 440, m_Rune.Accepted[2]);//you
					AddImage(325, 460, 211);//all
					AcceptButt(325, 480, m_Rune.Accepted[0]);//opp1
					AcceptButt(325, 500, m_Rune.Accepted[1]);//opp2

					if (!m_Rune.Accepted[2])
						AddAccept();
					break;
			}


			AddLabel(20, 440, 1152, "You:"); AddLabel(120, 440, 1152, you);
			AddLabel(20, 460, 1152, "Ally:"); AddLabel(120, 460, 1152, ally);
			AddLabel(20, 480, 1152, "Opponent 1:"); AddLabel(120, 480, 1152, opp1);
			AddLabel(20, 500, 1152, "Opponent 2:"); AddLabel(120, 500, 1152, opp2);
		}

		public override void OnResponse(NetState sender, RelayInfo info)
		{
			if (m_Rune.Starter.NetState == null)
			{
				foreach (Mobile m in m_Rune.Participants)
				{
					if (m != null)
					{
						Dueller.CloseGumps(m);
						m.SendMessage("The challenger has logged off.");
					}
				}
			}

			else
			{
				switch (info.ButtonID)
				{
					case 0:
						{
							m_Rune.Accepted[m_PartNr - 1] = false;
							m_Rune.Participants[m_PartNr] = null;

							Dueller.CloseGumps(m_Rune.Starter);
							m_Rune.Starter.SendGump(new DuelStarterGump(m_Rune));
							break;
						}
					case 1:
						{
							m_Rune.Accepted[m_PartNr - 1] = true;

							Dueller.CloseGumps(m_Rune.Starter);
							m_Rune.Starter.SendGump(new DuelStarterGump(m_Rune));
							break;
						}
				}
			}
		}
	}

	public class DuelTarget : Target
	{
		private DuelRune m_Rune;
		private int m_Partnr;

		public DuelTarget(DuelRune rune) : this(rune, 0)
		{
		}

		public DuelTarget(DuelRune rune, int partnr) : base(30, false, TargetFlags.None)
		{
			m_Rune = rune;
			m_Partnr = partnr;
			CheckLOS = false;
		}

		protected override void OnTargetCancel(Mobile from, TargetCancelType cancelType)
		{ ResendOnFailure(); from.SendMessage("Please target someone, or cancel."); }

		protected override void OnCantSeeTarget(Mobile from, object targeted)
		{ ResendOnFailure(); from.SendMessage("You can't see that target."); }

		protected override void OnTargetDeleted(Mobile from, object targeted)
		{ ResendOnFailure(); from.SendMessage("That target has been deleted."); }

		protected override void OnTargetNotAccessible(Mobile from, object targeted)
		{ ResendOnFailure(); from.SendMessage("That target is not accessible."); }

		protected override void OnTargetOutOfRange(Mobile from, object targeted)
		{ ResendOnFailure(); from.SendMessage("That target is out of range."); }

		protected override void OnTargetUntargetable(Mobile from, object targeted)
		{ ResendOnFailure(); from.SendMessage("You can't target that."); }

		protected override void OnTarget(Mobile from, object target)
		{
			from.RevealingAction(); //Lets see who challenged.

			if (target == m_Rune.Starter)
				from.SendMessage("You cannot duel yourself.");
			else if (target is PlayerMobile)
			{
				Mobile targ = (Mobile)target;

				DuelData targdata;
				DuelScoreSystem.DuelDataDictionaryArray[m_Rune.DType.TypeNumber].TryGetValue(targ, out targdata);

				if (!m_Rune.Npc.FactionOnly || Faction.Find(targ) != null)
				{
					if (!((PlayerMobile)targ).Young)
					{
						if (!m_Rune.Npc.Deleted && targ.InRange(m_Rune.Npc, 15))
						{
							if (m_Rune.Npc.Participants != null && !(Array.BinarySearch(m_Rune.Npc.Participants, targ) < 0))
								from.SendMessage("That opponent is already duelling.");
							else if (targ.HasGump( typeof( DuelAcceptGump )) )
								from.SendMessage("That opponent is currently considering another challenge. Please try again later.");
							// else if (targdata != null && targdata.Points < -5)
								// from.SendMessage("That character is probably not interested in this type of duel.");
							else
							{
								from.SendMessage("You have challenged your opponent, please wait for their reaction.");
								if (m_Partnr == 0)
								{
									Targeting.Target.Cancel(targ); //added by Blady - let's see if it will correctly cancel the target.
									Dueller.CloseGumps(targ);
									m_Rune.Participants[1] = targ;
									DuelAcceptTimer timer = new DuelAcceptTimer(from, targ, m_Rune.DType.TypeNumber);
									timer.Start();
									targ.SendGump(new DuelAcceptGump(m_Rune, timer));
									return;
								}
								else
								{
									if (!(Array.BinarySearch(m_Rune.Participants, targ) < 0))
										from.SendMessage("You cannot duel any of the other participants that you have already challenged.");

									else
									{
										m_Rune.Participants[m_Partnr] = targ;

										Dueller.CloseGumps(from);
										from.SendGump(new DuelStarterGump(m_Rune));
										return;
									}
								}
							}
						}
						else // More than 15 tiles for dueller, lets not bother the player with a gump.
							from.SendMessage("That opponent is too far away from the arena.");
					}
					else // too young
						from.SendMessage("That opponent is too young.");

				}
				else// Not in faction
					from.SendMessage("This Duel Arena is only open to faction members.");
			}
			else // Not Mobile
				from.SendMessage("You cannot duel that!");

			ResendOnFailure();
		}

		private void ResendOnFailure()
		{
			Dueller.CloseGumps(m_Rune.Starter);
			if (m_Partnr != 0)
				m_Rune.Starter.SendGump(new DuelStarterGump(m_Rune));

			else
				m_Rune.Starter.SendGump(new DuelStartGump(m_Rune));
		}
	}

	public class DuelEndGump : BaseDuelGump
	{
		public DuelEndGump(bool Winner, bool Draw)
			: base()
		{
			AddAccept();
			AddBackground(0, 0, 403, 200, 9270);

			string htmltext;
			if (Draw)
			{
				htmltext = "This duel has turned into a draw, you will have to try a lot harder next time. For no fame will be entitled to the one that cannot win.<br>You will receive a bankcheck with your inlay minus 30% the commission that is entitled to the Duel Master for his work.";
				AddLabel(30, 20, 1161, "A Draw");
			}

			else if (Winner)
			{
				htmltext = String.Format("You have won this duel, and are hereby entitled with the prize of {0} gp. ", (DuelSystem.GoldCost * 0.4).ToString()) +
										"It will be rewarded to you in the form of a bankcheck. Of course you have also won respect from your opponent, until you find your superior.";
				AddLabel(30, 20, 1161, "Victory!");
			}

			else
			{
				htmltext = "You have lost this duel!<br>How much worse can it get? All of your enemies will laugh at you when you face them.<br>You must win your next duel to keep your dignity in front of your friends. You will not receive any money, and have lost all your inlay.";
				AddLabel(30, 20, 1161, "Defeat!");
			}
			AddHtml(26, 52, 351, 116, htmltext, true, false);
		}
	}

	public class DuelScoreGump : AdvGump
	{
		int m_Page;
		int m_Type;
		bool canCloseGump = true;

		public DuelScoreGump() : base()
		{
			Closable = true;
			Disposable = true;
			Dragable = true;
			Resizable = false;

			AddPage(0);

		}

		public DuelScoreGump(Mobile from) : this()
		{
			canCloseGump = true;
			AddBackground(0, 0, 380, 255, 9300);
			AddBackground(64, 17, 227, 37, 9300);

			ArrayList dueldata = new ArrayList();
			List<string> dataarr = new List<string>();
			string[] colorarr = new string[] { "333333", "333333", "0000FF", "FF0000", "006600" };
			int[] collumnarr = new int[] { 125, 52, 52, 52, 52 };

			dataarr.Add("Type:");
			dataarr.Add("Rank:");
			dataarr.Add("Score:");
			dataarr.Add("Losses:");
			dataarr.Add("Wins:");

			for (int i = 0; i < 7; i ++)
			{
				switch (i)
				{
					case 0: dataarr.Add("Normal Duel"); break;
					case 1: dataarr.Add("Normal Mage Fight"); break;
					case 2: dataarr.Add("True Mage Fight"); break;
					case 3: dataarr.Add("Ultimate Mage Fight"); break;
					case 4: dataarr.Add("Normal Dex Fight"); break;
					case 5: dataarr.Add("True Dex Fight"); break;
					case 6: dataarr.Add("Ultimate Dex Fight"); break;
				}

				DuelData dd = null;
				if (!DuelScoreSystem.DuelDataDictionaryArray[i].TryGetValue(from, out dd))
				{
					dataarr.Add("n/a");
					for (int j = 1; j < 4; j++)
						dataarr.Add("0");
				}
				else
				{
					dataarr.Add(dd.Rank.ToString());
					dataarr.Add(dd.Points.ToString());
					dataarr.Add(dd.Losses.ToString());
					dataarr.Add(dd.Wins.ToString());
				}

				AddButton(5, 78 + i * 18, 5601, 5601, i + 10, GumpButtonType.Reply, 0);
			}

			AddTable(25, 60, collumnarr, dataarr,colorarr, 1, 2);
			//AddTable(25, 59, collumnarr, dataarr, colorarr, 3, 3);
			AddHtml(75, 25, 218, 20, Center("Personal Statistics"), false, false);
		}

		public DuelScoreGump(int type, int page) : this()
		{
			canCloseGump = false;
			AddBackground(0, 0, 355, 530, 9300);
			AddBackground(64, 17, 227, 37, 9300);
			m_Type = type;
			m_Page = page;

			List<string> dataarr = new List<string>();
			string[] colorarr = new string[] { "333333", "333333", "333333", "333333", "333333" };
			int[] collumnarr = new int[] { 48, 120, 48, 56, 48 };

			List<DuelData> list = DuelScoreSystem.DuelDataListArray[type];

			int listcount = list.Count;
			int maxpages = (int)(listcount / 20);
			if (page < 0)
				m_Page = 0;
			if (page > maxpages)
				m_Page = maxpages;
			int start = m_Page * 20;
			int max = start + 20;

			if (listcount < max)
					max = listcount;

			dataarr.Add("Rank:");
			dataarr.Add("Name:");
			dataarr.Add("Score:");
			dataarr.Add("Losses:");
			dataarr.Add("Wins:");

			if (listcount != 0)
			{
				for (int i = start; i < max; i++)
				{
					DuelData dd = (DuelData)list[i];
					dataarr.Add(dd.Rank.ToString());
					if (dd.Part != null)
						dataarr.Add(dd.Part.Name);
					else
						dataarr.Add("Anonymous");
					dataarr.Add(dd.Points.ToString());
					dataarr.Add(dd.Losses.ToString());
					dataarr.Add(dd.Wins.ToString());
				}
			}

			AddTable(20, 70, collumnarr, dataarr, colorarr, 5, 3);
			AddButton(15, 498, 9772, 9770, 100, GumpButtonType.Reply, 0);
			AddButton(315, 498, 9773, 9771, 101, GumpButtonType.Reply, 0);
			AddHtml(68, 25, 218, 20, Center( GetType(type) ), false, false);
			AddHtml(0, 498, 355, 486, Center(String.Format("{0}/{1}", m_Page + 1, maxpages + 1)), false, false);
		}

		private string GetType(int type)
		{
			switch (type)
			{
				case 0: return "Normal Duel";
				case 1: return "Normal Mage Fight";
				case 2: return "True Mage Fight";
				case 3: return "Ultimate Mage Fight";
				case 4: return "Normal Dex Fight";
				case 5: return "True Dex Fight";
				case 6: return "Ultimate Dex Fight";
				case 7: return "2 versus 2";
			}
			return "";
		}

		public override void OnResponse(NetState sender, RelayInfo info)
		{
			Mobile from = sender.Mobile;

			from.CloseGump(typeof(DuelScoreGump));
			switch (info.ButtonID)
			{
				case 0: if (canCloseGump) {break;}
					from.CloseGump(typeof(DuelScoreGump));
					from.SendGump(new DuelScoreGump(from));
					break;
				case 10: from.SendGump(new DuelScoreGump(0, 0)); break;
				case 11: from.SendGump(new DuelScoreGump(1, 0)); break;
				case 12: from.SendGump(new DuelScoreGump(2, 0)); break;
				case 13: from.SendGump(new DuelScoreGump(3, 0)); break;
				case 14: from.SendGump(new DuelScoreGump(4, 0)); break;
				case 15: from.SendGump(new DuelScoreGump(5, 0)); break;
				case 16: from.SendGump(new DuelScoreGump(6, 0)); break;
				case 100: from.SendGump(new DuelScoreGump(m_Type, m_Page - 1)); break;
				case 101: from.SendGump(new DuelScoreGump(m_Type, m_Page + 1)); break;
			}
		}
	}

	public class DuelSetupGump : Gump
	{
		private Dueller m_Dueller;

		public DuelSetupGump(Dueller dueller) : base(30, 30)
		{
			m_Dueller = dueller;

			Closable = true;
			Disposable = true;
			Dragable = true;
			Resizable = false;
			AddPage(0);
			AddBackground(0, 0, 450, 376, 3600);
			AddBackground(100, 22, 250, 80, 3600);
			AddHtml(115, 41, 220, 50, "", false, false);
			AddLabel(25, 125, 0x480, "Options");
			AddBackground(25, 150, 375, 89, 9270);
			AddLabel(45, 170, 0x480, "Wall ID:");
			AddLabel(45, 195, 0x480, "Wall Hue:");
			AddAlphaRegion(110, 195, 200, 20);
			AddAlphaRegion(110, 170, 200, 20);
			AddTextEntry(110, 170, 200, 20, 0x480, 1, dueller.WallID.ToString());
			AddTextEntry(110, 195, 200, 20, 0x480, 2, dueller.WallHue.ToString());
			AddButton(30, 245, 4005, 4006, 1, GumpButtonType.Reply, 0);
			AddLabel(70, 245, 0x480, "Confirm Options");
			AddButton(30, 275, 4005, 4006, 2, GumpButtonType.Reply, 0);
			AddLabel(70, 275, 0x480, "Setup Locations");
			AddButton(30, 305, 4005, 4006, 3, GumpButtonType.Reply, 0);
			AddLabel(70, 305, 0x480, "Props");
			AddButton(30, 335, 4005, 4006, 0, GumpButtonType.Reply, 0);
			AddLabel(70, 335, 0x480, "Cancel");
		}

		public override void OnResponse(NetState sender, RelayInfo info)
		{
			Mobile from = sender.Mobile;

			switch (info.ButtonID)
			{
				case 0: break;
				case 1:
					for (int i = 1; i < 3; i++)
					{
						TextRelay text = info.GetTextEntry(i);

						try
						{
							int value;
							if (text != null)
								switch (i)
								{
									case 1:
										if (Int32.TryParse(text.Text, out value))
											m_Dueller.WallID = value;
										else
											from.SendMessage("Wrong Format.");
										break;
									case 2:
										if (Int32.TryParse(text.Text, out value))
											m_Dueller.WallHue = value;
										else
											from.SendMessage("Wrong Format.");
										break;
								}
						}
						catch
						{
							from.SendMessage("Bad format. An integer was expected.");
						}
					}
					from.SendGump(new DuelSetupGump(m_Dueller));
					break;
				case 2: from.Target = new DuelSetupTarget(m_Dueller, from); break;
				case 3: from.SendGump(new PropertiesGump(from, m_Dueller)); break;
			}
		}

		private class DuelSetupTarget : Target
		{
			private List<Point3D> m_List;
			private Mobile m_Mob;
			private Dueller m_Dueller;

			public DuelSetupTarget(Dueller dueller, Mobile m) : this(dueller, m, new List<Point3D>())
			{
			}

			public DuelSetupTarget(Dueller dueller, Mobile m, List<Point3D> list) : base(-1, true, TargetFlags.None)
			{
				m_Dueller = dueller;
				m_List = list;
				m_Mob = m;
				m.SendMessage(Message());
			}

			private string Message()
			{
				switch (m_List.Count)
				{
					case 0: return "Please target the spectator location.";
					case 1: return "Please target the wall location.";
					case 2: return "Please target the start location of the starter.";
					case 3: return "Please target the start location of the opponent.";
					case 4: return "Please target the start location of the ally.";
					case 5: return "Please target the start location of the second opponent.";
					case 6: return "Please target the end location of the starter.";
					case 7: return "Please target the end location of the opponent.";
					case 8: return "Please target the end location of the ally.";
					case 9: return "Please target the end location of the second opponent.";
				}
				return "An error has occured, please cancel your actions";
			}

			protected override void OnTarget(Mobile from, object o)
			{
				IPoint3D p = o as IPoint3D;

				if (p != null)
				{
					if (p is Item)
						p = ((Item)p).GetWorldTop();
					else if (p is Mobile)
						p = ((Mobile)p).Location;

					m_List.Add(new Point3D(p));
				}

				if (m_List.Count <= 9)
					from.Target = new DuelSetupTarget(m_Dueller, from, m_List);

				else
				{
					m_Dueller.Locations = m_List.ToArray();
					from.SendMessage("All locations have been set.");
				}
			}
		}

	}
}