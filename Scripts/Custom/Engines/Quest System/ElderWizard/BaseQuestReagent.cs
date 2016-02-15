using System;
using Server;
using Server.Mobiles;
using Server.Network;
using Server.Regions;
using Server.Engines.Quests;
using Server.Engines.Quests.ElderWizard;

namespace Server.Items
{
	public abstract class BaseQuestReagent : Item
	{
		public class ElderWizardCreatureEntry
		{
			public Type m_tCreatureType;
			public string m_sCreatureName;
			public int m_iMaxAmount, m_iMinAmount;

			public ElderWizardCreatureEntry(Type CreatureType, string CreatureName, int MinAmount, int MaxAmount)
			{
				m_tCreatureType = CreatureType;
				m_sCreatureName = CreatureName;
				m_iMinAmount = MinAmount;
				m_iMaxAmount = MaxAmount;
			}
		}

		public static ElderWizardCreatureEntry[] m_ElderWizardCreatures = new ElderWizardCreatureEntry[]
		{
			new ElderWizardCreatureEntry( typeof( Snake ),			"Snakes",	  2, 6 ),
			new ElderWizardCreatureEntry( typeof( GiantSpider ),	"Spiders",	 2, 3 ),
			new ElderWizardCreatureEntry( typeof( GiantSerpent ),	"Serpents",	2, 3 ),
			new ElderWizardCreatureEntry( typeof( SilverSerpent ),	"Serpents",	1, 2 ),
			new ElderWizardCreatureEntry( typeof( GiantBlackWidow ),"Spiders",	 1, 2 ),
		};

		public virtual double NestDisturbChance{ get { return 0.35; } } // 35%
		public virtual double SpecialNestChance { get { return 0.35; } } // 35%
		private bool m_Picked;
		public abstract int GetPickedID();

		public BaseQuestReagent(int itemID)
			: base(itemID)
		{
			Movable = false;
		}

		public override void OnDoubleClick(Mobile from)
		{
			Map map = this.Map;
			Point3D loc = this.Location;
			PlayerMobile player = from as PlayerMobile;

			if (Parent != null || Movable || IsLockedDown || IsSecure || map == null || map == Map.Internal)
				return;

			if (!from.InRange(loc, 2) || !from.InLOS(this))
			{
				from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
			}
			else if (player != null && !m_Picked)
			{
				QuestSystem qs = player.Quest;

				if (qs is ElderWizardQuest)
				{
					FindPlantObjective obj = qs.FindObjective(typeof(FindPlantObjective)) as FindPlantObjective;

					if (obj != null && !obj.Completed)
					{
						FindPlantObjective.PlantEntry entry = obj.GetCurrentPlantEntry();
						if (entry.PlantType == this.GetType())
						{
							OnPicked(from, loc, map, obj);
						}
						else
						{
							player.SendMessage("This is not the reagent you should collect now.");
						}
					}
					else
						player.SendMessage("You see no reason to take that.");
				}
				else
					player.SendMessage("You see no reason to take that.");
			}
		}

		public virtual void OnPicked(Mobile from, Point3D loc, Map map, FindPlantObjective obj)
		{
			PlayerMobile player = from as PlayerMobile;

			if (obj != null)
			{
				ItemID = GetPickedID();
				m_Picked = true;

				FindPlantObjective.PlantEntry entry = obj.GetCurrentPlantEntry();

				obj.CurProgress++;
				player.SendMessage("This looks like the reagent the wizard wanted.");

				if( obj.CurProgress < entry.Amount )
					player.SendMessage("You need to collect {0} more of this plant.", entry.Amount - obj.CurProgress);

				if (NestDisturbChance > Utility.RandomDouble())
				{
					ElderWizardCreatureEntry creatureEntry = m_ElderWizardCreatures[Utility.Random(m_ElderWizardCreatures.Length)];
					int iCreatureAmount = Utility.RandomMinMax(creatureEntry.m_iMinAmount, creatureEntry.m_iMaxAmount);

					bool isElder=false, isPlagued=false;
					if (SpecialNestChance > Utility.RandomDouble())
					{
						if (Utility.RandomBool())
							isElder = true;
						else
							isPlagued = true;
					}

					for (int i = 0; i < iCreatureAmount; i++)
					{
						BaseCreature creature = Activator.CreateInstance(creatureEntry.m_tCreatureType) as BaseCreature;
						creature.Home = this.Location;

						if (isElder)
							creature.IsElder = true;
						else if (isPlagued)
							creature.IsPlagued = true;

						creature.MoveToWorld(this.Location, this.Map);

						// Attack player
						if (from != null)
							creature.Combatant = from;
					}

					string special = "A";
					if (isPlagued)
						special = "A plagued";
					else if (isElder)
						special = "An elder";

					string text = string.Format("{0} {1} nest was disturbed under the plant!", special, creatureEntry.m_sCreatureName);
					this.PublicOverheadMessage(MessageType.Regular, 0, true, text);
				}

				Timer.DelayCall(TimeSpan.FromMinutes(5.0), new TimerCallback(Delete));
			}
		}

		public BaseQuestReagent(Serial serial)
			: base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.WriteEncodedInt(0); // version

			// Version 0
			writer.Write(m_Picked);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadEncodedInt();

			switch (version)
			{
				case 0:
					m_Picked = reader.ReadBool();
					break;
			}

			if (m_Picked)
				Delete();
		}
	}
}