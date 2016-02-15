using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;
using Server.Items;
using Server.Engines.Quests;

namespace Server.Engines.Quests.QuestionableAlchemist
{
    public class EldaTheQuestionableAlchemist : BaseQuester
    {
        public override bool ClickTitle { get { return true; } }
        public override bool IsActiveVendor { get { return true; } }

        public override void InitSBInfo()
        {
            m_SBInfos.Add(new SBEldaTheQuestionableAlchemist());
        }

        [Constructable]
        public EldaTheQuestionableAlchemist()
            : base("the Questionable Alchemist")
        {
        }

        public override void InitBody()
        {
            InitStats(100, 100, 25);

            Hue = 33770;
            Female = true;
            Body = 0x191;
            Name = "Elda";
        }

        public override void InitOutfit()
        {
            AddItem(new FancyDress());
            AddItem(new Shoes());
            AddItem(new HalfApron());

            HairItemID = 0x203C;
            HairHue = 0x24;
        }

        public override bool OnDragDrop(Mobile from, Item dropped)
        {
            if (from is PlayerMobile && dropped is BloodBottle)
            {
                PlayerMobile player = (PlayerMobile)from;
                BloodBottle bloodBottle = (BloodBottle)dropped;

                Direction = GetDirectionTo(player);
                QuestSystem qs = player.Quest;

                if (qs is QuestionableAlchemistQuest)
                {
                    if (qs.IsObjectiveInProgress(typeof(CollectBloodObjective)))
                    {
                        CollectBloodObjective obj = (CollectBloodObjective)qs.FindObjective(typeof(CollectBloodObjective));
                        if (obj != null && obj.MonsterType != null && obj.MonsterType.Monster == bloodBottle.CreatureType)
                        {
                            if (bloodBottle.ID == 1003)
                            {
                                obj.CurProgress++;
                                this.PlaySound(0x240);

                                if (obj.IsAllCompleted && AddReward(player))
                                {
                                    qs.AddConversation(new EndConversation());
                                    qs.Complete();
                                }
                                else
                                {
                                    if (Utility.RandomBool())
                                        Say(1010457); // Thank ye!
                                    else
                                        Say(501840); // Thanks.
                                }

                                return true;
                            }
                            else
                                Say("That blood does not look mutated.");
                        }
                        else
                            Say("That is not the correct blood type.");
                    }
                }

                return false;
            }

            return base.OnDragDrop(from, dropped);
        }

        private bool AddReward(PlayerMobile player)
        {
            Bag rewardBag = new Bag();

            rewardBag.Hue = Utility.RandomDyedHue();
            LootPackEntry.AddRandomLoot(rewardBag, 5, 50, 5, 5, 50, 100);
            rewardBag.DropItem(new Gold(2000, 4000));

            if (0.05 > Utility.RandomDouble())
                rewardBag.DropItem(new BloodPentagramPartDeed());

            if (player.PlaceInBackpack(rewardBag))
                return true;
            else
            {
                rewardBag.Delete();
                player.SendLocalizedMessage(1046260); // You need to clear some space in your inventory to continue with the quest.  Come back here when you have more space in your inventory.
                return false;
            }
        }

        public override void OnTalk(PlayerMobile player, bool contextMenu)
        {
            Direction = GetDirectionTo(player);
            QuestSystem qs = player.Quest;

            // Doing this quest
            if (qs is QuestionableAlchemistQuest)
            {
                if (qs.IsObjectiveInProgress(typeof(CollectBloodObjective)))
                {
                    qs.AddConversation(new DuringCollectBloodConversation());
                }
                else
                {
                    CollectBloodObjective obj = (CollectBloodObjective)qs.FindObjective(typeof(CollectBloodObjective));

                    if (obj != null && obj.IsAllCompleted)
                    {
                        if (AddReward(player))
                        {
                            qs.AddConversation(new EndConversation());
                            qs.Complete();
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
                else if (qs == null && QuestSystem.CanOfferQuest(player, typeof(QuestionableAlchemistQuest)))
                {
                    Direction = GetDirectionTo(player);
                    new QuestionableAlchemistQuest(player).SendOffer();
                }
            }
        }

        public EldaTheQuestionableAlchemist(Serial serial)
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