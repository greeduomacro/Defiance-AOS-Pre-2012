using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;
using Server.Items;
using Server.Engines.Quests;

namespace Server.Engines.Quests.ElderWizard
{
	public class GabrielleTheElderWizard : BaseQuester
	{
		public override bool ClickTitle { get { return true; } }
		public override bool IsActiveVendor { get { return true; } }

		public override void InitSBInfo()
		{
			m_SBInfos.Add(new SBGabrielleTheElderWizard());
		}

		[Constructable]
		public GabrielleTheElderWizard()
			: base("the Elder Wizard")
		{
		}

		public override void InitBody()
		{
			InitStats(100, 100, 25);

			Hue = 33770;
			Female = true;
			Body = 0x191;
			Name = "Gabrielle";
		}

		public override void InitOutfit()
		{
			AddItem(new Robe(0x482));
			AddItem(new Sandals(0x482));
			AddItem(new WizardsHat(0x482));

			HairItemID = 0x203C;

			Item staff = new BlackStaff();
			staff.Movable = false;
			AddItem(staff);
		}

		public static bool RefillBag(BaseVendor vendor, Mobile from, Item dropped)
		{
			if (dropped is QuestReagentBag)
			{
				QuestReagentBag regBag = (QuestReagentBag)dropped;
				List<RefillEntry> refillEntryList = ERefillUtility.Refill(regBag, regBag.ReagentTypes, vendor, from, false, regBag.BagRefillAmount);
				//int cost = 0;
				int amount = 0;

				foreach (RefillEntry entry in refillEntryList)
					amount += entry.AmountToRefill;
					//cost += entry.TotalCost;

				if (amount <= 0)
					vendor.Say("That bag seems to be full.");
				else
					from.SendGump(new RefillGump(vendor, from, (QuestReagentBag)dropped, refillEntryList));
				return true;
			}
			return false;
		}

		public override bool OnDragDrop(Mobile from, Item dropped)
		{
			if (GabrielleTheElderWizard.RefillBag(this, from, dropped))
				return false;

			return base.OnDragDrop(from, dropped);
		}

		public override void OnTalk(PlayerMobile player, bool contextMenu)
		{
			Direction = GetDirectionTo(player);
			QuestSystem qs = player.Quest;

			// Doing this quest
			if (qs is ElderWizardQuest)
			{
				if (qs.IsObjectiveInProgress(typeof(FindPlantObjective)))
				{
					qs.AddConversation(new DuringFindPlantConversation());
				}
				else
				{
					QuestObjective obj = qs.FindObjective(typeof(ReportBackObjective));

					if (obj != null && !obj.Completed)
					{
						QuestReagentBag.ReagentBagType bagType = QuestReagentBag.ReagentBagType.Mage;

						if (player.Skills[SkillName.Necromancy].Base > player.Skills[SkillName.Magery].Base)
							bagType = QuestReagentBag.ReagentBagType.Necro;
						else if (player.Skills[SkillName.Necromancy].Base == player.Skills[SkillName.Magery].Base && Utility.RandomBool() )
							bagType = QuestReagentBag.ReagentBagType.Necro;

						Bag rewardBag = new QuestReagentBag(bagType);

						if (player.PlaceInBackpack(rewardBag))
							obj.Complete();
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
				else if (qs == null && QuestSystem.CanOfferQuest(player, typeof(ElderWizardQuest)))
				{
					Direction = GetDirectionTo(player);
					new ElderWizardQuest(player).SendOffer();
				}
			}
		}

		public GabrielleTheElderWizard(Serial serial)
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