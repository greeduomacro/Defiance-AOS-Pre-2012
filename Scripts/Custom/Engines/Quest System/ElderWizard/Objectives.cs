using System;
using Server;
using Server.Mobiles;
using Server.Items;
using Server.Network;
using Server.Regions;

namespace Server.Engines.Quests.ElderWizard
{
	public class FindPlantObjective : QuestObjective
	{
		public class PlantEntry
		{
			public Type PlantType;
			public string Name;
			public int Amount;

			public PlantEntry(Type type, string name, int amount)
			{
				PlantType = type;
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

		private static PlantEntry[] m_Types = new PlantEntry[]
		{
			new PlantEntry( typeof( MandrakeQuestReagent ),			"Rare Mandrake Root",				4 ),
			new PlantEntry( typeof( GarlicQuestReagent ),			"Rare Garlic Root", 				4 ),
			new PlantEntry( typeof( NightShadeQuestReagent ),		"Rare Nightshade Root",				4 ),
			new PlantEntry( typeof( GinsengQuestReagent ),			"Rare Ginseng Root",				4 )
		};

		private PlantEntry m_PlantEntry;
		private int m_iLevel;

		public override int MaxProgress
		{
			get
			{
				if (m_PlantEntry == null)
					return 1;
				else return m_PlantEntry.Amount;
			}
		}

		public override object Message { get { return "Find this rare plant for the Elder Wizard."; } }

		public FindPlantObjective(int level)
		{
			m_iLevel = level;
			m_PlantEntry = GetCurrentPlantEntry();
		}

		public PlantEntry GetCurrentPlantEntry()
		{
			if (m_iLevel >= m_Types.Length)
				return m_Types[m_Types.Length - 1];
			else
				return m_Types[m_iLevel];
		}

		public FindPlantObjective()
		{
		}

		public override void OnComplete()
		{
			m_iLevel++;
			if (m_iLevel >= m_Types.Length)
				System.AddObjective(new ReportBackObjective());
			else
				System.AddObjective(new FindPlantObjective(m_iLevel));
		}

		public override void RenderProgress(BaseQuestGump gump)
		{
			if (!Completed)
			{
				gump.AddHtml(70, 260, 270, 100, Color(m_PlantEntry.Name, HtmlBlue), false, false);
				gump.AddLabel(70, 280, 0x64, string.Format( "{0} / {1}", CurProgress.ToString(), m_PlantEntry.Amount.ToString()) );
			}
			else
			{
				base.RenderProgress(gump);
			}
		}

		public override void ChildDeserialize(GenericReader reader)
		{
			int version = reader.ReadEncodedInt();

			switch (version)
			{
				case 0:
					m_iLevel = reader.ReadEncodedInt();
					break;
			}

			m_PlantEntry = GetCurrentPlantEntry();
			if (CurProgress > MaxProgress)
				CurProgress = MaxProgress - 1;
		}

		public override void ChildSerialize(GenericWriter writer)
		{
			writer.WriteEncodedInt((int)0); // version

			writer.WriteEncodedInt((int)m_iLevel);
		}
	}

	public class ReportBackObjective : QuestObjective
	{
		public override object Message
		{
			get { return "Return to the Elder Wizard and tell her that you have found all ingredients."; }
		}

		public ReportBackObjective()
		{
		}

		public override void OnComplete()
		{
			System.AddConversation(new EndConversation());
		}
	}
}