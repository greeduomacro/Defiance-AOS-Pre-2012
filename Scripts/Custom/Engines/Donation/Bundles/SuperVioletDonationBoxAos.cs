using System;
using Server.Mobiles;
using Server.Multis.Deeds;
using Server.Misc;

namespace Server.Items
{
	[DynamicFliping]
	[Flipable( 0x9A8, 0xE80 )]
	public class SuperVioletDonationBoxAos : MetalBox
	{
		[Constructable]
		public SuperVioletDonationBoxAos()
		{
			Weight = 1.0;
			Hue = 1376;
			Item item = null;
			Name = "Blast Violet Member Box of Holding";

			CharacterCreation.PlaceItemIn(this, 16, 60, (item = new DonationSkillBall( 25, false )));
			CharacterCreation.PlaceItemIn(this, 28, 60, (item = new DonationSkillBall( 25, false )));
			CharacterCreation.PlaceItemIn(this, 40, 60, (item = new DonationSkillBall( 25, false )));

			CharacterCreation.PlaceItemIn(this, 16, 81, (item = new HoodedShroudOfShadows()));
			item.Hue = 1376;
			item.Name = "Blast Violet Shroud of Shadows";
						item.LootType = LootType.Blessed;

			BaseContainer cont;
			CharacterCreation.PlaceItemIn(this, 58, 57, (cont = new Backpack()));
			cont.Hue = 1376;
			cont.Name = "a donation backpack";

			CharacterCreation.PlaceItemIn(cont, 44, 65, new SulfurousAsh(10000));
			CharacterCreation.PlaceItemIn(cont, 77, 65, new Nightshade(10000));
			CharacterCreation.PlaceItemIn(cont, 110, 65, new SpidersSilk(10000));
			CharacterCreation.PlaceItemIn(cont, 143, 65, new Garlic(10000));

			CharacterCreation.PlaceItemIn(cont, 44, 128, new Ginseng(10000));
			CharacterCreation.PlaceItemIn(cont, 77, 128, new Bloodmoss(10000));
			CharacterCreation.PlaceItemIn(cont, 110, 128, new BlackPearl(10000));
			CharacterCreation.PlaceItemIn(cont, 143, 128, new MandrakeRoot(10000));

			//CharacterCreation.PlaceItemIn(this, 74, 64, new DonationBandana());
			//Replaced the bandana with a deed - Edit by Blady
			CharacterCreation.PlaceItemIn(this, 74, 64, (item = new DonationDeed()));
			item.Hue = 1376;

			CharacterCreation.PlaceItemIn(this, 103, 58, (item = new Sandals()));
			item.Hue = 1376; //Utility.RandomList(1376, 1281, 1161, 1376, 1376, 1167, 1266, 1420, 1109, 1645);
			item.LootType = LootType.Blessed;

			CharacterCreation.PlaceItemIn( this, 122, 53, new SpecialDonateDye() );
			CharacterCreation.PlaceItemIn(this, 11376, 53, (item = new PigmentsOfTokuno( 5 )));
			((PigmentsOfTokuno)item).Type = PigmentType.VioletCouragePurple;
			CharacterCreation.PlaceItemIn(this, 156, 55, (item = new EtherealHorse()));
			item.Hue = 1376;
			item.Name = "No Age Ethereal";
			((EtherealMount)item).IsDonationItem = true;

			CharacterCreation.PlaceItemIn(this, 34, 83, (item = new HolyDeedofBlessing()));
			item.Hue = 1376;
			CharacterCreation.PlaceItemIn(this, 43, 83, (item = new ClothingBlessDeed()));
			item.Hue = 1376;
			CharacterCreation.PlaceItemIn(this, 58, 83, (item = new TreasureMap(6, Map.Felucca)));
			item.Hue = 1376;
			CharacterCreation.PlaceItemIn(this, 73, 83, (item = new SmallBrickHouseDeed()));
			item.Hue = 1376;
			CharacterCreation.PlaceItemIn(this, 88, 83, (item = new NameChangeDeed()));
			item.Hue = 1376;
			CharacterCreation.PlaceItemIn(this, 103, 83, (item = new MiniHouseDeed()));
			item.Hue = 1376;
			//CharacterCreation.PlaceItemIn(this, 118, 83, (item = new BankCheck(100000)));
			//item.Hue = 1376;
			CharacterCreation.PlaceItemIn(this, 130, 83, (item = new MembershipTicket()));
			item.Hue = 1376;
			((MembershipTicket)item).MemberShipTime = TimeSpan.MaxValue;
		}

		public SuperVioletDonationBoxAos( Serial serial ) : base( serial )
		{
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