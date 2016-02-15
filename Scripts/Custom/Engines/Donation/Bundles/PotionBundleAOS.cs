using System;
using Server.Mobiles;
using Server.Multis.Deeds;
using Server.Misc;

namespace Server.Items
{
	[DynamicFliping]
	[Flipable(0x9A8, 0xE80)]
	public class PotionBundleAoS : MetalChest
	{
		[Constructable]
		public PotionBundleAoS()
		{
			Weight = 1.0;
			Hue = 0;
			Name = "Box of Potions";

			PotionKeg keg;
			CharacterCreation.PlaceItemIn(this, 18, 105, (keg = new PotionKeg()));
			keg.Type = PotionEffect.HealGreater;
			keg.Held = 100;
			keg.Hue = 54;
			CharacterCreation.PlaceItemIn(this, 23, 105, (keg = new PotionKeg()));
			keg.Type = PotionEffect.HealGreater;
			keg.Held = 100;
			keg.Hue = 54;
			CharacterCreation.PlaceItemIn(this, 28, 105, (keg = new PotionKeg()));
			keg.Type = PotionEffect.HealGreater;
			keg.Held = 100;
			keg.Hue = 54;
			CharacterCreation.PlaceItemIn(this, 33, 105, (keg = new PotionKeg()));
			keg.Type = PotionEffect.HealGreater;
			keg.Held = 100;
			keg.Hue = 54;
			CharacterCreation.PlaceItemIn(this, 38, 105, (keg = new PotionKeg()));
			keg.Type = PotionEffect.HealGreater;
			keg.Held = 100;
			keg.Hue = 54;
			CharacterCreation.PlaceItemIn(this, 58, 105, (keg = new PotionKeg()));
			keg.Type = PotionEffect.CureGreater;
			keg.Held = 100;
			keg.Hue = 43;
			CharacterCreation.PlaceItemIn(this, 63, 105, (keg = new PotionKeg()));
			keg.Type = PotionEffect.CureGreater;
			keg.Held = 100;
			keg.Hue = 43;
			CharacterCreation.PlaceItemIn(this, 68, 105, (keg = new PotionKeg()));
			keg.Type = PotionEffect.CureGreater;
			keg.Held = 100;
			keg.Hue = 43;
			CharacterCreation.PlaceItemIn(this, 73, 105, (keg = new PotionKeg()));
			keg.Type = PotionEffect.CureGreater;
			keg.Held = 100;
			keg.Hue = 43;
			CharacterCreation.PlaceItemIn(this, 78, 105, (keg = new PotionKeg()));
			keg.Type = PotionEffect.CureGreater;
			keg.Held = 100;
			keg.Hue = 43;
			CharacterCreation.PlaceItemIn(this, 98, 105, (keg = new PotionKeg()));
			keg.Type = PotionEffect.RefreshTotal;
			keg.Held = 100;
			keg.Hue = 38;
			CharacterCreation.PlaceItemIn(this, 103, 105, (keg = new PotionKeg()));
			keg.Type = PotionEffect.RefreshTotal;
			keg.Held = 100;
			keg.Hue = 38;
			CharacterCreation.PlaceItemIn(this, 108, 105, (keg = new PotionKeg()));
			keg.Type = PotionEffect.RefreshTotal;
			keg.Held = 100;
			keg.Hue = 38;
			CharacterCreation.PlaceItemIn(this, 18, 129, (keg = new PotionKeg()));
			keg.Type = PotionEffect.StrengthGreater;
			keg.Held = 100;
			keg.Hue = 1001;
			CharacterCreation.PlaceItemIn(this, 28, 129, (keg = new PotionKeg()));
			keg.Type = PotionEffect.StrengthGreater;
			keg.Held = 100;
			keg.Hue = 1001;
			CharacterCreation.PlaceItemIn(this, 48, 129, (keg = new PotionKeg()));
			keg.Type = PotionEffect.AgilityGreater;
			keg.Held = 100;
			keg.Hue = 99;
			CharacterCreation.PlaceItemIn(this, 58, 129, (keg = new PotionKeg()));
			keg.Type = PotionEffect.AgilityGreater;
			keg.Held = 100;
			keg.Hue = 99;
			CharacterCreation.PlaceItemIn(this, 88, 129, (keg = new PotionKeg()));
			keg.Type = PotionEffect.ExplosionGreater;
			keg.Held = 100;
			keg.Hue = 15;
			CharacterCreation.PlaceItemIn(this, 98, 129, (keg = new PotionKeg()));
			keg.Type = PotionEffect.PoisonDeadly;
			keg.Held = 100;
			keg.Hue = 62;
		}

		public PotionBundleAoS(Serial serial)
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