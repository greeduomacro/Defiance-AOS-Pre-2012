using System;
using Server;
using Server.Mobiles;
using Server.Items;
using Server.Network;
using Server.Regions;

namespace Server.Engines.Quests.QuestionableAlchemist
{
    public class CollectBloodObjective : QuestObjective
    {
        public class MonsterEntry
        {
            public Type Monster;
            public string Name;
            public int Amount;

            public MonsterEntry(Type type, string name, int amount)
            {
                Monster = type;
                Name = name;
                Amount = amount;
            }
        }

        // Used for non-localized HTML.
        public static string HtmlBlue = "8CADFF";

        public static string Color(string text, string color)
        {
            return String.Format("<BASEFONT COLOR=#{0}>{1}</BASEFONT>", color, text);
        }

        private static MonsterEntry[] m_Types = new MonsterEntry[]
		{
			new MonsterEntry( typeof( GiantBlackWidow ),	"Giant Black Widow",	1 ),
			new MonsterEntry( typeof( TheCursed ),			"A Cursed",				1 ),
			new MonsterEntry( typeof( TheCursedArcher ),    "A Cursed Archer",		1 ),
			new MonsterEntry( typeof( TheCursedGuardian ),  "A Cursed Guardian",	1 ),
		};

        private MonsterEntry m_MonsterType;
        public MonsterEntry MonsterType { get { return m_MonsterType; } }

        private bool m_bIsAllCompleted;
        public bool IsAllCompleted { get { return m_bIsAllCompleted; } }

        private int m_iLevel;

        public override int MaxProgress
        {
            get
            {
                if (m_MonsterType == null)
                    return 1;
                else return m_MonsterType.Amount;
            }
        }

        public override object Message { get { return "Elda needs some mutated blood from the following monster in the Cursed Cave."; } }

        public CollectBloodObjective(int level)
        {
            m_iLevel = level;
            m_MonsterType = GetCurMonster(m_iLevel);
        }

        public CollectBloodObjective()
        {
        }

        public override void OnComplete()
        {
            m_iLevel++;
            if (m_iLevel >= m_Types.Length)
                m_bIsAllCompleted = true;
            else
                System.AddObjective(new CollectBloodObjective(m_iLevel));
        }

        public override void RenderProgress(BaseQuestGump gump)
        {
            if (!Completed)
            {
                gump.AddHtml(70, 260, 270, 100, Color(m_MonsterType.Name, HtmlBlue), false, false);
                gump.AddLabel(70, 280, 0x64, string.Format("{0} / {1}", CurProgress.ToString(), m_MonsterType.Amount.ToString()));
            }
            else
            {
                base.RenderProgress(gump);
            }
        }

        private MonsterEntry GetCurMonster(int level)
        {
            if (level >= m_Types.Length)
                return m_Types[m_Types.Length - 1];
            else
                return m_Types[level];
        }

        public override void ChildDeserialize(GenericReader reader)
        {
            int version = reader.ReadEncodedInt();

            switch (version)
            {
                case 0:
                    m_bIsAllCompleted = reader.ReadBool();
                    m_iLevel = reader.ReadInt();
                    break;
            }

            m_MonsterType = GetCurMonster(m_iLevel);
            if (CurProgress > MaxProgress)
                CurProgress = MaxProgress - 1;
        }

        public override void ChildSerialize(GenericWriter writer)
        {
            writer.WriteEncodedInt((int)0); // version

            writer.Write(m_bIsAllCompleted);
            writer.Write(m_iLevel);
        }
    }
}