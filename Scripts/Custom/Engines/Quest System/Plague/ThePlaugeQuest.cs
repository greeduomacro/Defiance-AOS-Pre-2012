using System;
using Server;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.Quests.Plauge
{
	public class ThePlaugeQuest : QuestSystem
	{
		private static Type[] m_TypeReferenceTable = new Type[]
			{
				typeof( Plauge.AcceptConversation ),
				typeof( Plauge.CollectFungusObjective ),
				typeof( Plauge.VanquishDaemonConversation ),
				typeof( Plauge.DontOfferConversation ),
				typeof( Plauge.DuringCollectFungusConversation )
			};

		public override Type[] TypeReferenceTable{ get{ return m_TypeReferenceTable; } }

		private Zuleika m_Zuleika;

		public Zuleika Zuleika
		{
			get{ return m_Zuleika; }
		}

		public override object Name
		{
			get
			{
				return "The Plague";
			}
		}

		public override object OfferMessage
		{
			get
			{
				return "<I>Zuleika turns to you and says...</I><BR><BR>I can help you find these evil plague beasts, i need 200 Poison fungus to create a spell that can teleport you and your party to the Alternate Dimension. If you accept my help, I will store the Poison fungus for you until you have collected all 200 of them. Once the fungus are collected in full, I will teleport you and your party to the Alternate Dimension.<BR><BR>";
			}
		}

		public override bool IsTutorial{ get{ return false; } }
		public override TimeSpan RestartDelay{ get{ return TimeSpan.Zero; } }
		public override int Picture{ get{ return 0x15B5; } }

		public ThePlaugeQuest( Zuleika Zuleika, PlayerMobile from ) : base( from )
		{
			m_Zuleika = Zuleika;
		}

		public ThePlaugeQuest()
		{
		}

		public override void Cancel()
		{
			base.Cancel();

			QuestObjective obj = FindObjective( typeof( CollectFungusObjective ) );

			if ( obj != null && obj.CurProgress > 0 )
			{
				BankBox box = From.BankBox;

				if ( box != null )
					box.DropItem( new PoisonFungus( obj.CurProgress ) );

				From.SendMessage( "The Poison fungus that you have thus far given to Zuleika have been returned to you." );
			}
		}

		public override void Accept()
		{
			base.Accept();

			AddConversation( new AcceptConversation() );
		}

		public override void ChildDeserialize( GenericReader reader )
		{
			int version = reader.ReadEncodedInt();

			m_Zuleika = reader.ReadMobile() as Zuleika;
		}

		public override void ChildSerialize( GenericWriter writer )
		{
			writer.WriteEncodedInt( (int) 0 ); // version

			writer.Write( (Mobile) m_Zuleika );
		}
	}
}