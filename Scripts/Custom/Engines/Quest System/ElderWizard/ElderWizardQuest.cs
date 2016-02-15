using System;
using Server;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.Quests.ElderWizard
{
	public class ElderWizardQuest : QuestSystem
	{
		private static Type[] m_TypeReferenceTable = new Type[]
			{
				typeof( ElderWizard.AcceptConversation ),
				typeof( ElderWizard.DuringFindPlantConversation ),
				typeof( ElderWizard.DontOfferConversation ),
				typeof( ElderWizard.EndConversation  )
			};

		public override Type[] TypeReferenceTable { get { return m_TypeReferenceTable; } }

		public override object Name { get { return "Elder Wizard"; } }
		public override object OfferMessage
		{
			get { return "<I>The Elder Wizard looks busy sorting some strange looking plants as you approach her, she turns around and says. </I><BR><BR>Hail traverel!<BR>Would you like to collect some rare plants for me? As reward i can give you a reagent bag that is easy to refill."; }
		}
		public override TimeSpan RestartDelay { get { return TimeSpan.Zero; } }
		public override bool IsTutorial { get { return false; } }
		public override int Picture { get { return 0x15C9; } }

		public ElderWizardQuest(PlayerMobile from)
			: base(from)
		{
		}

		// Serialization
		public ElderWizardQuest()
		{
		}

		public override void Accept()
		{
			base.Accept();
			if (From.Backpack != null)
				From.Backpack.DropItem(new RarePlantsBook());
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