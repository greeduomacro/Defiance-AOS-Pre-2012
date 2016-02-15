using System;
using System.Collections.Generic;
using Server.Network;
using Server.Gumps;
using Server.Accounting;
using Server.Mobiles;
using Server.Commands;


namespace Server.Misc
{
	public static class ToSRulesChecker
	{
		public static int Version = 0; // Increment on new rules

		private static string m_Rules = @"1.) Insults and Harassment [J]
No racial slurs or harassing, including but not limited to, following a player repeatedly to spam insults. Insulting someone in any way, especially after you have been warned about it, will surely result in some jail time if witnessed by a staff member.
Of course, staff are not babysitters, so do not expect to have someone banned just because they said a single bad word.

2.) Amount of clients [J]
No more than 1 client online in arena events such as tournaments, capture the flags, last man standing etc. Outside events you are allowed to have as many clients as you need.

3.) Trapping
Trapping players or pets is legal, though it is not encouraged. If you are gating people to a closed area and killing them, you are encouraged but not obliged to gate them back after that. If you get killed after entering such a gate, it is all your fault to have entered it.

4.) Littering [J]
No placing large amounts of items on the ground in order to block roads or to cause lag. Please use the trash chests or trash barrels.

5.) Bug Exploiting [B]
No exploiting/abuse of ingame bugs. Should you discover a bug we ask you kindly to report it at our forums.

6.) Advertisement [J]/[B]
No advertising of other servers or products.

7.) Third party programs [J]/[B]
No 3rd party programs, that includes but is not limited to PlayUO & Speedhacks of any type, however we do permit the use of Razor and EasyUO.

8.) Hacking [B]
No hacking or guessing passwords tolerated. However if you share your account info with anyone, it is all your fault when you find your items missing.

9.) Unattended Macroing [J]/[E1]/[B]
Collecting resources while NOT watching your character (unattended macroing) is not allowed. This rule applies to all resources (ore, wood, cotton, gold etc.). You are allowed to use scripts and macros, as long as you are watching your character all the time.
However, training skills with unattended macroing is allowed.

10.) Bod collecting [B]
You are allowed to have no more than 2 characters that can collect tailoring bods and 2 characters that can collect blacksmith bods. You can collect Hunter BODs with all your characters, as long as you are not creating characters especially for collecting BODs.

11.) Multi-Factioning [J]
You are allowed to be in only one faction at the same time. The only exception is when making a faction switch, which should only take a short time.

12.) Disrespect towards staffmembers [J]
We do not tolerate any disrespecting actions towards any of our staffmembers, this includes badmouthing.

13.) Others
Any other actions intending to spoil the game of other players may be punished according to staff's personal judgement. If you believe that staff's decision is wrong, you may contact higher staff with a detailed explanation of what happened.


Having an account within Defiance Network is a privilege, not a right. This privilege may be withdrawn at any time by any of the Defiance staff, should they find you breaking these simple rules, or in general spending your time being a problem to the staff/shard. The punishment as stated at each rule is a guideline and may vary when your situation is being reviewed. Consecutively breaking one of the jailable rules will automatically result in a ban.

By playing on one of our game servers or participating on one of our forums you automatically agree on our terms of service and rules.

