using System;
using Server.Items;

namespace Server.Events.CTF
{
	public class CTFRewardKatana : Katana
	{
		public CTFRewardKatana() : base()
		{
			Name = Name + "[CTF-Item]";
			LootType = LootType.Blessed;
			Attributes.SpellChanneling = 1;
			Attributes.WeaponDamage = 30;
			WeaponAttributes.MageWeapon = 20;
		}

		public CTFRewardKatana( Serial serial ) : base( serial )
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

	public class CTFRewardWarFork : WarFork
	{
		public CTFRewardWarFork() : base()
		{
			Name = Name + "[CTF-Item]";
			LootType = LootType.Blessed;
			Attributes.SpellChanneling = 1;
			Attributes.WeaponDamage = 30;
			WeaponAttributes.MageWeapon = 20;
		}

		public CTFRewardWarFork( Serial serial ) : base( serial )
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

	public class CTFRewardWarHammer : WarHammer
	{
		public CTFRewardWarHammer() : base()
		{
			Name = Name + "[CTF-Item]";
			LootType = LootType.Blessed;
			Attributes.SpellChanneling = 1;
			Attributes.WeaponDamage = 30;
			WeaponAttributes.MageWeapon = 20;
		}

		public CTFRewardWarHammer( Serial serial ) : base( serial )
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

	public class CTFRewardBracelet : GoldBracelet
	{
		public CTFRewardBracelet() : base()
		{
			Name = Name + "[CTF-Item]";
			LootType = LootType.Blessed;
			Attributes.EnhancePotions = 10;
		}

		public CTFRewardBracelet( Serial serial ) : base( serial )
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

	public class CTFRewardBow : Bow
	{
		public CTFRewardBow() : base()
		{
			Name = Name + "[CTF-Item]";
			LootType = LootType.Blessed;
			Attributes.SpellChanneling = 1;
			Attributes.WeaponDamage = 30;
			WeaponAttributes.MageWeapon = 20;
		}

		public CTFRewardBow( Serial serial ) : base( serial )
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

	public class CTFSpellBook : Spellbook
	{
		public CTFSpellBook() : base( UInt64.MaxValue )
		{
			Name = Name + "[CTF-Item]";
			LootType = LootType.Blessed;
			Attributes.SpellDamage = 10;
		}

		public CTFSpellBook( Serial serial ) : base( serial )
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