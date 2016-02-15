using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	public class EvilOne : BaseCreature
	{
		[Constructable]
		public EvilOne () : base( AIType.AI_NecroMage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = NameList.RandomName( "daemon" );
			Body = 784;
			Hue = 777;

			SetStr( 300, 315 );
			SetDex( 56, 65 );
			SetInt( 826, 850 );

			SetHits( 446, 499 );

			SetDamage( 9, 11 );

			SetDamageType( ResistanceType.Physical, 30 );
			SetDamageType( ResistanceType.Fire, 50 );
			SetDamageType( ResistanceType.Energy, 20 );

			SetResistance( ResistanceType.Physical, 45, 50 );
			SetResistance( ResistanceType.Fire, 45, 50 );
			SetResistance( ResistanceType.Cold, 45, 50 );
			SetResistance( ResistanceType.Poison, 45, 50 );
			SetResistance( ResistanceType.Energy, 45, 50 );

			SetSkill( SkillName.SpiritSpeak, 100.1, 105.0 );
			SetSkill( SkillName.Necromancy, 105.1, 115.0 );
			SetSkill( SkillName.EvalInt, 100.1, 105.0 );
			SetSkill( SkillName.Magery, 105.1, 115.0 );
			SetSkill( SkillName.Meditation, 90.4, 100.0 );
			SetSkill( SkillName.MagicResist, 90.1, 95.0 );
			SetSkill( SkillName.Tactics, 90.1, 100.0 );
			SetSkill( SkillName.Wrestling, 100.1, 110.0 );

			Fame = 12500;
			Karma = -12500;

			VirtualArmor = 60;
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.FilthyRich, 3 );
		}

		public override int TreasureMapLevel{ get{ return 5; } }

		public override void OnBeforeSpawn( Point3D location, Map m )
		{
			base.OnBeforeSpawn( location, m );

			IsParagon = false;
			IsElder = false;
			IsPlagued = false;
			Hue = 777;
		}

		public EvilOne( Serial serial ) : base( serial )
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

			if ( IsParagon )
				IsParagon = false;
		}
	}
}