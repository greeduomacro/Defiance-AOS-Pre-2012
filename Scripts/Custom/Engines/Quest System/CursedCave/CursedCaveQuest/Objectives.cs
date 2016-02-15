using System;
using Server;
using Server.Mobiles;
using Server.Items;
using Server.Network;
using Server.Regions;

namespace Server.Engines.Quests.CursedCave
{
	public class CursedCaveBeginObjective : QuestObjective
	{
		public class MonsterEntry
		{
			public Type Monster;
			public string Name;
			public int Amount;

			public MonsterEntry( Type type, string name, int amount )
			{
				Monster = type;
				Name = name;
				Amount = amount;
			}
		}

		// Used for non-localized HTML.
		public static string HtmlBlue = "8CADFF";

		public static string Color( string text, string color )
		{
			return String.Format( "<BASEFONT COLOR=#{0}>{1}</BASEFONT>", color, text );
		}

 		private static MonsterEntry[] m_Types = new MonsterEntry[]
		{
			new MonsterEntry( typeof( HellCat ),			"Hell Cat",				4 ),
			new MonsterEntry( typeof( HellHound ),			"Hell Hound",			4 ),
			new MonsterEntry( typeof( GiantBlackWidow ),	"Giant Black Widow",	4 ),
			new MonsterEntry( typeof( TheCursed ),			"A Cursed",				6 ),
			new MonsterEntry( typeof( CursedSkeleton ),		"Cursed Warrior",		2 ),
			new MonsterEntry( typeof( DevaTheCursedOne ),	"Deva The Cursed One",	1 ),
		};

		private MonsterEntry m_MonsterType;
		private int m_iLevel;

		public override int MaxProgress
		{
			get
			{
				if(m_MonsterType == null)
					return 1;
				else return m_MonsterType.Amount;
			}
		}

		public override object Message{	get{return "Help the guards by killing the following monster in the Cursed Cave.";} }

		public CursedCaveBeginObjective( int level )
		{
			m_iLevel = level;
			m_MonsterType = GetCurMonster( m_iLevel );
		}

		public CursedCaveBeginObjective()
		{
		}

		public override void OnComplete()
		{
			m_iLevel++;
			if(m_iLevel >= m_Types.Length)
				System.AddObjective( new ReportBackObjective() );
			else
				System.AddObjective( new CursedCaveBeginObjective(m_iLevel) );
		}

		public override void RenderProgress( BaseQuestGump gump )
		{
			if ( !Completed )
			{
				gump.AddHtml( 70, 260, 270, 100, Color(m_MonsterType.Name, HtmlBlue), false, false );
				gump.AddLabel(70, 280, 0x64, string.Format("{0} / {1}", CurProgress.ToString(), m_MonsterType.Amount.ToString()));
			}
			else
			{
				base.RenderProgress( gump );
			}
		}

		public override void OnKill( BaseCreature creature, Container corpse )
		{
			if ( creature != null && m_MonsterType != null && creature.GetType() == m_MonsterType.Monster && creature.Region is CursedCaveRegion )
				CurProgress++;
		}

		private MonsterEntry GetCurMonster( int level )
		{
			if(level >= m_Types.Length)
				return m_Types[m_Types.Length-1];
			else
				return m_Types[level];
		}

		public override void ChildDeserialize( GenericReader reader )
		{
			int version = reader.ReadEncodedInt();

			switch ( version )
			{
				case 0:
					m_iLevel = reader.ReadEncodedInt();
					break;
			}

			m_MonsterType = GetCurMonster( m_iLevel );
			if( CurProgress > MaxProgress )
				CurProgress = MaxProgress - 1;
		}

		public override void ChildSerialize( GenericWriter writer )
		{
			writer.WriteEncodedInt( (int) 0 ); // version

			writer.WriteEncodedInt( (int) m_iLevel );
		}
	}

	public class ReportBackObjective : QuestObjective
	{
		public override object Message
		{
			get{return "Return to Warren and tell him that you have completed the task.";}
		}

		public ReportBackObjective()
		{
		}

		public override void OnComplete()
		{
			System.AddConversation( new EndConversation() );
		}
	}
}