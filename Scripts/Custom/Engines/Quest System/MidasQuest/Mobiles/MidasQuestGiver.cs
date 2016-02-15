using System;
using Server;
using Server.Mobiles;
using Server.Items;
using Server.Engines.Quests;

namespace Server.Engines.Quests.Midas
{
    public class MidasQuestGiver : BaseQuester
	{
		[Constructable]
		public MidasQuestGiver() : base()
		{
		}

		public MidasQuestGiver( Serial serial ) : base( serial )
		{
		}

		public override void InitBody()
		{
			InitStats( 100, 100, 25 );

			Hue = 0x83ED;

			Female = false;
            		HairHue = 0x1F4;
            		HairItemID = 0x203C;
            		Body = 0x190;
			Name = "Nimre slayer of evil";
            //Direction = south;
		}

/************************************************************
		private SummoningAltarErmo m_Altar;

		private const int AltarRange = 24;

		public SummoningAltarErmo Altar
		{
			get
			{
				if ( m_Altar == null || m_Altar.Deleted || m_Altar.Map != this.Map || !Utility.InRange( m_Altar.Location, this.Location, AltarRange ) )
				{
					foreach ( Item item in GetItemsInRange( AltarRange ) )
					{
						if ( item is SummoningAltarErmo )
						{
							m_Altar = (SummoningAltarErmo)item;
							break;
						}
					}
				}

				return m_Altar;
			}
		}
*****************************************************/
		public override void InitOutfit()
		{

			PlateArms arms = new PlateArms();
			arms.LootType = LootType.Newbied;
			arms.Hue = 1153;
			AddItem( arms );

			PlateGloves gloves = new PlateGloves();
			gloves.LootType = LootType.Newbied;
			gloves.Hue = 1153;
			AddItem( gloves );

			PlateLegs legs = new PlateLegs();
			legs.LootType = LootType.Newbied;
			legs.Hue = 1153;
			AddItem( legs );

			PlateGorget neck = new PlateGorget();
			neck.LootType = LootType.Newbied;
			neck.Hue = 1153;
			AddItem( neck );

			PlateChest chest = new PlateChest();
			chest.LootType = LootType.Newbied;
			chest.Hue = 1153;
			AddItem( chest );

            OrderShield shield = new OrderShield();
            shield.LootType = LootType.Newbied;
            shield.Hue = 1153;
            AddItem( shield );

			AddItem( new PonyTail( 1000 ) );
			AddItem( new Goatee( 1000 ) );
		}

		public override void OnTalk( PlayerMobile player, bool contextMenu )
		{
			Direction = GetDirectionTo( player );

			QuestSystem qs = player.Quest;

            if (qs is MidasQuest)
            {
                if (qs.IsObjectiveInProgress(typeof(HeadlessObjective)))
                {
                    qs.AddConversation(new DuringHeadlessConversation());
                }
                else if (qs.IsObjectiveInProgress(typeof(ZombieObjective)))
                {
                 qs.AddConversation(new DuringHeadlessConversation());
                }
                else if (qs.IsObjectiveInProgress(typeof(SkeletonObjective)))
                {
                    qs.AddConversation(new DuringHeadlessConversation());
                }
		else if (qs.IsObjectiveInProgress(typeof(Skeleton1Objective)))
                {
                    qs.AddConversation(new DuringHeadlessConversation());
                }
		else if (qs.IsObjectiveInProgress(typeof(AbysmalObjective)))
                {
                    qs.AddConversation(new DuringHeadlessConversation());
                }
		else if (qs.IsObjectiveInProgress(typeof(DFObjective)))
                {
                    qs.AddConversation(new DuringHeadlessConversation());
                }
                else if (qs.IsObjectiveInProgress(typeof(ReturnToPriestObjective)))
                {
                    //
                    QuestObjective obj = qs.FindObjective(typeof(ReturnToPriestObjective));

                    if (obj != null && !obj.Completed)
                    {
                        obj.Complete();
                    }
                    //

                    qs.AddConversation(new ZombieConversation());
                }
                else if (qs.IsObjectiveInProgress(typeof(ReturnToPriest2Objective)))
                {
                    //
                    QuestObjective obj = qs.FindObjective(typeof(ReturnToPriest2Objective));

                    if (obj != null && !obj.Completed)
                    {
                        obj.Complete();
                    }
                    //

                    qs.AddConversation(new SkeletonConversation());
                }
		else if (qs.IsObjectiveInProgress(typeof(ReturnToPriest3Objective)))
                {
                    //
                    QuestObjective obj = qs.FindObjective(typeof(ReturnToPriest3Objective));

                    if (obj != null && !obj.Completed)
                    {
                        obj.Complete();
                    }
                    //

                    qs.AddConversation(new Skeleton1Conversation());
                }
		else if (qs.IsObjectiveInProgress(typeof(ReturnToPriest4Objective)))
                {
                    //
                    QuestObjective obj = qs.FindObjective(typeof(ReturnToPriest4Objective));

                    if (obj != null && !obj.Completed)
                    {
                        obj.Complete();
                    }
                    //

                    qs.AddConversation(new AbysmalConversation());
                }
		else if (qs.IsObjectiveInProgress(typeof(ReturnToPriest5Objective)))
                {
                    //
                    QuestObjective obj = qs.FindObjective(typeof(ReturnToPriest5Objective));

                    if (obj != null && !obj.Completed)
                    {
                        obj.Complete();
                    }
                    //

                    qs.AddConversation(new DFConversation());
                }

                else if (qs.IsObjectiveInProgress(typeof(ReturnToPriest6Objective)))
                {
                    //
                    QuestObjective obj = qs.FindObjective(typeof(ReturnToPriest6Objective));

                    if (obj != null && !obj.Completed)
                    {
                        obj.Complete();
                    }
                    //

                    qs.AddConversation(new EndConversation());
                }
            }
            else
            {
                QuestSystem newQuest = new MidasQuest(player);

                if (qs == null && QuestSystem.CanOfferQuest(player, typeof(MidasQuest)))
                {
                    newQuest.SendOffer();
                }

                {
                    //newQuest.AddConversation(new DontOfferConversation());
                }
            }
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}