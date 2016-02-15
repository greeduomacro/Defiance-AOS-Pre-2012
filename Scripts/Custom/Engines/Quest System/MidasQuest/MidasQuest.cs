
using System;
using Server;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.Quests.Midas
{
	public class MidasQuest : QuestSystem
	{
		private static Type[] m_TypeReferenceTable = new Type[]
			{
				typeof( Midas.HeadlessConversation ),
				typeof( Midas.HeadlessObjective ),
                typeof( Midas.ZombieConversation ),
				typeof( Midas.ZombieObjective ),
                typeof( Midas.SkeletonConversation ),
				typeof( Midas.SkeletonObjective ),
				typeof( Midas.Skeleton1Conversation ),
				typeof( Midas.Skeleton1Objective ),
				typeof( Midas.AbysmalConversation ),
				typeof( Midas.AbysmalObjective ),
				typeof( Midas.DFConversation ),
				typeof( Midas.DFObjective ),
				typeof( Midas.ReturnToPriestObjective ),
                typeof( Midas.ReturnToPriest2Objective ),
                typeof( Midas.ReturnToPriest3Objective ),
				typeof( Midas.ReturnToPriest4Objective ),
				typeof( Midas.ReturnToPriest5Objective ),
				typeof( Midas.ReturnToPriest6Objective ),
				typeof( Midas.DuringHeadlessConversation ),
				typeof( Midas.EndConversation ),
                /*typeof( Midas.DontOfferConversation ),*/
				typeof( Midas.DeclineConversation )
			};

		public override Type[] TypeReferenceTable{ get{ return m_TypeReferenceTable; } }

		public override object Name
		{
			get
			{
				return "Slayer of evil!";
			}
		}

		public override object OfferMessage
		{
			get
			{
				return "<i>Nimre starts to speak.</i><BR><BR>Pssst... I have a noble quest for you! Listen... A demon stole my lunch money this morning :( <BR><BR>Can you help me get revenge, please?";
			}
		}

        public override TimeSpan RestartDelay { get { return TimeSpan.Zero; } } //This makes the quest repeatable
		public override bool IsTutorial{ get{ return false; } }

		public override int Picture{ get{ return 0x15A9; } }
		//public override int Picture{ get{ return 0x15C9; } }
		public MidasQuest( PlayerMobile from ) : base( from )
		{
		}

		// Serialization
        public MidasQuest()
		{
		}

		public override void Accept()
		{
			base.Accept();

			AddConversation( new HeadlessConversation() );
		}

		public override void Decline()
		{
			base.Decline();

			AddConversation( new DeclineConversation() );
		}
//		/*
//		public static void GiveRewardTo( PlayerMobile player )
//		{
//			player.PlaceInBackpack( new RewardBag() );
//			//Item reward = new PowderOfTemperament();
//			//player.PlaceInBackpack( reward );
//			/*
//			//if( Utility.RandomDouble() < 0.95 )
//				//{
//				player.PlaceInBackpack( new RewardBag() );
//				//player.PlaceInBackpack( new BallOfSummoning() );
//				//player.PlaceInBackpack( new MidasTouch() );
//				//PackItem( new BallOfSummoning() );
//				//}
//			*/
//			Map map = player.Map;
//			DemonKnight daemon = new DemonKnight();
//
//					daemon.MoveToWorld( player.Location, map );
//		}*/
		public static void GiveRewardTo( PlayerMobile player )
		{
			Map map = player.Map;
			Bag rewardBag = new Bag();
			rewardBag.Hue = Utility.RandomDyedHue();
			LootPackEntry.AddRandomLoot(rewardBag, 5, 600,  5, 5, 50, 100);
			rewardBag.DropItem(new BankCheck( 5000 ));
			if (0.08 > Utility.RandomDouble())
					rewardBag.DropItem(new TreasureMap(6, Map.Felucca));
				else
					rewardBag.DropItem(new TreasureMap(Utility.RandomMinMax(3, 5), Map.Felucca));
			//Add arty
			if (0.1 > Utility.RandomDouble())
			{
			int rnd = Utility.Random(4);
			switch (rnd)
			{
				case 0: rewardBag.DropItem(new YetisPads()); break; //Yetis pads
				case 1: rewardBag.DropItem(new Necronomicon()); break;
				case 2: rewardBag.DropItem(new Behemoth()); break;
				case 3: rewardBag.DropItem(new DragonClenchingClaws()); break;
				//case 4: rewardBag.DropItem(CreatePaladinArmor(typeof(PlateGorget), "Platemail Gorget", Utility.Random(5))); break;
			}
				if (0.80 > Utility.RandomDouble())
				{
				rewardBag.DropItem(new MidasTouch());
				}

			}
			else if (0.80 > Utility.RandomDouble())
			{
				rewardBag.DropItem(new MidasTouch());
			}
			//player.PlaceInBackpack( rewardBag );
			//if(!player.KarmaLocked)
				//player.Karma = Math.Min(15000, player.Karma + 1500);

			//player.SendMessage( "Reward bag has been placed in your backpack." );
			if (player.PlaceInBackpack(rewardBag))
						{
							//obj.Complete();
							player.SendMessage( "Reward bag has been placed in your backpack." );
							if (!player.KarmaLocked)
								player.Karma = Math.Min(15000, player.Karma + 1500);
						}
						else
						{
							if (!player.KarmaLocked)
								player.Karma = Math.Min(15000, player.Karma + 1500);
							player.SendMessage( "Your backpack if full, reward is on the ground." );
							rewardBag.MoveToWorld( player.Location, map );
						}
		}
	}
}