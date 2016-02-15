using System;
using Server;
using Server.Gumps;
using Server.Network;

namespace Server.Misc
{
	public class SaveGump
	{
		public static void ShowSaveGump()
		{
			for ( int i = 0; i < NetState.Instances.Count; ++i )
			{
				Mobile m = ((NetState)NetState.Instances[i]).Mobile;
				if( m != null && !m.Deleted && m.NetState != null && m.Player )
					m.SendGump( new ESaveGump() );
			}
		}

		public static void CloseSaveGump()
		{
			for ( int i = 0; i < NetState.Instances.Count; ++i )
			{
				Mobile m = ((NetState)NetState.Instances[i]).Mobile;
				if( m != null && !m.Deleted && m.NetState != null && m.Player )
					m.CloseGump( typeof( ESaveGump ) );
			}
		}
	}
}

namespace Server.Gumps
{
	public class ESaveGump : Gump
	{
		private string GetRandomHint(){return m_sHints[Utility.Random( m_sHints.Length )];}
		private static string[] m_sHints = new string[]
		{
			"Using the voting link below once a week, you help to increase the shard's playerbase.",
			"Using the donation link will provide you with many useful options.",
			"You must set a MagicWord and an email for your account at our homepage.",
			"Developers read and post at the DefianceUO forums daily.",
			"In factions you can win control of whole cities, which increases the gold you get when you kill enemies.",
			"In guild wars you get more insurance gold than usual.",
			"When you use the Help menu to send a message to staff, it is NOT stored after you logout.",
			"There are a few useful books in the Britain Library. You can find info about quests there.",
			"If you write a book and give it to any staff member, it may be added to the Britain Library for anyone to read.",
			"There are maps for quick access in the Britain Library. You can find maps of Felucca, Ilshenar, Malas and the Lost Lands.",
			"Every time you place a boat two keys are given to you - 1 in backpack and 1 in your bankbox.",
			"You may hire NPCs from towns. They are cheap but helpful.",
			"You can collect Hunter BODs, just like tailor and blacksmith BODs. There is a Hunter in Britain Furtrader's Shop.",
			"You can buy bones from the Hunter in Britain or in Trinsic.",
			"You can cut some bone piles into bones and use them for crafting bone armor.",
			"Using a skinning knife on an animal corpse will place the cut leather directly into your backpack.",
			"After the ALL COME command your pets run as fast as you do. They move much slower after the ALL GUARD command.",
			"Pets are stabled automatically on server load, even pet-ghosts are (if you have free slots in stable).",
			"Use the [servertime command to see the current server time (GMT).",
			"The clocks at Britain Bank and the Duel Arena show the current Server Time (GMT).",
			"There is a daily Pentagram event at Luna at 19:15 GMT.",
			"There is a daily CTF game at 18:30 GMT.",
			"You don't need any equipment to join the CTF game. All you need is 600 total skills and you will be automatically equipped.",
			"You can exit the CTF game before it has started if you want by double-clicking the CTF Exit Stone.",
			"You can see the score of all recent CTF games in the CTF Game Score Board at Britain Bank.",
			"Almost all questions you may think of, are answered in our forums.",
			"Every Saturday morning from 9 to 10 GMT there is a Server Wars event. Anything that happens then is reverted after the server restart.",
			"Houses decay after 3 months of inactivity of the account, unless you use a permanent membership ticket from our Donation Centre.",
			"There is a Market Place in Trinsic and everyone can rent a vendor there.",
			"You can win Copper Bars from events and buy a reward of your choice with them.",
			"There is a casino just North-East of Britain Bank.",
			"There are Poker Slots in the Casino next to Britain Bank.",
			"There is an Event Reward Vendor downstairs in the Casino next to Britain Bank.",
			"We recommend you to use up-to-date client version. When you update your UO client, you should update Razor as well.",
			"We recommend you to use up-to-date client version. If you use an old client version (older than 6.0.1), you will not be able to see the correct map.",
			"A random tip shows up every time when the server is saving ;)",
			"The Pentagram event has a small chance of spawning Barracoon instead of reward.",
			"If you collect 10 statues with your name from the Pentagram event, you can have them replaced with one in special color.",
			"There is a special Duel Arena and it opens two times a day. There are stone-teleporters to it at Britain Bank and Buccaneer's Den",
			"You have a small chance to receive a special artifact when completing the Cursed Cave quest.",
			"There is a Cursed Cave to the north of Minoc Mines.",
			"The Elder Wizard in Buccaneer's Den may give you a special reagent bag that can be easily refilled.",
			"You can rent a house in-town if you have a forum subscription.",
			"You get a special Talisman if you have a forum subscription.",
			"The Defiance Talisman will be yours forever if you are a forum subscriber for at least 3 months.",
			"You can change the names of all kinds of containers by selecting Inscribe from their context menu.",
			"You have a small chance to find Blood Pentagram parts from Blood Elementals in Felucca.",
			"When you double-click a Blood Pentagram part, it shows you which number the part is.",
			"All runic tools now have a chance for applying properties with 100% intensity.",
			"There is a Faction Dungeon and you can go there through your faction Stronghold.",
			"Using the New Player Ticket, you can get a full spellbook and a bag of reagents.",
			"You gain skills 50% faster in your Power Hour. That is the first hour you play every 24 hours."
		};

