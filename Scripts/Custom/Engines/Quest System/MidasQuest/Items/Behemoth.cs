using System;
using Server;

namespace Server.Items
{
	public class Behemoth : PlateGorget
	{
		//public override int LabelNumber{ get{ return 1041000; } } // Voice of the Fallen King
		public override int ArtifactRarity{ get{ return 11; } }

		public override int BasePhysicalResistance{ get{ return 1; } }
		public override int BaseFireResistance{ get{ return 7; } }
		public override int BaseColdResistance{ get{ return 12; } }
		public override int BasePoisonResistance{ get{ return 13; } }
		public override int BaseEnergyResistance{ get{ return 15; } }


		public override int InitMinHits{ get{ return 255; } }
		public override int InitMaxHits{ get{ return 255; } }

		[Constructable]
		public Behemoth()
		{
			Name = "Behemoth";
			Hue = 0x44F;
			Attributes.BonusStr = 10;
			ArmorAttributes.MageArmor = 1;
			ArmorAttributes.SelfRepair = 20;
			Attributes.LowerManaCost = 8;
			//ArmorBonus.StaminaIncrease = 10;
		}

		public Behemoth( Serial serial ) : base( serial )
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