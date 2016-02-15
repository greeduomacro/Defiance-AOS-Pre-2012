using System;
using Server;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.Quests.QuestionableAlchemist
{
    public class QuestionableAlchemistQuest : QuestSystem
    {
        private static Type[] m_TypeReferenceTable = new Type[]
			{
				typeof( QuestionableAlchemist.AcceptConversation ),
				typeof( QuestionableAlchemist.DuringCollectBloodConversation ),
				typeof( QuestionableAlchemist.DontOfferConversation ),
				typeof( QuestionableAlchemist.EndConversation  )
			};

        public override Type[] TypeReferenceTable { get { return m_TypeReferenceTable; } }

        public override object Name { get { return "Questionable Alchemist"; } }
        public override object OfferMessage
        {
            get { return "<I>The alchemist looks at you from behind some strange bottles and says.. </I><BR><BR>The blood... the mutated blood that some of the cursed creatures have seems to have magical properties in the cursed cave near here. I need to get some more of this blood from the foul creatures so that i can experiment with it.<BR><BR>As a reward i will try to make a blood pentagram for you, but since it is very difficult and expensive task to make these pentagrams i will only try to make it for you one time."; }
        }
        public override TimeSpan RestartDelay { get { return TimeSpan.Zero; } }
        public override bool IsTutorial { get { return false; } }
        public override int Picture { get { return 0x15C9; } }

        public QuestionableAlchemistQuest(PlayerMobile from)
            : base(from)
        {
        }

        // Serialization
        public QuestionableAlchemistQuest()
        {
        }

        public override void Accept()
        {
            base.Accept();
            AddConversation(new AcceptConversation());
        }

        public override void ChildDeserialize(GenericReader reader)
        {
            int version = reader.ReadEncodedInt();
        }

        public override void ChildSerialize(GenericWriter writer)
        {
            writer.WriteEncodedInt((int)0); // version
        }
    }
}