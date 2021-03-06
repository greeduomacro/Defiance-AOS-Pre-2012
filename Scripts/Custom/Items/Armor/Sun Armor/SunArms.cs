using System;
using Server.Items;

namespace Server.Items
{
	[FlipableAttribute( 0x2657, 0x2658 )]
	public class SunArms : BaseArmor
	{
		public override int BasePhysicalResistance{ get{ return 6; } }
		public override int BaseFireResistance{ get{ return 6; } }
		public override int BaseColdResistance{ get{ return 6; } }
		public override int BasePoisonResistance{ get{ return 6; } }
		public override int BaseEnergyResistance{ get{ return 6; } }

		public override int InitMinHits{ get{ return 30; } }
		public override int InitMaxHits{ get{ return 40; } }

		public override int AosStrReq{ get{ return 20; } }
		public override int OldStrReq{ get{ return 10; } }

		public override int ArmorBase{ get{ return 13; } }

		public override ArmorMaterialType MaterialType{ get{ return ArmorMaterialType.Dragon; } }

		public override ArmorMeditationAllowance DefMedAllowance{ get{ return ArmorMeditationAllowance.All; } }

		public override string DefaultName{ get{ return "Armor of the Sun"; } }

		[Constructable]
		public SunArms() : base( 0x2657 )
		{
			Hue = 2213;
			Weight = 5.0;

			BaseRunicTool.ApplyAttributesTo(this, Utility.RandomMinMax(3, 4), 40, 80);
			Attributes.Luck = 0;

		}

		public override bool OnEquip( Mobile from )
		{

			Item tHelm;
			Item tArmor;
			Item tGorget;
			Item tLegs;
			//Item tSleeves;
			Item tGloves;

			tHelm = from.FindItemOnLayer( Layer.Helm );
			tArmor = from.FindItemOnLayer( Layer.InnerTorso );
			tGorget = from.FindItemOnLayer( Layer.Neck );
			tLegs = from.FindItemOnLayer( Layer.Pants );
			//tSleeves = from.FindItemOnLayer( Layer.Arms );
			tGloves = from.FindItemOnLayer( Layer.Gloves );

			if ( ( tHelm != null ) && ( tLegs != null ) && ( tGorget != null ) && ( tArmor != null ) && ( tGloves != null ) )
			{
				if ( ( tHelm is SunHelm ) && ( tLegs is SunLegs ) && ( tGorget is SunGorget ) && ( tArmor is SunArmor ) && ( tGloves is SunGloves ) )
				{
					((BaseArmor)tLegs).Attributes.Luck = 900;
				}
			}

			return base.OnEquip( from );
		}

		public override void OnRemoved( object parent )
		{
			Item tLegs;

			base.OnRemoved( parent );

			if ( parent is Mobile )
			{
				tLegs = ((Mobile)parent).FindItemOnLayer( Layer.Pants );
				if ( ( tLegs != null ) && ( tLegs is SunLegs ) )
				{
					((BaseArmor)tLegs).Attributes.Luck = 0;
				}
			}
		}

		public SunArms( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.WriteEncodedInt( (int) 0 );
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );
			int version = reader.ReadEncodedInt();

		}
	}
}