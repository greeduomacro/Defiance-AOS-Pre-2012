using System;
using Server;

namespace Server.Engines.Quests.ElderWizard
{
	public class AcceptConversation : QuestConversation
	{
		public override object Message
		{
			get
			{
				return "<I>The wizard looks happy as you accept the task.</I><BR><BR>Return to me when you have collected all of the rare plants. The location of these plants can be found in this book.<BR><BR><I>She gives you a book.</I><BR><BR>Good Luck.";
			}
		}

		public AcceptConversation()
		{
		}

		public override void OnRead()
		{
			System.AddObjective(new FindPlantObjective(0));
		}
	}

	public class DuringFindPlantConversation : QuestConversation
	{
		public override object Message
		{
			get{return "<I>The elder wizard looks up from her cauldron and says...</I><BR><BR>Back so soon..? Have you collected the reagents I told you to? No? Well return here when you have and I will reward you.";}
		}
		public override bool Logged{ get{ return false; } }
	}

	public class DontOfferConversation : QuestConversation
	{
		public override object Message
		{
			get { return "<I>The elder wizard says...</I><BR><BR>I could use some help, but you seem too busy with another quest. Come and speak with me again when you have completed your present quest."; }
		}
		public override bool Logged{ get{ return false; } }
	}

	public class EndConversation : QuestConversation
	{
		public override object Message
		{
			get { return "<I>The elder wizard looks happy as you tell her of your completed task.</I><BR><BR>Well done! I did not think you would find all of them!<BR><BR><I>She smiles and takes the reagents from you and place them in a bag.</I><BR><BR>Here is your reward just as I promised.<BR><BR><I>She gives you a Refillable Reagent Bag.</I><BR><BR>The refillable reagent bag can be easily refilled by any vendor that sells reagents, just drag and drop the bag on the vendor and he will tell you the cost to refill the bag."; }
		}

		public override void OnRead()
		{
			System.Complete();
		}
	}
}