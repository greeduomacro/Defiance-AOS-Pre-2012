using System;
using Server;
using Server.Mobiles;
using Server.Items;

namespace Server.Engines.Quests.Midas
{
	public class HeadlessObjective : QuestObjective
	{
		public override object Message
		{
			get
			{
				return "<U>Darknight creepers</U> Search britania's doom dungeon. There you can find plenty of Darknight creepers. Kill 1 of them.";
			}
		}

		public override int MaxProgress { get { return 1; } } //This sets how many do you have to kill

		public HeadlessObjective()
		{
		}

		public override void RenderProgress(BaseQuestGump gump)
		{
			if (!Completed)
			{
				gump.AddLabel(70, 260, 270, "Darknight Creeper");
				gump.AddLabel(70, 280, 0x64, CurProgress.ToString());
				gump.AddLabel(100, 280, 0x64, "/");
				gump.AddLabel(130, 280, 0x64, MaxProgress.ToString());
			}
			else
			{
				base.RenderProgress(gump);
			}
		}


		public override void OnKill(BaseCreature creature, Container corpse)
		{
			if (creature is DarknightCreeper)
			CurProgress++; //On every kill curprogress increase.When CurProgress >= MaxProgress OnComplete() runs.This is something from quest core.
		}


		public override void OnComplete()
		{
			System.AddObjective( new ReturnToPriestObjective() );
		}
	}

	public class ZombieObjective : QuestObjective
	{
		public override object Message
		{
			get
			{
				return "<U>Flesh renderer</U> Search britania's Doom dungeon. There you can find plenty of Flesh Renderers. Kill 1 of them.";
			}
		}

		public override int MaxProgress { get { return 1; } }

		public ZombieObjective()
		{
		}

		public override void RenderProgress(BaseQuestGump gump)
		{
			if (!Completed)
			{
				gump.AddLabel(70, 260, 270, "Flesh Renderer");
				gump.AddLabel(70, 280, 0x64, CurProgress.ToString());
				gump.AddLabel(100, 280, 0x64, "/");
				gump.AddLabel(130, 280, 0x64, MaxProgress.ToString());
			}
			else
			{
				base.RenderProgress(gump);
			}
		}


		public override void OnKill(BaseCreature creature, Container corpse)
		{
			if (creature is FleshRenderer)
				CurProgress++;
		}


		public override void OnComplete()
		{
			System.AddObjective(new ReturnToPriest2Objective());
		}
	}

	public class SkeletonObjective : QuestObjective
	{
		public override object Message
		{
			get
			{
				return "<U>Impaler</U> Search britania's Doom dungeon. There you can find plenty of impalers. Kill 1 of them.";
			}
		}

		public override int MaxProgress { get { return 1; } }

		public SkeletonObjective()
		{
		}

		public override void RenderProgress(BaseQuestGump gump)
		{
			if (!Completed)
			{
				gump.AddLabel(70, 260, 270, "Impaler");
				gump.AddLabel(70, 280, 0x64, CurProgress.ToString());
				gump.AddLabel(100, 280, 0x64, "/");
				gump.AddLabel(130, 280, 0x64, MaxProgress.ToString());
			}
			else
			{
				base.RenderProgress(gump);
			}
		}


		public override void OnKill(BaseCreature creature, Container corpse)
		{
			if (creature is Impaler)
				CurProgress++;
		}


		public override void OnComplete()
		{
			System.AddObjective(new ReturnToPriest3Objective());
		}
	}

	public class Skeleton1Objective : QuestObjective
	{
		public override object Message
		{
			get
			{
				return "<U>Shadow knight</U> Search britania's Doom dungeon. There you can find plenty of Shadow knights. Kill 1 of them.";
			}
		}

		public override int MaxProgress { get { return 1; } }

		public Skeleton1Objective()
		{
		}

		public override void RenderProgress(BaseQuestGump gump)
		{
			if (!Completed)
			{
				gump.AddLabel(70, 260, 270, "ShadowKnight");
				gump.AddLabel(70, 280, 0x64, CurProgress.ToString());
				gump.AddLabel(100, 280, 0x64, "/");
				gump.AddLabel(130, 280, 0x64, MaxProgress.ToString());
			}
			else
			{
				base.RenderProgress(gump);
			}
		}


