using System;
using Server;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.Quests.Plauge
{
	public class AcceptConversation : QuestConversation
	{
		public override object Message
		{
			get
			{
				return "You have accepted Zuleika's help. She requires 200 Poison fungus to create a spell that can teleport you and your party to the Alternate Dimension.<BR><BR>You may hand Zuleika the fungus as you collect them and she will keep count of how many you have brought her.<BR><BR>Poison fungus can be collected from (Plagued) creatures, these creatures can be found in all the dungeons in britannia.<BR><BR>Good luck.";
			}
		}

		public AcceptConversation()
		{
		}

		public override void OnRead()
		{
			System.AddObjective( new CollectFungusObjective() );
		}
	}

	public class DontOfferConversation : QuestConversation
	{
		public override object Message
		{
			get{return "<I>The gray skinned necromancer looks at you and says...</I><BR><BR>You seem to be busy with another quest. Come and speak with me again when you have completed your present quest.";}
		}
		public override bool Logged{ get{ return false; } }
	}

	public class DuringCollectFungusConversation : QuestConversation
	{
		public override object Message
		{
			get{return "<I>The old necromancer looks at you and says...</I><BR><BR>Have you collected the poison fungus already? If so give them to me so i can create the spell.";}
		}
		public override bool Logged{ get{ return false; } }
	}

	public class VanquishDaemonConversation : QuestConversation
	{
		public override object Message
		{
			get
			{
				return "Well done! I shall now use the spell to teleport you and your party to the Alternate Dimension.";
			}
		}

		public VanquishDaemonConversation()
		{
		}

		public override void OnRead()
		{
			Zuleika zuleika = ((ThePlaugeQuest)System).Zuleika;

			if ( zuleika == null )
			{
				System.From.SendMessage( "Internal error: unable to find Zuleika. Quest unable to continue." );
				System.Cancel();
			}
			else
			{
				PlagueSummoningAltar altar = zuleika.Altar;

				if ( altar == null )
				{
					System.From.SendMessage( "Internal error: unable to find summoning altar. Quest unable to continue." );
					System.Cancel();
				}
				else
				{
					altar.MainQueue++;

					zuleika.MovePlayers( System.From );

					System.Complete();
				}
			}
		}
	}
}