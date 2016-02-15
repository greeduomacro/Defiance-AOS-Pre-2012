using System;
using Server;
using Server.Misc;
using Server.Items;

namespace Server.Mobiles
{
	public class MindlessZombie : BaseCreature
	{
		[Constructable]
		public MindlessZombie() : base( AIType.AI_Stealth, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Body = 400;
			Name = "Zombie";
			Hue = 1150;

			SetStr( 100 );
			SetDex( 100 );
			SetInt( 100 );

			SetHits( 90, 100 );

			SetDamage( 13, 15 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 45, 50 );
			SetResistance( ResistanceType.Fire, 10, 15 );
			SetResistance( ResistanceType.Cold, 45, 50 );
			SetResistance( ResistanceType.Poison, 45, 50 );
			SetResistance( ResistanceType.Energy, 45, 50 );

			SetSkill( SkillName.Wrestling, 100.0 );
			SetSkill( SkillName.Focus, 100.0 );

			Fame = 5000;
			Karma = -6000;

			VirtualArmor = 5;

		}

		public override bool AlwaysMurderer{ get{ return true; } }
		public override bool ShowFameTitle{ get{ return false; } }

		public MindlessZombie( Serial serial ) : base( serial )
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