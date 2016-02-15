using System;
using Server;

namespace Server.Engines.Quests.CursedCave
{
	public class AcceptConversation : QuestConversation
	{
		public override object Message
		{
			get
			{
				return "<I><U>About the cursed cave</U></I><BR><BR>People in the town say that they occasionally hear screams from the cave during the night... and strange noises... some people have even reported strange green creatures standing at the entrance of the cave."+
					"<BR><BR>Mages who have ventured to the cave report that their spells do not work within, bards say that their music has no effect upon any creature."+
					"<BR><BR><I><U>The Lost Rares</U></I><BR><BR>Everybody in the nearby town have been talking about some kind of rares that are hidden in this cave, but the cursed creatures seem to guard them with their lifes. If you happen to find these rares and somehow manage to steal them bring them to Warren the Guard he will buy them, or you can just keep them for yourself.";
			}
		}

		public AcceptConversation()
		{
		}

		public override void OnRead()
		{
			System.AddObjective( new CursedCaveBeginObjective(0) );
		}
	}

	public class DuringKillTaskConversation : QuestConversation
	{
		public override object Message
		{
			get{return "<I>The guard looks up to you and says...</I><BR><BR>Back so soon..? Have you killed the creatures I told you to? No? Well return here when you have and I will reward you.";}
		}
		public override bool Logged{ get{ return false; } }
	}

	public class DontOfferConversation : QuestConversation
	{
		public override object Message
		{
			get{return "<I>The guard looks up to you and says...</I><BR><BR>Hail Stranger!<BR>I could use some help, but you seem too busy with another quest. Come and speak with me again when you have completed your present quest.";}
		}
		public override bool Logged{ get{ return false; } }
	}

	public class EndConversation : QuestConversation
	{
		public override object Message
		{
			get{return "<I>The guard smiles as you tell him of your completed task.</I><BR><BR>Well done! I did not think you would return here alive! Here is thy reward just as I promised.<BR><BR><I>The guard gives you a bag.</I>";}
		}

		public override void OnRead()
		{
			System.Complete();
		}
	}
}