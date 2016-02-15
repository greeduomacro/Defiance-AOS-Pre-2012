using System;
using Server;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.Quests.CursedCave
{
	public class CursedCaveQuest : QuestSystem
	{
		private static Type[] m_TypeReferenceTable = new Type[]
			{
				typeof( CursedCave.AcceptConversation ),
				typeof( CursedCave.DuringKillTaskConversation ),
				typeof( CursedCave.DontOfferConversation ),
				typeof( CursedCave.EndConversation  )
			};

		public override Type[] TypeReferenceTable{ get{ return m_TypeReferenceTable; } }

		public override object Name{ get{return "Cursed Cave";} }
		public override object OfferMessage
		{
			get{return "<I>The guard speaks to you as you approach.. </I><BR><BR>Hail to thee adventurer!<BR>Would you like to help us killing a few monsters in the cursed cave? I will reward you if you complete your task... all I have to give as a reward are some magical items, gold and reagents.";}
		}
		public override TimeSpan RestartDelay{ get{ return TimeSpan.Zero; } }
		public override bool IsTutorial{ get{ return false; } }
		public override int Picture{ get{return 0x15C9;} }

		public CursedCaveQuest( PlayerMobile from ) : base( from )
		{
		}

		// Serialization
		public CursedCaveQuest()
		{
		}

		public override void Accept()
		{
			base.Accept();
			AddConversation( new AcceptConversation() );
		}

		public override void ChildDeserialize( GenericReader reader )
		{
			int version = reader.ReadEncodedInt();
		}

		public override void ChildSerialize( GenericWriter writer )
		{
			writer.WriteEncodedInt( (int) 0 ); // version
		}
	}
}