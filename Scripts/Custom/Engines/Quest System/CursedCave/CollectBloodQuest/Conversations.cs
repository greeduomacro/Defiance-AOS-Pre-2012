using System;
using Server;

namespace Server.Engines.Quests.QuestionableAlchemist
{
	public class AcceptConversation : QuestConversation
	{
		public override object Message
		{
			get
			{
                return "<I><U>About draining blood</U></I><BR><BR>To drain blood from a creature you will need a <I>blood draining toolkit</I>, using this toolkit and some empty bottles you can drain blood from almost all corpses.<BR><BR>You can buy a <I>blood draining toolkit</I> from alchemists.<BR><BR><I><U>About rare blood</U></I><BR><BR>Rare blood can be found on creatures like paragons & plagued creatures, with this blood it is possible to color a blood pentagram.";
			}
		}

		public AcceptConversation()
		{
		}

		public override void OnRead()
		{
            System.AddObjective(new CollectBloodObjective(0));
		}
	}

	public class DuringCollectBloodConversation : QuestConversation
	{
		public override object Message
		{
			get{return "<I>Elda is mixing some potions... she puts down the bottle she is holding and says.</I><BR><BR>Back so soon..? Have you collected the blood that i need? If you have then give it to me so i can check if its the correct blood.";}
		}
		public override bool Logged{ get{ return false; } }
	}

	public class DontOfferConversation : QuestConversation
	{
		public override object Message
		{
			get{return "<I>The alchemist looks up to you and says...</I><BR><BR>Hello there!<BR>I could use some help, but you seem too busy with another quest. Come and speak with me again when you have completed your present quest.";}
		}
		public override bool Logged{ get{ return false; } }
	}

	public class EndConversation : QuestConversation
	{
		public override object Message
		{
			get{return "<I>Elda smiles as you tell her that you have collected the blood that she needed.</I><BR><BR>Well done! I did not think you would return here alive! Here is thy reward just as I promised.<BR><BR><I>Elda gives you a bag.</I>";}
		}

		public override void OnRead()
		{
		}
	}
}