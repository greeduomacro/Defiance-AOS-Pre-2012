using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Regions;
using Server.Commands;
using Server.Gumps;

namespace Server.Events.CTF
{
	public class CTFRewardGump : Gump
	{
		public static void Initialize()
		{
			CommandSystem.Register("ShowCTFRewards", AccessLevel.Player, new CommandEventHandler(ShowRewards_Command));
		}

		private static void ShowRewards_Command( CommandEventArgs e )
		{
			if( e.Mobile.Region is CTFGameRegion )
				e.Mobile.SendGump( new CTFRewardGump( e.Mobile, 0 ) );

			else
				e.Mobile.SendMessage("You can only use this command at the CTF Arena.");
		}

		public class CTFRewardInfo
		{
			public Type m_tItemType;
			public string m_sText;
			public int m_iItemID, m_iXOffset, m_iYOffset;

			public CTFRewardInfo( Type type, int itemID, int xOffset, int yOffset, string text )
			{
				m_tItemType = type;
				m_sText = text;
				m_iItemID = itemID;
				m_iXOffset = xOffset;
				m_iYOffset = yOffset;
			}
		}

		/// <summary>
		/// Important! New rewards MUST be added to the end of this array.
		/// </summary>
		public static CTFRewardInfo[] m_ctfRewards = new CTFRewardInfo[]
			{
				new CTFRewardInfo( typeof( CTFRewardKatana ), 0x13FF, 0, 20, "A Katana with...<br>30% Damage Increase.<br>Spell Channeling<br>Mage Weapon -10" ),
				new CTFRewardInfo( typeof( CTFRewardWarHammer ), 0x1439, 0, 20, "A War Hammer with...<br>30% Damage Increase.<br>Spell Channeling<br>Mage Weapon -10" ),
				new CTFRewardInfo( typeof( CTFRewardWarFork ), 0x1405, 0, 20, "A War Fork with...<br>30% Damage Increase.<br>Spell Channeling<br>Mage Weapon -10" ),
				//Disabled by Silver new CTFRewardInfo( typeof( CTFRewardBracelet ), 0x1086, 10, 20, "A Bracelet with...<br>10% Enhance Potions" ),
				new CTFRewardInfo( typeof( CTFRewardBow ), 0x13B2, 0, 20, "A Bow with...<br>30% Damage Increase.<br>Spell Channeling<br>Mage Weapon -10" ),
				new CTFRewardInfo( typeof( CTFSpellBook ), 0xEFA, 0, 20, "A Spell Book with...<br>10% Spell Damage Increase.<br>All Spells." ),
		};


		private static int m_Chosen(Mobile m)
		{
			if (CTFGame.Running)
			{
				CTFPlayerGameData pgd = CTFGame.GameData.GetPlayerData(m);
				if (pgd != null)
					return pgd.RewardsChosen;
			}
			return 0;
		}

		private static int m_Possible(Mobile m)
		{
			CTFPlayerData pd = CTFData.GetPlayerData(m);
			if (pd != null)
				return pd.Rank / 4;
			return 0;
		}

		public int m_iLoc;
		public virtual int AmountToGet{ get{ return 1; } }

		public CTFRewardGump( Mobile from, int loc ) : base( 150, 150 )
		{
			m_iLoc = loc;

			AddPage(0);
			AddBackground(0, 0, 426, 259, 2620);
			AddAlphaRegion(6, 6, 414, 244);
			AddHtml(145, 15, 157, 15, HtmlUtility.Color("<center>Choose a reward</center>", HtmlUtility.HtmlYellow), (bool)false, (bool)false);

			AddHtml(145, 42, 170, 15, HtmlUtility.Color(string.Format("Rewards left to choose: {0}", (m_Possible(from) - m_Chosen(from))), HtmlUtility.HtmlYellow), (bool)false, (bool)false);
			AddHtml(145, 62, 170, 15, HtmlUtility.Color(string.Format("Rewards used: {0}", m_Chosen(from)), HtmlUtility.HtmlYellow), (bool)false, (bool)false);

			AddButton(374, 221, 4023, 4024, (int)Buttons.OKButton, GumpButtonType.Reply, 0);

			if (loc + AmountToGet < m_ctfRewards.Length)
				AddButton(274, 100, 9762, 9763, (int)Buttons.Next, GumpButtonType.Reply, 0);

			if (loc > 0)
				AddButton(148, 100, 9766, 9767, (int)Buttons.Prev, GumpButtonType.Reply, 0);

			for (int i = 0; i < AmountToGet; i++)
			{
				if (i + loc > m_ctfRewards.Length - 1)
					break;

				AddItem(186 + (60 * i) + ((CTFRewardInfo)m_ctfRewards[i + loc]).m_iXOffset, 82 + ((CTFRewardInfo)m_ctfRewards[i + loc]).m_iYOffset, ((CTFRewardInfo)m_ctfRewards[i + loc]).m_iItemID);

				AddHtml(70, 165, 286, 79, ((CTFRewardInfo)m_ctfRewards[i + loc]).m_sText, (bool)true, (bool)true);
			}
		}

		public bool HasRewards( Mobile player )
		{
			if(CTFGame.Running)
				return ((m_Possible(player) - m_Chosen(player)) > 0);

			return false;
		}

		public enum Buttons
		{
			CloseButton,
			Next,
			Prev,
			OKButton
		}

		public virtual void OnRequestGump( Mobile from, int loc )
		{
			from.SendGump( new CTFRewardGump( from, loc ) );
		}

		public override void OnResponse( Server.Network.NetState sender, RelayInfo info )
		{
			Mobile from = sender.Mobile;

			switch (info.ButtonID)
			{
				case (int)Buttons.OKButton:

					from.CloseGump(typeof(CTFRewardGump));

					if (!HasRewards(from))
						from.SendMessage("You do not have enough rewards left.");

					else
					{
						List<Item> itemlist;
						if (SunnySystem.ArmedDictionary.TryGetValue(from, out itemlist) && from.Backpack != null)
						{
							CTFGame.GameData.GetPlayerData(from).RewardsChosen++;
							Item item = (Item)Activator.CreateInstance(((CTFRewardGump.CTFRewardInfo)CTFRewardGump.m_ctfRewards[m_iLoc]).m_tItemType);
							from.Backpack.AddItem(item);
							itemlist.Add(item);
							from.SendLocalizedMessage(1072223); // An item has been placed in your backpack.
						}
					}

					// Open again if player has enough rewards
					if (HasRewards(from))
						OnRequestGump(from, m_iLoc);

					return;
				case (int)Buttons.CloseButton:
					return;
				case (int)Buttons.Next:
					OnRequestGump(from, m_iLoc + AmountToGet);
					return;
				case (int)Buttons.Prev:
					OnRequestGump(from, m_iLoc - AmountToGet);
					return;
			}
		}
	}
}