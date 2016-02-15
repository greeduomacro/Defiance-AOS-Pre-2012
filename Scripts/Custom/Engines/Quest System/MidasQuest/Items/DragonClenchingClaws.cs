using System;
using Server;


namespace Server.Items
{
	public class DragonClenchingClaws : DragonGloves
	{
		public override int BasePhysicalResistance{ get{ return 14; } }
		public override int BaseFireResistance{ get{ return 8; } }
		public override int BaseColdResistance{ get{ return 0; } }
		public override int BasePoisonResistance{ get{ return 15; } }
		public override int BaseEnergyResistance{ get{ return 10; } }
		public override int ArtifactRarity{ get{ return 11; } }

		public override int InitMinHits{ get{ return 255; } }
		public override int InitMaxHits{ get{ return 255; } }

		[Constructable]
		public DragonClenchingClaws()
		{
			Name = "Dragon clenching claws";
			Hue = 0x489;
			Attributes.WeaponSpeed = 10;
			Attributes.WeaponDamage = 15;
			SkillBonuses.SetValues( 0, Utility.RandomCombatSkill(), 10.0 );
		}


		public override bool OnEquip( Mobile from )
		{
			from.SendMessage(" Suddenly you feel more swift and thy grip feels stronger then ever " );
			if ((from.Skills[SkillName.Chivalry].Value > 40) & (from.Kills <5))
				from.SendMessage(" A distant whisper in your head echoes: I shall be used wisely on your righteous hands");
			return base.OnEquip( from );
		}

		[Constructable]
		public DragonClenchingClaws( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 );
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}