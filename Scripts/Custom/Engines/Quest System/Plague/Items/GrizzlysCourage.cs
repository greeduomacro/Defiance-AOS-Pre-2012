using System;
using Server;

namespace Server.Items
{
	public class GrizzlysCourage : BearMask
	{
		public override int InitMinHits{ get{ return 255; } }
		public override int InitMaxHits{ get{ return 255; } }

		public override int ArtifactRarity{ get{ return 11; } }

		[Constructable]
		public GrizzlysCourage()
		{
			Name = "grizzly's courage";
			Hue = 0x840;

			Attributes.BonusStr = 20;
			Attributes.NightSight = 1;
			Attributes.AttackChance = 15;
			Resistances.Physical = 10;
			Resistances.Cold = 10;
			Resistances.Fire = 10;
		}

		public GrizzlysCourage( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}