using System;
using Server.Items;

namespace Server.Events.CTF
{
	[FlipableAttribute( 0x1f03, 0x1f04 )]
	public class CTFRobe : BaseOuterTorso
	{
		public CTFRobe( CTFTeam team ) : base( 0x1F03, team.Hue )
		{
			Name = "[Event Item]";
			Weight = 0.1;
			Movable = false;
			Attributes.LowerRegCost = 100;
			Attributes.CastRecovery = 4;
			Attributes.CastSpeed = 2;
			Attributes.LowerManaCost = 30;
			Resistances.Physical = 70;
			Resistances.Fire = 65;
			Resistances.Cold = 65;
			Resistances.Poison = 65;
			Resistances.Energy = 65;
			LootType = LootType.Cursed;
		}

		public CTFRobe( Serial serial ) : base( serial )
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