		public override void OnKill(BaseCreature creature, Container corpse)
		{
			if (creature is ShadowKnight)
				CurProgress++;
		}

		public override void OnComplete()
		{
			System.AddObjective(new ReturnToPriest4Objective());
		}
	}


	public class AbysmalObjective : QuestObjective
	{
		public override object Message
		{
			get
			{
				return "<U>Abysmal Horror</U> Search britania's Doom dungeon. There you can find plenty of Abysmal horrors. Kill 1 of them.";
			}
		}

		public override int MaxProgress { get { return 1; } }

		public AbysmalObjective()
		{
		}

		public override void RenderProgress(BaseQuestGump gump)
		{
			if (!Completed)
			{
				gump.AddLabel(70, 260, 270, "Abysmal Horror");
				gump.AddLabel(70, 280, 0x64, CurProgress.ToString());
				gump.AddLabel(100, 280, 0x64, "/");
				gump.AddLabel(130, 280, 0x64, MaxProgress.ToString());
			}
			else
			{
				base.RenderProgress(gump);
			}
		}


		public override void OnKill(BaseCreature creature, Container corpse)
		{
			if (creature is AbysmalHorror)
				CurProgress++;
		}


		public override void OnComplete()
		{
			System.AddObjective(new ReturnToPriest5Objective());
		}
	}

	public class DFObjective : QuestObjective
	{
		public override object Message
		{
			get
			{
				return "<U>Dark Father</U> Search britania's Doom dungeon. There you can find plenty of Dark Fathers. Kill 1 of them.";
			}
		}

		public override int MaxProgress { get { return 1; } }

		public DFObjective()
		{
		}

		public override void RenderProgress(BaseQuestGump gump)
		{
			if (!Completed)
			{
				gump.AddLabel(70, 260, 270, "Dark Father");
				gump.AddLabel(70, 280, 0x64, CurProgress.ToString());
				gump.AddLabel(100, 280, 0x64, "/");
				gump.AddLabel(130, 280, 0x64, MaxProgress.ToString());
			}
			else
			{
				base.RenderProgress(gump);
			}
		}


		public override void OnKill(BaseCreature creature, Container corpse)
		{
			if (creature is DemonKnight)
				CurProgress++;
		}


		public override void OnComplete()
		{
			System.AddObjective(new ReturnToPriest6Objective());
		}
	}

	public class ReturnToPriestObjective : QuestObjective
	{
		public override object Message
		{
			get
			{
				return "Return to Nimre for further instructions.";
			}
		}

		public ReturnToPriestObjective()
		{
		}

		public override void OnComplete()
		{
		}
	}

	public class ReturnToPriest2Objective : QuestObjective
	{
		public override object Message
		{
			get
			{
				return "Return to Nimre for further instructions.";
			}
		}

		public ReturnToPriest2Objective()
		{
		}

		public override void OnComplete()
		{
		}
	}
	public class ReturnToPriest3Objective : QuestObjective
	{
		public override object Message
		{
			get
			{
				return "RReturn to Nimre for further instructions..";
			}
		}

		public ReturnToPriest3Objective()
		{
		}

		public override void OnComplete()
		{
		}
	}
	public class ReturnToPriest4Objective : QuestObjective
	{
		public override object Message
		{
			get
			{
				return "Return to Nimre for further instructions.";
			}
		}

		public ReturnToPriest4Objective()
		{
		}

		public override void OnComplete()
		{
		}
	}
	public class ReturnToPriest5Objective : QuestObjective
	{
		public override object Message
		{
			get
			{
				return "Return to Nimre for further instructions.";
			}
		}

		public ReturnToPriest5Objective()
		{
		}

		public override void OnComplete()
		{
		}
	}
	public class ReturnToPriest6Objective : QuestObjective
	{
		public override object Message
		{
			get
			{
				return "Return to Nimre to talk and get your reward.";
			}
		}

		public ReturnToPriest6Objective()
		{
		}

		public override void OnComplete() //You can add rewards here.
		{
		}
	}

}