You can review the shard rules at any time by using the [ShardRules command.
";

		public static string Rules { get { return m_Rules; } }

		public static void Configure()
		{
			ColorizeMessage();
			EventSink.CharacterCreated += new CharacterCreatedEventHandler(OnCharacterCreated);
			EventSink.Login += new LoginEventHandler(OnLogin);
			CommandSystem.Register("ShardRules", AccessLevel.Player, new CommandEventHandler(OnCommand));
			CustomSaving.AddSaveModule(new SaveData(new DC.SaveMethod(Serialize), new DC.LoadMethod(Deserialize)), "tosandrules");
		}

		//m_Rules.Replace("[w]", GetBaseFont(0));
		//m_Rules.Replace("[b]", GetBaseFont(1));
		//m_Rules.Replace("[r]", GetBaseFont(2));
		//m_Rules.Replace("[y]", GetBaseFont(3));
		//m_Rules.Replace("[/]", endbasefont);

		private static void ColorizeMessage()
		{
			string endbasefont = "<basefont color=#ffffff>";

			m_Rules = m_Rules.Replace("[J]", GetBaseFont(3) + "[Jail]" + endbasefont);
			m_Rules = m_Rules.Replace("[B]", GetBaseFont(2) + "[Ban]" + endbasefont);
			m_Rules = m_Rules.Replace("[E1]", GetBaseFont(2) + "[Resources Removal]" + endbasefont);

			//m_Rules = GetBaseFont(0) + m_Rules + endbasefont;
		}

		private static string GetBaseFont(int color)
		{
			string htmlcolor = "";

			switch(color)
			{
				case 0: htmlcolor = "ffffff";break;//white
				case 1: htmlcolor = "00ffff";break;//blue
				case 2: htmlcolor = "ff0000";break;//red
				case 3: htmlcolor = "ffff00";break;//yellow
			}

			return String.Format("<basefont color=#{0}>", htmlcolor);
		}

		private static void OnCharacterCreated(CharacterCreatedEventArgs args)
		{
			Timer.DelayCall(TimeSpan.FromMinutes(1.5), new TimerStateCallback(SendGumpTo), args.Mobile);
		}

		[Usage("ShardRules")]
		[Description("Display the shardrules")]
		private static void OnCommand(CommandEventArgs args)
		{
			Mobile m = args.Mobile;

			if (m != null)
				m.SendGump(new ToSGump());
		}

		private static void OnLogin(LoginEventArgs args)
		{
			Mobile m = args.Mobile;
			Account acct = (Account)m.Account;

			if (m != null && !Convert.ToBoolean(acct.GetTag("ToS_accepted")))
				m.SendGump(new ToSGump());
		}

		private static void SendGumpTo(object o)
		{
			Mobile m = (Mobile)o;

			if (m != null)
				m.SendGump(new ToSGump());
		}

		public static void SetAccepted(Mobile m)
		{
			Account acct = m.Account as Account;
			if (acct != null)
				acct.SetTag("ToS_accepted", "true");
		}

		public static void NewVersion()
		{
			foreach (Mobile m in World.Mobiles.Values)
			{
				if (m == null || !(m is PlayerMobile))
					continue;

				PlayerMobile pm = (PlayerMobile)m;
				Account acct = (Account)m.Account;
				if (acct == null)
					continue;

				acct.SetTag("ToS_accepted", "false");
			}
		}

		#region Serialising
		private static void Serialize(GenericWriter writer)
		{
			writer.Write(Version);//version
		}

		private static void Deserialize(GenericReader reader)
		{
			int version = reader.ReadInt();

			if (version != Version)
				NewVersion();
		}
		#endregion
	}

	public class ToSGump : AdvGump
	{
		public ToSGump()
			: base(60, 30)
		{
			Closable = false;
			Disposable = false;
			Dragable = false;
			Resizable = false;
			AddPage(0);

			AddBackground(0, 0, 550, 460, 3600);
			AddBackground(100, 20, 350, 50, 3600);
			AddHtml(0, 40, 550, 18, Colorize(Center("Defiance AOS Terms of Service and Rules"), "ffffff"), false, false);
			AddBackground(40, 75, 470, 335, 3600);
			AddAlphaRegion(55, 95, 435, 300);
			AddHtml(55, 95, 435, 300, ToSRulesChecker.Rules, false, true);
			AddCheck(40, 420, 210, 211, false, 0);
			AddLabel(70, 420, 1152, "I have read and accepted the ToS/Rules");
			AddButton(440, 420, 247, 248, 1, GumpButtonType.Reply, 0);
		}
		public override void OnResponse(NetState sender, RelayInfo info)
		{
			bool id = info.ButtonID == 1;
			bool hasaccepted = info.IsSwitched(0);

			if (id && hasaccepted)
				ToSRulesChecker.SetAccepted(sender.Mobile);

			else
				sender.Mobile.SendGump(this);
		}
	}
}