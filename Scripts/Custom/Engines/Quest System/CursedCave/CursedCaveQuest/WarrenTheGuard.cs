using System;
using Server;
using Server.Mobiles;
using Server.Network;
using Server.Items;
using Server.Engines.Quests;
using System.Collections;

namespace Server.Engines.Quests.CursedCave
{
	public class WarrenTheGuard : BaseQuester
	{
		public virtual int ArtifactAmount { get { return 10; } }

		[Constructable]
		public WarrenTheGuard()
			: base("the Paladin Guard")
		{
		}

		public override void InitBody()
		{
			InitStats(100, 100, 25);

			Hue = 33770;
			Female = false;
			Body = 0x190;
			Name = "Warren";

			HairItemID = 0x203B;
			HairHue = 0x44E;
		}

		private BaseArmor CreatePaladinArmor(Type type, string sName, int props)
		{
			BaseArmor baArmor = (BaseArmor)Activator.CreateInstance(type);
			BaseRunicTool.ApplyAttributesTo(baArmor, props, 40, 80);
			baArmor.Hue = 0x528;

			baArmor.Name = string.Format("{0} of the Paladin", sName);
			baArmor.SkillBonuses.SetValues(0, SkillName.Chivalry, 5.0);
			return baArmor;
		}

		public override void InitOutfit()
		{
			AddItem(CreatePaladinArmor(typeof(PlateChest), "Platemail Tunic", 5));
			AddItem(CreatePaladinArmor(typeof(PlateArms), "Platemail Arms", 5));
			AddItem(CreatePaladinArmor(typeof(PlateGloves), "Platemail Gloves", 5));
			AddItem(CreatePaladinArmor(typeof(PlateLegs), "Platemail Legs", 5));
			AddItem(CreatePaladinArmor(typeof(PlateGorget), "Platemail Gorget", 5));
			AddItem(new Cloak(1150));

			AddItem(new LongswordOfJustice());
			AddItem(new OrderShield());
		}

		public override void OnMovement(Mobile m, Point3D oldLocation)
		{
			if (m.Alive && m is PlayerMobile)
			{
				PlayerMobile pm = (PlayerMobile)m;

				int range = 4;

				if (m.Alive && range >= 0 && InRange(m, range) && !InRange(oldLocation, range))
					OnRareTalk(pm);
			}
		}

		public void OnRareTalk(PlayerMobile player)
		{
			ArrayList foundItems = CursedCaveUtility.GetCCRares(player, ArtifactAmount);

			if (foundItems.Count >= ArtifactAmount)
				player.SendGump(new CursedCaveUtility.CCRaresGump(foundItems, this, player));
			else if (foundItems.Count > 0)
				this.Say(string.Format("I found {0} of the lost treasures of the Cursed Cave in your backpack, bring me {1} of them to get a reward.", foundItems.Count, ArtifactAmount));
		}

		public override void OnTalk(PlayerMobile player, bool contextMenu)
		{
			Direction = GetDirectionTo(player);
			QuestSystem qs = player.Quest;

			// Doing this quest
			if (qs is CursedCaveQuest)
			{
				if (qs.IsObjectiveInProgress(typeof(CursedCaveBeginObjective)))
				{
					qs.AddConversation(new DuringKillTaskConversation());
				}
				else
				{
					QuestObjective obj = qs.FindObjective(typeof(ReportBackObjective));

					if (obj != null && !obj.Completed)
					{
						Bag rewardBag = new Bag();
						rewardBag.Hue = Utility.RandomDyedHue();
						LootPackEntry.AddRandomLoot(rewardBag, 5, 600,  5, 5, 50, 100);
						rewardBag.DropItem(new Bandage(Utility.RandomMinMax(200, 300)));
						rewardBag.DropItem(new Gold(2000, 4000));
						if (0.08 > Utility.RandomDouble())
							rewardBag.DropItem(new TreasureMap(6, Map.Felucca));
						else
							rewardBag.DropItem(new TreasureMap(Utility.RandomMinMax(3, 5), Map.Felucca));

						// Add Artifact ***
						if (0.05 > Utility.RandomDouble())
						{
							if (GetBestFightingSkill(player) == SkillName.Archery)
								rewardBag.DropItem(new BowOfHephaestus());
							else
								rewardBag.DropItem(new LongswordOfJustice());
						}
						else if (0.2 > Utility.RandomDouble())
						{
							int rnd = Utility.Random(5);
							switch (rnd)
							{
								case 0: rewardBag.DropItem(CreatePaladinArmor(typeof(PlateChest), "Platemail Tunic", Utility.Random(5))); break; //Up to 4 props + Chivalry
								case 1: rewardBag.DropItem(CreatePaladinArmor(typeof(PlateArms), "Platemail Arms", Utility.Random(5))); break;
								case 2: rewardBag.DropItem(CreatePaladinArmor(typeof(PlateGloves), "Platemail Gloves", Utility.Random(5))); break;
								case 3: rewardBag.DropItem(CreatePaladinArmor(typeof(PlateLegs), "Platemail Legs", Utility.Random(5))); break;
								case 4: rewardBag.DropItem(CreatePaladinArmor(typeof(PlateGorget), "Platemail Gorget", Utility.Random(5))); break;
							}
						}

						if (player.PlaceInBackpack(rewardBag))
						{
							obj.Complete();
							if (!player.KarmaLocked)
								player.Karma = Math.Min(15000, player.Karma + 1500);
						}
						else
						{
							rewardBag.Delete();
							player.SendLocalizedMessage(1046260); // You need to clear some space in your inventory to continue with the quest.  Come back here when you have more space in your inventory.
						}
					}
				}
			}
			else
			{
				// Busy with another quest
				if (qs != null)
				{
					qs.AddConversation(new DontOfferConversation());
				}
				// Offer Quest
				else if (qs == null && QuestSystem.CanOfferQuest(player, typeof(CursedCaveQuest)))
				{
					Direction = GetDirectionTo(player);
					new CursedCaveQuest(player).SendOffer();
				}
			}
		}

		private static SkillName[] snFightingSkills = new SkillName[]
				{
					SkillName.Fencing,
					SkillName.Macing,
					SkillName.Swords,
					SkillName.Archery,
				};

		public static SkillName GetBestFightingSkill(Mobile from)
		{
			double iHighestSkill = -1;
			SkillName skillName = 0;

			for (int i = 0; i < snFightingSkills.Length; i++)
			{
				if (from.Skills[snFightingSkills[i]].Base > iHighestSkill)
				{
					iHighestSkill = from.Skills[snFightingSkills[i]].Base;
					skillName = snFightingSkills[i];
				}
			}

			return skillName;
		}

		public WarrenTheGuard(Serial serial)
			: base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write((int)0); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();
		}
	}
}