using System;
using System.Collections;
using Server;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.Quests.Plauge
{
	public class CollectFungusObjective : QuestObjective
	{
		public override object Message{	get{return "Find 200 Poison fungus and hand them to Zuleika as you find them.";} }

		public override int MaxProgress{ get{ return 200; } }

		public CollectFungusObjective()
		{
		}

		public override void OnComplete()
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
					System.AddConversation( new VanquishDaemonConversation() );
			}
		}

		public override void RenderMessage( BaseQuestGump gump )
		{
			if ( CurProgress > 0 && CurProgress < MaxProgress )
				gump.AddHtmlObject( 70, 130, 300, 100, "Zuleika has accepted the Poison fungus, but the requirement is not yet met.", BaseQuestGump.Blue, false, false );
			else
				base.RenderMessage( gump );
		}

		public override void RenderProgress( BaseQuestGump gump )
		{
			if ( CurProgress > 0 && CurProgress < MaxProgress )
			{
				gump.AddHtmlObject( 70, 260, 270, 100, "Number of fungus collected:", BaseQuestGump.Blue, false, false );

				gump.AddLabel( 70, 280, 100, CurProgress.ToString() );
				gump.AddLabel( 100, 280, 100, "/" );
				gump.AddLabel( 130, 280, 100, MaxProgress.ToString() );
			}
			else
			{
				base.RenderProgress( gump );
			}
		}
	}
}