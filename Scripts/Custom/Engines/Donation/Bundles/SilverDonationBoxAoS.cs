using System;
using Server.Mobiles;
using Server.Multis.Deeds;
using Server.Misc;

namespace Server.Items
{
	[DynamicFliping]
	[Flipable( 0x9A8, 0xE80 )]
	public class SilverDonationBoxAoS : MetalBox
	{
		[Constructable]
		public SilverDonationBoxAoS()
		{
			Weight = 1.0;
			Hue = 2401;
			Item item = null;
			Name = "Defiance Gold Member Box of Holding";

			CharacterCreation.PlaceItemIn(this, 16, 60, (item = new DonationSkillBall( 25, false )));
			CharacterCreation.PlaceItemIn(this, 28, 60, (item = new DonationSkillBall( 25, false )));
			CharacterCreation.PlaceItemIn(this, 40, 60, (item = new DonationSkillBall( 25, false )));

			CharacterCreation.PlaceItemIn(this, 16, 81, (item = new HoodedShroudOfShadows()));
			item.Hue = 2401;
			item.Name = "Silver Shroud of Shadows";
						item.LootType = LootType.Blessed;

						BaseContainer cont;
						CharacterCreation.PlaceItemIn(this, 58, 57, (cont = new Backpack()));
			cont.Hue = 2401;
			cont.Name = "a silver bag";

			CharacterCreation.PlaceItemIn(cont, 29, 39, new SulfurousAsh(5000));
			CharacterCreation.PlaceItemIn(cont, 29, 64, new Nightshade(5000));
			CharacterCreation.PlaceItemIn(cont, 29, 89, new SpidersSilk(5000));

			CharacterCreation.PlaceItemIn(cont, 60, 64, new Garlic(5000));
			CharacterCreation.PlaceItemIn(cont, 60, 89, new Ginseng(5000));

			CharacterCreation.PlaceItemIn(cont, 88, 39, new Bloodmoss(5000));
			CharacterCreation.PlaceItemIn(cont, 88, 64, new BlackPearl(5000));
			CharacterCreation.PlaceItemIn(cont, 88, 89, new MandrakeRoot(5000));

			CharacterCreation.PlaceItemIn(this, 103, 58, (item = new Sandals()));
			item.Hue = Utility.RandomList(5, 70, 90, 110);
			item.LootType = LootType.Blessed;

			CharacterCreation.PlaceItemIn(this, 122, 53, new SpecialDonateDye());

			CharacterCreation.PlaceItemIn(this, 156, 55, (item = new EtherealHorse()));
			item.Hue = 2401;
			item.Name = "No Aged Ethereal";
			((EtherealMount)item).IsDonationItem = true;

			CharacterCreation.PlaceItemIn(this, 43, 83, (item = new ClothingBlessDeed()));
			item.Hue = 2401;
			CharacterCreation.PlaceItemIn(this, 63, 83, (item = new KillResetDeedAOS())); //by Blady
			item.Hue = 2213;
			CharacterCreation.PlaceItemIn(this, 83, 83, (item = new SmallBrickHouseDeed()));
			item.Hue = 2401;
			CharacterCreation.PlaceItemIn(this, 103, 83, (item = new NameChangeDeed()));
			item.Hue = 2401;
			CharacterCreation.PlaceItemIn(this, 123, 83, (item = new AntiBlessDeed()));
			item.Hue = 2401;
			CharacterCreation.PlaceItemIn(this, 143, 83, (item = new MembershipTicket()));
			item.Hue = 2213;
			((MembershipTicket)item).MemberShipTime = TimeSpan.MaxValue;

		}

		public SilverDonationBoxAoS( Serial serial ) : base( serial )
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