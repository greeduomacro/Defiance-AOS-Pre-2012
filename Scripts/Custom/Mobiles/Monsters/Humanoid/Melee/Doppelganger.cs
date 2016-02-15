using System;
using System.Collections;
using Server.Items;
using Server.Targeting;

namespace Server.Mobiles
{
	[CorpseName( "a doppelgangers corpse" )]
	public class Doppelganger : BaseCreature
	{
		[Constructable]
		public Doppelganger() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Body = 777;
			Name = "a doppelganger";

			SetStr( 339, 382 );
			SetDex( 100, 115 );
			SetInt( 32, 52 );

			SetSkill( SkillName.Wrestling, 83, 91 );
			SetSkill( SkillName.Tactics, 81, 97 );
			SetSkill( SkillName.MagicResist, 77, 102 );

			SetResistance( ResistanceType.Physical, 50, 60 );
			SetResistance( ResistanceType.Fire, 10, 20 );
			SetResistance( ResistanceType.Cold, 40, 50 );
			SetResistance( ResistanceType.Poison, 50, 60 );
			SetResistance( ResistanceType.Energy, 30, 40 );

			VirtualArmor = 48;
			SetFameLevel( 3 );
			SetKarmaLevel( 3 );

			PackGold( 60, 90 );
		}

		public override bool AlwaysMurderer{ get{ return true; } }
		public override int TreasureMapLevel{ get{ return 3; } }

		public Doppelganger( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}