		private static string[] Links = new string[]
		{
			"http://www.gamesites200.com/ultimaonline/in.php?id=1298",
			"http://www.defianceuo.com/donationcart/cart.php?target=category&category_id=249",
			"http://www.defianceuo.com/forums/forumdisplay.php?f=158",
			"http://www.mydotdot.com/dfiwiki/index.php?title=Main_Page"
		};

		private static string[] Descriptions = new string[]
		{
			"Ultima Online Top 200 Voting",
			"Defiance AOS Donations",
			"Defiance AoS Forums",
			"Defiance Wiki site"
		};

		public ESaveGump() : base( 0, 0 )
		{
			this.Closable=false;
			this.Disposable=false;
			this.Dragable=false;
			this.Resizable=false;

			this.AddPage(0);

			//this.AddBackground(312, 269, 381, 295, 9200);
			//this.AddBackground(349, 253, 301, 32, 9200);
			//this.AddLabel(477, 259, 152, @"Defiance");
			//this.AddImage(660, 226, 10410);
			//this.AddImage(262, 463, 10402);
			//this.AddLabel(444, 295, 152, @"The world is saving...");
			//this.AddLabel(467, 318, 152, @"Please wait...");
			//this.AddBackground(335, 382, 342, 71, 9450);
			//this.AddLabel(340, 366, 152, @"Did you know that...");
			//this.AddLabel(340, 465, 152, @"Links");
			//this.AddBackground(334, 481, 342, 72, 9450);
			//this.AddHtml( 345, 389, 321, 57, GetRandomHint(), (bool)false, (bool)false);



			this.Closable=false;
			this.Disposable=false;
			this.Dragable=true;
			this.Resizable=false;
			this.AddPage(0);
			this.AddBackground(198, 123, 381, 318, 9390);
			this.AddLabel(362, 158, 0, @"Defiance");
			this.AddImage(532, 91, 10410);
			this.AddImage(164, 342, 10402);
			this.AddLabel(325, 177, 0, @"The world is saving...");
			this.AddLabel(348, 198, 0, @"Please wait...");
			this.AddBackground(237, 246, 309, 64, 9350);
			this.AddLabel(226, 220, 0, @"Did you know that...");
			this.AddLabel(228, 318, 0, @"Links");
			this.AddBackground(256, 339, 275, 69, 9350);


			this.AddHtml( 239, 248, 321, 57, GetRandomHint(), (bool)false, (bool)false);

			for ( int i = 0; i < Links.Length; i++ )
				this.AddHtml( 266, 345 + 16 * i, 321, 30, String.Format( "<a href=\"{0}\">{1}</a>", Links[i], Descriptions[i] ), (bool)false, (bool)false);
		}
	}
}