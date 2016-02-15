using System;
using System.Collections;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Network;

namespace Server.Factions
{
	public class FactionBaseHorseVendor : FactionHorseVendor
	{
		[Constructable]
		public FactionBaseHorseVendor() : this( null, null )
		{
		}

		[Constructable]
		public FactionBaseHorseVendor( Town town, Faction faction ) : base( town, faction )
		{
			SetSkill( SkillName.AnimalLore, 64.0, 100.0 );
			SetSkill( SkillName.AnimalTaming, 90.0, 100.0 );
			SetSkill( SkillName.Veterinary, 65.0, 88.0 );
		}

		public override void InitSBInfo()
		{
		}

		public FactionBaseHorseVendor( Serial serial ) : base( serial )
		{
		}

		public override void VendorBuy( Mobile from )
		{
			if ( this.Faction == null || Faction.Find( from, true ) != this.Faction )
				PrivateOverheadMessage( MessageType.Regular, 0x3B2, 1042201, from.NetState ); // You are not in my faction, I cannot sell you a horse!
			else if ( FactionGump.Exists( from ) )
				from.SendLocalizedMessage( 1042160 ); // You already have a faction menu open.
			else if ( from is PlayerMobile )
				from.SendGump( new ExpensiveHorseBreederGump( (PlayerMobile) from, this.Faction ) );
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