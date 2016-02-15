using System;
using Server;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.Quests.Midas
{
	public class DeclineConversation : QuestConversation
	{
		public override object Message
		{
			get
			{
				return "Oh please :( My lunch money... *cries*";
			}
		}

		public override bool Logged{ get{ return false; } }

		public DeclineConversation()
		{
		}
	}

	public class HeadlessConversation : QuestConversation
	{
		public override object Message
		{
			get
			{
				return "Nice! I think the Darknight creeper stole my lunch money. Kill the sucker and return to me!";
			}
		}

		public HeadlessConversation()
		{
		}

		public override void OnRead()
		{
			System.AddObjective( new HeadlessObjective() );
		}
	}

    public class ZombieConversation : QuestConversation
    {
        public override object Message
        {
            get
            {
                return "Oh, you killed it? Only 10 gp in its pocket? Damn! Find the Flesh renderer then. I bet he has my lunch money. Kill it and then return to me.";
            }
        }

        public ZombieConversation()
        {
        }

        public override void OnRead()
        {
            System.AddObjective(new ZombieObjective());
        }
    }

    public class SkeletonConversation : QuestConversation
    {
        public override object Message
        {
            get
            {
                return "No lunch money again? Man, these demons really suck. Maybe the Impaler did it after all. Go kill him and return to me.";
            }
        }

        public SkeletonConversation()
        {
        }

        public override void OnRead()
        {
            System.AddObjective(new SkeletonObjective());
        }
    }

    public class Skeleton1Conversation : QuestConversation
    {
        public override object Message
        {
            get
            {
                return "Ah, you are back again! Did you find my lunch money? No?!! Go to the lair of the Shadow knight. If he has a hangover he may try to hide. Do not let him escape! Nail him and return to me after he is dead!";
            }
        }

        public Skeleton1Conversation()
        {
        }

        public override void OnRead()
        {
            System.AddObjective(new Skeleton1Objective());
        }
    }

    public class AbysmalConversation : QuestConversation
    {
        public override object Message
        {
            get
            {
                return "What? Nothing again? Ok! Dark father? No, no, no he is innocent! I am sure the Abysmal Horror is involved! Kill him and return to me!";
            }
        }

        public AbysmalConversation()
        {
        }

        public override void OnRead()
        {
            System.AddObjective(new AbysmalObjective());
        }
    }

    public class DFConversation : QuestConversation
    {
        public override object Message
        {
            get
            {
                return "Still no lunch money? Ah! I cannot believe it! It was the Dark father after all... I trusted him so much :( Kill him and return to me!";
            }
        }

        public DFConversation()
        {
        }

        public override void OnRead()
        {
            System.AddObjective(new DFObjective());
        }
    }

	public class DuringHeadlessConversation : QuestConversation
	{
		public override object Message
		{
			get
			{
				return "You didn't finish your task yet! Continue when you want.";
			}
		}

		public override bool Logged{ get{ return false; } }

		public DuringHeadlessConversation()
		{
		}
	}

	public class EndConversation : QuestConversation
	{
		public override object Message
		{
			get
			{
				Mobile from = System.From;
				Map map = from.Map;
				return "Oh my lunch money! I am so happy! \nYour reward is in your backpack!";
				//ErmoQuest ermo = ((UndeadHuntingQuest)System).ErmoQuest;
				//SummoningAltarErmo altar = ermo.Altar;

					//BoneDaemon daemon = new BoneDaemon();

					//daemon.MoveToWorld( from.Location, map );
					//altar.Daemon = daemon;
			}
		}

		public EndConversation()
		{
		}

		public override void OnRead()
		{
			MidasQuest.GiveRewardTo(System.From);
			System.Complete();
		}
	}

}