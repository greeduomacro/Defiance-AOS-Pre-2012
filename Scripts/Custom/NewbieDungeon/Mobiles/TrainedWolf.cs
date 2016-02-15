using System;
using Server.Mobiles;
using Server.Items;

namespace Server.Mobiles
{
	public class TrainedWolf : BaseCreature
	{
		[Constructable]
		public TrainedWolf() : this( null )
		{
		}

		[Constructable]
		public TrainedWolf( WolfMaster owner ) : base( AIType.AI_Melee,FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a trained wolf";
			Body = 23;
			BaseSoundID = 0xE5;

			SetStr( 96, 120 );
			SetDex( 121, 145 );
			SetInt( 36, 60 );

			SetHits( 38, 47 );
			SetMana( 0 );

			SetDamage( 14, 18 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetSkill( SkillName.MagicResist, 57.6, 75.0 );
			SetSkill( SkillName.Tactics, 50.1, 70.0 );
			SetSkill( SkillName.Wrestling, 45.1, 60.0 );

			Fame = 2500;
			Karma = -2500;
		}

		public TrainedWolf(Serial serial) : base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write((int) 0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();
		}
	}
}