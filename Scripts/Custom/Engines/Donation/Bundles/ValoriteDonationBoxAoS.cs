using System;
using Server.Mobiles;
using Server.Multis.Deeds;
using Server.Misc;

namespace Server.Items
{
	[DynamicFliping]
	[Flipable( 0x9A8, 0xE80 )]
	public class ValoriteDonationBoxAoS : MetalBox
	{
		[Constructable]
		public ValoriteDonationBoxAoS()
		{
			Weight = 1.0;
			Hue = 2119;
			Item item = null;
			Name = "Defiance Valorite Member Box of Holding";

			CharacterCreation.PlaceItemIn(this, 16, 60, (item = new DonationSkillBall( 25, false )));
			CharacterCreation.PlaceItemIn(this, 28, 60, (item = new DonationSkillBall( 25, false )));
			CharacterCreation.PlaceItemIn(this, 40, 60, (item = new DonationSkillBall( 25, false )));

			CharacterCreation.PlaceItemIn(this, 18, 80, (item = new HoodedShroudOfShadows()));
			item.Hue = 2119;
			item.Name = "Valorite Shroud of Shadows";
			item.LootType = LootType.Blessed;

			BaseContainer cont;
			CharacterCreation.PlaceItemIn(this, 64, 50, (cont = new Backpack()));
			cont.Hue = 2219;
			cont.Name = "a valorite backpack";

			CharacterCreation.PlaceItemIn(cont, 44, 65, new SulfurousAsh(10000));
			CharacterCreation.PlaceItemIn(cont, 77, 65, new Nightshade(10000));
			CharacterCreation.PlaceItemIn(cont, 110, 65, new SpidersSilk(10000));
			CharacterCreation.PlaceItemIn(cont, 143, 65, new Garlic(10000));

			CharacterCreation.PlaceItemIn(cont, 44, 128, new Ginseng(10000));
			CharacterCreation.PlaceItemIn(cont, 77, 128, new Bloodmoss(10000));
			CharacterCreation.PlaceItemIn(cont, 110, 128, new BlackPearl(10000));
			CharacterCreation.PlaceItemIn(cont, 143, 128, new MandrakeRoot(10000));

			CharacterCreation.PlaceItemIn(this, 93, 60, new SpecialDonateDye());

			CharacterCreation.PlaceItemIn(this, 50, 80, new ClothingBlessDeed());
			//CharacterCreation.PlaceItemIn(this, 60, 80, new GuildDeed());
			CharacterCreation.PlaceItemIn(this, 70, 80, new SmallBrickHouseDeed());
			CharacterCreation.PlaceItemIn(this, 80, 80, new NameChangeDeed());

			CharacterCreation.PlaceItemIn(this, 90, 80, (item = new MembershipTicket()));
			((MembershipTicket)item).MemberShipTime = TimeSpan.FromDays(90);

			CharacterCreation.PlaceItemIn(this, 110, 50, new BankCheck(50000));
		}

		public ValoriteDonationBoxAoS( Serial serial ) : base( serial )
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